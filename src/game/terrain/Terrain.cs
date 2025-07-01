using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Threading.Tasks;
using System.Net;

namespace Coreblock
{
    public class Terrain
    {

        // Controles de renderização
        public int RenderRadiusHorizontal { get; set; } = 2;
        public int RenderRadiusVertical { get; set; } = 1;

        private float amplitude = 50; // Valor máximo do noise para esse bioma
        //private float height = 50; // Valor máximo do noise para esse bioma
        private float dirHeight = 40; // Valor máximo do noise para esse bioma

        Random random = new Random();
        //Configurações de terreno
        private int heightAddition = 2000; // Adição de altura ao terreno
        private TileConfig tileConfig;
        public Tile[,,] worldData;

        public Point WorldSize = new Point(5300, 2300); // Tamanho do bloco em tiles

        private int seed = 1; // Número de camadas do terreno]
        public readonly Dictionary<Point, ChunkRenderer> chunkRenderers = new();
        public object chunkLock = new object();

        public List<Biome> specialBiomes = new List<Biome>();


        public bool IsGenerating { get; private set; } = false;
        public float Progress { get; private set; } = 0f;
        public string ProgressState { get; private set; } = "Gerando terreno...";

        public Terrain(int width, int height)
        {
            seed = new Random().Next(0, 10000); // Gera uma semente aleatória

            this.WorldSize.X = width;
            this.WorldSize.Y = height;

            worldData = new Tile[width, height, 2];
            tileConfig = new TileConfig(); // Inicializa o TileConfig

            heightAddition = (int)(height * (height * 0.000001f)); // Adiciona 10% da altura total do mapa

            _ = StartAsyncGeneration();

        }


        private async Task StartAsyncGeneration()
        {
            IsGenerating = true;
            await Task.Run(() => Generate());
        }

        private void Generate()
        {
            // Gera o terreno base (predominantemente floresta)
            CaveGanerator noise = new CaveGanerator(WorldSize.X, WorldSize.Y, 5, 53, 4);
            int[,] cavePoints = noise.generate(seed);

            // Lista de biomas especiais (deserto e neve)
            specialBiomes = GenerateSpecialBiomes();

            for (int x = 0; x < WorldSize.X; x++)
            {
                for (int y = 0; y < WorldSize.Y; y++)
                {
                    // Verifica se está dentro de algum bioma especial
                    BiomeType biomeType = BiomeType.Forest; // Padrão é floresta

                    foreach (var area in specialBiomes)
                    {
                        if (IsInBiomeArea(x, y, area))
                        {
                            biomeType = area.Type;
                            break;
                        }
                    }

                    // Aplica o bioma
                    SetTiles(x, y, biomeType, cavePoints[x, y] == 0);
                }

                Progress = (float)x / WorldSize.X;
                ProgressState = $"Gerando terreno... {Progress * 100:0}%";
            }

            // Ajuste baseado no tamanho do mundo
            int worldArea = WorldSize.X * WorldSize.Y;

            // 🔸 Número de poças proporcional (você pode ajustar o divisor)
            int count = worldArea / 150; // quanto menor o divisor, mais poças
            count = Math.Clamp(count, 3, 300); // limites mínimos e máximos

            // 🔸 Tamanho das poças proporcional
            int minSize = Math.Max(3, WorldSize.X / 200);
            int maxSize = Math.Max(7, WorldSize.X / 100);

            // 🔸 Geração no mundo todo, exceto muito perto do topo e bordas
            int minY = (int)(WorldSize.Y * 0.01f);
            int maxY = (int)(WorldSize.Y * 0.9f);

            // 🔸 Fator de densidade (ajuste se quiser mais espalhado ou mais concentrado)
            float densityFactor = 1.0f;

            // Gera água na superfície
            GenerateMultipleLiquidPonds(
                GameManager.Water,
                count: count,
                minSize: minSize,
                maxSize: maxSize,
                minY: minY,
                maxY: maxY,
                densityFactor: densityFactor
            );

            // Gera lava em áreas mais profundas
            minY = (int)(WorldSize.Y * 0.2f);
            GenerateMultipleLiquidPonds(
                GameManager.Lava,
                count: count,
                minSize: minSize,
                maxSize: maxSize,
                minY: minY,
                maxY: maxY,
                densityFactor: densityFactor
            );
            

            IsGenerating = false;
        }

