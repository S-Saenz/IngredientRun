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
        // protected Dictionary<NavPoint, NavPoint> _currPath;
        protected float _lastDist;
        protected NavPoint _currPos;
        protected NavPoint _lastPos;
        protected NavPoint _target;
        protected Vector2 _interestTarget;
        protected float _proxRange = 5;
        protected string _scene;
        protected bool _inConversation = false;
        protected Point? _occupying = null;
        protected bool _pointReached = true;

        // path abandon variables (how long before entity gives up after making no progress)
        protected float _abandonTimer = 0;
        protected float _abandonTime = 2;

        // sense variables
        protected float _sightDistance = 100;
        protected float _soundDistance = 150;

        // this is dumb but everyone needs to know what points are occupied
        protected static Dictionary<string, List<Point>> _occupied = new Dictionary<string, List<Point>>();
        public static Dictionary<string, List<Point>> _occupiedPoints { get { return _occupied; } }

        // Interaction events
        private event AIEventHandler _reachedConversation;

        // Event delegate
        public delegate void AIEventHandler();

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
            Update(gameTime, new Vector2(_pos.X < _target._location.X ? 1 : -1, 0), true);
            float newDist = Vector2.Distance(_pos + new Vector2(0, _collisionBox._bounds.Height / 2), _target._location);

            if (_currPos == _lastPos) // not making progress
            {
                _abandonTimer += gameTime.GetElapsedSeconds();
            }
            else // moving toward target
            {
                _abandonTimer = 0;
            }

            if(_abandonTimer >= _abandonTime) // spent max time trying to make progress
            {
                _abandonTimer = 0;
                _timerStopped = false;
                _isMoving = false;
                Debug.WriteLine(name + " gave up.");

                if(_occupying.HasValue)
                    _occupied[_scene].Remove(_occupying.Value);
                _currPos = _navMesh.GetClosest(_pos + new Vector2(0, _collisionBox._bounds.Height / 2), _scene);
                _possibleMoves = _navMesh.GetAllPossible(_currPos);
                _occupying = _currPos._tileLoc;
                _occupied[_scene].Add(_occupying.Value);
                _pointReached = true;
                return;
            }
            else if (newDist < _proximityCut) // reached point
            {
                // stop moving, restart timer
                _timerStopped = false;
                _isMoving = false;
                _pointReached = true;
            }
            else
            {
                _lastDist = newDist;
                _pointReached = false;
            }

            _currPos = _navMesh.GetClosest(_pos + new Vector2(0, _collisionBox._bounds.Height / 2), _scene, true);

            if(_isMoving && _collisionBox._velocity.X == 0)
            {
                Jump(gameTime);
            }

            _lastPos = _currPos;
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
                Debug.WriteLine(name + ((_target != null) ? (" started wandering to " + _target._location) : " sat down"));
            }
        }

        private void ConverseUpdate(GameTime gameTime)
        {
            if(!_inConversation && _pointReached)
            {
                _reachedConversation?.Invoke();
                _inConversation = true;
            }
        }

        private void StopUpdate(GameTime gameTime)
        {

        }

        private void AttackUpdate(GameTime gameTime)
        {
            NavPoint attackTarget = _navMesh.GetClosest(_interestTarget, _scene, true);
            if (_target != attackTarget && Vector2.Distance(_interestTarget, _pos) <= _sightDistance) // target is visible and has moved
            {
                MoveTo(attackTarget);
            }
            else if (!_isMoving)
                ChangeState(AIState.Wander);
        }

        public void DrawDebug(SpriteBatch spriteBatch)
        {
            if (_isMoving && _target != null)
            {
                spriteBatch.DrawLine(_target._location - new Vector2(0, _collisionBox._bounds.Height / 2), _pos, Color.Crimson, 1);
                // spriteBatch.DrawLine(_currTarget._location - new Vector2(0, _collisionBox._bounds.Height / 2), _pos, Color.DarkGoldenrod, 1);
            }

            // _navMesh.DrawDebug(spriteBatch);
            // _navMesh.DrawPaths(spriteBatch, _possibleMoves);
            // spriteBatch.DrawPoint(_currPos._location, Color.BurlyWood, 4);

            spriteBatch.DrawPoint(_pos + new Vector2(0, _collisionBox._bounds.Height / 2), Color.GreenYellow);
            if(_target != null)
                spriteBatch.DrawPoint(_target._location, Color.Maroon);
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
                _target = _navMesh.GetRandomPoint(_currPos, _possibleMoves);
            else
                _target = loc;

            // add new occupation
            if (_target != null)
            {
                _occupying = _target._tileLoc;
                _occupied[_scene].Add(_occupying.Value);
            }
            else // no new occupation, reassign old 
            {
                _occupied[_scene].Add(_occupying.Value);
            }

            // start move (if not path of length 0)
            if (_target != null)//_currPath.Count > 0)
            {
                _lastDist = Vector2.Distance(_collisionBox._bounds.Center + new Vector2(0, _collisionBox._bounds.Height / 2), _target._location);
                _isMoving = true;
                _pointReached = false;
            }
            else
            {
                _isMoving = false;
                _pointReached = true;
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
            _target = null;
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
        public void AddConversationReachedListener(AIEventHandler handler)
        {
            _reachedConversation += handler;
        }
    }
}
