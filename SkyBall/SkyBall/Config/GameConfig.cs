using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using SkyBall.Core;

namespace SkyBall.Config
{
    public class GameConfig
    {
        public static int WIDTH = 500;
        public static int HEIGHT = 700;
        public static int WIN_BORDER = 3;
        private static string cfg_file = "userprefs.sky";
        public static bool Multiplayer { get; set; }
        public static UserConfig UserPref { get; set; }
        private static string currentPath;

        public static void LoadUserPrefs(string path){
            currentPath = path;
            if (File.Exists(Path.Combine(currentPath, cfg_file)))
            {
                UserPref = (UserConfig)Tools.LoadObjectFromFile(Path.Combine(path, cfg_file));
            }
            else
            {
                UserPref = new UserConfig
                {
                    IsMusicOn = true,
                    IsSoundFxOn = true,
                    Player1KeybLayout = new KeyboardLayout(KeyboardLayout.Default.Arrows),
                    Player2KeybLayout = new KeyboardLayout(KeyboardLayout.Default.AWSD)
                };
                SaveUserPrefs();
            }
        }

        public static void SaveUserPrefs()
        {
            Tools.SaveObjectToFile(UserPref, (Path.Combine(currentPath, cfg_file)));
        }

    }
}
