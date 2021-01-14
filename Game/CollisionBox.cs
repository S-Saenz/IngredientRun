using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace IngredientRun
{
    delegate void CollisionEventHandler(CollisionInfo info);

    class CollisionBox
    {
        public RectangleF _bounds;
        public string _label { get; set; }
        CollisionHandler _collisionHandler;

        // Events

        public event CollisionEventHandler _onCollision;
        public event CollisionEventHandler _onOverlap;

        public CollisionBox(RectangleF bounds, CollisionEventHandler onCollision, CollisionEventHandler onOverlap, CollisionHandler collisionHandler)
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
        }

        public Vector2 Move(Vector2 pos)
        {
            return _collisionHandler.TryMove(this, pos);
        }

        public void Update(Vector2 pos)
        {
            _bounds.Position = pos;
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