        private void TryPlaceOre(int x, int y, Biome biome)
        {
            foreach (var oreData in biome.Ores)
            {
                // Verifica se está na altura adequada para este minério
               // if (!oreData.IsInHeight(y)) continue;

                // Gera um noise específico para este minério
                float oreNoise = PerlinNoise.NormalizedValue(
                    x * oreData.space, 
                    y * oreData.space, 
                    seed + oreData.tileType.GetHashCode(),
                    scale: oreData.scale,         // Menor escala = manchas maiores
                    octaves: 4,            // Mais detalhe
                    persistence: 0.5f
                    );

                // Se o noise passar do threshold, coloca o minério
                if (oreNoise > oreData.chance)
                {
                    PlaceTile(x, y, oreData.tileType, 1);
                    return; // Apenas um minério por tile
                }
            }
        }

        private List<Biome> GenerateSpecialBiomes()
        {
            List<Biome> specialBiomes = new List<Biome>();
            Random rand = new Random(seed);

            // Fator de alongamento vertical (1.5x mais alto que largo)
            float verticalStretch = 1.5f + WorldSize.Y * 0.00001f;

            // Gera 3-5 biomas especiais de cada tipo
            int desertCount = rand.Next(1, 2);
            int snowCount = rand.Next(1, 2);

            for (int i = 0; i < desertCount; i++)
            {
                specialBiomes.Add(new Biome
                {
                    Type = BiomeType.Desert,
                    Center = new Vector2(
                        rand.Next(100, WorldSize.X - 100),
                        rand.Next(100, WorldSize.Y / 3)), // Mais comuns na superfície
                    Radius = rand.Next((int)(BiomeDatabase.Biomes[BiomeType.Desert].Radius * .6f), (int)BiomeDatabase.Biomes[BiomeType.Desert].Radius) + WorldSize.X * 0.045f,
                    VerticalStretch = verticalStretch // Aplica o alongamento
                });
            }

            for (int i = 0; i < snowCount; i++)
            {
                specialBiomes.Add(new Biome
                {
                    Type = BiomeType.Snow,
                    Center = new Vector2(
                        rand.Next(100, WorldSize.X - 100),
                        rand.Next(100, WorldSize.Y / 3)), // Mais comuns em áreas altas
                    Radius = rand.Next((int)(BiomeDatabase.Biomes[BiomeType.Snow].Radius * .6f), (int)BiomeDatabase.Biomes[BiomeType.Snow].Radius) + WorldSize.X * 0.045f,
                    VerticalStretch = verticalStretch // Aplica o alongamento
                });
            }

            return specialBiomes;
        }

        private bool IsInBiomeArea(int x, int y, Biome area)
        {
            // Aplica o alongamento vertical no cálculo da distância
            float dx = (x - area.Center.X) / area.Radius;
            float dy = (y - area.Center.Y) / (area.Radius * area.VerticalStretch);
            float distanceSquared = dx * dx + dy * dy;

            float noise = IslandGenerator.GenerateOrganicNoise(x, y, seed, 3, 0.5f);
            float threshold = 1.0f + (noise - 0.5f) * 0.3f;

            return distanceSquared < threshold;
        }

