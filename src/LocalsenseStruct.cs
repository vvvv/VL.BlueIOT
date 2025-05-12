using System.Collections.Generic;

namespace LsWebsocketClient
{
    public enum LsFrameType
    {
        FrameTypeUnknown = 0,
        FrameTypeCustomIot = 0x66,
        FrameTypePosition = 0x81,
        FrameTypeBattery = 0x85,
        FrameTypeBaseStatus = 0x87,
        FrameTypeAlarmEx = 0x89,
        FrameTypeAreaStatistics = 0xa1,
        FrameTypeExternData = 0x88,
        FrameTypeTagCountStatistics = 0xB1,
        FrameTypeAreaInOut = 0xB3,
        FrameTypeWgsPosition = 0xB4,
        FrameTypeGlobalPosition = 0xB5,
        FrameTypeJsonVideoTraceRes = 0xBF,
        FrameTypeVitalSignData = 0xD7,
    }

    public enum LsWebsocketSubProtocol
    {
        SubProtocolUnknown,
        SubProtocolRealTimeData = 0x01,
        SubProtocolCtrl = 0x02,
        SubProtocolRealCtrlBoth = 0x03,
    }

    public enum LsWebsocketStatus
    {
        WebsocketStatusUnknown,
        WebsocketStatusConnected,
        WebsocketStatusClosed,
        WebsocketStatusError,
    }

    public enum LsTagControlType
    {
        ControlTypeUnknown,
        ControlTypeVibrateOnly,
        ControlTypeBuzzOnly,
        ControlTypeVibrateBuzz,
        ControlTypeStop,
    }

    public enum LsVitalSignType
    {
        VitalSignTypeUnknown,
        VitalSignTypeHeartRate,
        VitalSignTypeBloodOxygen,
        VitalSignTypeTemperature,
        VitalSignTypeBloodPressure,
    }

    public struct LsPosition
    {
        public ulong TagId;
        public double XPos;
        public double YPos;
        public float ZPos;
        public uint MapId;
        public uint Capacity;
        public bool BSleep;
        public bool BCharge;
        public uint Timestamp;
        public int FloorNo;
        public int Indicator;
    }

    public struct LsBatteryInfo
    {
        public ulong TagId;
        public uint Capacity;
        public bool BCharge;
    }

    public struct LsBaseStatus
    {
        public uint BaseId;
        public int Status;
        public double XPos;
        public double YPos;
        public float ZPos;
        public int MapId;
    }

    public struct LsExtendAlarmInfo
    {
        public int AlarmType;
        public ulong AlarmId;
        public ulong TagId;
        public ulong Timestamp;
        public double XPos;
        public double YPos;
        public int MapId;
        public ulong AreaId;
        public string AreaName;
        public string AlarmInfo;
    }

    public struct LsTagCustomIot
    {
        public ulong TagId;
        public ulong UploadTime;
        public int DataLen;
        public byte[] Data;
    }

    public struct LsTagExtendData
    {
        public ulong TagId;
        public int DataType;
        public object Data;
        public ulong Timestamp;
    }

    public struct LsHeartRate
    {
        public int HeartRate;
        public ulong Timestamp;
        public ushort Reserve;
    }

    public struct LsAreaInOut
    {
        public ulong TagId;
        public string TagName;
        public ulong AreaId;
        public string AreaName;
        public int MapId;
        public string MapName;
        public int Status;
        public ulong TimeStamp;
    }

    public struct LsTagInAreaInfo
    {
        public ulong TagId;
        public string TagName;
        public string GroupName;
        public uint Reserve1;
        public ulong InTime;
        public ulong Reserve2;
        public uint Duration;
        public uint Reserve3;
    }

    public struct LsAreaStatistics
    {
        public ulong AreaId;
        public string AreaName;
        public List<LsTagInAreaInfo> TagsIn;
    }

    public struct LsAreaStatisticsVector
    {
        public ushort FrameTotal;
        public ushort CurFrameNo;
        public List<LsAreaStatistics> AreaStatistics;
    }

    public struct LsMapTags
    {
        public uint MapId;
        public string MapName;
        public List<ulong> Tags;
    }

    public struct LsTagCountStatistics
    {
        public ushort FrameTotal;
        public ushort CurFrameNo;

        public uint TotalTagCounts;
        public uint OnlineTagCounts;

        public List<LsMapTags> Maps;
    }

    public struct LsTagVideoInfo
    {
        public ulong TagId;
        public string DeviceId;
        public string Ip;
        public int Port;
        public string Username;
        public string Password;

        // 摄像头类型, 1：海康 2：天地伟业 3：大华 4： 宇视
        // Camera type, 1: Haikang 2: Tiandi Weiye 3: Dahua 4: Yushi
        public int DeviceType;

        // 码流类型,1: 主码流  2：子码流
        // Bitstream type,1: main bitstream and 2: subbitstream
        public string StreamType;
        public string ProtocolType;
        public int Channel;
    }

    public struct LsVideoTraceResponse
    {
        public LsTagVideoInfo VideoInfo;
        public List<LsTagVideoInfo> Videos;
    }

    public struct LsVitalSignData
    {
        public ulong TagId;
        public LsVitalSignType Type;
        public short Data;
        public short Data_Extend;
        public ulong Timestamp;
    }
    
    public struct LsWebsocketParams
    {
        public string Domain;
        public int Port;
        public string Username;
        public string Password;
        public string Salt;
        public bool BReconnect;
        public bool BWss;
        public int ReconnectInterval;
        public LsWebsocketSubProtocol SubProtocol;
    };
}