using System;
using System.IO;
using System.Management;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Text;
using System.Runtime.InteropServices;

class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        if (args.Length != 1) {
            Console.WriteLine("usage: path-kun.exe filepath");
            return;
        }
        
        try {
            DriveMap map = new DriveMap();
            SharablePath sharablePath = new SharablePath(map, args[0]);

            string result = "<" + sharablePath.ToString() + ">";
            Console.WriteLine(result);
            Clipboard.SetText(result);
        } catch (Exception e) {
            Console.WriteLine(e.ToString());
        }
    }
}

class SharablePath
{
    DriveMap driveMap;
    string originalPath;
    string sharablePath;
    
    public SharablePath(DriveMap map, string originalPath)
    {
        this.driveMap = map;
        this.originalPath = originalPath;
        this.sharablePath = this.originalPath;
        
        extractPathRecursively();
    }

    private void extractPathRecursively()
    {
        Console.WriteLine(this.sharablePath);
        for (int maxRepeat = 26; maxRepeat > 0; maxRepeat--) {
            if (!extractPath())
                break;
            
            Console.WriteLine(" -> " + this.sharablePath);
        }
    }

    private bool extractPath()
    {
        MatchCollection mc = Regex.Matches(this.sharablePath, @"^([a-zA-Z]:)\\?(.*)$");
        
        if (mc.Count != 0) {
            string driveName = mc[0].Groups[1].Value;
            string restPath = mc[0].Groups[2].Value;
            
            if (this.driveMap.hasNetworkDrive(driveName)) {
                string networkPath = this.driveMap.getPath(driveName);
                this.sharablePath = System.IO.Path.Combine(networkPath, restPath);

                return true;
            }
        }
        return false;
    }

    override public string ToString()
    {
        return this.sharablePath;
    }
}

class MountedPath
{
    public string driveName;
    public string path;
    
    public MountedPath(string driveName, string path)
    {
        this.driveName = driveName;
        this.path = path;
    }

    public bool isNetworkPath()
    {
        if (this.path.StartsWith(@"\\"))
            return true;
        if (Regex.IsMatch(this.path, @"^([a-zA-Z]:)"))
            return true;
        return false;
    }
}

class DriveMap
{
    Dictionary<string, MountedPath> deviceMap;
    
    public DriveMap()
    {
        this.deviceMap = new Dictionary<string, MountedPath>();
        this.loadDeviceMap();
    }

    private void loadDeviceMap()
    {
        using (ManagementObjectSearcher searcher = new ManagementObjectSearcher())
        {
            searcher.Query = new ObjectQuery("SELECT * FROM Win32_LogicalDisk");
            using (ManagementObjectCollection drives = searcher.Get())
            {
                foreach (ManagementObject mo in drives)
                {
                    using (mo)
                    {
                        if (mo["DriveType"].ToString() != "4")
                            continue;

                        string driveName = (mo["Name"] ?? string.Empty).ToString();
                        string path = (mo["ProviderName"] ?? string.Empty).ToString();
                        if (path.Length == 0)
                            path = loadDosDrivePath(driveName);
                        this.deviceMap.Add(driveName.ToUpper(), new MountedPath(driveName, path));
                    }
                }
            }
        }
    }
    
    // subst command case
    private string loadDosDrivePath(string driveName)
    {                    
        StringBuilder pathInformation = new StringBuilder(250);
        QueryDosDevice(driveName, pathInformation, 250);
                            
        // Strip the \??\ prefix.
        return pathInformation.ToString().Remove(0, 4);
    }
    
    public bool hasNetworkDrive(string driveName)
    {
        driveName = driveName.ToUpper();
        if (!this.deviceMap.ContainsKey(driveName))
            return false;

        return this.deviceMap[driveName].isNetworkPath();
    }

    public string getPath(string driveName)
    {
        return this.deviceMap[driveName.ToUpper()].path;
    }

    public void dump()
    {
        foreach (KeyValuePair<string, MountedPath> pair in this.deviceMap) {   
            Console.WriteLine(string.Format("{0}={1}", pair.Key, pair.Value.path));
        }
    }
    
    [DllImport("kernel32.dll")]
    private static extern uint QueryDosDevice(string lpDeviceName, StringBuilder lpTargetPath, int ucchMax);
}
