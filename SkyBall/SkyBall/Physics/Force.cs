using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SkyBall.Physics
{
    public struct Force
    {
        public Vector2 speed;
        public Vector2 acceleration;
        public bool enabled;

        public static Force GroundFriction
        {
            get
            {
                return new Force { enabled = true, speed = new Vector2(0.85f) };
            }
        }

        public static Force HighSpeedFriction
        {
            get
            {
                return new Force { enabled = true, speed = new Vector2(0.8f) };
            }
        }

        public static Force IceFriction
        {
            get
            {
                return new Force { enabled = true, speed = new Vector2(0.95f) };
            }
        }


    }
}
