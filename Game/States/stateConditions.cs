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
            conditionList.Add(new Condition("isRaining", true));
        }
    }
}