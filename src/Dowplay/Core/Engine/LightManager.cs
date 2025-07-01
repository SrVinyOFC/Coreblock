using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Coreblock {
    public class LightManager {
        private int worldWidth;
        private int worldHeight;
        private float lightDecay = 0.95f;
        public float mitigation = 0.75f;
        private float minLightIntensity = 0.1f;
        private float brightness = 3f; // valor padrão
        private float sunIntensity = 0.75f; // valor padrão

        private Vector3[,] readGrid;
        private Vector3[,] writeGrid;
        private Tile[,,] tiles;

        public bool render = true;
        public bool rendering = false;

        private List<(string uid, Point position, Color color, float intensity)> lightSources = new();

        public readonly object bufferSwapLock = new();
        

        public LightManager(Tile[,,] tiles, int width, int height) {
            worldWidth = width;
            worldHeight = height;
            this.tiles = tiles;

            readGrid = new Vector3[width, height];
            writeGrid = new Vector3[width, height];

            _ = PropagateLight();
        }



        private async Task PropagateLight() {
            await Task.Run(async () => {
                while (true) {
                    rendering = true;
                    
                    int tileSize = 8;
                    var cameraPoint = new Point((int)Camera.Position.X / tileSize, (int)Camera.Position.Y / tileSize);
                    var (startX, startY, endX, endY) = CalculateVisibleArea(cameraPoint, tileSize, worldWidth, worldHeight, 80);

                    //reset
                    for (int x = startX; x < endX; x++)
                        for (int y = startY; y < endY; y++)
                        {
                            if (!render) writeGrid[x, y] = new Vector3(1, 1, 1);
                            else writeGrid[x, y] = Vector3.Zero;
                        }

                    Queue<(int x, int y, int originX, int originY, Vector3 color, float intensity)> queue = new();

                    //verifica os pontos de luz
                    lock(bufferSwapLock){
                        foreach (var (uid, pos, color, intensity) in lightSources) {
                            if (pos.X >= startX && pos.X < endX && pos.Y >= startY && pos.Y < endY) {
                                queue.Enqueue((pos.X, pos.Y, pos.X, pos.Y, color.ToVector3(), intensity));
                            }
                        }   

                    }

                    for (int x = startX; x < endX - 1; x++)
                        for (int y = endY - 1; y >= startY; y--)
                        {
                            if (tiles[x, y, 1].id == 0 && tiles[x, y, 0].id == 0)
                                queue.Enqueue((x, y, x, y, Vector3.One, sunIntensity));

                            if (tiles[x, y, 1].LiquidType == LiquidType.Lava && tiles[x, y, 1].LiquidLevel > 0)
                                queue.Enqueue((x, y, x, y, new Vector3(1, 0.5f, 0.5f), 0.6f));
                        }

                    while (queue.Count > 0)
                            {
                                var (x, y, originX, originY, color, intensity) = queue.Dequeue();

                                if (x < 0 || x >= worldWidth || y < 0 || y >= worldHeight)
                                    continue;

                                if (x < startX || x >= endX || y < startY || y >= endY)
                                    continue;


                                float distance = Vector2.Distance(new Vector2(x, y), new Vector2(originX, originY));
                                float falloff = 1.75f / (1f + distance * 0.95f);
                                float currentIntensity = intensity * falloff;
                                Vector3 lightColor = color * currentIntensity;

                                if (writeGrid[x, y].LengthSquared() >= lightColor.LengthSquared())
                                    continue;

                                writeGrid[x, y] = lightColor;


                                float newBaseIntensity = intensity * lightDecay;

                                if (tiles[x, y, 1].id != 0 && !HasNeighborAir(x, y) && tiles[x, y, 1].solid)
                                    newBaseIntensity *= mitigation;


                                if (newBaseIntensity < minLightIntensity)
                                    continue;
                                //if (tiles[x, y, 1].LiquidType == LiquidType.None) continue;

                                if (y > 0) queue.Enqueue((x, y - 1, originX, originY, color, newBaseIntensity));
                                if (x > 0) queue.Enqueue((x - 1, y, originX, originY, color, newBaseIntensity));
                                if (x < worldWidth - 1) queue.Enqueue((x + 1, y, originX, originY, color, newBaseIntensity));
                                if (y < worldHeight - 1) queue.Enqueue((x, y + 1, originX, originY, color, newBaseIntensity));
                            }
                    

                    SwapBuffers();
                    rendering = false;

                    lightSources.Clear();
                    await Task.Delay(16);
                    
                }
            });
        }

        private (int startX, int startY, int endX, int endY) CalculateVisibleArea(Point cameraPosition, int tileSize, int worldWidth, int worldHeight, int offset = 10) {
            int startX = Math.Max(cameraPosition.X - offset, 0);
            int startY = Math.Max(cameraPosition.Y - offset, 0);
            int endX = Math.Min(cameraPosition.X + offset, worldWidth);
            int endY = Math.Min(cameraPosition.Y + offset, worldHeight);
            return (startX, startY, endX, endY);
        }

        private void SwapBuffers() {
            lock (bufferSwapLock) {
                var temp = readGrid;
                readGrid = writeGrid;
                writeGrid = temp;

            }
        }


        public Color GetLightData(int x, int y) {
            lock (bufferSwapLock) {
                if (x < 0 || x >= worldWidth || y < 0 || y >= worldHeight)
                    return Color.Black;
                Vector3 val = readGrid[x, y];
                val = Vector3.Clamp(val * brightness, Vector3.Zero, Vector3.One);
                return new Color(val);
            }
        }
        public Vector3 GetLightDataVec(int x, int y) {
            lock (bufferSwapLock) {
                if (x < 0 || x >= worldWidth || y < 0 || y >= worldHeight)
                    return Vector3.Zero;
                return readGrid[x, y];
            }
        }

        public void AddLightSource(string uid, Point pos, Color color, float intensity = 0.5f){

            lock (bufferSwapLock) {
                int index = lightSources.FindIndex(l => l.uid == uid);
                if (index < 0 && (pos.X < 0 || pos.X >= worldWidth || pos.Y < 0 || pos.Y >= worldHeight)) {
                    return; // Não adiciona se a posição estiver fora dos limites
                }
                // if (index >= 0)
                //     lightSources[index] = (uid, pos, color, intensity);
                // else
                    lightSources.Add((uid, pos, color, intensity));
            }
            
            // Marca área ao redor como dirty
            //MarkAreaDirty(pos.X, pos.Y, 10); // Raio maior para fontes de luz
        }

        public void RemoveLightSource(string uid) {
            lightSources.RemoveAll(l => l.uid == uid);
        }

        public int GetLightSourcesCount(){
            lock(bufferSwapLock) return lightSources.Count;
        }

        public void SetBrightness(float value, bool add) {
            if(!add) return;

            brightness += value;
        }


        private bool HasNeighborAir(int x, int y) {
            return (y + 1 < worldHeight && tiles[x, y + 1, 1].id == 0) ||
                   (y - 1 >= 0 && tiles[x, y - 1, 1].id == 0) ||
                   (x + 1 < worldWidth && tiles[x + 1, y, 1].id == 0) ||
                   (x - 1 >= 0 && tiles[x - 1, y, 1].id == 0);
        }
    }
}
