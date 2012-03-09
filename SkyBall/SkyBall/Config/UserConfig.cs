using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SkyBall.Config
{
    [Serializable]
    public class UserConfig
    {
        public KeyboardLayout Player1KeybLayout{get;set;}
        public KeyboardLayout Player2KeybLayout{get;set;}
        public bool IsMusicOn { get; set; }
        public bool IsSoundFxOn { get; set; }
    }
}
