using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Tiled;
using System;
using System.Collections.Generic;

namespace WillowWoodRefuge
{
    public class NavPointMap
    {
        public Dictionary<Point, NavPoint> _navPoints { get; private set; }
        public Size _entityTileSize { get; private set; }
        private int _tileSize;

        public NavPointMap(TiledMap tileMap, RectangleF collisionBox)
        {
            // Setup empty navpoint list
            _navPoints = new Dictionary<Point, NavPoint>();

            // Save tilesize
            _tileSize = tileMap.TileHeight;

            // Calculate size of entity hitbox in tiles
            _entityTileSize = new Size((int)Math.Ceiling(collisionBox.Width / tileMap.TileWidth), 
                                       (int)Math.Ceiling(collisionBox.Height / tileMap.TileHeight));

            // Generate map of nav points
            int platformIndex = 0;
            bool platformStarted = false;
            TiledMapTile? tile;
            TiledMapTileLayer tileLayer = tileMap.GetLayer<TiledMapTileLayer>("Walls");
            for(int y = 0; y < tileLayer.Height; ++y)
            {
                platformStarted = false;
                for (int x = 0; x < tileLayer.Width; ++x)
                {
                    Point tilePoint = new Point(x, y);
                    tileLayer.TryGetTile((ushort)tilePoint.X, (ushort)tilePoint.Y, out tile);
                    if (!tile.Value.IsBlank && isValidLocation(tilePoint, tileLayer)) // is valid location to stand
                    {
                        TiledMapTile? prevTile = tile;
                        tileLayer.TryGetTile((ushort)(tilePoint.X + 1), (ushort)tilePoint.Y, out tile); // check tile right of current
                        if (!platformStarted) // no started platform
                        {
                            if (!tile.HasValue || prevTile.Value.IsBlank ||
                                !isValidLocation(new Point(tilePoint.X + 1, tilePoint.Y), tileLayer)) // tilePoint is left solo
                            {
                                _navPoints.Add(tilePoint, new NavPoint(NavPointType.solo, platformIndex, 
                                               new Vector2(tilePoint.X * _tileSize, tilePoint.Y * _tileSize)));
                                ++platformIndex;
                            }
                            else // platform left start
                            {
                                _navPoints.Add(tilePoint, new NavPoint(NavPointType.leftEdge, platformIndex, 
                                               new Vector2(tilePoint.X * _tileSize, tilePoint.Y * _tileSize)));
                                
                                if(!tile.HasValue || tile.Value.IsBlank)
                                {
                                    _navPoints.Add(new Point(tilePoint.X + 1, tilePoint.Y), new NavPoint(NavPointType.rightEdge, platformIndex,
                                                   new Vector2((tilePoint.X + 1) * _tileSize, tilePoint.Y * _tileSize)));
                                }
                                else
                                {
                                    platformStarted = true;
                                }
                            }
                        }
                        else // platform already started
                        {
                            if (!tile.HasValue || tile.Value.IsBlank ||
                                !isValidLocation(new Point(tilePoint.X + 1, tilePoint.Y), tileLayer)) // platform right end
                            {
                                if (isValidLocation(new Point(tilePoint.X + 1, tilePoint.Y), tileLayer)) // overhang edge
                                {
                                    _navPoints.Add(tilePoint, new NavPoint(NavPointType.platform, platformIndex, 
                                                   new Vector2(tilePoint.X * _tileSize, tilePoint.Y * _tileSize)));
                                    _navPoints.Add(new Point(tilePoint.X + 1, tilePoint.Y), new NavPoint(NavPointType.rightEdge, platformIndex, 
                                                   new Vector2((tilePoint.X + 1) * _tileSize, tilePoint.Y * _tileSize)));
                                }
                                else // against wall
                                {
                                    _navPoints.Add(tilePoint, new NavPoint(NavPointType.rightEdge, platformIndex, 
                                                   new Vector2(tilePoint.X * _tileSize, tilePoint.Y * _tileSize)));
                                }
                                ++platformIndex;
                                platformStarted = false;
                            }
                            else // platform continued
                            {
                                _navPoints.Add(tilePoint, new NavPoint(NavPointType.platform, platformIndex, 
                                               new Vector2(tilePoint.X * _tileSize, tilePoint.Y * _tileSize)));
                            }
                        }
                    }
                    else if(!tile.Value.IsBlank)
                    {
                        tileLayer.TryGetTile((ushort)(tilePoint.X + 1), (ushort)tilePoint.Y, out tile); // check tile right of current
                        if (tile.HasValue && tile.Value.IsBlank &&
                             isValidLocation(new Point(tilePoint.X + 1, tilePoint.Y), tileLayer)) // tilePoint is right solo
                        {
                            _navPoints.Add(new Point(tilePoint.X + 1, tilePoint.Y), new NavPoint(NavPointType.solo, platformIndex, 
                                           new Vector2(tilePoint.X * _tileSize, tilePoint.Y * _tileSize)));
                            ++platformIndex;
                        } 
                    }
                }
            }
        }

        // returns true if collision box fits in space above tile(with lower left corner of collision box on given tile)
        bool isValidLocation(Point navPoint, TiledMapTileLayer tileLayer)
        {
            TiledMapTile? tile;
            for (int x = -(int)Math.Ceiling(_entityTileSize.Width / 2f); x < (int)Math.Ceiling(_entityTileSize.Width / 2f); ++x)
            {
                for(int y = 1; y <= _entityTileSize.Height; ++y)
                {
                    tileLayer.TryGetTile((ushort)(navPoint.X + x), (ushort)(navPoint.Y - y), out tile);
                    if(!tile.HasValue || !tile.Value.IsBlank)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public void Draw(SpriteBatch spriteBatch, bool isDebug)
        {
            if(!isDebug)
            {
                return;
            }

            foreach(Point navPoint in _navPoints.Keys)
            {
                switch(_navPoints[navPoint]._pointType)
                {
                    case NavPointType.leftEdge:
                        spriteBatch.DrawPoint(new Vector2((navPoint.X) * _tileSize, navPoint.Y * _tileSize), Color.Purple, 4);
                        break;
                    case NavPointType.platform:
                        spriteBatch.DrawPoint(new Vector2((navPoint.X) * _tileSize, navPoint.Y * _tileSize), Color.Black, 4);
                        break;
                    case NavPointType.rightEdge:
                        spriteBatch.DrawPoint(new Vector2((navPoint.X) * _tileSize, navPoint.Y * _tileSize), Color.Green, 4);
                        break;
                    case NavPointType.solo:
                        spriteBatch.DrawPoint(new Vector2((navPoint.X) * _tileSize, navPoint.Y * _tileSize), Color.DarkOrange, 4);
                        break;
                }
            }
        }
    }
}
