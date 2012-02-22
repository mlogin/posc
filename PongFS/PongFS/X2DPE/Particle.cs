using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace X2DPE
{
	public class Particle
	{
		public Texture2D Texture { get; set; }
		public Vector2 Position { get; set; }
		public float Speed { get; set; }
		public float Direction { get; set; }
		public float TotalLifetime { get; set; }
		public float Rotation { get; set; }
		public float RotationSpeed { get; set; }
		public Vector2 Center { get; set; }
		public Color Color { get; set; }
		public int Fade { get; set; }
		public float Scale { get; set; }
		public int InitialOpacity { get; private set; }

		public Particle(Texture2D texture, Vector2 position, float speed, float direction, float rotation, float rotationSpeed, int opacity)
		{
			Texture = texture;
			Position = position;
			Speed = speed;
			Direction = direction;
			TotalLifetime = 0;
			Rotation = rotation;
			RotationSpeed = rotationSpeed;
			Center = new Vector2((float)texture.Width / 2, (float)texture.Height / 2);
			Scale = 1.0f;
			Fade = opacity;
			InitialOpacity = opacity;
			Color = new Color(Fade, Fade, Fade, Fade);
		}
	}
}
