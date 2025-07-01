using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Coreblock;

public class Main : Game
{
    private static GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    public static int Width => 960;
    public static int Height => 582;
    public static int HalfWidth => Width / 2;
    public static int HalfHeight => Height / 2;
    public static float AspectRatio => (float)Width / Height;
    public static float HalfAspectRatio => (float)HalfWidth / HalfHeight;
    public static float HalfAspectRatioInverse => (float)HalfHeight / HalfWidth;
    public static float HalfWidthInverse => (float)1 / HalfWidth;
    public static float HalfHeightInverse => (float)1 / HalfHeight;

    public GameManager gameManager { get; set; }



    public Main()
    {

        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        Window.Title = "Dowplay Game"; // Set the initial window title


        //Componests

    }

    protected override void Initialize()
    {
        _graphics.SynchronizeWithVerticalRetrace = false;
        Globals.WindowSize = new(Width, Height);
        _graphics.PreferredBackBufferWidth = Width; // Set the desired width
        _graphics.PreferredBackBufferHeight = Height; // Set the desired height

        IsFixedTimeStep = true;
        TargetElapsedTime = TimeSpan.FromSeconds(1.0 / 75.0); // 120 FPS
        _graphics.SynchronizeWithVerticalRetrace = true; // Desliga o V-Sync
        //_graphics.IsFullScreen = true;
        _graphics.ApplyChanges();

        Globals.Content = Content;
        Globals.GraphicsDevice = GraphicsDevice;
        Globals.Font = Content.Load<SpriteFont>("Fonts/Default");
        gameManager = new GameManager();

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        // TODO: use this.Content to load your game content here



        Globals.Initialize(_spriteBatch, GraphicsDevice, Content);

        //sprite[i,j] = new Sprite(Texture, new Vector2(i * 8, j * 8), new Rectangle(i, 0, 8, 8));

    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        Window.Title = "Dowplay Game"; // Set the initial window title
        // TODO: Add your update logic here

        Globals.Update(gameTime);
        gameManager.Update();

        UI.Update(gameTime);


        var input = InputManager.IsSingleKeyPress(Keys.F11);
        if (input)
        {
            Globals.SetFullScreen(_graphics, !_graphics.IsFullScreen);
        }


        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {

        GraphicsDevice.Clear(Color.Transparent);

        // TODO: Add your drawing code here

        gameManager.Draw();
        UI.DrawUI(gameTime);
        UI.ShowFPSCounter(gameTime);



        base.Draw(gameTime);
    }
    
    protected override void UnloadContent()
    {
        CustomBlendStates.Dispose();
        base.UnloadContent();
    }
}
