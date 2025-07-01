
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Coreblock
{
    public static class Globals
    {
        
        public static Texture2D PixelTexture { get; set; }
        public static ChunkRenderer[,] ChunkRenderers { get; set; } = new ChunkRenderer[0, 0];
        public static float Time { get; set; }
        public static float DeltaTime => Time;
        
        public static ContentManager Content { get; set; }
        public static Microsoft.Xna.Framework.Graphics.SpriteBatch SpriteBatch { get; set; }
        public static System.Drawing.Point WindowSize { get; set; }
        private static System.Drawing.Point PreviousWindowSize { get; set; }
        public static System.Drawing.Point CameraZoom { get; internal set; }
        public static GraphicsDevice GraphicsDevice { get; internal set; }

        public static Microsoft.Xna.Framework.Color BackgroundColor { get; set; } = new Microsoft.Xna.Framework.Color(85, 95, 112, 255); // Cor de fundo padrão

        public static SpriteFont Font;

        public static float TileSize = 8;
        public static int FrameCount { get; private set; }
        public static System.Random Random { get; internal set; }

        public static void Update(GameTime gt)
        {
            Time = (float)gt.ElapsedGameTime.TotalSeconds;
            FrameCount++;
        }   

        public static void Initialize(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, ContentManager contentManager){
            SpriteBatch = spriteBatch; // Inicializa o SpriteBatch
            GraphicsDevice = graphicsDevice; // Inicializa o GraphicsDevice
            Content = contentManager; // Inicializa o ContentManager
            Random = new System.Random(); // 🔥 ESSENCIAL

            

            PixelTexture = new Texture2D(GraphicsDevice, 8, 8); // Inicializa o PixelTextureLight com tamanho 8x8
            PixelTexture.SetData(Enumerable.Repeat(Microsoft.Xna.Framework.Color.White, PixelTexture.Width * PixelTexture.Height).ToArray()); // Define a cor do PixelTextureLight como branco    
        }
        public static void SetFullScreen(GraphicsDeviceManager graphics, bool isFullScreen)
        {
            if (isFullScreen)
            {
                // Armazena o tamanho da janela antes de entrar no modo de tela cheia
                PreviousWindowSize = WindowSize;

                graphics.IsFullScreen = true; // Não usar tela cheia exclusiva
                graphics.HardwareModeSwitch = false; // Garante modo janela sem borda
                
                graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

                WindowSize = new System.Drawing.Point(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height);
            }
            else
            {
                graphics.IsFullScreen = false;

                // Restaura o tamanho da janela anterior
                WindowSize = PreviousWindowSize;
                graphics.PreferredBackBufferWidth = WindowSize.X;
                graphics.PreferredBackBufferHeight = WindowSize.Y;
            }
            graphics.ApplyChanges();
        }
    }
}