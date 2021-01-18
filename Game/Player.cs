﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using IngredientRun.Game;

namespace IngredientRun
{


    public class Player : BaseCharacter
    {
<<<<<<< Updated upstream
        private Texture2D idle, runRight, FOW, FOWT;
        private Animation runRightAnimation;
        private Vector2 pos = new Vector2(40, 190);
=======
        private Texture2D texture, runRight, FOW, FOWT;
        private Animation runRightAnimation;
        private float _scale = 1.5f;
        private Vector2 _pos;
>>>>>>> Stashed changes
        private int hp = 10;
        private Sprite FOWTSprite;
        private int speed = 5;
        Rectangle mapMoveBorder;
        Vector2 mapPos = Vector2.Zero;
        GraphicsDeviceManager graphics;

        public Player(GraphicsDeviceManager graphic)
        {
            graphics = graphic;
            mapMoveBorder = new Rectangle(new Point((graphics.PreferredBackBufferWidth / 2) - 150,
                (graphics.PreferredBackBufferHeight / 2) - 150), new Point(300, 300));

        }

        public Vector2 GetPos()
        {
            return pos;
        }

        public void Shake()
        {
            pos.X += 10;
            pos.Y += 10;
        }

        public void DoDMG(int dmg)
        {
            hp -= dmg;
        }

        public Vector2 Update( MouseState mouseState, KeyboardState keyState)
        {
            //Movement
            if (Keyboard.GetState().IsKeyDown(Keys.Right) || Keyboard.GetState().IsKeyDown(Keys.D))
            {
                runRightAnimation.Update();
<<<<<<< Updated upstream
                if (pos.X < mapMoveBorder.Right)
                {
                    pos.X += speed;
                }
                else
                {
                    mapPos.X -= speed;
                }
=======
                _pos.X += speed;
>>>>>>> Stashed changes
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Left) || Keyboard.GetState().IsKeyDown(Keys.A))
            {
                if (pos.X > mapMoveBorder.Left)
                {
                    pos.X -= speed;
                }
                else
                {
                    mapPos.X += speed;
                }
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Up) || Keyboard.GetState().IsKeyDown(Keys.W))
            {
                if (pos.Y > mapMoveBorder.Top)
                {
                    pos.Y -= speed;
                }
                else
                {
                    mapPos.Y += speed;
                }
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Down) || Keyboard.GetState().IsKeyDown(Keys.S))
            {
                if (pos.Y < mapMoveBorder.Bottom)
                {
                    pos.Y += speed;
                }
                else
                {
                    mapPos.Y -= speed;
                }
            }

            Vector2 mousePosition = new Vector2(mouseState.X, mouseState.Y);
            FOWTSprite.pos = pos;
            Vector2 FOWPosVec = pos - mousePosition;
            FOWTSprite.Rotation = (float)((Math.Atan2(
                FOWPosVec.X,
                FOWPosVec.Y
                )));


            //overlap
            /*if (RectCollision(pos,))
            {
                isColliding = true;
            else
                isColliding = false;
            }
            //if overlap pick up and ingredient is invisible     
            if (Keyboard.GetState().IsKeyDown(Keys.Space && bool isColliding = true))
              this.ingredient.Visible = false;
            */

            return mapPos;


        }


        public void Load(ContentManager Content)
        {
<<<<<<< Updated upstream
            idle = Content.Load<Texture2D>("chars/refugee");
            runRight = Content.Load<Texture2D>("animations/main_character_run_right");
=======
            texture = Content.Load<Texture2D>("chars/refugee");
            runRight = Content.Load<Texture2D>("chars/runRight");
            runRightAnimation = new Animation(runRight, 1, 11);
>>>>>>> Stashed changes
            FOW = Content.Load<Texture2D>("ui/visionFade");
            FOWT = Content.Load<Texture2D>("ui/visionFadeTriangle");
            FOWTSprite = new Sprite(FOWT)
            {
                pos = new Vector2(100, 100),
                Color = Color.White,
                Rotation = 0f,
                Scale = 1f,
                Origin = new Vector2(FOWT.Bounds.Center.X, FOWT.Bounds.Center.Y),
                Depth = 0.1f
            };
            runRightAnimation = new Animation(runRight, 1, 11);
        }


        public void Draw(SpriteBatch spriteBatch)
        {

            spriteBatch.Draw(texture, pos, null, Color.White, 0f, new Vector2(texture.Bounds.Center.X, texture.Bounds.Center.Y), 0.8f, SpriteEffects.None, 0.5f);
            FOWTSprite.Draw(spriteBatch);

            //spriteBatch.Draw(myTexture, position, null, Color.White, rotation, origin, scale, SpriteEffects.FlipHorizontally, layer);


        }


        public bool RectCollision(Rectangle rect)
        {
            if (pos.X > rect.Left && pos.X < rect.Right && pos.Y > rect.Top && pos.Y < rect.Bottom)
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
