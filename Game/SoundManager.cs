using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace IngredientRun
{
    public class SoundManager
    {
        Dictionary<string, Song> songs;
        List<Condition> songConditions = new List<Condition>();
        public SoundManager(ContentManager Content)
        {
            MediaPlayer.IsRepeating = true;
            songs = new Dictionary<string, Song>();
            //song names
            songs.Add("forestSong", Content.Load<Song>("music/forestSong"));
            songs.Add("caveSong", Content.Load<Song>("music/spooky1test2"));
            //song names end
        }
        public void playSong(string name)
        {
            if (songs.ContainsKey(name))
                MediaPlayer.Play(songs[name]);
            else
                Debug.WriteLine("Incorrect Song name " + name + ", refrence lines 17, onwards in the sound manager class for correct name");
        }

        public void stop()
        {
            MediaPlayer.Stop();
        }
    }
}
