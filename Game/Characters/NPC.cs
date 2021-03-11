using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;

namespace WillowWoodRefuge
{
    public class NPC : AICharacter
    {
        private Vector2 _dialogueLoc;

        public NPC(string name, Vector2 pos, PhysicsHandler collisionHandler,
                             RectangleF worldBounds = new RectangleF(), Dictionary<string, Animation> animationDict = null,
                             Area area = null)
                     : base(name, pos, "NPC", new Vector2(), collisionHandler, worldBounds, animationDict, area)
        {
            _walkSpeed = 20;
            _runSpeed = 120;
            _collisionBox._friction = 0.5f;
            _collisionBox._maxSpeed = new Vector2(_runSpeed, 500);

            // set dialogue position
            _dialogueLoc = new Vector2((_texture.Bounds.Width * _scale) / 2 + 2, -(_texture.Height * _scale) / 2 - 2);
        }

        public void Update(GameTime gameTime, Vector2 playerLoc)
        {
            base.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
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
    }
}