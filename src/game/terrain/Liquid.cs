using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;

namespace Coreblock
{
    public enum LiquidType
    {
        None,
        Water,
        Lava
    }

    public class Liquid
    {
        public LiquidType Type { get; private set; }
        public Tiles.TileType TileType { get; private set; }

        private Terrain terrain;
        private Random random;
        private readonly object _liquidLock = new object();
        private HashSet<Point> activeLiquidTiles = new HashSet<Point>();

        public Liquid(LiquidType type, Tiles.TileType tileType = Tiles.TileType.Water, int delay = 16)
        {
            Type = type;
            TileType = tileType;
            terrain = GameManager.terrain;
            random = new Random();
            _ = Update(delay);
        }

        public void AddLiquid(int x, int y, int amount = 2)
        {
            if (!IsValidTile(x, y)) return;

            lock (GameManager.terrain.chunkLock)
            {
                ref Tile tile = ref terrain.worldData[x, y, 1];

                tile.LiquidType = Type;
                tile.TileType = TileType;
                tile.id = Tiles.GetTileId(TileType);
                tile.LiquidLevel = (byte)Math.Clamp(tile.LiquidLevel + amount, 0, 8);

                RegisterLiquid(x, y);
            }
        }

        private async Task Update(int delay = 16)
        {
            await Task.Run(async () =>
            {
                while (true)
                {
                    List<Point> tilesToProcess;
                    lock (_liquidLock)
                    {
                        tilesToProcess = new List<Point>(activeLiquidTiles);
                        activeLiquidTiles.Clear();
                    }

                    foreach (var point in tilesToProcess)
                    {
                        bool stillActive = false;
                        lock (GameManager.terrain.chunkLock)
                        {
                            stillActive = ProcessLiquidTile(point.X, point.Y);
                        }

                        if (stillActive)
                        {
                            RegisterLiquid(point.X, point.Y);
                        }
                    }

                    await Task.Delay(delay);
                }
            });
        }

        private bool ProcessLiquidTile(int x, int y)
        {
            ref Tile currentTile = ref terrain.worldData[x, y, 1];

            if (currentTile.LiquidType != Type || currentTile.LiquidLevel <= 0)
                return false;

             // NOVO: verificação de contato entre líquidos
            if (CheckForLiquidInteraction(x, y))
            {
                // Se ocorreu interação (lava + água), a tile virou obsidiana.
                return false;
            }

            // Evaporation
            if (currentTile.LiquidLevel <= 1 && random.NextDouble() < 0.02)
            {
                ClearLiquid(ref currentTile);
                return false;
            }

            // Flow downward
            if (TryFlowTo(x, y, x, y + 1))
                return true;

            // Diagonal flow
            if (TryFlowTo(x, y, x - 1, y + 1))
                return true;

            if (TryFlowTo(x, y, x + 1, y + 1))
                return true;

            // Sideways flow
            FlowSideways(x, y);

            return currentTile.LiquidLevel > 0;
        }

        private bool TryFlowTo(int fromX, int fromY, int toX, int toY)
        {
            if (!IsValidTile(toX, toY)) return false;

            ref Tile fromTile = ref terrain.worldData[fromX, fromY, 1];
            ref Tile toTile = ref terrain.worldData[toX, toY, 1];

            if (toTile.solid) return false;
            
            if (toY > fromY && IsValidTile(toX, fromY))
            {
                ref Tile midTile = ref terrain.worldData[toX, fromY, 1];
                if (midTile.solid) return false;
            }

            if (toTile.LiquidLevel >= 8) return false;

            PlaceLiquid(ref toTile, TileType, (byte)(toTile.LiquidLevel + 1));
            fromTile.LiquidLevel--;

            if (fromTile.LiquidLevel <= 0)
                ClearLiquid(ref fromTile);

            RegisterLiquid(toX, toY);
            RegisterLiquid(fromX, fromY);

            return true;
        }

