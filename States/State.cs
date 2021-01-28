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

        // protected GraphicsDevice _graphicsDevice;

        protected Game1 game;

        protected SpriteBatch _spriteBatch;

        //need tile map variable

        #endregion

        #region Methods
        public State(Game1 game, GraphicsDevice graphicsDevice, ContentManager content, SpriteBatch spritebatch)
        {
            this.game = game;

            _spriteBatch = spritebatch;
            // _graphicsDevice = graphicsDevice;

            _content = content;
        }

        public abstract void LoadContent();


        public abstract void Update(GameTime gameTime);

        public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);

        public abstract void PostUpdate(GameTime gameTime);

        public abstract void unloadState();

        #endregion
    }
}
