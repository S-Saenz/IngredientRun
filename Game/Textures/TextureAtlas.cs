using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace WillowWoodRefuge
{
    class TextureAtlas
    {
        Dictionary<string, Rectangle> _textureList = new Dictionary<string, Rectangle>();
        Texture2D _textureAtlas;

        public TextureAtlas(string filename, ContentManager content)
        {
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("WillowWoodRefuge.Content.textureAtlas." + filename + ".json");

            using (StreamReader reader = new StreamReader(stream))
            {
                string json = reader.ReadToEnd();
                Root obj = JsonConvert.DeserializeObject<Root>(json);

                foreach (Frame element in obj.frames)
                {
                    string name = element.filename.Substring(0, element.filename.Length - 4);
                    SourceRectangle sourceRectangle = element.frame;
                    _textureList.Add(name, new Rectangle(sourceRectangle.x, sourceRectangle.y, sourceRectangle.w, sourceRectangle.h));
                }
                _textureAtlas = content.Load<Texture2D>("textureAtlas/" + filename);
            }
        }

        public void DrawTexture(SpriteBatch spriteBatch, string textureName, Vector2 loc, Color color, float scale = 1, bool centered = false)
        {
            if(_textureList.ContainsKey(textureName))
            {
                Rectangle sourceRect = _textureList[textureName];
                // (Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, SpriteEffects effects, float layerDepth);
                if(!centered)
                    spriteBatch.Draw(_textureAtlas, (Rectangle)new RectangleF(loc.X, loc.Y, sourceRect.Width * scale, sourceRect.Height * scale), sourceRect, color);
                else
                    spriteBatch.Draw(_textureAtlas, (Rectangle)new RectangleF(loc.X - sourceRect.Width * scale / 2, loc.Y - sourceRect.Height * scale / 2, sourceRect.Width * scale, sourceRect.Height * scale), sourceRect, color);
            }
        }

        public void DrawTexture(SpriteBatch spriteBatch, string textureName, Rectangle destinationRectangle, Color color)
        {
            if (_textureList.ContainsKey(textureName))
            {
                // (Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, SpriteEffects effects, float layerDepth);
                spriteBatch.Draw(_textureAtlas, destinationRectangle, _textureList[textureName], color);
            }
        }

        private class SourceRectangle
        {
            public int x { get; set; }
            public int y { get; set; }
            public int w { get; set; }
            public int h { get; set; }
        }

        private class SourceSize
        {
            public int w { get; set; }
            public int h { get; set; }
        }

        private class Frame
        {
            public string filename { get; set; }
            public SourceRectangle frame { get; set; }
            public bool rotated { get; set; }
            public bool trimmed { get; set; }
            public SourceRectangle spriteSourceSize { get; set; }
            public SourceSize sourceSize { get; set; }
        }

        private class Size
        {
            public int w { get; set; }
            public int h { get; set; }
        }

        private class Meta
        {
            public string app { get; set; }
            public string version { get; set; }
            public string image { get; set; }
            public string format { get; set; }
            public Size size { get; set; }
            public string scale { get; set; }
            public string smartupdate { get; set; }
        }

        private class Root
        {
            public List<Frame> frames { get; set; }
            public Meta meta { get; set; }
        }
    }
}
