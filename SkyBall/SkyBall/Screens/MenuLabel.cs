using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace SkyBall
{
    class MenuLabel
    {
        public string Text{get;set;}
        public Vector2 Position {get;set;}
        public Color Color {get;set;}
        public MenuLabel(string text, Color color)
        {
            Text = text;
            Color = color;
        }
        public MenuLabel(string text)
        {
            Text = text;
        }

        public virtual void Update(MenuScreen screen, GameTime gameTime)
        {
        }
        
        public virtual void Draw(MenuScreen screen, GameTime gameTime)
        {
            float scale = 1f ;

            Color color = Color;

            // Modify the alpha to fade text out during transitions.
            color *= screen.TransitionAlpha;

            // Draw text, centered on the middle of each line.
            ScreenManager screenManager = screen.ScreenManager;
            SpriteBatch spriteBatch = screenManager.SpriteBatch;
            SpriteFont font = screenManager.Font;

            Vector2 origin = new Vector2(0, font.LineSpacing / 2);

            spriteBatch.DrawString(font, Text, Position, Color, 0,
                                   origin, scale, SpriteEffects.None, 0);
        }

        public virtual int GetHeight(MenuScreen screen)
        {
            return screen.ScreenManager.Font.LineSpacing;
        }

        public virtual int GetWidth(MenuScreen screen)
        {
            return (int)screen.ScreenManager.Font.MeasureString(Text).X;
        }
        
    }
}
