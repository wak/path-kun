﻿using System;
using System.Windows.Forms;
using PathKunLib;

namespace PathKunBasic
{
    class PathKunBasic
    {
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("usage: path-kun.exe filepath");
                return;
            }

            try
            {
                DriveMap map = new DriveMap();
                SharablePath sharablePath = new SharablePath(map, args[0]);

                string result = string.Format("<{0}>", sharablePath.ToString());
                Console.WriteLine(result);
                Clipboard.SetText(result);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
