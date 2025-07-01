using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Coreblock
{
    public enum SpawnShape
    {
        Point,
        Circle,
        Ring,
        Rectangle,
        Line
    }

    public class ParticleSystem
    {
        private readonly List<Particle> particles = new();
        private Vector2 gravity = new Vector2(0, 98f);

        public Vector2 Gravity 
        { 
            get => gravity; 
            set => gravity = value; 
        }

        public int ParticleCount => particles.Count;

        public void AddParticle(Particle particle)
        {
            particles.Add(particle);
        }

        public void Clear()
        {
            particles.Clear();
        }

        public void Update(float deltaTime, LightManager lightManager)
        {
            for (int i = particles.Count - 1; i >= 0; i--)
            {
                particles[i].Update(deltaTime);
                
                // Gerenciamento de luz
                if (particles[i].EmitsLight)
                {
                    if (particles[i].IsAlive)
                    {
                        var lightData = particles[i].GetLightData();
                        lightManager.AddLightSource(lightData.uid, lightData.position, lightData.color, lightData.intensity);
                    }
                    else
                    {
                        //lightManager.RemoveLightSource(particles[i].LightUID);
                    }
                }
                
                if (!particles[i].IsAlive)
                {
                    particles.RemoveAt(i);
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var particle in particles)
                particle.Draw(spriteBatch);
        }

        public void SpawnBurst(Texture2D texture, Vector2 origin, SpawnShape shape,
                             int count, float minSpeed, float maxSpeed, float lifetime,
                             float minScale, float maxScale, float scaleDelta,
                             Color startColor, Color endColor,
                             float minRotationSpeed, float maxRotationSpeed,
                             float fadeInTime = 0.1f, float fadeOutTime = 0.3f,
                             float spreadAngle = MathHelper.TwoPi,
                             float shapeRadius = 10f,
                             Vector2 rectangleSize = default,
                             Vector2 lineDirection = default,
                             bool emitsLight = false, Color? lightColor = null,
                             float minLightIntensity = 0.3f, float maxLightIntensity = 0.7f,
                             float lightRadius = 1f)
        {
            for (int i = 0; i < count; i++)
            {
                // Posição baseada no formato de spawn
                Vector2 position = GetSpawnPosition(origin, shape, shapeRadius, rectangleSize, lineDirection);

                // Velocidade e direção
                float angle = spreadAngle * ((float)i / count - 0.5f);
                float speed = MathHelper.Lerp(minSpeed, maxSpeed, (float)Globals.Random.NextDouble());
                Vector2 velocity = speed * new Vector2((float)System.Math.Cos(angle), (float)System.Math.Sin(angle));

                // Configurações aleatórias
                float rotation = (float)Globals.Random.NextDouble() * MathHelper.TwoPi;
                float rotationSpeed = MathHelper.Lerp(minRotationSpeed, maxRotationSpeed, (float)Globals.Random.NextDouble());
                float scale = MathHelper.Lerp(minScale, maxScale, (float)Globals.Random.NextDouble());
                float lightIntensity = MathHelper.Lerp(minLightIntensity, maxLightIntensity, (float)Globals.Random.NextDouble());

                var p = new Particle(
                    texture,
                    position,
                    velocity,
                    Gravity,
                    rotation,
                    rotationSpeed,
                    scale,
                    scaleDelta,
                    lifetime,
                    startColor,
                    endColor,
                    fadeInTime,
                    fadeOutTime,
                    emitsLight,
                    lightColor,
                    lightIntensity,
                    lightIntensityDelta: -0.5f,
                    lightRadius: lightRadius
                );

                AddParticle(p);
            }
        }

        private Vector2 GetSpawnPosition(Vector2 origin, SpawnShape shape, float radius, 
                                        Vector2 rectangleSize, Vector2 lineDirection)
        {
            switch (shape)
            {
                case SpawnShape.Circle:
                    {
                        float r = radius * (float)System.Math.Sqrt(Globals.Random.NextDouble());
                        float theta = (float)Globals.Random.NextDouble() * MathHelper.TwoPi;
                        return origin + new Vector2(r * (float)System.Math.Cos(theta), r * (float)System.Math.Sin(theta));
                    }
                case SpawnShape.Ring:
                    {
                        float theta = (float)Globals.Random.NextDouble() * MathHelper.TwoPi;
                        return origin + new Vector2(radius * (float)System.Math.Cos(theta), radius * (float)System.Math.Sin(theta));
                    }
                case SpawnShape.Rectangle:
                    {
                        float x = (float)Globals.Random.NextDouble() * rectangleSize.X - rectangleSize.X / 2;
                        float y = (float)Globals.Random.NextDouble() * rectangleSize.Y - rectangleSize.Y / 2;
                        return origin + new Vector2(x, y);
                    }
                case SpawnShape.Line:
                    {
                        float t = (float)Globals.Random.NextDouble();
                        return origin + lineDirection * t;
                    }
                default: // SpawnShape.Point
                    return origin;
            }
        }
    }
}