        public void SetTiles(int x, int y, BiomeType biomeType, bool isCave = false)
        {
            Biome biome = BiomeDatabase.Biomes[biomeType];

            float seedOffset = seed * 0.2f;

            // 🔹 Cálculo da altura do terreno (otimizado)
            float baseNoise = PerlinNoise.NormalizedValue(x, seedOffset, seed);

            float baseHeight = baseNoise * amplitude / 6f;
            float mountainHeight = baseNoise * amplitude * 4;

            float flatness = PerlinNoise.NormalizedValue((x + seed) * 0.02f, seed * 0.02f, seed);
            float flatFactor = MathHelper.Lerp(1f, 0.6f, flatness);

            int terrainHeight = (int)Math.Floor((baseHeight + mountainHeight) * flatFactor) - heightAddition;

            // 🔹 Pré-calcula valores de noise para esta posição
            float SecondaryTile = PerlinNoise.NormalizedValue(x * biome.SecondaryTileScale, y * biome.SecondaryTileScale, seed);
            bool isSurfaceLayer = y < terrainHeight + (dirHeight + mountainHeight / 3) && y > terrainHeight;
            bool isSurfaceLayerBackground = y > terrainHeight + 3;
            bool isUnderground = y > terrainHeight;
            bool isTopLayer = y == terrainHeight + 1;


            if (isSurfaceLayerBackground) PlaceTile(x, y, biome.BackgroundTile, 0);

            if (isCave) return;


            if (isSurfaceLayer)
            {
                if (isTopLayer)
                {

                    PlaceTile(x, y, biome.TopTile, 1);
                }
                else if (SecondaryTile > .65f)
                {
                    PlaceTile(x, y, biome.UndergroundTile, 1);
                }
                else
                {
                    PlaceTile(x, y, biome.SurfaceTile, 1);

                }
            }
            else if (isUnderground)
            {
                if (SecondaryTile > .6f)
                {
                    PlaceTile(x, y, biome.SecondaryTile, 1);
                }
                else
                {
                    PlaceTile(x, y, biome.UndergroundTile, 1);
                }
            }
            // Primeiro tenta colocar minérios (tem prioridade sobre blocos normais)
            if (y > terrainHeight + 5) // Não coloca minérios muito perto da superfície
            {
                TryPlaceOre(x, y, biome);
                return; // Se colocou um minério, não coloca bloco normal
            }
        }


        public void GenerateLiquidPond(Liquid liquid, int centerX, int centerY, int baseRadius)
        {
            // Lista para armazenar bordas orgânicas
            List<Point> edgePoints = new List<Point>();
            HashSet<Point> filledPoints = new HashSet<Point>();
            Queue<Point> fillQueue = new Queue<Point>();

            if (!IsInWorld(centerX, centerY)) return;

            // Gera um raio variável para bordas orgânicas
            float radiusVariation = 0.3f; // 30% de variação no raio

            // Primeiro passada: identificar todos os pontos válidos
            for (int y = -baseRadius; y <= baseRadius; y++)
            {
                for (int x = -baseRadius; x <= baseRadius; x++)
                {

                    int wx = centerX + x;
                    int wy = centerY + y;

                    if (!IsInWorld(wx, wy)) continue;

                    if (wx >= worldData.GetLength(0) || wy >= worldData.GetLength(1))
                        continue;

                    // Distância com noise para bordas irregulares
                    float distance = MathF.Sqrt(x * x + y * y);
                    float noise = PerlinNoise.Value(wx * 0.1f, wy * 0.1f, seed);
                    float effectiveRadius = baseRadius * (1 + (noise - 0.5f) * radiusVariation);

                    if (distance > effectiveRadius) continue;

                    ref Tile tile = ref worldData[wx, wy, 1];

                    if (!tile.solid)
                    {
                        // Verifica se é borda (tem sólido adjacente)
                        if (IsEdgeTile(wx, wy))
                        {
                            edgePoints.Add(new Point(wx, wy));
                        }
                        else
                        {
                            fillQueue.Enqueue(new Point(wx, wy));
                        }
                    }
                }
            }

            // Preenche o interior primeiro
            while (fillQueue.Count > 0)
            {
                var point = fillQueue.Dequeue();
                if (filledPoints.Contains(point)) continue;

                ref Tile tile = ref worldData[point.X, point.Y, 1];
                tile.LiquidType = liquid.Type;
                tile.TileType = liquid.TileType;
                tile.LiquidLevel = 8; // Preenche completamente
                liquid.RegisterLiquid(point.X, point.Y);
                filledPoints.Add(point);
            }

            // Preenche as bordas com níveis variados para efeito mais natural
            foreach (var point in edgePoints)
            {
                ref Tile tile = ref worldData[point.X, point.Y, 1];
                tile.LiquidType = liquid.Type;
                tile.TileType = liquid.TileType;

                // Nível de líquido reduzido nas bordas
                tile.LiquidLevel = (byte)(4 + (PerlinNoise.Value(point.X * 0.5f, point.Y * 0.5f, seed) * 4));
                liquid.RegisterLiquid(point.X, point.Y);
            }
        }

