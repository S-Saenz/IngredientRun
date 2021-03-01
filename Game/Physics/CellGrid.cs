using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace WillowWoodRefuge
{
    class CellGrid
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
            Vector2 gridLoc = worldToGrid(box._bounds.Position);

            if (_container.ContainsKey(gridLoc))
            {
                _container[gridLoc].Add(box);
            }
            else
            {
                _container.Add(gridLoc, new List<CollisionBox>());
                _container[gridLoc].Add(box);
            }
        }

        public CollisionBox removeElement(CollisionBox box)
        {
            Vector2 gridLoc = worldToGrid(box._bounds.Position);

            if (_container.ContainsKey(gridLoc))
            {
                for(int i = 0; i < _container[gridLoc].Count; ++i)
                {
                    if(_container[gridLoc][i] == box)
                    {
                        _container[gridLoc].RemoveAt(i);
                        return box;
                    }
                }
            }
            return null;
        }

        // Checks if box is still in correct cell and moves it to correct cell if it isn't. returns wasCorrect bool
        public bool CheckBox(CollisionBox box, Vector2 prevLoc)
        {
            Vector2 gridLoc = worldToGrid(prevLoc);
            bool expiredPrevLoc = false; // whether previous location actually contains box

            // check if still in same cell
            if(gridLoc == worldToGrid(box._bounds.Position) && _container.ContainsKey(gridLoc))
            {
                for (int i = 0; i < _container[gridLoc].Count; ++i)
                {
                    if (_container[gridLoc][i] == box)
                    {
                        return true;
                    }
                }
                expiredPrevLoc = true;
            }

            // find in prev cell and move to new cell
            if (_container.ContainsKey(gridLoc) && !expiredPrevLoc)
            {
                for (int i = 0; i < _container[gridLoc].Count; ++i)
                {
                    if (_container[gridLoc][i] == box)
                    {
                        _container[gridLoc].RemoveAt(i);
                        return false;
                    }
                }
            }
            
            // if prev cell doesn't contain elem, search entire container for elem
            foreach(List<CollisionBox> cell in _container.Values)
            {
                for (int i = 0; i < cell.Count; ++i)
                {
                    if (cell[i] == box)
                    {
                        cell.RemoveAt(i);
                        return false;
                    }
                }
            }

            addElement(box);
            if(_container[gridLoc].Count == 0)
            {
                _container.Remove(gridLoc);
            }
            return false;
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
            foreach(List<CollisionBox> list in _container.Values)
            {
                foreach(CollisionBox box in list)
                {
                    spriteBatch.DrawRectangle(box._bounds, color, 0.5f);

                    spriteBatch.DrawLine(box._bounds.Center, box._bounds.Center + box._velocity / 2, Color.Aquamarine, 0.5f);

                    foreach (CollisionInfo info in box._downInfo)
                    {
                        spriteBatch.DrawRectangle(info._overlapRect, Color.Red);
                    }
                    foreach (CollisionInfo info in box._upInfo)
                    {
                        spriteBatch.DrawRectangle(info._overlapRect, Color.Red);
                    }
                    foreach (CollisionInfo info in box._rightInfo)
                    {
                        spriteBatch.DrawRectangle(info._overlapRect, Color.Red);
                    }
                    foreach (CollisionInfo info in box._leftInfo)
                    {
                        spriteBatch.DrawRectangle(info._overlapRect, Color.Red);
                    }
                }
            }

            // draw cell grid
            foreach(Vector2 loc in _container.Keys)
            {
                spriteBatch.DrawRectangle(loc.X * _dimension, loc.Y * _dimension, _dimension, _dimension,
                                          _checked.Contains(loc) ? Color.HotPink : Color.White,
                                          _checked.Contains(loc) ? 1 : 0.25f);
            }
        }
    }
}
