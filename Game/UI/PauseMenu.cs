using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace WillowWoodRefuge
{
    public class PauseMenu
    {
        //exit button
        UIButton _toGame;
        UIButton _toTutorial;
        UIButton _toMainMenu;
        float _buttonScale = 3;

        public PauseMenu()
        {
        }

        public void Load(ContentManager contentManager)
        {
            _toGame = new UIButton("ButtonNormal", Game1.instance._cameraController._screenDimensions / 2 - new Vector2(0, 120), "Return to Game", true);
            _toGame.reScale(_buttonScale);
            _toTutorial = new UIButton("ButtonNormal", Game1.instance._cameraController._screenDimensions / 2 + new Vector2(0, 0), "Controls", true);
            _toTutorial.reScale(_buttonScale);
            _toMainMenu = new UIButton("ButtonNormal", Game1.instance._cameraController._screenDimensions / 2 + new Vector2(0, 120), "Exit to Main Menu", true);
            _toMainMenu.reScale(_buttonScale);

            _toGame.Click += ReturnToGame;
            _toTutorial.Click += GoToTutorial;
            _toMainMenu.Click += ExitToMain;
        }

        public void Update(MouseState mouseState, KeyboardState keyState, GameTime gameTime)
        {
            GameplayState gameState = Game1.instance._currentState as GameplayState;
            if (gameState != null)
                gameState._player.DisableInput();

            _toGame.Update(mouseState);
            _toTutorial.Update(mouseState);
            _toMainMenu.Update(mouseState);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            TextureAtlasManager.DrawTexture(spriteBatch, "UI", "Background_Opacity", new Rectangle(0, 0, (int)Game1.instance._cameraController._screenDimensions.X, (int)Game1.instance._cameraController._screenDimensions.Y), Color.White);

            _toGame.Draw(spriteBatch);
            _toTutorial.Draw(spriteBatch);
            _toMainMenu.Draw(spriteBatch);
        }

        private void ReturnToGame(object sender, EventArgs e)
        {
            Game1.instance.UI.SwitchState(UIState.None);
        }

        private void GoToTutorial(object sender, EventArgs e)
        {
            Game1.instance.RequestStateChange("TutorialState");
        }

        private void ExitToMain(object sender, EventArgs e)
        {
            Game1.instance.RequestStateChange("MenuState");
        }
    }
}
