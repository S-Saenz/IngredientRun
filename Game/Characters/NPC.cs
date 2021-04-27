using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace WillowWoodRefuge
{
    public class NPC : AICharacter
    {
        private Vector2 _dialogueLoc;
        public string _cureItem { get; private set; } // name of item needed to cure
        public bool _isCured { get; private set; }
        private float _displayTime = 3;
        private float _currTime = -1;
        private bool _inConversation = false;

        // Event delegate
        public delegate void NPCEventHandler();

        // Interaction events
        private event NPCEventHandler _reachedConversation;

        public NPC(string name, Vector2 pos, PhysicsHandler collisionHandler, string scene,
                             RectangleF worldBounds = new RectangleF(), Dictionary<string, Animation> animationDict = null,
                             Area area = null)
                     : base(name, pos, "NPC", new Vector2(), collisionHandler, scene, worldBounds, animationDict, area)
        {
            _walkSpeed = 20;
            _runSpeed = 120;
            _collisionBox._friction = 0.5f;
            _collisionBox._maxSpeed = new Vector2(_runSpeed, 500);
            _isCured = true;

            // set dialogue position
            _dialogueLoc = new Vector2((_texture.Bounds.Width * _scale) / 2 + 2, -(_texture.Height * _scale) / 2 - 2);
        }

        public void Update(GameTime gameTime)
        {
            if (_currTime >= 0 && _currTime < _displayTime)
                _currTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_currState == AIState.Converse && !_isMoving && !_inConversation)
            {
                _reachedConversation?.Invoke();
                _inConversation = true;
            }
            base.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch, _isCured ? Color.White : Color.Gray);
            if (_currTime >= 0 && _currTime < _displayTime)
            {
                string statement = _isCured ? (name + " cured!") : ("incorrect item");
                spriteBatch.DrawString(FontManager._dialogueFont, statement, _pos, Color.Black);
            }
        }

        public void DrawDebug(SpriteBatch spriteBatch)
        {
            base.DrawDebug(spriteBatch);
        }

        public Vector2 GetDialogueLoc(OrthographicCamera camera)
        {
            var pos = camera.WorldToScreen(_pos + _dialogueLoc); // _pos + _dialogueLoc
            return pos;
        }

        public void Load(ContentManager Content)
        {
            animationDict = new Dictionary<string, Animation>();
            animationDict.Add("idle", new Animation(_texture, 1, 1, 100));
            animationDict.Add("walkLeft", new Animation(_texture, 1, 1, 100));
            animationDict.Add("walkRight", new Animation(_texture, 1, 1, 100));
            animationDict.Add("runLeft", new Animation(_texture, 1, 1, 100));
            animationDict.Add("runRight", new Animation(_texture, 1, 1, 100));
        }

        // Adds an "injury" to npc, along with assigning what item is needed to remove the injury.
        // Returns whether character was already injured. Cure item is only assigned if character was not already injured.
        public bool Injure(string cureItem)
        {
            if (!_isCured)
                return true;

            _isCured = false;
            _cureItem = cureItem;
            return false;
        }

        // Attempts to cure characte with given item, returning whether cure was successful or not
        public bool Cure(string item)
        {
            _currTime = 0;
            if (_isCured || item != _cureItem)
                return false;

            _isCured = true;
            Debug.WriteLine(name + " cured with " + item);
            return true;
        }

        public Vector2 GoConverse(Vector2? loc)
        {
            if (loc.HasValue)
                _interestTarget = loc.Value;
            else
                _interestTarget = _navMesh.GetRandomPoint(_currPos, _possibleMoves)._location;

            ChangeState(AIState.Converse);
            _inConversation = false;
            return _interestTarget;
        }

        public void StopConverse()
        {
            ChangeState(AIState.Wander);
        }

        public void AddConversationReachedListener(NPCEventHandler handler)
        {
            _reachedConversation += handler;
        }
    }
}