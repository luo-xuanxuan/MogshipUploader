using FFXIVClientStructs.FFXIV.Client.Game;
using System.Collections.Generic;
using System.Text;

namespace MogshipUploader.Util
{
    public class SubmarineDataPacket
    {
        public UserData User;
        public SubmarineData Submarine;
        public unsafe SubmarineDataPacket(SubmarineData sub, UserData user)
        {
            this.User = user;
            this.Submarine = sub;
        }

    }

    public class SubmarineData
    {
        public string SubmarineName;
        public int SubmarineId;
        public int Rank;
        public int Experience;
        public int HullId;
        public int SternId;
        public int BowId;
        public int BridgeId;
        public int HullDurability;
        public int SternDurability;
        public int BowDurability;
        public int BridgeDurability;
        public uint ReturnTime;
        public List<int> Voyage;

        public SubmarineData()
        {
            this.SubmarineName = "";
            this.SubmarineId = 0;
            this.Rank = 0;
            this.Experience = 0;
            this.HullId = 0;
            this.SternId = 0;
            this.BowId = 0;
            this.BridgeId = 0;
            this.HullDurability = 0;
            this.SternDurability = 0;
            this.BowDurability = 0;
            this.BridgeDurability = 0;
            this.ReturnTime = 0;
            this.Voyage = new List<int>();
        }

        public unsafe SubmarineData(SubmersibleData submersibleData, InventoryContainer* inventory, int id)
        {

            this.SubmarineName = Encoding.UTF8.GetString(submersibleData.Name, 20).TrimEnd('\0');
            this.SubmarineId = id;
            this.Rank = submersibleData.RankId;
            this.Experience = (int)submersibleData.Experience;
            this.HullId = submersibleData.HullId;
            this.SternId = submersibleData.SternId;
            this.BowId = submersibleData.BowId;
            this.BridgeId = submersibleData.BridgeId;
            this.HullDurability = ((InventoryItem*)inventory->GetInventorySlot(0 + (id * 5)))->Condition;
            this.SternDurability = ((InventoryItem*)inventory->GetInventorySlot(1 + (id * 5)))->Condition;
            this.BowDurability = ((InventoryItem*)inventory->GetInventorySlot(2 + (id * 5)))->Condition;
            this.BridgeDurability = ((InventoryItem*)inventory->GetInventorySlot(3 + (id * 5)))->Condition;
            this.ReturnTime = submersibleData.ReturnTime;
            this.Voyage = new List<int>();
            for (int i = 0; i < 5; i++)
            {
                if (submersibleData.Route[i] != 0)
                {
                    this.Voyage.Add(submersibleData.Route[i]);
                }
                else
                {
                    break;
                }
            }
        }

        public static bool operator ==(SubmarineData sub1, SubmarineData sub2)
        {
            if (sub1.SubmarineId != sub2.SubmarineId) return false;
            if (sub1.SubmarineName != sub2.SubmarineName) return false;
            if (sub1.Rank != sub2.Rank) return false;
            if (sub1.Experience != sub2.Experience) return false;
            if (sub1.HullId != sub2.HullId) return false;
            if (sub1.SternId != sub2.SternId) return false;
            if (sub1.BowId != sub2.BowId) return false;
            if (sub1.BridgeId != sub2.BridgeId) return false;
            if (sub1.HullDurability != sub2.HullDurability) return false;
            if (sub1.SternDurability != sub2.SternDurability) return false;
            if (sub1.BowDurability != sub2.BowDurability) return false;
            if (sub1.BridgeDurability != sub2.BridgeDurability) return false;
            if (sub1.ReturnTime != sub2.ReturnTime) return false;
            if (sub1.Voyage.Count != sub2.Voyage.Count) return false;
            for (int i = 0; i < sub1.Voyage.Count; i++)
                if (sub1.Voyage[i] != sub2.Voyage[i]) return false;
            return true;
        }

        public static bool operator !=(SubmarineData sub1, SubmarineData sub2)
        {
            if (sub1.SubmarineId != sub2.SubmarineId) return true;
            if (sub1.SubmarineName != sub2.SubmarineName) return true;
            if (sub1.Rank != sub2.Rank) return true;
            if (sub1.Experience != sub2.Experience) return true;
            if (sub1.HullId != sub2.HullId) return true;
            if (sub1.SternId != sub2.SternId) return true;
            if (sub1.BowId != sub2.BowId) return true;
            if (sub1.BridgeId != sub2.BridgeId) return true;
            if (sub1.HullDurability != sub2.HullDurability) return true;
            if (sub1.SternDurability != sub2.SternDurability) return true;
            if (sub1.BowDurability != sub2.BowDurability) return true;
            if (sub1.BridgeDurability != sub2.BridgeDurability) return true;
            if (sub1.ReturnTime != sub2.ReturnTime) return true;
            if (sub1.Voyage.Count != sub2.Voyage.Count) return true;
            for (int i = 0; i < sub1.Voyage.Count; i++)
                if (sub1.Voyage[i] != sub2.Voyage[i]) return true;
            return false;
        }
    }
}
