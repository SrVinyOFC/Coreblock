using System;
using Microsoft.Xna.Framework;

namespace Coreblock
{
    public static class IslandGenerator
    {
        public static float GenerateIsland(float x, float y, int octaves, float noiseScale, float islandRadius, float seed)
        {
            // Calcula a distância do ponto ao centro da ilha
            float centerX = islandRadius;
            float centerY = islandRadius;
            float dx = x - centerX;
            float dy = y - centerY;
            float distance = MathF.Sqrt(dx * dx + dy * dy);

            // Falloff mais suave com curva exponencial
            float falloff = 1.0f - SmoothStep(0, islandRadius, distance);

            // Se estiver completamente fora da ilha, retorna 0
            if (falloff <= 0) return 0f;

            // Gera o noise com offset baseado na seed
            float noise = GenerateOctaveNoise(x, y, octaves, noiseScale, seed);

            // Combina noise com falloff
            return MathHelper.Clamp(noise * falloff, 0f, 1f);
        }

        private static float SmoothStep(float edge0, float edge1, float x)
        {
            // Scale, bias and saturate x to 0..1 range
            x = MathHelper.Clamp((x - edge0) / (edge1 - edge0), 0.0f, 1.0f);
            // Evaluate polynomial
            return x * x * (3 - 2 * x);
        }

        public static float GenerateOrganicNoise(int x, int y, int seed, int octaves, float persistence)
        {
            float total = 0;
            float frequency = 1;
            float amplitude = 1;
            float maxValue = 0;
            
            for (int i = 0; i < octaves; i++)
            {
                total += NoiseHelper.Noise(
                    (x + seed) * frequency * 0.01f, 
                    (y + seed) * frequency * 0.01f) * amplitude;
                
                maxValue += amplitude;
                amplitude *= persistence;
                frequency *= 2;
            }
            
            return (total / maxValue + 1) * 0.5f; // Normaliza para 0-1
        }

        private static float GenerateOctaveNoise(float x, float y, int octaves, float noiseScale, float seed)
        {
            float total = 0f;
            float frequency = 1f;
            float amplitude = 1f;
            float maxValue = 0f;

            float offsetX = seed * 1000f;
            float offsetY = (seed + 0.5f) * 1000f;

            for (int i = 0; i < octaves; i++)
            {
                float nx = (x + offsetX) * frequency / noiseScale;
                float ny = (y + offsetY) * frequency / noiseScale;

                total += NoiseHelper.Noise(nx, ny) * amplitude;
                maxValue += amplitude;

                frequency *= 2f;
                amplitude *= 0.5f;
            }

            return (total / maxValue + 1f) * 0.5f;
        }
    }
}