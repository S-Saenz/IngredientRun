using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Text;

namespace WillowWoodRefuge
{
    public class AICharacter : BaseCharacter
    {
        // navigation
        protected NavMesh _navMesh;

        public AICharacter(string name, Vector2 pos, string collisionLabel, Vector2 bounds, PhysicsHandler collisionHandler, 
                           RectangleF worldBounds = default, Dictionary<string, Animation> animationDict = null) 
                           : base(name, pos, collisionLabel, bounds, collisionHandler, worldBounds, animationDict)
        {

        }
    }
}
