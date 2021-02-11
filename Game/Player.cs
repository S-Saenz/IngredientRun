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
        private Texture2D idleTex, runRightTex, runLeftTex, walkRightTex, walkLeftTex, FOW, FOWT;
        private Animation runRightAnimation, runLeftAnimation, walkRightAnimation, walkLeftAnimation, idleAnimation;
        private Vector2 _FOWTPos;
        private int hp = 25;
        private Sprite FOWTSprite;
        private int _runSpeed = 120; // maximum speed for player to move at
        private int _walkSpeed = 50;
        private int _walkAccel = 50;
        private int _runAccel = 100;
        private int _acceleration = 50; // rate at which player increases speed. should be the same as _walkAccel
        private float _friction = 0.6f; // rate at which player stops
        private int _jump = 13000; // force on player to move upward
        GraphicsDeviceManager graphics;
        private bool _jumpClicked = false;
        public RectangleF _overlap;
        CollisionBox _collisionBox;
        public bool _isDark = false;
        public bool _inAir = false;
        public bool _isWalking = false;
        //private InputManager input = new InputManager();

        public Player(GraphicsDeviceManager graphic, Vector2 pos, PhysicsHandler collisionHandler) : base(new Dictionary<string, Animation>(), "player", Vector2 .Zero)
        {
            graphics = graphic;
            _pos = pos;

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
            if (Game1.instance.input.IsDown("right"))
            {
                _collisionBox.Accelerate(new Vector2(_acceleration, 0));
                if (Math.Abs(_collisionBox._velocity.X) > _walkSpeed+1)
                {
                    currentAnimation = "runRight";
                }
                else
                {
                    currentAnimation = "walkRight";
                }
            }
            if (Game1.instance.input.IsDown("left"))
            {
                _collisionBox.Accelerate(new Vector2(-_acceleration, 0));
                if (Math.Abs(_collisionBox._velocity.X) > _walkSpeed+1)
                {
                    currentAnimation = "runLeft";
                }
                else
                {
                    currentAnimation = "walkLeft";
                }
            }
            Debug.WriteLine(_collisionBox._velocity.X);
            Debug.WriteLine(_collisionBox._acceleration.X);
            if (((!Game1.instance.input.IsDown("right") && _collisionBox._velocity.X > 0) ||
               (!Game1.instance.input.IsDown("left") && _collisionBox._velocity.X < 0)) && _collisionBox._downBlocked)
            {
                currentAnimation = "idle";
                _collisionBox._acceleration.X = 0;
            }
            if (Game1.instance.input.IsDown("jump"))
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
            if(Game1.instance.input.IsDown("run"))
            {
                _collisionBox._maxSpeed.X = _runSpeed;
                _acceleration = _runAccel;
            }
            else
            {
                _collisionBox._maxSpeed.X = _walkSpeed;
                _acceleration = _walkAccel;
            }
            
            if (Game1.instance.input.JustPressed("interact"))
            {
                foreach(CollisionInfo item in _collisionBox.IsOverlapping())
                {
                    PickupItem obj = item._other as PickupItem;
                    if(obj != null)
                    {
                        Debug.WriteLine(obj._name);
                        // TODO: try adding to inventory, returning whether successful or not
                        if(true)
                        {
                            obj._spawn.Despawn();
                        }
                    }
                }
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
            walkRightTex = Content.Load<Texture2D>("animations/character_1_walk_right");
            walkRightAnimation = new Animation(walkRightTex, 1, 12, 50);
            runLeftTex = Content.Load<Texture2D>("animations/main_character_run_left");
            runLeftAnimation = new Animation(runLeftTex, 1, 10, 50);
            walkLeftTex = Content.Load<Texture2D>("animations/character_1_walk_left");
            walkLeftAnimation = new Animation(walkLeftTex, 1, 12, 50);

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
            _FOWTPos = new Vector2(0,0);

            _pos.Y -= idleTex.Height * _scale;

            _pos.Y -= idleTex.Height * _scale / 2;

            //create list of Animations
            animationDict.Add("idle", idleAnimation);
            animationDict.Add("runRight", runRightAnimation);
            animationDict.Add("runLeft", runLeftAnimation);
            animationDict.Add("walkRight", walkRightAnimation);
            animationDict.Add("walkLeft", walkLeftAnimation);

            // Add collision box
            _collisionBox = new CollisionBox(new RectangleF(_pos,
                new Size2(idleTex.Bounds.Width * _scale, idleTex.Bounds.Height * _scale)),
                collisionHandler, this, worldBounds, maxSpeed: new Vector2(_walkSpeed, 500),
                friction: _friction);
            _collisionBox.AddMovementStartListener(onStartMove);
            _collisionBox.AddMovementEndListener(onEndMove);
            collisionHandler.AddObject("Player", _collisionBox);
        }


        public void Draw(SpriteBatch spriteBatch, bool isDebug = false)
        {
            base.Draw(spriteBatch);

            if (isDebug)
            {
                _collisionBox.Draw(spriteBatch);
            }
            else if (_isDark)
            {
                // Draw light
                FOWTSprite.Draw(spriteBatch);
            }
        }

        public bool RemoveCollision(PhysicsHandler collisionHandler)
        {
            return collisionHandler.RemoveObject(_collisionBox);
        }

        public void onStartMove(Vector2 move)
        {
            // Debug.WriteLine("Start");
            if (_collisionBox._downBlocked)
                _isWalking = true;
        }
        
        public void onEndMove(Vector2 move)
        {
            // Debug.WriteLine("Stop");
            _isWalking = false;
        }
    }
}
