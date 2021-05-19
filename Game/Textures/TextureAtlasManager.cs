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
            _atlasList.Add("UI", new TextureAtlas("uiTextures", content));
        }

        public static void DrawTexture(SpriteBatch spriteBatch, string textureType, string textureName, Vector2 loc, Color color, Vector2? scale = null, bool centered = false, float rotation = 0, Vector2 origin = new Vector2())
        {
            if (_atlasList.ContainsKey(textureType))
            {
                _atlasList[textureType].DrawTexture(spriteBatch, textureName, loc, color, scale.HasValue ? scale.Value : Vector2.One, centered, rotation, origin);
            }
        }

        public static void DrawTexture(SpriteBatch spriteBatch, string textureType, string textureName, Rectangle destinationRectangle, Color color, float rotation = 0, Vector2 origin = new Vector2())
        {
            if (_atlasList.ContainsKey(textureType))
            {
                _atlasList[textureType].DrawTexture(spriteBatch, textureName, destinationRectangle, color, rotation, origin);
            }
        }

        public static Size2 GetSize(string textureType, string textureName)
        {
            return _atlasList[textureType].GetSize(textureName);
        }
    }
}
