using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Coreblock
{
    public class Sprite(Texture2D texture, Rectangle? sourceRectangle = null, Color? color = null){
        private Texture2D _texture = texture;
        public Rectangle? rect = sourceRectangle; // Define a região da textura
        public Vector2 position;
        public Vector2 size = new(1, 1);
        public Vector2 Origin { get; protected set; } // Define o ponto de origem do sprite;
        private float _rotation ;
        // Removed unused private member '_scale'
        private Color _color = color ?? Color.White; // Define a cor do sprite
        public void Draw()
        {
            // Update color based on light data
            _color = GameManager.lightManager.GetLightData((int)(position.X / 8), (int)(position.Y / 8));

            // Determine source rectangle and origin
            Rectangle? sourceRect = rect;
            Vector2 origin = sourceRect.HasValue
            ? new Vector2(rect.Value.Width / 2f, rect.Value.Height / 2f)
            : new Vector2(_texture.Width / 2f, _texture.Height / 2f);

            Globals.SpriteBatch.Draw(
            _texture,
            position,
            sourceRect,
            _color,
            _rotation,
            origin,
            1f,
            SpriteEffects.None,
            0f
            );
        }

        public void SetPosition(Vector2 position){
            this.position = position;
        }

        public void SetTexture(Texture2D texture)
        {
            _texture = texture;
            Origin = rect.HasValue
                ? new Vector2(rect.Value.Width / 2f, rect.Value.Height / 2f)
                : new Vector2(_texture.Width / 2f, _texture.Height / 2f);
        }

        public void SetRotation(float rotation)
        {
            _rotation = rotation;
        }

        public void SetColor(Color color)
        {
            _color = color;
        }

        public void SetSourceRectangle(Rectangle? sourceRectangle)
        {
            rect = sourceRectangle;
            Origin = sourceRectangle.HasValue
                ? new Vector2(sourceRectangle.Value.Width / 2f, sourceRectangle.Value.Height / 2f)
                : new Vector2(_texture.Width / 2f, _texture.Height / 2f);
        }
    }
}