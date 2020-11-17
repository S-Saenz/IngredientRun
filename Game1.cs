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
        Texture2D caveBG;
        Texture2D chara1;
        Texture2D chara2;
        Texture2D chara3;
        Texture2D chara4;

        Player player;
        Enemy enemy1;





        Vector2 refugeePos;
        Vector2 chara1Pos;
        Vector2 chara2Pos;
        Vector2 chara3Pos;
        Vector2 chara4Pos;
        Vector2 bgPos = new Vector2(0,0);

        //classes
        Inventory inventory = new Inventory();


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
            //positions of characters
            refugeePos = new Vector2(40, 60 );

            chara1Pos = new Vector2((_graphics.PreferredBackBufferWidth / 2) + 310, 800);
            chara2Pos = new Vector2((_graphics.PreferredBackBufferWidth / 2) + 240, 800);
            chara3Pos = new Vector2((_graphics.PreferredBackBufferWidth / 2) + 170, 800);
            chara4Pos = new Vector2((_graphics.PreferredBackBufferWidth / 2) + 100, 800);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            //backgrounds
            background = Content.Load<Texture2D>("bg/Ingredient Run Camp");
            caveBG = Content.Load<Texture2D>("bg/caveMapPlan");
            //player
            refugee = Content.Load<Texture2D>("chars/refugee");
            //characters
            chara1 = Content.Load<Texture2D>("chars/chara1");
            chara2 = Content.Load<Texture2D>("chars/chara2");
            chara3 = Content.Load<Texture2D>("chars/chara3");
            chara4 = Content.Load<Texture2D>("chars/chara4");
            enemy1 =  new Enemy(Content.Load<Texture2D>("monsters/monster"));

            player = new Player(_graphics);
            player.Load(Content);

            //class loads
            inventory.Load(Content);

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            inventory.Update(Mouse.GetState() ,Keyboard.GetState());

            bgPos = player.Update(Mouse.GetState(), Keyboard.GetState());

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            _spriteBatch.Begin();
            _spriteBatch.Draw(caveBG, bgPos, null, Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0.9f);
            //characters 1-4
            _spriteBatch.Draw(chara1, chara1Pos, null, Color.White, 0f, Vector2.Zero, 0.3f, SpriteEffects.None, 0f);
            _spriteBatch.Draw(chara2, chara2Pos, null, Color.White, 0f, Vector2.Zero, 0.3f, SpriteEffects.None, 0f);
            _spriteBatch.Draw(chara3, chara3Pos, null, Color.White, 0f, Vector2.Zero, 0.3f, SpriteEffects.None, 0f);
            _spriteBatch.Draw(chara4, chara4Pos, null, Color.White, 0f, Vector2.Zero, 0.3f, SpriteEffects.None, 0f);
            //player
            //_spriteBatch.Draw(refugee, refugeePos,null, Color.White, 0f, Vector2.Zero, 0.8f, SpriteEffects.None, 0f);
            enemy1.Draw(_spriteBatch);
            player.Draw(_spriteBatch);
            //spriteBatch.Draw(texture, position, null, Color.White, 0f, 
            //Vector2.Zero, 0.5f, SpriteEffects.None, 0f);

            //class draws

            if(inventory.showInv)
                inventory.Draw(_spriteBatch);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
