using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hackland.AccessControl.Shared
{
    public static class Settings
    {
        public static bool IsRunningInDocker
        {
            get
            {
                return Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";
            }
        }

        public static bool UseSqlServer => false;

    }
}
