using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;

namespace WillowWoodRefuge
{
    public class ForageSpot : IPhysicsObject
    {
        public CollisionBox _bounds { get; protected set; }
        public string _rangeType { get; protected set; } // any, only, family (what pool of possible items to spawn
        public string _spawnType { get; protected set; } // item name or family name (null if rangeType any)
        public float _growDuration { get; protected set; } // how long it takes before it can be foraged again
        public bool _isRipe { get; protected set; } // whether item can currently be foraged or not
        public string _currSpawn { get; protected set; } // the name of the current item being grown
        public int _numPhases { get; protected set; } // number of texture phases

        // Static container of all forage spots
        static protected List<ForageSpot> _spots = new List<ForageSpot>();
        static public List<ForageSpot> _forageSpots { get { return _spots; } }

        // Timer variables
        public float _timeElapsed { get; protected set; } // current time on clock
        public float _growPercent { get; protected set; } // 0-1 growing progress
        private bool _isPaused = false;

        public ForageSpot(Vector2 pos, string rangeType, PhysicsHandler physicsHandler, string spawnType = null)
        {
            _rangeType = rangeType;
            _spawnType = spawnType;
            _isRipe = false;

            if (rangeType == "only")
            {
                _currSpawn = _spawnType;
            }
            else
            {

            }

            ForageInfo info = ForageInfo.GetInfo(_spawnType);
            _numPhases = info != null ? info._numPhases : 0;
            _bounds = new CollisionBox(new RectangleF(pos, TextureAtlasManager.GetSize("Foraging",
                                           _spawnType + _numPhases)),
                                           physicsHandler, this);
            _bounds._bounds.Position -= new Vector2(_bounds._bounds.Width / 2, _bounds._bounds.Height);
            physicsHandler.AddObject("Foraging", _bounds);
            _growDuration = info != null ? info._growDuration : 0;
            _timeElapsed = 0;
            _growPercent = 0;
            _forageSpots.Add(this);
        }

        public void Update(GameTime gameTime)
        {
            if (!_isPaused)
            {
                _timeElapsed += gameTime.GetElapsedSeconds();
                if (_timeElapsed >= _growDuration) // end of timer reached
                {
                    _isPaused = true;
                    _isRipe = true;
                    _growPercent = 1;
                    _timeElapsed = 0;
                }
                else
                {
                    _growPercent = _timeElapsed / _growDuration;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            TextureAtlasManager.DrawTexture(spriteBatch, "Foraging", _currSpawn + (Math.Floor(_growPercent * (_numPhases - 1)) + 1),
                                            _bounds._bounds.Position, Color.White);
        }

        public string TryHarvest()
        {
            if (_isRipe)
            {
                _isPaused = false;
                _isRipe = false;
                _growPercent = 0;
                return _currSpawn;
            }
            else
            {
                return null;
            }
        }
    }
}
