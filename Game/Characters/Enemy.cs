using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Tiled;

namespace WillowWoodRefuge
{
    public class Enemy : AICharacter, ISpawnable
    {
        float _sightDistance = 100;
        public Enemy(string type, Vector2 pos, PhysicsHandler collisionHandler,
                     RectangleF worldBounds = new RectangleF(), Dictionary<string, Animation> animationDict = null)
                     : base(type, pos, "Enemy", new Vector2(), collisionHandler, worldBounds, animationDict)
        {
            _walkSpeed = 45;
            _runSpeed = 100;
            _collisionBox._friction = 0.5f;
            _collisionBox._maxSpeed = new Vector2(_runSpeed, 500);

            _collisionBox.AddOverlapListener(onOverlap);

            _timerRange = new Vector2(5, 10);
        }

        public void Update(GameTime gameTime, Vector2 playerLoc)
        {
            if(Vector2.Distance(playerLoc, _pos) <= _sightDistance)
            {
                _currState = AIState.Attack;
            }
            else
            {
                _currState = AIState.Wander;
            }
            _target = playerLoc;
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

        public void Load(ContentManager Content)
        {
            animationDict = new Dictionary<string, Animation>();
            animationDict.Add("idle", new Animation(_texture, 1, 1, 100));
            animationDict.Add("walkLeft", new Animation(_texture, 1, 1, 100));
            animationDict.Add("walkRight", new Animation(_texture, 1, 1, 100));
            animationDict.Add("runLeft", new Animation(_texture, 1, 1, 100));
            animationDict.Add("runRight", new Animation(_texture, 1, 1, 100));
        }

        private void onOverlap(OverlapInfo info)
        {
            Player player = info._other as Player;
            if(player != null)
            {
                player.Reset();
            }
        }
    }
}
