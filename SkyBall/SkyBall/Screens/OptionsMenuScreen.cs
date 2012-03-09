using Microsoft.Xna.Framework;
using SkyBall.Config;


namespace SkyBall
{
    /// <summary>
    /// The options screen is brought up over the top of the main menu
    /// screen, and gives the user a chance to configure the game
    /// in various hopefully useful ways.
    /// </summary>
    class OptionsMenuScreen : MenuScreen
    {
        MenuEntry customizeKeysMenuEntry;
        MenuEntry musicMenuEntry;
        MenuEntry soundMenuEntry;

        /// <summary>
        /// Constructor.
        /// </summary>
        public OptionsMenuScreen()
            : base("")
        {
            // Create our menu entries.
            customizeKeysMenuEntry = new MenuEntry(string.Empty);
            musicMenuEntry = new MenuEntry(string.Empty);
            soundMenuEntry = new MenuEntry(string.Empty);

            SetMenuEntryText();

            MenuEntry back = new MenuEntry("Back");

            // Hook up menu event handlers.
            customizeKeysMenuEntry.Selected += CustomizeKeysMenuEntrySelected;
            musicMenuEntry.Selected += MusicMenuEntrySelected;
            soundMenuEntry.Selected += ElfMenuEntrySelected;
            back.Selected += OnCancel;
            
            // Add entries to the menu.
            MenuEntries.Add(customizeKeysMenuEntry);
            MenuEntries.Add(musicMenuEntry);
            MenuEntries.Add(soundMenuEntry);
            MenuEntries.Add(back);
        }


        /// <summary>
        /// Fills in the latest values for the options screen menu text.
        /// </summary>
        void SetMenuEntryText()
        {
            customizeKeysMenuEntry.Text = "Customize keys";
            musicMenuEntry.Text = "Music: " + (GameConfig.UserPref.IsMusicOn ? "on" : "off");
            soundMenuEntry.Text = "Sound: " + (GameConfig.UserPref.IsSoundFxOn ? "on" : "off");
        }

        void CustomizeKeysMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.AddScreen(new CustomizeKeysScreen(), e.PlayerIndex);
        }

        void MusicMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            GameConfig.UserPref.IsMusicOn = !GameConfig.UserPref.IsMusicOn;
            //TODO: start or stop music here
            SetMenuEntryText();
        }

        void ElfMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            GameConfig.UserPref.IsSoundFxOn = !GameConfig.UserPref.IsSoundFxOn;

            SetMenuEntryText();
        }


        
    }
}
