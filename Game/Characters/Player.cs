﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;

namespace WillowWoodRefuge
{
    class Player : AnimatedObject,  IPhysicsObject
    {
        private Texture2D idleTex, idleLeftTex, idleRightTex, runRightTex, runLeftTex, walkRightTex, walkLeftTex, jumpRightTex, climbRightTex, climbLeftTex, hangRightTex, hangLeftTex, FOW, FOWT;
        private Texture2D jumpSquatRightTex, jumpSquatLeftTex, risingRightTex, risingLeftTex, apexRightTex, apexLeftTex, fallingRightTex, fallingLeftTex, landingRightTex, landingLeftTex;
        private Animation runRightAnimation, runLeftAnimation, walkRightAnimation, walkLeftAnimation, jumpRightAnimation, 
                          idleAnimation, idleLeftAnimation, idleRightAnimation, climbRightAnimation, climbLeftAnimation, hangRightAnimation, hangLeftAnimation;
        private Animation jumpSquatRightAnimation, jumpSquatLeftAnimation, risingRightAnimation, risingLeftAnimation, apexRightAnimation, apexLeftAnimation, 
                            fallingRightAnimation, fallingLeftAnimation, landingRightAnimation, landingLeftAnimation;
        private bool interuptAnimationUpdate = false;
        private bool interuptInputUpdate = false;
        private bool jumpSquatLanding = false;
        private int delayFrames = 0;
        private bool landCheck = false;
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
        private int _jump = 5150; // force on player to move upward
        GraphicsDeviceManager graphics;
        private bool _jumpClicked = false;
        public RectangleF _overlap;
        CollisionBox _collisionBox;
        public bool _isDark = false;
        public bool _inAir = false;
        string _currentDirection = "Right";
        string _currentMoveType = "idle";
        public bool _isRunning = false;
        public Vector2? _anchorPoint = null; // in world
        public bool _grabLeft = false;   // which side player currently grabbing in (should probably combine with _currentDirection at some point
        public float _grabDist = 10f; // amount of top of hit box used for grab
        public float _yClearance = 16;

        private string previousMoveType;
        private string previousDirection;
        public bool _overlappingInteractable { get; private set; }
        public string _overlapName { get; private set; }
        public float _maxFallSpeed = 0;
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
            //if(delayFrames > 0)
            //{
            //    delayFrames--;
            //}
            //else if(delayFrames == 0)
            //{
            //    interuptAnimationUpdate = false;
            //    interuptInputUpdate = false;
            //}
            if(climbRightAnimation.currentFrame == climbRightAnimation.totalFrames - 1 || climbLeftAnimation.currentFrame == climbLeftAnimation.totalFrames - 1)
            {
                interuptAnimationUpdate = false;
                interuptInputUpdate = false;
                climbRightAnimation.reset();
                climbLeftAnimation.reset();
            }
            if (jumpSquatRightAnimation.currentFrame == jumpSquatRightAnimation.totalFrames - 1 || jumpSquatLeftAnimation.currentFrame == jumpSquatLeftAnimation.totalFrames - 1)
            {
                interuptAnimationUpdate = false;
                interuptInputUpdate = false;
                jumpSquatLanding = false;
                if (!_jumpClicked && !_anchorPoint.HasValue && (_collisionBox._downBlocked || _collisionBox.HangTime(gameTime)))
                {
                    _collisionBox._velocity.Y -= _jump * gameTime.GetElapsedSeconds();
                }
                _jumpClicked = true;
                _collisionBox._downLastBlocked = float.NegativeInfinity;//what does this do?
                jumpSquatRightAnimation.reset();
                jumpSquatLeftAnimation.reset();
            }
            if(_collisionBox._downBlocked && previousMoveType == "falling")
            {
                interuptAnimationUpdate = true;
                interuptInputUpdate = true;
                landingRightAnimation.reset();
                landingLeftAnimation.reset();
                currentAnimation = "landingRight";
            }
            if (landingRightAnimation.currentFrame == landingRightAnimation.totalFrames - 1)
            {
                interuptAnimationUpdate = false;
                interuptInputUpdate = false;
                jumpSquatLanding = false;
                landingRightAnimation.reset();
                landingLeftAnimation.reset();
            }
            if (jumpSquatLanding)// changes horizontal momentum to 0 during jumpsquat and landing.
            {
                //_collisionBox._velocity.X = 0;
            }
            // read player inputs
            if (!interuptInputUpdate)
            {
                ReadInputs(gameTime);
            }
            

