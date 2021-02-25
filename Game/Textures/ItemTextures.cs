using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace WillowWoodRefuge
{
    public static class ItemTextures
    {
        public static List<string> _allItems { get; private set; }
        private static Dictionary<string, Texture2D> _itemTextures;
        private static ContentManager _content;

        public static void Initialize(ContentManager content)
        {
            _content = content;
            _itemTextures = new Dictionary<string, Texture2D>();
            _allItems = new List<string>{
                    "acorn",
                    "apple",
                    "apple_mushroom_soup",
                    "beetle",
                    "berries",
                    "carrot",
                    "carrot_spice_soup",
                    "cricket",
                    "egg",
                    "fish",
                    "glowing_plant",
                    "gooseberry",
                    "grilled_fish",
                    "honey",
                    "meat",
                    "mousemelon",
                    "mushroom",
                    "mushroom_medicine",
                    "rabbit_spice_soup",
                    "snail",
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
                Texture2D newTexture = _content.Load<Texture2D>("ingredient/" + itemName);
                if (newTexture != null)
                {
                    _itemTextures.Add(itemName, newTexture);
                }
                return newTexture;
            }
        }
    }
}
