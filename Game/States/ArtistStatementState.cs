using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace WillowWoodRefuge
{
    class ArtistStatementState : State
    {
        private List<UIButton> _components;
        private string _statement =  "";
        private string _customLibs = "";
        private string _externLibs = "";

        private Vector2 _statePos = new Vector2(1728 / 2, 350);
        private Vector2 _customPos = new Vector2(100, 550);
        private Vector2 _externPos = new Vector2(1000, 550);

        Color _bgColor = new Color(203, 170, 142);

        public ArtistStatementState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content, SpriteBatch spritebatch) 
            : base(game, graphicsDevice, content, spritebatch)
        {
            var buttonTexture = _content.Load<Texture2D>("Controls/ButtonNormal");
            var menuButton = new UIButton(buttonTexture, 
                 new Vector2(Game1.instance._cameraController._screenDimensions.X / 2 - buttonTexture.Width, 75), "Return");
            menuButton.reScale(2f);

            menuButton.Click += menuButton_Click;

            _components = new List<UIButton>()
            {
                menuButton
            };

            // read statement
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("WillowWoodRefuge.Content.dialogue.artistStatement.txt");
            using (StreamReader reader = new StreamReader(stream))
            {
                string line;
                while (!reader.EndOfStream)
                {
                    line = reader.ReadLine();
                    _statement += line + '\n';
                }
            }

            // read custom libraries
            stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("WillowWoodRefuge.Content.dialogue.customLibraries.txt");
            using (StreamReader reader = new StreamReader(stream))
            {
                // throw away first line (headers)
                string line;
                while (!reader.EndOfStream)
                {
                    line = reader.ReadLine();
                    _customLibs += line + '\n';
                }
            }

            // read external libraries
            stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("WillowWoodRefuge.Content.dialogue.externalLibraries.txt");
            using (StreamReader reader = new StreamReader(stream))
            {
                // throw away first line (headers)
                string line;
                while (!reader.EndOfStream)
                {
                    line = reader.ReadLine();
                    _externLibs += line + '\n';
                }
            }
        }
        public override void LoadContent()
        {
        }

        public override void Update(GameTime gameTime)
        {
            _components[0].Update(Mouse.GetState());
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            game.GraphicsDevice.Clear(_bgColor);

            spriteBatch.Begin(sortMode: SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp);

            _components[0].Draw(spriteBatch);
            FontManager.PrintText(FontManager._bigdialogueFont, spriteBatch, _statement, _statePos * Game1.instance._cameraController._screenScale,
                                  Alignment.Centered, Color.Black, true);
            FontManager.PrintText(FontManager._bigdialogueFont, spriteBatch, _customLibs, _customPos * Game1.instance._cameraController._screenScale,
                                  Alignment.Left, Color.Black, false);
            FontManager.PrintText(FontManager._bigdialogueFont, spriteBatch, _externLibs, _externPos * Game1.instance._cameraController._screenScale,
                                  Alignment.Left, Color.Black, false);
            // spriteBatch.DrawString(FontManager._bigdialogueFont, _text, new Vector2(20, 70), Color.Black);

            spriteBatch.End();
        }

        public override void PostUpdate(GameTime gameTime)
        {
        }

        public override void unloadState()
        {
        }

        private void menuButton_Click(object sender, EventArgs e)
        {
            game.ChangeState("MenuState");
        }
    }
}
