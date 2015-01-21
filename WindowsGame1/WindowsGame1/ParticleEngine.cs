using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace WindowsGame1 {
    public class ParticleEngine {
        private readonly Random random;
        public Vector2 EmitterLocation { private get; set; }
        private readonly List<Particle> particles;
        private readonly List<Texture2D> textures;

        private const float linearVelocityFactor = 6.0f;
        private const float angularVelocityFactor = 4.0f;
        private const float sizeFactor = 0.3f;
        private const int colorLowerBound = 192;
        private const int colorUpperBound = 255;


        public ParticleEngine(List<Texture2D> textures, Vector2 location) {
            EmitterLocation = location;
            this.textures = textures;
            particles = new List<Particle>();
            random = new Random();
        }

        public void Update() {
            const int total = 8;
            for (int i = 0; i < total; i++) {
                particles.Add(GenerateNewParticle());
            }

            for (int particle = 0; particle < particles.Count; particle++) {
                particles[particle].Update();
                if (particles[particle].TTL <= 0) {
                    particles.RemoveAt(particle);
                    particle--;
                }
            }
        }

        private Particle GenerateNewParticle() {
            Texture2D texture = textures[random.Next(textures.Count)];
            
            Vector2 position = EmitterLocation + new Vector2(
                (float) (random.NextDouble() * 16.0f - 16.0f),
                (float) (random.NextDouble() * 10.0f - 10.0f)
            );
            Vector2 velocity = new Vector2(
                linearVelocityFactor * ((float) (random.NextDouble() - 0.5f)),
                linearVelocityFactor * ((float) (random.NextDouble() - 0.5f))
            );
            
            const float angle = 0;
            float angularVelocity = angularVelocityFactor * ((float) (random.NextDouble() - 0.5f));
            //float angularVelocity = -1.0f * (float) Math.Atan2(velocity.X, velocity.Y);
            var color = new Color(
                random.Next(colorLowerBound, colorUpperBound),
                random.Next(colorLowerBound, colorUpperBound),
                random.Next(colorLowerBound, colorUpperBound),
                255
            );
            const float fade = -1.0f;
            float fadeDelta = 0.01f;
            float size = sizeFactor * ((float) random.NextDouble() - 0.5f);
            const float sizeDelta = 0.0001f;
            int ttl = 600 + random.Next(64);

            return new Particle(
                texture, position, velocity, angle, angularVelocity,
                color, size, ttl, fade, fadeDelta, sizeDelta
            );
        }

        public void Draw(SpriteBatch spriteBatch) {
            spriteBatch.Begin();
            foreach (Particle p in particles) {
                p.Draw(spriteBatch);
            }
            spriteBatch.End();
        }
    }
}