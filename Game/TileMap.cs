using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using MonoGame.Extended;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;

namespace IngredientRun
{
    public class TileMap
    {
        TiledMap _map;
        TiledMapRenderer _renderer;

        public TileMap(string mapPath, ContentManager content, GraphicsDevice graphics)
        {
            _map = content.Load<TiledMap>(mapPath);
            _renderer = new TiledMapRenderer(graphics, _map);
        }

        public void Update(GameTime gameTime)
        {
            _renderer.Update(gameTime);
        }

        public void Draw(Matrix viewMatrix, Matrix projMatrix)
        {
            _renderer.Draw(viewMatrix, projMatrix);
        }
    }
}
