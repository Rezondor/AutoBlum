using AutoBlum.Helpers;
using System.Diagnostics;
using System.Drawing;
using static AutoBlum.Helpers.WindowsActionHelper;
namespace AutoBlum;

internal class Program
{
   

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

        int repeatInSecond = 50;
        int delay = 1000 / repeatInSecond;

        for (int i = 0; i < ticketCount; i++)
        {
            Console.WriteLine($"Билет - {i + 1}| Начало");

            SearchPixelAndClick(leftX, topY, rightX, bottomY, ColorHelper.IsRed);

            await Task.Delay(20);

            Stopwatch sw = Stopwatch.StartNew();

            while (sw.Elapsed.TotalSeconds < 40)
            {
                SearchPixelAndClick(leftX, topY, rightX, bottomY, ColorHelper.IsGreen);
                await Task.Delay(delay);
            }

            sw.Stop();
            Console.WriteLine($"Билет - {i+1}| Окончание");

        }
    }

    private static bool SearchPixelAndClick(int leftX, int topY, int rightX, int bottomY, Func<Color, bool> isColor)
    {
        bool isBreakThisIter = false;
        var screenshotRow = TakeScreenshot(leftX, topY, rightX, bottomY);
        Bitmap screenshot = screenshotRow.bitmap;

        for (int y = screenshot.Height - 30; y >= 0; y-=5)
        {
            if (isBreakThisIter)
            {
                break;
            }

            for (int x = 0; x < screenshot.Width; x+=5)
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
        screenshotRow.bitmap.Dispose();
        screenshotRow.graphics.Dispose();
        return isBreakThisIter;
    }

    static (Bitmap bitmap, Graphics graphics) TakeScreenshot(int leftX, int topY, int rightX, int bottomY)
    {
        int width = rightX - leftX;
        int height = bottomY - topY;
        Bitmap bitmap = new Bitmap(width, height);
        Graphics g = Graphics.FromImage(bitmap);
        g.CopyFromScreen(leftX, topY, 0, 0, new Size(width, height));
        return (bitmap, g);
    }

    static void ClickAtPosition(int x, int y)
    {
        SetCursorPos(x, y);
        mouse_event(MOUSEEVENTF_LEFTDOWN, x, y, 0, 0);
        mouse_event(MOUSEEVENTF_LEFTUP, x, y, 0, 0);
    }
}
