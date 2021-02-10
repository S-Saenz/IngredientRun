using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace IngredientRun
{
    class NPCDialogueSystem
    {
        List<NPCInteraction> _interactions;
        Dictionary<string, List<int>> _conditionBins = new Dictionary<string, List<int>>(); // bins containing index of all events satisfied by given
        Dictionary<int, int> _valid = new Dictionary<int, int>(); // all currently valid interactions and count of validation instances, needs to be updated before searching for interaction
        float _validProbabilityTotal;
        int _currentInteraction;
        Dictionary<string, NPC> _characters;

        public NPCDialogueSystem(string filePath, Game1 game)
        {
            // initialize list
            _interactions = new List<NPCInteraction>();

            // grow system from file
            using (StreamReader reader = new StreamReader(filePath))
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

            foreach(Condition cond in game._stateConditions)
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
            foreach(Condition cond in game._stateConditions)
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
            _currentInteraction = -1;
        }

        public void Draw(OrthographicCamera camera, GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (_currentInteraction != -1)
            {
                Dictionary<string, NPC> characters = new Dictionary<string, NPC>();
                foreach(string name in _interactions[_currentInteraction]._characters)
                {
                    characters.Add(name, _characters[name]);
                }
                _interactions[_currentInteraction].Draw(camera, gameTime, spriteBatch, characters);
            }
        }
    }
}
