using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.IO;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace SkyBall.Core
{
    public class SoundFactory
    {
        private ContentManager content;
        private static SoundFactory factory = null;
        private Dictionary<String, SoundEffect> sounds = new Dictionary<string, SoundEffect>();
        private Dictionary<String, SoundEffectInstance> instances = new Dictionary<string, SoundEffectInstance>();

        private SoundFactory() { }

        public static SoundFactory getFactory()
        {
            if (factory == null)
            {
                factory = new SoundFactory();
            }
            return factory;
        }

        public void Initialize(ContentManager content)
        {
            this.content = content;
        }

        public void AddSound(string key, string filename, bool isLooped = false)
        {
            if (!sounds.ContainsKey(key))
            {
                sounds.Add(key, content.Load<SoundEffect>(filename));
            }
        }

        public void PlaySound(string key, bool isSingle = false)
        {
            if (sounds.ContainsKey(key))
            {
                if (!isSingle)
                {
                    sounds[key].Play();
                }
                else
                {
                    if(!instances.ContainsKey(key)){
                        instances.Add(key, sounds[key].CreateInstance());
                    }

                    if(instances[key].State != SoundState.Playing){
                        instances[key].Play();
                    }
                }
            }
        }

        public void Stop(string key)
        {
            if (instances.ContainsKey(key))
            {
                instances[key].Stop();
            }
        }

        public void StopAll()
        {
            foreach (SoundEffectInstance sound in instances.Values)
            {
                sound.Stop();
            }
        }

        internal void RemoveAll()
        {
            instances.Clear();
            sounds.Clear();
        }
    }
}
