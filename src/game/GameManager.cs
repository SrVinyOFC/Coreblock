using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Coreblock
{
    public class GameManager{
        public static Terrain terrain;
        public static LightManager lightManager;
        public static Player player;
        public static Liquid Water;
        public static Liquid Lava;

        ParticleSystem particles = new ParticleSystem();

        private TileConfig tileConfig;


        public GameManager()
        {
            terrain = new Terrain(512, 512);

            player = new Player(Globals.Content.Load<Texture2D>("Textures/Entitys/Player"));
            player.position.X = 200;

            player.SetBounds(terrain.WorldSize);
            Camera.Zoom = 2f;

            tileConfig = new TileConfig(); // Inicializa o TileConfig

            lightManager = new LightManager(terrain.worldData, terrain.WorldSize.X, terrain.WorldSize.Y);

            Water = new Liquid(LiquidType.Water, Tiles.TileType.Water, 16);
            Lava = new Liquid(LiquidType.Lava, Tiles.TileType.Lava, 100);
        }

        public string uid;
        

        public void Update(){
            

            if (terrain.IsGenerating)
            {
                Console.WriteLine(terrain.ProgressState);
            }
            else
            {
                // Mapa pronto
                // Pule o frame de atualização do líquido a cada 2 frames
                //liquid.Update(); // Atualiza o líquido
                
                player.Update(); // Atualiza o jogador
                Camera.FollowPlayer(player.position, terrain.WorldSize.X, terrain.WorldSize.Y, Globals.TileSize, 1f); // Atualiza a posição da câmera para o jogador
                particles.Update((float)Globals.DeltaTime, lightManager); // Atualiza o sistema de partículas

                //Testes ///////////////////////////////////////////////////////////////////////////////////////


                var worldPos = InputManager.GetMousePositionWorld();
                
                // if (Globals.FrameCount % 5 == 0) // Cria novas partículas a cada 5 frames
                // {
                //     particles.SpawnBurst(
                //         texture: Globals.PixelTexture,
                //         origin: new Vector2(worldPos.X * Globals.TileSize, worldPos.Y * Globals.TileSize) + new Vector2((float)Globals.Random.NextDouble() * 10f - 5f, 0),
                //         shape: SpawnShape.Point,
                //         count: 3,
                //         minSpeed: 20f,
                //         maxSpeed: 60f,
                //         lifetime: 1.5f,
                //         minScale: 0.8f,
                //         maxScale: 1.3f,
                //         scaleDelta: -0.2f,
                //         startColor: Color.Orange,
                //         endColor: Color.DarkRed,
                //         minRotationSpeed: -1f,
                //         maxRotationSpeed: 1f,
                //         fadeInTime: 0.1f,
                //         fadeOutTime: 0.5f,
                //         emitsLight: true,
                //         lightColor: new Color(1f, 0.5f, 0.2f),
                //         minLightIntensity: 0.4f,
                //         maxLightIntensity: 0.7f,
                //         lightRadius: 1.2f
                //     );
                // }


                if (InputManager.IsSingleMouseButtonPress(InputManager.MouseButton.Left))
                {

                    terrain.PlaceTile((int)worldPos.X, (int)worldPos.Y, Tiles.TileType.ar, 1); // Define o tile no terreno

                    // Spawna uma explosão de partículas na posição do mouse ao clicar com o botão esquerdo
                    particles.SpawnBurst(
                        texture: Globals.PixelTexture, // Textura da partícula
                        origin: new Vector2(worldPos.X * Globals.TileSize, worldPos.Y * Globals.TileSize), // Posição inicial das partículas (em pixels)
                        shape: SpawnShape.Circle, // Forma de distribuição das partículas (círculo)
                         count: 30,
                        minSpeed: 50f,
                        maxSpeed: 300f,
                        lifetime: 1.2f,
                        minScale: 0.2f,
                        maxScale: 0.1f,
                        scaleDelta: -0.8f,
                        startColor: Color.Yellow,
                        endColor: Color.Red,
                        minRotationSpeed: -5f,
                        maxRotationSpeed: 5f,
                        fadeInTime: 0.05f,
                        fadeOutTime: 0.4f,
                        spreadAngle: MathHelper.TwoPi,
                        shapeRadius: 15f,
                        emitsLight: true,
                        lightColor: new Color(1f, 0.6f, 0.3f), // Laranja claro
                        minLightIntensity: 0.1f,
                        maxLightIntensity: 0.5f,
                        lightRadius: 1f
                    );


                }
                else if (InputManager.IsMouseButtonPressed(InputManager.MouseButton.Right))
                {
                    Water.AddLiquid((int)worldPos.X, (int)worldPos.Y);

                    //terrain.PlaceTile((int)worldPos.X, (int)worldPos.Y, Tiles.TileType.Coal, 1); // Define o tile no terreno
                }

                if (InputManager.IsMouseButtonPressed(InputManager.MouseButton.XButton1)) // por exemplo, tecla L
                {

                    var random = new Random();
                    var color = new Color(
                        random.Next(256),
                        random.Next(256),
                        random.Next(256)
                    );
                    uid = Guid.NewGuid().ToString();

                    lightManager.AddLightSource(uid, new Point((int)worldPos.X, (int)worldPos.Y), new Color(0, 255, 255), 1f);
                    //lightManager.AddLightSource(uid, worldPos, color );

                    //terrain.UpdateChunk((int)worldPos.X, (int)worldPos.Y);
                }


                //Console.WriteLine(terrain.GetTile((int)worldPos.X, (int)worldPos.Y, 1).solid);

                if (InputManager.IsSingleKeyPress(Keys.L))
                {
                    lightManager.render = !lightManager.render;
                }

                if (InputManager.IsSingleKeyPress(Keys.I))
                {
                    player.inspect = !player.inspect;
                }
            }


            // Exemplo de controle por teclado
            if (InputManager.IsSingleKeyPress(Keys.Right))
                terrain.RenderRadiusHorizontal++;
            if (InputManager.IsSingleKeyPress(Keys.Left))
                terrain.RenderRadiusHorizontal--;

            if (InputManager.IsSingleKeyPress(Keys.Up))
                terrain.RenderRadiusVertical++;
            if (InputManager.IsSingleKeyPress(Keys.Down))
                terrain.RenderRadiusVertical--;
                
            }
            

        public void Draw(){

            if (terrain.IsGenerating) return;

            Globals.SpriteBatch.Begin(
                transformMatrix: Camera.Transform,
                samplerState: SamplerState.PointClamp,
                blendState: BlendState.AlphaBlend);
            
            
            terrain.Draw(c => c.DrawTiles());
            player.Draw();
            particles.Draw(Globals.SpriteBatch);
            terrain.Draw(c => c.DrawLiquids());

            //terrain.Draw(chunk => chunk.DrawLiquids());

            var mousePos = InputManager.GetMousePosition();
            var worldPos = Camera.ScreenToWorld(mousePos, Globals.TileSize);

            DebugLine.DrawRect((int)worldPos.X*Globals.TileSize + 4, (int)worldPos.Y*Globals.TileSize + 4, 1, 1, color: new Color(0.1f, 1, .6f, 0.1f));

            DebugLine.DrawRect(player.position.X, player.position.Y, player.size.X,player.size.Y, color: Color.Red);


            
            Globals.SpriteBatch.End();

        }
    }
}