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

        private List<Animation> _tutorialPages = new List<Animation>();

        public string _returnTo;

        Color _bgColor = new Color(203, 170, 142);

        public TutorialState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content, SpriteBatch spritebatch) : base(game, graphicsDevice, content, spritebatch)
        {
            var buttonTexture = _content.Load<Texture2D>("Controls/ButtonNormal");
            var buttonFont = FontManager._dialogueFont;

            var menuButton = new UIButton(buttonTexture, new Vector2((game.GraphicsDevice.Viewport.Width - buttonTexture.Width * 3) / 2, 50), "Return");
            menuButton.reScale(3f);

            menuButton.Click += menuButton_Click;

            _components = new List<UIButton>()
            {
                menuButton,
            };
            _tutorialPages.Add(new Animation(_content.Load<Texture2D>("Tutorial/MovementControls-Sheet"), 1, 12, 50));
            _tutorialPages.Add(new Animation(_content.Load<Texture2D>("Tutorial/Controls"), 1, 12, 50));
            // tutorial = new Animation(tuttex, 1, 12, 50);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            game.GraphicsDevice.Clear(_bgColor);

            spriteBatch.Begin(sortMode: SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp);

            _components[0].Draw(spriteBatch);

            _tutorialPages[0].Draw(spriteBatch, new Vector2(400, game.GraphicsDevice.Viewport.Height / 2 + 20), Game1.instance._cameraController._screenScale);
            _tutorialPages[1].Draw(spriteBatch, new Vector2(game.GraphicsDevice.Viewport.Width - 400, game.GraphicsDevice.Viewport.Height / 2 + 20), Game1.instance._cameraController._screenScale);

            //_tutorialPages[_currPage].Draw(spriteBatch, new Vector2 (game.GraphicsDevice.Viewport.Width / 2, game.GraphicsDevice.Viewport.Height / 2 + 50), 1f);

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
            foreach(Animation obj in _tutorialPages)
                obj.Update(gameTime);
        }

        private void menuButton_Click(object sender, EventArgs e)
        {
            game.ChangeState(_returnTo);
        }
    }
}
