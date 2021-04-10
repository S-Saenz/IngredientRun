using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.IO;

namespace WillowWoodRefuge
{
    class GameplayState : State
    {
        // Shaders
        protected Effect _shadowEffect;
        protected Effect _ditherOpacityEffect;
        protected LightManager _staticLightManager;
        protected LightManager _dynamicLightManager;
        protected Color _shadowColor = new Color(26, 17, 7, 255);

        // Render targets
        public RenderTarget2D _backgroundBuffer;
        public RenderTarget2D _foregroundBuffer;
        public RenderTarget2D _casterBuffer;
        public RenderTarget2D _bakedShadowBuffer;
        public RenderTarget2D _shadowBuffer;
        public RenderTarget2D _ditherShadowBuffer;

        // Saved shader textures
        protected Texture2D _blankTexture;

        // Light info
        protected bool _isDark = false;

        // Player instance
        protected Player _player;
        protected int _playerLightIndex = -1;

        // Start location
        public string _startLocLabel;

        // Spawnable instances
        public List<Enemy> _enemies = new List<Enemy>();
        public List<PickupItem> _items = new List<PickupItem>();

        // NPC Parameters
        protected NPCDialogueSystem _dialogueSystem = null;
        public Dictionary<string, NPC> _characters { protected set; get; }

        // Backgrounds
        public TileMap _tileMap { protected set; get; }
        protected List<Texture2D> _backgroundLayers = null; // TODO: make (maybe ordered) list of layer class instances

        // Debug mode
        static protected bool _showMiniDebug = false;
        static protected bool _showFullDebug = false;
        // 0 = camera, 1 = physics, 2 = ai, 3 = player
        static protected int _fullDebugMode = 3;
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

            _physicsHandler.SetCollision("Player", "Walls");
            _physicsHandler.SetCollision("NPC", "Walls");
            _physicsHandler.SetCollision("Enemy", "Walls");
            _physicsHandler.SetOverlap("Player", "Pickup");
            _physicsHandler.SetOverlap("Enemy", "Player");
            _physicsHandler.SetOverlap("Player", "Areas");

            _characters = new Dictionary<string, NPC>();

            // Setup light shader
            _shadowEffect = content.Load<Effect>("shaders/CastShadows");
            _dynamicLightManager = new LightManager(_shadowEffect);
            _staticLightManager = new LightManager(_shadowEffect);

            _ditherOpacityEffect = content.Load<Effect>("shaders/DitherOpacity");

            // Add player light
            _playerLightIndex = _dynamicLightManager._numDLights;
            _dynamicLightManager.AddLight(new Vector2(336, 239), 300, new Vector2(0, 1), 200, 0.3f);
        }

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

