using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace WillowWoodRefuge
{
    class TutorialState : State
    {
        private List<Component> _components;

        private Animation tutorial;

        Texture2D tuttex;

        public TutorialState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content, SpriteBatch spritebatch) : base(game, graphicsDevice, content, spritebatch)
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
            tuttex = _content.Load<Texture2D>("Controls/tutorial");
            tutorial = new Animation(tuttex, 1, 12, 50);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            game.GraphicsDevice.Clear(Color.Aqua);

            spriteBatch.Begin(sortMode: SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp);

            _components[0].Draw(gameTime, spriteBatch);

            tutorial.Draw(spriteBatch, new Vector2 (game.GraphicsDevice.Viewport.Width/2 , 500), 1.5f);
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
            tutorial.Update(gameTime);
        }

        private void menuButton_Click(object sender, EventArgs e)
        {
            game.ChangeState("MenuState");
        }
    }
}
