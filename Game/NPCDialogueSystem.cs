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
    delegate void NPCAction();
    public class Condition
    {
        public string _name;
        public bool _flag;

        public Condition(string name, bool startState)
        {
            _name = name;
            _flag = startState;
        }
    }

    class NPCDialogueSystem
    {
        List<NPCInteraction> _interactions;
        Dictionary<string, List<int>> _conditionBins = new Dictionary<string, List<int>>(); // bins containing index of all events satisfied by given
        Dictionary<int, int> _valid = new Dictionary<int, int>(); // all currently valid interactions and count of validation instances, needs to be updated before searching for interaction
        float _validProbabilityTotal;
        int _currentInteraction;

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

        public void Draw(SpriteFont font, Vector2 loc, GameTime gameTime, SpriteBatch spriteBatch)
        {
            _interactions[_currentInteraction].Draw(font, loc, gameTime, spriteBatch);
        }
    }

    class NPCInteraction // describes one conversation event
    {
        string _name;
        string[] _requirements; // or requirements
        public float _probability { get; private set; }
        string[] _characters;  // change to dictionary of npc references when npc class implemented
        List<DialogueLine> _dialogue;
        int _currentLine = 0;

        public NPCInteraction(string unparsed)
        {
            string[] values = unparsed.Split('\t');

            _name = values[0];

            ParseRequirement(values[1]);

            _probability = float.Parse(values[2]);

            _characters = values[3].Split(',');
            for(int i = 0; i < _characters.Length; ++i)
            {
                _characters[i] = _characters[i].Trim();
            }

            _dialogue = new List<DialogueLine>();
            ParseDialogue(values[4]);
        }

        public bool isSatisfied()
        {
            // foreach(EventCondition cond in _requirements)
            // {
            //     if(cond.isSatisfied())
            //     {
            //         return true;
            //     }
            // }
            return false;
        }

        class EventCondition // and requirements
        {
            List<Condition> _conditions;

            public bool isSatisfied()
            {
                foreach(Condition cond in _conditions)
                {
                    if(!cond._flag)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        private void ParseRequirement(string unparsed)
        {
            if (unparsed.Length == 0)
            {
                return;
            }
            _requirements = unparsed.Substring(1, unparsed.Length - 2).Split("] [");
        }

        private void ParseDialogue(string unparsed)
        {
            string[] values = unparsed.Split(')');
            foreach (string val in values)
            {
                if (val.Length > 1)
                {
                    _dialogue.Add(new DialogueLine(val));
                }
            }
        }

        public string[] GetRequirements()
        {
            return _requirements;
        }

        public void Draw(SpriteFont font, Vector2 loc, GameTime time, SpriteBatch spriteBatch)
        {
            if(_currentLine < _dialogue.Count() && _dialogue[_currentLine].Draw(font, loc, time, spriteBatch))
            {
                _currentLine += 1;
            }
            // Debug.WriteLine(_name);
            // foreach(DialogueLine line in _dialogue)
            // {
            //     line.Draw();
            // }
            // Debug.WriteLine("\n");
        }
    }

    class DialogueLine
    {
        string _character; // change to npc reference when 
        string _speech; // spoken words
        Dictionary<float, string> _actions; // list of actions taken by player, in order of execution. key: time, value: action
        float _speed; // speed at which the dialogue plays
        float _currTime; 

        public DialogueLine(string unparsedLine)
        {
            _currTime = 0;

            // extract character name
            string[] values = unparsedLine.Split(']', '(');
            _character = values[0].Substring(values[0].IndexOf('[') + 1);

            // extract duration
            if (values.Length > 2)
            {
                _speed = float.Parse(values[2]);
            }

            // extract speech/actions
            _actions = new Dictionary<float, string>();

            int start = values[1].IndexOf('\"');
            int end = values[1].LastIndexOf('\"');

            ParseDialogue(values[1].Substring(start + 1, end - start - 1));
        }

        private void ParseDialogue(string unparsed)
        {
            if (unparsed.Length == 0)
            {
                return;
            }

            string[] values = unparsed.Split('*');
            List<float> actionTimes = new List<float>();
            List<string> actions = new List<string>();
            _speech = values[0];
            for(int i = 1; i < values.Length; ++i)
            {
                if (values[i].Length > 0)
                {
                    if (i % 2 == 0) // spoken words
                    {
                        _speech += values[i];
                    }
                    else // action
                    {
                        actionTimes.Add(_speech.Length);
                        actions.Add(values[i]);
                    }
                }
            }

            // calculate times of actions
            for(int i = 0; i < actions.Count; ++i)
            {
                // actionTimes[i] = actionTimes[i] / _speech.Length * _duration;
                _actions.Add(actionTimes[i], actions[i]);
            }
        }

        public void Update(GameTime gameTime)
        {
            _currTime += gameTime.GetElapsedSeconds();
        }

        // returns whether line ended or not
        public bool Draw(SpriteFont font, Vector2 loc, GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(font, _character + ": " + _speech, loc, Color.Black);
            _currTime += gameTime.GetElapsedSeconds();
            if (_currTime > 3)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
