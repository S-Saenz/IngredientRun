using MonoGame.Extended;

namespace WillowWoodRefuge
{
    class Area : IPhysicsObject
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
    }
}
