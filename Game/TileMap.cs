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
        TiledMapTileLayer _collision;

        public TileMap(string mapPath, ContentManager content, GraphicsDevice graphics)
        {
            _map = content.Load<TiledMap>(mapPath);
            _renderer = new TiledMapRenderer(graphics, _map);

            _collision = _map.GetLayer<TiledMapTileLayer>("Walls");
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

        public Vector2 GetWaypoint(string layer, string name)
        {
            var objects = ((TiledMapObjectLayer)_map.GetLayer(layer)).Objects;
            for(int i = 0; i < objects.Length; ++i)
            {
                if(objects[i].Name == name)
                {
                    return objects[i].Position;
                }
            }
            return Vector2.Zero;
        }
    }
}
