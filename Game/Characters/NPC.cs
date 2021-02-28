using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Text;

namespace WillowWoodRefuge
{
    class NPC : BaseCharacter
    {
        private Vector2 _dialogueLoc;
        private Texture2D texture;
        private PhysicsHandler _collisionHandler;

        public NPC(string name, Vector2 pos, PhysicsHandler collisionHandler,
                             RectangleF worldBounds = new RectangleF(), Dictionary<string, Animation> animationDict = null)
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
        }

        public void Update(GameTime gameTime, Vector2 playerLoc)
        {
            _collisionBox.TryMoveHorizontal(0);
            base.Update(gameTime, 0, false);
        }

        // public void Draw(SpriteBatch spriteBatch)
        // {
        //     RectangleF dest = new RectangleF(_pos.X, _pos.Y - _texture.Height * _scale, _texture.Width * _scale, _texture.Height * _scale);
        //     spriteBatch.Draw(_texture, (Rectangle)dest, null, Color.White);
        // }

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
    }
}