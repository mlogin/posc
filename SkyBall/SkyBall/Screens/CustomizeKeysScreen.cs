using Microsoft.Xna.Framework;
using SkyBall.Config;
using Microsoft.Xna.Framework.Input;


namespace SkyBall
{
    /// <summary>
    /// The options screen is brought up over the top of the main menu
    /// screen, and gives the user a chance to configure the game
    /// in various hopefully useful ways.
    /// </summary>
    class CustomizeKeysScreen : MenuScreen
    {
        MenuEntry p1LeftMenuEntry, p1RightMenuEntry, p1UpMenuEntry, p1DownMenuEntry, p1ActionMenuEntry;
        MenuEntry p2LeftMenuEntry, p2RightMenuEntry, p2UpMenuEntry, p2DownMenuEntry, p2ActionMenuEntry;
        private bool choosingKey;
        private string currentMsg;

        /// <summary>
        /// Constructor.
        /// </summary>
        public CustomizeKeysScreen()
            : base("")
        {
            p1LeftMenuEntry = new MenuEntry(string.Empty);
            p1RightMenuEntry = new MenuEntry(string.Empty);
            p1UpMenuEntry = new MenuEntry(string.Empty);
            p1DownMenuEntry = new MenuEntry(string.Empty);
            p1ActionMenuEntry = new MenuEntry(string.Empty);
            p2LeftMenuEntry = new MenuEntry(string.Empty);
            p2RightMenuEntry = new MenuEntry(string.Empty);
            p2UpMenuEntry = new MenuEntry(string.Empty);
            p2DownMenuEntry = new MenuEntry(string.Empty);
            p2ActionMenuEntry = new MenuEntry(string.Empty);
            SetMenuEntryText();
            
            MenuEntry back = new MenuEntry("Back");

            // Hook up menu event handlers.
            back.Selected += OnCancel;
            MenuEntries.Add(new MenuLabel("Player 1", Color.Yellow ));
            MenuEntries.Add(p1LeftMenuEntry);
            MenuEntries.Add(p1RightMenuEntry);
            MenuEntries.Add(p1UpMenuEntry);
            MenuEntries.Add(p1DownMenuEntry);
            MenuEntries.Add(p1ActionMenuEntry);
            MenuEntries.Add(new MenuLabel("", Color.Transparent));
            MenuEntries.Add(new MenuLabel("Player 2", Color.Yellow));
            MenuEntries.Add(p2LeftMenuEntry);
            MenuEntries.Add(p2RightMenuEntry);
            MenuEntries.Add(p2UpMenuEntry);
            MenuEntries.Add(p2DownMenuEntry);
            MenuEntries.Add(p2ActionMenuEntry);
            MenuEntries.Add(new MenuLabel("", Color.Transparent));
            MenuEntries.Add(back);

            this.SelectMenuEntryIndex(1);

            p1LeftMenuEntry.Selected += new System.EventHandler<PlayerIndexEventArgs>(p1LeftMenuEntry_Selected);
            p1RightMenuEntry.Selected += new System.EventHandler<PlayerIndexEventArgs>(p1RightMenuEntry_Selected);
            p1UpMenuEntry.Selected += new System.EventHandler<PlayerIndexEventArgs>(p1UpMenuEntry_Selected);
            p1DownMenuEntry.Selected += new System.EventHandler<PlayerIndexEventArgs>(p1DownMenuEntry_Selected);
            p1ActionMenuEntry.Selected += new System.EventHandler<PlayerIndexEventArgs>(p1ActionMenuEntry_Selected);
            p2LeftMenuEntry.Selected += new System.EventHandler<PlayerIndexEventArgs>(p2LeftMenuEntry_Selected);
            p2RightMenuEntry.Selected += new System.EventHandler<PlayerIndexEventArgs>(p2RightMenuEntry_Selected);
            p2UpMenuEntry.Selected += new System.EventHandler<PlayerIndexEventArgs>(p2UpMenuEntry_Selected);
            p2DownMenuEntry.Selected += new System.EventHandler<PlayerIndexEventArgs>(p2DownMenuEntry_Selected);
            p2ActionMenuEntry.Selected += new System.EventHandler<PlayerIndexEventArgs>(p2ActionMenuEntry_Selected);
        }

        private void p1ActionMenuEntry_Selected(object sender, PlayerIndexEventArgs e)
        {
            GetNewInputKeyFor(1, "hit", e);
        }

        private void p1DownMenuEntry_Selected(object sender, PlayerIndexEventArgs e)
        {
            GetNewInputKeyFor(1, "move down", e);
        }

        private void p1UpMenuEntry_Selected(object sender, PlayerIndexEventArgs e)
        {
            GetNewInputKeyFor(1, "move up", e);
        }

        private void p1RightMenuEntry_Selected(object sender, PlayerIndexEventArgs e)
        {
            GetNewInputKeyFor(1, "move right", e);
        }

