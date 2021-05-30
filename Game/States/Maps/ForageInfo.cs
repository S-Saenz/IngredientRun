using System;
using System.Collections.Generic;
using System.Text;

namespace WillowWoodRefuge
{
    class ForageInfo
    {
        static Dictionary<string, ForageInfo> _allForageInfo = new Dictionary<string, ForageInfo>()
        {
            // Name                           phases  duration  fromEmpty
            { "test",             new ForageInfo(6,      10,      false) },
            { "Bay_Nut",          new ForageInfo(3,      10,      false) },
            { "Calamint",         new ForageInfo(1,      10,      true ) },
            { "Mint",             new ForageInfo(1,      10,      true ) },
            { "Huckleberry",      new ForageInfo(2,      10,      false) },
            { "Hummingbird_Sage", new ForageInfo(1,      10,      true ) },
            { "Manzanita",        new ForageInfo(2,      10,      false) },
            { "Nodding_Onion",    new ForageInfo(1,      10,      true ) },
            { "Oyster_Mushroom",  new ForageInfo(1,      10,      true ) },
            { "Thimbleberry",     new ForageInfo(2,      10,      false) },
            { "Toothwort",        new ForageInfo(1,      10,      true ) },
            { "Wolfberry",        new ForageInfo(2,      10,      false) },
            { "water",            new ForageInfo(1,       0,      true ) },
        };
        public static Dictionary<string, ForageInfo> _forageInfo
        {
            get { return _allForageInfo; }
        }

        public int _numPhases { get; private set; }
        public float _growDuration { get; private set; }
        public bool _fromEmpty { get; private set; } // whether 0 growth is empty space or not

        public ForageInfo(int numPhases, float growDuration, bool fromEmpty)
        {
            _numPhases = numPhases;
            _growDuration = growDuration;
            _fromEmpty = fromEmpty;
        }

        public static ForageInfo GetInfo(string name)
        {
            if(_allForageInfo.ContainsKey(name))
            {
                return _allForageInfo[name];
            }
            else
            {
                return null;
            }
        }
    }
}
