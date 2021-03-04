using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace WillowWoodRefuge
{
    class GameplayState : State
    {
        // Player instance
        protected Player _player;

        // Spawnable instances
        protected List<Enemy> _enemies = new List<Enemy>();
        protected List<PickupItem> _items = new List<PickupItem>();

        // NPC Parameters
        protected NPCDialogueSystem _dialogueSystem = null;
        public Dictionary<string, NPC> _characters { protected set; get; }

        // Backgrounds
        public TileMap _tileMap { protected set; get; }
        protected List<Texture2D> _backgroundLayers = null; // TODO: make (maybe ordered) list of layer class instances

        // Debug mode
        static protected bool _isDebug = false;

        // Physics handler
        protected PhysicsHandler _physicsHandler;


        protected int walkTimer;

        protected GameplayState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content, SpriteBatch spritebatch) 
                         : base(game, graphicsDevice, content, spritebatch)
        {
            content.RootDirectory = "Content";

            // Setup collision
            _physicsHandler = new PhysicsHandler();
            _physicsHandler.AddLayer("Player");
            _physicsHandler.AddLayer("NPC");
            _physicsHandler.AddLayer("Enemy");
            _physicsHandler.AddLayer("Pickup");
            _physicsHandler.AddLayer("Walls");
            _physicsHandler.AddLayer("Areas");

            _physicsHandler.SetCollision("Player", "Walls");
            _physicsHandler.SetCollision("NPC", "Walls");
            _physicsHandler.SetCollision("Enemy", "Walls");
            _physicsHandler.SetOverlap("Player", "Pickup");
            _physicsHandler.SetOverlap("Enemy", "Player");
            _physicsHandler.SetOverlap("Player", "Areas");

            _characters = new Dictionary<string, NPC>();
        }

        public override void LoadContent()
        {
            // Temp, just respawns objects and enemies
            _tileMap.SpawnPickups(ref _items);
            _tileMap.SpawnEnemies(ref _enemies);

            // Setup player
            _player = new Player(game.graphics, _tileMap.GetWaypoint("PlayerObjects", "PlayerSpawn"), _physicsHandler);
            _player.Load(_content, _physicsHandler, _tileMap._mapBounds);

            // Setup enemies
            if (_enemies != null)
            {
                foreach (Enemy enemy in _enemies)
                {
                    enemy.Load(game.Content);
                }
            }

            // Setup camera
            game._cameraController.SetWorldBounds(_tileMap._mapBounds);
        }

        public override void Update(GameTime gameTime)
        {
            // Switch to debug mode
            if (Game1.instance.input.JustPressed("debug"))
            {
                _isDebug = !_isDebug;
            }

            // Exit to main menu TODO: change to pause/settings menu
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                game.Exit();

            if (_player._isWalking)
            {
                game.sounds.walkSound(gameTime);
            }

            // Update player
            _player.Update(Mouse.GetState(), Keyboard.GetState(), game._cameraController._camera, gameTime);

            // Update enemies
            foreach (Enemy enemy in _enemies)
            {
                enemy.Update(gameTime, _player._pos);
            }

            if (_player._isWalking)
            {
                game.sounds.walkSound(gameTime);
            }

            // Update NPCs
            foreach (NPC character in _characters.Values)
            {
                character.Update(gameTime, _player._pos);
            }

            // Update camera
            game._cameraController.Update(gameTime, _player._pos);

            // Update inventory TODO: make UI manager, update UI manager
            game.inventory.Update(Mouse.GetState(), Keyboard.GetState());

            // Update tilemap
            _tileMap.Update(gameTime);
        }

        public override void PostUpdate(GameTime gameTime) { }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Find projection matrix
            Matrix projectionMatrix = Matrix.CreateOrthographicOffCenter(0, game._cameraController._screenDimensions.X, game._cameraController._screenDimensions.Y, 0, 1, 0);

            // If background layers, draw in order TODO: parallax
            if (_backgroundLayers != null)
            {
                _spriteBatch.Begin(transformMatrix: game._cameraController.GetViewMatrix(), sortMode: SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp);
                Rectangle destination = (Rectangle)_tileMap._mapBounds;
                destination.Height /= 2;
                destination.Y += destination.Height;
                foreach (Texture2D layer in _backgroundLayers)
                {
                    _spriteBatch.Draw(layer, destination, Color.White);
                }
                _spriteBatch.End();
            }

            // Draw tilemap background/walls
            spriteBatch.Begin(sortMode: SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp);
            _tileMap.DrawLayer(spriteBatch, game._cameraController.GetViewMatrix(), projectionMatrix, "Background");
            _tileMap.DrawLayer(spriteBatch, game._cameraController.GetViewMatrix(), projectionMatrix, "Walls", _isDebug);
            // if (_isDebug)
            // {
            //     _tileMap.DrawDebug(spriteBatch, game._cameraController.GetViewMatrix(), projectionMatrix);
            // }
            spriteBatch.End();

            // If dialogue, draw dialogue
            if (_dialogueSystem != null)
            {
                spriteBatch.Begin(sortMode: SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp);
                _dialogueSystem.Draw(game._cameraController._camera, gameTime, spriteBatch);
                spriteBatch.End();
            }

            // Draw sprites
            _spriteBatch.Begin(transformMatrix: game._cameraController.GetViewMatrix(), sortMode: SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp);
            // Draw NPCs
            if (_characters != null)
            {
                foreach (NPC obj in _characters.Values)
                {
                    obj.Draw(spriteBatch);
                }
            }
            // Draw enemies
            _tileMap.DrawEnemies(spriteBatch, _isDebug);
            // Draw pickup items
            _tileMap.DrawPickups(spriteBatch, _isDebug);
            // Draw player
            _player.Draw(_spriteBatch, _isDebug);
            _spriteBatch.End();

            // Draw tilemap foreground
            spriteBatch.Begin(sortMode: SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp);
            _tileMap.DrawLayer(spriteBatch, game._cameraController.GetViewMatrix(), projectionMatrix, "Foreground");
            spriteBatch.End();

            // Draw physics debug
            if (_isDebug)
            {
                _spriteBatch.Begin(sortMode: SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp);
                _physicsHandler.DrawDebug(spriteBatch);
                _spriteBatch.End();
            }

            // Draw UI
            _spriteBatch.Begin(sortMode: SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp);
            if (game.inventory.showInv)
                game.inventory.Draw(_spriteBatch);
            _spriteBatch.End();

            // Draw camera debugging window
            if (_isDebug)
            {
                game._cameraController.Draw(spriteBatch);
            }
        }

        public override void unloadState()
        {
            // Remove player hitbox
            _player.RemoveCollision(_physicsHandler);

            // Remove enemy hitboxes
            foreach (Enemy enemy in _enemies)
            {
                enemy.RemoveCollision(_physicsHandler);
            }
            _enemies.Clear();

            // Remove pickup item hitboxes
            foreach (PickupItem item in _items)
            {
                item.RemoveCollision(_physicsHandler);
            }
            _items.Clear();

            // remove NPC hitboxes
            foreach (NPC character in _characters.Values)
            {
                character.RemoveCollision(_physicsHandler);
            }
            _characters.Clear();
        }
    }
}
