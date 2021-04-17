using System;

namespace WillowWoodRefuge
{
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
