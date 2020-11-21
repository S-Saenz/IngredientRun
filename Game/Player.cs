using System;
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

namespace IngredientRun
{


    class Player
    {
        private Texture2D texture, FOW, FOWT;
        private Vector2 pos = new Vector2(40, 190);
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
                (graphics.PreferredBackBufferHeight / 2)- 150), new Point(300, 300));
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
            //do movement here
            if( Keyboard.GetState().IsKeyDown(Keys.Right) || Keyboard.GetState().IsKeyDown(Keys.D) )
            {
                if (pos.X < mapMoveBorder.Right)
                {
                    pos.X += speed;
                }
                else
                {
                    mapPos.X -= speed;
                }
            }
            if ( Keyboard.GetState().IsKeyDown(Keys.Left) || Keyboard.GetState().IsKeyDown(Keys.A) )
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
            if ( Keyboard.GetState().IsKeyDown(Keys.Up) || Keyboard.GetState().IsKeyDown(Keys.W) )
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
            if ( Keyboard.GetState().IsKeyDown(Keys.Down) || Keyboard.GetState().IsKeyDown(Keys.S) )
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
            return mapPos;
            /*
            //overlap
            if (Player.Intersects(Ingredient))
            {
                isColliding = true;
            else
                isColliding = false;
            }
            //if overlap pick up and ingredient is invisible     
            if (Keyboard.GetState().IsKeyDown(Keys.Space && bool isColliding = true))
              this.ingredient.Visible = false;
            */
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
                Scale = 1f,
                Origin = new Vector2(FOWT.Bounds.Center.X, FOWT.Bounds.Center.Y),
                Depth = 0.1f
            };
        }


        public void Draw(SpriteBatch spriteBatch)
        {

            spriteBatch.Draw(texture, pos, null, Color.White, 0f, new Vector2(texture.Bounds.Center.X, texture.Bounds.Center.Y), 0.8f, SpriteEffects.None, 0.5f);
            FOWTSprite.Draw(spriteBatch);

            //spriteBatch.Draw(myTexture, position, null, Color.White, rotation, origin, scale, SpriteEffects.FlipHorizontally, layer);


        }


        bool RectCollision(Vector2 objPos, Rectangle rect)
        {
             if (objPos.X > rect.Left && objPos.X < rect.Right && objPos.Y > rect.Top && objPos.Y < rect.Bottom)
             {
                 return true;
             }
            return false;
        }
    }


}
