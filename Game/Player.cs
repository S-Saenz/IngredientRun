﻿using System;

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

        public Player(GraphicsDeviceManager graphic, Vector2 pos)
        {
            graphics = graphic;
            _pos = pos;
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
            if (Keyboard.GetState().IsKeyDown(Keys.Right) || Keyboard.GetState().IsKeyDown(Keys.D))
            {
                _pos.X += speed;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Left) || Keyboard.GetState().IsKeyDown(Keys.A))
            {
                _pos.X -= speed;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Up) || Keyboard.GetState().IsKeyDown(Keys.W))
            {
                _pos.Y -= speed;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Down) || Keyboard.GetState().IsKeyDown(Keys.S))
            {
                _pos.Y += speed;
            }

            Vector2 mousePosition = new Vector2(mouseState.X, mouseState.Y);
            FOWTSprite.pos = _pos;
            Vector2 FOWPosVec = camera.WorldToScreen(_pos) - mousePosition;
            FOWTSprite.Rotation = (float)((Math.Atan2(
                FOWPosVec.X,
                FOWPosVec.Y
                )));

            return _pos;

        }


        public void Load(ContentManager Content)
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

            _pos.Y -= texture.Height * _scale / 2;
        }


        public void Draw(SpriteBatch spriteBatch)
        {

            spriteBatch.Draw(texture, _pos, null, Color.White, 0f, new Vector2(texture.Bounds.Center.X, texture.Bounds.Center.Y), _scale, SpriteEffects.None, 0.5f);
            FOWTSprite.Draw(spriteBatch);

        }


        public bool RectCollision(Rectangle rect)
        {
            if (_pos.X > rect.Left && _pos.X < rect.Right && _pos.Y > rect.Top && _pos.Y < rect.Bottom)
            {
                return true;
            }
            return false;
        }

        bool RectRectCollision(Rectangle rect1, Rectangle rect2)
        {
            if (rect1.Right > rect2.Left && rect1.Right < rect2.Right && rect1.Top > rect2.Top && rect1.Bottom < rect2.Bottom)
            {
                return true;
            }
            return false;
        }
    }


}
