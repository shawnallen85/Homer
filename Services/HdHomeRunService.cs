using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Timers;
using System.Xml;
using Homer.Models;
using Mono.Nat;
using RestSharp;
using RestSharp.Serializers.SystemTextJson;

namespace Homer.Services
{
    public class HdHomeRunService
    {
        private Timer _stopTimer;
        
        public HdHomeRunService()
        {
            Hosts = new ObservableCollection<HdHomeRunHost>();
        }
        
        private void UnknownDeviceFound(object sender, DeviceEventUnknownArgs e)
        {
            if (string.IsNullOrEmpty(e?.Data))
                return;
            using TextReader textReader = new StringReader(e.Data);
            string line, location = null;
            while ((line = textReader.ReadLine()) != null)
            {
                if (!line.StartsWith("Location: ")) continue;
                location = line.Substring(10);
                break;
            }
            if (string.IsNullOrEmpty(location))
                return;
            Uri uri = new Uri(location);
            HdHomeRunHost host = new HdHomeRunHost();
            host.RestClient = new RestClient($"{uri.Scheme}://{uri.Host}:{uri.Port}");
            host.RestClient.UseSystemTextJson();
            RestRequest deviceRequest = new RestRequest(uri.AbsolutePath, DataFormat.Xml);
            IRestResponse deviceResponse = host.RestClient.Get(deviceRequest);
            if (!deviceResponse.IsSuccessful)
                return;
            const string regexPattern = "HDHomeRun/(\\d+).(\\d+)";
            Regex regex = new Regex(regexPattern);
            if (!regex.IsMatch(deviceResponse.Server))
                return;
            XmlDocument deviceDocument = new XmlDocument();
            deviceDocument.LoadXml(deviceResponse.Content);
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(deviceDocument.NameTable);  
            nsmgr.AddNamespace("upnp", "urn:schemas-upnp-org:device-1-0");  
            XmlNode friendlyNameNode = deviceDocument.SelectSingleNode("//upnp:device//upnp:friendlyName", nsmgr);
            if (friendlyNameNode != null)
                host.FriendlyName = friendlyNameNode.InnerText;
            if (Hosts.Contains(host))
                return;
            Hosts.Add(host);
        }

        public void StartScan()
        {
            if (_stopTimer != null)
            {
                _stopTimer.Enabled = false;
                _stopTimer.Elapsed -= StopTimerOnElapsed;
                _stopTimer.Dispose();
            }
            _stopTimer = new Timer(60000);
            _stopTimer.Elapsed += StopTimerOnElapsed;
            _stopTimer.AutoReset = false;
            _stopTimer.Enabled = true;
            NatUtility.UnknownDeviceFound += UnknownDeviceFound;
            NatUtility.StartDiscovery(NatProtocol.Upnp);
        }

        private void StopTimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            StopScan();
        }

        public void StopScan()
        {
            if (_stopTimer != null)
            {
                _stopTimer.Enabled = false;
                _stopTimer.Elapsed -= StopTimerOnElapsed;
                _stopTimer.Dispose();
            }
            NatUtility.StopDiscovery();
            NatUtility.UnknownDeviceFound -= UnknownDeviceFound;
        }
        
        public ObservableCollection<HdHomeRunHost> Hosts { get; }
    }
}