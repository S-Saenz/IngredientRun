using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using Keys = Microsoft.Xna.Framework.Input.Keys;

namespace WillowWoodRefuge
{
    public class InputManager
    {
        private Dictionary<string, Button> _buttons = new Dictionary<string, Button>();
        private KeyboardState oldstate = Keyboard.GetState();

        public void Initialize()//initializes default button mapping
        {
            List<Keys> leftKeys = new List<Keys>()
            {
                Keys.Left,
                Keys.A
            };
            List<Keys> rightKeys = new List<Keys>()
            {
                Keys.Right,
                Keys.D
            };
            List<Keys> downKeys = new List<Keys>()
            { 
                Keys.Down,
                Keys.S
            };
            List<Keys> upKeys = new List<Keys>()
            {
                Keys.Up,
                Keys.W
            };
            List<Keys> spaceKeys = new List<Keys>()
            {
                Keys.Space
            };
            List<Keys> jumpKeys = new List<Keys>()
            {
                Keys.Space,
                Keys.Up,
                Keys.W
            };
            List<Keys> cookKeys = new List<Keys>()
            {
                Keys.Space
            };
            List<Keys> superCookKeys = new List<Keys>()
            {
                Keys.Enter
            };
            List<Keys> interactKeys = new List<Keys>()
            {
                Keys.E
            };
            List<Keys> runKeys = new List<Keys>()
            {
                Keys.LeftShift,
                Keys.RightShift
            };
            List<Keys> inventory = new List<Keys>()
            {
                Keys.I
            };
            List<Keys> alternate = new List<Keys>()
            {
                Keys.LeftAlt,
                Keys.RightAlt
            };
            List<Keys> toggleWindowed = new List<Keys>()
            {
                Keys.Enter
            };
            List<Keys> debugFullToggle = new List<Keys>()
            {
                Keys.LeftControl
            };
            List<Keys> debugMiniToggle = new List<Keys>()
            {
                Keys.RightControl
            };
            List<Keys> changeCampState = new List<Keys>()
            {
                Keys.D1
            };
            List<Keys> changeCaveState = new List<Keys>()
            {
                Keys.D2
            };
            List<Keys> restartState = new List<Keys>()
            {
                Keys.R
            };
            List<Keys> togglePerformance = new List<Keys>()
            {
                Keys.P
            };
            List<Keys> toggleLight = new List<Keys>()
            {
                Keys.L
            };
            List<Keys> escape = new List<Keys>()
            {
                Keys.Escape
            };

            _buttons.Add("left", new Button(leftKeys));
            _buttons.Add("right", new Button(rightKeys));
            _buttons.Add("down", new Button(downKeys));
            _buttons.Add("up", new Button(upKeys));
            _buttons.Add("space", new Button(spaceKeys));
            _buttons.Add("jump", new Button(jumpKeys));
            _buttons.Add("cook", new Button(cookKeys));
            _buttons.Add("superCook", new Button(superCookKeys));
            _buttons.Add("interact", new Button(interactKeys));
            _buttons.Add("run", new Button(runKeys));
            _buttons.Add("inventory", new Button(inventory));
            _buttons.Add("debugFullToggle", new Button(debugFullToggle));
            _buttons.Add("debugMiniToggle", new Button(debugMiniToggle));
            _buttons.Add("alternate", new Button(alternate));
            _buttons.Add("toggleWindowed", new Button(toggleWindowed));
            _buttons.Add("changeCampState", new Button(changeCampState));
            _buttons.Add("changeCaveState", new Button(changeCaveState));
            _buttons.Add("restartState", new Button(restartState));
            _buttons.Add("performance", new Button(togglePerformance));
            _buttons.Add("light", new Button(toggleLight));
            _buttons.Add("escape", new Button(escape));
        }

        public void Update(GameTime time)
        {
            KeyboardState newstate = Keyboard.GetState();
            //for each button
            foreach (KeyValuePair<string, Button> entry in _buttons)
            {
                Button button = entry.Value;
                button._isDown = false;
                button._justPressed = false;
                button._justReleased = false;
                //for each key in button
                foreach (Keys key in button._keys)
                {
                    //check state and update
                    
                    if (newstate.IsKeyDown(key) && oldstate.IsKeyUp(key))
                    {
                        button._isDown = true;
                        button._justPressed = true;
                        break;
                        //Debug.WriteLine("is down"); this works
                    }
                    else if (newstate.IsKeyDown(key) && oldstate.IsKeyDown(key))
                    {
                        button._isDown = true;
                        button._justPressed = false;
                        break;
                    }
                    else if(newstate.IsKeyUp(key) && oldstate.IsKeyDown(key))
                    {
                        button._isDown = false;
                        button._justReleased = true;
                        break;
                    }
                    /*else if (newstate.IsKeyUp(key) && oldstate.IsKeyUp(key))
                    {
                        button._isDown = false;
                        button._justReleased = false;
                    }*/
                    
                }
            }
            oldstate = newstate;
        }

        public bool IsDown(string buttonName)
        {
            return _buttons[buttonName]._isDown;
        }
        public bool JustPressed(string buttonName)
        {
            return _buttons[buttonName]._justPressed;
        }
        public bool JustReleased(string buttonName)
        {
            return _buttons[buttonName]._justReleased;
        }
        public void newmap(string buttonName, Keys newKey)
        {
            _buttons[buttonName]._keys.Add(newKey);
        }
        public bool unmap(string buttonName, Keys oldKey)
        {
            return _buttons[buttonName]._keys.Remove(oldKey);
        }
        public bool swapmap(string buttonName, Keys oldKey, Keys newKey)
        {
            if(unmap(buttonName, oldKey))
            {
                newmap(buttonName, newKey);
                return true;
            }
            return false;           
        }
    }

    class Button
    {
        public bool _isDown, _justPressed, _justReleased = false;
        public List<Keys> _keys;

        public Button(List<Keys> keys)
        {
            _keys = keys;
        }
    }
}
