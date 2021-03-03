using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace WillowWoodRefuge
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

        List<Enemy> _enemies = new List<Enemy>();
        List<PickupItem> _items = new List<PickupItem>();

        private PhysicsHandler _collisionHandler;

        public CampState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content, SpriteBatch spriteBatch)
            : base(game, graphicsDevice, content, spriteBatch)
        {
            // initialize NPC dialogue content
            _dialogueSystem = new NPCDialogueSystem(game);

            // setup collision
            _collisionHandler = new PhysicsHandler();
            _collisionHandler.AddLayer("Player");
            _collisionHandler.AddLayer("NPC");
            _collisionHandler.AddLayer("Enemy");
            _collisionHandler.AddLayer("Pickup");
            _collisionHandler.AddLayer("Walls");
            _collisionHandler.AddLayer("Areas");

            _collisionHandler.SetCollision("Player", "Walls");
            _collisionHandler.SetCollision("NPC", "Walls");
            _collisionHandler.SetCollision("Enemy", "Walls");
            _collisionHandler.SetOverlap("Player", "Pickup");
            _collisionHandler.SetOverlap("Enemy", "Player");
            _collisionHandler.SetOverlap("Player", "Areas");

            //backgrounds
            campPNGBackground = _content.Load<Texture2D>("bg/campsiteprototypemap");
            campTileMap = new TileMap("tilemaps/camp/TempCampMap", _content, game.GraphicsDevice, _collisionHandler);
            _tileMap = campTileMap;
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
            spriteBatch.End();

            // Draw dialogue
            spriteBatch.Begin(sortMode: SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp);
            _dialogueSystem.Draw(game._cameraController._camera, gameTime, spriteBatch);
            spriteBatch.End();

            // Draw physics debug
            if (_isDebug)
            {
                _spriteBatch.Begin(sortMode: SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp);
                _collisionHandler.DrawDebug(spriteBatch);
                _spriteBatch.End();
            }

            // Draw sprites
            _spriteBatch.Begin(transformMatrix: game._cameraController.GetViewMatrix(), sortMode: SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp);
            foreach(NPC obj in _characters.Values)
            {
                obj.Draw(spriteBatch, _isDebug);
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
            //music
            game.sounds.playSong("forestSong");

            // temp, just respawns objects when entering cave
            campTileMap.SpawnPickups(ref _items);
            campTileMap.SpawnEnemies(ref _enemies);

            // player
            player = new Player(game.graphics, campTileMap.GetWaypoint("PlayerObjects", "PlayerSpawn"), _collisionHandler);
            player.Load(_content, _collisionHandler, campTileMap._mapBounds);

            // characters
            Area campArea = campTileMap.GetAreaObject("Camp")[0];
            Random rand = new Random();
            _characters = new Dictionary<string, NPC>();
            _characters.Add("Lura", new NPC("lura", 
                            new Vector2(rand.Next() % (campArea._bounds.Width - 16) + campArea._bounds.Left + 8, campArea._bounds.Bottom), 
                            _collisionHandler, campTileMap._mapBounds));
            _characters.Add("Snäll", new NPC("snall",
                            new Vector2(rand.Next() % (campArea._bounds.Width - 16) + campArea._bounds.Left + 8, campArea._bounds.Bottom),
                            _collisionHandler, campTileMap._mapBounds));
            _characters.Add("Kall", new NPC("kall",
                            new Vector2(rand.Next() % (campArea._bounds.Width - 16) + campArea._bounds.Left + 8, campArea._bounds.Bottom),
                            _collisionHandler, campTileMap._mapBounds));
            _characters.Add("Arg", new NPC("arg",
                            new Vector2(rand.Next() % (campArea._bounds.Width - 16) + campArea._bounds.Left + 8, campArea._bounds.Bottom),
                            _collisionHandler, campTileMap._mapBounds));
            _characters.Add("Aiyo", new NPC("aiyo",
                            new Vector2(rand.Next() % (campArea._bounds.Width - 16) + campArea._bounds.Left + 8, campArea._bounds.Bottom),
                            _collisionHandler, campTileMap._mapBounds));
            foreach (NPC character in _characters.Values)
            {
                character.Load(_content);
            }

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
            player.RemoveCollision(_collisionHandler);

            foreach (Enemy enemy in _enemies)
            {
                enemy.RemoveCollision(_collisionHandler);
            }
            _enemies.Clear();

            foreach (PickupItem item in _items)
            {
                item.RemoveCollision(_collisionHandler);
            }
            _items.Clear();

            foreach (NPC character in _characters.Values)
            {
                character.RemoveCollision(_collisionHandler);
            }
            _characters.Clear();
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

            // update enemies
            foreach (Enemy enemy in _enemies)
            {
                enemy.Update(gameTime, player._pos);
            }

            if (player._isWalking)
            {
                game.sounds.walkSound(gameTime);
            }

            // update
            foreach (NPC character in _characters.Values)
            {
                character.Update(gameTime, player._pos);
            }

            Matrix projectionMatrix = Matrix.CreateOrthographicOffCenter(0, game._cameraController._screenDimensions.X, game._cameraController._screenDimensions.Y, 0, 1, 0);
            player.Update(Mouse.GetState(), Keyboard.GetState(), game._cameraController._camera, gameTime);
            game._cameraController.Update(gameTime, player._pos);
            game.inventory.Update(Mouse.GetState(), Keyboard.GetState());

            campTileMap.Update(gameTime);
        }
    }
}
