using System;
using System.Collections.Generic;
using System.Text;
using ChaiFoxes.FMODAudio.Studio;
using ChaiFoxes.FMODAudio;

namespace IngredientRun
{
    class MusicManager
    {
        private List<Bank> banksList = new List<Bank>();
        private List<Bank> caveBanksList = new List<Bank>();


        public void LoadBanks()
        {
            banksList.Add(StudioSystem.LoadBank("Master.bank"));
            banksList.Add(StudioSystem.LoadBank("Master.strings.bank"));
            caveBanksList.Add(StudioSystem.LoadBank("Cave.bank")); //it would probably be better to have separate functions for each level's banks, since you have to load undload etc

        }

        /*
        public void UnloadBanks(List currentBanks)
        {

        }
        */

        public void PlayCaveStream() //demo dependent
        {
            EventDescription caveAmbienceDesc = StudioSystem.GetEvent("event:/Cave Ambience");
            EventInstance caveAmbience = caveAmbienceDesc.CreateInstance();
            caveAmbience.Start();
        }







    }
}
