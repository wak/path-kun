using System;
using System.IO;
using System.Management;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;

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
    
    public SharablePath(DriveMap map, string originalPath)
    {
        this.driveMap = map;
        this.originalPath = originalPath;
    }

    private string convertToSharablePath()
    {
        MatchCollection mc = Regex.Matches(this.originalPath, @"^([a-zA-Z]:)\\?(.*)$");

        if (mc.Count == 0) {
            return this.originalPath;
        }
        
        string driveName = mc[0].Groups[1].Value;
        string restPath = mc[0].Groups[2].Value;
        if (!this.driveMap.hasNetworkDrive(driveName)) {
            return this.originalPath;
        }

        string networkPath = this.driveMap.getPath(driveName);
        return System.IO.Path.Combine(networkPath, restPath);
    }

    override public string ToString()
    {
        return convertToSharablePath();
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
        return this.path.StartsWith(@"\\");
    }
}

class DriveMap
{
    Dictionary<string, MountedPath> drivePathMap;
    
    public DriveMap()
    {
        this.drivePathMap = new Dictionary<string, MountedPath>();
        this.loadMountedPathMap();
    }

    void loadMountedPathMap()
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
                        string driveName = (mo["Name"] ?? string.Empty).ToString();
                        string provider = (mo["ProviderName"] ?? string.Empty).ToString();

                        if (provider.Length != 0) {
                            this.drivePathMap.Add(driveName.ToUpper(), new MountedPath(driveName, provider));
                        }
                    }
                }
            }
        }
    }

    public bool hasNetworkDrive(string driveName)
    {
        driveName = driveName.ToUpper();
        
        if (!this.drivePathMap.ContainsKey(driveName)) {
            return false;
        }

        return this.drivePathMap[driveName].isNetworkPath();
    }

    public string getPath(string driveName)
    {
        return this.drivePathMap[driveName.ToUpper()].path;
    }

    public void dump()
    {
        foreach (KeyValuePair<string, MountedPath> pair in this.drivePathMap) {   
            Console.WriteLine(string.Format("{0}={1}", pair.Key, pair.Value));
        }
    }
}
