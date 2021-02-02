using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;

namespace IngredientRun
{


    class Player : AnimatedObject,  IPhysicsObject
    {
        private Texture2D idleTex, runRightTex, runLeftTex, FOW, FOWT;
        private Animation runRightAnimation, runLeftAnimation, idleAnimation;
        private Vector2 _FOWTPos;
        private int hp = 25;
        private Sprite FOWTSprite;
        private int _runSpeed = 120; // maximum speed for player to move at
        private int _walkSpeed = 50;
        private int _acceleration = 90; // rate at which player increases speed
        private float _friction = 0.6f; // rate at which player stops
        private int _jump = 9000; // force on player to move upward
        GraphicsDeviceManager graphics;
        private bool _jumpClicked = false;
        public RectangleF _overlap;
        CollisionBox _collisionBox;

        public Player(GraphicsDeviceManager graphic, Vector2 pos, PhysicsHandler collisionHandler) : base(new Dictionary<string, Animation>(), "player", Vector2 .Zero)
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

        public Vector2 Update( MouseState mouseState, KeyboardState keyState, in OrthographicCamera camera, GameTime gameTime)
        {
            base.Update(gameTime);
            //Movement
            if (Keyboard.GetState().IsKeyDown(Keys.Right) || Keyboard.GetState().IsKeyDown(Keys.D))
            {
                _collisionBox.Accelerate(new Vector2(_acceleration, 0));
                currentAnimation = "runRight";
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Left) || Keyboard.GetState().IsKeyDown(Keys.A))
            {
                _collisionBox.Accelerate(new Vector2(-_acceleration, 0));
                currentAnimation = "runLeft";
            }
            if((!Keyboard.GetState().IsKeyDown(Keys.Right) && !Keyboard.GetState().IsKeyDown(Keys.D) && _collisionBox._velocity.X > 0) ||
               (!Keyboard.GetState().IsKeyDown(Keys.Left) && !Keyboard.GetState().IsKeyDown(Keys.A) && _collisionBox._velocity.X < 0))
            {
                _collisionBox._acceleration.X = 0;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Up) || Keyboard.GetState().IsKeyDown(Keys.W))
            {
                if (_collisionBox._downBlocked && !_jumpClicked)
                {
                    _collisionBox._velocity.Y -= _jump * gameTime.GetElapsedSeconds();
                }
                _jumpClicked = true;
            }
            else
            {
                _jumpClicked = false;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Down) || Keyboard.GetState().IsKeyDown(Keys.S))
            {
                // pos.Y += _speed * gameTime.GetElapsedSeconds();
            }
            _pos = _collisionBox.Update(gameTime) + new Vector2(_collisionBox._bounds.Width / 2, _collisionBox._bounds.Height / 2);

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
            idleTex = Content.Load<Texture2D>("chars/refugee");
            idleAnimation = new Animation(idleTex, 1, 1, 0);
            runRightTex = Content.Load<Texture2D>("animations/main_character_run_right");
            runRightAnimation = new Animation(runRightTex, 1, 10, 50);
            runLeftTex = Content.Load<Texture2D>("animations/main_character_run_left");
            runLeftAnimation = new Animation(runLeftTex, 1, 10, 50);

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
            _FOWTPos = new Vector2(0,0);

            _pos.Y -= idleTex.Height * _scale;

            _pos.Y -= idleTex.Height * _scale / 2;

            //create list of Animations
            animationDict.Add("idle", idleAnimation);
            animationDict.Add("runRight", runRightAnimation);
            animationDict.Add("runLeft", runLeftAnimation);
            
            // Add collision box
            _collisionBox = new CollisionBox(new RectangleF(_pos,
                new Size2(idleTex.Bounds.Width * _scale, idleTex.Bounds.Height * _scale)),
                collisionHandler, this, worldBounds, maxSpeed: new Vector2(_runSpeed, 500),
                friction: _friction);
            _collisionBox.AddMovementStartListener(onStartMove);
            _collisionBox.AddMovementEndListener(onEndMove);
            collisionHandler.AddObject("Player", _collisionBox);
        }


        public void Draw(SpriteBatch spriteBatch, bool isDebug = false)
        {
            base.Draw(spriteBatch);

            if (!isDebug)
            {
                // Draw light
                FOWTSprite.Draw(spriteBatch);
            }
            else
            {
                _collisionBox.Draw(spriteBatch);
            }
        }

        public void onStartMove(Vector2 move)
        {
            // Debug.WriteLine("Start");
        }
        
        public void onEndMove(Vector2 move)
        {
            // Debug.WriteLine("Stop");
        }
    }
}
