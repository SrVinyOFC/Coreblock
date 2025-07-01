using System.Numerics;

namespace Coreblock;

public static class TileCoords
{
    public enum TileCoordsType
    {
        Top,
        Bottom,
        Left,
        Right,

        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight,

        LeftBottomRight,
        TopBottomRight,
        TopLeftRight,
        TopLeftBottom,

        TopBottom,
        LeftRight,

        None,
        All
    }

    public static Vector2 GetCoordLiquid(int level, int variant)
    {
        
        return level switch
        {
            1 => Level1[variant],
            2 => Level2[variant],
            3 => Level3[variant],
            4 => Level4[variant],
            5 => Level5[variant],
            6 => Level6[variant],
            7 => Level7[variant],
            8 => Level8[variant],
            _ => throw new System.NotImplementedException()
        };
    }

    public static Vector2 getCoord(TileCoordsType type, int variant)
    {
        return type switch
        {
            TileCoordsType.Top => TopCord[variant],
            TileCoordsType.Bottom => BottomCord[variant],
            TileCoordsType.Left => LeftCord[variant],
            TileCoordsType.Right => RightCord[variant],

            TileCoordsType.TopLeft => TopLeftCord[variant],
            TileCoordsType.TopRight => TopRightCord[variant],
            TileCoordsType.BottomLeft => BottomLeftCord[variant],
            TileCoordsType.BottomRight => BottomRightCord[variant],

            TileCoordsType.LeftBottomRight => LeftBottomRightCord[variant],
            TileCoordsType.TopBottomRight => TopBottomRightCord[variant],
            TileCoordsType.TopLeftRight => TopLeftRightCord[variant],
            TileCoordsType.TopLeftBottom => TopLeftBottomCord[variant],

            TileCoordsType.TopBottom => TopBottom[variant],
            TileCoordsType.LeftRight => LeftRight[variant],

            TileCoordsType.None => NoneCord[variant],
            TileCoordsType.All => AllCord[variant],
            _ => throw new System.NotImplementedException()
        };
    }

    public static Vector2[] TopCord = {
        new Vector2(1, 0), // Variante 1
        new Vector2(2, 0), // Variante 2
        new Vector2(3, 0), // Variante 3
    };

    public static Vector2[] BottomCord = {
        new Vector2(1, 2), // Variante 1
        new Vector2(2, 2), // Variante 2
        new Vector2(3, 2), // Variante 3
    };

    public static Vector2[] LeftCord = {
        new Vector2(0, 0), // Variante 1
        new Vector2(0, 1), // Variante 2
        new Vector2(0, 2), // Variante 3
    };

    public static Vector2[] RightCord = {
        new Vector2(4, 0), // Variante 1
        new Vector2(4, 1), // Variante 2
        new Vector2(4, 2), // Variante 3
    };

    public static Vector2[] TopLeftCord = {
        new Vector2(1, 4), // Variante 1
        new Vector2(3, 4), // Variante 2
        new Vector2(5, 4), // Variante 3
    };

    public static Vector2[] TopRightCord = {
        new Vector2(0, 4), // Variante 1
        new Vector2(2, 4), // Variante 2
        new Vector2(4, 4), // Variante 3
    };

    public static Vector2[] BottomLeftCord = {
        new Vector2(1, 3), // Variante 1
        new Vector2(3, 3), // Variante 2
        new Vector2(5, 3), // Variante 3
    };

    public static Vector2[] BottomRightCord = {
        new Vector2(0, 3), // Variante 1
        new Vector2(2, 3), // Variante 2
        new Vector2(4, 3), // Variante 3
    };

    public static Vector2[] AllCord = {
        new Vector2(1, 1), // Variante 1
        new Vector2(2, 1), // Variante 2
        new Vector2(3, 1), // Variante 3
    };

    public static Vector2[] LeftBottomRightCord = {
        new Vector2(6, 0), // Variante 1
        new Vector2(7, 0), // Variante 2
        new Vector2(8, 0), // Variante 3
    };

    public static Vector2[] TopBottomRightCord = {
        new Vector2(9, 0), // Variante 1
        new Vector2(9, 1), // Variante 2
        new Vector2(9, 2), // Variante 3
    };

    public static Vector2[] TopLeftRightCord = {
        new Vector2(6, 3), // Variante 1
        new Vector2(7, 3), // Variante 2
        new Vector2(8, 3), // Variante 3
    };


    public static Vector2[] TopLeftBottomCord = {
        new Vector2(12, 0), // Variante 1
        new Vector2(12, 1), // Variante 2
        new Vector2(12, 2), // Variante 3
    };

    public static Vector2[] NoneCord = {
        new Vector2(9, 3), // Variante 1
        new Vector2(10, 3), // Variante 2
        new Vector2(11, 3), // Variante 3
    };

    public static Vector2[] TopBottom = {
        new Vector2(6, 4), // Variante 1
        new Vector2(7, 4), // Variante 2
        new Vector2(8, 4), // Variante 3
    };

    public static Vector2[] LeftRight = {
        new Vector2(5, 0), // Variante 1
        new Vector2(5, 1), // Variante 2
        new Vector2(5, 2), // Variante 3
    };

    public static Vector2[] Level1 = {
        new Vector2(1, 0), // Variante 1
        new Vector2(2, 0), // Variante 2
        new Vector2(3, 0), // Variante 3
    };

    public static Vector2[] Level2 = {
        new Vector2(4, 0), // Variante 1
        new Vector2(5, 0), // Variante 2
        new Vector2(6, 0), // Variante 3
    };

    public static Vector2[] Level3 = {
        new Vector2(7, 0), // Variante 1
        new Vector2(8, 0), // Variante 2
        new Vector2(9, 0), // Variante 3
    };

    public static Vector2[] Level4 = {
        new Vector2(10, 0), // Variante 1
        new Vector2(11, 0), // Variante 2
        new Vector2(12, 0), // Variante 3
    };

    public static Vector2[] Level5 = {
        new Vector2(0, 1), // Variante 1
        new Vector2(1, 1), // Variante 2
        new Vector2(2, 1), // Variante 3
    };

    public static Vector2[] Level6 = {
        new Vector2(3, 1), // Variante 1
        new Vector2(4, 1), // Variante 2
        new Vector2(5, 1), // Variante 3
    };

    public static Vector2[] Level7 = {
        new Vector2(6, 1), // Variante 1
        new Vector2(7, 1), // Variante 2
        new Vector2(8, 1), // Variante 3
    };

    public static Vector2[] Level8 = {
        new Vector2(9, 1), // Variante 1
        new Vector2(10, 1), // Variante 2
        new Vector2(11, 1), // Variante 3
    };
    public static Vector2[] Level9 = {
        new Vector2(0, 0), // Variante 1
        new Vector2(0, 0), // Variante 2
        new Vector2(0, 0), // Variante 3

    };
}