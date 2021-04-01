using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;

namespace WillowWoodRefuge
{
    class Player : AnimatedObject,  IPhysicsObject
    {
        private Texture2D idleTex, runRightTex, runLeftTex, walkRightTex, walkLeftTex, jumpRightTex, FOW, FOWT;
        private Animation runRightAnimation, runLeftAnimation, walkRightAnimation, walkLeftAnimation, jumpRightAnimation, idleAnimation;
        private Vector2 _FOWTPos;
        private int hp = 25;
        private Sprite FOWTSprite;
        private int _runSpeed = 140; // maximum speed for player to move at
        private int _walkSpeed = 50;
        private int _currSpeed = 0;
        private int _walkAccel = 50;
        private int _runAccel = 100;
        private int _acceleration = 50; // rate at which player increases speed. should be the same as _walkAccel
        private float _friction = 0.5f; // rate at which player stops
        private int _jump = 13000; // force on player to move upward
        GraphicsDeviceManager graphics;
        private bool _jumpClicked = false;
        public RectangleF _overlap;
        CollisionBox _collisionBox;
        public bool _isDark = false;
        public bool _inAir = false;
        string _currentDirection = "";
        string _currentMoveType = "idle";
        public bool _isRunning = false;
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
            //Movement
            if (Game1.instance.input.IsDown("right"))
            {
                _collisionBox.TryMoveHorizontal(_currSpeed);
            }
            if (Game1.instance.input.IsDown("left"))
            {
                _collisionBox.TryMoveHorizontal(-_currSpeed);
            }
            // Debug.WriteLine(_collisionBox._velocity.X);
            // Debug.WriteLine(_collisionBox._acceleration.X);
            if (((!Game1.instance.input.IsDown("right") && _collisionBox._velocity.X > 0) ||
               (!Game1.instance.input.IsDown("left") && _collisionBox._velocity.X < 0)) && _collisionBox._downBlocked)
            {
                _collisionBox.TryMoveHorizontal(0);
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
                _currSpeed = _runSpeed;
            }
            else
            {
                _currSpeed = _walkSpeed;
            }

            

            if (Game1.instance.input.JustPressed("interact"))
            {
                foreach(CollisionInfo item in _collisionBox.IsOverlapping())
                {
                    // check if pickup item
                    PickupItem obj = item._other as PickupItem;
                    if(obj != null)
                    {
                        Debug.WriteLine(obj._name);
                        // TODO: try adding to inventory, returning whether successful or not
                        if(Game1.instance.inventory.addIngredient(null, obj._name))
                        {
                            (Game1.instance._currentState as GameplayState)._items.Remove(obj);
                            obj._spawn.Despawn();
                        }
                    }

                    // check if area
                    Area area = item._other as Area;
                    if(area != null)
                    {
                        if(area._name == "fire")
                        {
                            Debug.WriteLine("Fire");
                            // Open cooking ui
                        }
                        else if(area._name.Contains("state"))
                        {
                            if(area._name.Contains("Cave"))
                            {
                                Game1.instance.RequestStateChange("CaveState");
                            }
                            else if(area._name.Contains("Camp"))
                            {
                                Game1.instance.RequestStateChange("CampState");
                            }
                        }
                    }
                }
            }

            _pos = _collisionBox.Update(gameTime) + new Vector2(_collisionBox._bounds.Width / 2, _collisionBox._bounds.Height / 2);

            // update animation type
            UpdateAnimationInfo();

            Vector2 mousePosition = new Vector2(mouseState.X, mouseState.Y);
            FOWTSprite.pos = _pos + _FOWTPos;
            Vector2 FOWPosVec = camera.WorldToScreen(FOWTSprite.pos) - mousePosition;
            FOWTSprite.Rotation = 0 - (float)((Math.Atan2(
                FOWPosVec.X,
                FOWPosVec.Y
                ))) - 1.5f;

            base.Update(gameTime);

            return FOWPosVec;
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
            jumpRightTex = Content.Load<Texture2D>("animations/main_character_jump_right");
            jumpRightAnimation = new Animation(jumpRightTex, 1, 11, 50);


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


            //create list of Animations
            animationDict.Add("idle", idleAnimation);
            animationDict.Add("runRight", runRightAnimation);
            animationDict.Add("runLeft", runLeftAnimation);
            animationDict.Add("walkRight", walkRightAnimation);
            animationDict.Add("walkLeft", walkLeftAnimation);
            animationDict.Add("jumpRight", jumpRightAnimation);
            animationDict.Add("jump", jumpRightAnimation);
            animationDict.Add("jumpLeft", jumpRightAnimation);

            // Add collision box
            _collisionBox = new CollisionBox(new RectangleF(_pos,
                new Size2(idleTex.Bounds.Width * _scale, idleTex.Bounds.Height * _scale)),
                collisionHandler, this, worldBounds, maxSpeed: new Vector2(_runSpeed, 500),
                friction: _friction);
            _collisionBox.AddMovementStartListener(onStartMove);
            _collisionBox.AddMovementChangeDirectionListener(onChangeDirection);
            collisionHandler.AddObject("Player", _collisionBox);
            _pos = _collisionBox._bounds.Center;
        }


        public void Draw(SpriteBatch spriteBatch, bool isDebug = false)
        {
            base.Draw(spriteBatch);

            if (_isDark && !isDebug)
            {
                // Draw light
                FOWTSprite.Draw(spriteBatch);
            }
        }

        public bool RemoveCollision(PhysicsHandler collisionHandler)
        {
            return collisionHandler.RemoveObject(_collisionBox);
        }

        private void UpdateAnimationInfo()
        {
            if (_collisionBox._velocity.X == 0) // stopped
            {
                _currentMoveType = "idle";
            }
            else if (Math.Abs(_collisionBox._velocity.X) > _walkSpeed + 1 && _collisionBox._downBlocked) // if running
            {
                _currentMoveType = "run";
            }
            else if (Math.Abs(_collisionBox._velocity.X) < _walkSpeed + 1 && _collisionBox._downBlocked) // if walking
            {
                _currentMoveType = "walk";
            }
            else if (!_collisionBox._downBlocked) // if walking
            {
                _currentMoveType = "jump";
            }
            currentAnimation = _currentMoveType + _currentDirection;
        }

        public void onStartMove(Vector2 move)
        {
            // Debug.WriteLine("Start");

            if (move.X > 0) // moving right
            {
                _currentDirection = "Right";
            }
            else if(move.X < 0) // moving left
            {
                _currentDirection = "Left";
            }
            else if (move.X == 0) // horizontal movement stopped
            {
                _currentDirection = "";
            }

            if (move.X != 0) // moving horizontally
            {
                if (_collisionBox._downBlocked)
                    _isRunning = true;
            }
        }
        
        public void onChangeDirection(Vector2 move)
        {
            if (move.X > 0) // moving right
            {
                _currentDirection = "Right";
            }
            else if (move.X < 0) // moving left
            {
                _currentDirection = "Left";
            }
            else if (move.X == 0) // horizontal movement stopped
            {
                _currentDirection = "";
            }            
        }

        public void Reset()
        {
            Game1.instance.RequestStateChange(Game1.instance._currentStateName);
        }
    }
}
