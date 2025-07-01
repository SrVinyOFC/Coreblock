using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Coreblock;

public class ChunkRenderer
{
    public const int ChunkSize = 32;

    public RenderTarget2D CacheBackground;
    public RenderTarget2D CacheTiles;
    public RenderTarget2D CacheLiquids;

    public bool Dirty = true;
    public bool NeedsLightUpdate = false;

    private int lastCheckFrame = -1;
    private bool lastCheckResult = false;

    public readonly Point ChunkPosition;
    public Tile[,,] tiles;

    public Tile[,,] CacheTile;
    private Vector3[,] CacheLight;

    private readonly SpriteBatch internalBatch;

    public ChunkRenderer(Point chunkPos, Tile[,,] tiles)
    {
        this.tiles = tiles;
        ChunkPosition = chunkPos;
        internalBatch = new SpriteBatch(Globals.GraphicsDevice);

        int size = ChunkSize * (int)Globals.TileSize;
        CacheBackground = new RenderTarget2D(Globals.GraphicsDevice, size, size);
        CacheTiles = new RenderTarget2D(Globals.GraphicsDevice, size, size);
        CacheLiquids = new RenderTarget2D(Globals.GraphicsDevice, size, size);

        CacheTile = new Tile[ChunkSize, ChunkSize, 2];
        CacheLight = new Vector3[ChunkSize, ChunkSize];
    }

    public void Redraw(TileConfig tileConfig)
    {
        lock (GameManager.terrain.chunkLock)
        {
            DrawLayer(tileConfig, CacheBackground, 0, drawLiquids: false);
            DrawLayer(tileConfig, CacheTiles, 1, drawLiquids: false);
            DrawLayer(tileConfig, CacheLiquids, 1, drawLiquids: true);

            UpdateCacheData();
        }

        Dirty = false;
        NeedsLightUpdate = false;
    }

    private void DrawLayer(TileConfig tileConfig, RenderTarget2D target, int layer, bool drawLiquids)
    {
        int tileSize = (int)Globals.TileSize;
        int worldX = ChunkPosition.X * ChunkSize;
        int worldY = ChunkPosition.Y * ChunkSize;

        Globals.GraphicsDevice.SetRenderTarget(target);
        Globals.GraphicsDevice.Clear(Color.Transparent);
        internalBatch.Begin(samplerState: SamplerState.PointClamp);

        for (int y = 0; y < ChunkSize; y++)
        {
            for (int x = 0; x < ChunkSize; x++)
            {
                int wx = worldX + x;
                int wy = worldY + y;

                if (wx < 0 || wy < 0 || wx >= tiles.GetLength(0) || wy >= tiles.GetLength(1))
                    continue;

                ref Tile tile = ref tiles[wx, wy, layer];
                Color light = GameManager.lightManager.GetLightData(wx, wy);

                if (!drawLiquids)
                {
                    if (tile.id == 0 || tile.LiquidType != LiquidType.None) continue;

                    var texture = tileConfig.GetTexture(Tiles.GetTileType(tile.id));
                    if (texture == null) continue;

                    var texCoord = tileConfig.GetCordTexture(tiles, wx, wy, layer, tile.variant);
                    DrawTile(texture, texCoord, x, y, light);
                }
                else
                {
                    if (tile.LiquidType == LiquidType.None || tile.LiquidLevel <= 0) continue;

                    var texture = tileConfig.GetTexture(tile.TileType);
                    if (texture == null) continue;

                    var texCoord = tileConfig.GetCordLiquidTexture(tiles, wx, wy, tile.LiquidLevel, tile.variant);
                    DrawTile(texture, texCoord, x, y, light);
                }
            }
        }

        internalBatch.End();
        Globals.GraphicsDevice.SetRenderTarget(null);
    }

    private void DrawTile(Texture2D texture, Vector2 texCoord, int x, int y, Color light)
    {
        int tileSize = (int)Globals.TileSize;
        var coord = texCoord * tileSize;
        var sourceRect = new Rectangle((int)coord.X, (int)coord.Y, tileSize, tileSize);
        var drawPos = new Vector2(x * tileSize, y * tileSize);
        internalBatch.Draw(texture, drawPos, sourceRect, light);
    }

    private void UpdateCacheData()
    {
        for (int y = 0; y < ChunkSize; y++)
        {
            for (int x = 0; x < ChunkSize; x++)
            {
                int worldX = ChunkPosition.X * ChunkSize + x;
                int worldY = ChunkPosition.Y * ChunkSize + y;

                if (worldX < 0 || worldY < 0 || worldX >= tiles.GetLength(0) || worldY >= tiles.GetLength(1))
                    continue;

                CacheTile[x, y, 0] = new Tile(tiles[worldX, worldY, 0].id, tiles[worldX, worldY, 0].variant);
                CacheTile[x, y, 1] = new Tile(tiles[worldX, worldY, 1].id, tiles[worldX, worldY, 1].variant);
                CacheLight[x, y] = GameManager.lightManager.GetLightDataVec(worldX, worldY);
            }
        }
    }

    public void DrawTiles()
    {
        DrawRenderTarget(CacheBackground);
        DrawRenderTarget(CacheTiles);
    }

    public void DrawLiquids()
    {
        DrawRenderTarget(CacheLiquids);
    }

    private void DrawRenderTarget(RenderTarget2D target)
    {
        if (target == null || target.IsDisposed) return;

        Vector2 drawPos = new Vector2(
            ChunkPosition.X * ChunkSize * (int)Globals.TileSize,
            ChunkPosition.Y * ChunkSize * (int)Globals.TileSize
        );

        Globals.SpriteBatch.Draw(target, drawPos, Color.White);
    }

    public bool CheckForChanges()
    {
        if (Dirty) return true;
        if (Globals.FrameCount == lastCheckFrame) return lastCheckResult;

        lastCheckFrame = Globals.FrameCount;
        lastCheckResult = PerformDeepCheck();
        return lastCheckResult;
    }

    private bool PerformDeepCheck()
    {
        for (int y = 0; y < ChunkSize; y++)
        {
            for (int x = 0; x < ChunkSize; x++)
            {
                int worldX = ChunkPosition.X * ChunkSize + x;
                int worldY = ChunkPosition.Y * ChunkSize + y;

                for (int layer = 0; layer <= 1; layer++)
                {
                    if (worldX < 0 || worldY < 0 || worldX >= tiles.GetLength(0) || worldY >= tiles.GetLength(1))
                        continue;
                    if (tiles[worldX, worldY, layer].id != CacheTile[x, y, layer].id)
                        return true;
                }

                var currentLight = GameManager.lightManager.GetLightDataVec(worldX, worldY);
                if (Vector3.DistanceSquared(currentLight, CacheLight[x, y]) > 0.0005f)
                {
                    NeedsLightUpdate = true;
                    return true;
                }
            }
        }

        return false;
    }
}
