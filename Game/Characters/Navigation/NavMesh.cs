using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Text;

namespace WillowWoodRefuge
{
    public class NavMesh
    {
        NavPointMap _pointMap;
        Dictionary<NavPoint, List<NavPoint>> _edges;


        public NavMesh(NavPointMap pointMap, bool canJump = false, bool canFall = false, float jumpHeight = 0, float jumpDist = 0, float airControl = 0)
        {
            _pointMap = pointMap;
            _edges = new Dictionary<NavPoint, List<NavPoint>>();

            FillMesh(canJump, canFall, jumpHeight, jumpDist, airControl);
        }

        public void Draw(SpriteBatch spriteBatch, bool isDebug)
        {
            _pointMap.Draw(spriteBatch, isDebug);
            if (isDebug)
            {
                foreach (NavPoint start in _edges.Keys)
                {
                    foreach (NavPoint end in _edges[start])
                    {
                        spriteBatch.DrawLine(start._location, end._location, Color.Blue);
                    }
                }
            }
        }

        private void FillMesh(bool canJump, bool canFall, float jumpHeight, float jumpDist, float airControl)
        {
            Point lastPoint = new Point();
            // calculate walking connections
            foreach(Point point in _pointMap._navPoints.Keys)
            {
                if (_pointMap._navPoints[point]._pointType != NavPointType.leftEdge &&
                    _pointMap._navPoints[point]._pointType != NavPointType.solo)
                {
                    AddEdge(lastPoint, point);
                    AddEdge(point, lastPoint);
                }

                lastPoint = point;
            }
        }

        private void AddEdge(Point start, Point end)
        {
            if(!_edges.ContainsKey(_pointMap._navPoints[start]))
            {
                _edges.Add(_pointMap._navPoints[start], new List<NavPoint>());
            }
            _edges[_pointMap._navPoints[start]].Add(_pointMap._navPoints[end]);
        }
    }
}
