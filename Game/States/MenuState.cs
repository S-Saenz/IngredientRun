using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace WillowWoodRefuge
{
    class MenuState : State
    {
        private List<Component> _components;
        Texture2D menuBackground;

        public MenuState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content, SpriteBatch spritebatch)
            : base(game, graphicsDevice, content, spritebatch)
        {
            var buttonTexture = _content.Load<Texture2D>("Controls/ButtonNormal");
            menuBackground = _content.Load<Texture2D>("bg/TitleScreen");
            var buttonFont = FontManager._dialogueFont;

            var newGameButton = new MenuButton(buttonTexture, buttonFont)
            {
                Position = new Vector2(game.GraphicsDevice.Viewport.Width / 2 - 80, 200),
                Text = "New Game",
            };

            newGameButton.Click += NewGameButton_Click;

            var creditsButton = new MenuButton(buttonTexture, buttonFont)
            {
                Position = new Vector2(game.GraphicsDevice.Viewport.Width / 2 - 80, 300),
                Text = "Credits",
            };

            creditsButton.Click += creditsButton_Click;

            var TutorialButton = new MenuButton(buttonTexture, buttonFont)
            {
                Position = new Vector2(game.GraphicsDevice.Viewport.Width / 2 - 80, 400),
                Text = "Tutorial",
            };

            TutorialButton.Click += TutorialButton_Click;

            var quitGameButton = new MenuButton(buttonTexture, buttonFont)
            {
                Position = new Vector2(game.GraphicsDevice.Viewport.Width / 2 - 80, 500),
                Text = "Quit Game",
            };

            quitGameButton.Click += QuitGameButton_Click;

            

            _components = new List<Component>()
            {
                newGameButton,
                creditsButton,
                TutorialButton,
                quitGameButton,
            };
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            game.GraphicsDevice.Clear(Color.Bisque);
            spriteBatch.Begin(sortMode: SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp);
            spriteBatch.Draw(menuBackground, new Rectangle(0, 0, (int)Game1.instance._cameraController._screenDimensions.X, (int)Game1.instance._cameraController._screenDimensions.Y), Color.White);
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

        private void creditsButton_Click(object sender, EventArgs e)
        {
            game.ChangeState("CreditsState");
        }

        private void NewGameButton_Click(object sender, EventArgs e)
        {
            game.ChangeState("LoadingState");
        }

        private void TutorialButton_Click(object sender, EventArgs e)
        {
            game.ChangeState("TutorialState");
        }

        private void QuitGameButton_Click(object sender, EventArgs e)
        {
            game.Exit();
        }
    }
}
