using System;
using System.Collections.Generic;
using System.Numerics;
using Microsoft.Xna.Framework.Graphics;

namespace Coreblock
{
    public static class Tiles{
        public enum TileType {
            ar = 0,
            Stone = 1,
            Dirt = 2,
            Grass = 3,
            Sand = 4,
            DirtBackground = 5,
            CoalOre = 6,
            SilverOre = 7,
            CopperOre = 8,
            IronOre = 9,
            GoldOre = 10,
            DiamondOre = 11,
            Snow = 12,
            SnowBackground = 13, 
            SandBackground = 14, 
            Water = 15, 
            Lava = 16,
            Obsidian = 17 // Added Obsidian
        }
        
        public static bool IsOre(TileType type)
        {
            switch (type){
                
            case TileType.CoalOre:
            case TileType.SilverOre:
            case TileType.CopperOre:
            case TileType.IronOre:
            case TileType.GoldOre:
            case TileType.DiamondOre:
                return true;
            default:
                return false;
            }
        }

        
        public static TileType GetTileType(int id)
        {
            switch (id)
            {
                case 1:
                    return TileType.Stone;
                case 2:
                    return TileType.Dirt;
                case 3:
                    return TileType.Grass;
                case 4:
                    return TileType.Sand;
                case 5:
                    return TileType.DirtBackground;
                case 6:
                    return TileType.CoalOre;
                case 7:
                    return TileType.SilverOre;
                case 8:
                    return TileType.CopperOre;
                case 9:
                    return TileType.IronOre;
                case 10:
                    return TileType.GoldOre;
                case 11:
                    return TileType.DiamondOre;
                case 12:
                    return TileType.Snow;
                case 13:
                    return TileType.SnowBackground;
                case 14:
                    return TileType.SandBackground;
                case 15:
                    return TileType.Water;
                case 16:
                    return TileType.Lava;
                case 17:
                    return TileType.Obsidian; // Added Obsidian
                default:
                    return TileType.ar;
            }
        }

        public static int GetTileId(TileType type)
        {
            switch (type)
            {
                case TileType.Stone:
                    return 1;
                case TileType.Dirt:
                    return 2;
                case TileType.Grass:
                    return 3;
                case TileType.Sand:
                    return 4;
                case TileType.DirtBackground:
                    return 5;
                case TileType.CoalOre:
                    return 6;
                case TileType.SilverOre:
                    return 7;
                case TileType.CopperOre:
                    return 8;
                case TileType.IronOre:
                    return 9;
                case TileType.GoldOre:
                    return 10;
                case TileType.DiamondOre:
                    return 11;
                case TileType.Snow:
                    return 12;
                case TileType.SnowBackground:
                    return 13;
                case TileType.SandBackground:
                    return 14;
                case TileType.Water:
                    return 15;
                case TileType.Lava:
                    return 16;
                case TileType.Obsidian:
                    return 17; // Added Obsidian
                default:
                    return 0;
            }
        }

