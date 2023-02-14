using FFXIVClientStructs.Interop.Attributes;
using FFXIVClientStructs.Interop;
using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace MogshipUploader.Util
{
    [StructLayout(LayoutKind.Explicit, Size = 0xB8C0)]
    public unsafe partial struct HousingWorkshopTerritory
    {
        [FixedSizeArray<AirshipData>(4)]
        [FieldOffset(0x60)] public fixed byte AirshipDataList[0x1C8 * 4];

        [FieldOffset(0x7D8)] public byte ActiveAirshipId; // 0-3, 255 if none
        [FieldOffset(0x7D9)] public byte AirshipCount;
        [FieldOffset(0x7DA)] public byte AirshipMax;

        [FixedSizeArray<SubmersibleData>(4)]
        [FieldOffset(0x2960)] public fixed byte SubmersibleDataList[0x2320 * 4];

        [FixedSizeArray<Pointer<SubmersibleData>>(5)]
        [FieldOffset(0xB5E0)] public fixed byte SubmersibleDataPointerList[0x8 * 5]; // 0-3 is the same as SubmersibleDataList, 4 is the one you are currently using

        public Span<SubmersibleData> SubmersibleList => new(Unsafe.AsPointer(ref SubmersibleDataList[0]), 4);
    }

    [StructLayout(LayoutKind.Explicit, Size = 0x1C8)]
    public unsafe partial struct AirshipData
    {
        [FieldOffset(0xC)] public int RegisterTime;
        [FieldOffset(0x14)] public byte RankId;
        [FieldOffset(0x18)] public int ReturnTime;
        [FieldOffset(0x1C)] public uint CurrentExp;
        [FieldOffset(0x20)] public uint NextLevelExp;

        [FieldOffset(0x28)] public ushort HullId;
        [FieldOffset(0x30)] public ushort SternId;
        [FieldOffset(0x32)] public ushort BowId;
        [FieldOffset(0x34)] public ushort BridgeId;

        [FieldOffset(0x36)] public ushort Surveillance;
        [FieldOffset(0x38)] public ushort Retrieval;
        [FieldOffset(0x40)] public ushort Speed;
        [FieldOffset(0x42)] public ushort Range;
        [FieldOffset(0x44)] public ushort Favor;

        [FieldOffset(0x42)] public fixed byte Route[5];

        [FieldOffset(0x3F)] public fixed byte Name[20];
    }

    [StructLayout(LayoutKind.Explicit, Size = 0x2320)]
    public unsafe partial struct SubmersibleData
    {
        [FieldOffset(0x0)] public SubmersibleData* Self;
        [FieldOffset(0x0E)] public byte RankId;
        [FieldOffset(0x10)] public int RegisterTime;
        [FieldOffset(0x14)] public int ReturnTime;
        [FieldOffset(0x18)] public uint Experience;
        [FieldOffset(0x1C)] public uint ExperienceToNextRank;

        [FieldOffset(0x22)] public fixed byte Name[20];

        [FieldOffset(0x3A)] public ushort HullId;
        [FieldOffset(0x3C)] public ushort SternId;
        [FieldOffset(0x3E)] public ushort BowId;
        [FieldOffset(0x40)] public ushort BridgeId;

        [FieldOffset(0x42)] public fixed byte Route[5];

        [FieldOffset(0x4A)] public ushort SurveillanceBase;
        [FieldOffset(0x4C)] public ushort RetrievalBase;
        [FieldOffset(0x4E)] public ushort SpeedBase;
        [FieldOffset(0x50)] public ushort RangeBase;
        [FieldOffset(0x52)] public ushort FavorBase;

        [FieldOffset(0x54)] public ushort SurveillanceBonus;
        [FieldOffset(0x56)] public ushort RetrievalBonus;
        [FieldOffset(0x58)] public ushort SpeedBonus;
        [FieldOffset(0x5A)] public ushort RangeBonus;
        [FieldOffset(0x5C)] public ushort FavorBonus;

        [FixedSizeArray<SectorData>(5)]
        [FieldOffset(0x64)] public fixed byte VoyageSectorData[0x38 * 5];

        public Span<SectorData> VoyageSectors => new(Unsafe.AsPointer(ref VoyageSectorData[0]), 5);
    }

    [StructLayout(LayoutKind.Explicit, Size = 0x38)]
    public unsafe partial struct SectorData
    {
        [FieldOffset(0x0)] public byte SectorId;
        [FieldOffset(0x1)] public byte ExpRating;
        [FieldOffset(0x2)] public byte DiscoveredSectorId;
        [FieldOffset(0x3)] public byte FirstTimeExplored;
        [FieldOffset(0x4)] public byte UnlockedSubmarine;
        [FieldOffset(0x5)] public byte DoubleDip;

        [FieldOffset(0x8)] public uint FavorResult;
        [FieldOffset(0xC)] public uint Exp;

        [FieldOffset(0x10)] public fixed uint ItemId[2];
        [FieldOffset(0x18)] public fixed ushort Quantity[2];
        [FieldOffset(0x1C)] public fixed byte isHQ[2];
        [FieldOffset(0x1E)] public fixed byte isNotTier3[2];
        [FieldOffset(0x20)] public fixed uint SurveillanceResult[2];
        [FieldOffset(0x28)] public fixed uint RetrievalResult[2];
        [FieldOffset(0x30)] public fixed uint ItemQualityMessage[2];
    }
}
