using System;

namespace AppName.Web.Models
{
    public class Healthcheck
    {
        public string Version { get; set; }

        public DateTime ServerTimestamp { get; set; }

        public Healthcheck()
        {
            
        }

        public Healthcheck(string version, DateTime serverTimestamp)
        {
            Version = version;
            ServerTimestamp = serverTimestamp;
        }
    }
}