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
    
    public SharablePath(DriveMap map, string originalPath)
    {
        this.driveMap = map;
        this.originalPath = originalPath;
    }

    private string convertToSharablePath()
    {
        int maxRepeat = 26;
        string currentPath = this.originalPath;

        Console.WriteLine(currentPath);
        while (maxRepeat-- > 0) {
            MatchCollection mc = Regex.Matches(currentPath, @"^([a-zA-Z]:)\\?(.*)$");

            if (mc.Count == 0)
                break;
        
            string driveName = mc[0].Groups[1].Value;
            string restPath = mc[0].Groups[2].Value;
            if (!this.driveMap.hasNetworkDrive(driveName))
                return currentPath;

            string networkPath = this.driveMap.getPath(driveName);
            string nextPath = System.IO.Path.Combine(networkPath, restPath);

            Console.WriteLine(" -> " + nextPath);
            
            if (currentPath == nextPath)
                break;

            currentPath = nextPath;
        }
        return currentPath;
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
        if (this.path.StartsWith(@"\\"))
            return true;
        if (Regex.IsMatch(this.path, @"^([a-zA-Z]:)"))
            return true;
        return false;
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
                        if (mo["DriveType"].ToString() != "4")
                            continue;

                        string driveName = (mo["Name"] ?? string.Empty).ToString();
                        string path = (mo["ProviderName"] ?? string.Empty).ToString();
                        if (path.Length == 0) {
                            // subst command case
                            
                            StringBuilder pathInformation = new StringBuilder(250);
                            QueryDosDevice(driveName, pathInformation, 250);
                            
                            // Strip the \??\ prefix.
                            path = pathInformation.ToString().Remove(0, 4);
                        }
                        this.drivePathMap.Add(driveName.ToUpper(), new MountedPath(driveName, path));
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
            Console.WriteLine(string.Format("{0}={1}", pair.Key, pair.Value.path));
        }
    }
    
    [DllImport("kernel32.dll")]
    static extern uint QueryDosDevice(string lpDeviceName, StringBuilder lpTargetPath, int ucchMax);
}
