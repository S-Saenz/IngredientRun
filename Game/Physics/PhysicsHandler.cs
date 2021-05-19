using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;

namespace WillowWoodRefuge
{
    public class PhysicsHandler
    {
        protected Dictionary<string, CellGrid> _layers;
        protected Dictionary<string, List<string>> _collisionMask;
        protected Dictionary<string, List<string>> _overlapMask;

        private static Dictionary<string, Color> _layerColor = new Dictionary<string, Color>()
        {
            { "Player", Color.LawnGreen },
            { "Enemy", Color.Orange },
            { "Pickup", Color.Blue },
            { "Walls", Color.Black },
            { "Areas", Color.Yellow },
            { "NPC", Color.BlueViolet }
        };

        public PhysicsHandler()
        {
            _layers = new Dictionary<string, CellGrid>();
            _collisionMask = new Dictionary<string, List<string>>();
            _overlapMask = new Dictionary<string, List<string>>();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Check possible collision interactions
            foreach (CellGrid layer in _layers.Values)
            {
                foreach (CollisionBox box in layer.getList())
                {
                    box.Draw(spriteBatch);
                } 
            }
        }

        public void Draw(SpriteBatch spriteBatch, string layer)
        {
            foreach (CollisionBox box in _layers[layer].getList())
            {
                box.Draw(spriteBatch);
            }
        }

        public Vector2 TryMove(CollisionBox box, Vector2 newPos)
        {
            // Check collision
            Vector2 origPos = box._bounds.Position;
            Vector2 movePos = box._bounds.Position = newPos;
            RectangleF overlapRect;
            foreach (string layer in _collisionMask[box._label])
            {
                List<CollisionBox> other = _layers[layer].getNeighbors(box);
                List<Vector2> priority = new List<Vector2>(); // x = index of box, y = priority
                for (int i = 0; i < other.Count; ++i)
                {
                    RectangleF.Intersection(ref box._bounds, ref other[i]._bounds, out overlapRect);
                    if (!overlapRect.IsEmpty)
                    {
                        priority.Add(new Vector2(i, Math.Abs(Vector2.Distance(box._bounds.Center, other[i]._bounds.Center))));
                    }
                }
                priority.Sort(delegate(Vector2 obj1, Vector2 obj2)
                              {
                                  if (obj1.Y > obj2.Y)
                                  {
                                      return 1;
                                  }
                                  else if (obj1.Y == obj2.Y)
                                  {
                                      return 0;
                                  }
                                  else
                                  {
                                      return -1;
                                  }
                              });
                foreach(Vector2 obj in priority)
                {
                    RectangleF.Intersection(ref box._bounds, ref other[(int)obj.X]._bounds, out overlapRect);
                    CollisionInfo info = new CollisionInfo(box, other[(int)obj.X], ref overlapRect);
                    box.CallCollision(info);
                    if(info._hitDir == new Vector2(-1, 0)) // left
                    {
                        movePos.X = other[(int)obj.X]._bounds.Right;
                    }
                    else if (info._hitDir == new Vector2(1, 0)) // right
                    {
                        movePos.X = other[(int)obj.X]._bounds.Left - box._bounds.Width;
                    }
                    else if (info._hitDir == new Vector2(0, -1)) // up
                    {
                        movePos.Y = other[(int)obj.X]._bounds.Bottom;
                    }
                    else if (info._hitDir == new Vector2(0, 1)) // down
                    {
                        movePos.Y = other[(int)obj.X]._bounds.Top - box._bounds.Height;
                    }

                    // if (Math.Abs(info._overlapDist) > _correctionError)
                    // {
                    //     movePos -= info._overlapDist * info._hitDir;
                    // }
                    box._bounds.Position = movePos;
                }
            }

            // Check overlap
            foreach (string layer in _overlapMask[box._label])
            {
                foreach(CollisionBox other in _layers[layer].getNeighbors(box))
                {
                    RectangleF.Intersection(ref box._bounds, ref other._bounds, out overlapRect);
            
                    if (overlapRect.Width != 0 && overlapRect.Height != 0)
                    {
                        OverlapInfo info = new OverlapInfo(box, other, ref overlapRect);
                        box.CallOverlap(info);
                    }
                }
            }

            // Check world bounds
            if(!(box._worldBounds.Width == 0 || box._worldBounds.Height == 0))
            {
                RectangleF.Intersection(ref box._bounds, ref box._worldBounds, out overlapRect);
                if (box._worldBounds.Top > movePos.Y) // out top
                {
                    movePos.Y = box._worldBounds.Top;
                    box._upBlocked = true;
                }
                if(box._worldBounds.Bottom < movePos.Y + box._bounds.Height) // out bottom
                {
                    movePos.Y = box._worldBounds.Bottom - box._bounds.Height;
                    box._downBlocked = true;
                }
                if(box._worldBounds.Left > movePos.X) // out left
                {
                    movePos.X = box._worldBounds.Left;
                    box._leftBlocked = true;
                }
                if(box._worldBounds.Right < movePos.X + box._bounds.Width) // out right
                {
                    movePos.X = box._worldBounds.Right - box._bounds.Width;
                    box._rightBlocked = true;
                }
            }

            _layers[box._label].CheckBox(box, origPos);
            return movePos;
        }

