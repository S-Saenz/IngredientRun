
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace WillowWoodRefuge
{
    public class AnimatedObject
    {
        public Dictionary<string, Animation> animationDict;
        public string name;
        public Vector2 _pos;
        protected float _scale = 1f;
        protected string currentAnimation;
        public AnimatedObject(Dictionary<string, Animation> animationDict_, string name_, Vector2 pos_)
        {
            animationDict = animationDict_;
            name = name_;
            _pos = pos_;
            currentAnimation = "idle";
        }

        /*public Animation getAnimation(int idx)
        {
            //code here
        }*/
        public void Draw(SpriteBatch spriteBatch)
        {
            //spriteBatch.Draw(idle, _pos, null, Color.White, 0f, new Vector2(idle.Bounds.Center.X, idle.Bounds.Center.Y), _scale, SpriteEffects.None, 0.5f);
            animationDict[currentAnimation].Draw(spriteBatch, _pos, _scale);
        }

        public void Update(GameTime gameTime)
        {
            if (animationDict.ContainsKey(currentAnimation))
            {
                animationDict[currentAnimation].Update(gameTime);
            }
            else
            {
                currentAnimation = "idle";
            }
        }
    }


}