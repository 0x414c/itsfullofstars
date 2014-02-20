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


        public ParticleEngine(List<Texture2D> textures, Vector2 location) {
            EmitterLocation = location;
            this.textures = textures;
            particles = new List<Particle>();
            random = new Random();
        }

        public void Update() {
            const int total = 6;
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
                (float) random.NextDouble() * 16.0f - 16,
                (float) random.NextDouble() * 10.0f - 10);
            var velocity = new Vector2(
                                    2.0f * (float) (random.NextDouble() * 2.0f - 1),
                                    2.0f * (float) (random.NextDouble() * 2.0f - 1));
            const float angle = 0;
            float angularVelocity = 0.1f * (float) (random.NextDouble() * 2 - 1);
            //float angularVelocity = -1.0f * (float) Math.Atan2(velocity.X, velocity.Y);
            var color = new Color(
                random.Next(128, 255),
                random.Next(128, 255),
                random.Next(128, 255),
                255);
            const float fadeValue = -0.1065f;
            float fadeDelta = 0.0015f * (float) random.NextDouble();
            float size = (float) random.NextDouble() * 0.225f;
            const float sizeDelta = -0.00025f;
            int ttl = 600 + random.Next(48);

            return new Particle(texture, position, velocity, angle, angularVelocity, color, size, ttl, fadeValue, fadeDelta, sizeDelta);
        }

        public void Draw(SpriteBatch spriteBatch) {
            spriteBatch.Begin();
            foreach (Particle t in particles) {
                t.Draw(spriteBatch);
            }
            spriteBatch.End();
        }
    }
}