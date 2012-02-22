using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PongFS.Physics
{
    public class Force
    {
        public Vector2 Speed { get; set; }
        public Vector2 Acceleration { get; set; }
        public bool Enabled { get; set; }

        public Force()
        {
            Enabled = true;
        }
    }
}