        public static Tile GetTile(TileType type)
        {
            switch (type)
            {
                case TileType.Stone:
                    return new Tile(1, 0, 0, 0, true, false);
                case TileType.Dirt:
                    return new Tile(2, 0, 0, 0, true, false);
                case TileType.Grass:
                    return new Tile(3, 0, 0, 0, true, false);
                case TileType.Sand:
                    return new Tile(4, 0, 0, 0, true, false);
                case TileType.DirtBackground:
                    return new Tile(5, 0, 0, 0, false, false);
                case TileType.CoalOre:
                    return new Tile(6, 0, 0, 0, true, false);
                case TileType.SilverOre:
                    return new Tile(7, 0, 0, 0, true, false);
                case TileType.CopperOre:
                    return new Tile(8, 0, 0, 0, true, false);
                case TileType.IronOre:
                    return new Tile(9, 0, 0, 0, true, false);
                case TileType.GoldOre:
                    return new Tile(10, 0, 0, 0, true, false);
                case TileType.DiamondOre:
                    return new Tile(11, 0, 0, 0, true, false);
                case TileType.Snow:
                    return new Tile(12, 0, 0, 0, true, false);
                case TileType.SnowBackground:
                    return new Tile(13, 0, 0, 0, false, false);
                case TileType.SandBackground:
                    return new Tile(14, 0, 0, 0, false, false);
                case TileType.Water:
                    return new Tile(15, 0, 0, 0, false, false, liquidType: LiquidType.Water);
                case TileType.Lava:
                    return new Tile(16, 0, 0, 0, false, false, liquidType: LiquidType.Lava);
                case TileType.Obsidian:
                    return new Tile(17, 0, 0, 0, true, false); // Added Obsidian
                default:
                    return new Tile(0, 0, 0, 0, false, false);
            }
        }

    }
    public class TileConfig{
        
        
        public Texture2D GetTexture(Tiles.TileType type)
        {
            switch (type)
            {
                case Tiles.TileType.Stone:
                    return Globals.Content.Load<Texture2D>("Textures/Tiles/Stone");
                case Tiles.TileType.Dirt:
                    return Globals.Content.Load<Texture2D>("Textures/Tiles/Dirt");
                case Tiles.TileType.Grass:
                    return Globals.Content.Load<Texture2D>("Textures/Tiles/Grass");
                case Tiles.TileType.Sand:
                    return Globals.Content.Load<Texture2D>("Textures/Tiles/Sand");
                case Tiles.TileType.DirtBackground:
                    return Globals.Content.Load<Texture2D>("Textures/Tiles/Dirt_background");
                case Tiles.TileType.CoalOre:
                    return Globals.Content.Load<Texture2D>("Textures/Tiles/Coal");
                case Tiles.TileType.SilverOre:
                    return Globals.Content.Load<Texture2D>("Textures/Tiles/Silver");
                case Tiles.TileType.CopperOre:
                    return Globals.Content.Load<Texture2D>("Textures/Tiles/Copper");
                case Tiles.TileType.IronOre:
                    return Globals.Content.Load<Texture2D>("Textures/Tiles/Iron");
                case Tiles.TileType.GoldOre:
                    return Globals.Content.Load<Texture2D>("Textures/Tiles/Gold");
                case Tiles.TileType.DiamondOre:
                    return Globals.Content.Load<Texture2D>("Textures/Tiles/Diamond");
                case Tiles.TileType.Snow:
                    return Globals.Content.Load<Texture2D>("Textures/Tiles/Snow");
                case Tiles.TileType.SnowBackground:
                    return Globals.Content.Load<Texture2D>("Textures/Tiles/Snow_background");
                case Tiles.TileType.SandBackground:
                    return Globals.Content.Load<Texture2D>("Textures/Tiles/Sand_background");
                case Tiles.TileType.Water:
                    return Globals.Content.Load<Texture2D>("Textures/Tiles/Water");
                case Tiles.TileType.Lava:
                    return Globals.Content.Load<Texture2D>("Textures/Tiles/Lava");
                case Tiles.TileType.Obsidian:
                    return Globals.Content.Load<Texture2D>("Textures/Tiles/Obsidian"); // Added Obsidian
                default:
                    return null;
            }
        }

        public Vector2 GetCordLiquidTexture(Tile[,,] tiles, int x, int y, int level, int variant = 0)
        {

            int Width = tiles.GetLength(0);
            int Height = tiles.GetLength(1);


            List<Vector2> cords = new List<Vector2>();


            int top = y - 1;

            bool topTile = top >= 0 && tiles[x, top, 1].LiquidType != LiquidType.None;

            // 🔲 1 direção
            if (topTile) return TileCoords.Level9[variant];
            

            return TileCoords.GetCoordLiquid(level, variant);
        }



