using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace IngredientRun.States
{
    class CampState : State
    {
        private NPCDialogueSystem _dialogueSystem;
        private SpriteFont _dialogueFont;

        Texture2D campPNGBackground;
        TileMap campTileMap;

        // Debug mode
        bool _isDebug = true;
        bool _ctrlPrevDown = false;

        Player player;

        Vector2 bgPos;

        private PhysicsHandler _collisionHandler;

        public CampState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content, SpriteBatch spriteBatch)
            : base(game, graphicsDevice, content, spriteBatch)
        {
            // initialize NPC dialogue content
            _dialogueSystem = new NPCDialogueSystem("Content/dialogue/NPCDialogue.tsv", game);

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
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            game.GraphicsDevice.Clear(Color.Gray);
            Matrix projectionMatrix = Matrix.CreateOrthographicOffCenter(0, game.screenDimensions.X, game.screenDimensions.Y, 0, 1, 0);

            // Draw png background
            _spriteBatch.Begin(transformMatrix: game._camera.GetViewMatrix(), sortMode: SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp);
            Rectangle destination = (Rectangle)campTileMap._mapBounds;
            destination.Height /= 2;
            destination.Y += destination.Height;
            _spriteBatch.Draw(campPNGBackground, destination, Color.White);
            _spriteBatch.End();


            // Draw tilemap background/walls
            spriteBatch.Begin(sortMode: SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp);
            campTileMap.DrawLayer(spriteBatch, game._camera.GetViewMatrix(), projectionMatrix, "Background");
            campTileMap.DrawLayer(spriteBatch, game._camera.GetViewMatrix(), projectionMatrix, "Walls");
            if (_isDebug)
            {
                campTileMap.DrawDebug(spriteBatch, game._camera.GetViewMatrix(), projectionMatrix);
            }
            spriteBatch.End();

            // Draw sprites
            _spriteBatch.Begin(transformMatrix: game._camera.GetViewMatrix(), sortMode: SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp);
            campTileMap.DrawPickups(spriteBatch, _isDebug);
            player.Draw(_spriteBatch, _isDebug);
            _spriteBatch.End();

            // Draw tilemap foreground
            spriteBatch.Begin(sortMode: SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp);
            campTileMap.DrawLayer(spriteBatch, game._camera.GetViewMatrix(), projectionMatrix, "Foreground");
            spriteBatch.End();

            // Draw UI
            _spriteBatch.Begin(sortMode: SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp);
            if (game.inventory.showInv)
                game.inventory.Draw(_spriteBatch);
            _spriteBatch.End();

            spriteBatch.Begin();
            _dialogueSystem.Draw(_dialogueFont, new Vector2(100, 100), gameTime, spriteBatch);
            // spriteBatch.DrawString(_dialogueFont, "Arg: Again with your fuckin' omens!  Did your \"omens\" tell you about that silent nightmare that fuckin' destroyed our homes?", new Vector2(100, 100), Color.White);
            spriteBatch.End();
        }

        public override void LoadContent()
        {
            // dialogue system
            _dialogueFont = _content.Load<SpriteFont>("fonts/NPCDialogue");
            _dialogueSystem.PlayInteraction(game);

            //backgrounds
            campPNGBackground = _content.Load<Texture2D>("bg/campsiteprototypemapANNOTATED");
            campTileMap = new TileMap("tilemaps/camp/TempCampMap", _content, game.GraphicsDevice, _collisionHandler);

            // temp, just respawns objects when entering cave
            campTileMap.SpawnPickups();
            campTileMap.SpawnEnemies();

            // player
            player = new Player(game.graphics, campTileMap.GetWaypoint("PlayerObjects", "PlayerSpawn"), _collisionHandler);
            player.Load(_content, _collisionHandler, campTileMap._mapBounds);
        }

        public override void PostUpdate(GameTime gameTime)
        {

        }

        public override void unloadState()
        {
            _dialogueSystem.EndInteraction();
        }

        public override void Update(GameTime gameTime)
        {
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

            campTileMap.Update(gameTime);
        }
    }
}
