using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Coreblock;

[Flags]
public enum CollisionType {
    None = 0,
    Block = 1 << 0,
    Enemy = 1 << 1,
    Player = 1 << 2,
    Projectile = 1 << 3,
    All = ~0
}

    public enum SlopeType
    {
        None,
        SlopeUpRight,    // Diagonal para cima e direita (/)
        SlopeUpLeft,     // Diagonal para cima e esquerda (\)
        SlopeDownRight,  // Diagonal para baixo e direita (\)
        SlopeDownLeft    // Diagonal para baixo e esquerda (/)
    }

public static class PhysicsBodyManager {
    public static readonly List<UniversalPhysicsBody> allBodies = new();

    public static void Register(UniversalPhysicsBody body) {
        if (!allBodies.Contains(body)) allBodies.Add(body);
    }

    public static void Unregister(UniversalPhysicsBody body) {
        allBodies.Remove(body);
    }
}

public abstract class UniversalPhysicsBody : Sprite {
    public const int TileSize = 8;
    
    // Parâmetros de física
    public Vector2 velocity;
    public Vector2 acceleration;
    
    public bool autoJump = true;
    public float jumpForce = 20f;
    public bool inspect = false;
    
    public bool useGravity = false;
    private float gravity = 90f;
    private float maxFallSpeed = 50f;
    private float linearDrag = 300f;
    
    // Tamanho do corpo em unidades de mundo (1 = 1 tile)
    public Vector2 Offset = Vector2.Zero;
    
    public CollisionType collisionType = CollisionType.Block;
    public CollisionType collisionMask = CollisionType.Block | CollisionType.Enemy;
    
    public bool grounded { get; private set; } = true;
    public List<UniversalPhysicsBody> collidedEntities { get; private set; } = new();
    
    private int worldWidth;
    private int worldHeight;
    private int layer = 1;
    private Tile[,,] tiles;

    public UniversalPhysicsBody(Texture2D texture) : base(texture){
        
        tiles = GameManager.terrain.worldData;
        worldWidth = GameManager.terrain.WorldSize.X;
        worldHeight = GameManager.terrain.WorldSize.Y;

        PhysicsBodyManager.Register(this);
    }

    public void Dispose() {
        PhysicsBodyManager.Unregister(this);
    }

    public void AddForce(Vector2 force) => acceleration += force;

    public void OnJump(float force) {
        if (grounded) {
            velocity.Y = -force * 0.8f;
        }
    }

    public void Update(float deltaTime) {
        HandleAutoJump();
        ApplyPhysics(deltaTime);
        MoveAndCollide(deltaTime);
    }

    void ApplyPhysics(float deltaTime) {
        if (useGravity) acceleration.Y += gravity;
        
        if (inspect) velocity = acceleration * 4;
        else velocity += acceleration * deltaTime;

        if (MathF.Abs(acceleration.X) < 0.01f)
        {
            float drag = linearDrag * deltaTime;
            velocity.X = MathF.Sign(velocity.X) * MathF.Max(0f, MathF.Abs(velocity.X) - drag);
        }

        velocity.Y = MathF.Min(velocity.Y, maxFallSpeed);
        acceleration = Vector2.Zero;

    }

    void MoveAndCollide(float deltaTime) {
        grounded = false;
        collidedEntities.Clear();

        position.X = HorizontalCollide(position, velocity.X * 20f * deltaTime);
        position.Y = VerticalCollide(position, velocity.Y * 20f * deltaTime);
    }

    // Trata o salto automático sobre obstáculos baixos
    void HandleAutoJump() {
        // Verifica se o auto jump está habilitado e se há movimento significativo
        if (!autoJump || MathF.Abs(acceleration.X) < 0.01f) return;

        // Calcula a posição do pé do personagem
        int x = (int)MathF.Floor(position.X) + MathF.Sign(acceleration.X);
        int y = (int)MathF.Floor(position.Y) - (int)(size.Y / 2f) - 8;
        
        // Verifica se há um tile sólido à frente (mas não um slope)
        if (FootRaycast(position) && !HeadRaycast(position)) {
            
            // Só executa auto jump se o tile não for um slope
            if (!CheckCollision(new Vector2(x, y))) {
                position = Vector2.Lerp(position, new Vector2(position.X, y), 0.4f);
                velocity.X = acceleration.X;
            }
        }
    }