        private void p1LeftMenuEntry_Selected(object sender, PlayerIndexEventArgs e)
        {
            GetNewInputKeyFor(1, "move left", e);
        }

        private void p2ActionMenuEntry_Selected(object sender, PlayerIndexEventArgs e)
        {
            GetNewInputKeyFor(2, "hit", e);
        }

        private void p2DownMenuEntry_Selected(object sender, PlayerIndexEventArgs e)
        {
            GetNewInputKeyFor(2, "move down", e);
        }

        private void p2UpMenuEntry_Selected(object sender, PlayerIndexEventArgs e)
        {
            GetNewInputKeyFor(2, "move up", e);
        }

        private void p2RightMenuEntry_Selected(object sender, PlayerIndexEventArgs e)
        {
            GetNewInputKeyFor(2, "move right", e);
        }

        private void p2LeftMenuEntry_Selected(object sender, PlayerIndexEventArgs e)
        {
            GetNewInputKeyFor(2, "move left", e);
        }

        private void GetNewInputKeyFor(int playerNb, string action, PlayerIndexEventArgs e)
        {
            if (!choosingKey)
            {
                currentMsg = GetInputMessage(playerNb, action);
                GetInputKeyMsgBoxScreen screen = new GetInputKeyMsgBoxScreen(currentMsg);
                screen.KeyHit += new System.EventHandler(screen_KeyHit);
                ScreenManager.AddScreen(screen, e.PlayerIndex);
                choosingKey = true;
            }
        }

        void screen_KeyHit(object sender, System.EventArgs e)
        {
            Keys Key = (Keys)sender;
            if (currentMsg == GetInputMessage(1, "move left"))
            {
                GameConfig.UserPref.Player1KeybLayout.KeyLeft = Key;
            }
            else if (currentMsg == GetInputMessage(1, "move right"))
            {
                GameConfig.UserPref.Player1KeybLayout.KeyRight = Key;
            }
            else if (currentMsg == GetInputMessage(1, "move up"))
            {
                GameConfig.UserPref.Player1KeybLayout.KeyUp = Key;
            }
            else if (currentMsg == GetInputMessage(1, "move down"))
            {
                GameConfig.UserPref.Player1KeybLayout.KeyDown = Key;
            }
            else if (currentMsg == GetInputMessage(1, "hit"))
            {
                GameConfig.UserPref.Player1KeybLayout.KeyAction = Key;
            } else if (currentMsg == GetInputMessage(2, "move left"))
            {
                GameConfig.UserPref.Player2KeybLayout.KeyLeft = Key;
            }
            else if (currentMsg == GetInputMessage(2, "move right"))
            {
                GameConfig.UserPref.Player2KeybLayout.KeyRight = Key;
            }
            else if (currentMsg == GetInputMessage(2, "move up"))
            {
                GameConfig.UserPref.Player2KeybLayout.KeyUp = Key;
            }
            else if (currentMsg == GetInputMessage(2, "move down"))
            {
                GameConfig.UserPref.Player2KeybLayout.KeyDown = Key;
            }
            else if (currentMsg == GetInputMessage(2, "hit"))
            {
                GameConfig.UserPref.Player2KeybLayout.KeyAction = Key;
            }
            GameConfig.SaveUserPrefs();
            SetMenuEntryText();
            choosingKey = false;
        }

        void SetMenuEntryText()
        {
            p1LeftMenuEntry.Text = "LEFT: " + GameConfig.UserPref.Player1KeybLayout.KeyLeft.ToString();
            p1RightMenuEntry.Text = "RIGHT: " + GameConfig.UserPref.Player1KeybLayout.KeyRight.ToString();
            p1UpMenuEntry.Text = "UP: " + GameConfig.UserPref.Player1KeybLayout.KeyUp.ToString();
            p1DownMenuEntry.Text = "DOWN: " + GameConfig.UserPref.Player1KeybLayout.KeyDown.ToString();
            p1ActionMenuEntry.Text = "ACTION: " + GameConfig.UserPref.Player1KeybLayout.KeyAction.ToString();
            p2LeftMenuEntry.Text = "LEFT: " + GameConfig.UserPref.Player2KeybLayout.KeyLeft.ToString();
            p2RightMenuEntry.Text = "RIGHT: " + GameConfig.UserPref.Player2KeybLayout.KeyRight.ToString();
            p2UpMenuEntry.Text = "UP: " + GameConfig.UserPref.Player2KeybLayout.KeyUp.ToString();
            p2DownMenuEntry.Text = "DOWN: " + GameConfig.UserPref.Player2KeybLayout.KeyDown.ToString();
            p2ActionMenuEntry.Text = "ACTION: " + GameConfig.UserPref.Player2KeybLayout.KeyAction.ToString();
        }

        private string GetInputMessage(int playerNb, string action)
        {
            return "Player " + playerNb + ": Enter new key to " + action;
        }

        
    }
}
