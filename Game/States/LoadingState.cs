using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace WillowWoodRefuge
{
    class LoadingState : State
    {
        // Bake shadows (Turn on for one build after tilemap changed, then turn back off)
        private bool _bakeStaticShadows = false;
        
        // Thread for loading
        // Thread _initializationThread = new Thread(InitializeData);
        // Thread _contentLoadingThread = new Thread(LoadContentData);

        private bool _initializationStarted = false;
        private bool _initializationDone = false;
        private bool _loadingDataStarted = false;
        private bool _loadingDataDone = false;
        private bool _loadingPlayerStarted = false;
        private bool _loadingPlayerDone = false;
        private bool _loadingCampStarted = false;
        private bool _loadingCampDone = false;
        private bool _loadingCaveStarted = false;
        private bool _loadingCaveDone = false;

        // Game1 stored references
        static private Dictionary<string, State> _states;
        static private SpriteBatch _spriteBatch;
        static private LoadingState _instance;

        // Loading timer
        private float _loadTime = 0;

        public LoadingState(Game1 game, ContentManager content, SpriteBatch spriteBatch, Dictionary<string, State> states) 
            : base(game, Game1.instance.graphics.GraphicsDevice, content, spriteBatch)
        {
            _spriteBatch = spriteBatch;
            _states = states;
            _instance = this;
        }

        public static void InitializeData()
        {
            // setup bulk texture managers
            ItemTextures.Initialize(Game1.instance.Content);
            EnemyTextures.Initialize(Game1.instance.Content);
            TextureAtlasManager.Initialize(Game1.instance.Content);

            _instance._initializationDone = true;
        }

        private static void LoadContentData()
        {
            Game1.instance.stateConditions = new StateConditions();
            //whenever a new state is added, it will need to be added to this list
            _states.Add("colorState", new colorState(Game1.instance, Game1.instance.graphics.GraphicsDevice, Game1.instance.Content, _spriteBatch));
            _states.Add("CreditsState", new CreditsState(Game1.instance, Game1.instance.graphics.GraphicsDevice, Game1.instance.Content, _spriteBatch));
            _states.Add("TutorialState", new TutorialState(Game1.instance, Game1.instance.graphics.GraphicsDevice, Game1.instance.Content, _spriteBatch));

            _instance._loadingDataDone = true;
        }

        private static void LoadPlayerData()
        {
            Game1.instance.UI = new UIManager();
            Game1.instance.inventory = new Inventory();
            Game1.instance.cookingGame = new Cook();
            Game1.instance.recipeMenu = new RecipeSelection(Game1.instance);
            Game1.instance.gameHUD = new HUD();

            // load inventory
            Game1.instance.inventory.Load(Game1.instance.Content);
            Game1.instance.gameHUD.Load(Game1.instance.Content);

            _instance._loadingPlayerDone = true;
        }

        private static void LoadCampData()
        {
            if (_states.ContainsKey("CampState"))
                _states.Remove("CampState");
            _states.Add("CampState", new CampState(Game1.instance, Game1.instance.graphics.GraphicsDevice, Game1.instance.Content, _spriteBatch));
            _instance._loadingCampDone = true;
        }

        private static void LoadCaveData()
        {
            if (_states.ContainsKey("CaveState"))
                _states.Remove("CaveState");
            _states.Add("CaveState", new CaveState(Game1.instance, Game1.instance.graphics.GraphicsDevice, Game1.instance.Content, _spriteBatch));
            _instance._loadingCaveDone = true;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Debug.WriteLine("Initialization: " + _initializationDone + "  Load Content: " + _loadingDone + "  Load Time: " + _loadTime);

        }

        public override void LoadContent()
        {
        }

        public override void PostUpdate(GameTime gameTime)
        {
        }

        public override void unloadState()
        {
        }

        public override void Update(GameTime gameTime)
        {
            _loadTime += gameTime.GetElapsedSeconds();
            if(!_initializationStarted)
            {
                Debug.WriteLine("Initialization started");
                ThreadPool.QueueUserWorkItem(state =>
                {
                    InitializeData();
                });
                _initializationStarted = true;
            }
            else if(_initializationDone && !_loadingDataStarted)
            {
                Debug.WriteLine("Data Loading started");
                Game1.instance.sounds = new SoundManager(Game1.instance.Content);
                ThreadPool.QueueUserWorkItem(state =>
                {
                    LoadContentData();
                });
                _loadingDataStarted = true;
            }
            else if (_loadingDataDone && !_loadingPlayerStarted)
            {
                Debug.WriteLine("Player loading started");
                ThreadPool.QueueUserWorkItem(state =>
                {
                    LoadPlayerData();
                });
                _loadingPlayerStarted = true;
            }
            else if(_loadingPlayerDone && !_loadingCampStarted)
            {
                Debug.WriteLine("Camp loading started");
                ThreadPool.QueueUserWorkItem(state =>
                {
                    LoadCampData();
                });
                _loadingCampStarted = true;
            }
            else if(_loadingCampDone && !_loadingCaveStarted)
            {
                // finish camp load
                (_states["CampState"] as GameplayState).PostConstruction();
                if(_bakeStaticShadows)
                    (_states["CampState"] as GameplayState).BakeStaticLights();

                Debug.WriteLine("Cave loading started");
                ThreadPool.QueueUserWorkItem(state =>
                {
                    LoadCaveData();
                });
                _loadingCaveStarted = true;
            }
            else if(_loadingCaveDone)
            {
                // finish cave load
                (_states["CaveState"] as GameplayState).PostConstruction();
                if (_bakeStaticShadows)
                    (_states["CaveState"] as GameplayState).BakeStaticLights();

                Debug.WriteLine("Loading done " + _loadTime);
                Game1.instance.ChangeState("CampState");

                // reset gameplay initialization, so game reloads when re-entering
                _loadingPlayerStarted = 
                _loadingPlayerDone = 
                _loadingCampStarted = 
                _loadingCampDone = 
                _loadingCaveStarted = 
                _loadingCaveDone = false;
            }
        }
    }
}
