using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
//using WillowWoodRefuge.Game.Weather;

namespace WillowWoodRefuge
{
    abstract class GameplayState : State
    {
        // Shader manager classes
        protected Rain _rain;
        protected Fog _fog;
        protected LightManager _lightManager;
        protected Color _shadowColor = new Color(26, 17, 7, 255);
        static protected bool _occlusion = true;

        // Camera zoom
        protected Vector2 _cameraSize;
        protected RectangleF _playerCamBounds;

        // Render targets
        protected RenderTarget2D _backgroundBuffer;
        protected RenderTarget2D _foregroundBuffer;

        // Light info
        protected bool _isDark = false;

        // Weather info
        protected bool _isRaining = true;
        protected bool _isFoggy = true;

        // Player instance
        protected Player _player;
        protected int _playerLightIndex = -1;

        // Start location
        public string _startLocLabel;

        // Spawnable instances
        public List<Enemy> _enemies = new List<Enemy>();
        public List<SpawnItem> _items = new List<SpawnItem>();

        // NPC Parameters
        protected NPCDialogueSystem _dialogueSystem = null;
        public Dictionary<string, NPC> _characters { protected set; get; }

        // Backgrounds
        public TileMap _tileMap { protected set; get; }
        protected List<Background> _backgroundLayers = null; // TODO: make (maybe ordered) list of layer class instances

        // Debug mode
        static protected bool _showMiniDebug = false;
        static protected bool _showFullDebug = false;
        // 0 = camera, 1 = physics, 2 = ai, 3 = player
        static protected int _fullDebugMode = 2;
        static protected int _miniDebugMode = 1;
        static protected int _numDebugModes = 4;

        // Physics handler
        protected PhysicsHandler _physicsHandler;

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
            _physicsHandler.AddLayer("Foraging");
            _physicsHandler.AddLayer("PickupItems");

            _physicsHandler.SetCollision("Player", "Walls");
            _physicsHandler.SetCollision("NPC", "Walls");
            _physicsHandler.SetCollision("Enemy", "Walls");
            _physicsHandler.SetOverlap("Player", "NPC");
            _physicsHandler.SetOverlap("Player", "Pickup");
            _physicsHandler.SetOverlap("Player", "Foraging");
            _physicsHandler.SetOverlap("Enemy", "Player");
            _physicsHandler.SetOverlap("Player", "Areas");
            _physicsHandler.SetOverlap("Player", "PickupItems");

            _characters = new Dictionary<string, NPC>();

            // Setup weather effects
            _rain = new Rain(new Vector2(30, 200), Vector2.Zero, .00001f, Color.Blue, _content);
            _fog = new Fog(new Vector2(-10, 0), Vector2.Zero, .5f, Color.White, 6f, 10, _content);

            // Setup light manager
            _lightManager = new LightManager(_content);

            // Add player light
            _playerLightIndex = _lightManager._numDLights;
            _lightManager.AddLight(new Vector2(336, 239), 300, new Vector2(0, 1), 200, 0.3f);

            // Load Tilemap
            LoadTilemap(content);

            // Add tilemap lighting
            _tileMap.AddLightObjects(_lightManager);

            PostConstruction();
        }

        abstract protected void LoadTilemap(ContentManager content);

        public override void LoadContent()
        {
            // Temp, just respawns objects and enemies
            _tileMap.SpawnPickups(ref _items);
            _tileMap.SpawnEnemies(ref _enemies);

            // Setup player
            _player = new Player(game.graphics, _tileMap.GetWaypoint("PlayerObjects", "PlayerSpawn." + _startLocLabel), _physicsHandler);
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
            game._cameraController.SetPixelDimensions(_cameraSize);
            game._cameraController.SetPlayerBounds(_playerCamBounds);

            _rain.ChangeParam(bounds: new Vector2(_tileMap._mapBounds.Width, _tileMap._mapBounds.Height));
            _rain.Generate(_spriteBatch);
            _fog.ChangeParam(bounds: new Vector2(_tileMap._mapBounds.Width, _tileMap._mapBounds.Height));
            _fog.Generate(_spriteBatch);
            _lightManager.Load(_content);
        }

