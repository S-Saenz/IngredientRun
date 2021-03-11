using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace WillowWoodRefuge
{
    public class Area : IPhysicsObject
    {
        public string _name { private set; get; }
        public RectangleF _bounds { private set; get; }
        private CollisionBox _collisionBox;

        public Area(PhysicsHandler collisionHandler, RectangleF bounds, string name)
        {
            _name = name;
            _bounds = bounds;
            _collisionBox = new CollisionBox(bounds, collisionHandler, this);
            collisionHandler.AddObject("Areas", _collisionBox);
        }

        public void Draw(SpriteBatch spriteBatch, string name, CameraController cameraController, Color color)
        {
            // spriteBatch.Begin(transformMatrix: cameraController.GetViewMatrix(), sortMode: SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp);
            // spriteBatch.DrawRectangle(_bounds, Color.Yellow, 0.5f);
            // spriteBatch.End();
            spriteBatch.Begin(sortMode: SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp);
            spriteBatch.DrawString(FontManager._dialogueFont, name + " (press E)", 
                                   cameraController._camera.WorldToScreen(_bounds.Center) - FontManager._dialogueFont.MeasureString("To " + name + " (press E)") / 2,
                                   color);
            spriteBatch.End();
        }
    }
}
