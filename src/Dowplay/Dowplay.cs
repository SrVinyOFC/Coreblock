using System;
using Microsoft.Xna.Framework;

namespace Coreblock {
    public static class DebugLine{
        public static void DrawRect(float x, float y, float w, float h, int thickness = 1, Color? color = null) {
            if (color == null) color = Color.White;

            Rectangle rect = new Rectangle(
                (int)MathF.Floor(x - w*8/2),
                (int)MathF.Floor(y - h*8/2),
                (int)MathF.Floor(w*8),
                (int)MathF.Floor(h*8)

            );
            Globals.SpriteBatch.Draw(
                Globals.PixelTexture,
                new Rectangle(
                    rect.X,
                    rect.Y,
                    rect.Width,
                    thickness
                ),
                color.Value
            );
            Globals.SpriteBatch.Draw(
                Globals.PixelTexture,
                new Rectangle(
                    rect.X,
                    rect.Bottom - thickness,
                    rect.Width,
                    thickness
                ),
                color.Value
            );
            Globals.SpriteBatch.Draw(
                Globals.PixelTexture,
                new Rectangle(
                    rect.X,
                    rect.Y,
                    thickness,
                    rect.Height
                ),
                color.Value
            );
            Globals.SpriteBatch.Draw(
                Globals.PixelTexture,
                new Rectangle(
                    rect.Right - thickness,
                    rect.Y,
                    thickness,
                    rect.Height
                ),
                color.Value
            );
        }
    }
}   
