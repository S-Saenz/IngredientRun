using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace WillowWoodRefuge
{
    public delegate void CollisionEventHandler(CollisionInfo info);
    public delegate void OverlapEventHandler(OverlapInfo info);
    public delegate void MovementEventHandler(Vector2 movement);

    public interface IPhysicsObject
    {
    }

    public class CollisionBox
    {
        public RectangleF _bounds;
        public IPhysicsObject _parent { get; set; }
        public string _label;
        public Vector2 _velocity = Vector2.Zero;
        private Vector2 _prevVelocity = Vector2.Zero;
        public Vector2 _acceleration;
        public Vector2 _maxSpeed;
        public Vector2 _gravity;
        private Vector2 _prevPos;
        public float _friction; // 0-1
        public float _damping = 0.1f;
        public RectangleF _worldBounds;

        private static float _minFloor = .1f;

        PhysicsHandler _collisionHandler;

        // Hang time
        public float _downLastBlocked = float.NegativeInfinity;
        public float _hangTime = .1f;

        // Blocked information
        public bool _upBlocked;
        private bool _upWasBlocked;
        public List<CollisionInfo> _upInfo = new List<CollisionInfo>();
        public RectangleF _upBox;

        public bool _downBlocked;
        private bool _downWasBlocked;
        public List<CollisionInfo> _downInfo = new List<CollisionInfo>();
        public RectangleF _downBox;

        public bool _leftBlocked;
        private bool _leftWasBlocked;
        public List<CollisionInfo> _leftInfo = new List<CollisionInfo>();
        public RectangleF _leftBox;

        public bool _rightBlocked;
        private bool _rightWasBlocked;
        public List<CollisionInfo> _rightInfo = new List<CollisionInfo>();
        public RectangleF _rightBox;

        // Events
        private event CollisionEventHandler _onCollision; // called every frame that object is colliding with something
        private event CollisionEventHandler _onCollisionStart; // called once when colliding with something new (new material/side)
        private event CollisionEventHandler _onCollisionEnd; // called once when no longer colliding with anything on given side
        private event OverlapEventHandler _onOverlap;

        private event MovementEventHandler _onMovementStart;
        private event MovementEventHandler _onMovementEnd;
        private event MovementEventHandler _onMovementChangeDirection;

        public List<Vector2> _cells = new List<Vector2>();

        public CollisionBox(RectangleF bounds, PhysicsHandler collisionHandler, IPhysicsObject parent = null, RectangleF worldBounds = new RectangleF(),
                            Vector2? maxSpeed = null, float gravity = 9.8f, float damping = 1, float friction = 1)
        {
            _bounds = bounds;

            _collisionHandler = collisionHandler;
            _parent = parent;
            _worldBounds = worldBounds;
            _gravity = new Vector2(0, gravity * 0.4f);
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
            // update last blocked time
            if (_downBlocked)
            {
                _downLastBlocked = (float)gameTime.TotalGameTime.TotalSeconds;
            }

            // clear out touching info
            _upInfo.Clear();
            _downInfo.Clear();
            _leftInfo.Clear();
            _rightInfo.Clear();

            Vector2 pos = _bounds.Position;
            _prevPos = _bounds.Position;

            // Apply gravity
            if (!HangTime(gameTime))
            {
                _acceleration += _gravity * gameTime.GetElapsedSeconds() * 350;
            }

            // apply acceleration
            _velocity += _acceleration * gameTime.GetElapsedSeconds();

            // Update smoothStep "friction"
            if((_downBlocked || !HangTime(gameTime)) || _upBlocked) // horizontal friction
            {
                if (Math.Abs(_velocity.X) > Math.Abs(_prevVelocity.X)) // accelerating
                {
                    // Debug.WriteLine("Accelerating " + _prevVelocity.X + " " + _velocity.X);
                }
                else if(Math.Abs(_velocity.X) < Math.Abs(_prevVelocity.X)) // slowing
                {
                    // Debug.WriteLine("Slowing " + _prevVelocity.X + " " + _velocity.X);
                    if (MathF.Abs(_prevVelocity.X) < _friction * 2.0f) // round to zero if close enough
                    {
                        _prevVelocity.X = 0;
                    }
                }
                _velocity.X = MathHelper.Lerp(_prevVelocity.X, _velocity.X, _friction * gameTime.GetElapsedSeconds() * 20);
            }
            // if (_rightBlocked || _leftBlocked) // vertical friction
            // {
            //     if (Math.Abs(_velocity.Y) > Math.Abs(_prevVelocity.Y)) // accelerating
            //     {
            //         // Debug.WriteLine("Accelerating " + _prevVelocity.Y + " " + _velocity.Y);
            //     }
            //     else if (Math.Abs(_velocity.Y) < Math.Abs(_prevVelocity.Y)) // slowing
            //     {
            //         // Debug.WriteLine("Slowing " + _prevVelocity.Y + " " + _velocity.Y);
            //         if (MathF.Abs(_prevVelocity.Y) < _friction * 2.0f) // round to zero if close enough
            //         {
            //             _prevVelocity.Y = 0;
            //         }
            //     }
            //     _velocity.Y = MathHelper.Lerp(_prevVelocity.Y, _velocity.Y, _friction);
            // }

            // Update velocity
            if (MathF.Abs(_velocity.X) >= _maxSpeed.X)
            {
                _velocity.X = Math.Clamp(_velocity.X, -_maxSpeed.X, _maxSpeed.X);
            }
            if (MathF.Abs(_velocity.Y) >= _maxSpeed.Y)
            {
                _acceleration.Y = 0;
                _velocity.Y = Math.Clamp(_velocity.Y, -_maxSpeed.Y, _maxSpeed.Y);
            }
            else if (MathF.Abs(_velocity.Y) < 0.01f)
            {
                _velocity.Y = 0;
            }

            // Apply final velocity and try move
            pos += _velocity * gameTime.GetElapsedSeconds() + 0.5f * _acceleration * gameTime.GetElapsedSeconds() * gameTime.GetElapsedSeconds();
            IncrementBlocked();
            _bounds.Position = _collisionHandler.TryMove(this, pos);

            // Update velocity based on actual move
            if (_velocity.Length() > 0 && (!_downBlocked || (_downBox.Width > _minFloor)))
            {
                _velocity = Vector2.Divide(_bounds.Position - _prevPos, gameTime.GetElapsedSeconds());
            }

            if((_velocity.X != 0 && _prevVelocity.X == 0) || // started moving
               (_velocity.Y != 0 && _prevVelocity.Y == 0))
            {
                _onMovementStart?.Invoke(_velocity);
            }
            else if ((_velocity.X == 0 && _prevVelocity.X != 0) || // stopped moving
               (_velocity.Y == 0 && _prevVelocity.Y != 0))
            {
                _onMovementChangeDirection?.Invoke(_velocity);
                _onMovementEnd?.Invoke(_velocity);
            }
            else if((_velocity.X > 0) == (_prevVelocity.X > 0) || // changed direction
                    (_velocity.Y > 0) == (_prevVelocity.Y > 0))
            {
                _onMovementChangeDirection?.Invoke(_velocity);
            }

            // Stop acceleration if against wall
            if ((_upBlocked && _acceleration.Y < 0) ||
               (_downBlocked && _acceleration.Y > 0))
            {
                _acceleration.Y = 0;
            }
            if ((_leftBlocked && _acceleration.X < 0) ||
               (_rightBlocked && _acceleration.X > 0))
            {
                _acceleration.X = 0;
            }

            // Stop velocity if against wall
            if ((_upBlocked && _velocity.Y < 0) ||
               (_downBlocked && _velocity.Y > 0))
            {
                _velocity.Y = 0;
            }
            if ((_leftBlocked && _velocity.X < 0) ||
               (_rightBlocked && _velocity.X > 0))
            {
                _velocity.X = 0;
            }

            // update prev velocity before adjusted for next move
            _prevVelocity = _velocity;

            // update side information
            CollisionUpdateSide(ref _upWasBlocked, ref _upBlocked, _upInfo); // check up states
            CollisionCalcBoxes(_upInfo, out _upBox);
            CollisionUpdateSide(ref _downWasBlocked, ref _downBlocked, _downInfo); // check down states
            CollisionCalcBoxes(_downInfo, out _downBox);
            CollisionUpdateSide(ref _leftWasBlocked, ref _leftBlocked, _leftInfo); // check left states
            CollisionCalcBoxes(_leftInfo, out _leftBox);
            CollisionUpdateSide(ref _rightWasBlocked, ref _rightBlocked, _rightInfo); // check right states
            CollisionCalcBoxes(_rightInfo, out _rightBox);

            // Debug.WriteLine(_prevVelocity);
            // Debug.WriteLine("Up: " + _upBlocked + " Left: " + _leftBlocked + " Right: " + _rightBlocked + " Down: " + _downBlocked);
            // Debug.WriteLine("curr: " + _downBlocked + " prev: " + _downWasBlocked);

            _collisionHandler.CheckBox(this, _prevPos);
            return _bounds.Position;
        }

        // tries to move collision box at desired velocity, taking into account momentum and friction
        public void TryMoveHorizontal(float desiredVelocity)
        {
            _velocity.X = desiredVelocity;
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
            spriteBatch.DrawLine(_bounds.Center, _bounds.Center + _velocity / 2, Color.Aquamarine);
            
            foreach(CollisionInfo info in _downInfo)
            {
                spriteBatch.DrawRectangle(info._overlapRect, Color.Red);
            }
            foreach (CollisionInfo info in _upInfo)
            {
                spriteBatch.DrawRectangle(info._overlapRect, Color.Red);
            }
            foreach (CollisionInfo info in _rightInfo)
            {
                spriteBatch.DrawRectangle(info._overlapRect, Color.Red);
            }
            foreach (CollisionInfo info in _leftInfo)
            {
                spriteBatch.DrawRectangle(info._overlapRect, Color.Red);
            }
        }

        private void IncrementBlocked() // steps forward blocked bools, setting wasBlocked and resetting blocked
        {
            _upWasBlocked = _upBlocked;
            _downWasBlocked = _downBlocked;
            _leftWasBlocked = _leftBlocked;
            _rightWasBlocked = _rightBlocked;

            _upBlocked = _downBlocked = _leftBlocked = _rightBlocked = false;
        }

        public void CallOverlap(OverlapInfo info)
        {
            _onOverlap?.Invoke(info);
        }

        public void CallCollision(CollisionInfo info)
        {
            _onCollision?.Invoke(info);
        }

        public void AddOverlapListener(OverlapEventHandler overlapFunction)
        {
            _onOverlap += overlapFunction;
        }

        public void AddCollisionListener(CollisionEventHandler collisionFunction)
        {
            _onCollision += collisionFunction;
        }

        public void AddCollisionStartListener(CollisionEventHandler collisionFunction)
        {
            _onCollisionStart += collisionFunction;
        }

        public void AddCollisionEndListener(CollisionEventHandler collisionFunction)
        {
            _onCollisionEnd += collisionFunction;
        }

        public void AddMovementStartListener(MovementEventHandler moveFunction)
        {
            _onMovementStart += moveFunction;
        }

        public void AddMovementEndListener(MovementEventHandler moveFunction)
        {
            _onMovementEnd += moveFunction;
        }
        public void AddMovementChangeDirectionListener(MovementEventHandler moveFunction)
        {
            _onMovementChangeDirection += moveFunction;
        }

        private void CollisionUpdateSide(ref bool prevState, ref bool currState, List<CollisionInfo> infoList)
        {
            if (!prevState && currState) // start hit
            {
                // Debug.WriteLine("Start " + info._hitDir);
                foreach (CollisionInfo info in infoList)
                {
                    _onCollisionStart?.Invoke(info);
                }
            }
            else if (prevState && !currState) // end hit
            {
                // Debug.WriteLine("End " + info._hitDir);
                foreach (CollisionInfo info in infoList)
                {
                    _onCollisionEnd?.Invoke(info);
                }
            }
        }

        public List<CollisionInfo> IsOverlapping()
        {
            return _collisionHandler.IsOverlapping(this);
        }

        public void CollisionCalcBoxes(List<CollisionInfo> infos, out RectangleF box)
        {
            Vector2 min = new Vector2(float.PositiveInfinity, float.PositiveInfinity);
            Vector2 max = Vector2.Zero;

            if (infos.Count > 0)
            {
                foreach (CollisionInfo info in infos)
                {
                    if (info._overlapRect.Top < min.Y)
                    {
                        min.Y = info._overlapRect.Top;
                    }
                    if (info._overlapRect.Left < min.X)
                    {
                        min.X = info._overlapRect.Left;
                    }
                    if (info._overlapRect.Bottom > max.Y)
                    {
                        max.Y = info._overlapRect.Bottom;
                    }
                    if (info._overlapRect.Right > max.X)
                    {
                        max.X = info._overlapRect.Right;
                    }
                }
                box = new RectangleF(min.X, min.Y, max.X - min.X, max.Y - min.Y);
            }
            else
            {
                box = RectangleF.Empty;
            }
        }

        public bool HangTime(GameTime gameTime)
        {
            return (float)gameTime.TotalGameTime.TotalSeconds - _downLastBlocked < _hangTime;
        }
    }
}
