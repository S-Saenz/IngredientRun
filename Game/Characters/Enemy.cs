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
        float _attackCooldown = 1;
        float _cooldownTimer = 0;
        float _attackDamage = 1;
        bool _isAttacking = false;
        float _attackDuration = 0.5f;
        public Enemy(string type, Vector2 pos, PhysicsHandler collisionHandler, string scene, TileMap tileMap,
                     RectangleF worldBounds = new RectangleF(), Dictionary<string, Animation> animationDict = null)
                     : base(type, pos, "Enemy", new Vector2(), collisionHandler, scene, tileMap, worldBounds, animationDict)
        {
            _walkSpeed = 45;
            _runSpeed = 100;
            _jumpHeight = 7500;
            _collisionBox._friction = 0.5f;
            _collisionBox._maxSpeed = new Vector2(_runSpeed, 500);

            _collisionBox.AddOverlapListener(onOverlap);

            _timerRange = new Vector2(5, 10);
        }

        public void Update(GameTime gameTime, Vector2 playerLoc)
        {
            if(Vector2.Distance(playerLoc, _pos) <= _sightDistance && _currState != AIState.Stop)
            {
                _currState = AIState.Attack;
                if (name.Equals("spider"))
                {
                    Game1.instance.sounds.spiderAttack(gameTime);
                }
            }

            //ambient spider noise
            if (Vector2.Distance(playerLoc, _pos) <= 300 && name.Equals("spider") && _currState == AIState.Wander)
            {
                Game1.instance.sounds.spiderAmbient(gameTime, Vector2.Distance(playerLoc, _pos));
            }

            _cooldownTimer += gameTime.GetElapsedSeconds();

            _interestTarget = playerLoc;
            base.Update(gameTime);

            // last seen point reached and player not visible
            if (_currState == AIState.Attack && Vector2.Distance(playerLoc, _pos) > _sightDistance && _currPos == _target) 
            {
                _currState = AIState.Wander;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch, _isAttacking ? Color.Lerp(Color.Red, Color.Yellow, (_stopCooldown / _attackDuration)) : Color.White);
        }

        public void DrawDebug(SpriteBatch spriteBatch)
        {
            base.DrawDebug(spriteBatch);
        }

        public void Load(ContentManager Content)
        {
            Vector2 offset = new Vector2(0, 5);
            animationDict = new Dictionary<string, Animation>();
            animationDict.Add("idle", new Animation(Content.Load<Texture2D>("animations/" + name + "_side_left"), 1, 1, 0, offset));
            animationDict.Add("idleLeft", new Animation(Content.Load<Texture2D>("animations/" + name + "_side_left"), 1, 1, 0, offset));
            animationDict.Add("idleRight", new Animation(Content.Load<Texture2D>("animations/" + name + "_side_right"), 1, 1, 0, offset));
            animationDict.Add("walkLeft", new Animation(Content.Load<Texture2D>("animations/" + name + "_walk_left"), 1, 8, 100, offset));
            animationDict.Add("walkRight", new Animation(Content.Load<Texture2D>("animations/" + name + "_walk_right"), 1, 8, 100, offset));
        }

        private void onOverlap(OverlapInfo info)
        {
            Player player = info._other as Player;
            if(player != null)
            {
                if (_cooldownTimer >= _attackCooldown && _currState == AIState.Attack)
                {
                    // player.Reset();

                    _isAttacking = true;
                    _stopCooldown = _attackDuration; // set stop movement to attack
                    _stopTimerEnabled = true;
                    ChangeState(AIState.Stop);
                }
            }
        }

        override protected void LeaveStopState()
        {
            base.LeaveStopState();
            if(_isAttacking) // returned from stop state after attack cooldown
            {
                foreach (OverlapInfo info in _collisionBox.IsOverlapping())
                {
                    Player player = info._other as Player;
                    if(player != null) // overlapping player
                    {
                        Game1.instance.sounds.hitSound();
                        player.Hit(_attackDamage);
                    }
                }
                _isAttacking = false;
                _cooldownTimer = 0;
            }
        }
    }
}
