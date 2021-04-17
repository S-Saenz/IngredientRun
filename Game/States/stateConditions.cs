using System;
using System.Collections.Generic;
using System.Text;

namespace WillowWoodRefuge
{
    public class StateConditions
    {
        public List<Condition> conditionList = new List<Condition>();


        public StateConditions()
        {
            conditionList.Add(new Condition("fedMushroomPrior", true));
            conditionList.Add(new Condition("curedPrior", true));
            conditionList.Add(new Condition("isMorning", true));
            conditionList.Add(new Condition("isRaining", false));
            conditionList[3].ThresholdReached += c_ThresholdReached;
        }
        static void c_ThresholdReached(object sender, ThresholdReachedEventArgs e)
        {
            Console.WriteLine("The rain started at {1}.", e.TimeReached);
            Environment.Exit(0);
        }

        delegate void NPCAction();
        public class Condition
        {
            public string _name;
            public bool _flag;
            public event EventHandler<ThresholdReachedEventArgs> ThresholdReached;

            public Condition(string name, bool startState)
            {
                _name = name;
                _flag = startState;
            }
        }

        public class ThresholdReachedEventArgs : EventArgs
        {
            public DateTime TimeReached { get; set; }
        }
    }
}