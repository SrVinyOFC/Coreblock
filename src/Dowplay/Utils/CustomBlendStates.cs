// CustomBlendStates.cs
using Microsoft.Xna.Framework.Graphics;

namespace Coreblock
{
    public static class CustomBlendStates
    {
        public static BlendState Multiply { get; private set; }

        static CustomBlendStates()
        {
            Multiply = new BlendState
            {
                ColorSourceBlend = Blend.DestinationColor,
                ColorDestinationBlend = Blend.Zero,
                ColorBlendFunction = BlendFunction.Add,
                
                AlphaSourceBlend = Blend.DestinationAlpha,
                AlphaDestinationBlend = Blend.Zero,
                AlphaBlendFunction = BlendFunction.Add
            };
        }

        public static void Dispose()
        {
            Multiply?.Dispose();
        }
    }
}