        public Vector2 GetCordTexture(Tile[,,] tiles, int x, int y, int layer, int variant = 0)
        {

            int Width = tiles.GetLength(0);
            int Height = tiles.GetLength(1);


            List<Vector2> cords = new List<Vector2>();


            int top = y - 1;
            int bottom = y + 1;
            int left = x - 1;
            int right = x + 1;

            bool topTile = top >= 0 && tiles[x, top, layer].id != 0 && tiles[x, top, layer].LiquidType == LiquidType.None;
            bool bottomTile = bottom < Height && tiles[x, bottom, layer].id != 0 && tiles[x, bottom, layer].LiquidType == LiquidType.None;
            bool leftTile = left >= 0 && tiles[left, y, layer].id != 0 && tiles[left, y, layer].LiquidType == LiquidType.None;
            bool rightTile = right < Width && tiles[right, y, layer].id != 0 && tiles[right, y, layer].LiquidType == LiquidType.None;

            bool topTileDirt = top >= 0 && (tiles[x, top, layer].TileType == Tiles.TileType.Grass || tiles[x, top, layer].TileType == Tiles.TileType.Dirt);
            bool bottomTileDirt = bottom < Height && (tiles[x, bottom, layer].TileType == Tiles.TileType.Grass || tiles[x, bottom, layer].TileType == Tiles.TileType.Dirt);
            bool leftTileDirt = left >= 0 && (tiles[left, y, layer].TileType == Tiles.TileType.Grass || tiles[left, y, layer].TileType == Tiles.TileType.Dirt);
            bool rightTileDirt = right < Width && (tiles[right, y, layer].TileType == Tiles.TileType.Grass || tiles[right, y, layer].TileType == Tiles.TileType.Dirt);

            bool topLeft = topTile && leftTile;
            bool topRight = topTile && rightTile;
            bool bottomLeft = bottomTile && leftTile;
            bool bottomRight = bottomTile && rightTile;

            bool topBottom = topTile && bottomTile;
            bool leftRight = leftTile && rightTile;

            bool topLeftRight = topTile && leftTile && rightTile;
            bool topBottomLeft = topTile && bottomTile && leftTile;
            bool topBottomRight = topTile && bottomTile && rightTile;
            bool bottomLeftRight = bottomTile && leftTile && rightTile;

            bool all = topTile && bottomTile && leftTile && rightTile;

            // 🔲 Todas as direções preenchidas
            if (all) return TileCoords.getCoord(TileCoords.TileCoordsType.All, variant);

            // 🔲 3 direções
            if (topLeftRight) return TileCoords.getCoord(TileCoords.TileCoordsType.Bottom, variant);
            if (topBottomLeft) return TileCoords.getCoord(TileCoords.TileCoordsType.Right, variant);
            if (topBottomRight) return TileCoords.getCoord(TileCoords.TileCoordsType.Left, variant);
            if (bottomLeftRight) return TileCoords.getCoord(TileCoords.TileCoordsType.Top, variant);

            // 🔲 2 direções opostas
            if (topBottom) return TileCoords.getCoord(TileCoords.TileCoordsType.LeftRight, variant);
            if (leftRight) return TileCoords.getCoord(TileCoords.TileCoordsType.TopBottom, variant);

            // 🔲 2 direções em L
            if (topTile && leftTile) return TileCoords.getCoord(TileCoords.TileCoordsType.TopLeft, variant);
            if (topTile && rightTile) return TileCoords.getCoord(TileCoords.TileCoordsType.TopRight, variant);
            if (bottomTile && leftTile) return TileCoords.getCoord(TileCoords.TileCoordsType.BottomLeft, variant);
            if (bottomTile && rightTile) return TileCoords.getCoord(TileCoords.TileCoordsType.BottomRight, variant);

            // 🔲 1 direção
            if (topTile) return TileCoords.getCoord(TileCoords.TileCoordsType.TopLeftRight, variant);
            if (bottomTile) return TileCoords.getCoord(TileCoords.TileCoordsType.LeftBottomRight, variant);
            if (leftTile) return TileCoords.getCoord(TileCoords.TileCoordsType.TopLeftBottom, variant);
            if (rightTile) return TileCoords.getCoord(TileCoords.TileCoordsType.TopBottomRight, variant);

            // 🔲 Sem conexões
            return TileCoords.getCoord(TileCoords.TileCoordsType.None, variant);
        }

        internal int GetTileId(Tiles.TileType grass)
        {
            throw new NotImplementedException();
        }
    }
}