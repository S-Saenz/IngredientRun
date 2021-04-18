using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace WillowWoodRefuge
{
    public enum NavPointType { leftEdge, platform, rightEdge, solo};
    public class NavPoint
    {
        public NavPointType _pointType;
        public int _platformIndex;
        public Vector2 _location;
        public Point2 _tileLoc;

        public NavPoint(NavPointType type, int platformIndex, Vector2 location, Point2 tileLoc)
        {
            _pointType = type;
            _platformIndex = platformIndex;
            _location = location;
            _tileLoc = tileLoc;
        }
    }
}
