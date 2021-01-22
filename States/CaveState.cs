using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;
using Microsoft.Xna.Framework.Input;

namespace IngredientRun.States
{
    class CaveState : State
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

        // private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private PhysicsHandler _collisionHandler;

        private OrthographicCamera _camera;

        public CaveState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content)
            : base(game, graphicsDevice, content)
        {
            game.Window.Title = "Ingredient Time";
            game.IsMouseVisible = true;
            content.RootDirectory = "Content";
            Game1.instance.graphics.GraphicsProfile = GraphicsProfile.HiDef;
            screenDimensions = new Vector2(1728, 972);

            _collisionHandler = new PhysicsHandler();


            Game1.instance.graphics.PreferredBackBufferWidth = (int)screenDimensions.X;  // set this value to the desired width of your window
            Game1.instance.graphics.PreferredBackBufferHeight = (int)screenDimensions.Y;   // set this value to the desired height of your window
            Game1.instance.graphics.ApplyChanges();

            // Set start location
            bgPos = new Vector2(0, 0);

            // Set up camera and viewport
            DefaultViewportAdapter viewportAdapter = new DefaultViewportAdapter(Game1.instance.GraphicsDevice);
            _camera = new OrthographicCamera(viewportAdapter);
            _camera.Zoom = 4;
        }

        public override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(Game1.instance.GraphicsDevice);

            //backgrounds
            // caveMapBackground = new TileMap("tilemaps/prototype/MapPrototypeTiledCollider", Content, GraphicsDevice);
            caveMapBackground = new TileMap("tilemaps/prototype/CollisionTestMap", _content, Game1.instance.GraphicsDevice, _collisionHandler);

            // pickup
            pickUp1 = new PickUpable(_content.Load<Texture2D>("Ingredient/acorn"), caveMapBackground.GetWaypoint("ItemObjects", "Acorn"));
            pickUp1.Load(_content);

            // player
            player = new Player(Game1.instance.graphics, caveMapBackground.GetWaypoint("PlayerObjects", "PlayerSpawn"), _collisionHandler);
            player.Load(_content, _collisionHandler, caveMapBackground._mapBounds);

            // enemy
            enemy1 = new Enemy(_content.Load<Texture2D>("monsters/monster"), caveMapBackground.GetWaypoint("EnemyObjects", "EnemySpawn"));
            enemy1.Load(_content);

            //class loads
            inventory.Load(_content);
        }

        public override void Update(GameTime gameTime)
        {
            //Debug.WriteLine();
            // Print collision boxes, remove FOWT sprite
            if (Keyboard.GetState().IsKeyDown(Keys.LeftControl) && !_ctrlPrevDown)
            {
                _isDebug = !_isDebug;
                _ctrlPrevDown = true;
            }
            else if (!Keyboard.GetState().IsKeyDown(Keys.LeftControl))
            {
                _ctrlPrevDown = false;
            }

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                game.Exit();

            // TODO: Add your update logic here
            inventory.Update(Mouse.GetState(), Keyboard.GetState());

            Matrix projectionMatrix = Matrix.CreateOrthographicOffCenter(0, screenDimensions.X, screenDimensions.Y, 0, 1, 0);
            bgPos = player.Update(Mouse.GetState(), Keyboard.GetState(), _camera) - screenDimensions / 2;
            _camera.Position = bgPos;
             pickUp1.Update(bgPos);
            enemy1.Update(bgPos);

            caveMapBackground.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Game1.instance.GraphicsDevice.Clear(Color.Brown);

            Matrix projectionMatrix = Matrix.CreateOrthographicOffCenter(0, screenDimensions.X, screenDimensions.Y, 0, 1, 0);

            // Draw tilemap background
            caveMapBackground.Draw(_spriteBatch, _camera.GetViewMatrix(), projectionMatrix, _isDebug);

            // Draw sprites
            _spriteBatch.Begin(transformMatrix: _camera.GetViewMatrix(), sortMode: SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp);

            enemy1.Draw(_spriteBatch);
            pickUp1.Draw(_spriteBatch);
            player.Draw(_spriteBatch, _isDebug);

            //class draws

            if (inventory.showInv)
                inventory.Draw(_spriteBatch);

            _spriteBatch.End();

            //base.Draw(gameTime);
        }

        

        public override void PostUpdate(GameTime gameTime)
        {
            //  throw new NotImplementedException();
        }

        
    }
}
