using System;
using RestSharp;

namespace Homer.Models
{
    public class HdHomeRunHost
    {
        public RestClient RestClient { get; set; }
        public string FriendlyName { get; set; }

        public override bool Equals(object obj)
        {
            var hdHomeRun = obj as HdHomeRunHost;
            if (hdHomeRun == null)
                return false;
            return hdHomeRun.FriendlyName == FriendlyName;
        }

        protected bool Equals(HdHomeRunHost other)
        {
            return Equals(RestClient?.BaseHost, other.RestClient?.BaseHost) && FriendlyName == other.FriendlyName;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(RestClient, FriendlyName);
        }
    }
}