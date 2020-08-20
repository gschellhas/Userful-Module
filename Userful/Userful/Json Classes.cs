using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Newtonsoft.Json;  

namespace Userful
{
    public class ParamsItem
    {
        public string type { get; set; }
        public string paramName { get; set; }
        public string paramValue { get; set; }
    }

    public class SourcesItem
    {
        public string sourceId { get; set; }
        public string sourceName { get; set; }
        public string sourceType { get; set; }

        [JsonProperty("params")] //decorating
        public List <ParamsItem> parameters { get; set; }
    }

    public class UserfulSource
    {
        public List<SourcesItem> sources { get; set; }
    }


    public class ZoneDisplaysItem
    {
        public int displayId { get; set; }
        public string isSelected { get; set; }
    }

    public class ZonesItem
    {
        public string zoneId { get; set; }
        public string zoneName { get; set; }
        public string isPlaying { get; set; }
        public string playingSourceId { get; set; }
        public string isShowingWelcomeScreen { get; set; }
        public string videowallId { get; set; }
        public int controlDisplayId { get; set; }
        public List<ZoneDisplaysItem> zoneDisplays { get; set; }
    }

    public class UserfulZone
    {
        public List<ZonesItem> zones { get; set; }
    }


    public class Preset
    {
        public string id { get; set; }
        public string name { get; set; }
        public string playing { get; set; }        
    }

    public class Zone
    {
        public string zoneId { get; set; }
        public string zoneName { get; set; }
        public bool isPlaying { get; set; }
        public string playingSourceId { get; set; }
        public string videowallId { get; set; }
    }
}