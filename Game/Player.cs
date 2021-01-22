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
using MonoGame.Extended;

namespace IngredientRun
{


    class Player : AnimatedObject
    {
        private Texture2D idle, runRight, runLeft, FOW, FOWT;
        private Animation runRightAnimation, runLeftAnimation, idleAnimation;
        private float _scale = 1.5f;
        //private Vector2 _pos;
        private int hp = 10;
        private Sprite FOWTSprite;
        private int speed = 5;
        Rectangle mapMoveBorder;
        GraphicsDeviceManager graphics;

        public Player(GraphicsDeviceManager graphic, Vector2 pos) : base(new List<Animation>(), "player", Vector2 .Zero)
        {
            
            graphics = graphic;
            mapMoveBorder = new Rectangle(new Point((graphics.PreferredBackBufferWidth / 2) - 150,
                (graphics.PreferredBackBufferHeight / 2) - 150), new Point(300, 300));
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

        public Vector2 Update( MouseState mouseState, KeyboardState keyState, in OrthographicCamera camera, GameTime gameTime)
        {
            base.Update(gameTime);
            //Movement
            if (Keyboard.GetState().IsKeyDown(Keys.Right) || Keyboard.GetState().IsKeyDown(Keys.D))
            {
                currentAnimation = 1;
                _pos.X += speed;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Left) || Keyboard.GetState().IsKeyDown(Keys.A))
            {
                currentAnimation = 2;
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

            return _pos;
        }


        public void Load(ContentManager Content)
        {
            idle = Content.Load<Texture2D>("chars/refugee");
            idleAnimation = new Animation(idle, 1, 1, 0);
            runRight = Content.Load<Texture2D>("animations/main_character_run_right");
            runRightAnimation = new Animation(runRight, 1, 10, 50);
            runLeft = Content.Load<Texture2D>("animations/main_character_run_left");
            runLeftAnimation = new Animation(runLeft, 1, 10, 50);

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

            _pos.Y -= idle.Height * _scale / 2;

            //create list of Animations
            animationList.Add(idleAnimation);//index 0
            animationList.Add(runRightAnimation);//index 1
            animationList.Add(runLeftAnimation);//index 2
            
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            //draws the light
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
