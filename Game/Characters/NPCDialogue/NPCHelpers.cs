namespace WillowWoodRefuge
{
    delegate void NPCAction();
    public class Condition
    {
        public string _name;
        public bool _flag;

        public Condition(string name, bool startState)
        {
            _name = name;
            _flag = startState;
        }
    }
}