        public override void Update(GameTime gameTime)
        {
            
            UpdateDebug();

            // Exit to main menu TODO: change to pause/settings menu
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                game.Exit();

            // Update enemies
            foreach (Enemy enemy in _enemies)
            {
                enemy.Update(gameTime, _player._pos);
            }

            // Update NPCs
            foreach (NPC character in _characters.Values)
            {
                character.Update(gameTime);
            }

            // Update dialogue
            if (_dialogueSystem != null)
            {
                _dialogueSystem.Update(gameTime);
            }

            // Update all foraging (for all scenes)
            foreach(ForageSpot spot in ForageSpot._forageSpots)
            {
                spot.Update(gameTime);
            }

            // Update camera
            game._cameraController.Update(gameTime, _player._pos);

            // Update UI manager
            game.UI.Update(gameTime);

            // Update tilemap
            _tileMap.Update(gameTime);

            // Update player
            Vector2 dir = _player.Update(Mouse.GetState(), Keyboard.GetState(), game._cameraController._camera, gameTime);
            if(_playerLightIndex != -1)
            {
                _lightManager.ChangeDirectionLight(_playerLightIndex, loc: _player._pos, direction: -dir);
            }

            if(_isRaining)
                _rain.Update(gameTime);
            if (_isFoggy)
                _fog.Update(gameTime);
            if (_isDark)
                _lightManager.Update(_spriteBatch);
                
            // Toggle performance (wall occlusion for shader)
            if (game.input.JustPressed("performance"))
            {
                _occlusion = !_occlusion;
                _lightManager.SetOcclusion(_occlusion);
            }

            // Toggle lighting
            if(game.input.JustPressed("light"))
            {
                _isDark = !_isDark;
            }
        }

        public override void PostUpdate(GameTime gameTime) { }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Find projection matrix
            Matrix projectionMatrix = Matrix.CreateOrthographicOffCenter(0, game._cameraController._screenDimensions.X, game._cameraController._screenDimensions.Y, 0, 1, 0);
            
            // Render background target
            game.GraphicsDevice.SetRenderTarget(_backgroundBuffer);
            game.GraphicsDevice.Clear(Color.Transparent);

            // Draw tilemap background/walls
            spriteBatch.Begin(sortMode: SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp);
            _tileMap.DrawLayer(spriteBatch, "Background");
            _tileMap.DrawLayer(spriteBatch, "Walls");
            spriteBatch.End();


            // render foreground target
            game.GraphicsDevice.SetRenderTarget(_foregroundBuffer);
            game.GraphicsDevice.Clear(Color.Transparent);

            // Draw tilemap foreground
            spriteBatch.Begin(sortMode: SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp);
            _tileMap.DrawLayer(spriteBatch, "Foreground");
            spriteBatch.End();

            game.GraphicsDevice.SetRenderTarget(null);

            _spriteBatch.Begin(transformMatrix: game._cameraController.GetViewMatrix(), sortMode: SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp);

            // If background layers, draw in order TODO: parallax
            if (_backgroundLayers != null)
            {
                Rectangle destination = (Rectangle)_tileMap._mapBounds;
                destination.Height /= 2;
                destination.Y += destination.Height;

                foreach (Background layer in _backgroundLayers)
                {
                    //_spriteBatch.Draw(layer, destination, Color.White);
                    layer.Draw(spriteBatch, game._cameraController._cameraOffset);
                }
            }

            game.GraphicsDevice.SetRenderTarget(null);

            _spriteBatch.Begin(transformMatrix: game._cameraController.GetViewMatrix(), sortMode: SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp);

            // If background layers, draw in order TODO: parallax
            if (_backgroundLayers != null)
            {
                Rectangle destination = (Rectangle)_tileMap._mapBounds;
                destination.Height /= 2;
                destination.Y += destination.Height;

                foreach (Background layer in _backgroundLayers)
                {
                    //_spriteBatch.Draw(layer, destination, Color.White);
                    layer.Draw(spriteBatch, game._cameraController._cameraOffset);
                }
            }

