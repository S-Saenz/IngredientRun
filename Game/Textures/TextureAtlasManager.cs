using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System.Collections.Generic;

namespace WillowWoodRefuge
{
    static class TextureAtlasManager
    {
        static Dictionary<string, TextureAtlas> _atlasList = new Dictionary<string, TextureAtlas>();

        public static void Initialize(ContentManager content)
        {
            _atlasList.Add("Item", new TextureAtlas("itemTextures", content));
            _atlasList.Add("Foraging", new TextureAtlas("foragingTextures", content));
        }

        public static void DrawTexture(SpriteBatch spriteBatch, string textureType, string textureName, Vector2 loc, Color color, float scale = 1, bool centered = false)
        {
            if (_atlasList.ContainsKey(textureType))
            {
                _atlasList[textureType].DrawTexture(spriteBatch, textureName, loc, color, scale, centered);
            }
        }

        public static void DrawTexture(SpriteBatch spriteBatch, string textureType, string textureName, Rectangle destinationRectangle, Color color)
        {
            if (_atlasList.ContainsKey(textureType))
            {
                _atlasList[textureType].DrawTexture(spriteBatch, textureName, destinationRectangle, color);
            }
        }

        public static Size2 GetSize(string textureType, string textureName)
        {
            return _atlasList[textureType].GetSize(textureName);
        }
    }
}
