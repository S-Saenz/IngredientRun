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

            // Setup lights
            // _staticLightManager.AddLight(new Vector2(64, 256), 50);
            // _staticLightManager.AddLight(new Vector2(160, 256), 50);
            // _staticLightManager.AddLight(new Vector2(368, 256), 50);
            // _staticLightManager.AddLight(new Vector2(488, 256), 50);
            _dynamicLightManager.AddLight(new Vector2(0, 0), 32);
            _dynamicLightManager.AddLight(new Vector2(336, 239), 350, new Vector2(0, 1), .75f * (float)MathHelper.Pi);

            _lightEffect.Parameters["TextureDimensions"].SetValue(new Vector2(_tileMap._mapBounds.Width, _tileMap._mapBounds.Height));
            _ditherEffect.Parameters["TextureDimensions"].SetValue(new Vector2(_tileMap._mapBounds.Width, _tileMap._mapBounds.Height));
            PostConstruction();

            _isDark = true;
        }

        public override void LoadContent()
        {
            base.LoadContent();
            //music
            game.sounds.playSong("caveSong");
            
            // Make dark
            // _player._isDark = true;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            game.GraphicsDevice.Clear(Color.Brown);
            base.Draw(gameTime, spriteBatch);
        }
    }
}
