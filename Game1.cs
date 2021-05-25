using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using MonoGame.Extended.ViewportAdapters;
using MonoGame.Extended;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Media;

//hi
//123
namespace WillowWoodRefuge
{
    public class Game1 : Game
    {
        public InputManager input = new InputManager();
        public static Game1 instance;
        public GraphicsDeviceManager graphics;
        private SpriteBatch _spriteBatch;
        Dictionary<string, State> _states;
        public SoundManager sounds;

        public StateConditions stateConditions;

        // private SpriteBatch _spriteBatch;

        //User Interface
        public UIManager UI;
        public Inventory inventory;
        public Cook cookingGame;
        public RecipeSelection recipeMenu;
        public HUD gameHUD;


        // create vatiable for the state manager

        public State _currentState;
        public string _currentStateName;
        public string _changeRequest = null;

        public State _nextState;

        public CameraController _cameraController;

        // temp button clicking var so changing scene doesn't happen multiple times
        private bool _wasPressed = false;

        public void ChangeState(string sState, string spawnLocLabel = "Default")
        {
            sounds?.stop();
            _nextState = _states[sState];
            _currentState?.unloadState();
            GameplayState state = _nextState as GameplayState;
            if (state != null) // next state is gameplay state
            {
                state = _currentState as CaveState;
                if (state != null)
                {
                    spawnLocLabel = "fromCave";
                }
                state = _currentState as CampState;
                if (state != null)
                {
                    spawnLocLabel = "fromCamp";
                }
                state = _nextState as GameplayState;
                state._startLocLabel = spawnLocLabel;
            }
            _nextState.LoadContent();
            _currentStateName = sState;
        }

        public Game1()
        {
            this.Window.Title = "Willow Wood Refuge";
            graphics = new GraphicsDeviceManager(this);
            _states = new Dictionary<string, State>();
            // create song manager
            
            //_spriteBatch = new SpriteBatch();
            instance = this;
            Content.RootDirectory = "Content";
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
            this.IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _spriteBatch = new SpriteBatch(graphics.GraphicsDevice);

            // setup camera controller
            // _cameraController = new CameraController(graphics, new Vector2(16, 9), new Vector2(640, 360), new Vector2(1728, 972));
            // _cameraController = new CameraController(graphics, new Vector2(16, 9), new Vector2(240, 135), new Vector2(1728, 972));
            _cameraController = new CameraController(graphics, new Vector2(16, 9), new Vector2(480, 270), new Vector2(1728, 972));
            _cameraController.SetPlayerBounds(new RectangleF(0, 0, 160f, 90f));

            // Temp debug add print out of new size when resizing
            _cameraController.AddResizeListener(onResize);

            // load sprite fonts
            FontManager.Initialize(Content);

            _states.Add("LoadingState", new LoadingState(this, Content, _spriteBatch, _states));
            _states.Add("MenuState", new MenuState(this, graphics.GraphicsDevice, Content, _spriteBatch));
            
            ChangeState("MenuState");

            input.Initialize();

            base.Initialize();
        }

        protected override void LoadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            if (_currentStateName != "MenuState" && _currentStateName != "LoadingState")
            {
                input.Update(gameTime);
                //Debug.WriteLine();
                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                    Exit();

                stateConditions.ConditionUpdate(gameTime);

                if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                    ChangeState("MenuState");
            }

            if (_nextState != null)
            {
                _currentState = _nextState;

                _nextState = null;
            }

            _currentState.Update(gameTime);

            _currentState.PostUpdate(gameTime);

            if (_currentStateName != "MenuState")
            {
                if (_changeRequest != null)
                {
                    ChangeState(_changeRequest);
                    _changeRequest = null;
                }

                // toggle windowed/fullscreen
                if (input.IsDown("alternate") && input.JustPressed("toggleWindowed"))
                    _cameraController.ToggleFullscreen();

                if (Keyboard.GetState().IsKeyDown(Keys.D1) ||
                        Keyboard.GetState().IsKeyDown(Keys.D2) ||
                        Keyboard.GetState().IsKeyDown(Keys.D3))
                {
                    _wasPressed = true;
                }
                else if (!Keyboard.GetState().IsKeyDown(Keys.D1) &&
                         !Keyboard.GetState().IsKeyDown(Keys.D2) &&
                         !Keyboard.GetState().IsKeyDown(Keys.D3))
                {
                    _wasPressed = false;
                }
            }

                base.Update(gameTime);

            // this.UI.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _currentState.Draw(gameTime, _spriteBatch);

            base.Draw(gameTime);
        }

        public TileMap GetCurrentTilemap()
        {
            GameplayState state;
            if (_nextState != null)
            {
                state = _nextState as GameplayState;
                if (state != null)
                {
                    return state._tileMap;
                }
            }

            state = _currentState as GameplayState;
            if (state != null)
            {
                return state._tileMap;
            }

            return null;
        }

        public void RequestStateChange(string nextState)
        {
            _changeRequest = nextState;
        }

        // called on screen resize
        void onResize(Vector2 size)
        {
            Debug.WriteLine(size);
        }
    }
}
