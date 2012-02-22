using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PongFS.Config;
using PongFS.Drawable;

namespace PongFS.Entity
{
    public class Player
    {
        private static int nbPlayers;
        private string name;
        public List<Wall> Walls { get; set; }
        public Character Sprite { get; set; }
        public int Score { get; set; }
        private KeyboardLayout kb;
        private bool isIA = false;
        private int iaLevel;
        private Game game;
        private Character.PlayerPosition playerPosition;

        public Player(Game game)
        {
            this.game = game;
            Walls = new List<Wall>();
            isIA = true;
            Player.nbPlayers++;
            name = "elroy-" + Player.nbPlayers.ToString();
            if (Player.nbPlayers == 1)
            {
                playerPosition = Character.PlayerPosition.Top;
            }
            else
            {
                playerPosition = Character.PlayerPosition.Bottom;
            }
            Sprite = new Character(game, name);
            Sprite.ScreenPosition = playerPosition;

            for (var i = 0; i < 8; i++)
            {
                Wall wall = new Wall(game, "wall-" + Player.nbPlayers.ToString() + i.ToString());
                wall.ScreenPosition = playerPosition;
                wall.InitialPosition = new Vector2(i * 62 + Engine.WIN_BORDER, playerPosition == Character.PlayerPosition.Top ? 0 : Engine.HEIGHT - 20);
                wall.ModColor = Color.Red;
                Walls.Add(wall);
            }
        }
        
        public Player(Game game, KeyboardLayout layout) : this(game)
        {
            kb = layout;
            isIA = false;
        }

        private void SetIALevel(int difficulty)
        {
            isIA = true;
            iaLevel = difficulty;
        }

        // PAss in the ball for IA
        public void HandleKeys(Microsoft.Xna.Framework.Input.KeyboardState keyboard, Ball ball)
        {
            if (!isIA)
            {
                Sprite.HandleKeys(keyboard, kb, ball);
            }
            else // simple IA for now
            {
                if (ball.Position.X < Sprite.Rect.Center.X)
                {
                    //sprite.AccelerateWest();
                }
                else if (ball.Position.X > Sprite.Rect.Center.X)
                {
                    //sprite.AccelerateEast();
                }

            }
        }

    }
}
