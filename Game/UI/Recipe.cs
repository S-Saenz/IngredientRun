using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System.Collections.Generic;

namespace WillowWoodRefuge
{
    // recipe class
    class Recipe
    {
        public string _name { get; private set; }
        public List<string> _ingredients { get; private set; }
        public bool _canCook { get; set; }
        public Point _gridCoord { get; private set; }
        public RectangleF _area { get; private set; }

        public Recipe(string name, List<string> ingredients, bool canCook, Point gridCoord, RectangleF area)
        {
            _name = name;
            _ingredients = ingredients;
            _canCook = canCook;
            _gridCoord = gridCoord;
            _area = area;
        }
    }
}
