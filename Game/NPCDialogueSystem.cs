using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace IngredientRun
{
    delegate void NPCAction();
    class Condition
    {
        public bool _flag;
    }

    class NPCDialogueSystem
    {
        List<NPCInteraction> _interactions;

        public NPCDialogueSystem(string filePath)
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
        }
    }

    class NPCInteraction // describes one conversation event
    {
        string _name;
        string[] _requirements; // or requirements
        float _probability;
        string[] _characters;  // change to dictionary of npc references when npc class implemented
        List<DialogueLine> _dialogue;

        public NPCInteraction(string unparsed)
        {
            string[] values = unparsed.Split('\t');

            _name = values[0];

            ParseRequirement(values[1]);

            _probability = float.Parse(values[2]);

            _characters = values[3].Split(',');

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
    }

    class DialogueLine
    {
        string _character; // change to npc reference when 
        string _speech; // spoken words
        Dictionary<float, string> _actions; // list of actions taken by player, in order of execution. key: time, value: action
        float _duration; // number of seconds that dialogue remains on screen
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
                _duration = float.Parse(values[2].Substring(0, values[2].Length - 3));
            }

            // extract speech/actions
            _actions = new Dictionary<float, string>();
            ParseDialogue(values[1].Substring(values[1].IndexOf('\"') + 1, values[1].LastIndexOf('\"') - 2));
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
                actionTimes[i] = actionTimes[i] / _speech.Length * _duration;
                _actions.Add(actionTimes[i], actions[i]);
            }
        }

        public void Update(GameTime gameTime)
        {
            _currTime += gameTime.GetElapsedSeconds();
        }

        public void Draw(RectangleF loc)
        {
            Debug.WriteLine(_speech);
        }
    }
}
