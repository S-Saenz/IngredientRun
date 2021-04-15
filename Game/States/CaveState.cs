﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace WillowWoodRefuge
{
    class CaveState : GameplayState
    {
        public CaveState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content, SpriteBatch spriteBatch)
            : base(game, graphicsDevice, content, spriteBatch)
        {
            // Setup Tilemap
            _tileMap = new TileMap("tilemaps/cave/CollisionTestMap", _content, game.GraphicsDevice, _physicsHandler);

            _isDark = true;

            // Setup lights
            _lightManager.Initialize(_tileMap, _content.Load<Texture2D>("dither/dithersheet"), _shadowColor);

            _lightManager.AddLight(new Vector2(224, 608), 100);
            _lightManager.AddLight(new Vector2(656, 240), 100);
            _lightManager.AddLight(new Vector2(240, 208), 100);
            _lightManager.AddLight(new Vector2(0, 35), 300);

            _lightManager.RenderStatic(content);

            PostConstruction();
        }

        public override void LoadContent()
        {
            base.LoadContent();
            //music
            game.sounds.playSong("caveSong");
            
            // Make dark
            _isDark = true;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            game.GraphicsDevice.Clear(Color.Brown);
            base.Draw(gameTime, spriteBatch);
        }
    }
}
