using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Text;

namespace WillowWoodRefuge
{
    public class NPC : AICharacter
    {
        private Vector2 _dialogueLoc;
        private Texture2D texture;
        private PhysicsHandler _collisionHandler;
        private Dictionary<NavPoint, NavPoint> _possibleMoves;
        private List<NavPoint> _currPath;
        private float _lastDist;
        private Area _area;

        // timer
        private float _moveTimer;
        private bool _timerStopped = false;
        private Vector2 _timerRange = new Vector2(3, 6);

        public NPC(string name, Vector2 pos, PhysicsHandler collisionHandler,
                             RectangleF worldBounds = new RectangleF(), Dictionary<string, Animation> animationDict = null,
                             Area area = null)
                     : base(name, pos, "NPC", new Vector2(), collisionHandler, worldBounds, animationDict)
        {
            _friction = 0.2f;
            _walkSpeed = 50;
            _runSpeed = 120;

            _collisionHandler = collisionHandler;

            texture = Game1.instance.Content.Load<Texture2D>("chars/" + name);

            // offset position
            _pos -= new Vector2(texture.Width * _scale / 2, texture.Height * _scale);

            _collisionHandler.RemoveObject(_collisionBox); // remove default collision box
            _collisionBox = new CollisionBox(new RectangleF(_pos, new Size2(texture.Width * _scale, texture.Height * _scale)),
                collisionHandler, this, worldBounds, maxSpeed: new Vector2(_runSpeed, 500), friction: _friction);
            _collisionHandler.AddObject("NPC", _collisionBox);

            // setup _pos for texture
            _pos = _collisionBox._bounds.Center;

            // set dialogue position
            _dialogueLoc = new Vector2((texture.Bounds.Width * _scale) / 2 + 2, -(texture.Height * _scale) / 2 - 2);

            // add navigation mesh
            _navMesh = new NavMesh(Game1.instance.GetCurrentTilemap().GenerateNavPointMap(_collisionBox._bounds));

            _possibleMoves = _navMesh.GetAllPossible(_pos);

            _moveTimer = new Random().Next() % (_timerRange.Y - _timerRange.X) + _timerRange.X;
        }

        public void Update(GameTime gameTime, Vector2 playerLoc)
        {
            if(!_timerStopped)
            {
                _moveTimer -= gameTime.GetElapsedSeconds();
            }

            if(_moveTimer <= 0)
            {
                
            }

            _collisionBox.TryMoveHorizontal(0);
            base.Update(gameTime, 0, false);
        }

        public void Draw(SpriteBatch spriteBatch, bool isDebug = false)
        {
            base.Draw(spriteBatch);

            if (isDebug)
            {
                //_navMesh.Draw(spriteBatch, isDebug);
                _navMesh.DrawPaths(spriteBatch, _possibleMoves);
            }
        }

        public Vector2 GetDialogueLoc(OrthographicCamera camera)
        {
            var pos = camera.WorldToScreen(_pos + _dialogueLoc); // _pos + _dialogueLoc
            return pos;
        }

        public void Load(ContentManager Content)
        {
            animationDict = new Dictionary<string, Animation>();
            animationDict.Add("idle", new Animation(texture, 1, 1, 100));
            animationDict.Add("walkLeft", new Animation(texture, 1, 1, 100));
            animationDict.Add("walkRight", new Animation(texture, 1, 1, 100));
            animationDict.Add("runLeft", new Animation(texture, 1, 1, 100));
            animationDict.Add("runRight", new Animation(texture, 1, 1, 100));
        }

        private void Move()
        {
            _moveTimer = new Random().Next() % (_timerRange.Y - _timerRange.X) + _timerRange.X;
            _timerStopped = true;
            // Vector2 newPos = new Vector2(new Random().Next % ())
            // _currPath = _navMesh.GetPathTo(_pos, _possibleMoves.)
        }
    }
}