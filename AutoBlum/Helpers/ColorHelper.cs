using System.Drawing;

namespace AutoBlum.Helpers;

public static class ColorHelper
{
    public static bool IsGreen(Color color)
    {
        return
            color.R > 199 && color.R < 210 &&
            color.G > 210 && color.G < 230 &&
            color.B == 0;
    }

    public static bool IsRed(Color color)
    {
        return
            color.R > 240 && color.R < 256 &&
            color.G > 100 && color.G < 115 &&
            color.B > 110 && color.B < 140;
    }
}
