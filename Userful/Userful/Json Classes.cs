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
        private bool inUse { get; set; }

        [JsonProperty("params")] //decorating
        public List <ParamsItem> parameters { get; set; }

        public SourcesItem()
        {
            this.inUse = false;
        }

        public void setInUse(bool val)
        {
            inUse = val;
        }

        public bool getInUse()
        {
            return inUse;
        }
    }

    public class UserfulSource
    {
        public List<SourcesItem> sources { get; set; }

        public bool inUseByName(string name)
        {
            if (sources.Exists(x => x.sourceName == name))
                return sources.Find(x => x.sourceName == name).getInUse();
            else
                return false;
        }

        public void setAllNotInUse()
        {
            sources.ForEach(x => x.setInUse(false));
        }
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

    public class UserfulPreset
    {
        public List<PresetItem> presets { get; set; }
    }

    public class PresetItem
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