            _pos = _collisionBox.Update(gameTime) + new Vector2(_collisionBox._bounds.Width / 2, _collisionBox._bounds.Height / 2);

            // update animation type
            if (!interuptAnimationUpdate)
            {
                UpdateAnimationInfo();
            }

            Vector2 mousePosition = new Vector2(mouseState.X, mouseState.Y);
            FOWTSprite.pos = _pos + _FOWTPos;
            Vector2 FOWPosVec = camera.WorldToScreen(FOWTSprite.pos) - mousePosition;
            FOWTSprite.Rotation = 0 - (float)((Math.Atan2(
                FOWPosVec.X,
                FOWPosVec.Y
                ))) - 1.5f;

            base.Update(gameTime);

            previousMoveType = _currentMoveType;
            previousDirection = _currentDirection;
            return FOWPosVec;
        }


        public void Load(ContentManager Content, PhysicsHandler collisionHandler, RectangleF worldBounds = new RectangleF())
        {
            idleTex = Content.Load<Texture2D>("chars/refugee");
            idleAnimation = new Animation(idleTex, 1, 1, 0);
            idleLeftTex = Content.Load<Texture2D>("animations/main_character_side_left");
            idleLeftAnimation = new Animation(idleLeftTex, 1, 1, 0, new Vector2(2, 1));
            idleRightTex = Content.Load<Texture2D>("animations/main_character_side_right");
            idleRightAnimation = new Animation(idleRightTex, 1, 1, 0, new Vector2(-2, 1));
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
            climbRightTex = Content.Load<Texture2D>("animations/ledge_crawl2");
            climbRightAnimation = new Animation(climbRightTex, 1, 16, 50, new Vector2(-7, 13));
            climbLeftTex = Content.Load<Texture2D>("animations/ledge_crawl_left");
            climbLeftAnimation = new Animation(climbLeftTex, 1, 16, 50, new Vector2(8, 19));
            hangLeftTex = Content.Load<Texture2D>("animations/ledge_hang_left");
            hangLeftAnimation = new Animation(hangLeftTex, 1, 1, 50);
            hangRightTex = Content.Load<Texture2D>("animations/ledge_hang_right");
            hangRightAnimation = new Animation(hangRightTex, 1, 1, 50);

            jumpSquatRightTex = Content.Load<Texture2D>("animations/jumpSquat_right");
            jumpSquatRightAnimation = new Animation(jumpSquatRightTex, 1, 3, 30, new Vector2(0, -16));
            jumpSquatLeftTex = Content.Load<Texture2D>("animations/jumpSquat_left");
            jumpSquatLeftAnimation = new Animation(jumpSquatLeftTex, 1, 3, 30, new Vector2(0, -6));
            risingRightTex = Content.Load<Texture2D>("animations/rising_right");
            risingRightAnimation = new Animation(risingRightTex, 1, 1, 0);
            risingLeftTex = Content.Load<Texture2D>("animations/rising_Left");
            risingLeftAnimation = new Animation(risingLeftTex, 1, 1, 0);
            apexRightTex = Content.Load<Texture2D>("animations/apex_right");
            apexRightAnimation = new Animation(apexRightTex, 1, 1, 0);
            apexLeftTex = Content.Load<Texture2D>("animations/apex_left");
            apexLeftAnimation = new Animation(apexLeftTex, 1, 1, 0);
            fallingRightTex = Content.Load<Texture2D>("animations/falling_right");
            fallingRightAnimation = new Animation(fallingRightTex, 1, 1, 0);
            fallingLeftTex = Content.Load<Texture2D>("animations/falling_left");
            fallingLeftAnimation = new Animation(fallingLeftTex, 1, 1, 0);
            landingRightTex = Content.Load<Texture2D>("animations/landing_right");
            landingRightAnimation = new Animation(landingRightTex, 1, 30, 30, new Vector2(0, -16));
            landingLeftTex = Content.Load<Texture2D>("animations/landing_left");
            landingLeftAnimation = new Animation(landingLeftTex, 1, 30, 30, new Vector2(0, -16));

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
            animationDict.Add("idleLeft", idleLeftAnimation);
            animationDict.Add("idleRight", idleRightAnimation);
            animationDict.Add("runRight", runRightAnimation);
            animationDict.Add("runLeft", runLeftAnimation);
            animationDict.Add("walkRight", walkRightAnimation);
            animationDict.Add("walkLeft", walkLeftAnimation);
            animationDict.Add("jumpRight", jumpRightAnimation);
            animationDict.Add("jump", jumpRightAnimation);
            animationDict.Add("jumpLeft", jumpRightAnimation);
            animationDict.Add("climbRight", climbRightAnimation);
            animationDict.Add("climbLeft", climbLeftAnimation);
            animationDict.Add("hangLeft", hangLeftAnimation);
            animationDict.Add("hangRight", hangRightAnimation);

            animationDict.Add("jumpSquatRight", jumpSquatRightAnimation);
            animationDict.Add("risingRight", risingRightAnimation);
            animationDict.Add("apexRight", apexRightAnimation);
            animationDict.Add("fallingRight", fallingRightAnimation);
            animationDict.Add("landingRight", landingRightAnimation);

            animationDict.Add("jumpSquatLeft", jumpSquatLeftAnimation);
            animationDict.Add("risingLeft", risingLeftAnimation);
            animationDict.Add("apexLeft", apexLeftAnimation);
            animationDict.Add("fallingLeft", fallingLeftAnimation);
            animationDict.Add("landingLeft", landingLeftAnimation);

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

        private void ReadInputs(GameTime gameTime)
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

            
            if (landCheck && _collisionBox._velocity.Y > _maxFallSpeed)
            {
                _maxFallSpeed = _collisionBox._velocity.Y;
            }
            if (landCheck && _collisionBox._downBlocked)
            {
                landCheck = false;
                Debug.WriteLine("landed");
                Game1.instance.sounds.landSound(_maxFallSpeed, _collisionBox._maxSpeed.Y);
                _maxFallSpeed = 0;
            }
            if (Game1.instance.input.IsDown("jump") && _collisionBox._downBlocked)
            {
                jumpSquatRightAnimation.reset();
                if (_currentDirection == "Right")
                {
                    currentAnimation = "jumpSquatRight";
                    Game1.instance.sounds.jumpSound();
                    landCheck = true;
                    _collisionBox._velocity.Y -= _jump * gameTime.GetElapsedSeconds();
                }
                else
                {
                    currentAnimation = "jumpSquatLeft";
                    Game1.instance.sounds.jumpSound();
                    landCheck = true;
                    _collisionBox._velocity.Y -= _jump * gameTime.GetElapsedSeconds();
                }
                interuptAnimationUpdate = true;
                interuptInputUpdate = true;
                jumpSquatLanding = true;
                //if (!_jumpClicked && !_anchorPoint.HasValue && (_collisionBox._downBlocked || _collisionBox.HangTime(gameTime)))
                //{
                //    _collisionBox._velocity.Y -= _jump * gameTime.GetElapsedSeconds();
                //}
                //_jumpClicked = true;
                //_collisionBox._downLastBlocked = float.NegativeInfinity;
            }
            else
            {
                _jumpClicked = false;
            }
            if (!_collisionBox._downBlocked)
            {
                landCheck = true;
            }
            if (Game1.instance.input.IsDown("run"))
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
                ClimbLedge(gameTime);
            }

