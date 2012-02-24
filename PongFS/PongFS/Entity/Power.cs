using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PongFS.Entity
{
    public struct Power
    {
        // type set in order for sprite
        public enum PowerType
        {
            HealAll = 0,
            ReverseKeys = 1,
            Power = 2,
            Ice = 3,
            Speed = 4,
            Wall = 5,
            RevealAim = 6,
            Fortress = 7,
            Bend = 8,
            HealSingle = 9
        }

        public PowerType type;
        public TimeSpan acquired;
    }
}
