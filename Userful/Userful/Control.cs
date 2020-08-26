using System;
using System.Text;
using System.Net.Sockets;
using Crestron.SimplSharp;
using Crestron.SimplSharp.CrestronIO;
using Crestron.SimplSharp.Net.Http;
using Newtonsoft.Json;                                 // For Basic SIMPL# Classes


namespace Userful
{
    public class UserfulControl
    {

        /// <summary>
        /// SIMPL+ can only execute the default constructor. If you have variables that require initialization, please
        /// use an Initialize method
        /// </summary>
        /// 
        //public event TriggerUpdateZonesHandler UpdateZones;
        //public delegate void TriggerUpdateZonesHandler(object o, EventArgs e);

        public User usr;
        private string cookie;
        public string hostAddress;
        public ushort debug {set; get;}
        public UserfulSource sources;
        private UserfulZone zones;
        private UserfulPreset presets;
        //public UserfulDisplay dispalys;

        public event TriggerUpdateSourcesInUseHandler UpdateSourcesInUse;
        public delegate void TriggerUpdateSourcesInUseHandler(object o, EventArgs e);


        public UserfulControl()
        {
            usr = new User();
            hostAddress = "http://localhost:9000";
        }

        //private void updateZones(object o)

        public void getSysInfo()
        {
            sources = getSourceInfo();
            zones = getZoneInfo();
            presets = getPresetInfo();
        }



        private string getSessionId() 
        {
            using (var httpClient = new HttpClient())
            {
                HttpClientRequest req = new HttpClientRequest();
                HttpClientResponse resp;
                req.Url = new UrlParser(hostAddress + "/api/session");
                req.RequestType = RequestType.Post;
                var auth = JsonConvert.SerializeObject(usr);
                var byteContent = Encoding.ASCII.GetBytes(auth);
                req.ContentBytes = byteContent;
                resp = httpClient.Dispatch(req);
                return handleGetSessionId(resp);
            }
        }
        private string handleGetSessionId(HttpClientResponse response)
        {
            try
            {
                if (debug == 1)
                {
                    CrestronConsole.PrintLine("Get Session Id - Response Code:{0}, Response Conetent String: {1}, Response Url: {2}", response.Code, response.ContentString, response.ResponseUrl);
                }
                return response.Header.GetHeaderValue("Set-Cookie");
            }
            catch (Exception e)
            {
                CrestronConsole.PrintLine("Cookie Not Found, Message: {0}", e.Message);
                return null;
            }
        }

        public UserfulSource getSourceInfo()
        {
            using (var httpClient = new HttpClient())
            {
                cookie = getSessionId();
                HttpClientRequest req = new HttpClientRequest();
                HttpClientResponse resp;
                req.Url = new UrlParser(hostAddress + "/api/sources");
                req.RequestType = RequestType.Get;
                req.Header.SetHeaderValue("cookie", cookie);
                resp = httpClient.Dispatch(req);
                var sourceInfo = handleGetSourceInfo(resp);
                if (debug == 1)
                {
                    sourceInfo.sources.ForEach(x =>
                    {
                        CrestronConsole.PrintLine(x.sourceName);
                    });
                }
                return sourceInfo;
            }
        }

        private UserfulSource handleGetSourceInfo(HttpClientResponse response)
        {
            string s;
            s = response.ContentString;
            return JsonConvert.DeserializeObject<UserfulSource>(s);
        }


        public UserfulZone getZoneInfo()
        {
            using (var httpClient = new HttpClient())
            {
                cookie = getSessionId();
                HttpClientRequest req = new HttpClientRequest();
                HttpClientResponse resp;
                req.Url = new UrlParser(hostAddress + "/api/zones");
                req.RequestType = RequestType.Get;
                req.Header.SetHeaderValue("cookie", cookie);
                resp = httpClient.Dispatch(req);
                var zoneInfo = handleGetZone(resp);
                if (debug == 1)
                {
                    zoneInfo.zones.ForEach(x =>
                    {
                        CrestronConsole.PrintLine(x.zoneName);
                    });
                }
                return zoneInfo;
            }
        }
        private UserfulZone handleGetZone(HttpClientResponse response)
        {
            string s;
            s = response.ContentString;
            return JsonConvert.DeserializeObject<UserfulZone>(s);
        }

