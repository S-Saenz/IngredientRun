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
        Random random = new Random();
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

        
        // this is temporary for the playtest
        public void walkSound(GameTime gameTime) {
            if ((gameTime.TotalGameTime.TotalMilliseconds - walkTimer >= 500))
            {
                float v = (random.Next(5) + 5.0f) / (100);
                soundeffects[0].Play(volume: v, pitch: 0.0f, pan: 0.0f);
                walkTimer = (int)gameTime.TotalGameTime.TotalMilliseconds;
            }
        }
        public void runSound(GameTime gameTime)
        {
            if ((gameTime.TotalGameTime.TotalMilliseconds - walkTimer >= 500))
            {
                float v = 10.0f / (100.0f - random.Next(5));
                soundeffects[0].Play(volume: 1.0f, pitch: 0.0f, pan: 0.0f);
                walkTimer = (int)gameTime.TotalGameTime.TotalMilliseconds;
            }
        }

        public void stop()
        {
            MediaPlayer.Stop();
        }
    }
}
