﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace WillowWoodRefuge
{
    static class TextureAtlasManager
    {
        static Dictionary<string, TextureAtlas> _atlasList = new Dictionary<string, TextureAtlas>();

        public static void Initialize(ContentManager content)
        {
            _atlasList.Add("Item", new TextureAtlas("itemTextures", content));
        }

        public static void DrawTexture(SpriteBatch spriteBatch, string textureType, string textureName, Vector2 loc, Color color, float scale = 1)
        {
            if (_atlasList.ContainsKey(textureType))
            {
                _atlasList[textureType].DrawTexture(spriteBatch, textureName, loc, color, scale);
            }
        }

        public static void DrawTexture(SpriteBatch spriteBatch, string textureType, string textureName, Rectangle destinationRectangle, Color color)
        {
            if (_atlasList.ContainsKey(textureType))
            {
                _atlasList[textureType].DrawTexture(spriteBatch, textureName, destinationRectangle, color);
            }
        }
    }
}
