using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Coreblock;
using Microsoft.Xna.Framework;


public enum BiomeType
{
    None,
    Forest,
    Desert,
    Snow,
    Jungle,
    Ocean,
    Lava,
    Space
}



public class Biome {
    public Tiles.TileType TopTile { get; set; }
    public Tiles.TileType SurfaceTile { get; set; }
    public Tiles.TileType UndergroundTile { get; set; }
    public Tiles.TileType SecondaryTile { get; set; }
    public Tiles.TileType BackgroundTile { get; set; } = Tiles.TileType.DirtBackground;
    public float SecondaryTileScale { get; set; }

    public Vector2 position { get; set; }
    public float Radius { get; set; }
    public Vector2 Center { get; set; }
    public float VerticalStretch { get; set; } = 1.0f;

    public List<BiomeOreData> Ores = new List<BiomeOreData>();
    public BiomeType Type { get; set; }
}

public class BiomeOreData
{
    public Tiles.TileType tileType;
    public float chance;          // Chance entre 0 e 1
    public float scale;      // Escala do noise (0.01-0.1 para veios grandes, 0.1-0.5 para pequenos)
    public int minHeight;         // Altura mínima (em tiles)
    public int maxHeight;         // Altura máxima (em tiles)
    public float space;     // Tamanho dos aglomerados (1-5)

    public BiomeOreData(Tiles.TileType tileType, float chance, float noiseScale, int minHeight, int maxHeight, float space = 1f)
    {
        this.tileType = tileType;
        this.chance = Math.Clamp(chance, 0, 1);
        this.scale = noiseScale;
        this.minHeight = minHeight;
        this.maxHeight = maxHeight;
        this.space = space;
    }

    public bool IsInHeight(int y) => y >= minHeight && y <= maxHeight;
}

public static class BiomeDatabase {

    public static Dictionary<BiomeType, Biome> Biomes = new Dictionary<BiomeType, Biome>()
    {
        {
            BiomeType.Forest,
            new Biome()
            {
                Type = BiomeType.Forest,
                Radius = 150,

                TopTile = Tiles.TileType.Grass,
                SurfaceTile = Tiles.TileType.Dirt,
                UndergroundTile = Tiles.TileType.Stone,
                SecondaryTile = Tiles.TileType.Dirt,
                SecondaryTileScale = 2f,

                Ores = new List<BiomeOreData>
                {
                    new(Tiles.TileType.CoalOre,    0.7f, 0.15f, 10, 200, 0.5f),
                    new(Tiles.TileType.IronOre,    0.72f, 0.17f, 30, 100, 0.4f),
                    new(Tiles.TileType.SilverOre,    0.72f, 0.18f, 30, 50, 0.4f),
                    new(Tiles.TileType.CopperOre,    0.70f, 0.18f, 30, 200, 0.4f),
                    new(Tiles.TileType.GoldOre,    0.73f, 0.19f, 10, 50, 0.5f),
                    new(Tiles.TileType.DiamondOre, 0.75f, 0.22f, 80, 50, 0.5f)
                }
            }
        },
        {
            BiomeType.Desert,
            new Biome()
            {
                Type = BiomeType.Desert,
                Radius = 150,

                TopTile = Tiles.TileType.Sand,
                SurfaceTile = Tiles.TileType.Sand,
                UndergroundTile = Tiles.TileType.Sand,
                SecondaryTile = Tiles.TileType.Sand,
                BackgroundTile = Tiles.TileType.SandBackground,

                SecondaryTileScale = 2f,

                Ores = new List<BiomeOreData>
                {
                    new(Tiles.TileType.CoalOre,    0.74f, 0.15f, 10, 2000, 0.5f),
                    new(Tiles.TileType.IronOre,    0.72f, 0.17f, 30, 1800, 0.4f),
                    new(Tiles.TileType.SilverOre,    0.72f, 0.18f, 30, 1800, 0.4f),
                    new(Tiles.TileType.CopperOre,    0.68f, 0.18f, 30, 1800, 0.4f),
                    new(Tiles.TileType.GoldOre,    0.68f, 0.19f, 10, 1500, 0.5f),
                    new(Tiles.TileType.DiamondOre, 0.75f, 0.22f, 80, 1200, 0.5f)
                }

            }
        },
        {
            BiomeType.Snow, 
            new Biome()
            {
                Type = BiomeType.Snow,
                Radius = 150,

                TopTile = Tiles.TileType.Snow,
                SurfaceTile = Tiles.TileType.Snow,
                UndergroundTile = Tiles.TileType.Snow,
                SecondaryTile = Tiles.TileType.Snow,
                BackgroundTile = Tiles.TileType.SnowBackground,
                SecondaryTileScale = 2f,

                Ores = new List<BiomeOreData>
                {
                    new(Tiles.TileType.CoalOre,    0.74f, 0.15f, 10, 2000, 0.5f),
                    new(Tiles.TileType.IronOre,    0.72f, 0.17f, 30, 1800, 0.4f),
                    new(Tiles.TileType.SilverOre,    0.72f, 0.18f, 30, 1800, 0.4f),
                    new(Tiles.TileType.CopperOre,    0.68f, 0.18f, 30, 1800, 0.4f),
                    new(Tiles.TileType.GoldOre,    0.68f, 0.19f, 10, 1500, 0.5f),
                    new(Tiles.TileType.DiamondOre, 0.75f, 0.22f, 80, 1200, 0.5f)
                }

            }
        },
    };
}

