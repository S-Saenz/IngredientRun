using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System.Collections.Generic;

namespace WillowWoodRefuge
{
    // recipe class
    class Recipe
    {
        public string _displayName { get; private set; }
        public string _name { get; private set; }
        public string _cures { get; private set; }
        public string _description { get; private set; }
        public List<string> _ingredients { get; private set; }
        public bool _canCook { get; set; }
        public Point _gridCoord { get; private set; }
        public RectangleF _area { get; private set; }

        public Recipe(string displayName, string name, string cures, string description, List<string> ingredients, bool canCook, Point gridCoord, RectangleF area)
        {
            _displayName = displayName;
            _name = name;
            _cures = cures;
            _description = description;
            _ingredients = ingredients;
            _canCook = canCook;
            _gridCoord = gridCoord;
            _area = area;
        }
    }
}
