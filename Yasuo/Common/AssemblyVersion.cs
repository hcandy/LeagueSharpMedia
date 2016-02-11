using System;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;

using LeagueSharp;
using LeagueSharp.Common;

using Version = System.Version;

namespace Yasuo.Common
{
    public class AssemblyVersion
    {
        public Version LocalVersion;

        public AssemblyVersion(Version assemblyVersion = null)
        {
            if (assemblyVersion == null)
            {
                assemblyVersion = Assembly.GetCallingAssembly().GetName().Version;
            }

            LocalVersion = assemblyVersion;
        }

        public void Check(string name, int displayTime, string path)
        {
            try
            {
                var gitVersion = GetNewVersion(path);
                var version = Convert.ToInt32(LocalVersion);

                var versionRaisedBy = gitVersion - version;

                if (versionRaisedBy == 0)
                {
                    return;
                }

                if (versionRaisedBy < 0)
                {
                    Notifications.AddNotification(
                        string.Format("[{0}] Recent change got Reverted: {1} => {2}!", name, version, gitVersion),
                        displayTime);
                }

                CustomEvents.Game.OnGameLoad += delegate
                    {
                        switch (versionRaisedBy)
                        {
                            case 1:
                                Notifications.AddNotification(
                                    string.Format(
                                        "[{0}] Bugfix/Small update available: {1} => {2}!", name, version, gitVersion),
                                    displayTime);
                                break;

                            case 10:
                                Notifications.AddNotification(
                                    string.Format("[{0}] Hotfix available: {1} => {2}!", name, version, gitVersion),
                                    displayTime);
                                break;
                            case 100:
                                Notifications.AddNotification(
                                    string.Format("[{0}] New Feature available: {1} => {2}!", name, version, gitVersion),
                                    displayTime);
                                break;
                            case 1000:
                                Notifications.AddNotification(
                                    string.Format("[{0}] Milestone reached: {1} => {2}!", name, version, gitVersion),
                                    displayTime);
                                break;
                            default:
                                Notifications.AddNotification(
                                    string.Format("[{0}] Update available: {1} => {2}!", name, version, gitVersion),
                                    displayTime);
                                break;
                        }
                    };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public int GetNewVersion(string path)
        {
            try
            {
                using (var client = new WebClient())
                {
                    var data =
                        client.DownloadString(
                            string.Format("https://raw.githubusercontent.com/{0}/Properties/AssemblyInfo.cs", path));

                    var gitVersion =
                        Version.Parse(
                            new Regex("AssemblyFileVersion\\((\"(.+?)\")\\)").Match(data).Groups[1].Value.Replace( "\"", ""));

                    return Convert.ToInt32(gitVersion);
                }

                }
            catch (Exception ex)
            {

                Console.WriteLine(ex);
            }
            return 0;
        }

        //public static int GetVersion()
        //{
            
        //    return Convert.ToInt32(Assembly.GetExecutingAssembly().GetName().Version);
        //}
    }
}



    