        private void FlowSideways(int x, int y)
        {
            ref Tile currentTile = ref terrain.worldData[x, y, 1];
            if (currentTile.LiquidLevel <= 0) return;

            int[] directions = { -1, 1 };

            foreach (var dir in directions)
            {
                int nx = x + dir;
                if (!IsValidTile(nx, y)) continue;

                ref Tile neighbor = ref terrain.worldData[nx, y, 1];
                if (neighbor.solid) continue;
                

                if (neighbor.LiquidType == LiquidType.None || neighbor.LiquidType == Type)
                {
                    int total = currentTile.LiquidLevel + neighbor.LiquidLevel;
                    int ideal = total > 8 ? 8 : total / 2;
                    int move = currentTile.LiquidLevel - ideal;

                    if (move > 0 || (move <= 0 && currentTile.LiquidLevel > neighbor.LiquidLevel))
                    {
                        if (neighbor.LiquidType == LiquidType.None)
                        {
                            neighbor.LiquidType = Type;
                            neighbor.TileType = TileType;
                            neighbor.id = Tiles.GetTileId(TileType);
                        }

                        neighbor.LiquidLevel = (byte)Math.Min(neighbor.LiquidLevel + move, 8);
                        currentTile.LiquidLevel = (byte)Math.Max(currentTile.LiquidLevel - move, 0);

                        if (neighbor.LiquidLevel <= 0)
                            ClearLiquid(ref neighbor);

                        if (currentTile.LiquidLevel <= 0)
                            ClearLiquid(ref currentTile);

                        RegisterLiquid(nx, y);
                        RegisterLiquid(x, y);
                    }
                }
            }
        }

        public void RegisterLiquid(int x, int y)
        {
            if (!IsValidTile(x, y)) return;

            lock (_liquidLock)
            {
                ref Tile tile = ref terrain.worldData[x, y, 1];
                if (tile.LiquidType == Type && tile.LiquidLevel > 0)
                {
                    activeLiquidTiles.Add(new Point(x, y));
                }
            }
        }

        private void PlaceLiquid(ref Tile tile, Tiles.TileType tileType, byte liquidLevel = 8)
        {
            tile.LiquidType = Type;
            tile.TileType = tileType;
            tile.id = Tiles.GetTileId(tileType);
            tile.variant = Random.Shared.Next(0, 3);
            tile.LiquidLevel = liquidLevel;
        }

        private void ClearLiquid(ref Tile tile)
        {
            tile.LiquidType = LiquidType.None;
            tile.TileType = Tiles.TileType.ar;
            tile.id = Tiles.GetTileId(Tiles.TileType.ar);
            tile.LiquidLevel = 0;
        }

        private bool IsValidTile(int x, int y)
        {
            return x >= 0 && y >= 0 &&
                   x < terrain.WorldSize.X &&
                   y < terrain.WorldSize.Y;
        }

        private bool CheckForLiquidInteraction(int x, int y)
        {
            ref Tile currentTile = ref terrain.worldData[x, y, 1];

            // Só lava faz a checagem. Água não precisa.
            if (currentTile.LiquidType != LiquidType.Lava)
                return false;

            int[] dx = { -1, 1, 0, 0 };
            int[] dy = { 0, 0, -1, 1 };

            for (int i = 0; i < 4; i++)
            {
                int nx = x + dx[i];
                int ny = y + dy[i];

                if (!IsValidTile(nx, ny)) continue;

                ref Tile neighbor = ref terrain.worldData[nx, ny, 1];

                if (neighbor.LiquidType == LiquidType.Water && neighbor.LiquidLevel > 0)
                {
                    // Lava vira obsidiana
                    CreateObsidianAt(x, y);

                    // Água evapora
                    ClearLiquid(ref neighbor);
                    return true;
                }
            }

            return false;
        }


        private void CreateObsidianAt(int x, int y)
        {
            ref Tile tile = ref terrain.worldData[x, y, 1];

            // Transforma a tile em obsidiana sólida
            tile.solid = true;
            tile.TileType = Tiles.TileType.Obsidian;
            tile.id = Tiles.GetTileId(Tiles.TileType.Obsidian);
            tile.variant = Random.Shared.Next(0, 3);

            // Remove líquido
            tile.LiquidType = LiquidType.None;
            tile.LiquidLevel = 0;
        }

    }
}