using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace IngredientRun.Game
{
    public class AnimatedObject
    {
        public List<Animation> animationList;
        public string name;
        public double x;
        public double y;
        public AnimatedObject(List<Animation> animationList_, string name_, double x_, double y_)
        {
            animationList = animationList_;
            name = name_;
            x = x_;
            y = y_;
        }

        public Animation getAnimation(int idx)
        {
            //code here
        }
    }

    
}
