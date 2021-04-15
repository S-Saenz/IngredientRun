using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace WillowWoodRefuge
{
    public class SoundManager
    {
        Dictionary<string, Song> songs;
        List<SoundEffect> soundeffects;

        int walkTimer = 0;
        public SoundManager(ContentManager Content)
        {
            MediaPlayer.IsRepeating = true;
            songs = new Dictionary<string, Song>();
            soundeffects = new List<SoundEffect>();
            // Song names
            songs.Add("forestSong", Content.Load<Song>("music/forestSong"));
            songs.Add("caveSong", Content.Load<Song>("music/spooky1test2"));
            // song names end
            // sound effects
            // 0
            soundeffects.Add(Content.Load<SoundEffect>("soundEffects/stepC"));
            // Sound effects end
        }
        public void playSong(string name)
        {
            if (songs.ContainsKey(name))
                MediaPlayer.Play(songs[name]); //Derek doesn't want the music!
            else
                Debug.WriteLine("Incorrect Song name " + name + ", refrence lines after the comment labeled \"Song names\" in the sound manager class for correct name");
        }

        public void playSF(int i) {
            soundeffects[i].Play(volume: 0.1f, pitch: 0.0f, pan: 0.0f);
        }

        // this is temporary for the playtest
        public void runSound(GameTime gameTime) {
            if ((gameTime.TotalGameTime.TotalMilliseconds - walkTimer >= 550))
            {
                playSF(0);
                walkTimer = (int)gameTime.TotalGameTime.TotalMilliseconds;
            }
        }

        public void stop()
        {
            MediaPlayer.Stop();
        }
    }
}
