﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace WillowWoodRefuge
{
    class Enemy : BaseCharacter, ISpawnable
    {
        private Texture2D texture;
        private PhysicsHandler _collisionHandler;

        public Enemy(string type, Vector2 pos, PhysicsHandler collisionHandler,
                             RectangleF worldBounds = new RectangleF(), Dictionary<string, Animation> animationDict = null)
                     : base(type, pos, "Enemy", new Vector2(), collisionHandler, worldBounds, animationDict)
        {
            _friction = 0.2f;
            _walkSpeed = 50;
            _runSpeed = 120;

            _collisionHandler = collisionHandler;

            texture = EnemyTextures.GetTexture(type);

            // offset position
            _pos -= new Vector2(texture.Width * _scale / 2, texture.Height * _scale);

            _collisionHandler.RemoveObject(_collisionBox); // remove default collision box
            _collisionBox = new CollisionBox(new RectangleF(_pos, new Size2(texture.Width * _scale, texture.Height * _scale)),
                collisionHandler, this, worldBounds, maxSpeed: new Vector2(_runSpeed, 500), friction: _friction);
            _collisionHandler.AddObject("Enemy", _collisionBox);
            _collisionBox.AddOverlapListener(onOverlap);

            // setup _pos for texture
            _pos = _collisionBox._bounds.Center;
        }

        public void Update(GameTime gameTime, Vector2 playerLoc)
        {
            _collisionBox.TryMoveHorizontal(0);
            base.Update(gameTime, 0, false);
        }

        public void Draw(SpriteBatch spriteBatch, bool isDebug = false)
        {
            base.Draw(spriteBatch);
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
