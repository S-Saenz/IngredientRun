using Microsoft.Xna.Framework;
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
        public float _damping;
        public float _friction;
        public RectangleF _worldBounds;

        PhysicsHandler _collisionHandler;

        // Blocked information
        public bool _upBlocked { get; set; }
        public bool _downBlocked { get; set; }
        public bool _leftBlocked { get; set; }
        public bool _rightBlocked { get; set; }

        // Events
        public event CollisionEventHandler _onCollision;
        public event CollisionEventHandler _onOverlap;

        public CollisionBox(RectangleF bounds, PhysicsHandler collisionHandler, CollisionEventHandler onCollision = null, 
                            CollisionEventHandler onOverlap = null, IPhysicsObject parent = null, RectangleF worldBounds = new RectangleF(),
                            Vector2? maxSpeed = null, float gravity = 9.8f, float damping = 1, float friction = 0)
        {
            _bounds = bounds;

            if (onCollision != null)
            {
                _onCollision = onCollision;
            }
            if (onOverlap != null)
            {
                _onOverlap = onOverlap;
            }

            _collisionHandler = collisionHandler;
            _parent = parent;
            _worldBounds = worldBounds;
            _gravity = new Vector2(0, gravity);
            _damping = damping;
            _friction = friction;
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

        private Vector2 Move(Vector2 pos)
        {
            _upBlocked = _downBlocked = _leftBlocked = _rightBlocked = false;
            return _collisionHandler.TryMove(this, pos);
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

            // Apply friction
            // if(_downBlocked || _upBlocked)
            // {
            //     //_velocity.X /= 1 + _friction * gameTime.GetElapsedSeconds();
            //     _acceleration.X -= _acceleration.X /2;
            // }

            // Apply damping (air resistance)
            // _velocity /= 1 + _damping * gameTime.GetElapsedSeconds();

            // Update velocity
            if (MathF.Abs(_velocity.X) >= _maxSpeed.X)
            {
                _acceleration.X = 0;
            }
            if(MathF.Abs(_velocity.Y) >= _maxSpeed.Y)
            {
                _acceleration.Y = 0;
            }

            _velocity += _acceleration * gameTime.GetElapsedSeconds();

            // Apply final velocity and try move
            pos += _velocity * gameTime.GetElapsedSeconds();
            _bounds.Position = Move(pos);

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

            return _bounds.Position;
            // Debug.WriteLine("Up: " + _upBlocked + " Left: " + _leftBlocked + " Right: " + _rightBlocked + " Down: " + _downBlocked);
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

        public void CallOverlap(CollisionInfo info)
        {
            _onOverlap?.Invoke(info);
        }

        public void CallCollision(CollisionInfo info)
        {
            _onCollision?.Invoke(info);
        }
    }
}
