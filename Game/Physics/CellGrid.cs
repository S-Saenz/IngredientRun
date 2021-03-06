using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace WillowWoodRefuge
{
    public class CellGrid
    {
        Dictionary<Vector2, List<CollisionBox>> _container;
        public List<Vector2> _checked = new List<Vector2>();
        float _dimension;

        public CellGrid(float dimension = 64)
        {
            _container = new Dictionary<Vector2, List<CollisionBox>>();
            _dimension = dimension;
        }

        public Vector2 worldToGrid(Vector2 worldLoc)
        {
            return new Vector2(MathF.Floor(worldLoc.X / _dimension), MathF.Floor(worldLoc.Y / _dimension));
        }

        public Vector2 gridToWorld(Vector2 gridLoc)
        {
            return gridLoc * _dimension + new Vector2(_dimension / 2, _dimension / 2);
        }

        public void addElement(CollisionBox box)
        {
            Vector2 minPoint = worldToGrid(box._bounds.TopLeft);
            Vector2 maxPoint = worldToGrid(box._bounds.BottomRight);

            for (int x = (int)minPoint.X; x <= maxPoint.X; ++x)
            {
                for (int y = (int)minPoint.Y; y <= maxPoint.Y; ++y)
                {
                    Vector2 cell = new Vector2(x, y);
                    if (_container.ContainsKey(cell))
                    {
                        _container[cell].Add(box);
                    }
                    else
                    {
                        _container.Add(cell, new List<CollisionBox>());
                        _container[cell].Add(box);
                    }
                    box._cells.Add(cell);
                }
            }
        }

        public CollisionBox removeElement(CollisionBox box)
        {
            foreach(Vector2 cell in box._cells)
            {
                _container[cell].Remove(box);
                if(_container[cell].Count == 0)
                {
                    _container.Remove(cell);
                }
            }
            box._cells.Clear();
            return box;
        }

        // Checks if box is still in correct cell and moves it to correct cell if it isn't. returns wasCorrect bool
        public void CheckBox(CollisionBox box, Vector2 prevLoc)
        {
            // hasn't moved
            if((Vector2)box._bounds.Position == prevLoc)
            {
                return;
            }

            // check all corners
            if(box._cells.Contains(worldToGrid(box._bounds.TopLeft)) &&
               box._cells.Contains(worldToGrid(box._bounds.TopRight)) &&
               box._cells.Contains(worldToGrid(box._bounds.BottomLeft)) &&
               box._cells.Contains(worldToGrid(box._bounds.BottomRight)))
            {
                return;
            }

            removeElement(box);

            addElement(box);
        }

        public List<CollisionBox> getNeighbors(CollisionBox box)
        {
            List<CollisionBox> neighbors = new List<CollisionBox>();

            Vector2 lowerBound = worldToGrid(box._bounds.TopLeft);// - new Vector2(1,1);
            Vector2 upperBound = worldToGrid(box._bounds.BottomRight);// + new Vector2(1,1);

            for(int x = (int)lowerBound.X; x <= (int)upperBound.X; ++x)
            {
                for(int y = (int)lowerBound.Y; y <= (int)upperBound.Y; ++y)
                {
                    if(_container.ContainsKey(new Vector2(x, y)))
                    {
                        _checked.Add(new Vector2(x, y));
                        foreach (CollisionBox other in _container[new Vector2(x, y)])
                        {
                            neighbors.Add(other);
                        }
                    }
                }
            }

            return neighbors;
        }

        // Returns list of all elements in layer
        public List<CollisionBox> getList()
        {
            List<CollisionBox> all = new List<CollisionBox>();

            foreach (List<CollisionBox> cell in _container.Values)
            {
                for (int i = 0; i < cell.Count; ++i)
                {
                    all.Add(cell[i]);
                }
            }

            return all;
        }

        public void DrawDebug(SpriteBatch spriteBatch, Color color)
        {
            // draw collision boxes
            foreach (List<CollisionBox> list in _container.Values)
            {
                foreach(CollisionBox box in list)
                {
                    spriteBatch.DrawRectangle(box._bounds, color, 1f);

                    spriteBatch.DrawLine(box._bounds.Center, box._bounds.Center + box._velocity / 2, Color.Aquamarine, 1f);

                    // foreach (CollisionInfo info in box._downInfo)
                    // {
                    //     spriteBatch.DrawRectangle(info._overlapRect, Color.Red);
                    // }
                    // foreach (CollisionInfo info in box._upInfo)
                    // {
                    //     spriteBatch.DrawRectangle(info._overlapRect, Color.Red);
                    // }
                    // foreach (CollisionInfo info in box._rightInfo)
                    // {
                    //     spriteBatch.DrawRectangle(info._overlapRect, Color.Red);
                    // }
                    // foreach (CollisionInfo info in box._leftInfo)
                    // {
                    //     spriteBatch.DrawRectangle(info._overlapRect, Color.Red);
                    // }

                    if (!box._upBox.IsEmpty)
                    {
                        spriteBatch.DrawRectangle(box._upBox, Color.Red, 1);
                    }
                    if (!box._downBox.IsEmpty)
                    {
                        spriteBatch.DrawRectangle(box._downBox, Color.Red, 1);
                    }
                    if (!box._leftBox.IsEmpty)
                    {
                        spriteBatch.DrawRectangle(box._leftBox, Color.Red, 1);
                    }
                    if (!box._rightBox.IsEmpty)
                    {
                        spriteBatch.DrawRectangle(box._rightBox, Color.Red, 1);
                    }
                }
            }

            // draw cell grid
            foreach (Vector2 loc in _container.Keys)
            {
                spriteBatch.DrawRectangle(loc.X * _dimension, loc.Y * _dimension, _dimension, _dimension,
                                          _checked.Contains(loc) ? Color.HotPink : Color.White,
                                          _checked.Contains(loc) ? 1.5f : 1f);
            }
        }
    }
}
