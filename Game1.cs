using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Windows.Forms.VisualStyles;
//hi
//123
namespace IngredientRun
{
    public class Game1 : Game
    {
        Texture2D refugee;
        Texture2D background;

        Vector2 refugeePos;
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public Game1()
        {
            this.Window.Title = "Ingredient Time";
            this.IsMouseVisible = true;
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            _graphics.GraphicsProfile = GraphicsProfile.HiDef;


            _graphics.PreferredBackBufferWidth = 1728;//1241;  // set this value to the desired width of your window
            _graphics.PreferredBackBufferHeight = 972;   // set this value to the desired height of your window
            _graphics.ApplyChanges();

        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            refugeePos = new Vector2(_graphics.PreferredBackBufferWidth / 2,
            _graphics.PreferredBackBufferHeight / 2);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            background = Content.Load<Texture2D>("bg/Ingredient Run Camp");
            refugee = Content.Load<Texture2D>("chars/refugee");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            _spriteBatch.Begin();
            _spriteBatch.Draw(background, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.9f);
            _spriteBatch.Draw(refugee, refugeePos,null, Color.White, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
         
            //spriteBatch.Draw(texture, position, null, Color.White, 0f, 
            //Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
