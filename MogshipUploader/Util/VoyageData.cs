using Newtonsoft.Json;
using System.Collections.Generic;

namespace MogshipUploader.Util
{
    public class VoyageDataPacket
    {
        public UserData? User;
        public VoyageLogData VoyageLog;
        public unsafe VoyageDataPacket(SubmersibleData submersibleData, int id, UserData user)
        {
            this.User = user;
            this.VoyageLog = new VoyageLogData(submersibleData, id);
        }
        public unsafe VoyageDataPacket(SubmersibleData submersibleData)
        {
            User = null;
            this.VoyageLog = new VoyageLogData(submersibleData);
        }

        public string getJSON()
        {
            var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
            return JsonConvert.SerializeObject(this, settings);
        }
    }

    public class VoyageLogData
    {
        public int SubmarineId;
        public uint ReturnTime;
        public List<VoyageSectorData> Log;

        public unsafe VoyageLogData(SubmersibleData submersibleData)
        {
            this.SubmarineId = 0;
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
    }

    public class VoyageSectorData
    {
        public int SectorId;
        public int ExperienceRating;
        public int DiscoveredSectorId;
        public bool IsFirstTimeExplored;
        public int UnlockedSubmarine;
        public bool IsDoubleDip;
        public int FavorResult;
        public int Experience;
        public int Surveillance;
        public int Retrieval;
        public int Favor;
        public List<DipData> DipData;

        public unsafe VoyageSectorData(SubmersibleData submersibleData, int i)
        {
            this.DipData = new List<DipData>();
            this.SectorId = submersibleData.VoyageSectors[i].SectorId;
            this.ExperienceRating = submersibleData.VoyageSectors[i].ExpRating;
            this.DiscoveredSectorId = submersibleData.VoyageSectors[i].DiscoveredSectorId;
            this.IsFirstTimeExplored = submersibleData.VoyageSectors[i].FirstTimeExplored != 0;
            this.UnlockedSubmarine = submersibleData.VoyageSectors[i].UnlockedSubmarine;
            this.IsDoubleDip = submersibleData.VoyageSectors[i].DoubleDip != 0;
            this.FavorResult = (int)submersibleData.VoyageSectors[i].FavorResult;
            this.Experience = (int)submersibleData.VoyageSectors[i].Exp;
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
    }

    public class DipData
    {
        public int ItemId;
        public int Quantity;
        public bool IsHq;
        public bool IsNotTier3;
        public int SurveillanceResult;
        public int RetrievalResult;
        public int QualityResult;

        public unsafe DipData(SectorData sectorData, int i)
        {
            this.ItemId = (int)sectorData.ItemId[i];
            this.Quantity = (int)sectorData.Quantity[i];
            this.IsHq = sectorData.isHQ[i] != 0;
            this.IsNotTier3 = sectorData.isNotTier3[i] != 0;
            this.SurveillanceResult = (int)sectorData.SurveillanceResult[i];
            this.RetrievalResult = (int)sectorData.RetrievalResult[i];
            this.QualityResult = (int)sectorData.ItemQualityMessage[i];
        }
    }
}
