using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;

namespace IngredientRun
{
    class CollisionHandler
    {
        protected Dictionary<string,List<CollisionBox>> _layers;
        protected Dictionary<string, List<string>> _collisionMask;
        protected Dictionary<string, List<string>> _overlapMask;

        public CollisionHandler()
        {
            _layers = new Dictionary<string, List<CollisionBox>>();
            _collisionMask = new Dictionary<string, List<string>>();
            _overlapMask = new Dictionary<string, List<string>>();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Check possible collision interactions
            foreach (List<CollisionBox> layer in _layers.Values)
            {
                foreach (CollisionBox box in layer)
                {
                    box.Draw(spriteBatch);
                } 
            }
        }

        public Vector2 TryMove(CollisionBox box, Vector2 newPos)
        {
            // Check collision
            Vector2 origPos = box._bounds.Position;
            Vector2 movePos = box._bounds.Position = newPos;
            foreach (string layer in _collisionMask[box._label])
            {
                List<CollisionBox> other = _layers[layer];
                if (origPos.X < newPos.X && origPos.Y < newPos.Y)
                {
                    for (int i = 0; i < other.Count; ++i)
                    {
                        RectangleF overlapRect;
                        RectangleF.Intersection(ref box._bounds, ref other[i]._bounds, out overlapRect);

                        if (!overlapRect.IsEmpty)
                        {
                            CollisionInfo info = new CollisionInfo(box, other[i], ref overlapRect);
                            box.CallCollision(info);
                            box._bounds.Position = movePos -= info._overlapDist * info._hitDir;
                        }
                    }
                }
                else
                {
                    for (int i = other.Count - 1; i >= 0; --i)
                    {
                        RectangleF overlapRect;
                        RectangleF.Intersection(ref box._bounds, ref other[i]._bounds, out overlapRect);

                        if (!overlapRect.IsEmpty)
                        {
                            CollisionInfo info = new CollisionInfo(box, other[i], ref overlapRect);
                            box.CallCollision(info);
                            box._bounds.Position = movePos -= info._overlapDist * info._hitDir;
                        }
                    }
                }
            }

            // Check overlap
            foreach (string layer in _overlapMask[box._label])
            {
                foreach(CollisionBox other in _layers[layer])
                {
                    RectangleF overlapRect;
                    RectangleF.Intersection(ref box._bounds, ref other._bounds, out overlapRect);

                    if (!overlapRect.IsEmpty)
                    {
                        CollisionInfo info = new CollisionInfo(box, other, ref overlapRect);
                        box.CallOverlap(info);
                    }
                }
            }

            return movePos;
        }

        public void AddLayer(string layerLabel)
        {
            if(!_layers.ContainsKey(layerLabel))
            {
                _layers.Add(layerLabel, new List<CollisionBox>());
                _collisionMask.Add(layerLabel, new List<string>());
                _overlapMask.Add(layerLabel, new List<string>());
            }
        }

        public bool AddObject(string layerLabel, CollisionBox obj)
        {
            if(_layers.ContainsKey(layerLabel))
            {
                _layers[layerLabel].Add(obj);
                obj._label = layerLabel;
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
    }

    class CollisionInfo
    {
        public string _otherLabel { get; } // label(type/mask) of contact object
        public Vector2 _loc { get; }  // center point of contact on edge of other
        public Vector2 _hitDir { get; set; } // direction vector of collision (points to side collided)
        public float _overlapDist { get; } // distance overlapped/penetrated
        public RectangleF _overlapRect { get; } // rectangle describing overlap

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
                    box1._upBlocked = true;
                }
                else // Bottom
                {
                    loc.Y += overlapRect.Height / 2;
                    box1._downBlocked = true;
                }
                overlapDist = overlapRect.Height;
            }
            else // left or right hit
            {
                hitDir.X = box1._bounds.Center.X > box2._bounds.Center.X ? -1 : 1;
                if (box1._bounds.Center.X > box2._bounds.Center.X) // Left
                {
                    loc.X -= overlapRect.Width / 2;
                    box1._leftBlocked = true;
                }
                else // Right
                {
                    loc.X += overlapRect.Width / 2;
                    box1._rightBlocked = true;
                }
                overlapDist = overlapRect.Width;
            }

            _otherLabel = otherLabel;
            _loc = loc;
            _hitDir = hitDir;
            _overlapDist = overlapDist;
            _overlapRect = overlapRect;
        }
    }
}
