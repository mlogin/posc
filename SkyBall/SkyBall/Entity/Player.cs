using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SkyBall.Config;
using SkyBall.Drawable;
using SkyBall.Core;

namespace SkyBall.Entity
{
    public class Player
    {
        public enum Side { Up, Down }
        public string Id { get; private set; }
        private List<Wall> walls = new List<Wall>();
        public Character Sprite { get; private set; }
        private KeyboardLayout kb;
        public bool IsNPC{get; set;}
        public Side Placement{get; private set;}
        public Player Opponent { get; set; }
        public event EventHandler PlayerLost;

        public Player(string id, Side placement, KeyboardLayout layout = null)
        {
            Id = id;
            if (layout == null)
            {
                IsNPC = true;
            }
            else
            {
                kb = layout;
            }
            Placement = placement;
            InitializeWalls();
            Sprite = new Character(this, TextureFactory.getFactory().Get("player"));
        }

        private void InitializeWalls()
        {
            for (var i = 0; i < 8; i++)
            {
                Vector2 position = Vector2.Zero;

                if (Placement == Side.Down)
                {
                    position = new Vector2(i * 62 + GameConfig.WIN_BORDER, GameConfig.HEIGHT - 20);
                }
                else
                {
                    position = new Vector2(i * 62 + GameConfig.WIN_BORDER, 0);
                }
                Wall wall = CreateNewWallAt(position, false);
            }
        }

        private Wall CreateNewWallAt(Vector2 position, bool isExtra)
        {
            Wall newWall = new Wall(position);
            newWall.IsBonus = isExtra;
            if (!isExtra)
            {
                newWall.OnDestroyed += new EventHandler(newWall_OnDestroyed);
            }
            walls.Add(newWall);
            return newWall;
        }

        void newWall_OnDestroyed(object sender, EventArgs e)
        {
            if (PlayerLost != null) PlayerLost(this, null);
        }

        internal void GrantPower(Power power)
        {
            switch (power.type)
            {
                case Power.PowerType.HealSingle:
                    walls.Sort(delegate(Wall a, Wall b) { return a.Life.CompareTo(b.Life); });
                    walls[0].Heal();
                    SoundFactory.getFactory().PlaySound("shields");
                    break;
                case Power.PowerType.HealAll:
                    foreach (Wall wall in walls)
                    {
                        wall.Heal();
                    }
                    SoundFactory.getFactory().PlaySound("shields");
                    break;
                case Power.PowerType.Wall:
                    CreateNewWallAt(Sprite.Position, true);
                    SoundFactory.getFactory().PlaySound("shields");
                    break;
                case Power.PowerType.Fortress:
                    for (var i = 0; i < 8; i++)
                    {
                        CreateNewWallAt(new Vector2(i * 62 + GameConfig.WIN_BORDER, Placement == Side.Up ? 20 : GameConfig.HEIGHT - 40), true);
                    }
                    SoundFactory.getFactory().PlaySound("shields");
                    break;
                default:
                    SoundFactory.getFactory().PlaySound("powerup");
                    Sprite.CurrentPower = power;
                    break;
                    

            }
        }
        
        public void HandleKeys(Microsoft.Xna.Framework.Input.KeyboardState keyboard)
        {
            if (IsNPC)
            {
                Sprite.HandleIA();
            }
            else // simple IA for now
            {
                Sprite.HandleKeys(keyboard, kb);
            }
        }
        
        public void Update(GameTime gameTime)
        {
            foreach (Wall wall in walls) wall.Update(gameTime);
            Sprite.Update(gameTime);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (Wall wall in walls) wall.Draw(gameTime, spriteBatch);
            Sprite.Draw(gameTime, spriteBatch);
        }
    }
}
