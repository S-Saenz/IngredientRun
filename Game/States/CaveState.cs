using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended;

namespace WillowWoodRefuge
{
    class CaveState : GameplayState
    {
        public CaveState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content, SpriteBatch spriteBatch)
            : base(game, graphicsDevice, content, spriteBatch)
        {
            _cameraSize = new Vector2(480, 270);
            _playerCamBounds = new RectangleF(0, 0, 160f, 90f);
        }

        protected override void LoadTilemap(ContentManager content)
        {
            // Setup Tilemap
            _tileMap = new TileMap("tilemaps/cave/CaveMapForaging", _content, game.GraphicsDevice, _physicsHandler, "cave");

            _isDark = true;

            // Setup lights
            _staticLightManager.AddLight(new Vector2(224, 608), 100);
            _staticLightManager.AddLight(new Vector2(656, 240), 100);
            _staticLightManager.AddLight(new Vector2(240, 208), 100);
            _staticLightManager.AddLight(new Vector2(0, 35), 300);
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
