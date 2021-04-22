using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace WillowWoodRefuge
{
    public class StateConditions
    {
        public List<Condition> conditionList = new List<Condition>();
        delegate void NPCAction();

        public StateConditions()
        {
            // add all the game's onditions
            conditionList.Add(new Condition("fedMushroomPrior", true));
            conditionList.Add(new Condition("curedPrior", true));
            conditionList.Add(new Condition("isMorning", true));
            conditionList.Add(new Condition("isRaining", false));
            // add the events that will ocurr on each event
            conditionList[0].ThisEvent += c_FedMushroom;
        }
        // create the event methods
        static void c_FedMushroom(object sender, OnEventArgs e)
        {
            
            Debug.WriteLine("The mushroom was fed at {0}.", e.TimeReached);
        }

        public class Condition
        {
            public string _name;
            public bool _flag;


            public Condition(string name, bool startState)
            {
                _name = name;
                _flag = startState;
            }

            public void OnEvent(OnEventArgs e)
            {
                EventHandler<OnEventArgs> handler = ThisEvent;
                e.TimeReached = DateTime.Now;
                if (handler != null)
                {
                    handler(this, e);
                }
            }

            public event EventHandler<OnEventArgs> ThisEvent;
        }

        

        public void ConditionUpdate (GameTime gametime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.O))
            {
                OnEventArgs args = new OnEventArgs();
                args.TimeReached = DateTime.Now;
                conditionList[0].OnEvent(args);
            }
        }

        public class OnEventArgs : EventArgs
        {
            public DateTime TimeReached { get; set; }
        }
    }
}