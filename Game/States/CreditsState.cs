using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace WillowWoodRefuge
{
    class CreditsState : State
    {
        public CreditsState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content, SpriteBatch spritebatch)
            : base(game, graphicsDevice, content, spritebatch)
        {
            var buttonTexture = _content.Load<Texture2D>("Controls/Button");
            var buttonFont = FontManager._dialogueFont;

            var menuButton = new MenuButton(buttonTexture, buttonFont)
            {
                Position = new Vector2(game.GraphicsDevice.Viewport.Width / 2 - 80, 0),
                Text = "New Game",
            };

            menuButton.Click += menuButton_Click;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            throw new NotImplementedException();
        }

        public override void LoadContent()
        {
            throw new NotImplementedException();
        }

        public override void PostUpdate(GameTime gameTime)
        {
            throw new NotImplementedException();
        }

        public override void unloadState()
        {
            throw new NotImplementedException();
        }

        public override void Update(GameTime gameTime)
        {
            throw new NotImplementedException();
        }

        private void menuButton_Click(object sender, EventArgs e)
        {
            game.ChangeState("menuState");
        }
    }
}
