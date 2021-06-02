using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace WillowWoodRefuge
{
    public class Ingredient
    {
        public bool holding = false;
        public float Rotation;
        public Vector2 Origin = new Vector2(0,0);
        public Vector2 index = new Vector2(1, 0);
        public bool falling = false;
        public Vector2 pos;
        //public bool highest = false;
        public float Scale = 1f;
        public string _name { get; protected set; }

        public string orientation = "up";           //which way is our ingredient rotated - up, down, left, right
        public bool doubleSquare = false;           //does this ingredient occupy more than one square? 
        public Vector2 index2 = new Vector2(1, 0);  //index of the other square your ingredient occupies 

        //right side inventory data
        public int _stars = 1;
        public string _ingredient_Or_Dish;
        public string _use;
        public string _description = "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. sed diam nonummy nibh euismod tincidunt ut laoreet dolore magna aliquam erat volutpat. Ut wisi enim ad ";

        // public Texture2D img;

        public Ingredient(Vector2 position, string name)
        {
            pos = position;
            Scale = 4f;
            // Origin = new Vector2(img.Bounds.Center.X, img.Bounds.Center.Y);
            _name = name;
        }

        public Ingredient(Vector2 position, string name, string description)
        {
            pos = position;
            Scale = 4f;
            _name = name;
            _description = description == "" ? _description : description;
        }

        // public Ingredient(Texture2D image)
        // {
        //     img = image;
        //     Scale = .25f;
        //     Origin = new Vector2(img.Bounds.Center.X, img.Bounds.Center.Y);
        // }

        public void Update(GameTime gameTime)
        {
            //timeSinceLastDrop += (float)gameTime.ElapsedGameTime.TotalSeconds; //add elapsed time to counter

        }
        public void Draw(SpriteBatch spriteBatch)
        {
            TextureAtlasManager.DrawTexture(spriteBatch, "Item", _name, pos * Game1.instance._cameraController._screenScale, Color.White,
                                            new Vector2(Scale) * Game1.instance._cameraController._screenScale, true);
            // spriteBatch.Draw(img, pos, null, Color.White, Rotation, Origin, scale, SpriteEffects.None, 1f);

            //spriteBatch.Draw(myTexture, position, null, Color.White, rotation, origin, scale, SpriteEffects.FlipHorizontally, layer);
        }

        //public void resetCounter()
        //{
        //    this.timeSinceLastDrop = 0f;
        //}

        //change the orientation variable by one rotation (pi/2) 
        public void updateOrientation()
        {
            switch (this.orientation) {
                case "up":
                    this.orientation = "right";
                    break;
                case "right":
                    this.orientation = "down";
                    break;
                case "down":
                    this.orientation = "left";
                    break;
                case "left":
                    this.orientation = "up";
                    break;
            }
            //Debug.WriteLine(this.orientation);
        }

        //call this function to update index2 in relation to the ingredient rotation/orientation
        public void updateIndex2()
        {
            switch (this.orientation) {
                case "right":
                    //index2 is right of index
                    this.index2 = new Vector2(this.index.X + 1, this.index.Y);
                    break;
                case "down":
                    //index2 is below index
                    this.index2 = new Vector2(this.index.X, this.index.Y - 1);
                    break;
                case "left":
                    //index 2 is left of index
                    this.index2 = new Vector2(this.index.X - 1, this.index.Y);
                    break;
                case "up":
                    //index2 is above index
                    this.index2 = new Vector2(this.index.X, this.index.Y + 1);
                    break;
            }

        }
        public Rectangle Bounds()
        {
            Rectangle rect = new Rectangle(new Point((int)(pos.X * Game1.instance._cameraController._screenScale), (int)(pos.Y * Game1.instance._cameraController._screenScale)), 
                                           (Point)(TextureAtlasManager.GetSize("Item", _name) * Game1.instance._cameraController._screenScale));
            return rect;
        }

        public void SetPosByMouse(Point p)
        {
            //pos = new Vector2(p.X-(img.Width/2*scale), p.Y+20-(img.Height/2*scale));
            pos = new Vector2(p.X, p.Y) / Game1.instance._cameraController._screenScale;
        }

        public bool IsPointOver(Point point)
        {
            return this.Bounds().Contains(point.X, point.Y);
        }
    }
}
   
