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

        public UserfulControl()
        {
            usr = new User();
            hostAddress = "http://localhost:9000";
        }

        //private void updateZones(object o)

        private string getSessionId() 
        {
            using (var httpClient = new HttpClient())
            {
                HttpClientRequest req = new HttpClientRequest();
                HttpClientResponse resp;
                req.Url = new UrlParser(hostAddress + "/api/session");
                req.RequestType = RequestType.Post;
                //httpClient.HostAddress = hostAddress;
                var auth = JsonConvert.SerializeObject(usr);
                var byteContent = Encoding.ASCII.GetBytes(auth);
                req.ContentBytes = byteContent;
                resp = httpClient.Dispatch(req);
                return postCallBack(resp);
            }
        }
        private string postCallBack(HttpClientResponse response)
        {
            try
            {
                CrestronConsole.PrintLine("Get Session Id - Response Code:{0}, Response Conetent String: {1}, Response Url: {2}",  response.Code, response.ContentString, response.ResponseUrl);
                return response.Header.GetHeaderValue("Set-Cookie");
            }
            catch (Exception e)
            {
                CrestronConsole.PrintLine("Cookie Not Found");
                return null;
            }
        }


        public void getSourceInfo()
        {
            using (var httpClient = new HttpClient())
            {
                cookie = getSessionId();
                HttpClientRequest req = new HttpClientRequest();
                HttpClientResponse resp;
                req.Url = new UrlParser(hostAddress + "/api/sources");
                req.RequestType = RequestType.Get;
                //httpClient.HostAddress = hostAddress;
                req.Header.SetHeaderValue("cookie", cookie);
                resp = httpClient.Dispatch(req);
                var h = handleGetSource(resp);
                h.sources.ForEach(x =>
                {
                    CrestronConsole.PrintLine(x.sourceName);
                });
            }
        }
        private UserfulSource handleGetSource(HttpClientResponse response)
        {
            string s;
            s = response.ContentString;
            return JsonConvert.DeserializeObject<UserfulSource>(s);
        }


        public void getZoneInfo()
        {
            using (var httpClient = new HttpClient())
            {
                cookie = getSessionId();
                HttpClientRequest req = new HttpClientRequest();
                HttpClientResponse resp;
                req.Url = new UrlParser(hostAddress + "/api/zones");
                req.RequestType = RequestType.Get;
                //httpClient.HostAddress = hostAddress;
                req.Header.SetHeaderValue("cookie", cookie);
                resp = httpClient.Dispatch(req);
                var h = handleGetZone(resp);
                h.zones.ForEach(x =>
                {
                    CrestronConsole.PrintLine(x.zoneName);
                });
            }
        }
        private UserfulZone handleGetZone(HttpClientResponse response)
        {
            string s;
            s = response.ContentString;
            return JsonConvert.DeserializeObject<UserfulZone>(s);
        }


        public void setPreset(string str)
        {
            using (var httpClient = new HttpClient())
            {
                cookie = getSessionId();
                HttpClientRequest req = new HttpClientRequest();
                HttpClientResponse resp;
                req.Url = new UrlParser(hostAddress + "/api/presets/byname/" + str + "/switch");
                req.RequestType = RequestType.Put;
                //httpClient.HostAddress = hostAddress;
                req.Header.SetHeaderValue("cookie", cookie);
                resp = httpClient.Dispatch(req);
                var h = handlePutPreset(resp);
                CrestronConsole.PrintLine("id:{0}, name:{1}, playing:{2}", h.id, h.name, h.playing);
            }

        }
        private Preset handlePutPreset(HttpClientResponse response)
        {
            string s;
            s = response.ContentString;
            return JsonConvert.DeserializeObject<Preset>(s);
        }


        public void setZone(string str1, string str2)
        {
            using (var httpClient = new HttpClient())
            {
                cookie = getSessionId();
                HttpClientRequest req = new HttpClientRequest();
                HttpClientResponse resp;
                req.Url = new UrlParser(hostAddress + "/api/zones/byname/" + str1 + "/switch?destinationSourceName=" + str2);
                req.RequestType = RequestType.Put;
                //httpClient.HostAddress = hostAddress;
                req.Header.SetHeaderValue("cookie", cookie);
                resp = httpClient.Dispatch(req);
                var h = handlePutZone(resp);
                CrestronConsole.PrintLine("zoneId:{0}, zoneName:{1}, isPlaying:{2}, playingSourceId:{3}, videowallId: {4}", h.zoneId, h.zoneName, h.isPlaying, h.playingSourceId, h.videowallId);
                //UpdateZones(object o);
            }
        }
        private Zone handlePutZone(HttpClientResponse response)
        {
            string s;
            s = response.ContentString;
            return JsonConvert.DeserializeObject<Zone>(s);
        }
        public void playZone(string str1)
        {
            using (var httpClient = new HttpClient())
            {
                cookie = getSessionId();
                HttpClientRequest req = new HttpClientRequest();
                HttpClientResponse resp;
                req.Url = new UrlParser(hostAddress + "/api/zones/byname/" + str1 + "/play");
                req.RequestType = RequestType.Put;
                //httpClient.HostAddress = hostAddress;
                req.Header.SetHeaderValue("cookie", cookie);
                resp = httpClient.Dispatch(req);
            }
        }
    }
 }
