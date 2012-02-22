using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace PongFS.Config
{
    public class KeyboardLayout
    {
        public Keys KeyUp{get;set;}
        public Keys KeyDown{get;set;}
        public Keys KeyLeft{get;set;}
        public Keys KeyRight{get;set;}
        public Keys KeyAction { get; set; }

        public enum Default { Arrows, QZSD, AWSD }

        public KeyboardLayout(Default defaultSet)
        {
            switch(defaultSet){
                case Default.Arrows: default:
                    KeyUp = Keys.Up;
                    KeyDown = Keys.Down;
                    KeyLeft = Keys.Left;
                    KeyRight = Keys.Right;
                    KeyAction = Keys.RightControl;
                    break;
                case Default.QZSD:
                    KeyUp = Keys.Z;
                    KeyDown = Keys.S;
                    KeyLeft = Keys.Q;
                    KeyRight = Keys.D;
                    KeyAction = Keys.LeftControl;
                    break;
                case Default.AWSD:
                    KeyUp = Keys.W;
                    KeyDown = Keys.S;
                    KeyLeft = Keys.A;
                    KeyRight = Keys.D;
                    KeyAction = Keys.LeftControl;
                    break;
            }
        }

    }
}
