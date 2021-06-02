using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Text;

namespace WillowWoodRefuge
{
    class DialogueLine
    {
        string _character; // change to npc reference when 
        string _speech; // spoken words
        Dictionary<float, string> _actions; // list of actions taken by player, in order of execution. key: time, value: action
        float _speed; // speed at which the dialogue plays
        float _currTime = 0;
        static float _maxLineWidth = 350;
        Size2 _textSize;

        static Dictionary<int, float> _typeSpeed = new Dictionary<int, float>()
        {
            //Kall
            { 1, 0.1f   },
            { 2, 0.125f },
            { 3, 0.1f  },
            { 4, 0.1f },
            { 5, 0.075f   }
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
            _textSize = FontManager._dialogueFont.MeasureString(_speech);
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

            // add line breaks
            _speech = FontManager.AddLineBreaks(_speech, FontManager._dialogueFont, _maxLineWidth);
            // values = _speech.Split(" ");
            // _speech = "";
            // float currWidth = 0;
            // foreach(string word in values)
            // {
            //     if(word.Length > 0)
            //     {
            //         float wordWidth = FontManager._dialogueFont.MeasureString(" " + word).X;
            //         if(currWidth + wordWidth > _maxLineWidth) // start new line
            //         {
            //             currWidth = -1;
            //             _speech += "\n";
            //         }
            //         else // continue current line
            //         {
            //             _speech += " ";
            //         }
            //         currWidth += wordWidth;
            //         _speech += word;
            //     }
            // }

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
        public bool Draw(OrthographicCamera camera, SpriteBatch spriteBatch, Dictionary<string, NPC> characters)
        {
            string speech = _speech.Substring(0, (int)Math.Clamp(MathF.Floor(_currTime / _speed), 0, _speech.Length)).TrimStart();
            Vector2 size = TextureAtlasManager.GetSize("UI", "Container and Stem");
            float scale = 0.75f * Game1.instance._cameraController._screenScale;
            Vector2 loc = characters[_character].GetDialogueLoc(camera) - new Vector2((100 - size.X / 2) * scale, (size.Y / 2) * scale);
            Vector2 nameBoxLoc = loc - new Vector2(0, (size.Y - 10) / 2 * scale);
            // Vector2 size = FontManager._dialogueFont.MeasureString(_character + "\n" + _speech);
            // loc.Y -= size.Y;
            // spriteBatch.FillRectangle(new RectangleF(loc.X, loc.Y, size.X, size.Y), Color.Bisque);
            TextureAtlasManager.DrawTexture(spriteBatch, "UI", "Container and Stem", loc, Color.White, new Vector2(scale), centered: true);
            TextureAtlasManager.DrawTexture(spriteBatch, "UI", "Name Box", nameBoxLoc, Color.White, new Vector2(scale), centered: true);
            FontManager.PrintText(FontManager._dialogueFont, spriteBatch, speech, loc - (Vector2)_textSize / 2 - new Vector2(10, 25), Alignment.Left, Color.White, false);
            FontManager.PrintText(FontManager._dialogueFont, spriteBatch, _character, nameBoxLoc, Alignment.Centered, Color.White, true);
            // spriteBatch.DrawString(FontManager._dialogueFont, _character + "\n" + speech, loc, Color.Black);
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
