using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;

namespace IngredientRun
{


    class Player
    {
        private Texture2D texture, FOW, FOWT;
        private float _scale = 1.5f;
        private Vector2 _pos;
        private int hp = 10;
        private Sprite FOWTSprite;
        private int speed = 5;
        GraphicsDeviceManager graphics;

        public RectangleF _overlap;

        CollisionBox _collisionBox;

        public Player(GraphicsDeviceManager graphic, Vector2 pos, CollisionHandler collisionHandler)
        {
            graphics = graphic;
            _pos = pos;

            collisionHandler.AddLayer("Player");
            collisionHandler.SetOverlap("Player", "Walls");
            collisionHandler.SetCollision("Player", "Walls");

            _overlap = new RectangleF();
        }

        public Vector2 GetPos()
        {
            return _pos;
        }

        public void Shake()
        {
            _pos.X += 10;
            _pos.Y += 10;
        }

        public void DoDMG(int dmg)
        {
            hp -= dmg;
        }

        public Vector2 Update( MouseState mouseState, KeyboardState keyState, in OrthographicCamera camera)
        {
            //Movement
            Vector2 pos = _pos;
            if (Keyboard.GetState().IsKeyDown(Keys.Right) || Keyboard.GetState().IsKeyDown(Keys.D))
            {
                pos.X += speed;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Left) || Keyboard.GetState().IsKeyDown(Keys.A))
            {
                pos.X -= speed;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Up) || Keyboard.GetState().IsKeyDown(Keys.W))
            {
                pos.Y -= speed;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Down) || Keyboard.GetState().IsKeyDown(Keys.S))
            {
                pos.Y += speed;
            }
            _pos = _collisionBox.Move(pos);
            _collisionBox.Update(_pos);

            Vector2 mousePosition = new Vector2(mouseState.X, mouseState.Y);
            FOWTSprite.pos = _pos;
            Vector2 FOWPosVec = camera.WorldToScreen(_pos) - mousePosition;
            FOWTSprite.Rotation = (float)((Math.Atan2(
                FOWPosVec.X,
                FOWPosVec.Y
                )));

            return _pos;

        }


        public void Load(ContentManager Content, CollisionHandler collisionHandler)
        {
            texture = Content.Load<Texture2D>("chars/refugee");
            FOW = Content.Load<Texture2D>("ui/visionFade");
            FOWT = Content.Load<Texture2D>("ui/visionFadeTriangle");
            FOWTSprite = new Sprite(FOWT)
            {
                pos = new Vector2(100, 100),
                Color = Color.White,
                Rotation = 0f,
                Scale = .15f,
                Origin = new Vector2(FOWT.Bounds.Center.X, FOWT.Bounds.Center.Y),
                Depth = 0.1f
            };

            _pos.Y -= texture.Height * _scale;

            _collisionBox = new CollisionBox(new RectangleF(_pos,
                new Size2(texture.Bounds.Width * _scale, texture.Bounds.Height * _scale)),
                onCollision, onOverlap, collisionHandler);
            collisionHandler.AddObject("Player", _collisionBox);
        }


        public void Draw(SpriteBatch spriteBatch)
        {

            spriteBatch.Draw(texture, _pos, null, Color.White, 0f, Vector2.Zero, _scale, SpriteEffects.None, 0.5f);
            FOWTSprite.Draw(spriteBatch);

            // _collisionBox.Draw(spriteBatch);
            // spriteBatch.DrawRectangle(_overlap, Color.Red);
        }

        public void onCollision(CollisionInfo info)
        {
            Debug.WriteLine("Hit");
        }

        public void onOverlap(CollisionInfo info)
        {
            // _overlap = info._overlapRect;
        }
    }
}
