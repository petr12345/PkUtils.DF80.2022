// Ignore Spelling: Utils, Taskbar
//
using System;
using System.Drawing;
using PK.PkUtils.Consoles;

namespace PK.TestConsoleAppWithIcon;

public class Program
{
    public static void Main()
    {
        // Example resource byte arrays (replace with actual resource data)
        byte[][] resourceIcons =
        [
            Resources.nav_refresh_blue,
            Resources.nav_refresh_green,
            Resources.nav_refresh_red,
            Resources.nav_refresh_yellow
        ];

        Console.WriteLine("Press any key to cycle icons (Console window & Taskbar), or ESC to exit.");

        for (int currentIndex = 0; ;)
        {
            SetIcons(resourceIcons[currentIndex]);
            ConsoleKeyInfo key = Console.ReadKey(true);

            if (key.Key == ConsoleKey.Escape) break;
            currentIndex = (currentIndex + 1) % resourceIcons.Length;
        }
    }

    private static void SetIcons(byte[] resourceData)
    {
        Icon icon = ConsoleIconManager.CreateIcon(resourceData);
        ConsoleIconManager.SetConsoleWindowIcon(icon);
        ConsoleIconManager.SetTaskbarIcon(icon);
    }
}