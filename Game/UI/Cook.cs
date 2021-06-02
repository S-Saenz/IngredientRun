using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace WillowWoodRefuge
{
    public class Cook
    {
        KeyboardState oldKeyState;

        public string foodName;
        public bool _cookingVisible = true;
        public bool _finished = false;

        static float _screenWidth = 1728;
        float _screenHeight = 972;
        float _scale = 2.7f;

        float _needleX;
        float _needleSpeed = 4f;
        float _needleStart = _screenWidth / 2;
        bool _attemptRemaining = true; //you can't spam the space button to move the needle

        float _meterEnd = 1458;
        float _meterStart = 272;

        float _zoneX = 500;
        float _zoneVelocity = 2.5f;

        float _progress = 0; //progress bar - range from 0 - 100

        //var rand = new Random();
        float target; // guide for hotzone bounces

        //for the timer to turn the UI off
        int counter = 1;
        int limit = 3;
        float countDuration = 1f; //every  1s.
        float currentTime = 0f;

        //for the fire version of the counter
        int fireCounter = 1;
        int fireCounterLimit = 3;
        float fireCountDuration = 0.2f;
        float fireCurrentTime = 0f;



        int fireFrame; //which frame to display for the fire animation
        string fireTexture = "campfire-2";

        bool _debugMode = false;

        public bool loaded = false;



        public Cook(string selectedFood)
        {
            foodName = selectedFood;
            _cookingVisible = true;
        }

        public Cook()
        {

        }

        public void Load(ContentManager Content)
        {

            _needleX = _needleStart; //set needle to starting position
            target = (_meterEnd - _meterStart) * 2 / 3;
            _attemptRemaining = true;
            _finished = false;
        }

        public void Update(MouseState mouseState, KeyboardState keyState, GameTime gameTime)
        {
            Game1.instance.sounds.cookingSound(gameTime, _progress > 100 & _attemptRemaining);

            if (_progress > 100 & _attemptRemaining)
            {
                //_progress = 0;
                CookingFinished(gameTime);
                _attemptRemaining = false;
            }


            //space bar held down - move needle
            if (Game1.instance.input.IsDown("cook") && _needleX <= _meterEnd && _attemptRemaining)
                _needleX += _needleSpeed * gameTime.GetElapsedSeconds() * 100;
            //needle moves left by itself, if space bar not pressed
            else if (!Game1.instance.input.IsDown("cook") && _needleX >= _meterStart && _attemptRemaining)
                _needleX -= _needleSpeed * gameTime.GetElapsedSeconds() * 100;

            //press ENTER while needle is in the hottest zone for a lil jump
            if (Game1.instance.input.JustPressed("superCook") && _needleX <= (_zoneX + 25) && _needleX >= (_zoneX - 25))
            {
                //derek - little jump in cooking progress, maybe cooking gets extra sizzly or something to convey like a surge in progress
                _progress += 15;
            }
            //don't let needle move past the end of the meter
            if (_needleX > _meterEnd)
            {
                //derek - needle collides with edge of meter, maybe like a bonk sound?
                _needleX = _meterEnd;
            }

            //don't let it pass behind the beginning either
            else if (_needleX < _meterStart)
            {
                //derek - needle collides with edge of meter, maybe like a bonk sound?
                _needleX = _meterStart;
            }


            //how size is determined in the draw function
            Size2 zoneSize = TextureAtlasManager.GetSize("UI", "Hot_Zone");
            float zoneWidth = zoneSize.Width * Game1.instance._cameraController._screenDimensions.X / 450;
            
            if (_attemptRemaining)
                _zoneX += _zoneVelocity;

            //zone reaches right edge 
            if (_zoneVelocity > 0 && (_zoneX + zoneWidth / 2 > _meterEnd))
            {
                _zoneX = _meterEnd - zoneWidth / 2;
                _zoneVelocity *= -1;
                target = _zoneX - shift();
            }
            //zone reaches right-side target
            else if (_zoneVelocity > 0 && _zoneX > target)
            {
                _zoneVelocity *= -1;
                target = _zoneX - shift();
            }
            //zone reaches left edge
            else if (_zoneVelocity < 0 && (_zoneX - zoneWidth / 2 < _meterStart))
            {
                _zoneX = _meterStart + zoneWidth / 2;
                _zoneVelocity *= -1;
                target = _zoneX + shift();
            }
            //zone reaches left side target
            else if (_zoneVelocity < 0 && _zoneX < target)
            {
                _zoneVelocity *= -1;
                target = _zoneX + shift();
            }

            if (_needleX > (_zoneX - zoneWidth / 2) && _needleX < (_zoneX + zoneWidth / 2))
            {
                //derek - cooking sound
                _progress += 0.1f;
                Console.WriteLine(_progress);
            }


            if (!_attemptRemaining)
                CookingFinished(gameTime);

            fireAnimationFrameRateModerator(gameTime);

            ////////////////////////////// debugging tools \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
            //press 1 to check needleX
            if (oldKeyState.IsKeyUp(Keys.D1) && keyState.IsKeyDown(Keys.D1) && keyState.IsKeyDown(Keys.LeftAlt))
                debug($"{_needleX}");

            //press 3 to check mouse position 
            if (oldKeyState.IsKeyUp(Keys.D3) && keyState.IsKeyDown(Keys.D3) && keyState.IsKeyDown(Keys.LeftAlt))
                debug($"{mouseState.Position.X} {mouseState.Position.Y}");

            //press 2 to restart needle
            if (oldKeyState.IsKeyUp(Keys.D2) && keyState.IsKeyDown(Keys.D2) && keyState.IsKeyDown(Keys.LeftAlt))
                ResetNeedle();

            //press 4 to en/disable visiblity 
            if (oldKeyState.IsKeyUp(Keys.D4) && keyState.IsKeyDown(Keys.D4) && keyState.IsKeyDown(Keys.LeftAlt))
            {
                debug($"before: {_cookingVisible}");
                _cookingVisible = _cookingVisible ? false : true;
                debug($"after: {_cookingVisible}");
            }

            //press HOME to toggle debug mode
            if (oldKeyState.IsKeyUp(Keys.Home) && keyState.IsKeyDown(Keys.Home))
                _debugMode = !_debugMode;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            //redesign
            Vector2 center = new Vector2(Game1.instance._cameraController._screenDimensions.X / 2, Game1.instance._cameraController._screenDimensions.Y / 2);
            float newScale = Game1.instance._cameraController._screenDimensions.X / 100;
            //TextureAtlasManager.DrawTexture(spriteBatch, "UI", "Background_Opacity", center, Color.White, newScale);
            TextureAtlasManager.DrawTexture(spriteBatch, "UI", "Background_Opacity", new Rectangle(0, 0, (int)Game1.instance._cameraController._screenDimensions.X, (int)Game1.instance._cameraController._screenDimensions.Y), Color.White);
            float width = Game1.instance._cameraController._screenDimensions.X;
            float height = Game1.instance._cameraController._screenDimensions.Y;

            //TextureAtlasManager.DrawTexture(spriteBatch, "UI", "Main_Container", new Rectangle(0, 0, width / 2, height / 5), Color.White);
            TextureAtlasManager.DrawTexture(spriteBatch, "UI", "Main_Container", new Vector2(width / 2, height / 3), Color.White, new Vector2(width / 100), true);
            TextureAtlasManager.DrawTexture(spriteBatch, "UI", "Meter_Container", new Vector2(width / 2, height * (.45f)), Color.White, new Vector2(width / 437), true);
            TextureAtlasManager.DrawTexture(spriteBatch, "UI", "Food_Container", new Vector2(width / 2, height / 8), Color.White, new Vector2(width / 200), true);


            //food being cooked
            //float foodScale = foodImage.ToString() == "Ingredient/acornScaled" ? 0.5f : .15f; //scale for an acorn or the grilled fish
            //Size2 foodSize = TextureAtlasManager.GetSize("Item", foodName);
            float foodScale = 4 * Game1.instance._cameraController._screenScale; //reuse the scale value from the recipe menu
            float foodX = _screenWidth / 2;// - foodSize.Width / 2 * foodScale;
            // spriteBatch.Draw(foodImage, new Vector2(foodX, _screenHeight / 7), null, Color.White * cookingOpacity, 0f, Vector2.Zero, foodScale, SpriteEffects.None, 1f);
            TextureAtlasManager.DrawTexture(spriteBatch, "Item", foodName, new Vector2(foodX, height / 8) * Game1.instance._cameraController._screenScale, 
                                            Color.White, new Vector2(foodScale * .9f * Game1.instance._cameraController._screenScale), true);

            //fire 
            //string fireTexture = this.chooseFireTexture();
            TextureAtlasManager.DrawTexture(spriteBatch, "UI", fireTexture, new Vector2(width/2, height*0.27f), Color.White, new Vector2(width / 1500), true);

            //progress bar
            // string progressBar = "progress" + (int)Math.Min(_progress / 100 * (17 * 4),(17 * 4));
            DrawProgressBar(spriteBatch, new Vector2(width / 2, height / 8), _progress / 100);
            // TextureAtlasManager.DrawTexture(spriteBatch, "UI", progressBar, new Vector2(width / 2, height / 8), Color.White, new Vector2(width / 630.0f), true);

            //hot zones
            Vector2 zonePos = new Vector2(_zoneX * Game1.instance._cameraController._screenScale, height * 0.45f);
            TextureAtlasManager.DrawTexture(spriteBatch, "UI", "Hot_Zone", zonePos, Color.White, new Vector2(width / 520), true);
            TextureAtlasManager.DrawTexture(spriteBatch, "UI", "Hottest_Zone", zonePos, Color.White, new Vector2(width / 520), true);

            //needle
            //spriteBatch.Draw(needle, new Vector2(_needleX, _screenHeight * 0.317f), null, Color.White, 0f, Vector2.Zero, _scale, SpriteEffects.None, 1f);
            TextureAtlasManager.DrawTexture(spriteBatch, "UI", "Spoon", new Vector2(_needleX * Game1.instance._cameraController._screenScale, height * (.45f)), Color.White, new Vector2(width / 300), true);
            // Debug.WriteLine(_needleX * Game1.instance._cameraController._screenScale + " " + new Vector2(width / 2, height * (.45f)));

            // temp tutorial text
            Vector2 textSize = FontManager._bigdialogueFont.MeasureString("Keep the spoon in the hot zone with SPACE to make food!\nPress ENTER while in the sweet spot to cook faster!");
            spriteBatch.DrawString(FontManager._bigdialogueFont, "Keep the spoon in the hot zone with SPACE to make food!\nPress ENTER while in the sweet spot to cook faster!", 
                                   new Vector2(Game1.instance._cameraController._screenDimensions.X / 2 - textSize.X / 2, Game1.instance._cameraController._screenDimensions.Y * 0.75f),
                                   Color.White);

            // spriteBatch.DrawRectangle(_meterStart * Game1.instance._cameraController._screenScale, 200, (_meterEnd - _meterStart) * Game1.instance._cameraController._screenScale, 50, Color.Red);
            // Debug.WriteLine(_meterStart * Game1.instance._cameraController._screenScale);
            // Debug.WriteLine(TextureAtlasManager.GetSize("UI", "Meter_Container") * (width / 400) + " " + (_meterEnd - _meterStart) * Game1.instance._cameraController._screenScale);
            // Debug.WriteLine(width / 400);
        }

        void debug(string message)
        {
            Debug.WriteLineIf(_debugMode, message);
        }

        void ResetNeedle()
        {
            _needleX = _needleStart;
            _attemptRemaining = true;
        }


        //inspired from - https://stackoverflow.com/questions/13394892/how-to-create-a-timer-counter-in-c-sharp-xna
        void CookingFinished(GameTime gameTime)
        {
            //derek - cooking is finished! success sound

            //for the timer to turn the UI off
            //int counter = 1;
            //int limit = 3;
            //float countDuration = 0.25f; //every  1s.
            //float currentTime = 0f;

            //debug($"before: {currentTime}");
            currentTime += (float)gameTime.ElapsedGameTime.TotalSeconds; //Time passed since last Update() 
            //debug($"after: {currentTime}");
            //debug($"{gameTime.ElapsedGameTime.TotalSeconds}");

            if (currentTime >= countDuration) //this is true at least once every sec
            {
                //update how much time has passed
                counter++;
                currentTime -= countDuration; // "use up" the time & recalibrate the currentTime                  
            }

            //debug($"counter: {counter}\ncurrentTime: {currentTime}");

            //timer finished
            if (counter >= limit)
            {
                counter = 0; //reset counter
                debug("counter hit limit!");

                UpdateInventoryAfterCooking();

                //turn off the cooking UI
                //_cookingVisible = false; 
                //ResetNeedle();
                _progress = 0;
                _finished = true;
                Game1.instance.UI.SwitchState(UIState.RecipeMenu); //have UI manager switch back to HUD
            }
        }

        public void UpdateInventoryAfterCooking()
        {
            //add cooked food to inventory
            Game1.instance.inventory.addIngredient(foodName);

            //remove used ingredients from inventory
            List<string> ingredients = Game1.instance.recipeMenu.GetIngredients(foodName);
            foreach (string ingredient in ingredients)
                Game1.instance.inventory.removeIngredient(ingredient);
        }

        //Texture names are formatted as "ingredient/[actualIngredientName]"
        string formatFoodName(string foodName)
        {
            string realName = foodName.Split("/")[1];
            Debug.WriteLine($"{foodName} -> {realName}");
            return realName;
        }

        float shift()
        {
            var rand = new Random();
            float shift = (float)Math.Round(rand.NextDouble() * 500f + 100f);
            return shift;
        }

        float roundByFive(float num)
        {
            float mod = num % 5.0f;
            float diff = mod - 2.50f;

            if (diff >= 0) //round up
                return num + 5.0f - mod;
            else //round down
                return num - mod;
        }

        // 
        string chooseFireTexture()
        {
            //fireFrames range is (1-12)
            int totalFrames = 12;

            this.fireFrame = this.fireFrame % totalFrames; //range is now (0 -11)
            this.fireFrame++; // increment!   range is now from (1 - 12)
            return "campfire-" + this.fireFrame;
        }

        void fireAnimationFrameRateModerator(GameTime gameTime)
        {
            //every time fireCurrentTime > fireCountDuration 
            //fireCurrentTime =- fireCounterLimit
            //also fireCounter ++ until fireCounter == fireCounterLimit

            fireCurrentTime += (float)gameTime.ElapsedGameTime.TotalSeconds; //Time passed since last Update() 

            if (fireCurrentTime >= fireCountDuration) //this is true at least once every sec
            // if more time has passed since last UPdate() than the countDuration
            {
                //update how much time has passed
                fireCounter++;
                fireCurrentTime -= fireCountDuration; // "use up" the time & recalibrate the currentTime      
                
                this.fireTexture = chooseFireTexture();
            }
        }

        void DrawProgressBar(SpriteBatch spriteBatch, Vector2 center, float percentage)
        {
            int frame = (int)Math.Min(((18 * 4 - 1) * percentage + 1), 18 * 4);
            float width = (int)Game1.instance._cameraController._screenDimensions.X / 200;
            // Debug.WriteLine(frame);

            for (int quadrant = 0; quadrant < 4 && frame > 0; ++quadrant)
            {
                TextureAtlasManager.DrawTexture(spriteBatch, "UI", "ProgressBar" + Math.Min(frame, 18), center, Color.LawnGreen,
                                                new Vector2(width), false, -quadrant * (MathF.PI / 2), new Vector2(1));
                frame -= 18;

                if(frame <= 0)
                {
                    TextureAtlasManager.DrawTexture(spriteBatch, "UI", "ProgressBaro" + (frame + 18), center, Color.LawnGreen,
                                                new Vector2(width), false, -quadrant * (MathF.PI / 2), new Vector2(1));
                }
            }

            // spriteBatch.DrawPoint(center, Color.Red, 3);
        }

        void doneAnimation()
        {

        }
    }




}