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
        public List<List <Condition>> masterConditionList = new List<List<Condition>>();
        public List<Condition> generalConditionList = new List<Condition>();
        public List<Condition> weatherConditions = new List<Condition>();
        public List<Condition> curedCondtions = new List<Condition>();
        delegate void NPCAction();

        public StateConditions()
        {
            // master condition list
            masterConditionList.Add(generalConditionList);
            masterConditionList.Add(weatherConditions);
            masterConditionList.Add(curedCondtions);
            // add all the game's conditions

            // general conditions
            generalConditionList.Add(new Condition("isMorning", true));

            //cured (quest) conditions
            curedCondtions.Add(new Condition("fedMushroomPrior", true));
            curedCondtions.Add(new Condition("curedPrior", true));
            
            // weather Conditions
            weatherConditions.Add(new Condition("isRaining", false));
            weatherConditions.Add(new Condition("isFoggy", false));
            // add the events that will ocurr on each event
            generalConditionList[0].ThisEvent += c_FedMushroom;
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
                generalConditionList[0].OnEvent(args);
            }
        }

        public class OnEventArgs : EventArgs
        {
            public DateTime TimeReached { get; set; }
        }
    }
}