using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Linq;
using System.Text;

namespace WillowWoodRefuge
{
    class NPCDialogueSystem
    {
        List<NPCInteraction> _interactions;
        Dictionary<string, List<int>> _conditionBins = new Dictionary<string, List<int>>(); // bins containing index of all events satisfied by given
        Dictionary<int, int> _valid = new Dictionary<int, int>(); // all currently valid interactions and count of validation instances, needs to be updated before searching for interaction
        float _validProbabilityTotal;
        int _currentInteraction;
        bool _talking = false;
        Dictionary<string, NPC> _characters;

        // timer variables
        float _timer;
        float _currTime;
        bool _isPaused;
        Vector2 _silenceRange = new Vector2(2, 6);

        public NPCDialogueSystem(Game1 game)
        {
            // initialize list
            _interactions = new List<NPCInteraction>();

            // Set up stream
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("WillowWoodRefuge.Content.dialogue.NPCDialogue.tsv");

            // grow system from file
            using (StreamReader reader = new StreamReader(stream))
            {
                // throw away first line (headers)
                string line = reader.ReadLine();
                while (!reader.EndOfStream)
                {
                    line = reader.ReadLine();
                    _interactions.Add(new NPCInteraction(line));
                }
            }

            // Populate condition bins
            _conditionBins.Add("noRequirements", new List<int>());

            foreach(StateConditions.Condition cond in game.stateConditions.conditionList)
            {
                _conditionBins.Add(cond._name, new List<int>());
                _conditionBins.Add('!' + cond._name, new List<int>());
            }

            for(int i = 0; i < _interactions.Count; ++i)
            {
                string[] requirements = _interactions[i].GetRequirements();
                if (requirements != null)
                {
                    foreach (string condition in requirements)
                    {
                        if (_conditionBins.ContainsKey(condition))
                        {
                            _conditionBins[condition].Add(i);
                        }
                        else
                        {
                            _conditionBins.Add(condition, new List<int>());
                            _conditionBins[condition].Add(i);
                        }
                    }
                }
                else
                {
                    _conditionBins["noRequirements"].Add(i);
                }
            }

            RecalculateValid(game);
        }

        public void Load(Dictionary<string, NPC> characters)
        {
            _characters = characters;
        }

        private void RecalculateValid(Game1 game)
        {
            // repopulate _valid container
            _valid.Clear();
            foreach(StateConditions.Condition cond in game.stateConditions.conditionList)
            {
                if(cond._flag == true)
                {
                    if (_conditionBins.ContainsKey(cond._name))
                    {
                        foreach (int npcEvent in _conditionBins[cond._name])
                        {
                            if (_valid.ContainsKey(npcEvent))
                            {
                                _valid[npcEvent] += 1;
                            }
                            else
                            {
                                _valid.Add(npcEvent, 1);
                            }
                        }
                    }
                }
                else
                {
                    if (_conditionBins.ContainsKey('!' + cond._name))
                    {
                        foreach (int npcEvent in _conditionBins['!' + cond._name])
                        {
                            if (_valid.ContainsKey(npcEvent))
                            {
                                _valid[npcEvent] += 1;
                            }
                            else
                            {
                                _valid.Add(npcEvent, 1);
                            }
                        }
                    }
                }
            }

            // add events without requirements
            foreach(int npcEvent in _conditionBins["noRequirements"])
            {
                if (_valid.ContainsKey(npcEvent))
                {
                    _valid[npcEvent] += 1;
                }
                else
                {
                    _valid.Add(npcEvent, 1);
                }
            }

            // update valid probability total
            _validProbabilityTotal = 0;
            foreach (int npcEvent in _valid.Keys)
            {
                _validProbabilityTotal += _interactions[npcEvent]._probability;
            }
        }

        public void PlayInteraction(Game1 game)
        {
            RecalculateValid(game);
            float val = new Random(System.DateTime.Now.Second).Next() % _validProbabilityTotal;
            float total = 0;
            int[] elem = _valid.Keys.ToArray();
            int chosen = -1; // index in _interactions
            for(int i = 0; i < _valid.Count && chosen == -1; ++i)
            {
                total += _interactions[elem[i]]._probability;
                if(total > val)
                {
                    chosen = elem[i];
                }
            }

            if(chosen != -1)
            {
                _currentInteraction = chosen;
                _interactions[chosen].Start(_characters);
                _interactions[chosen].CallCall();
                _interactions[chosen].AddEndListener(onInteractionEnded);
                _interactions[chosen].AddStartListener(onStartInteraction);
                _talking = false;

                // remove from bins, now invalid
                string[] requirements = _interactions[chosen].GetRequirements();
                if (requirements != null) // if has requirements
                {
                    foreach (string condition in requirements)
                    {
                        _conditionBins[condition].Remove(chosen);
                    }
                }
                else // if no requirements
                {
                    _conditionBins["noRequirements"].Remove(chosen);
                }
            }
            else
            {
                // no chosen to play
            }
        }

        public void EndInteraction()
        {
            if(_currentInteraction != -1)
                _interactions[_currentInteraction].CallEnd();
        }

        public void onInteractionEnded(NPCInteraction interaction)
        {
            _currentInteraction = -1;
            _isPaused = false;

            // free characters
            foreach (string characterName in interaction._characters)
            {
                _characters[characterName].StopConverse();
            }
        }

        public void onStartInteraction(NPCInteraction interaction)
        {
            _talking = true;
        }

        public void Update(GameTime gameTime)
        {
            if(!_isPaused)
            {
                _currTime += gameTime.GetElapsedSeconds();
            }

            if(_currTime >= _timer)
            {
                PlayInteraction(Game1.instance);
                _isPaused = true;
                _currTime = 0;
                _timer = new Random().Next((int)_silenceRange.X, (int)_silenceRange.Y);
            }

            if (_currentInteraction != -1 && _talking)
            {
                _interactions[_currentInteraction].Update(gameTime);
            }
        }

        public void Draw(OrthographicCamera camera, SpriteBatch spriteBatch)
        {
            if (_currentInteraction != -1 && _talking)
            {
                Dictionary<string, NPC> characters = new Dictionary<string, NPC>();
                foreach(string name in _interactions[_currentInteraction]._characters)
                {
                    characters.Add(name, _characters[name]);
                }
                _interactions[_currentInteraction].Draw(camera, spriteBatch, characters);
            }
        }
    }
}
