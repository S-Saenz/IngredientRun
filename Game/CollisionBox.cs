using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System.Diagnostics;

namespace IngredientRun
{
    delegate void CollisionEventHandler(CollisionInfo info);

    class CollisionBox
    {
        public RectangleF _bounds;
        public IPhysicsObject _parent { get; set; }
        public string _label { get; set; }
        public float _drag { get; set; }
        public float _friction { get; set; }
        public float _mass { get; set; }

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
                            CollisionEventHandler onOverlap = null, IPhysicsObject parent = null)
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
        }

        public Vector2 Move(Vector2 pos)
        {
            _upBlocked = _downBlocked = _leftBlocked = _rightBlocked = false;
            return _collisionHandler.TryMove(this, pos);
        }

        public void Update(Vector2 pos)
        {
            _bounds.Position = pos;
            // Debug.WriteLine("Up: " + _upBlocked + " Left: " + _leftBlocked + " Right: " + _rightBlocked + " Down: " + _downBlocked);
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
