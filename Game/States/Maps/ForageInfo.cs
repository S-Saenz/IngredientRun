using System;
using System.Collections.Generic;
using System.Text;

namespace WillowWoodRefuge
{
    class ForageInfo
    {
        static Dictionary<string, ForageInfo> _allForageInfo = new Dictionary<string, ForageInfo>()
        {
            // Name                                screen name    phases  duration  fromEmpty
            { "test",             new ForageInfo("Test",             6,      10,      false) },
            { "Bay_Nut",          new ForageInfo("Bay Nut",          3,       8,      false) },
            { "Calamint",         new ForageInfo("Calamint",         1,      20,      true ) },
            { "Mint",             new ForageInfo("Mint",             1,      10,      true ) },
            { "Huckleberry",      new ForageInfo("Huckleberry",      2,      10,      false) },
            { "Hummingbird_Sage", new ForageInfo("Hummingbird Sage", 1,      10,      true ) },
            { "Manzanita",        new ForageInfo("Manzanita",        2,      11,      false) },
            { "Nodding_Onion",    new ForageInfo("Nodding Onion",    1,      10,      true ) },
            { "Oyster_Mushroom",  new ForageInfo("Oyster Mushroom",  1,      10,      true ) },
            { "Thimbleberry",     new ForageInfo("Thimbleberry",     2,      10,      false) },
            { "Toothwort",        new ForageInfo("Toothwort",        1,      10,      true ) },
            { "Wolfberry",        new ForageInfo("Wolfberry",        2,      15,      false) },
            { "water",            new ForageInfo("Water",            1,       0,      true ) },
        };
        public static Dictionary<string, ForageInfo> _forageInfo
        {
            get { return _allForageInfo; }
        }

        public string _screenName { get; private set; }
        public int _numPhases { get; private set; }
        public float _growDuration { get; private set; }
        public bool _fromEmpty { get; private set; } // whether 0 growth is empty space or not

        public ForageInfo(string screenName, int numPhases, float growDuration, bool fromEmpty)
        {
            _screenName = screenName;
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
