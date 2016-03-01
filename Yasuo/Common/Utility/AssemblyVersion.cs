using Version = System.Version;

namespace Yasuo.Common.Utility
{
    using System;
    using System.Net;
    using System.Reflection;
    using System.Text.RegularExpressions;

    using LeagueSharp.Common;

    using Version = System.Version;

    public class AssemblyVersion
    {
        public Version LocalVersion;

        public bool Updated = false;

        public AssemblyVersion(Version assemblyVersion = null)
        {
            if (assemblyVersion == null)
            {
                assemblyVersion = Assembly.GetCallingAssembly().GetName().Version;
            }

            this.LocalVersion = assemblyVersion;
        }

        public void Check(string path)
        {
            try
            {
                CustomEvents.Game.OnGameLoad += delegate
                    {
                        var gitVersion = this.GetNewVersion(path);
                        var version = LocalVersion;

                        if (gitVersion.Equals(version))
                        {
                            return;
                        }

                        Updated = true;
                    };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public Version GetNewVersion(string path)
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
                            new Regex("AssemblyFileVersion\\((\"(.+?)\")\\)").Match(data).Groups[1].Value.Replace(
                                "\"",
                                ""));

                    return gitVersion;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return 0;
        }
    }
}



    


