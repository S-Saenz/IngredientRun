using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace WillowWoodRefuge
{
    public enum NavPointType { leftEdge, platform, rightEdge, solo};
    public class NavPoint
    {
        public NavPointType _pointType;
        public int _platformIndex;
        public Vector2 _location;

        public NavPoint(NavPointType type, int platformIndex, Vector2 location)
        {
            _pointType = type;
            _platformIndex = platformIndex;
            _location = location;
        }
    }
}
