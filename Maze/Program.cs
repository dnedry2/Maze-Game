using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Maze
{
    class Program
    {
        static Random r = new Random();
        static Dictionary<int, int> directionsX = new Dictionary<int, int>();
        static Dictionary<int, int> directionsY = new Dictionary<int, int>();
        static Dictionary<int, int> directionsOpp = new Dictionary<int, int>();
        static int[] cDirs = { 1, 2, 4, 8 }; //nsew

        static int l = 0;
        static int h = 0;

        static int[,] grid;

        const int STD_OUTPUT_HANDLE = -11;
        const uint ENABLE_VIRTUAL_TERMINAL_PROCESSING = 4;

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll")]
        static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

        [DllImport("kernel32.dll")]
        static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        private const int MF_BYCOMMAND = 0x00000000;
        public const int SC_CLOSE = 0xF060;
        public const int SC_MINIMIZE = 0xF020;
        public const int SC_MAXIMIZE = 0xF030;
        public const int SC_SIZE = 0xF000;

        [DllImport("user32.dll")]
        public static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);

        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();

        static void Main(string[] args)
        {
            Console.Title = "Maze";
            Console.CursorVisible = false;
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.SetWindowSize(120, 32);
            Console.SetBufferSize(120, 32);

            var handle = GetStdHandle(STD_OUTPUT_HANDLE);
            uint mode;
            GetConsoleMode(handle, out mode);
            mode |= ENABLE_VIRTUAL_TERMINAL_PROCESSING;
            SetConsoleMode(handle, mode);

            IntPtr handle2 = GetConsoleWindow();

            IntPtr sysMenu = GetSystemMenu(handle2, false);

            DeleteMenu(sysMenu, SC_MAXIMIZE, MF_BYCOMMAND);
            DeleteMenu(sysMenu, SC_SIZE, MF_BYCOMMAND);

            int width = 59;
                int height = 20;

                l = width;
                h = height;

                {
                    directionsX.Add(cDirs[2], 1);
                    directionsX.Add(cDirs[3], -1);
                    directionsX.Add(cDirs[0], 0);
                    directionsX.Add(cDirs[1], 0);
                    directionsY.Add(cDirs[2], 0);
                    directionsY.Add(cDirs[3], 0);
                    directionsY.Add(cDirs[0], -1);
                    directionsY.Add(cDirs[1], 1);
                    directionsOpp.Add(cDirs[2], cDirs[3]);
                    directionsOpp.Add(cDirs[3], cDirs[2]);
                    directionsOpp.Add(cDirs[0], cDirs[1]);
                    directionsOpp.Add(cDirs[1], cDirs[0]);
                }

            bool hc = false;

            while (true)
            {
                int cPosX = 0;
                int cPosY = 0;

                grid = new int[width, height];

                rb(0, 0);

                bool win = false;

                string cU = "\x1B[4mз\x1B[0m";
                string c = "з";
                string s = "\x1B[4m \x1B[0m";

                while (!win)
                {
                    StringBuilder stringBuilder = new StringBuilder();

                    stringBuilder.AppendLine(title);

                    for (int z = 1; z < width * 2 - 1; z++)
                        stringBuilder.Append("_");

                    stringBuilder.AppendLine();

                    for (int y = 0; y < height; y++)
                    {
                        stringBuilder.Append("│");

                        for (int x = 0; x < width; x++)
                        {
                            bool cS = (cPosX == x && cPosY == y);

                            if (!cS)
                                stringBuilder.Append(((grid[x, y] & 2) != 0) ? " " : s);
                            else
                                stringBuilder.Append(((grid[x, y] & 2) != 0) ? c : cU);
                            if ((grid[x, y] & 4) != 0)
                            {

                                stringBuilder.Append(((grid[x, y] & 2) != 0) ? " " : s);
                            }
                            else
                                stringBuilder.Append("│");

                        }
                        stringBuilder.Append("\n");
                    }

                    Console.Write(stringBuilder.ToString());

                    Console.SetCursorPosition(65, 9);
                    Console.Write("\x1B[31mhardcore\x1B[0m");

                    switch (Console.ReadKey().KeyChar)
                    {
                        case 'w':
                            if ((cPosX == 58 && cPosY-1 == -1) || cPosY -1 != -1)
                               if ((grid[cPosX, cPosY] & 1) != 0 || cPosY - 1 == -1)
                                    cPosY--;
                            break;
                        case 'a':
                            if (cPosX - 1 >= 0)
                                if ((grid[cPosX, cPosY] & 8) != 0)
                                    cPosX--;
                            break;
                        case 's':
                            if (cPosY + 1 < 20)
                                if ((grid[cPosX, cPosY] & 2) != 0)
                                    cPosY++;
                            break;
                        case 'd':
                            if (cPosX + 1 <= 58)
                                if ((grid[cPosX, cPosY] & 4) != 0)
                                    cPosX++;
                            break;
                        case 'h':
                            DeleteMenu(sysMenu, SC_CLOSE, MF_BYCOMMAND);
                            hc = true;
                            break;
                    }

                    win = cPosX == 58 && cPosY == -1;

                    Console.Clear();
                }
                Console.Clear();

                Console.Write("\n\n\n\n\n\n\n\n\n\n\n\n");
                Console.Write(winT);

                Console.ReadKey();
                if (hc) Environment.Exit(0);
                Console.Clear();
            }
        }
        static void rb(int currentX, int currentY)
        {
            int[] rcDirs = cDirs.OrderBy(x => r.Next()).ToArray();

            foreach (int d in rcDirs)
            {
                int nextX = currentX + directionsX[d];
                int nextY = currentY + directionsY[d];

                if ((nextY >= 0 && nextY <= h - 1) && (nextX >= 0 && nextX <= l - 1) && (grid[nextX, nextY] == 0))
                {
                    grid[currentX, currentY] |= d;
                    grid[nextX, nextY] |= directionsOpp[d];
                    rb(nextX, nextY);
                }
            }
        }
        static string title = @"                __/\\\\____________/\\\\_____/\\\\\\\\\_____/\\\\\\\\\\\\\\\__/\\\\\\\\\\\\\\\_        
                 _\/\\\\\\________/\\\\\\___/\\\\\\\\\\\\\__\////////////\\\__\/\\\///////////__       
                  _\/\\\//\\\____/\\\//\\\__/\\\/////////\\\___________/\\\/___\/\\\_____________      
                   _\/\\\\///\\\/\\\/_\/\\\_\/\\\_______\/\\\_________/\\\/_____\/\\\\\\\\\\\_____     
                    _\/\\\__\///\\\/___\/\\\_\/\\\\\\\\\\\\\\\_______/\\\/_______\/\\\///////______    
                     _\/\\\____\///_____\/\\\_\/\\\/////////\\\_____/\\\/_________\/\\\_____________   
                      _\/\\\_____________\/\\\_\/\\\_______\/\\\___/\\\/___________\/\\\_____________  
                       _\/\\\_____________\/\\\_\/\\\_______\/\\\__/\\\\\\\\\\\\\\\_\/\\\\\\\\\\\\\\\_ 
                        _\///______________\///__\///________\///__\///////////////__\///////////////__
                                                   Press 'h' for";
        static string winT = @"                                                   _       ___       __
                                                  | |     / (_)___  / /
                                                  | | /| / / / __ \/ / 
                                                  | |/ |/ / / / / /_/  
                                                  |__/|__/_/_/ /_(_)   ";
    }
}
