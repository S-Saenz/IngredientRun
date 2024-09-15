using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Sprites;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace WillowWoodRefuge
{
    class CreditsState : State
        
    {
        private List<UIButton> _components;

        string credits;
        string tools;
        Texture2D creditsImg;

        public CreditsState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content, SpriteBatch spritebatch)
            : base(game, graphicsDevice, content, spritebatch)
        {
            var buttonTexture = _content.Load<Texture2D>("Controls/ButtonNormal");
            var buttonFont = FontManager._dialogueFont;
            creditsImg = _content.Load<Texture2D>("bg/WWRCredits");

            var menuButton = new UIButton(buttonTexture, new Vector2(50, 50), "Return");
            menuButton.reScale(2f);

            menuButton.Click += menuButton_Click;

            _components = new List<UIButton>()
            {
                menuButton
            };

            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("WillowWoodRefuge.Content.dialogue.credits.txt");
            // grow system from file
            using (StreamReader reader = new StreamReader(stream))
            {
                string line;
                while (!reader.EndOfStream)
                {
                    line = reader.ReadLine();
                    credits += line + '\n';
                }
            }

            stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("WillowWoodRefuge.Content.dialogue.credits_tools.txt");
            // grow system from file
            using (StreamReader reader = new StreamReader(stream))
            {
                string line;
                while (!reader.EndOfStream)
                {
                    line = reader.ReadLine();
                    tools += line + '\n';
                }
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            game.GraphicsDevice.Clear(Color.Bisque);

            spriteBatch.Begin(sortMode: SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp);
            spriteBatch.Draw(creditsImg, new Rectangle(0, 0, (int)Game1.instance._cameraController._screenDimensions.X, (int)Game1.instance._cameraController._screenDimensions.Y), Color.White);
            _components[0].Draw(spriteBatch);
            spriteBatch.End();
        }

        public override void LoadContent()
        {
            
        }

        public override void PostUpdate(GameTime gameTime)
        {
            
        }

        public override void unloadState()
        {
            
        }

        public override void Update(GameTime gameTime)
        {
            _components[0].Update(Mouse.GetState());
        }

        private void menuButton_Click(object sender, EventArgs e)
        {
            game.ChangeState("MenuState");
        }
    }
}