        public UserfulPreset getPresetInfo()
        {
            using (var httpClient = new HttpClient())
            {
                cookie = getSessionId();
                HttpClientRequest req = new HttpClientRequest();
                HttpClientResponse resp;
                req.Url = new UrlParser(hostAddress + "/api/presets");
                req.RequestType = RequestType.Get;
                req.Header.SetHeaderValue("cookie", cookie);
                resp = httpClient.Dispatch(req);
                var presetInfo = handleGetPreset(resp);
                if (debug == 1)
                {
                    presetInfo.presets.ForEach(x =>
                    {
                        CrestronConsole.PrintLine(x.name);
                    });
                }
                return presetInfo;
            }
        }
        private UserfulPreset handleGetPreset(HttpClientResponse response)
        {
            string s;
            s = response.ContentString;
            return JsonConvert.DeserializeObject<UserfulPreset>(s);
        }

        public void setPreset(string name)
        {
            if (presets.presets.Exists(x => x.name.Equals(name)))
            {
                sources.setAllNotInUse();
                using (var httpClient = new HttpClient())
                {
                    cookie = getSessionId();
                    HttpClientRequest req = new HttpClientRequest();
                    HttpClientResponse resp;
                    req.Url = new UrlParser(hostAddress + "/api/presets/byname/" + presets.presets.Find(x => x.name.Equals(name)).name + "/switch");
                    req.RequestType = RequestType.Put;
                    req.Header.SetHeaderValue("cookie", cookie);
                    resp = httpClient.Dispatch(req);
                    var h = handlePutPreset(resp);
                    if (debug == 1) { CrestronConsole.PrintLine("id:{0}, name:{1}, playing:{2}", h.id, h.name, h.playing); }
                }
            }
            else
            {
                CrestronConsole.PrintLine("Userful Module: Preset Named {0} doesnt exist!", name);
                CrestronConsole.PrintLine("Available presets:");
                presets.presets.ForEach(x => CrestronConsole.PrintLine( "Name: {0), Id: {1}", x.name, x.id));
            }
            getSysInfo();

        }
        private PresetItem handlePutPreset(HttpClientResponse response)
        {
            string s;
            s = response.ContentString;
            return JsonConvert.DeserializeObject<PresetItem>(s);
        }


