using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System.Collections.Generic;
using System.Linq;

namespace WillowWoodRefuge
{
    class NPCInteraction // describes one conversation event
    {
        string _name;
        string[] _requirements; // or requirements
        public float _probability { get; private set; }
        public string[] _characters { private set; get; }  // change to dictionary of npc references when npc class implemented
        List<DialogueLine> _dialogue;
        int _currentLine = 0;

        public NPCInteraction(string unparsed)
        {
            string[] values = unparsed.Split('\t');

            _name = values[0];

            ParseRequirement(values[1]);

            _probability = float.Parse(values[2]);

            _characters = values[3].Split(',');
            for (int i = 0; i < _characters.Length; ++i)
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
            List<StateConditions.Condition> _conditions;

            public bool isSatisfied()
            {
                foreach (StateConditions.Condition cond in _conditions)
                {
                    if (!cond._flag)
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

        public void Draw(OrthographicCamera camera, GameTime time, SpriteBatch spriteBatch, Dictionary<string, NPC> characters)
        {
            if (_currentLine < _dialogue.Count() && _dialogue[_currentLine].Draw(camera, time, spriteBatch, characters))
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
}