        public void AddLayer(string layerLabel)
        {
            if(!_layers.ContainsKey(layerLabel))
            {
                _layers.Add(layerLabel, new CellGrid());
                _collisionMask.Add(layerLabel, new List<string>());
                _overlapMask.Add(layerLabel, new List<string>());
            }
        }

        public bool AddObject(string layerLabel, CollisionBox obj)
        {
            if(_layers.ContainsKey(layerLabel))
            {
                _layers[layerLabel].addElement(obj);
                obj._label = layerLabel;
                return true;
            }
            return false;
        }

        public bool RemoveObject(CollisionBox obj)
        {
            if(_layers.ContainsKey(obj._label))
            {
                _layers[obj._label].removeElement(obj);
                return true;
            }
            return false;
        }

        public bool SetCollision(string layer1, string layer2)
        {
            if(_collisionMask.ContainsKey(layer1) && !_collisionMask[layer1].Contains(layer2))
            {
                _collisionMask[layer1].Add(layer2);
                return true;
            }
            return false;
        }

        public bool SetOverlap(string layer1, string layer2)
        {
            if (_overlapMask.ContainsKey(layer1) && !_overlapMask[layer1].Contains(layer2))
            {
                _overlapMask[layer1].Add(layer2);
                return true;
            }
            return false;
        }

        public List<OverlapInfo> IsOverlapping(CollisionBox box)
        {
            List<OverlapInfo> others = new List<OverlapInfo>();
            foreach (string layer in _overlapMask[box._label])
            {
                foreach (CollisionBox other in _layers[layer].getNeighbors(box))
                {
                    RectangleF overlapRect;
                    RectangleF.Intersection(ref box._bounds, ref other._bounds, out overlapRect);

                    if (overlapRect.Width != 0 && overlapRect.Height != 0)
                    {
                        OverlapInfo info = new OverlapInfo(box, other, ref overlapRect);
                        others.Add(info);
                    }
                }
            }
            return others;
        }

        public bool CanFit(CollisionBox box, Vector2 pos, float yClearance = 0)
        {
            RectangleF newPosBox = box._bounds;
            newPosBox.Position = pos;
            newPosBox.Height += yClearance;
            RectangleF overlapRect;
            foreach (string layer in _collisionMask[box._label])
            {
                foreach (CollisionBox other in _layers[layer].getNeighbors(newPosBox))
                {
                    RectangleF.Intersection(ref newPosBox, ref other._bounds, out overlapRect);

                    if (overlapRect.Width > 0 && overlapRect.Height > 0)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public void CheckBox(CollisionBox box, Vector2 prevLoc)
        {
            _layers[box._label].CheckBox(box, prevLoc);
        }

        public void DrawDebug(SpriteBatch spriteBatch)
        {
            foreach(string layer in _layers.Keys)
            {
                if(_layerColor.ContainsKey(layer))
                {
                    _layers[layer].DrawDebug(spriteBatch, _layerColor[layer]);
                }
                else
                {
                    _layers[layer].DrawDebug(spriteBatch, Color.Gray);
                }
            }

            // clear checked
            foreach (CellGrid grid in _layers.Values)
            {
                grid._checked.Clear();
            }
        }
    }
}
