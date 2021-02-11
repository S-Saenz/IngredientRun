using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Windows.Forms.VisualStyles;
using System.Diagnostics;
using MonoGame.Extended.ViewportAdapters;
using MonoGame.Extended;
using IngredientRun.States;
using System.Collections.Generic;

//hi
//123
namespace IngredientRun
{
    public class Game1 : Game
    {
        public InputManager input = new InputManager();
        public static Game1 instance;
        public GraphicsDeviceManager graphics;
        private SpriteBatch _spriteBatch;
        Dictionary<string, State> _states;
        public SoundManager sounds;

        public List<Condition> _stateConditions = new List<Condition>();

        // private SpriteBatch _spriteBatch;

        //classes
        public static Inventory inventory = new Inventory();
        static public Cook cookingUI = new Cook();
        public RecipeSelection recipeUI = new RecipeSelection(ref cookingUI, ref inventory);

        // create vatiable for the state manager

        private State _currentState;

        private State _nextState;

        public CameraController _cameraController;

        // temp button clicking var so changing scene doesn't happen multiple times
        private bool _wasPressed = false;

        public void ChangeState(string sState)
        {
            sounds.stop();
            _nextState = _states[sState];
            _currentState.unloadState();
            _nextState.LoadContent();
        }

        public Game1()
        {
            this.Window.Title = "Ingredient Time";
            graphics = new GraphicsDeviceManager(this);
            _states = new Dictionary<string, State>();
            // create song manager
            
            //_spriteBatch = new SpriteBatch();
            instance = this;
            Content.RootDirectory = "Content";
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
            this.IsMouseVisible = true;

            // setup camera controller
            // _cameraController = new CameraController(graphics, new Vector2(16, 9), new Vector2(640, 360), new Vector2(1728, 972));
            _cameraController = new CameraController(graphics, new Vector2(16, 9), new Vector2(512, 288), new Vector2(1728, 972));
            _cameraController.SetPlayerBounds(new RectangleF(0, 0, 204.8f, 115.2f));

            // setup bulk texture managers
            ItemTextures.Initialize(Content);
            EnemyTextures.Initialize(Content);
            FontManager.Initialize(Content);
        }

        protected override void Initialize()
        {
            InitializeConditions();
            base.Initialize();
            input.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            sounds = new SoundManager(Content);
            //whenever a new state is added, it will need to be added to this list
            _states.Add("CaveState", new CaveState(this, graphics.GraphicsDevice, Content, _spriteBatch));
            _states.Add("colorState", new colorState(this, graphics.GraphicsDevice, Content, _spriteBatch));
            _states.Add("CampState", new CampState(this, GraphicsDevice, Content, _spriteBatch));

            _currentState = _states["CaveState"];
            _currentState.LoadContent();

            // load inventory
            inventory.Load(Content);
            cookingUI.Load(Content);
            recipeUI.Load(Content);
        }

        protected override void Update(GameTime gameTime)
        {
            input.Update(gameTime);
            //Debug.WriteLine();
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            inventory.Update(Mouse.GetState() ,Keyboard.GetState());
            recipeUI.Update(Mouse.GetState(), Keyboard.GetState());
            cookingUI.Update(Mouse.GetState(), Keyboard.GetState(), gameTime);
            
            if(_nextState != null)
            {
                _currentState = _nextState;

                _nextState = null;
            }

            _currentState.Update(gameTime);

            _currentState.PostUpdate(gameTime);

            if (Keyboard.GetState().IsKeyDown(Keys.D1) && !_wasPressed)
                ChangeState("colorState");
            else if (Keyboard.GetState().IsKeyDown(Keys.D2) && !_wasPressed)
                ChangeState("CaveState");
            else if (Keyboard.GetState().IsKeyDown(Keys.D3) && !_wasPressed)
                ChangeState("CampState");
            
            if(Keyboard.GetState().IsKeyDown(Keys.D1) ||
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

                base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _currentState.Draw(gameTime, _spriteBatch);

            base.Draw(gameTime);
        }

        private void InitializeConditions()
        {
            _stateConditions.Add(new Condition("fedMushroomPrior", true));
            _stateConditions.Add(new Condition("curedPrior", true));
            _stateConditions.Add(new Condition("isMorning", true));
            _stateConditions.Add(new Condition("isRaining", true));
        }
    }
}