            _overlappingInteractable = false;
            foreach (OverlapInfo item in _collisionBox.IsOverlapping())
            {
                bool actionComplete = false; // bool for if any interaction had resul, stopping the loop so multiple interactions don't happen at once
                NPC character = item._other as NPC;
                if (character != null && !character._isCured)
                {
                    if (Game1.instance.input.JustPressed("interact"))
                    {
                        Game1.instance.inventory._gifting = true;
                        Game1.instance.inventory._recipient = character;
                        Game1.instance.UI.SwitchState(UIState.Inventory);
                        actionComplete = true;
                    }
                    else
                    {
                        _overlappingInteractable = true;
                        _overlapName = "give to " + character.name;
                    }
                }

                // check if pickup item
                SpawnItem obj = item._other as SpawnItem;
                if (obj != null)
                {
                    Debug.WriteLine(obj._name);
                    // TODO: try adding to inventory, returning whether successful or not
                    if (Game1.instance.input.JustPressed("interact") && Game1.instance.inventory.addIngredient(obj._name))
                    {
                        (Game1.instance._currentState as GameplayState)._items.Remove(obj);
                        obj._spawn.Despawn();
                        actionComplete = true;
                    }
                    else
                    {
                        _overlappingInteractable = true;
                        _overlapName = "pick up " + obj._name;
                    }
                }

                // chec if foragable object
                ForageSpot forage = item._other as ForageSpot;
                if (forage != null)
                {
                    if (Game1.instance.input.JustPressed("interact"))
                    {
                        Debug.WriteLine(forage._currSpawn + " is " + (forage._isRipe ? "ripe." : "not ripe."));
                        // TODO: check if inventory is empty before harvesting
                        if (forage._isRipe)
                        {
                            string harvested = forage.TryHarvest();
                            if (harvested != null) // something harvested
                            {
                                Game1.instance.inventory.addIngredient(harvested);
                                actionComplete = true;
                            }
                        }
                    }
                    else
                    {
                        _overlappingInteractable = true;
                        _overlapName = "forage " + forage._spawnType;
                    }
                }

                // check if area
                Area area = item._other as Area;
                if (area != null)
                {
                    if (area._name == "fire")
                    {
                        //Debug.WriteLine("Fire");
                        // Open cooking ui
                        if (Game1.instance.input.JustPressed("interact"))
                        {
                            Game1.instance.UI.SwitchState(UIState.RecipeMenu);
                            actionComplete = true;
                        }
                        else
                        {
                            _overlappingInteractable = true;
                            _overlapName = "cook";
                        }
                    }
                    else if (area._name.Contains("state"))
                    {
                        if (area._name.Contains("Cave"))
                        {
                            if (Game1.instance.input.JustPressed("interact"))
                            {
                                Game1.instance.RequestStateChange("CaveState");
                                actionComplete = true;
                            }
                            else
                            {
                                _overlappingInteractable = true;
                                _overlapName = "go to cave";
                            }
                        }
                        else if (area._name.Contains("Camp"))
                        {
                            if (Game1.instance.input.JustPressed("interact"))
                            {
                                Game1.instance.RequestStateChange("CampState");
                                actionComplete = true;
                            }
                            else
                            {
                                _overlappingInteractable = true;
                                _overlapName = "go to camp";
                            }
                        }
                    }
                }

                if (actionComplete || _overlappingInteractable)
                    break;
            }

