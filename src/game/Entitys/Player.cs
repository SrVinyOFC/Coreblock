using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Coreblock{

    public class Player : GameObject{


        public System.Numerics.Vector2 _minPosition; // Posição mínima
        public System.Numerics.Vector2 _maxPosition ; // Posição máxima


        public int Id { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }

        public static float SPEED = 5f; // Velocidade padrão
        public float maxSpeed = 8f;

        public Player(Texture2D texture) : base(texture)
        {
            collisionType = CollisionType.Player;
            size = new Vector2(1.8f, 2.8f);
            inspect = true;
            //Offset = new Vector2(0.9f, 1.4f); // Ajuste fino da hitbox
            //Offset = new Vector2(8, 4f);
        }



        public void SetBounds(Point mapSize){
            _minPosition = new System.Numerics.Vector2(Origin.X, Origin.Y);
            _maxPosition = new System.Numerics.Vector2((mapSize.X * (int)Globals.TileSize) - (int)Globals.TileSize +Origin.X, (mapSize.Y * (int)Globals.TileSize) - (int)Globals.TileSize +Origin.Y);
        }

        public override void Update(){

            useGravity = !inspect;
            autoJump = !inspect;
            Vector2 force = Vector2.Zero;
            var input = InputManager.GetDirectionalInput();

            if (!inspect)
            {
                if (!float.IsNaN(input.X) && MathF.Abs(velocity.X) < maxSpeed)
                {
                    force.X = input.X * SPEED; // Atualiza a posição do jogador com base na entrada

                    //velocity += input * SPEED * Globals.Time; // Atualiza a posição do jogador com base na entrada
                }

                AddForce(force);

                var Jump = InputManager.IsSingleKeyPress(Microsoft.Xna.Framework.Input.Keys.Space);

                if (Jump)
                {
                    OnJump(50);
                }
            }

            else
            {
                if (!float.IsNaN(input.X) || !float.IsNaN(input.Y))
                {
                    force = input * SPEED; // Atualiza a posição do jogador com base na entrada

                    //velocity += input * SPEED * Globals.Time; // Atualiza a posição do jogador com base na entrada
                }
                else
                {
                    velocity = Vector2.Zero;
                }

                AddForce(force);
            }

            base.Update();
            //position = Vector2.Clamp(position, _minPosition, _maxPosition);
        }
    }
}