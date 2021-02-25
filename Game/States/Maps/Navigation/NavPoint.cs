using System;
using System.Collections.Generic;
using System.Text;

namespace WillowWoodRefuge
{
    public enum NavPointType { leftEdge, platform, rightEdge, solo};
    class NavPoint
    {
        public NavPointType _pointType;
        public int _platformIndex;

        public NavPoint(NavPointType type, int platformIndex)
        {
            _pointType = type;
            _platformIndex = platformIndex;
        }
    }
}