        private bool IsEdgeTile(int x, int y)
        {
            // Verifica os 4 vizinhos diretos
            int[,] directions = { { -1, 0 }, { 1, 0 }, { 0, -1 }, { 0, 1 } };

            for (int i = 0; i < 4; i++)
            {
                int nx = x + directions[i, 0];
                int ny = y + directions[i, 1];

                if (IsInWorld(nx, ny) && worldData[nx, ny, 1].solid)
                {
                    return true;
                }
            }
            return false;
        }

        public void GenerateMultipleLiquidPonds( Liquid liquid, int count, int minSize = 3, int maxSize = 7, int minY = 0, int maxY = -1, float densityFactor = 1.0f)
        {
            if (maxY == -1) maxY = WorldSize.Y;

            Random rand = new Random(seed);
            List<Point> existingPonds = new List<Point>();
            int minDistance = (int)((maxSize + minSize) * 1.5f * densityFactor);

            for (int i = 0; i < count; i++)
            {
                int attempts = 0;
                while (attempts < 100) // Limite de tentativas
                {
                    int px = rand.Next(50, WorldSize.X - 50);
                    int py = rand.Next(minY, Math.Clamp(maxY, minY, WorldSize.Y - 50));

                    // Ajusta o tamanho baseado na densidade e profundidade
                    float depthFactor = 1.0f - ((float)(py - minY) / (maxY - minY)) * 0.5f;
                    int adjustedMaxSize = (int)(maxSize * depthFactor * densityFactor);
                    int radius = rand.Next(minSize, Math.Max(minSize + 1, adjustedMaxSize));

                    // Verifica distância mínima de outras poças
                    bool tooClose = false;
                    foreach (var pond in existingPonds)
                    {
                        if (Math.Abs(pond.X - px) < minDistance &&
                            Math.Abs(pond.Y - py) < minDistance)
                        {
                            tooClose = true;
                            break;
                        }
                    }

                    if (!tooClose)
                    {
                        GenerateLiquidPond(liquid, px, py, radius);
                        existingPonds.Add(new Point(px, py));
                        break;
                    }
                    attempts++;
                }
            }
        }


        private bool IsInWorld(int x, int y)
        {
            return x >= 0 && y >= 0 && x < WorldSize.X && y < WorldSize.Y;
        }

        public void PlaceTile(int x, int y, Tiles.TileType tileType, int layer)
        {
            lock (chunkLock)
            {
                if (x >= 0 && x < WorldSize.X && y >= 0 && y < WorldSize.Y)
                {
                    Tile tile = Tiles.GetTile(tileType);
                    tile.variant = random.Next(0, 3);
                    tile.slopeType = SlopeType.SlopeUpRight;

                    worldData[x, y, layer] = tile;


                }
            }
        }


        public Tile GetTile(int x, int y, int layer)
        {
            if (x >= 0 && x < WorldSize.X && y >= 0 && y < WorldSize.Y)
            {
                return worldData[x, y, layer];
            }
            return new Tile(); // Retorna um bloco vazio se fora dos limites
        }


