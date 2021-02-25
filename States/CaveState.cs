using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
using MonoGame.Extended;

using Microsoft.Xna.Framework.Input;

namespace IngredientRun.States
{
    class CaveState : State
    {
        Player player;

        TileMap caveTileMap;

        Vector2 bgPos;

        // Debug mode
        bool _isDebug = false;
        bool _ctrlPrevDown = false;

        // private GraphicsDeviceManager _graphics;

        // Temp navmesh for test
        NavPointMap _navMap;
        

        private PhysicsHandler _collisionHandler;
        int walkTimer;
        

        public CaveState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content, SpriteBatch spriteBatch)
            : base(game, graphicsDevice, content, spriteBatch)
        {
            content.RootDirectory = "Content";

            // setup collision
            _collisionHandler = new PhysicsHandler();
            _collisionHandler.AddLayer("Player");
            _collisionHandler.AddLayer("Enemy");
            _collisionHandler.AddLayer("Pickup");
            _collisionHandler.AddLayer("Walls");
            _collisionHandler.AddLayer("Areas");

            _collisionHandler.SetCollision("Player", "Walls");
            _collisionHandler.SetCollision("Enemy", "Walls");
            _collisionHandler.SetOverlap("Player", "Pickup");
            _collisionHandler.SetOverlap("Enemy", "Player");
            _collisionHandler.SetOverlap("Player", "Areas");

            //backgrounds
            caveTileMap = new TileMap("tilemaps/cave/CollisionTestMap", _content, game.GraphicsDevice, _collisionHandler);

            // nav mesh test
            _navMap = caveTileMap.GenerateNavPointMap(new RectangleF(0, 0, 64, 48));
        }

        public override void LoadContent()
        {
            game.sounds.playSong("caveSong");
            // temp, just respawns objects when entering cave
            caveTileMap.SpawnPickups();
            caveTileMap.SpawnEnemies();

            // player
            player = new Player(game.graphics, caveTileMap.GetWaypoint("PlayerObjects", "PlayerSpawn"), _collisionHandler);
            player.Load(_content, _collisionHandler, caveTileMap._mapBounds);
            player._isDark = true;

            // setup camera
            game._cameraController.SetWorldBounds(caveTileMap._mapBounds);
        }

        public override void Update(GameTime gameTime)
        {
            //Debug.WriteLine();
            // Print collision boxes, remove FOWT sprite
            if (Keyboard.GetState().IsKeyDown(Keys.LeftControl) && !_ctrlPrevDown)
            {
                _isDebug = !_isDebug;
                _ctrlPrevDown = true;
            }
            else if (!Keyboard.GetState().IsKeyDown(Keys.LeftControl))
            {
                _ctrlPrevDown = false;
            }

            //play walking sound effect
            if (player._isWalking)
            {
                game.sounds.walkSound(gameTime);
            }
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                game.Exit();

            Matrix projectionMatrix = Matrix.CreateOrthographicOffCenter(0, game._cameraController._screenDimensions.X, game._cameraController._screenDimensions.Y, 0, 1, 0);
            bgPos = player.Update(Mouse.GetState(), Keyboard.GetState(), game._cameraController._camera, gameTime) - game._cameraController._screenDimensions / 2;
            game._cameraController.Update(gameTime, player._pos);
            game.inventory.Update(Mouse.GetState(), Keyboard.GetState());

            caveTileMap.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            game.GraphicsDevice.Clear(Color.Brown);

            Matrix projectionMatrix = Matrix.CreateOrthographicOffCenter(0, game._cameraController._screenDimensions.X, game._cameraController._screenDimensions.Y, 0, 1, 0);

            // Draw tilemap background/walls
            // caveTileMap.Draw(_spriteBatch, game._camera.GetViewMatrix(), projectionMatrix, _isDebug);
            spriteBatch.Begin(sortMode: SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp);
            caveTileMap.DrawLayer(spriteBatch, game._cameraController.GetViewMatrix(), projectionMatrix, "Background");
            caveTileMap.DrawLayer(spriteBatch, game._cameraController.GetViewMatrix(), projectionMatrix, "Walls", _isDebug);
            if (_isDebug)
            {
                caveTileMap.DrawDebug(spriteBatch, game._cameraController.GetViewMatrix(), projectionMatrix);
            }
            spriteBatch.End();

            // Draw sprites
            _spriteBatch.Begin(transformMatrix: game._cameraController.GetViewMatrix(), sortMode: SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp);
            caveTileMap.DrawPickups(spriteBatch, _isDebug);
            caveTileMap.DrawEnemies(spriteBatch, _isDebug);
            player.Draw(_spriteBatch, _isDebug);
            _navMap.Draw(spriteBatch, _isDebug);
            _spriteBatch.End();

            // Draw tilemap foreground
            spriteBatch.Begin(sortMode: SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp);
            caveTileMap.DrawLayer(spriteBatch, game._cameraController.GetViewMatrix(), projectionMatrix, "Foreground");
            spriteBatch.End();

            // Draw UI
            _spriteBatch.Begin(sortMode: SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp);
            if (game.inventory.showInv)
                game.inventory.Draw(_spriteBatch);
            
            _spriteBatch.End();

            // Draw camera debug
            if (_isDebug)
            {
                game._cameraController.Draw(spriteBatch);
            }

            // base.Draw(gameTime);
        }

        

        public override void PostUpdate(GameTime gameTime)
        {
            // throw new NotImplementedException();
        }

        public override void unloadState()
        {
            player.RemoveCollision(_collisionHandler);
        }
    }
}
