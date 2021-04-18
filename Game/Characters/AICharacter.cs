using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace WillowWoodRefuge
{
    public enum AIState { Wander, Converse, Stop, Attack, }
    abstract public class AICharacter : BaseCharacter
    {
        // random number generator
        protected Random _rand = new Random();

        // navigation constants
        protected NavMesh _navMesh;
        protected Area _area;
        protected float _proximityCut = 1f;

        // navigation variables
        protected Dictionary<NavPoint, NavPoint> _possibleMoves;
        protected Dictionary<NavPoint, NavPoint> _currPath;
        protected float _lastDist;
        protected NavPoint _currTarget;
        protected NavPoint _currPos;
        protected Vector2 _target;
        protected Vector2 _attackTarget;
        protected float _proxRange = 5;

        // temp texture until animation set up
        protected Texture2D _texture;
        
        // State info
        protected AIState _currState = AIState.Wander;
        protected bool _isMoving = false;
        protected bool _isSitting = false;

        // timer
        protected float _moveTimer;
        protected bool _timerStopped = false;
        protected Vector2 _timerRange = new Vector2(8, 20);

        public AICharacter(string name, Vector2 pos, string collisionLabel, Vector2 bounds, PhysicsHandler collisionHandler, 
                           RectangleF worldBounds = default, Dictionary<string, Animation> animationDict = null, Area area = null) 
                           : base(name, pos, collisionLabel, bounds, collisionHandler, worldBounds, animationDict)
        {
            _area = area;
            
            switch (collisionLabel)
            {
                case "NPC":
                    _texture = Game1.instance.Content.Load<Texture2D>("chars/" + name);
                    break;
                case "Enemy":
                    _texture = Game1.instance.Content.Load<Texture2D>("monsters/" + name);
                    break;
            }

            // offset position
            _pos -= new Vector2(_texture.Width * _scale / 2, _texture.Height * _scale);

            collisionHandler.RemoveObject(_collisionBox); // remove default collision box
            _collisionBox = new CollisionBox(new RectangleF(_pos, new Size2(_texture.Width * _scale, _texture.Height * _scale)),
                collisionHandler, this, worldBounds, maxSpeed: new Vector2(_runSpeed, 500), friction: _friction);
            collisionHandler.AddObject(collisionLabel, _collisionBox);

            // setup _pos for texture
            _pos = _collisionBox._bounds.Center;

            // add navigation mesh
            _navMesh = new NavMesh(Game1.instance.GetCurrentTilemap().GenerateNavPointMap(_collisionBox._bounds), area: area);

            _possibleMoves = _navMesh.GetAllPossible(_pos + new Vector2(0, _collisionBox._bounds.Height / 2));

            _moveTimer = _rand.Next() % (_timerRange.Y - _timerRange.X) + _timerRange.X;
        }

        public void Update(GameTime gameTime)
        {
            switch(_currState)
            {
                case AIState.Wander:
                    WanderUpdate(gameTime);
                    break;
                case AIState.Converse:
                    ConverseUpdate(gameTime);
                    break;
                case AIState.Stop:
                    StopUpdate(gameTime);
                    break;
                case AIState.Attack:
                    AttackUpdate(gameTime);
                    break;
            }
            base.Update(gameTime);
        }

        private void WanderUpdate(GameTime gameTime)
        {
            // increment timer
            if(!_timerStopped)
            {
                _moveTimer -= gameTime.GetElapsedSeconds();
            }

            // timer ended, start new wander
            if(_moveTimer <= 0)
            {
                Wander();
                Debug.WriteLine(name + ((_currTarget != null) ? (" started wandering to " + _currTarget._location) : " sat down"));
            }

            // move
            if (_isMoving)
            {
                base.Update(gameTime, new Vector2(_pos.X < _currTarget._location.X ? 1 : -1, 0), true);
                float newDist = Vector2.Distance(_pos + new Vector2(0, _collisionBox._bounds.Height / 2), _currTarget._location);

                if (newDist >= _lastDist) // moved further away from target point
                {
                    _timerStopped = false;
                    _isMoving = false;
                    Debug.WriteLine(name + "'s path broken.");
                    if (!_possibleMoves.ContainsKey(_navMesh.GetClosest(_pos))) // off of determined possible paths
                    {
                        _possibleMoves = _navMesh.GetAllPossible(_pos + new Vector2(0, _collisionBox._bounds.Height / 2));
                    }
                }
                else if (newDist < _proximityCut) // reached point
                {
                    if (_currPath.ContainsKey(_currTarget)) // another point in path
                    {
                        _currTarget = _currPath[_currTarget];
                        _lastDist = Vector2.Distance(_pos + new Vector2(0, _collisionBox._bounds.Height / 2), _currTarget._location);
                    }
                    else // target reached
                    {
                        // stop moving, restart timer
                        _timerStopped = false;
                        _isMoving = false;
                    }
                }
                else
                {
                    _lastDist = newDist;
                }
            }
            else // not moving, stop moving
            {
                base.Update(gameTime, Vector2.Zero, false);
            }
        }

        private void ConverseUpdate(GameTime gameTime)
        {

        }

        private void StopUpdate(GameTime gameTime)
        {

        }

        private void AttackUpdate(GameTime gameTime)
        {
            // move
            if (Vector2.Distance(_pos + new Vector2(0, _collisionBox._bounds.Height / 2), _attackTarget) > _proxRange)
            {
                if(_navMesh.GetClosest(_target) != _navMesh.GetClosest(_attackTarget))
                {
                    _target = _attackTarget;
                    _possibleMoves = _navMesh.GetAllPossible(_pos);
                    _currPos = _navMesh.GetClosest(_pos + new Vector2(0, _collisionBox._bounds.Height / 2));
                    NavPoint newTarget = _navMesh.GetClosest(_attackTarget, _possibleMoves);
                    _currTarget = _navMesh.GetPath(_currPos, newTarget, _possibleMoves, out _currPath);
                    _target = _currTarget._location;
                }
                base.Update(gameTime, new Vector2(_pos.X < _currTarget._location.X ? 1 : -1, 0), true);
                float newDist = Vector2.Distance(_pos + new Vector2(0, _collisionBox._bounds.Height / 2), _currTarget._location);

                if (newDist < _proximityCut) // reached point
                {
                    if (_currPath.ContainsKey(_currTarget)) // another point in path
                    {
                        _currTarget = _currPath[_currTarget];
                        _lastDist = Vector2.Distance(_pos + new Vector2(0, _collisionBox._bounds.Height / 2), _currTarget._location);
                    }
                    else // target reached
                    {
                        _currState = AIState.Wander;
                    }
                }
            }
            else // not moving, stop moving
            {
                // hit player
                base.Update(gameTime, Vector2.Zero, false);
            }
        }

        public void DrawDebug(SpriteBatch spriteBatch)
        {
            if (_isMoving && _currTarget != null)
            {
                spriteBatch.DrawLine(_target - new Vector2(0, _collisionBox._bounds.Height / 2), _pos, Color.Crimson, 1);
                spriteBatch.DrawLine(_currTarget._location - new Vector2(0, _collisionBox._bounds.Height / 2), _pos, Color.DarkGoldenrod, 1);
            }

            _navMesh.DrawDebug(spriteBatch);
            _navMesh.DrawPaths(spriteBatch, _possibleMoves);
        }

        private void Wander()
        {
            _currPos = _navMesh.GetClosest(_pos);
            _moveTimer = _rand.Next() % (_timerRange.Y - _timerRange.X) + _timerRange.X;
            _timerStopped = true;
            _possibleMoves = _navMesh.GetAllPossible(_pos);
            _target = _navMesh.GetRandomPath(_currPos, _possibleMoves, out _currPath)._location;

            if (_target != null && _currPath.ContainsKey(_currPos))
            {
                _currTarget = _currPath[_currPos];
                _lastDist = Vector2.Distance(_pos + new Vector2(0, _collisionBox._bounds.Height / 2), _currTarget._location);
                _isMoving = true;
            }
            else
            {
                _isMoving = false;
                _timerStopped = false;
            }
        }
    }
}
