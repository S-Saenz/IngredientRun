
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace IngredientRun
{
    public class AnimatedObject
    {
        public List<Animation> animationList;
        public string name;
        public Vector2 _pos;
        protected float _scale = 1.5f;
        protected int currentAnimation;
        public AnimatedObject(List<Animation> animationList_, string name_, Vector2 pos_)
        {
            animationList = animationList_;
            name = name_;
            _pos = pos_;
            currentAnimation = 0;
        }

        /*public Animation getAnimation(int idx)
        {
            //code here
        }*/
        public void Draw(SpriteBatch spriteBatch)
        {
            //spriteBatch.Draw(idle, _pos, null, Color.White, 0f, new Vector2(idle.Bounds.Center.X, idle.Bounds.Center.Y), _scale, SpriteEffects.None, 0.5f);
            animationList[currentAnimation].Draw(spriteBatch, _pos, _scale);
        }

        public void Update(GameTime gameTime)
        {
            animationList[currentAnimation].Update(gameTime);
        }
    }


}