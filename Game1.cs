using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Windows.Forms.VisualStyles;
using System.Diagnostics;

using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;

//hi
//123
namespace IngredientRun
{
    public class Game1 : Game
    {
        Player player;
        Enemy enemy1;

        TileMap caveMapBackground;

        Vector2 bgPos;
        Vector2 screenDimensions;

        //classes
        Inventory inventory = new Inventory();

        PickUpable pickUp1;

        // Debug mode
        bool _isDebug = false;
        bool _ctrlPrevDown = false;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private PhysicsHandler _collisionHandler;

        private OrthographicCamera _camera;

        public Game1()
        {
            this.Window.Title = "Ingredient Time";
            this.IsMouseVisible = true;
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            _graphics.GraphicsProfile = GraphicsProfile.HiDef;
            screenDimensions = new Vector2(1728, 972);

            _collisionHandler = new PhysicsHandler();

            
            _graphics.PreferredBackBufferWidth = (int)screenDimensions.X;  // set this value to the desired width of your window
            _graphics.PreferredBackBufferHeight = (int)screenDimensions.Y;   // set this value to the desired height of your window
            _graphics.ApplyChanges();

            // Set start location
            bgPos = new Vector2(0, 0);

            // Set up camera and viewport
            DefaultViewportAdapter viewportAdapter = new DefaultViewportAdapter(GraphicsDevice);
            _camera = new OrthographicCamera(viewportAdapter);
            _camera.Zoom = 4;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            //backgrounds
            // caveMapBackground = new TileMap("tilemaps/prototype/MapPrototypeTiledCollider", Content, GraphicsDevice);
            caveMapBackground = new TileMap("tilemaps/prototype/CollisionTestMap", Content, GraphicsDevice, _collisionHandler);

            // pickup
            pickUp1 = new PickUpable(Content.Load<Texture2D>("Ingredient/acorn"), caveMapBackground.GetWaypoint("ItemObjects", "Acorn"));
            pickUp1.Load(Content);

            // player
            player = new Player(_graphics, caveMapBackground.GetWaypoint("PlayerObjects", "PlayerSpawn"), _collisionHandler);
            player.Load(Content, _collisionHandler, caveMapBackground._mapBounds);

            // enemy
            enemy1 = new Enemy(Content.Load<Texture2D>("monsters/monster"), caveMapBackground.GetWaypoint("EnemyObjects", "EnemySpawn"));
            enemy1.Load(Content);

            //class loads
            inventory.Load(Content);

        }

        protected override void Update(GameTime gameTime)
        {
            //Debug.WriteLine();
            // Print collision boxes, remove FOWT sprite
            if(Keyboard.GetState().IsKeyDown(Keys.LeftControl) && !_ctrlPrevDown)
            {
                _isDebug = !_isDebug;
                _ctrlPrevDown = true;
            }
            else if(!Keyboard.GetState().IsKeyDown(Keys.LeftControl))
            {
                _ctrlPrevDown = false;
            }

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            inventory.Update(Mouse.GetState() ,Keyboard.GetState());

            Matrix projectionMatrix = Matrix.CreateOrthographicOffCenter(0, screenDimensions.X, screenDimensions.Y, 0, 1, 0);
            bgPos = player.Update(Mouse.GetState(), Keyboard.GetState(), _camera) - screenDimensions / 2;
            _camera.Position = bgPos;
            // pickUp1.Update(bgPos);
            enemy1.Update(bgPos);

            caveMapBackground.Update(gameTime);


            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            Matrix projectionMatrix = Matrix.CreateOrthographicOffCenter(0, screenDimensions.X, screenDimensions.Y, 0, 1, 0);

            // Draw tilemap background
            caveMapBackground.Draw(_spriteBatch, _camera.GetViewMatrix(), projectionMatrix, _isDebug);

            // Draw sprites
            _spriteBatch.Begin(transformMatrix: _camera.GetViewMatrix(), sortMode: SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp);
            
            enemy1.Draw(_spriteBatch);
            pickUp1.Draw(_spriteBatch);
            player.Draw(_spriteBatch, _isDebug);

            //class draws

            if(inventory.showInv)
                inventory.Draw(_spriteBatch);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