        // Obtém o tile em uma posição específica
    Tile GetTileAtPosition(Vector2 pos) {
        int tileX = (int)MathF.Floor(pos.X / TileSize);
        int tileY = (int)MathF.Floor(pos.Y / TileSize);
        
        if (tileX >= 0 && tileX < worldWidth && tileY >= 0 && tileY < worldHeight) {
            return tiles[tileX, tileY, layer];
        }
        
        return new Tile(); // Retorna um tile vazio se fora dos limites
    }

    public new  void Draw(){
        base.Draw();
        // int x = (int)MathF.Floor(position.X) + MathF.Sign(acceleration.X);
        // int y = (int)MathF.Floor(position.Y) - (int)(size.Y / 2f) -16; // Pé está na parte inferior
        // DebugLine.DrawRectHollow(x, y, size.X,size.Y, color: Color.Red);
    }

    float HorizontalCollide(Vector2 pos, float moveX) {
        float dir = MathF.Sign(moveX);
        float dist = MathF.Abs(moveX);

        while (dist > 0f) {
            float step = MathF.Min(dist, 0.05f);
            Vector2 check = pos + new Vector2(dir * step, 0);

            if (CheckCollision(check) && !inspect) {
                velocity.X = 0;
                return pos.X;
            }

            pos.X += dir * step;
            dist -= step;
        }

        return pos.X;
    }

    float VerticalCollide(Vector2 pos, float moveY) {
        float dir = MathF.Sign(moveY);
        float dist = MathF.Abs(moveY);

        while (dist > 0f) {
            float step = MathF.Min(dist, 0.05f);
            Vector2 check = pos + new Vector2(0, dir * step);

            if (CheckCollision(check) && !inspect) {
                if (dir > 0) grounded = true;
                velocity.Y = 0;
                return pos.Y;
            }

            pos.Y += dir * step;
            dist -= step;
        }

        return pos.Y;
    }

    bool CheckCollision(Vector2 pos) {
        // Aplica offset em unidades de mundo
        Vector2 checkPos = (pos + Offset )/ TileSize;
        return IsSolidCollision(checkPos) || IsEntityCollision(checkPos);
    }

    bool IsSolidCollision(Vector2 pos) {
        if ((collisionMask & CollisionType.Block) == 0) return false;

        Vector2 halfSize = size / 2f;

        // Converte para coordenadas de tile
        int minX = (int)MathF.Floor(pos.X - halfSize.X);
        int maxX = (int)MathF.Floor(pos.X + halfSize.X);
        int minY = (int)MathF.Floor(pos.Y - halfSize.Y);
        int maxY = (int)MathF.Floor(pos.Y + halfSize.Y);

        for (int x = minX; x <= maxX; x++) {
            for (int y = minY; y <= maxY; y++) {
                if (x < 0 || y < 0 || x >= worldWidth || y >= worldHeight)
                    continue;

                if (tiles[x, y, layer].solid)
                    return true;
            }
        }

        return false;
    }


    bool IsEntityCollision(Vector2 pos) {
        Vector2 min = pos - size / 2f;
        Vector2 max = pos + size / 2f;

        foreach (var body in PhysicsBodyManager.allBodies) {
            if (body == this || (collisionMask & body.collisionType) == 0) continue;

            Vector2 bodyPos = body.position + body.Offset / TileSize;
            Vector2 oMin = bodyPos - body.size / 2f;
            Vector2 oMax = bodyPos + body.size / 2f;

            if (min.X < oMax.X && max.X > oMin.X && min.Y < oMax.Y && max.Y > oMin.Y) {
                collidedEntities.Add(body);
                return true;
            }
        }

        return false;
    }

    bool FootRaycast(Vector2 pos) {

        int x = (int)MathF.Floor(pos.X / Globals.TileSize) + (int)(MathF.Sign(acceleration.X) *1.5f);
        int y = (int)MathF.Floor(pos.Y / Globals.TileSize) + (int)(size.Y / 2f); // Pé está na parte inferior

        if (x >= 0 && x < worldWidth && y >= 0 && y < worldHeight)
            return tiles[x, y, layer].solid;

        return false;
    }

    bool HeadRaycast(Vector2 pos) {
        int x = (int)MathF.Floor(pos.X / Globals.TileSize) + (int)(MathF.Sign(acceleration.X) *1.5f);
        int y = (int)MathF.Floor(pos.Y / Globals.TileSize) - (int)(size.Y / 2f) + 1;// Cabeça está na parte superior

        if (x >= 0 && x < worldWidth && y >= 0 && y < worldHeight)
            return tiles[x, y, layer].solid;

        return false;
    }
}
