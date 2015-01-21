using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WindowsGame1 {
    public class Particle {
        private Texture2D Texture { get; set; }      // The texture that will be drawn to represent the particle
        private Vector2 Position { get; set; }       // The current position of the particle        
        private Vector2 Velocity { get; set; }       // The speed of the particle at the current instance
        private float Angle { get; set; }            // The current angle of rotation of the particle
        private float AngularVelocity { get; set; }  // The speed that the angle is changing
        private Color Color { get; set; }            // The color of the particle
        private float Size { get; set; }             // The size of the particle
        public int TTL { get; private set; }         // The 'time to live' of the particle
        private float FadeValue { get; set; }        // The fading of the particle
        private float FadeRate { get; set; }         // The fading rate of the particle, they can appear/dissappear smoothly with this
        private float SizeRate { get; set; }         // The size rate of the particle to make them bigger/smaller over time
    
        public Particle(Texture2D texture, Vector2 position, Vector2 velocity,
            float angle, float angularVelocity, Color color, float size,
            int ttl, float fadeValue, float fadeRate, float sizeRate) {
                Texture = texture;
                Position = position;
                Velocity = velocity;
                Angle = angle;
                AngularVelocity = angularVelocity;
                Color = color;
                Size = size;
                TTL = ttl;
                FadeValue = fadeValue;
                FadeRate = fadeRate;
                SizeRate = sizeRate;
        }

        public void Update() {
            TTL--;
            FadeValue += FadeRate;
            Position += Velocity;
            Angle += AngularVelocity;
            Size += SizeRate;
        }

        public void Draw(SpriteBatch spriteBatch) {
            var sourceRectangle = new Rectangle(
                0, 0,
                Texture.Width, Texture.Height
            );
            var origin = new Vector2(Texture.Width / 2, Texture.Height / 2);

            spriteBatch.Draw(
                Texture, Position, sourceRectangle, Color * FadeValue,
                Angle, origin, Size, SpriteEffects.None, 0f
            );
        }
    }
}
