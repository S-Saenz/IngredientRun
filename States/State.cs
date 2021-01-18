using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace IngredientRun.States
{
    public abstract class State
    {
        #region Fields

        protected ContentManager _content;

        protected GraphicsDevice _graphicsDevice;

        protected Game1 game;

        //need tile map variable

        #endregion

        #region Methods
        public State(Game1 game, GraphicsDevice graphicsDevice, ContentManager content)
        {
            this.game = game;

            _graphicsDevice = graphicsDevice;

            _content = content;
        }

        public abstract void Initialize();

        public abstract void LoadContent();


        public abstract void Update(GameTime gameTime);

        public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);

        public abstract void PostUpdate(GameTime gameTime);

        #endregion
    }
}
