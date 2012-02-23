﻿using System;
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
            this.playerPosition = playerPosition;
            Sprite = new Character(game, "elroy-" + name, playerPosition, powerTex);
            Sprite.ScreenPosition = playerPosition;

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
                Sprite.HandleKeys(keyboard, kb);
            }
            else // simple IA for now
            {
                if ((ball.Speed.Y < 0 && playerPosition == Character.PlayerPosition.Bottom) ||
                    (ball.Speed.Y > 0 && playerPosition == Character.PlayerPosition.Top))
                {
                    // the ball is leaving, go back to middle
                    Rectangle safeZone = new Rectangle(Sprite.Bounds.Width / 2 - 10, Sprite.Bounds.Y, 20, Sprite.Bounds.Height);
                    if (!Sprite.Rect.Intersects(safeZone))
                    {
                        float accel = Sprite.Rect.X < safeZone.X ? 0.1f : -0.1f;
                        Sprite.Acceleration = new Vector2(accel, 0f);
                    }

                }
                else // ball is coming back towards us, go meet it
                {
                    Sprite.Acceleration = new Vector2(-1, -1);
                }

            }
        }

    }
}