        public void setZoneSources(string zone, string source)
        {
            if(zones.zones.Exists(x => x.zoneName == zone) && sources.sources.Exists(x => x.sourceName == source))
            {
                //See if the old routed source was in the list and set it to InUse False
                if (debug == 1) { CrestronConsole.PrintLine("SetZoneSources: Checking if Old Zone Source is in list then setting it to false"); }
                if (sources.sources.Exists(x => x.sourceId == zones.zones.Find(y => y.zoneName == zone).playingSourceId))
                {
                    if (debug == 1) { CrestronConsole.PrintLine("Old Source found: {0}", sources.sources.Find(x => x.sourceId == zones.zones.Find(y => y.zoneName == zone).playingSourceId).sourceName); }
                    sources.sources.Find(x => x.sourceId == zones.zones.Find(y => y.zoneName == zone).playingSourceId).setInUse(false);
                }
                else
                {
                    CrestronConsole.PrintLine("Userful: Setting Zone Source: Old Source not Found: {0}", zones.zones.Find(y => y.zoneName == zone).playingSourceId);
                }
                try
                {
                    using (var httpClient = new HttpClient())
                    {
                        cookie = getSessionId();
                        HttpClientRequest req = new HttpClientRequest();
                        HttpClientResponse resp;
                        req.Url = new UrlParser(hostAddress + "/api/zones/byname/" + zones.zones.Find(x => x.zoneName.Equals(zone)).zoneName + "/switch?destinationSourceName=" + sources.sources.Find(x => x.sourceName.Equals(source)).sourceName);
                        req.RequestType = RequestType.Put;
                        req.Header.SetHeaderValue("cookie", cookie);
                        try
                        {
                            resp = httpClient.Dispatch(req);
                            var h = handlePutZone(resp);
                            if (debug == 1)
                            {
                                CrestronConsole.PrintLine("zoneId:{0}, zoneName:{1}, isPlaying:{2}, playingSourceId:{3}, videowallId: {4}", h.zoneId, h.zoneName, h.isPlaying, h.playingSourceId, h.videowallId);
                            }
                            sources.sources.Find(x => x.sourceName.Equals(source)).setInUse(true);
                        }
                        catch (Exception e)
                        {
                            CrestronConsole.PrintLine("Setting a Zone Source Failed! Zone: {0}, Source {1}", zone, source);
                            CrestronConsole.PrintLine("Exception: {0}", e.Message);
                        }
                        updateSources();
                    }
                }
                catch (Exception)
                {
                    CrestronConsole.PrintLine("Your Issue is here!, Line 224 Try Catch");
                }
            }
            else if(zones.zones.Exists(x => x.zoneName == zone))
            {
                CrestronConsole.PrintLine("Userful Module: Source Named {0} doesnt exist!", source);
                CrestronConsole.PrintLine("Available Sources:");
                sources.sources.ForEach(x => CrestronConsole.PrintLine( "Name: {0), Id: {1}", x.sourceName, x.sourceId));
            }
            else if(sources.sources.Exists(x => x.sourceName == source))
            {
                CrestronConsole.PrintLine("Userful Module: Zone Named {0} doesnt exist!", zone);
                CrestronConsole.PrintLine("Available Zones:");
                zones.zones.ForEach(x => CrestronConsole.PrintLine( "Name: {0), Id: {1}", x.zoneName, x.zoneId));
            }
            else
            {
                CrestronConsole.PrintLine("Userful Module: Source Named {0} doesnt exist!", source);
                CrestronConsole.PrintLine("Available Sources:");
                sources.sources.ForEach(x => CrestronConsole.PrintLine( "Name: {0), Id: {1}", x.sourceName, x.sourceId));
                CrestronConsole.PrintLine("Userful Module: Zone Named {0} doesnt exist!", zone);
                CrestronConsole.PrintLine("Available Zones:");
                zones.zones.ForEach(x => CrestronConsole.PrintLine( "Name: {0), Id: {1}", x.zoneName, x.zoneId));
            }
            getSysInfo();
        }

        private Zone handlePutZone(HttpClientResponse response)
        {
            string s;
            s = response.ContentString;
            return JsonConvert.DeserializeObject<Zone>(s);
        }
        public void playZone(string zone)
        {
            if(zones.zones.Exists(x => x.zoneName == zone))
            {
                using (var httpClient = new HttpClient())
                {
                    cookie = getSessionId();
                    HttpClientRequest req = new HttpClientRequest();
                    HttpClientResponse resp;
                    req.Url = new UrlParser(hostAddress + "/api/zones/byname/" + zone + "/play");
                    req.RequestType = RequestType.Put;
                    req.Header.SetHeaderValue("cookie", cookie);
                    resp = httpClient.Dispatch(req);
                }
            }else
            {
                CrestronConsole.PrintLine("Userful Module: Zone Named {0} doesnt exist!", zone);
                CrestronConsole.PrintLine("Available Zones:");
                zones.zones.ForEach(x => CrestronConsole.PrintLine( "Name: {0), Id: {1}", x.zoneName, x.zoneId));
            }
            getSysInfo();
        }

        private void updateSources()
        {
            if (this.UpdateSourcesInUse != null)
                this.UpdateSourcesInUse(this.sources, null);
        }

        public ushort SourceInUseByName(string name)
        {
            return Convert.ToUInt16(sources.inUseByName(name));
        }
    }
 }
