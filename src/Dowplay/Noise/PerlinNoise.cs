using System;

namespace Coreblock
{
    public static class PerlinNoise
    {
        // Tabela de permutação
        private static int[] p;

        // Inicializa permutação uma única vez
        static PerlinNoise()
        {
            p = new int[512];
            int[] permutation = new int[256];
            for (int i = 0; i < 256; i++) permutation[i] = i;

            Random rand = new Random(1337); // Permutação padrão, não afeta o seed da noise
            for (int i = 255; i > 0; i--)
            {
                int j = rand.Next(i + 1);
                (permutation[i], permutation[j]) = (permutation[j], permutation[i]);
            }

            for (int i = 0; i < 512; i++) p[i] = permutation[i % 256];
        }

        public static float Value(float x, float y, int seed, float scale = 0.01f,
                                  int octaves = 4, float persistence = 0.5f,
                                  float lacunarity = 2.0f, float warpIntensity = 0f)
        {
            if (octaves <= 1)
                return RawNoise(x * scale, y * scale, seed);

            return FractalNoise(x, y, seed, scale, octaves, persistence, lacunarity, warpIntensity);
        }

        public static float NormalizedValue(float x, float y, int seed, float scale = 0.01f,
                                            int octaves = 4, float persistence = 0.5f,
                                            float lacunarity = 2.0f, float warpIntensity = 0f)
        {
            return (Value(x, y, seed, scale, octaves, persistence, lacunarity, warpIntensity) + 1f) * 0.5f;
        }

        private static float RawNoise(float x, float y, int seed)
        {
            int X = (FastFloor(x) + seed) & 255;
            int Y = (FastFloor(y) + seed) & 255;

            x -= FastFloor(x);
            y -= FastFloor(y);

            float u = Fade(x);
            float v = Fade(y);

            int A = p[X] + Y;
            int B = p[X + 1] + Y;

            float res = Lerp(v,
                Lerp(u, Grad(p[A], x, y), Grad(p[B], x - 1, y)),
                Lerp(u, Grad(p[A + 1], x, y - 1), Grad(p[B + 1], x - 1, y - 1)));

            return res;
        }

        private static float FractalNoise(float x, float y, int seed, float scale,
                                          int octaves, float persistence, float lacunarity, float warpIntensity)
        {
            float total = 0f;
            float amplitude = 1f;
            float frequency = scale;
            float maxAmplitude = 0f;

            for (int i = 0; i < octaves; i++)
            {
                float nx = x * frequency;
                float ny = y * frequency;

                if (warpIntensity > 0)
                {
                    float warpX = RawNoise(nx + 1000, ny + 1000, seed) * warpIntensity;
                    float warpY = RawNoise(nx - 1000, ny - 1000, seed) * warpIntensity;
                    nx += warpX;
                    ny += warpY;
                }

                float noise = RawNoise(nx, ny, seed);
                total += noise * amplitude;

                maxAmplitude += amplitude;
                amplitude *= persistence;
                frequency *= lacunarity;
            }

            return total / maxAmplitude;
        }

        private static int FastFloor(float x) => x >= 0 ? (int)x : (int)x - 1;

        private static float Fade(float t)
        {
            return t * t * t * (t * (t * 6f - 15f) + 10f);
        }

        private static float Lerp(float t, float a, float b)
        {
            return a + t * (b - a);
        }

        private static float Grad(int hash, float x, float y)
        {
            int h = hash & 7; // Usa só os 3 últimos bits
            float u = h < 4 ? x : y;
            float v = h < 4 ? y : x;

            return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v);
        }
    }
}
