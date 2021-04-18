using System;
using System.Collections.Generic;
using System.Text;

namespace WillowWoodRefuge
{
    class ForageInfo
    {
        static Dictionary<string, ForageInfo> _allForageInfo = new Dictionary<string, ForageInfo>()
        {
            { "test", new ForageInfo(6, 20) },

        };
        public static Dictionary<string, ForageInfo> _forageInfo
        {
            get { return _allForageInfo; }
        }

        public int _numPhases { get; private set; }
        public float _growDuration { get; private set; }

        public ForageInfo(int numPhases, float growDuration)
        {
            _numPhases = numPhases;
            _growDuration = growDuration;
        }
    }
}
