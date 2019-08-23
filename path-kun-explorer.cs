using System;
using System.IO;
using System.Windows.Forms;

class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        if (args.Length != 1) {
            Console.WriteLine("usage: path-kun-explorer.exe filepath");
            return;
        }
        
        try {
            DriveMap map = new DriveMap();
            SharablePath sharablePath = new SharablePath(map, args[0]);

            System.Diagnostics.Process.Start(
                "EXPLORER.EXE", String.Format(@"/select,""{0}""", sharablePath.ToString())
            );
        } catch (Exception e) {
            Console.WriteLine(e.ToString());
        }
    }
}
