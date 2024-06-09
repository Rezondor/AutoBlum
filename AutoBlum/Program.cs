using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
namespace AutoBlum;

internal class Program
{
    [DllImport("user32.dll")]
    static extern bool SetCursorPos(int X, int Y);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool GetCursorPos(out POINT lpPoint);

    [DllImport("user32.dll")]
    static extern void mouse_event(uint dwFlags, int dx, int dy, uint dwData, int dwExtraInfo);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    [DllImport("user32.dll")]
    private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int X;
        public int Y;
    }

    const uint MOUSEEVENTF_LEFTDOWN = 0x02;
    const uint MOUSEEVENTF_LEFTUP = 0x04;

    static async Task Main()
    {
        string windowName = "TelegramDesktop";

        IntPtr hWnd = FindWindow(null, windowName);

        if (hWnd == IntPtr.Zero)
        {
            Console.WriteLine("Окно не найдено.");
            return;
        }

        if (!GetWindowRect(hWnd, out RECT rect))
        {
            Console.WriteLine("Не удалось получить координаты окна.");
            return;
        }

        int leftX = rect.Left;
        int topY = rect.Top;
        int rightX = rect.Right;
        int bottomY = rect.Bottom;

        Console.Write("Укажите количество билетов: ");
        int ticketCount = Convert.ToInt32(Console.ReadLine());

        Console.WriteLine("Приложение готово к работе. Нажмите кнопку Play в Blum и любую кнопку в консоли.");

        Console.Read();

        int repeatInSecond = 8;
        int delay = 1000 / repeatInSecond;

        for (int i = 0; i < ticketCount; i++)
        {
            Console.WriteLine($"Билет - {i + 1}| Начало");

            SearchPixelAndClick(leftX, topY, rightX, bottomY, IsRed);
            await Task.Delay(20);

            Stopwatch sw = Stopwatch.StartNew();

            while (sw.Elapsed.TotalSeconds < 40)
            {
                SearchPixelAndClick(leftX, topY, rightX, bottomY, IsGreen);
                await Task.Delay(delay);
            }

            sw.Stop();
            Console.WriteLine($"Билет - {i+1}| Окончание");

        }
    }

    private static bool SearchPixelAndClick(int leftX, int topY, int rightX, int bottomY, Func<Color, bool> isColor)
    {
        bool isBreakThisIter = false;
        Bitmap screenshot = TakeScreenshot(leftX, topY, rightX, bottomY);

        for (int y = screenshot.Height - 30; y >= 0; y--)
        {
            if (isBreakThisIter)
            {
                break;
            }

            for (int x = 0; x < screenshot.Width; x++)
            {
                if (isBreakThisIter)
                {
                    break;
                }

                Color pixelColor = screenshot.GetPixel(x, y);

                if (isColor(pixelColor))
                {
                    int screenX = leftX + x;
                    int screenY = topY + y;
                    ClickAtPosition(screenX, screenY);
                    isBreakThisIter = true;
                }
            }
        }

        return isBreakThisIter;
    }

    static Bitmap TakeScreenshot(int leftX, int topY, int rightX, int bottomY)
    {
        int width = rightX - leftX;
        int height = bottomY - topY;
        Bitmap bitmap = new Bitmap(width, height);
        Graphics g = Graphics.FromImage(bitmap);
        g.CopyFromScreen(leftX, topY, 0, 0, new Size(width, height));
        return bitmap;
    }

    static bool IsGreen(Color color)
    {
        return
            color.R > 199 && color.R < 210 &&
            color.G > 210 && color.G < 230 &&
            color.B == 0;
    }

    static bool IsRed(Color color)
    {
        return
            color.R > 240 && color.R < 256 &&
            color.G > 100 && color.G < 115 &&
            color.B > 110 && color.B < 140 ;
    }

    static void ClickAtPosition(int x, int y)
    {
        SetCursorPos(x, y);
        mouse_event(MOUSEEVENTF_LEFTDOWN, x, y, 0, 0);
        mouse_event(MOUSEEVENTF_LEFTUP, x, y, 0, 0);
    }
}
