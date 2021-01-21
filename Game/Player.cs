using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;

namespace IngredientRun
{
    class Player : IPhysicsObject
    {
        private Texture2D idle, FOW, FOWT;
        private float _scale = 1.5f;
        private Vector2 _pos;
        private Vector2 _FOWTPos;
        private int hp = 10;
        private Sprite FOWTSprite;
        private int speed = 5;
        GraphicsDeviceManager graphics;

        public RectangleF _overlap;

        CollisionBox _collisionBox;

        public Player(GraphicsDeviceManager graphic, Vector2 pos, PhysicsHandler collisionHandler)
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
            FOWTSprite.pos = _pos + _FOWTPos;
            Vector2 FOWPosVec = camera.WorldToScreen(FOWTSprite.pos) - mousePosition;
            FOWTSprite.Rotation = (float)((Math.Atan2(
                FOWPosVec.X,
                FOWPosVec.Y
                )));

            return _pos;

        }


        public void Load(ContentManager Content, PhysicsHandler collisionHandler, RectangleF worldBounds = new RectangleF())
        {
            idle = Content.Load<Texture2D>("chars/refugee");
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
            _FOWTPos = new Vector2(idle.Width / 2 * _scale, idle.Height / 2 * _scale);

            _pos.Y -= idle.Height * _scale;

            _collisionBox = new CollisionBox(new RectangleF(_pos,
                new Size2(idle.Bounds.Width * _scale, idle.Bounds.Height * _scale)),
                collisionHandler, onCollision, onOverlap, this, worldBounds);
            collisionHandler.AddObject("Player", _collisionBox);
        }


        public void Draw(SpriteBatch spriteBatch, bool isDebug = false)
        {

            spriteBatch.Draw(idle, _pos, null, Color.White, 0f, Vector2.Zero, _scale, SpriteEffects.None, 0.5f);
            if (!isDebug)
            {
                FOWTSprite.Draw(spriteBatch);
            }
            else
            {
                _collisionBox.Draw(spriteBatch);
            }

            // _collisionBox.Draw(spriteBatch);
            // spriteBatch.DrawRectangle(_overlap, Color.Red);
        }

        public void onCollision(CollisionInfo info)
        {
            // Debug.WriteLine("Hit");
        }

        public void onOverlap(CollisionInfo info)
        {
            // _overlap = info._overlapRect;
        }
    }
}
