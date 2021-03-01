using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Text;

namespace WillowWoodRefuge
{
    public class BaseCharacter : AnimatedObject, IPhysicsObject
    {
        // movement 
        protected float _walkSpeed;
        protected float _runSpeed;
        protected float _friction;
        protected float _jumpHeight;
        protected float _jumpDistance;

        // navigation
        protected NavMesh _navMesh;

        // collision
        protected CollisionBox _collisionBox;

        // parameters
        protected int _health;

        // animation
        protected string _currentDirection = "";
        protected string _currentMoveType = "idle";

        public BaseCharacter(string name, Vector2 pos, string collisionLabel, Vector2 bounds, PhysicsHandler collisionHandler,
                             RectangleF worldBounds = new RectangleF(), Dictionary<string, Animation> animationDict = null) 
                             : base(animationDict, name, pos)
        {
            // Add collision box
            _collisionBox = new CollisionBox(new RectangleF(_pos,
                new Size2(bounds.X, bounds.Y)), collisionHandler, this, worldBounds, maxSpeed: new Vector2(_runSpeed, 500),
                friction: _friction);
            _collisionBox.AddMovementStartListener(onStartMove);
            _collisionBox.AddMovementChangeDirectionListener(onChangeDirection);
            collisionHandler.AddObject(collisionLabel, _collisionBox);
        }

        // direction: -1 left, 0 not moving, 1 right
        public void Update(GameTime gameTime, int direction, bool isWalking)
        {
            // apply movement velocity
            if (isWalking)
            {
                _collisionBox.TryMoveHorizontal(_walkSpeed * direction);
            }
            else
            {
                _collisionBox.TryMoveHorizontal(_runSpeed * direction);
            }

            // update collision box and sprite position (center on collision box)
            _pos = _collisionBox.Update(gameTime) + new Vector2(_collisionBox._bounds.Width / 2, _collisionBox._bounds.Height / 2);

            // update animation type
            UpdateAnimationInfo();

            // update animation object base class
            base.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch, bool isDebug = false)
        {
            // draw animation frame of sprite
            base.Draw(spriteBatch);

            // debug box display
            if (isDebug)
            {
                _collisionBox.Draw(spriteBatch);
            }
        }


        private void UpdateAnimationInfo()
        {
            if (_collisionBox._velocity.X == 0) // stopped
            {
                _currentMoveType = "idle";
            }
            else if (Math.Abs(_collisionBox._velocity.X) > _walkSpeed + 1) // if running
            {
                _currentMoveType = "run";
            }
            else if (Math.Abs(_collisionBox._velocity.X) < _walkSpeed + 1) // if walking
            {
                _currentMoveType = "walk";
            }
            currentAnimation = _currentMoveType + _currentDirection;
        }

        public bool RemoveCollision(PhysicsHandler collisionHandler)
        {
            return collisionHandler.RemoveObject(_collisionBox);
        }

        public void onStartMove(Vector2 move)
        {
            // Debug.WriteLine("Start");

            if (move.X > 0) // moving right
            {
                _currentDirection = "Right";
            }
            else if (move.X < 0) // moving left
            {
                _currentDirection = "Left";
            }
            else if (move.X == 0) // horizontal movement stopped
            {
                _currentDirection = "";
            }
        }

        public void onChangeDirection(Vector2 move)
        {
            if (move.X > 0) // moving right
            {
                _currentDirection = "Right";
            }
            else if (move.X < 0) // moving left
            {
                _currentDirection = "Left";
            }
            else if (move.X == 0) // horizontal movement stopped
            {
                _currentDirection = "";
            }
        }
    }
}