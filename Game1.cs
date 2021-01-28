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
        public static Game1 instance;
        public GraphicsDeviceManager graphics;
        private SpriteBatch _spriteBatch;
        Dictionary<string, State> _states;
        public Vector2 screenDimensions;

        // private SpriteBatch _spriteBatch;

        // public Inventory inventory = new Inventory();

        // create vatiable for the state manager

        private State _currentState;

        private State _nextState;

        public void ChangeState(string sState)
        {
            _nextState = _states[sState];
            _currentState.LoadContent();
        }

        public OrthographicCamera _camera;

        public Game1()
        {
            this.Window.Title = "Ingredient Time";
            graphics = new GraphicsDeviceManager(this);
            _states = new Dictionary<string, State>();
            

            //_spriteBatch = new SpriteBatch();
            instance = this;
            Content.RootDirectory = "Content";
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
            this.IsMouseVisible = true;

            // setup the window
            screenDimensions = new Vector2(1728, 972);

            graphics.PreferredBackBufferWidth = (int)screenDimensions.X;  // set this value to the desired width of your window
            graphics.PreferredBackBufferHeight = (int)screenDimensions.Y;   // set this value to the desired height of your window
            graphics.ApplyChanges();
            DefaultViewportAdapter viewportAdapter = new DefaultViewportAdapter(GraphicsDevice);
            _camera = new OrthographicCamera(viewportAdapter);
            
        }

        protected override void Initialize()
        {
            

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            //whenever a new state is added, it will need to be added to this list
            _states.Add("CaveState", new CaveState(this, graphics.GraphicsDevice, Content, _spriteBatch));
            _states.Add("colorState", new colorState(this, graphics.GraphicsDevice, Content, _spriteBatch));
            _states.Add("CampState", new CampState(this, GraphicsDevice, Content, _spriteBatch));

            _currentState = _states["CaveState"];
            _currentState.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            //Debug.WriteLine();
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if(_nextState != null)
            {
                _currentState = _nextState;

                _nextState = null;
            }

            _currentState.Update(gameTime);

            _currentState.PostUpdate(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _currentState.Draw(gameTime, _spriteBatch);

            base.Draw(gameTime);
        }
    }
}