        public void Draw(Action<ChunkRenderer> drawMethod)
        {
            if (IsGenerating) return;

            ProcessNecessaryUpdates();

            var (visibleStart, visibleEnd) = CalculateVisibleArea();

            for (int cy = visibleStart.Y; cy <= visibleEnd.Y; cy++)
            {
                for (int cx = visibleStart.X; cx <= visibleEnd.X; cx++)
                {
                    Point chunkPos = new Point(cx, cy);

                    if (!chunkRenderers.TryGetValue(chunkPos, out var chunk))
                    {
                        chunk = new ChunkRenderer(chunkPos, worldData);
                        chunkRenderers[chunkPos] = chunk;
                        chunk.Dirty = true;
                        chunk.Redraw(tileConfig);
                    }

                    lock (chunkLock)
                    {
                        drawMethod(chunk);
                    }
                }
            }
        }


        private void ProcessNecessaryUpdates()
        {

            var (visibleStart, visibleEnd) = CalculateVisibleArea();
            int updatesThisFrame = 0;
            const int maxUpdatesPerFrame = 4; // Limite para manter FPS

            // Varre por raios concêntricos (do centro para fora)
            for (int radius = 0; radius <= Math.Max(RenderRadiusHorizontal, RenderRadiusVertical); radius++)
            {
                for (int cy = visibleStart.Y; cy <= visibleEnd.Y; cy++)
                {
                    for (int cx = visibleStart.X; cx <= visibleEnd.X; cx++)
                    {
                        // Verifica se está no raio atual de verificação
                        int centerX = (visibleStart.X + visibleEnd.X) / 2;
                        int centerY = (visibleStart.Y + visibleEnd.Y) / 2;
                        int dist = Math.Max(Math.Abs(cx - centerX), Math.Abs(cy - centerY));

                        if (dist != radius) continue;

                        if (updatesThisFrame >= maxUpdatesPerFrame)
                            return;

                        if (chunkRenderers.TryGetValue(new Point(cx, cy), out var chunk))
                        {
                            // VERIFICA SE PRECISA ATUALIZAR ANTES DE REDRAW
                            if (chunk.Dirty || chunk.CheckForChanges())
                            {
                                lock (chunkLock) { chunk.Redraw(tileConfig); }
                                updatesThisFrame++;
                            }
                        }
                    }
                }
            }
        }

        private (Point start, Point end) CalculateVisibleArea()
        {
            int camTileX = (int)(Camera.Position.X / Globals.TileSize);
            int camTileY = (int)(Camera.Position.Y / Globals.TileSize);
            int chunkSize = ChunkRenderer.ChunkSize;

            int startX = Math.Max(0, (camTileX / chunkSize) - RenderRadiusHorizontal);
            int startY = Math.Max(0, (camTileY / chunkSize) - RenderRadiusVertical);
            int endX = Math.Min(WorldSize.X / chunkSize,
                            (camTileX / chunkSize) + RenderRadiusHorizontal);
            int endY = Math.Min(WorldSize.Y / chunkSize,
                            (camTileY / chunkSize) + RenderRadiusVertical);

            return (new Point(startX, startY), new Point(endX, endY));
        }

        public ChunkRenderer GetChunk(int chunkX, int chunkY)
        {
            lock (chunkLock)
            {
                Point chunkPos = new Point(chunkX, chunkY);

                if (chunkRenderers.TryGetValue(chunkPos, out var chunk))
                    return chunk;

                return null;
            }
        }

        private void DebugDrawChunkBorders(Point visibleStart, Point visibleEnd)
        {
            for (int cy = visibleStart.Y; cy <= visibleEnd.Y; cy++)
            {
                for (int cx = visibleStart.X; cx <= visibleEnd.X; cx++)
                {
                    Vector2 pos = new Vector2(
                        cx * ChunkRenderer.ChunkSize * Globals.TileSize,
                        cy * ChunkRenderer.ChunkSize * Globals.TileSize);

                    Rectangle border = new Rectangle(
                        (int)pos.X, (int)pos.Y,
                        ChunkRenderer.ChunkSize * (int)Globals.TileSize,
                        ChunkRenderer.ChunkSize * (int)Globals.TileSize);

                    Globals.SpriteBatch.Draw(
                        Globals.PixelTexture,
                        border,
                        new Color(Color.Red, 0.3f));
                }
            }
        }
    }
}