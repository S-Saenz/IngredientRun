using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace IngredientRun
{
    public static class EnemyTextures
    {
        public static List<string> _allItems { get; private set; }
        private static Dictionary<string, Texture2D> _monsterTextures;
        private static ContentManager _content;

        public static void Initialize(ContentManager content)
        {
            _content = content;
            _monsterTextures = new Dictionary<string, Texture2D>();
            _allItems = new List<string>{
                    "lurker",
                    "wolf",
                    "spider"
                };
        }

        public static Texture2D GetTexture(string itemName)
        {
            if(!_allItems.Contains(itemName))
            {
                return null;
            }

            if(_monsterTextures.ContainsKey(itemName))
            {
                return _monsterTextures[itemName];
            }
            else
            {
                Texture2D newTexture = _content.Load<Texture2D>("monsters/" + itemName);
                if (newTexture != null)
                {
                    _monsterTextures.Add(itemName, newTexture);
                }
                return newTexture;
            }
        }
    }
}
