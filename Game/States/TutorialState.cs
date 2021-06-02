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
    class TutorialState : State
    {
        private List<UIButton> _components;

        private Animation tutorial;

        Texture2D tuttex;

        private string tut;

        public TutorialState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content, SpriteBatch spritebatch) : base(game, graphicsDevice, content, spritebatch)
        {
            var buttonTexture = _content.Load<Texture2D>("Controls/ButtonNormal");
            var buttonFont = FontManager._dialogueFont;

            var menuButton = new UIButton(buttonTexture, new Vector2(game.GraphicsDevice.Viewport.Width / 2 - 80, 100), "To Menu");
            menuButton.reScale(2f);

            menuButton.Click += menuButton_Click;

            _components = new List<UIButton>()
            {
                menuButton
            };
            tuttex = _content.Load<Texture2D>("Controls/tutorial");
            tutorial = new Animation(tuttex, 1, 12, 50);

            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("WillowWoodRefuge.Content.dialogue.Tutorial.txt");
            // grow system from file
            using (StreamReader reader = new StreamReader(stream))
            {
                // throw away first line (headers)
                string line;
                while (!reader.EndOfStream)
                {
                    line = reader.ReadLine();
                    tut += line + '\n';
                }
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            game.GraphicsDevice.Clear(Color.Aqua);

            spriteBatch.Begin(sortMode: SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp);

            _components[0].Draw(spriteBatch);

            tutorial.Draw(spriteBatch, new Vector2 (game.GraphicsDevice.Viewport.Width/2 + 500, game.GraphicsDevice.Viewport.Height / 2), 2f);

            spriteBatch.DrawString(FontManager._bigdialogueFont, tut, new Vector2(20, 70), Color.Black);
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
            tutorial.Update(gameTime);
        }

        private void menuButton_Click(object sender, EventArgs e)
        {
            game.ChangeState("MenuState");
        }
    }
}
