using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace WillowWoodRefuge
{
    public class CollisionInfo
    {
        public IPhysicsObject _other { get; } // other object hit
        public CollisionBox _otherCBox { get; }
        public string _otherLabel { get; } // label(type/mask) of contact object
        public Vector2 _loc { get; }  // center point of contact on edge of other
        public Vector2 _hitDir { get; set; } // direction vector of collision (points to side collided)
        public float _overlapDist { get; } // distance overlapped/penetrated
        public RectangleF _overlapRect { get; private set; } // rectangle describing overlap

        public CollisionInfo(CollisionBox box1, CollisionBox box2, ref RectangleF overlapRect)
        {
            string otherLabel = box2._label;
            Vector2 loc = overlapRect.Center;
            Vector2 hitDir = Vector2.Zero;
            float overlapDist;

            if (overlapRect.Width > overlapRect.Height) // top or bottom hit
            {
                hitDir.Y = box1._bounds.Center.Y > box2._bounds.Center.Y ? -1 : 1;
                if (box1._bounds.Center.Y > box2._bounds.Center.Y) // Top
                {
                    loc.Y -= overlapRect.Height / 2;
                    if (!(overlapRect.Width == 0 && overlapRect.Height == 0))
                    {
                        box1._upBlocked = true;
                        box1._upInfo.Add(this);
                    }
                }
                else // Bottom
                {
                    loc.Y += overlapRect.Height / 2;
                    if (!(overlapRect.Width == 0 && overlapRect.Height == 0))
                    {
                        box1._downBlocked = true;
                        box1._downInfo.Add(this);
                    }
                }
                overlapDist = overlapRect.Height;
            }
            else // left or right hit
            {
                hitDir.X = box1._bounds.Center.X > box2._bounds.Center.X ? -1 : 1;
                if (box1._bounds.Center.X > box2._bounds.Center.X) // Left
                {
                    loc.X -= overlapRect.Width / 2;
                    if (!(overlapRect.Width == 0 && overlapRect.Height == 0))
                    {
                        box1._leftBlocked = true;
                        box1._leftInfo.Add(this);
                    }
                }
                else // Right
                {
                    loc.X += overlapRect.Width / 2;
                    if (!(overlapRect.Width == 0 && overlapRect.Height == 0))
                    {
                        box1._rightBlocked = true;
                        box1._rightInfo.Add(this);
                    }
                }
                overlapDist = overlapRect.Width;
            }

            _other = box2._parent;
            _otherCBox = box2;
            _otherLabel = otherLabel;
            _loc = loc;
            _hitDir = hitDir;
            _overlapDist = overlapDist;
            _overlapRect = overlapRect;
        }

        public void UpdateOverlapRect(RectangleF overlapRect)
        {
            _overlapRect = overlapRect;
        }
    }
}
