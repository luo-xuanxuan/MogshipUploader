using FFXIVClientStructs.FFXIV.Client.Game;
using Newtonsoft.Json;

namespace MogshipUploader.Util
{
    public class DamageDataPacket
    {
        [JsonProperty]
        DamageData Damage;

        public DamageDataPacket(DamageData Damage)
        {
            this.Damage = Damage;
        }

        public string getJSON()
        {
            var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
            return JsonConvert.SerializeObject(this, settings);
        }
    }

    public class DamageData
    {
        [JsonProperty]
        public int HullId;
        [JsonProperty]
        public int SternId;
        [JsonProperty]
        public int BowId;
        [JsonProperty]
        public int BridgeId;

        [JsonProperty]
        ShipCondition InitialCondition;
        [JsonProperty]
        ShipCondition FinalCondition;

        [JsonProperty]
        public int[]? Route;

        public unsafe DamageData(SubmersibleData submersibleData, ShipCondition condition, ShipCondition condition2)
        {
            this.HullId = submersibleData.HullId;
            this.SternId = submersibleData.SternId;
            this.BowId = submersibleData.BowId;
            this.BridgeId = submersibleData.BridgeId;
            this.InitialCondition = condition;
            this.FinalCondition = condition2;
            this.Route = new int[5];
            for (int i = 0; i < 5; i++)
                this.Route[i] = (int)submersibleData.Route[i];
        }

        public string getJSON()
        {
            var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
            return JsonConvert.SerializeObject(this, settings);
        }
    }
    public class ShipCondition
    {
        [JsonProperty]
        public int HullDurability;
        [JsonProperty]
        public int SternDurability;
        [JsonProperty]
        public int BowDurability;
        [JsonProperty]
        public int BridgeDurability;

        public ShipCondition()
        {
            HullDurability = 0;
            SternDurability = 0;
            BowDurability = 0;
            BridgeDurability = 0;
        }

        public unsafe ShipCondition(InventoryContainer* inventory, int id)
        {
            this.HullDurability = ((InventoryItem*)inventory->GetInventorySlot(0 + (id * 5)))->Condition;
            this.SternDurability = ((InventoryItem*)inventory->GetInventorySlot(1 + (id * 5)))->Condition;
            this.BowDurability = ((InventoryItem*)inventory->GetInventorySlot(2 + (id * 5)))->Condition;
            this.BridgeDurability = ((InventoryItem*)inventory->GetInventorySlot(3 + (id * 5)))->Condition;
        }

        public static bool operator ==(ShipCondition ship1, ShipCondition ship2)
        {
            if (ship1.HullDurability != ship2.HullDurability) return false;
            if (ship1.SternDurability != ship2.SternDurability) return false;
            if (ship1.BowDurability != ship2.BowDurability) return false;
            if (ship1.BridgeDurability != ship2.BridgeDurability) return false;
            return true;
        }

        public static bool operator !=(ShipCondition ship1, ShipCondition ship2)
        {
            if (ship1.HullDurability != ship2.HullDurability) return true;
            if (ship1.SternDurability != ship2.SternDurability) return true;
            if (ship1.BowDurability != ship2.BowDurability) return true;
            if (ship1.BridgeDurability != ship2.BridgeDurability) return true;
            return false;
        }

        public string getJSON()
        {
            var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
            return JsonConvert.SerializeObject(this, settings);
        }
    }
}
