using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.IO;
using Microsoft.Xna.Framework.Audio;

namespace PongFS.Core
{
    public class SoundFactory
    {
        private Game game;
        private static SoundFactory factory = null;
        private Dictionary<String, SoundEffect> sounds = new Dictionary<string, SoundEffect>();

        private SoundFactory() { }

        public static SoundFactory getFactory()
        {
            if (factory == null)
            {
                factory = new SoundFactory();
            }
            return factory;
        }

        public void Initialize(Game game)
        {
            this.game = game;
        }

        public void Add(string soundFilename, bool isLooped)
        {
            if (!sounds.ContainsKey(soundFilename))
            {
                SoundEffect sound = game.Content.Load<SoundEffect>("fx/" + soundFilename);
                sounds.Add(soundFilename, sound);
                if (isLooped)
                {
                    SoundEffectInstance instance = sound.CreateInstance();
                    instance.IsLooped = true;
                }
            }
        }

        public void Play(string sound, bool canOverlap)
        {
            SoundEffect fx = null;
            sounds.TryGetValue(sound, out fx);
            SoundEffectInstance si = fx.CreateInstance();
            if (canOverlap || (si.State != SoundState.Playing && !canOverlap))
            {
                fx.Play();
            }
        }

        public void Stop(string sound)
        {
            SoundEffect fx = null;
            sounds.TryGetValue(sound, out fx);
            SoundEffectInstance si = fx.CreateInstance();
            si.Stop();
        }
    }
}
