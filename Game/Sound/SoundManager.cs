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
        List<SoundEffect> monsterSounds;
        Random random = new Random();
        int walkTimer = 0;
        int spiderTimer1 = 0;
        int spiderTimer2 = 0;
        int spiderTimer2_5 = 2000;
        public SoundManager(ContentManager Content)
        {
            MediaPlayer.IsRepeating = true;
            songs = new Dictionary<string, Song>();
            soundeffects = new List<SoundEffect>();
            monsterSounds = new List<SoundEffect>();
            // Song names
            songs.Add("forestSong", Content.Load<Song>("music/forestSong"));
            songs.Add("caveSong", Content.Load<Song>("music/spooky1test2"));
            // song names end

            // sound effects
            // 0
            soundeffects.Add(Content.Load<SoundEffect>("soundEffects/stepC"));
            // 1
            soundeffects.Add(Content.Load<SoundEffect>("soundEffects/Player_Jump"));
            // 2
            soundeffects.Add(Content.Load<SoundEffect>("soundEffects/Player_Landing"));
            // 3
            soundeffects.Add(Content.Load<SoundEffect>("soundEffects/Player_Hit"));

            //monster sounds
            //0
            monsterSounds.Add(Content.Load<SoundEffect>("soundEffects/Spider_Skitter"));
            //1
            monsterSounds.Add(Content.Load<SoundEffect>("soundEffects/Spider_Crawl"));
            // Sound effects end
        }
        public void playSong(string name)
        {
            if (songs.ContainsKey(name))
                MediaPlayer.Play(songs[name]); //Derek doesn't want the music!
            else
                Debug.WriteLine("Incorrect Song name " + name + ", refrence lines after the comment labeled \"Song names\" in the sound manager class for correct name");
        }

        
        public void walkSound(GameTime gameTime) {
            // change the last value of the if statement to increace or decreace the tiem between steps
            if ((gameTime.TotalGameTime.TotalMilliseconds - walkTimer >= 500))
            {
                float v = (random.Next(5) + 5.0f) / (100);
                soundeffects[0].Play(volume: v, pitch: 0.0f, pan: 0.0f);
                walkTimer = (int)gameTime.TotalGameTime.TotalMilliseconds;
            }
        }
        public void runSound(GameTime gameTime)
        {
            // change the last value of the if statement to increace or decreace the tiem between steps
            if ((gameTime.TotalGameTime.TotalMilliseconds - walkTimer >= 500))
            {
                float v = (10.0f + random.Next(5)) / (100.0f );
                soundeffects[0].Play(volume: v, pitch: 0.0f, pan: 0.0f);
                walkTimer = (int)gameTime.TotalGameTime.TotalMilliseconds;
            }
        }

        public void jumpSound()
        {
            soundeffects[1].Play();
        }

        public void landSound(float velocity, float maxVelocity)
        {

            float volume = velocity / maxVelocity;
            volume = volume * ( (90.0f + random.Next(10)) / (100.0f) );
            if (volume > 1)
            {
                volume = 1;
            }
            soundeffects[2].Play(volume: volume, pitch: 0.0f, pan: 0.0f);
        }

        public void hitSound()
        {
            soundeffects[3].Play();
        }

        public void spiderAmbient(GameTime gameTime, float distance)
        {
            if (gameTime.TotalGameTime.TotalMilliseconds - spiderTimer2 >= spiderTimer2_5)
            {
                monsterSounds[1].Play(volume: ((300 - distance)/300) * 0.5f, pitch: 0.0f, pan: 0.0f);
                spiderTimer2 = (int)gameTime.TotalGameTime.TotalMilliseconds;
                spiderTimer2_5 = 2000 + random.Next(1000) - 250;
            }
        }

        public void spiderAttack(GameTime gameTime)
        {
            if (gameTime.TotalGameTime.TotalMilliseconds - spiderTimer1 >= 500)
            {
                monsterSounds[0].Play();
                spiderTimer1 = (int)gameTime.TotalGameTime.TotalMilliseconds;
            }
        }

        public void stop()
        {
            MediaPlayer.Stop();
        }
    }
}
