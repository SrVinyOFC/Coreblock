using Microsoft.Xna.Framework;

namespace Coreblock
{
    public static class UI{
        private static FrameCounter _frameCounter = new FrameCounter();


        public static void Update(GameTime gameTime){
            
        }

        public static void DrawUI(GameTime gameTime){
            // Desenha a interface do usuário aqui
            Globals.SpriteBatch.Begin();
            string renderInfo = $"Render Radius: H{GameManager.terrain.RenderRadiusHorizontal} V{GameManager.terrain.RenderRadiusVertical}";
            Globals.SpriteBatch.DrawString(Globals.Font, renderInfo, new System.Numerics.Vector2(10, 10), Color.White);

            string lightSources = $"Light Sources: {GameManager.lightManager.GetLightSourcesCount()}";
            Globals.SpriteBatch.DrawString(Globals.Font, lightSources, new System.Numerics.Vector2(10, 30), Color.White);

            string playerPos = $"Player Position( x: {GameManager.player.position.X/8:0.0}, y: {GameManager.player.position.Y/8:0.0})";
            Globals.SpriteBatch.DrawString(Globals.Font, playerPos, new System.Numerics.Vector2(10, 50), Color.White);

            var worldPos = InputManager.GetMousePositionWorld();

            string mousePosition = $"Player Position( x: {worldPos.X:0.0}, y: {worldPos.Y:0.0})";
            Globals.SpriteBatch.DrawString(Globals.Font, mousePosition, new System.Numerics.Vector2(10, 70), Color.White);


            Globals.SpriteBatch.End();
        }

        public static void ShowFPSCounter(GameTime gameTime){
            Globals.SpriteBatch.Begin();
            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            _frameCounter.Update(deltaTime);
            var fps = string.Format("FPS: {0:0.0}", _frameCounter.AverageFramesPerSecond);

            var screenPosition = new Vector2(10, Globals.WindowSize.Y - 15);

            Globals.SpriteBatch.DrawString(Globals.Font, $"{fps}", screenPosition, Color.LightGreen);
            Globals.SpriteBatch.End();
        }
    }
}