using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace WillowWoodRefuge
{
    class OverlapInfo
    {
        public IPhysicsObject _other { get; } // other object hit
        public string _otherLabel { get; } // label(type/mask) of contact object
        public RectangleF _overlapRect { get; } // rectangle describing overlap

        public OverlapInfo(CollisionBox box1, CollisionBox box2, ref RectangleF overlapRect)
        {
            string otherLabel = box2._label;
            _other = box2._parent;
            _otherLabel = otherLabel;
            _overlapRect = overlapRect;
        }
    }
}
