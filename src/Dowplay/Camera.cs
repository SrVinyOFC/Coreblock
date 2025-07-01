using Microsoft.Xna.Framework;

namespace Coreblock;

public static class Camera {
    public static Vector2 Position { get; set; } = new Vector2(0, 0);
    public static float Rotation { get; set; } = 0f;
    public static float Zoom { get; set; } = 1f;
    public static Vector2 ScreenToWorld (Vector2 screenPosition, float tileSize) {
        // Converte a posição da tela para a posição do mundo
        Vector2 worldPosition = Vector2.Transform(screenPosition, Matrix.Invert(Transform));
        return new Vector2(worldPosition.X / tileSize, worldPosition.Y / tileSize);
    }
    
    public static Matrix Transform => Matrix.CreateTranslation(new Vector3(-Position, 0)) *
                                      Matrix.CreateRotationZ(Rotation) *
                                      Matrix.CreateScale(new Vector3(Zoom, Zoom, 1)) *
                                      Matrix.CreateTranslation(new Vector3(Globals.WindowSize.X / 2, Globals.WindowSize.Y / 2, 0));

    public static void ClampPosition(float terrainWidth, float terrainHeight, float tileSize) {
        float worldWidth = terrainWidth * tileSize;
        float worldHeight = terrainHeight * tileSize;

        float halfScreenWidth = Globals.WindowSize.X / (2f * Zoom);
        float halfScreenHeight = Globals.WindowSize.Y / (2f * Zoom);

        float minX = halfScreenWidth;
        float maxX = worldWidth - halfScreenWidth;
        float minY = halfScreenHeight;
        float maxY = worldHeight - halfScreenHeight;

        // Proteção caso o mundo for menor que a tela
        if (worldWidth < Globals.WindowSize.X / Zoom) {
            minX = maxX = worldWidth / 2f;
        }
        if (worldHeight < Globals.WindowSize.Y / Zoom) {
            minY = maxY = worldHeight / 2f;
        }

        float clampedX = MathHelper.Clamp(Position.X, minX, maxX);
        float clampedY = MathHelper.Clamp(Position.Y, minY, maxY);

        Position = new Vector2(clampedX, clampedY);
    }


    public static void FollowPlayer(Vector2 playerPosition, float terrainWidth, float terrainHeight, float tileSize, float smoothness = 0.1f) {
        // Target position for the camera to center on the player
        Vector2 targetPosition = playerPosition;

        // Smoothly interpolate the camera position towards the target
        Position = Vector2.Lerp(Position, targetPosition, smoothness);

        // Clamp the position to ensure the camera stays within bounds
        ClampPosition(terrainWidth, terrainHeight, tileSize);
    }
}