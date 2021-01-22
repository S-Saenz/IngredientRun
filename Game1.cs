using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Windows.Forms.VisualStyles;
using System.Diagnostics;

using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;
using IngredientRun.States;

//hi
//123
namespace IngredientRun
{
    public class Game1 : Game
    {
        public static Game1 instance;
        public GraphicsDeviceManager graphics;
        private SpriteBatch _spriteBatch;

        // create vatiable for the state manager
       
        private State _currentState;

        private State _nextState;

        public void ChangeState(State state)
        {
            _nextState = state;
        }

       
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            //_spriteBatch = new SpriteBatch();
            instance = this;
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _currentState = new CaveState(this, graphics.GraphicsDevice, Content);

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
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _currentState.Draw(gameTime, _spriteBatch);

            base.Draw(gameTime);
        }
    }
}
