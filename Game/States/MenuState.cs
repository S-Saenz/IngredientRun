using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace WillowWoodRefuge
{
    class MenuState : State
    {
        private List<Component> _components;

        public MenuState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content, SpriteBatch spritebatch)
            : base(game, graphicsDevice, content, spritebatch)
        {
            var buttonTexture = _content.Load<Texture2D>("Controls/Button");
            var buttonFont = FontManager._dialogueFont;

            var newGameButton = new MenuButton(buttonTexture, buttonFont)
            {
                Position = new Vector2(game._cameraController._screenDimensions.X/2 - 80, 200),
                Text = "New Game",
            };

            newGameButton.Click += NewGameButton_Click;

            var loadGameButton = new MenuButton(buttonTexture, buttonFont)
            {
                Position = new Vector2(game._cameraController._screenDimensions.X / 2 - 80, 300),
                Text = "Load Game",
            };

            loadGameButton.Click += LoadGameButton_Click;

            var quitGameButton = new MenuButton(buttonTexture, buttonFont)
            {
                Position = new Vector2(game._cameraController._screenDimensions.X / 2 - 80, 400),
                Text = "Quit Game",
            };

            quitGameButton.Click += QuitGameButton_Click;

            _components = new List<Component>()
            {
                newGameButton,
                loadGameButton,
                quitGameButton,
            };
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            game.GraphicsDevice.Clear(Color.Bisque);
            spriteBatch.Begin();

            foreach (var component in _components)
                component.Draw(gameTime, spriteBatch);

            spriteBatch.End();
        }

        public override void LoadContent()
        {
            
        }

        public override void PostUpdate(GameTime gameTime)
        {
            // remove sprite of not needed
        }

        public override void unloadState()
        {
            
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var component in _components)
                component.Update(gameTime);
        }

        private void LoadGameButton_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Load Game");
        }

        private void NewGameButton_Click(object sender, EventArgs e)
        {
            game.ChangeState("CampState");
        }

        private void QuitGameButton_Click(object sender, EventArgs e)
        {
            game.Exit();
        }
    }
}
