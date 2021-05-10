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
        protected NavPoint _target;
        protected Vector2 _interestTarget;
        protected float _proxRange = 5;
        protected string _scene;
        protected bool _inConversation = false;
        protected Point? _occupying = null;

        // this is dumb but everyone needs to know what points are occupied
        protected static Dictionary<string, List<Point>> _occupied = new Dictionary<string, List<Point>>();
        public static Dictionary<string, List<Point>> _occupiedPoints { get { return _occupied; } }

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

        public AICharacter(string name, Vector2 pos, string collisionLabel, Vector2 bounds, PhysicsHandler collisionHandler, string scene, 
                           RectangleF worldBounds = default, Dictionary<string, Animation> animationDict = null, Area area = null) 
                           : base(name, pos, collisionLabel, bounds, collisionHandler, worldBounds, animationDict)
        {
            _area = area;
            _scene = scene;
            if (!_occupied.ContainsKey(_scene))
                _occupied.Add(_scene, new List<Point>());
            
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

            // collisionHandler.RemoveObject(_collisionBox); // remove default collision box
            _collisionBox = new CollisionBox(new RectangleF(_pos, new Size2(_texture.Width * _scale, _texture.Height * _scale)),
                collisionHandler, this, worldBounds, maxSpeed: new Vector2(_runSpeed, 500), friction: _friction);
            _collisionBox.AddMovementStartListener(onStartMove);
            _collisionBox.AddMovementChangeDirectionListener(onChangeDirection);
            collisionHandler.AddObject(collisionLabel, _collisionBox);

            // add navigation mesh
            _navMesh = new NavMesh(Game1.instance.GetCurrentTilemap().GenerateNavPointMap(_collisionBox._bounds), scene, area: area);

            _currPos = _navMesh.GetClosest(_pos + new Vector2(0, _collisionBox._bounds.Height / 2), scene);
            _possibleMoves = _navMesh.GetAllPossible(_currPos);
            _collisionBox._bounds.Position = _currPos._location - new Vector2(_texture.Width * _scale / 2, _texture.Height * _scale);
            _occupied[_scene].Add(_currPos._tileLoc);
            _occupying = _currPos._tileLoc;

            _moveTimer = _rand.Next() % (_timerRange.Y - _timerRange.X) + _timerRange.X;

            // setup _pos for texture
            _pos = _collisionBox._bounds.Center;
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

            // move
            if(_isMoving)
            {
                MoveUpdate(gameTime);
            }
            else // not moving, stop moving
            {
                Update(gameTime, Vector2.Zero, false);
            }
        }

        public void ChangeState(AIState newState)
        {
            // leave old state
            switch(_currState)
            {
                case AIState.Wander:
                    LeaveWanderState();
                    break;
                case AIState.Converse:
                    LeaveConverseState();
                    break;
                case AIState.Stop:
                    LeaveStopState();
                    break;
                case AIState.Attack:
                    LeaveAttackState();
                    break;
            }

            _currState = newState;
            // enter new state
            switch (_currState)
            {
                case AIState.Wander:
                    StartWanderState();
                    break;
                case AIState.Converse:
                    StartConverseState();
                    break;
                case AIState.Stop:
                    StartStopState();
                    break;
                case AIState.Attack:
                    StartAttackState();
                    break;
            }
        }

        private void MoveUpdate(GameTime gameTime)
        {
            Update(gameTime, new Vector2(_pos.X < _currTarget._location.X ? 1 : -1, 0), true);
            float newDist = Vector2.Distance(_pos + new Vector2(0, _collisionBox._bounds.Height / 2), _currTarget._location);

            if (newDist >= _lastDist) // moved further away from target point
            {
                _timerStopped = false;
                _isMoving = false;
                Debug.WriteLine(name + "'s path broken.");
                if (!_possibleMoves.ContainsKey(_navMesh.GetClosest(_pos, _scene))) // off of determined possible paths
                {
                    if(_occupying.HasValue)
                        _occupied[_scene].Remove(_occupying.Value);
                    _currPos = _navMesh.GetClosest(_pos + new Vector2(0, _collisionBox._bounds.Height / 2), _scene);
                    _possibleMoves = _navMesh.GetAllPossible(_currPos);
                    _occupying = _currPos._tileLoc;
                    _occupied[_scene].Add(_occupying.Value);
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
                    _currPos = _currTarget;
                }
            }
            else
            {
                _lastDist = newDist;
            }
        }

        private void WanderUpdate(GameTime gameTime)
        {
            // increment timer
            if(!_timerStopped)
            {
                _moveTimer -= gameTime.GetElapsedSeconds();
            }

            if(_timerStopped && !_isMoving)
            {
                _timerStopped = false;
            }

            // timer ended, start new wander
            if(_moveTimer <= 0)
            {
                Wander();
                Debug.WriteLine(name + ((_currTarget != null) ? (" started wandering to " + _currTarget._location) : " sat down"));
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
            NavPoint attackTarget = _navMesh.GetClosest(_interestTarget, _possibleMoves, _scene, _target?._tileLoc);
            if (_target != attackTarget) // target has moved and another point in possible points is closer
            {
                MoveTo(attackTarget);
            }
        }

        public void DrawDebug(SpriteBatch spriteBatch)
        {
            if (_isMoving && _currTarget != null)
            {
                spriteBatch.DrawLine(_target._location - new Vector2(0, _collisionBox._bounds.Height / 2), _pos, Color.Crimson, 1);
                spriteBatch.DrawLine(_currTarget._location - new Vector2(0, _collisionBox._bounds.Height / 2), _pos, Color.DarkGoldenrod, 1);
            }

            _navMesh.DrawDebug(spriteBatch);
            _navMesh.DrawPaths(spriteBatch, _possibleMoves);
            // spriteBatch.DrawPoint(_currPos._location, Color.BurlyWood, 4);
        }

        private void Wander()
        {
            _moveTimer = _rand.Next() % (_timerRange.Y - _timerRange.X) + _timerRange.X;
            _timerStopped = true;

            MoveTo();
        }

        private void MoveTo(NavPoint loc = null)
        {
            // remove occupations
            if(_occupying.HasValue)
                _occupied[_scene].Remove(_occupying.Value);

            // setup navigation options for current position
            _currPos = _navMesh.GetClosest(_pos, _scene);
            _possibleMoves = _navMesh.GetAllPossible(_currPos);

            // assign target
            if (loc == null) // random path
                _target = _navMesh.GetRandomPath(_currPos, _possibleMoves, out _currPath);
            else
                _target = _navMesh.GetPath(_currPos, loc, _possibleMoves, out _currPath);

            // add new occupation
            if (_target != null)
            {
                _occupying = _target._tileLoc;
                _occupied[_scene].Add(_occupying.Value);
            }

            // start move (if not path of length 0)
            if (_currPath.Count > 0)
            {
                _currTarget = _currPath[_currPos];
                _lastDist = Vector2.Distance(_collisionBox._bounds.Center + new Vector2(0, _collisionBox._bounds.Height / 2), _currTarget._location);
                _isMoving = true;
            }
            else
            {
                _isMoving = false;
            }
        }

        public void Destroy(PhysicsHandler physicsHandler)
        {
            RemoveCollision(physicsHandler);
            if(_occupying.HasValue)
                _occupied[_scene].Remove(_occupying.Value);
        }

        protected void LeaveWanderState()
        {

        }

        protected void LeaveAttackState()
        {

        }

        protected void LeaveConverseState()
        {

        }

        protected void LeaveStopState()
        {

        }

        protected void StartWanderState()
        {

        }

        protected void StartAttackState()
        {

        }

        protected void StartConverseState()
        {
            _inConversation = false;
            NavPoint targetPoint = _navMesh.GetClosest(_interestTarget, _scene);
            MoveTo(targetPoint);
        }

        protected void StartStopState()
        {

        }
    }
}