            _spriteBatch.Draw(_backgroundBuffer, Vector2.Zero, Color.White);
            _spriteBatch.End();

            // (temp) Draw scene change areas
            foreach (Area area in _tileMap.GetAreaObject("state.Cave"))
            {
                area.Draw(spriteBatch, "To Cave", game._cameraController, Color.Gray);
            }
            foreach (Area area in _tileMap.GetAreaObject("stateCamp"))
            {
                area.Draw(spriteBatch, "To Camp", game._cameraController, Color.Gray);
            }
            foreach (Area area in _tileMap.GetAreaObject("fire"))
            {
                area.Draw(spriteBatch, "    Fire\n", game._cameraController, Color.White);
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
            // Draw entities
            _tileMap.DrawEnemies(spriteBatch);
            _tileMap.DrawPickups(spriteBatch);
            _tileMap.DrawForage(spriteBatch);

            // Draw player
            if (_player != null)
            {
                _player.Draw(_spriteBatch, _showFullDebug || _showMiniDebug);
            }
            _spriteBatch.End();

            // Draw foreground to screen
            _spriteBatch.Begin(transformMatrix: game._cameraController.GetViewMatrix(), sortMode: SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp); // add dither effect here
            _spriteBatch.Draw(_foregroundBuffer, Vector2.Zero, Color.White);
            _spriteBatch.End();

            // Draw weather
            if (_isRaining && !(_showMiniDebug || _showFullDebug))
            {
                _rain.Draw(spriteBatch, game._cameraController.GetViewMatrix());
            }

            if (_isFoggy && !(_showMiniDebug || _showFullDebug))
            {
                _fog.Draw(spriteBatch, game._cameraController.GetViewMatrix());
            }

            // Draw shadow
            if (_isDark && !(_showMiniDebug || _showFullDebug))
            {
               _lightManager.Draw(spriteBatch, game._cameraController.GetViewMatrix());
            }

            // If dialogue, draw dialogue
            if (_dialogueSystem != null)
            {
                spriteBatch.Begin(sortMode: SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp);
                _dialogueSystem.Draw(game._cameraController._camera, spriteBatch);
                spriteBatch.End();
            }

            // draw e to interact
            if (_player != null && _player._overlappingInteractable)
            {
                spriteBatch.Begin(sortMode: SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp);
                spriteBatch.DrawString(FontManager._dialogueFont, "Press E to " + _player._overlapName, Game1.instance._cameraController._camera.WorldToScreen(_player._pos - new Vector2(0,18)), Color.White);
                spriteBatch.End();
            }

            DrawDebug(spriteBatch);

            // Draw UI
            _spriteBatch.Begin(sortMode: SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp);
                game.UI.Draw(spriteBatch);
            _spriteBatch.End();

            // Stream stream = File.Create("shadow.png");
            // _bakedShadowBuffer.SaveAsPng(stream, _bakedShadowBuffer.Width, _bakedShadowBuffer.Height);
            // stream.Dispose();
        }

        public override void unloadState()
        {
            // Remove player hitbox
            _player.RemoveCollision(_physicsHandler);
            _player = null;

            // Remove enemy hitboxes
            foreach (Enemy enemy in _enemies)
            {
                enemy.Destroy(_physicsHandler);
            }
            _enemies.Clear();

            // Remove pickup item hitboxes
            foreach (SpawnItem item in _items)
            {
                item.RemoveCollision(_physicsHandler);
            }
            _items.Clear();

            // remove NPC hitboxes
            foreach (NPC character in _characters.Values)
            {
                character.Destroy(_physicsHandler);
            }
            _characters.Clear();
        }

