using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace IngredientRun
{
    public static class ItemTextures
    {
        public static List<string> _allItems;
        private static Dictionary<string, Texture2D> _itemTextures;
        private static ContentManager _content;

        public static void Initialize(ContentManager content)
        {
            _content = content;
            _itemTextures = new Dictionary<string, Texture2D>();
            _allItems = new List<string>
                {
                    "acorn",
                    "apple",
                    "carrot",
                    "egg",
                    "fish",
                    "gooseberry",
                    "meat",
                    "mousemelon",
                    "waterjug",
                    "wood"
                };
        }

        public static Texture2D GetTexture(string itemName)
        {
            if(!_allItems.Contains(itemName))
            {
                return null;
            }

            if(_itemTextures.ContainsKey(itemName))
            {
                return _itemTextures[itemName];
            }
            else
            {
                Texture2D newTexture = _content.Load<Texture2D>(itemName);
                if (newTexture != null)
                {
                    _itemTextures.Add(itemName, newTexture);
                }
                return newTexture;
            }
        }
    }
}
