using System;
using System.IO;
using System.Windows.Forms;

class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        if (args.Length != 1) {
            Console.WriteLine("usage: path-kun-pathonly.exe filepath");
            return;
        }
        
        try {
            DriveMap map = new DriveMap();
            SharablePath sharablePath = new SharablePath(map, args[0]);

            string result = sharablePath.ToString();
            Console.WriteLine(result);
            Clipboard.SetText(result);
        } catch (Exception e) {
            Console.WriteLine(e.ToString());
        }
    }
}