        void UpdateDebug()
        {
            // Switch to full debug mode
            if (Game1.instance.input.JustPressed("debugFullToggle") && !Game1.instance.input.IsDown("alternate"))
            {
                _showFullDebug = !_showFullDebug;
            }

            // Switch to mini debug mode
            if (Game1.instance.input.JustPressed("debugMiniToggle") && !Game1.instance.input.IsDown("alternate"))
            {
                _showMiniDebug = !_showMiniDebug;
            }

            // check for debug input
            if (_showFullDebug && Game1.instance.input.JustPressed("debugFullToggle") && Game1.instance.input.IsDown("alternate"))
            {
                _fullDebugMode = (_fullDebugMode + 1) % _numDebugModes;
            }

            // check for debug input
            if (_showMiniDebug && Game1.instance.input.JustPressed("debugMiniToggle") && Game1.instance.input.IsDown("alternate"))
            {
                _miniDebugMode = (_miniDebugMode + 1) % _numDebugModes;
            }

            // change scene
            if ((_showFullDebug || _showMiniDebug) && Game1.instance.input.JustPressed("changeCaveState"))
                game.RequestStateChange("CaveState");
            else if ((_showFullDebug || _showMiniDebug) && Game1.instance.input.JustPressed("changeCampState"))
                game.RequestStateChange("CampState");
            else if ((_showFullDebug || _showMiniDebug) && Game1.instance.input.JustPressed("restartState"))
                game.RequestStateChange(game._currentStateName);
        }

        void DrawDebug(SpriteBatch spriteBatch)
        {
            // mini map debug
            if (_showMiniDebug)
            {
                spriteBatch.Begin(sortMode: SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp);
                switch (_miniDebugMode)
                {
                    case 0: // camera
                        game._cameraController.Draw(spriteBatch);
                        break;
                    case 1: // physics
                        _physicsHandler.DrawDebug(spriteBatch);
                        break;
                    case 2: // ai
                        // NPCs
                        if (_characters != null)
                        {
                            foreach (NPC obj in _characters.Values)
                            {
                                obj.DrawDebug(spriteBatch);
                            }
                        }
                        // Enemies
                        foreach (Enemy enemy in _enemies)
                        {
                            enemy.DrawDebug(spriteBatch);
                        }
                        break;
                    case 3: // player
                        if (_player != null)
                        {
                            _player.DrawDebug(spriteBatch);
                        }
                        break;
                }
                spriteBatch.End();
            }

            // full screen debug
            if (_showFullDebug)
            {
                spriteBatch.Begin(transformMatrix: game._cameraController.GetViewMatrix(), sortMode: SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp);
                switch (_fullDebugMode)
                {
                    case 0: // camera
                        game._cameraController.Draw(spriteBatch);
                        break;
                    case 1: // physics
                        _physicsHandler.DrawDebug(spriteBatch);
                        break;
                    case 2: // ai
                        // NPCs
                        if (_characters != null)
                        {
                            foreach (NPC obj in _characters.Values)
                            {
                                obj.DrawDebug(spriteBatch);
                            }
                        }
                        // Enemies
                        foreach (Enemy enemy in _enemies)
                        {
                            enemy.DrawDebug(spriteBatch);
                        }
                        break;
                    case 3: // player
                        if (_player != null)
                        {
                            _player.DrawDebug(spriteBatch);
                        }
                        break;
                }
                spriteBatch.End();
            }
        }

        protected void PostConstruction()
        {

            // set up secondary render buffers
            _backgroundBuffer = new RenderTarget2D(
                game.GraphicsDevice,
                (int)_tileMap._mapBounds.Width,
                (int)_tileMap._mapBounds.Height,
                false,
                game.GraphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.Depth24);
            _foregroundBuffer = new RenderTarget2D(
                game.GraphicsDevice,
                (int)_tileMap._mapBounds.Width,
                (int)_tileMap._mapBounds.Height,
                false,
                game.GraphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.Depth24);

            // Add player light
            _playerLightIndex = 0;
            _lightManager.AddLight(new Vector2(336, 239), 300, new Vector2(0, 1), 200, 0.3f, false);
        }

        public void LockPlayerPos()
        {
            _player.LockPos();
        }

        public void UnlockPlayerPos()
        {
            _player.UnlockPos();
        }
    }
}
