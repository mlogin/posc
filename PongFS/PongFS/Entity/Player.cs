using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PongFS.Config;
using PongFS.Drawable;
using PongFS.Core;

namespace PongFS.Entity
{
    public class Player
    {
        private string name;
        public List<Wall> Walls { get; set; }
        public Character Sprite { get; set; }
        public int Score { get; set; }
        private KeyboardLayout kb;
        private bool isIA = false;
        private int iaLevel;
        private Game game;
        private Character.PlayerPosition playerPosition;

        public Player(Game game, string name, Character.PlayerPosition playerPosition, string powerTex)
        {
            this.game = game;
            Walls = new List<Wall>();
            isIA = true;
            this.name = name;
            ComponentFactory.getFactory().Add(name, this);
            this.playerPosition = playerPosition;
            Sprite = new Character(game, "player-" + name, playerPosition, powerTex);
            Sprite.ScreenPosition = playerPosition;
            Sprite.IA = true;

            for (var i = 0; i < 8; i++)
            {
                Wall wall = new Wall(game, "wall-" + name + "-" + i.ToString());
                wall.ScreenPosition = playerPosition;
                wall.InitialPosition = new Vector2(i * 62 + Engine.WIN_BORDER, playerPosition == Character.PlayerPosition.Top ? 0 : Engine.HEIGHT - 20);
                Walls.Add(wall);
            }
        }
        
        public Player(Game game, KeyboardLayout layout, string name, Character.PlayerPosition playerPosition, string powerTex) : 
            this(game, name, playerPosition, powerTex)
        {
            kb = layout;
            isIA = false;
            Sprite.IA = false;
        }

        private void SetIALevel(int difficulty)
        {
            isIA = true;
            iaLevel = difficulty;
        }

        internal void GrantPower(Power power)
        {
            switch (power.type)
            {
                case Power.PowerType.HealSingle:
                    Walls.Sort(delegate(Wall a, Wall b) { return a.Life.CompareTo(b.Life); });
                    Walls[0].Heal();
                    SoundFactory.getFactory().Play("shields", true);
                    break;
                case Power.PowerType.HealAll:
                    foreach (Wall wall in Walls)
                    {
                        wall.Heal();
                    }
                    SoundFactory.getFactory().Play("shields", true);
                    break;
                case Power.PowerType.Wall:
                    CreateNewWallAt(Sprite.Position);
                    SoundFactory.getFactory().Play("shields", true);
                    break;
                case Power.PowerType.Fortress:
                    for (var i = 0; i < 8; i++)
                    {
                        CreateNewWallAt(new Vector2(i * 62 + Engine.WIN_BORDER, playerPosition == Character.PlayerPosition.Top ? 20 : Engine.HEIGHT - 40));
                    }
                    SoundFactory.getFactory().Play("shields", true);
                    break;
                default:
                    SoundFactory.getFactory().Play("powerup", true);
                    Sprite.CurrentPower = power;
                    break;
                    

            }
        }

        private void CreateNewWallAt(Vector2 position)
        {
            Wall newWall = new Wall(game, "wall-" + ComponentFactory.getFactory().NewId());
            newWall.ScreenPosition = playerPosition;
            newWall.InitialPosition = position;
            newWall.Position = position;
            newWall.Initialize();
            newWall.LoadGraphics(Sprite.SpriteBatch);
            newWall.IsBonus = true;
            Walls.Add(newWall);
        }

        // PAss in the ball for IA
        public void HandleKeys(Microsoft.Xna.Framework.Input.KeyboardState keyboard)
        {
            if (!isIA)
            {
                Sprite.HandleKeys(keyboard, kb);
            }
            else // simple IA for now
            {
                Sprite.HandleIA();
            }
        }

        public void SetOpponent(Player player)
        {
            Sprite.Opponent = player.Sprite;
        }
    }
}
