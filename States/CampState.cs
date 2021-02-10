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

        Texture2D campPNGBackground;
        TileMap campTileMap;

        // Debug mode
        bool _isDebug = false;
        bool _ctrlPrevDown = false;

        Player player;

        public Dictionary<string, NPC> _characters { private set; get; }

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
            Matrix projectionMatrix = Matrix.CreateOrthographicOffCenter(0, game._cameraController._screenDimensions.X, game._cameraController._screenDimensions.Y, 0, 1, 0);

            // Draw png background
            _spriteBatch.Begin(transformMatrix: game._cameraController.GetViewMatrix(), sortMode: SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp);
            Rectangle destination = (Rectangle)campTileMap._mapBounds;
            destination.Height /= 2;
            destination.Y += destination.Height;
            _spriteBatch.Draw(campPNGBackground, destination, Color.White);
            _spriteBatch.End();


            // Draw tilemap background/walls
            spriteBatch.Begin(sortMode: SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp);
            campTileMap.DrawLayer(spriteBatch, game._cameraController.GetViewMatrix(), projectionMatrix, "Background");
            campTileMap.DrawLayer(spriteBatch, game._cameraController.GetViewMatrix(), projectionMatrix, "Walls", _isDebug);
            if (_isDebug)
            {
                campTileMap.DrawDebug(spriteBatch, game._cameraController.GetViewMatrix(), projectionMatrix);
            }
            spriteBatch.End();

            // Draw dialogue
            spriteBatch.Begin(sortMode: SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp);
            _dialogueSystem.Draw(game._cameraController._camera, gameTime, spriteBatch);
            spriteBatch.End();

            // Draw sprites
            _spriteBatch.Begin(transformMatrix: game._cameraController.GetViewMatrix(), sortMode: SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp);
            foreach(NPC obj in _characters.Values)
            {
                obj.Draw(spriteBatch);
            }
            campTileMap.DrawPickups(spriteBatch, _isDebug);
            player.Draw(_spriteBatch, _isDebug);
            _spriteBatch.End();

            // Draw tilemap foreground
            spriteBatch.Begin(sortMode: SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp);
            campTileMap.DrawLayer(spriteBatch, game._cameraController.GetViewMatrix(), projectionMatrix, "Foreground");
            spriteBatch.End();

            // Draw UI
            _spriteBatch.Begin(sortMode: SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp);
            if (game.inventory.showInv)
                game.inventory.Draw(_spriteBatch);
            _spriteBatch.End();

            if (_isDebug)
            {
                game._cameraController.Draw(spriteBatch);
            }
        }

        public override void LoadContent()
        {
            //backgrounds
            campPNGBackground = _content.Load<Texture2D>("bg/campsiteprototypemapANNOTATED");
            campTileMap = new TileMap("tilemaps/camp/TempCampMap", _content, game.GraphicsDevice, _collisionHandler);

            // temp, just respawns objects when entering cave
            campTileMap.SpawnPickups();
            campTileMap.SpawnEnemies();

            // player
            player = new Player(game.graphics, campTileMap.GetWaypoint("PlayerObjects", "PlayerSpawn"), _collisionHandler);
            player.Load(_content, _collisionHandler, campTileMap._mapBounds);

            // characters
            _characters = new Dictionary<string, NPC>();
            _characters.Add("Lura", new NPC(_content.Load<Texture2D>("chars/lura"), Vector2.Zero));
            _characters.Add("Snäll", new NPC(_content.Load<Texture2D>("chars/snall"), Vector2.Zero));
            _characters.Add("Kall", new NPC(_content.Load<Texture2D>("chars/kall"), Vector2.Zero));
            _characters.Add("Arg", new NPC(_content.Load<Texture2D>("chars/arg"), Vector2.Zero));
            campTileMap.PlaceNPCs(_characters);

            // dialogue system
            _dialogueSystem.Load(_characters);
            _dialogueSystem.PlayInteraction(game);

            // setup camera
            game._cameraController.SetWorldBounds(campTileMap._mapBounds);
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

            Matrix projectionMatrix = Matrix.CreateOrthographicOffCenter(0, game._cameraController._screenDimensions.X, game._cameraController._screenDimensions.Y, 0, 1, 0);
            bgPos = player.Update(Mouse.GetState(), Keyboard.GetState(), game._cameraController._camera, gameTime) - game._cameraController._screenDimensions / 2;
            game._cameraController.Update(gameTime, player._pos);
            game.inventory.Update(Mouse.GetState(), Keyboard.GetState());

            campTileMap.Update(gameTime);
        }
    }
}
