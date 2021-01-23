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

        private bool forHittingPlayOnce = true; //pro naming convention, i know. the thing about fmod events (particularly streams like ambiences and music) is that they get restarted every time start() is called.
        //thus, we need a way for the ambiences to only proc once. i'm using this dumb bool for demo purposes but a more reliable solution is needed


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
            if (forHittingPlayOnce)
            {
                EventDescription caveAmbienceDesc = StudioSystem.GetEvent("event:/Cave Ambience");
                EventInstance caveAmbience = caveAmbienceDesc.CreateInstance();
                caveAmbience.Start();
            }
        }







    }
}
