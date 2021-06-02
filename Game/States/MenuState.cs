using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace WillowWoodRefuge
{
    class MenuState : State
    {
        private List<UIButton> _components;
        Texture2D menuBackground;

        public MenuState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content, SpriteBatch spritebatch)
            : base(game, graphicsDevice, content, spritebatch)
        {
            var buttonTexture = _content.Load<Texture2D>("Controls/ButtonNormal");
            menuBackground = _content.Load<Texture2D>("bg/TitleScreen");
            var buttonFont = FontManager._dialogueFont;

            var newGameButton = new UIButton(buttonTexture, new Vector2(game.GraphicsDevice.Viewport.Width / 2 - 160, 550), "New Game");
            newGameButton.reScale(2f);
            newGameButton.Click += NewGameButton_Click;

            var creditsButton = new UIButton(buttonTexture, new Vector2(game.GraphicsDevice.Viewport.Width / 2 - 160, 625), "Credits");
            creditsButton.reScale(2f);
            creditsButton.Click += creditsButton_Click;

            var TutorialButton = new UIButton(buttonTexture, new Vector2(game.GraphicsDevice.Viewport.Width / 2, 550), "Tutorial");
            TutorialButton.reScale(2f);
            TutorialButton.Click += TutorialButton_Click;

            var quitGameButton = new UIButton(buttonTexture, new Vector2(game.GraphicsDevice.Viewport.Width / 2, 625), "Quit Game");
            quitGameButton.reScale(2f);
            quitGameButton.Click += QuitGameButton_Click;

            var showcaseTexture = _content.Load<Texture2D>("ui/showcaseCircle");
            var showcaseButton = new UIButton(showcaseTexture, new Vector2(game.GraphicsDevice.Viewport.Width - showcaseTexture.Width * .05f - 10, 10));
            showcaseButton.reScale(.05f);
            showcaseButton.Click += ShowcaseButton_Click;

            _components = new List<UIButton>()
            {
                newGameButton,
                creditsButton,
                TutorialButton,
                quitGameButton,
                showcaseButton,
            };
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            game.GraphicsDevice.Clear(Color.Bisque);
            spriteBatch.Begin(sortMode: SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp);
            spriteBatch.Draw(menuBackground, new Rectangle(0, 0, (int)Game1.instance._cameraController._screenDimensions.X, (int)Game1.instance._cameraController._screenDimensions.Y), Color.White);
            foreach (var component in _components)
                component.Draw(spriteBatch);

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
                component.Update(Mouse.GetState());
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

        private void ShowcaseButton_Click(object sender, EventArgs e)
        {
            game.ChangeState("ArtistStatement");
        }
    }
}
