using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Text;

namespace IngredientRun
{
    class DialogueLine
    {
        string _character; // change to npc reference when 
        string _speech; // spoken words
        Dictionary<float, string> _actions; // list of actions taken by player, in order of execution. key: time, value: action
        float _speed; // speed at which the dialogue plays
        float _currTime = 0;

        static Dictionary<int, float> _typeSpeed = new Dictionary<int, float>()
        {
            { 1, 0.2f   },
            { 2, 0.175f },
            { 3, 0.15f  },
            { 4, 0.125f },
            { 5, 0.1f   }
        };

        public DialogueLine(string unparsedLine)
        {
            // extract character name
            string[] values = unparsedLine.Split(']', '(');
            _character = values[0].Substring(values[0].IndexOf('[') + 1);

            // extract duration
            if (values.Length > 2)
            {
                _speed = _typeSpeed[int.Parse(values[2])];
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
            for (int i = 1; i < values.Length; ++i)
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
            for (int i = 0; i < actions.Count; ++i)
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
            string speech = _speech.Substring(0, (int)Math.Clamp(MathF.Floor(_currTime / _speed), 0, _speech.Length));
            spriteBatch.DrawString(font, _character + ": " + speech, loc, Color.Black);
            _currTime += gameTime.GetElapsedSeconds();
            if ((int)MathF.Floor(_currTime / _speed) > _speech.Length + 10)
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
