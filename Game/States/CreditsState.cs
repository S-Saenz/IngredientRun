using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace WillowWoodRefuge
{
    class CreditsState : State
        
    {
        private List<Component> _components;

        string credits;
        string tools;
        
        public CreditsState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content, SpriteBatch spritebatch)
            : base(game, graphicsDevice, content, spritebatch)
        {
            var buttonTexture = _content.Load<Texture2D>("Controls/Button");
            var buttonFont = FontManager._dialogueFont;

            var menuButton = new MenuButton(buttonTexture, buttonFont)
            {
                Position = new Vector2(game.GraphicsDevice.Viewport.Width / 2 - 80, 100),
                Text = "To Menu",
            };

            menuButton.Click += menuButton_Click;

            _components = new List<Component>()
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

            spriteBatch.Begin();

            _components[0].Draw(gameTime, spriteBatch);
            
            spriteBatch.DrawString(FontManager._bigdialogueFont, credits, new Vector2(100, 200), Color.Black);
            spriteBatch.DrawString(FontManager._bigdialogueFont, tools, new Vector2(900, 200), Color.Black);
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
            _components[0].Update(gameTime);
        }

        private void menuButton_Click(object sender, EventArgs e)
        {
            game.ChangeState("MenuState");
        }
    }
}
