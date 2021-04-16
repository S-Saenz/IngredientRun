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
        private int _jump = 11500; // force on player to move upward
        GraphicsDeviceManager graphics;
        private bool _jumpClicked = false;
        public RectangleF _overlap;
        CollisionBox _collisionBox;
        public bool _isDark = false;
        public bool _inAir = false;
        string _currentDirection = "";
        string _currentMoveType = "idle";
        public bool _isRunning = false;
        public Vector2? _anchorPoint = null; // in world
        public bool _grabLeft = false;   // which side player currently grabbing in (should probably combine with _currentDirection at some point
        public float _grabDist = 10f; // amount of top of hit box used for grab
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
            if (Game1.instance.input.IsDown("right") && !_anchorPoint.HasValue)
            {
                _collisionBox.TryMoveHorizontal(_currSpeed);
            }
            if (Game1.instance.input.IsDown("left") && !_anchorPoint.HasValue)
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
            // movement sound
            
            if (_collisionBox._downBlocked && Game1.instance.input.IsDown("run") && (Game1.instance.input.IsDown("left") || Game1.instance.input.IsDown("right")))
            {
                Game1.instance.sounds.runSound(gameTime);
            } else if (_collisionBox._downBlocked && (Game1.instance.input.IsDown("left") || Game1.instance.input.IsDown("right")))
            {
                Game1.instance.sounds.walkSound(gameTime);

            }

            if (Game1.instance.input.IsDown("jump"))
            {
                if (!_jumpClicked && !_anchorPoint.HasValue &&(_collisionBox._downBlocked || _collisionBox.HangTime(gameTime)))
                {
                    _collisionBox._velocity.Y -= _jump * gameTime.GetElapsedSeconds();
                }
                _jumpClicked = true;
                _collisionBox._downLastBlocked = float.NegativeInfinity;
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

            // Ledge grab controls
            if (!_anchorPoint.HasValue)
            {
                CheckForLedgeGrab(gameTime);
            }
            else
            {
                if (Game1.instance.input.JustPressed("down") ||
                    (_grabLeft && Game1.instance.input.JustPressed("right")) ||
                    (!_grabLeft && Game1.instance.input.JustPressed("left")))
                {
                    _collisionBox._posLock = false;
                    _collisionBox._hasGravity = true;
                    _anchorPoint = null;
                }
                else if (Game1.instance.input.JustPressed("up"))
                {
                    _collisionBox._bounds.Position = new Point2(_anchorPoint.Value.X - (_grabLeft ? _collisionBox._bounds.Width : 0),
                                                                _anchorPoint.Value.Y - _collisionBox._bounds.Height);
                    _collisionBox._posLock = false;
                    _collisionBox._hasGravity = true;
                    _anchorPoint = null;
                }
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
                            //Debug.WriteLine("Fire");
                            // Open cooking ui
                            Game1.instance.UI.SwitchState(UIState.RecipeMenu);

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

        public void DrawDebug(SpriteBatch spriteBatch)
        {
            _collisionBox.Draw(spriteBatch);
            
            // if(_collisionBox._rightBlocked)
            // {
            //     if(_collisionBox._rightBox.Top != _collisionBox._bounds.Top)
            //         Debug.WriteLine(_collisionBox._rightBox.Height + " " + _collisionBox._rightBox.Top + " " + _collisionBox._bounds.Top);
            //     foreach (CollisionInfo info in _collisionBox._rightInfo)
            //     {
            //         Debug.WriteLine("    " + (info._other as TileMap.Tile)._loc + " " + " " + info._overlapRect.Height);
            //     }
            // }
            if (_anchorPoint.HasValue)
            {
                spriteBatch.DrawPoint(_anchorPoint.Value, Color.DeepPink, 2);
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

        private void CheckForLedgeGrab(GameTime gameTime)
        {
            if(_collisionBox._velocity.Y < 0) // moving up
            {
                _anchorPoint = null;
                _collisionBox._hasGravity = true;
            }
            else if (_collisionBox._leftBlocked && Game1.instance.input.IsDown("left") && // check for side hit left grab
                _collisionBox._leftBox.Top > _collisionBox._bounds.Top &&
                _collisionBox._leftBox.Top <= _collisionBox._bounds.Top + _grabDist)
            {
                _grabLeft = true;
                _anchorPoint = _collisionBox._leftBox.TopRight;
                _collisionBox._bounds.Position = (Point2)_anchorPoint - new Vector2(0, _grabDist);
                _collisionBox._acceleration = _collisionBox._velocity = Vector2.Zero;
                _collisionBox._posLock = true;
                _collisionBox._hasGravity = false;
                Debug.WriteLine("Grabbed left");
            }
            else if (_collisionBox._rightBlocked && Game1.instance.input.IsDown("right") && // check for side hit right grab
                     _collisionBox._rightBox.Top > _collisionBox._bounds.Top &&
                     _collisionBox._rightBox.Top <= _collisionBox._bounds.Top + _grabDist)
            {
                _grabLeft = false;
                _anchorPoint = _collisionBox._rightBox.TopLeft;
                _collisionBox._bounds.Position = (Point2)_anchorPoint - new Vector2(_collisionBox._bounds.Width, _grabDist);
                _collisionBox._acceleration = _collisionBox._velocity = Vector2.Zero;
                _collisionBox._posLock = true;
                _collisionBox._hasGravity = false;
                Debug.WriteLine("Grabbed right");
            }
            else if(_collisionBox._downBlocked && Game1.instance.input.JustPressed("down") && 
                    _collisionBox._downBox.Width < _collisionBox._bounds.Width) // check for drop down
            {
                if (_collisionBox._bounds.Right - _collisionBox._downBox.Right > _collisionBox._downBox.Left - _collisionBox._bounds.Left) // down right grab left
                {
                    _grabLeft = true;
                    _anchorPoint = _collisionBox._downBox.TopRight;
                    _collisionBox._bounds.Position = (Point2)_anchorPoint - new Vector2(0, _grabDist);
                    _collisionBox._acceleration = _collisionBox._velocity = Vector2.Zero;
                    _collisionBox._posLock = true;
                    _collisionBox._hasGravity = false;
                    Debug.WriteLine("Drop grabbed left");
                }
                else // down left grab right
                {
                    _grabLeft = false;
                    _anchorPoint = _collisionBox._downBox.TopLeft;
                    _collisionBox._bounds.Position = (Point2)_anchorPoint - new Vector2(_collisionBox._bounds.Width, _grabDist);
                    _collisionBox._acceleration = _collisionBox._velocity = Vector2.Zero;
                    _collisionBox._posLock = true;
                    _collisionBox._hasGravity = false;
                    Debug.WriteLine("Drop grabbed right");
                }
            }
            else
            {
                _anchorPoint = null;
                _collisionBox._hasGravity = true;
            }
        }

        public void LockPos()
        {
            _collisionBox._posLock = true;
        }

        public void UnlockPos()
        {
            _collisionBox._posLock = false;
        }
    }
}
