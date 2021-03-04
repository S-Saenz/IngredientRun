using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WillowWoodRefuge
{
    public class NavMesh
    {
        NavPointMap _pointMap;
        Dictionary<NavPoint, List<NavPoint>> _edges;


        public NavMesh(NavPointMap pointMap, bool canJump = false, bool canFall = false, float jumpHeight = 0, float jumpDist = 0, 
                       float airControl = 0, Area area = null)
        {
            _pointMap = pointMap;
            _edges = new Dictionary<NavPoint, List<NavPoint>>();

            FillMesh(canJump, canFall, jumpHeight, jumpDist, airControl, area);
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

        private void FillMesh(bool canJump, bool canFall, float jumpHeight, float jumpDist, float airControl, Area area = null)
        {
            Point lastPoint = new Point();
            // calculate walking connections
            if (area != null)
            {
                foreach (Point point in _pointMap._navPoints.Keys)
                {
                    if (_pointMap._navPoints[point]._pointType != NavPointType.leftEdge &&
                        _pointMap._navPoints[point]._pointType != NavPointType.solo &&
                        area._bounds.Left <= point.X * _pointMap._tileSize && area._bounds.Right >= point.X * _pointMap._tileSize &&
                        area._bounds.Top <= point.Y * _pointMap._tileSize && area._bounds.Bottom >= point.Y * _pointMap._tileSize &&
                        area._bounds.Left <= lastPoint.X * _pointMap._tileSize && area._bounds.Right >= lastPoint.X * _pointMap._tileSize &&
                        area._bounds.Top <= lastPoint.Y * _pointMap._tileSize && area._bounds.Bottom >= lastPoint.Y * _pointMap._tileSize)
                    {
                        AddEdge(lastPoint, point);
                        AddEdge(point, lastPoint);
                    }

                    lastPoint = point;
                }
            }
            else
            {
                foreach (Point point in _pointMap._navPoints.Keys)
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
        }

        private void AddEdge(Point start, Point end)
        {
            if(!_edges.ContainsKey(_pointMap._navPoints[start]))
            {
                _edges.Add(_pointMap._navPoints[start], new List<NavPoint>());
            }
            _edges[_pointMap._navPoints[start]].Add(_pointMap._navPoints[end]);
        }

        // returns the shortest path to the closest navpoint to target (NOT IMPLEMENTED YET)
        public List<NavPoint> GetPathTo(Vector2 pos, Vector2 target)
        {
            List<NavPoint> path = new List<NavPoint>();
            path.Add(GetClosest(pos));

            return path;
        }

        public Dictionary<NavPoint, NavPoint> GetAllPossible(Vector2 pos)
        {
            NavPoint loc = GetClosest(pos);
            Dictionary<NavPoint, NavPoint> web;
            BFS(loc, out web);

            return web;
        }

        private void BFS(NavPoint start, out Dictionary<NavPoint, NavPoint> parent)
        {
            // set up parent container
            parent = new Dictionary<NavPoint, NavPoint>();
            parent.Add(start, null);

            // set up visited container
            List<NavPoint> visited = new List<NavPoint>();
            visited.Add(start);

            // set up queue container
            List<NavPoint> queue = new List<NavPoint>();
            queue.Add(start);
            while(queue.Count > 0)
            {
                NavPoint vertex = queue[0];
                queue.RemoveAt(0);

                if (_edges.ContainsKey(vertex))
                {
                    foreach (NavPoint point in _edges[vertex])
                    {
                        if(!visited.Contains(point))
                        {
                            visited.Add(point);
                            queue.Add(point);
                            parent.Add(point, vertex);
                        }
                    }
                }
            }
        }

        public NavPoint GetClosest(Vector2 loc)
        {
            NavPoint closest = null;
            float dist = float.MaxValue;
            foreach(NavPoint point in _pointMap._navPoints.Values)
            {
                float newDist = Vector2.DistanceSquared(loc, point._location);
                if (newDist < dist)
                {
                    closest = point;
                    dist = newDist;
                }
            }
            return closest;
        }

        public void DrawPaths(SpriteBatch spriteBatch, Dictionary<NavPoint, NavPoint> parent)
        {
            foreach(NavPoint start in parent.Keys)
            {
                if (parent[start] != null)
                {
                    spriteBatch.DrawLine(start._location, parent[start]._location, Color.BlueViolet);
                }
            }
        }

        public NavPoint GetRandomPath(NavPoint pos, Dictionary<NavPoint, NavPoint> parent, out Dictionary<NavPoint, NavPoint> path)
        {
            if (!parent.ContainsKey(pos))
            {
                BFS(pos, out parent); // populate tree with all possible edges
            }

            path = new Dictionary<NavPoint, NavPoint>();
            NavPoint endPoint = parent.Values.ElementAt(new Random().Next() % parent.Count); // choose point
            NavPoint point = endPoint;
            while(point != null && parent[point] != null) // while not first point
            {
                path.Add(point, parent[point]);
                point = parent[point];
            }

            return endPoint;
        }
    }
}
