﻿using Microsoft.Xna.Framework;
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

        TileMap caveMapBackground;

        Vector2 bgPos;

        // Debug mode
        bool _isDebug = false;
        bool _ctrlPrevDown = false;

        // private GraphicsDeviceManager _graphics;
        

        private PhysicsHandler _collisionHandler;

        

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

            _collisionHandler.SetCollision("Player", "Walls");
            _collisionHandler.SetCollision("Enemy", "Walls");
            _collisionHandler.SetOverlap("Player", "Pickup");
            _collisionHandler.SetOverlap("Enemy", "Player");


            // Set start location
            bgPos = new Vector2(0, 0);

            // Set up camera and viewport


            game._camera.Zoom = 4;
        }

        public override void LoadContent()
        {
            //backgrounds
            // caveMapBackground = new TileMap("tilemaps/prototype/MapPrototypeTiledCollider", Content, GraphicsDevice);
            caveMapBackground = new TileMap("tilemaps/cave/CollisionTestMap", _content, game.GraphicsDevice, _collisionHandler);

            // player
            player = new Player(game.graphics, caveMapBackground.GetWaypoint("PlayerObjects", "PlayerSpawn"), _collisionHandler);
            player.Load(_content, _collisionHandler, caveMapBackground._mapBounds);
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

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                game.Exit();

            Matrix projectionMatrix = Matrix.CreateOrthographicOffCenter(0, game.screenDimensions.X, game.screenDimensions.Y, 0, 1, 0);
            bgPos = player.Update(Mouse.GetState(), Keyboard.GetState(), game._camera, gameTime) - game.screenDimensions / 2;
            game._camera.Position = bgPos;
            game.inventory.Update(Mouse.GetState(), Keyboard.GetState());

            caveMapBackground.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            game.GraphicsDevice.Clear(Color.Brown);

            Matrix projectionMatrix = Matrix.CreateOrthographicOffCenter(0, game.screenDimensions.X, game.screenDimensions.Y, 0, 1, 0);

            // Draw tilemap background
            caveMapBackground.Draw(_spriteBatch, game._camera.GetViewMatrix(), projectionMatrix, _isDebug);

            // Draw sprites
            _spriteBatch.Begin(transformMatrix: game._camera.GetViewMatrix(), sortMode: SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp);

            player.Draw(_spriteBatch, _isDebug);

            _spriteBatch.End();

            // Draw UI
            _spriteBatch.Begin(sortMode: SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp);
            if (game.inventory.showInv)
                game.inventory.Draw(_spriteBatch);
            _spriteBatch.End();

            // base.Draw(gameTime);
        }

        

        public override void PostUpdate(GameTime gameTime)
        {
            // throw new NotImplementedException();
        }

        public override void unloadState()
        {
            // throw new NotImplementedException();
        }
    }
}
