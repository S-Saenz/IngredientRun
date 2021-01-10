using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

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

        public void Draw(SpriteBatch spriteBatch, Matrix viewMatrix, Matrix projMatrix)
        {
            spriteBatch.Begin(sortMode: SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp);
            _renderer.Draw(viewMatrix, projMatrix);
            spriteBatch.End();
        }
    }
}
