using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

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
        List<EventCondition> _requirements; // or requirements
        float _probability;
        Dictionary<int,string> _characters; // key: representation of character in current interaction, value: string name of character. 
                                            // change to dictionary of npc references when npc class implemented
        List<DialogueLine> _dialogue;

        public NPCInteraction(string unparsed)
        {
            string[] values = unparsed.Split('\t');
            _name = values[0];
        }

        public bool isSatisfied()
        {
            foreach(EventCondition cond in _requirements)
            {
                if(cond.isSatisfied())
                {
                    return true;
                }
            }
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
    }

    class DialogueLine
    {
        int _character; // change to npc reference when 
        string _speech; // spoken words
        List<Tuple<float, NPCAction>> _actions; // list of actions taken by player, in order of execution. first: time, second: action
        float _duration; // number of seconds that dialogue remains on screen
        float _currTime; 

        public DialogueLine(int character, string unparsedLine)
        {
            _currTime = 0;

            // Parse line, extract actions
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