            // Setup lighting
            _dynamicLightManager.CreateShaderArrays();
            _shadowEffect.Parameters["TextureDimensions"].SetValue(new Vector2(_tileMap._mapBounds.Width, _tileMap._mapBounds.Height));
            _ditherOpacityEffect.Parameters["TextureDimensions"].SetValue(new Vector2(_tileMap._mapBounds.Width, _tileMap._mapBounds.Height));
            _shadowEffect.Parameters["CasterTexture"].SetValue(_casterBuffer);
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
                character.Update(gameTime, _player._pos);
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
                _dynamicLightManager.ChangeDirectionLight(_playerLightIndex, loc: _player._pos, direction: -dir);
            }
        }

        public override void PostUpdate(GameTime gameTime) { }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Find projection matrix
            Matrix projectionMatrix = Matrix.CreateOrthographicOffCenter(0, game._cameraController._screenDimensions.X, game._cameraController._screenDimensions.Y, 0, 1, 0);
            
            // Render background target
            game.GraphicsDevice.SetRenderTarget(_backgroundBuffer);
            game.GraphicsDevice.Clear(Color.Gray);
            
            // If background layers, draw in order TODO: parallax
            if (_backgroundLayers != null)
            {
                _spriteBatch.Begin(sortMode: SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp);
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

            // render shadow target
            if (_isDark && !(_showMiniDebug || _showFullDebug))
            {
                game.GraphicsDevice.SetRenderTarget(_shadowBuffer);
                game.GraphicsDevice.Clear(Color.Transparent);

                // Render dynamic lights
                _spriteBatch.Begin(sortMode: SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp, effect: _shadowEffect);
                _spriteBatch.Draw(_bakedShadowBuffer, Vector2.Zero, Color.White);
                _spriteBatch.End();

                game.GraphicsDevice.SetRenderTarget(_ditherShadowBuffer);
                game.GraphicsDevice.Clear(Color.Transparent);

                _spriteBatch.Begin(blendState: BlendState.Additive, sortMode: SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp, effect: _ditherOpacityEffect);
                _spriteBatch.Draw(_shadowBuffer, Vector2.Zero, Color.White);
                _spriteBatch.End();
            }

            game.GraphicsDevice.SetRenderTarget(null);

            _spriteBatch.Begin(transformMatrix: game._cameraController.GetViewMatrix(), sortMode: SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp);
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
                area.Draw(spriteBatch, "    Fire\n", game._cameraController, Color.Red);
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
            foreach(Enemy enemy in _enemies)
            {
                enemy.Draw(spriteBatch);
            }
            // Draw pickup items
            foreach (PickupItem item in _items)
            {
                item.Draw(spriteBatch);
            }
            // Draw player
            if (_player != null)
            {
                _player.Draw(_spriteBatch, _showFullDebug || _showMiniDebug);
            }
            _spriteBatch.End();

            // Draw foreground and shadows to screen
            _spriteBatch.Begin(transformMatrix: game._cameraController.GetViewMatrix(), sortMode: SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp); // add dither effect here
            _spriteBatch.Draw(_foregroundBuffer, Vector2.Zero, Color.White);
            _spriteBatch.End();

            if (_isDark && !(_showMiniDebug || _showFullDebug))
            {
                _spriteBatch.Begin(transformMatrix: game._cameraController.GetViewMatrix(), sortMode: SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp);
                _spriteBatch.Draw(_ditherShadowBuffer, Vector2.Zero, Color.White);
                _spriteBatch.End();
            }

            // If dialogue, draw dialogue
            if (_dialogueSystem != null)
            {
                spriteBatch.Begin(sortMode: SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp);
                _dialogueSystem.Draw(game._cameraController._camera, gameTime, spriteBatch);
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
            _casterBuffer = new RenderTarget2D(
                game.GraphicsDevice,
                (int)_tileMap._mapBounds.Width,
                (int)_tileMap._mapBounds.Height,
                false,
                game.GraphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.Depth24);
            _shadowBuffer = new RenderTarget2D(
                game.GraphicsDevice,
                (int)_tileMap._mapBounds.Width,
                (int)_tileMap._mapBounds.Height,
                false,
                game.GraphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.Depth24);
            _bakedShadowBuffer = new RenderTarget2D(
                game.GraphicsDevice,
                (int)_tileMap._mapBounds.Width,
                (int)_tileMap._mapBounds.Height,
                false,
                game.GraphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.Depth24);
            _ditherShadowBuffer = new RenderTarget2D(
                game.GraphicsDevice,
                (int)_tileMap._mapBounds.Width,
                (int)_tileMap._mapBounds.Height,
                false,
                game.GraphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.Depth24);

            // populate shadow as black
            _blankTexture = new Texture2D(game.GraphicsDevice, (int)_tileMap._mapBounds.Width, (int)_tileMap._mapBounds.Height);
            Color[] data = new Color[(int)_tileMap._mapBounds.Width * (int)_tileMap._mapBounds.Height];
            for (int i = 0; i < data.Length; ++i)
            {
                data[i] = _shadowColor;
                // data[i] = new Color((float)(i % _blankTexture.Width) / _blankTexture.Width, 0, (float)(i / _blankTexture.Width) / _blankTexture.Height);
            }
            _blankTexture.SetData(data);

            // setup caster texture
            game.GraphicsDevice.SetRenderTarget(_casterBuffer);
            game.GraphicsDevice.Clear(Color.Transparent);

            _spriteBatch.Begin(sortMode: SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp);
            _tileMap.DrawLayer(_spriteBatch, "Walls");
            _tileMap.DrawLayer(_spriteBatch, "Foreground");
            _spriteBatch.End();

            _shadowEffect.Parameters["CasterTexture"].SetValue(_casterBuffer);
            //dither effect loader
            _ditherOpacityEffect.Parameters["ditherMap"].SetValue(_content.Load<Texture2D>("dither/dithersheet"));

            _staticLightManager.CreateShaderArrays();

            // bake static lights
            game.GraphicsDevice.SetRenderTarget(_bakedShadowBuffer);
            game.GraphicsDevice.Clear(Color.Transparent);

            _spriteBatch.Begin(sortMode: SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp, effect: _shadowEffect);
            _spriteBatch.Draw(_blankTexture, Vector2.Zero, Color.White);
            _spriteBatch.End();

            game.GraphicsDevice.SetRenderTarget(null);

            // Stream stream = File.Create("shadow.png");
            // _bakedShadowBuffer.SaveAsPng(stream, _bakedShadowBuffer.Width, _bakedShadowBuffer.Height);
            // stream.Dispose();
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