            // movement sound
            if (_collisionBox._downBlocked && Game1.instance.input.IsDown("run") && (Game1.instance.input.IsDown("left") || Game1.instance.input.IsDown("right")))
            {
                Game1.instance.sounds.runSound(gameTime);
            }
            else if (_collisionBox._downBlocked && (Game1.instance.input.IsDown("left") || Game1.instance.input.IsDown("right")))
            {
                Game1.instance.sounds.walkSound(gameTime);
            }
        }

        private void UpdateAnimationInfo()
        {
            if (!interuptAnimationUpdate)
            {
                if (_anchorPoint != null)
                {
                    _currentMoveType = "hang";
                }
                else if (_collisionBox._velocity.X == 0 && _anchorPoint == null && _collisionBox._downBlocked) // stopped
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
                else if (!_collisionBox._downBlocked && _anchorPoint == null && _collisionBox._velocity.Y < -60) // if airborne and rising
                {
                    _currentMoveType = "rising";
                }
                else if (!_collisionBox._downBlocked && _anchorPoint == null && _collisionBox._velocity.Y <= 60 && _collisionBox._velocity.Y >= -60) // if airborne and apex
                {
                    _currentMoveType = "apex";
                }
                else if (!_collisionBox._downBlocked && _anchorPoint == null && _collisionBox._velocity.Y > 60) // if airborne and falling
                {
                    _currentMoveType = "falling";
                }
                currentAnimation = _currentMoveType + _currentDirection;
            }
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
            else if(_anchorPoint != null)
            {
                _currentDirection = _grabLeft ? "Left" : "Right";
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
                _collisionBox._leftBox.Top <= _collisionBox._bounds.Top + _grabDist &&
                _collisionBox.CanFit(_collisionBox._leftBox.TopRight - new Vector2(0, _grabDist), _yClearance))
            {
                _grabLeft = true;
                _anchorPoint = _collisionBox._leftBox.TopRight;
                _collisionBox._bounds.Position = (Point2)_anchorPoint - new Vector2(0, _grabDist);
                _collisionBox._acceleration = _collisionBox._velocity = Vector2.Zero;
                _collisionBox._posLock = true;
                _collisionBox._hasGravity = false;
            }
            else if (_collisionBox._rightBlocked && Game1.instance.input.IsDown("right") && // check for side hit right grab
                     _collisionBox._rightBox.Top > _collisionBox._bounds.Top &&
                     _collisionBox._rightBox.Top <= _collisionBox._bounds.Top + _grabDist &&
                     _collisionBox.CanFit(_collisionBox._rightBox.TopLeft - new Vector2(_collisionBox._bounds.Width, _grabDist), _yClearance))
            {
                _grabLeft = false;
                _anchorPoint = _collisionBox._rightBox.TopLeft;
                _collisionBox._bounds.Position = (Point2)_anchorPoint - new Vector2(_collisionBox._bounds.Width, _grabDist);
                _collisionBox._acceleration = _collisionBox._velocity = Vector2.Zero;
                _collisionBox._posLock = true;
                _collisionBox._hasGravity = false;
            }
            else if(_collisionBox._downBlocked && Game1.instance.input.JustPressed("down") && 
                    _collisionBox._downBox.Width < _collisionBox._bounds.Width) // check for drop down
            {
                if (_collisionBox._bounds.Right - _collisionBox._downBox.Right > _collisionBox._downBox.Left - _collisionBox._bounds.Left && // down right grab left
                    _collisionBox.CanFit(_collisionBox._downBox.TopRight - new Vector2(0, _grabDist), _yClearance))
                {
                    _grabLeft = true;
                    _anchorPoint = _collisionBox._downBox.TopRight;
                    _collisionBox._bounds.Position = (Point2)_anchorPoint - new Vector2(0, _grabDist);
                    _collisionBox._acceleration = _collisionBox._velocity = Vector2.Zero;
                    _collisionBox._posLock = true;
                    _collisionBox._hasGravity = false;
                }
                else if(_collisionBox.CanFit(_collisionBox._downBox.TopLeft - new Vector2(_collisionBox._bounds.Width, _grabDist), _yClearance)) // down left grab right
                {
                    _grabLeft = false;
                    _anchorPoint = _collisionBox._downBox.TopLeft;
                    _collisionBox._bounds.Position = (Point2)_anchorPoint - new Vector2(_collisionBox._bounds.Width, _grabDist);
                    _collisionBox._acceleration = _collisionBox._velocity = Vector2.Zero;
                    _collisionBox._posLock = true;
                    _collisionBox._hasGravity = false;
                }
            }
            else
            {
                _anchorPoint = null;
                _collisionBox._hasGravity = true;
            }
        }

        private void ClimbLedge(GameTime gameTime)
        {
            if (Game1.instance.input.JustPressed("down") || // let go of ledge
                    (_grabLeft && Game1.instance.input.JustPressed("right")) ||
                    (!_grabLeft && Game1.instance.input.JustPressed("left")))
            {
                _collisionBox._posLock = false;
                _collisionBox._hasGravity = true;
                _anchorPoint = null;
            }
            else if (Game1.instance.input.JustPressed("up") && // climb up on top of ledge
                     _collisionBox.CanFit(new Point2(_anchorPoint.Value.X - (_grabLeft ? _collisionBox._bounds.Width : 0),
                                                      _anchorPoint.Value.Y - _collisionBox._bounds.Height)))
            {
                interuptAnimationUpdate = true;
                interuptInputUpdate = true;
                _collisionBox._bounds.Position = new Point2(_anchorPoint.Value.X - (_grabLeft ? _collisionBox._bounds.Width : 0),
                                                            _anchorPoint.Value.Y - _collisionBox._bounds.Height);
                _collisionBox._posLock = false;
                _collisionBox._hasGravity = true;
                _anchorPoint = null;
                //put ledge climb animation here.

                climbRightAnimation.reset();
                climbLeftAnimation.reset();
                if(_currentDirection == "Right")
                {
                    currentAnimation = "climbRight";
                }
                else
                {
                    currentAnimation = "climbLeft";
                }
                //_currentMoveType = "climb";
                //currentAnimation = _currentMoveType + _currentDirection;
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

        public void Hit(float attackDamage)
        {
            Debug.WriteLine("Hit -" + attackDamage);
        }
    }
}
