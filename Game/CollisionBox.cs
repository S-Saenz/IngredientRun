﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Diagnostics;

namespace IngredientRun
{
    delegate void CollisionEventHandler(CollisionInfo info);

    class CollisionBox
    {
        public RectangleF _bounds;
        public IPhysicsObject _parent { get; set; }
        public string _label;
        public Vector2 _velocity;
        public Vector2 _acceleration;
        public Vector2 _maxSpeed;
        public Vector2 _gravity;
        public Vector2 _prevPos;
        public float _friction; // 0-1
        // public float _damping;
        public RectangleF _worldBounds;

        PhysicsHandler _collisionHandler;

        // Blocked information
        public bool _upBlocked;
        private bool _upWasBlocked;
        public CollisionInfo _upInfo;

        public bool _downBlocked;
        private bool _downWasBlocked;
        public CollisionInfo _downInfo;

        public bool _leftBlocked;
        private bool _leftWasBlocked;
        public CollisionInfo _leftInfo;

        public bool _rightBlocked;
        private bool _rightWasBlocked;
        public CollisionInfo _rightInfo;

        // Events
        private event CollisionEventHandler _onCollision; // called every frame that object is colliding with something
        private event CollisionEventHandler _onCollisionStart; // called once when colliding with something new (new material/side)
        private event CollisionEventHandler _onCollisionEnd; // called once when no longer colliding with anything on given side
        private event CollisionEventHandler _onOverlap;

        public CollisionBox(RectangleF bounds, PhysicsHandler collisionHandler, CollisionEventHandler onCollision = null, 
                            CollisionEventHandler onOverlap = null, IPhysicsObject parent = null, RectangleF worldBounds = new RectangleF(),
                            Vector2? maxSpeed = null, float gravity = 9.8f, float damping = 1, float friction = 1)
        {
            _bounds = bounds;

            if (onCollision != null)
            {
                _onCollision += onCollision;
            }
            if (onOverlap != null)
            {
                _onOverlap += onOverlap;
            }

            _collisionHandler = collisionHandler;
            _parent = parent;
            _worldBounds = worldBounds;
            _gravity = new Vector2(0, gravity);
            _friction = friction;
            // _damping = damping;
            if (maxSpeed.HasValue)
            {
                _maxSpeed = maxSpeed.Value;
            }
            else
            {
                _maxSpeed = Vector2.One;
            }
            _prevPos = _bounds.Position;
        }

        public Vector2 Update(GameTime gameTime)
        {
            Vector2 pos = _bounds.Position;
            Vector2 _prevPos = _bounds.Position;

            // Apply gravity
            if (!_downBlocked)
            {
                _acceleration += _gravity * gameTime.GetElapsedSeconds() * 200;
            }

            // Apply damping (air resistance)
            // _velocity /= 1 + _damping * gameTime.GetElapsedSeconds();

            // Update velocity
            if (MathF.Abs(_velocity.X) >= _maxSpeed.X)
            {
                _velocity.X = Math.Clamp(_velocity.X, -_maxSpeed.X, _maxSpeed.X);
            }
            if(MathF.Abs(_velocity.Y) >= _maxSpeed.Y)
            {
                _acceleration.Y = 0;
                _velocity.Y = Math.Clamp(_velocity.Y, -_maxSpeed.Y, _maxSpeed.Y);
            }
            else if(MathF.Abs(_velocity.Y) < 0.01f)
            {
                _velocity.Y = 0;
            }

            // Update smoothStep "friction"
            if (_acceleration.X == 0 && _velocity.X != 0 && _downBlocked)
            {
                _velocity.X = MathHelper.Lerp(_velocity.X, 0, _friction);
                if (MathF.Abs(_velocity.X) < _friction / 2.0f)
                {
                    _velocity.X = 0;
                }
            }

            _velocity += _acceleration * gameTime.GetElapsedSeconds();

            // Apply final velocity and try move
            pos += _velocity * gameTime.GetElapsedSeconds();
            IncrementBlocked();
            _bounds.Position = _collisionHandler.TryMove(this, pos);

            // Update velocity based on move
            if (_velocity.Length() > 0)
            {
                _velocity = Vector2.Divide(_bounds.Position - _prevPos, gameTime.GetElapsedSeconds());
            }

            // Stop acceleration if against wall
            if((_upBlocked && _acceleration.Y < 0) ||
               (_downBlocked && _acceleration.Y > 0))
            {
                _acceleration.Y = 0;
            }
            if ((_leftBlocked && _acceleration.X < 0) ||
               (_rightBlocked && _acceleration.X > 0))
            {
                _acceleration.X = 0;
            }

            CollisionUpdateSide(ref _upWasBlocked, ref _upBlocked, _upInfo); // check up states
            CollisionUpdateSide(ref _downWasBlocked, ref _downBlocked, _downInfo); // check down states
            CollisionUpdateSide(ref _leftWasBlocked, ref _leftBlocked, _leftInfo); // check left states
            CollisionUpdateSide(ref _rightWasBlocked, ref _rightBlocked, _rightInfo); // check right states

            // Debug.WriteLine("Up: " + _upBlocked + " Left: " + _leftBlocked + " Right: " + _rightBlocked + " Down: " + _downBlocked);
            // Debug.WriteLine("curr: " + _downBlocked + " prev: " + _downWasBlocked);

            return _bounds.Position;
        }

        public void Accelerate(Vector2 acceleration)
        {
            _acceleration += acceleration;
        }
        public void AddVelocity(Vector2 velocity)
        {
            _velocity += velocity;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawRectangle(_bounds, Color.LawnGreen, 1);
        }

        private void IncrementBlocked() // steps forward blocked bools, setting wasBlocked and resetting blocked
        {
            _upWasBlocked = _upBlocked;
            _downWasBlocked = _downBlocked;
            _leftWasBlocked = _leftBlocked;
            _rightWasBlocked = _rightBlocked;

            _upBlocked = _downBlocked = _leftBlocked = _rightBlocked = false;
        }

        public void CallOverlap(CollisionInfo info)
        {
            _onOverlap?.Invoke(info);
        }

        public void CallCollision(CollisionInfo info)
        {
            _onCollision?.Invoke(info);
        }

        public void CallCollisionStart(CollisionInfo info)
        {
            _onCollisionStart?.Invoke(info);
        }

        public void CallCollisionEnd(CollisionInfo info)
        {
            _onCollisionEnd?.Invoke(info);
        }

        public void AddOverlapListener(CollisionEventHandler overlapEvent)
        {
            _onOverlap += overlapEvent;
        }

        public void AddCollisionListener(CollisionEventHandler collisionEvent)
        {
            _onCollision += collisionEvent;
        }

        public void AddCollisionStartListener(CollisionEventHandler collisionEvent)
        {
            _onCollisionStart += collisionEvent;
        }

        public void AddCollisionEndListener(CollisionEventHandler collisionEvent)
        {
            _onCollisionEnd += collisionEvent;
        }

        private void CollisionUpdateSide(ref bool prevState, ref bool currState, CollisionInfo info)
        {
            if (!prevState && currState) // start hit
            {
                // Debug.WriteLine("Start " + info._hitDir);
                CallCollisionStart(info);
            }
            else if (prevState && !currState) // end hit
            {
                // Debug.WriteLine("End " + info._hitDir);
                CallCollisionEnd(info);
            }
        }
    }
}
