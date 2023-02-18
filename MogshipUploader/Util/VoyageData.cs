using Newtonsoft.Json;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace MogshipUploader.Util
{
    public class VoyageDataPacket
    {
        [JsonProperty]
        public UserData? User;
        [JsonProperty]
        public VoyageLogData VoyageLog;
        [JsonProperty]
        public UploaderVersionPacket UploaderVersion;
        public unsafe VoyageDataPacket(SubmersibleData submersibleData, int id, UserData user)
        {
            this.User = user;
            this.VoyageLog = new VoyageLogData(submersibleData, id);
            this.UploaderVersion = new UploaderVersionPacket();
        }
        public unsafe VoyageDataPacket(SubmersibleData submersibleData)
        {
            User = null;
            this.VoyageLog = new VoyageLogData(submersibleData);
            this.UploaderVersion = new UploaderVersionPacket();
        }

        public string getJSON()
        {
            var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
            return JsonConvert.SerializeObject(this, settings);
        }

        public string getJSONString()
        {
            string json = "{";
            if( User != null)
                json += $"\"User\":{User.getJSONString()},";
            json += $"\"VoyageLog\":{VoyageLog.getJSONString()},";
            //json += $"\"UploaderVersion\":{UploaderVersion.getJSONString()}}}";
            return json;
        }
    }

    public class VoyageLogData
    {
        [JsonProperty]
        public int? SubmarineId;
        [JsonProperty]
        public uint ReturnTime;
        [JsonProperty]
        public List<VoyageSectorData> Log;

        public unsafe VoyageLogData(SubmersibleData submersibleData)
        {
            this.SubmarineId = null;
            this.Log = new List<VoyageSectorData>();
            for (int i = 0; i < 5; i++)
            {

                if (submersibleData.VoyageSectors[i].SectorId != 0)
                {
                    VoyageSectorData sector = new VoyageSectorData(submersibleData, i);
                    this.Log.Add(sector);
                    continue;
                }
                break;
            }
        }

        public unsafe VoyageLogData(SubmersibleData submersibleData, int id)
        {
            this.SubmarineId = id;
            this.Log = new List<VoyageSectorData>();
            for (int i = 0; i < 5; i++)
            {

                if (submersibleData.VoyageSectors[i].SectorId != 0)
                {
                    VoyageSectorData sector = new VoyageSectorData(submersibleData, i);
                    this.Log.Add(sector);
                    continue;
                }
                break;
            }
        }

        public void UpdateSubmarine(SubmarineData submarine)
        {
            this.ReturnTime = submarine.ReturnTime;
            foreach (VoyageSectorData sector in this.Log)
            {
                sector.Surveillance = submarine.Surveillance;
                sector.Retrieval = submarine.Retrieval;
                sector.Favor = submarine.Favor;
            }
        }

        public string getJSONString()
        {
            string json = "{";
            if (SubmarineId != null)
                json += $"\"SubmarineId\":{SubmarineId},";
            json += $"\"ReturnTime\":{ReturnTime},";
            json += $"\"Log\":[";
            for (int i = 0; i < Log.Count; i++)
            {
                json += Log[i].getJSONString();
                if (i != Log.Count-1)
                    json += "," ;
            }
            json += "]}";
            return json;
        }
    }

    public class VoyageSectorData
    {
        [JsonProperty]
        public int SectorId;
        [JsonProperty]
        public int ExperienceRating;
        [JsonProperty]
        public int DiscoveredSectorId;
        [JsonProperty]
        public bool IsFirstTimeExplored;
        [JsonProperty]
        public int UnlockedSubmarine;
        [JsonProperty]
        public bool IsDoubleDip;
        [JsonProperty]
        public int FavorResult;
        [JsonProperty]
        public int Experience;
        [JsonProperty]
        public int Surveillance;
        [JsonProperty]
        public int Retrieval;
        [JsonProperty]
        public int Favor;
        [JsonProperty]
        public List<DipData> DipData;

        public unsafe VoyageSectorData(SubmersibleData submersibleData, int i)
        {
            this.DipData = new List<DipData>();
            this.SectorId = submersibleData.VoyageSectors[i].SectorId;
            this.ExperienceRating = submersibleData.VoyageSectors[i].ExperienceRating;
            this.DiscoveredSectorId = submersibleData.VoyageSectors[i].DiscoveredSectorId;
            this.IsFirstTimeExplored = submersibleData.VoyageSectors[i].IsFirstTimeExplored != 0;
            this.UnlockedSubmarine = submersibleData.VoyageSectors[i].UnlockedSubmarine;
            this.IsDoubleDip = submersibleData.VoyageSectors[i].IsDoubleDip != 0;
            this.FavorResult = (int)submersibleData.VoyageSectors[i].FavorResult;
            this.Experience = (int)submersibleData.VoyageSectors[i].Experience;
            this.Surveillance = submersibleData.SurveillanceBase + submersibleData.SurveillanceBonus;
            this.Retrieval = submersibleData.RetrievalBase + submersibleData.RetrievalBonus;
            this.Favor = submersibleData.FavorBase + submersibleData.FavorBonus;
            for (int j = 0; j < 2; j++)
            {
                if (submersibleData.VoyageSectors[i].ItemId[j] != 0)
                {
                    DipData dip = new DipData(submersibleData.VoyageSectors[i], j);
                    this.DipData.Add(dip);
                    continue;
                }
                break;
            }
        }

        public string getJSONString()
        {
            string json = $"{{";
            return "";
        }
    }

    public class DipData
    {
        [JsonProperty]
        public int ItemId;
        [JsonProperty]
        public int Quantity;
        [JsonProperty]
        public bool IsHq;
        [JsonProperty]
        public bool IsNotTier3;
        [JsonProperty]
        public int SurveillanceResult;
        [JsonProperty]
        public int RetrievalResult;
        [JsonProperty]
        public int QualityResult;

        public unsafe DipData(SubmersibleSectorData sectorData, int i)
        {
            this.ItemId = (int)sectorData.ItemId[i];
            this.Quantity = (int)sectorData.Quantity[i];
            this.IsHq = sectorData.isHQ[i] != 0;
            this.IsNotTier3 = sectorData.isNotTier3[i] != 0;
            this.SurveillanceResult = (int)sectorData.SurveillanceResult[i];
            this.RetrievalResult = (int)sectorData.RetrievalResult[i];
            this.QualityResult = (int)sectorData.QualityResult[i];
        }
    }
}
