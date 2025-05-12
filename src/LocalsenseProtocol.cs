using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using WebSocketSharp;
using Newtonsoft.Json;

namespace LsWebsocketClient
{
    // 加密类包含crc16与MD5
    // Encryption class contains crc16 and md5
    public class LsCrypto
    {
        static readonly byte[] AucCrcHi =
        {
            0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41,
            0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
            0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41,
            0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
            0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41,
            0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
            0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
            0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
            0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41,
            0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
            0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41,
            0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
            0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41,
            0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
            0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
            0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
            0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41,
            0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
            0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41,
            0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
            0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41,
            0x00, 0xC1, 0x81, 0x40
        };

        static readonly byte[] AucCrcLo =
        {
            0x00, 0xC0, 0xC1, 0x01, 0xC3, 0x03, 0x02, 0xC2, 0xC6, 0x06, 0x07, 0xC7,
            0x05, 0xC5, 0xC4, 0x04, 0xCC, 0x0C, 0x0D, 0xCD, 0x0F, 0xCF, 0xCE, 0x0E,
            0x0A, 0xCA, 0xCB, 0x0B, 0xC9, 0x09, 0x08, 0xC8, 0xD8, 0x18, 0x19, 0xD9,
            0x1B, 0xDB, 0xDA, 0x1A, 0x1E, 0xDE, 0xDF, 0x1F, 0xDD, 0x1D, 0x1C, 0xDC,
            0x14, 0xD4, 0xD5, 0x15, 0xD7, 0x17, 0x16, 0xD6, 0xD2, 0x12, 0x13, 0xD3,
            0x11, 0xD1, 0xD0, 0x10, 0xF0, 0x30, 0x31, 0xF1, 0x33, 0xF3, 0xF2, 0x32,
            0x36, 0xF6, 0xF7, 0x37, 0xF5, 0x35, 0x34, 0xF4, 0x3C, 0xFC, 0xFD, 0x3D,
            0xFF, 0x3F, 0x3E, 0xFE, 0xFA, 0x3A, 0x3B, 0xFB, 0x39, 0xF9, 0xF8, 0x38,
            0x28, 0xE8, 0xE9, 0x29, 0xEB, 0x2B, 0x2A, 0xEA, 0xEE, 0x2E, 0x2F, 0xEF,
            0x2D, 0xED, 0xEC, 0x2C, 0xE4, 0x24, 0x25, 0xE5, 0x27, 0xE7, 0xE6, 0x26,
            0x22, 0xE2, 0xE3, 0x23, 0xE1, 0x21, 0x20, 0xE0, 0xA0, 0x60, 0x61, 0xA1,
            0x63, 0xA3, 0xA2, 0x62, 0x66, 0xA6, 0xA7, 0x67, 0xA5, 0x65, 0x64, 0xA4,
            0x6C, 0xAC, 0xAD, 0x6D, 0xAF, 0x6F, 0x6E, 0xAE, 0xAA, 0x6A, 0x6B, 0xAB,
            0x69, 0xA9, 0xA8, 0x68, 0x78, 0xB8, 0xB9, 0x79, 0xBB, 0x7B, 0x7A, 0xBA,
            0xBE, 0x7E, 0x7F, 0xBF, 0x7D, 0xBD, 0xBC, 0x7C, 0xB4, 0x74, 0x75, 0xB5,
            0x77, 0xB7, 0xB6, 0x76, 0x72, 0xB2, 0xB3, 0x73, 0xB1, 0x71, 0x70, 0xB0,
            0x50, 0x90, 0x91, 0x51, 0x93, 0x53, 0x52, 0x92, 0x96, 0x56, 0x57, 0x97,
            0x55, 0x95, 0x94, 0x54, 0x9C, 0x5C, 0x5D, 0x9D, 0x5F, 0x9F, 0x9E, 0x5E,
            0x5A, 0x9A, 0x9B, 0x5B, 0x99, 0x59, 0x58, 0x98, 0x88, 0x48, 0x49, 0x89,
            0x4B, 0x8B, 0x8A, 0x4A, 0x4E, 0x8E, 0x8F, 0x4F, 0x8D, 0x4D, 0x4C, 0x8C,
            0x44, 0x84, 0x85, 0x45, 0x87, 0x47, 0x46, 0x86, 0x82, 0x42, 0x43, 0x83,
            0x41, 0x81, 0x80, 0x40
        };

        public static ushort Crc16(byte[] frame, int len)
        {
            byte ucCrcHi = 0xFF;
            byte ucCrcLo = 0xFF;
            ushort iIndex;

            for (int i = 0; i < len; i++)
            {
                iIndex = (ushort)(ucCrcLo ^ (frame[i]));
                ucCrcLo = (byte)(ucCrcHi ^ AucCrcHi[iIndex]);
                ucCrcHi = AucCrcLo[iIndex];
            }

            return (ushort)((ushort)ucCrcHi << 8 | ucCrcLo);
        }

        public static string GetMd5(string pw)
        {
            string pwd = "";
            MD5 md5 = MD5.Create("md5");
            byte[] s = md5.ComputeHash(Encoding.ASCII.GetBytes(pw));
            for (int i = 0; i < s.Length; i++)
            {
                pwd = pwd + s[i].ToString("x2");
            }

            return pwd;
        }
    }

    public class LsProtocol
    {
        private enum LsServerType
        {
            ServerTypeUnknown,
            ServerTypeLocalSense,
            ServerTypeBlueIot,
        }

        static readonly LsServerType serverType = LsServerType.ServerTypeBlueIot;
        
        public static LsFrameType GetFrameType(byte[] data)
        {
            var tmp = Encoding.ASCII.GetString(data);
            if (tmp.Contains("localsense_video_response"))
            {
                return LsFrameType.FrameTypeJsonVideoTraceRes;
            }

            return (LsFrameType)data[2];
        }

        public static byte[] GetAuth(string username, string password, string salt)
        {
            password = LsCrypto.GetMd5(password);
            // 支持263密码加盐
            // Support 263 password with salt
            if (salt.Trim().Length != 0)
            {
                password = LsCrypto.GetMd5(password + salt);
            }

            var buff = new List<byte>();
            UInt16 head = (UInt16)0xcc5f;
            UInt16 tail = (UInt16)0xaabb;
            Byte type = (Byte)0x27;
            UInt32 ulen = (UInt32)(username.Length);
            UInt32 plen = (UInt32)(password.Length);

            buff.AddRange(head.ToByteArray(ByteOrder.Big));
            buff.Add(type);
            buff.AddRange(ulen.ToByteArray(ByteOrder.Big));
            buff.AddRange(Encoding.ASCII.GetBytes(username));
            buff.AddRange(plen.ToByteArray(ByteOrder.Big));
            buff.AddRange(Encoding.ASCII.GetBytes(password));
            // crc
            byte[] tmp = buff.ToArray();
            ushort crcValue = LsCrypto.Crc16(tmp.SubArray(2, tmp.Length - 2), tmp.Length - 2);
            buff.AddRange(((UInt16)(crcValue)).ToByteArray(ByteOrder.Big));
            buff.AddRange(tail.ToByteArray(ByteOrder.Big));

            return buff.ToArray();
        }

        public static List<LsPosition> DecodePosition(byte[] data)
        {
            List<LsPosition> vRet = new List<LsPosition>();
            int idx = 3;
            int cnt = data[idx];
            idx++;

            for (int i = 0; i < cnt; i++)
            {
                LsPosition item = new LsPosition();
                if (serverType == LsServerType.ServerTypeBlueIot)
                {
                    item.TagId = data.SubArray(idx, 8).To<UInt64>(ByteOrder.Big);
                    idx += 8;
                }
                else
                {
                    item.TagId = data.SubArray(idx, 4).To<uint>(ByteOrder.Big);
                    idx += 4;
                }

                item.XPos = data.SubArray(idx, 4).To<int>(ByteOrder.Big)/100.0;
                idx += 4;
                item.YPos = data.SubArray(idx, 4).To<int>(ByteOrder.Big) / 100.0;
                idx += 4;
                item.ZPos = (float)(data.SubArray(idx, 2).To<short>(ByteOrder.Big) / 100.0);
                idx += 2;
                item.MapId = data[idx];
                idx += 1;
                item.Capacity = data[idx];
                idx += 1;

                byte tmp = data[idx];
                idx += 1;
                item.BSleep = ((tmp & 0xf0) >> 4) == 1;
                item.BCharge = (tmp & 0x0f) == 1;

                item.Timestamp = data.SubArray(idx, 4).To<uint>(ByteOrder.Big);
                idx += 4;
                item.FloorNo = data[idx];
                idx += 1;
                item.Indicator = data[idx];
                idx += 1;

                vRet.Add(item);
            }

            return vRet;
        }

        public static List<LsPosition> DecodeWgsPosition(byte[] data)
        {
            List<LsPosition> vRet = new List<LsPosition>();
            int idx = 3;
            int cnt = data[idx];
            idx++;

            for (int i = 0; i < cnt; i++)
            {
                LsPosition item = new LsPosition();
                if (serverType == LsServerType.ServerTypeBlueIot)
                {
                    item.TagId = data.SubArray(idx, 8).To<UInt64>(ByteOrder.Big);
                    idx += 8;
                }
                else
                {
                    item.TagId = data.SubArray(idx, 4).To<uint>(ByteOrder.Big);
                    idx += 4;
                }

                item.XPos = data.SubArray(idx, 4).To<int>(ByteOrder.Big) / 10000000.0;
                idx += 4;
                item.YPos = data.SubArray(idx, 4).To<int>(ByteOrder.Big) / 10000000.0;
                idx += 4;
                item.ZPos = (float)(data.SubArray(idx, 2).To<short>(ByteOrder.Big) / 10000000.0);
                idx += 2;
                item.MapId = data[idx];
                idx += 1;
                item.Capacity = data[idx];
                idx += 1;

                byte tmp = data[idx];
                idx += 1;
                item.BSleep = ((tmp & 0xf0) >> 4) == 1;
                item.BCharge = (tmp & 0x0f) == 1;

                item.Timestamp = data.SubArray(idx, 4).To<uint>(ByteOrder.Big);
                idx += 4;
                item.FloorNo = data[idx];
                idx += 1;
                item.Indicator = data[idx];
                idx += 1;

                vRet.Add(item);
            }

            return vRet;
        }


        public static List<LsBatteryInfo> DecodeBattery(byte[] data)
        {
            List<LsBatteryInfo> vRet = new List<LsBatteryInfo>();
            int idx = 3;
            int cnt = data[idx];
            idx++;

            for (int i = 0; i < cnt; i++)
            {
                LsBatteryInfo item = new LsBatteryInfo();
                if (serverType == LsServerType.ServerTypeBlueIot)
                {
                    item.TagId = data.SubArray(idx, 8).To<UInt64>(ByteOrder.Big);
                    idx += 8;
                }
                else
                {
                    item.TagId = data.SubArray(idx, 4).To<uint>(ByteOrder.Big);
                    idx += 4;
                }

                item.Capacity = data[idx];
                idx += 1;
                item.BCharge = data[idx] == 1 ? true : false;
                idx += 1;

                vRet.Add(item);
            }

            return vRet;
        }

        public static List<LsBaseStatus> DecodeBaseStatus(byte[] data)
        {
            List<LsBaseStatus> vRet = new List<LsBaseStatus>();
            int idx = 3;
            int cnt = data[idx];
            idx++;

            for (int i = 0; i < cnt; i++)
            {
                LsBaseStatus item = new LsBaseStatus();
                item.BaseId = data.SubArray(idx, 4).To<uint>(ByteOrder.Big);
                idx += 4;
                item.Status = data[idx];
                idx += 1;
                item.XPos = data.SubArray(idx, 4).To<int>(ByteOrder.Big);
                idx += 4;
                item.XPos = (item.XPos / 100.0);
                item.YPos = data.SubArray(idx, 4).To<int>(ByteOrder.Big);
                idx += 4;
                item.YPos = (item.YPos / 100.0);
                item.ZPos = data.SubArray(idx, 2).To<short>(ByteOrder.Big);
                idx += 2;
                item.ZPos = (float)(item.ZPos / 100.0);
                item.MapId = data[idx];
                idx += 1;

                vRet.Add(item);
            }

            return vRet;
        }

        public static LsExtendAlarmInfo DecodeAlarmInfo(byte[] data)
        {
            // 跳过头部3字节
            // skip header 3 bytes
            int idx = 3;
            LsExtendAlarmInfo item = new LsExtendAlarmInfo();
            item.AlarmType = data[idx];
            idx += 1;
            if (serverType == LsServerType.ServerTypeBlueIot)
            {
                item.TagId = data.SubArray(idx, 8).To<UInt64>(ByteOrder.Big);
                idx += 8;
            }
            else
            {
                item.TagId = data.SubArray(idx, 4).To<uint>(ByteOrder.Big);
                idx += 4;
            }

            item.Timestamp = data.SubArray(idx, 8).To<ulong>(ByteOrder.Big);
            idx += 8;
            item.AreaId = data.SubArray(idx, 8).To<ulong>(ByteOrder.Big);
            idx += 8;
            item.XPos = data.SubArray(idx, 4).To<int>(ByteOrder.Big)/100.0;
            idx += 4;
            item.YPos = data.SubArray(idx, 4).To<int>(ByteOrder.Big)/100.0;
            idx += 4;

            idx += 30;
            item.AreaName = Encoding.Unicode.GetString(data.SubArray(idx, 34));
            idx += 34;
            item.AlarmId = data.SubArray(idx, 8).To<ulong>(ByteOrder.Big);
            idx += 8;
            idx += 19;
            item.MapId = data.SubArray(idx, 2).To<ushort>(ByteOrder.Big);
            idx += 2;
            idx += 10;
            byte alarmInfo = data[idx];
            idx += 1;
            if (alarmInfo == 2)
            {
                int len = data.SubArray(idx, 2).To<ushort>(ByteOrder.Big);
                idx += 2;
                item.AlarmInfo = Encoding.Unicode.GetString(data.SubArray(idx, len));
                idx += len;
            }

            return item;
        }

        public static LsTagExtendData DecodeTagExtendData(byte[] data)
        {
            int idx = 3;
            LsTagExtendData item = new LsTagExtendData();

            if (serverType == LsServerType.ServerTypeBlueIot)
            {
                item.TagId = data.SubArray(idx, 8).To<UInt64>(ByteOrder.Big);
                idx += 8;
            }
            else
            {
                item.TagId = data.SubArray(idx, 4).To<uint>(ByteOrder.Big);
                idx += 4;
            }


            int len = data.SubArray(idx, 2).To<ushort>(ByteOrder.Big);
            idx += 2;
            int subtype = data[idx];
            idx += 1;
            item.DataType = subtype;
            if (subtype == 0xD5)
            {
                var heartRate = new LsHeartRate();
                heartRate.HeartRate = data[idx];
                idx += 1;
                heartRate.Timestamp = data.SubArray(idx, 8).To<ulong>(ByteOrder.Big);
                idx += 8;
                heartRate.Reserve = data.SubArray(idx, 2).To<ushort>(ByteOrder.Big);
                item.Data = heartRate;
                idx += 2;
            }
            else
            {
                item.Data = data.SubArray(idx, len-1);
                idx += len-1;
            }
            item.Timestamp = data.SubArray(idx, 8).To<ulong>(ByteOrder.Big);
            return item;
        }

        public static LsAreaInOut DecodeAreaInOut(byte[] data)
        {
            int idx = 3;
            LsAreaInOut item = new LsAreaInOut();
            if (serverType == LsServerType.ServerTypeBlueIot)
            {
                item.TagId = data.SubArray(idx, 8).To<UInt64>(ByteOrder.Big);
                idx += 8;
            }
            else
            {
                item.TagId = data.SubArray(idx, 4).To<uint>(ByteOrder.Big);
                idx += 4;
            }

            int len = data.SubArray(idx, 2).To<ushort>(ByteOrder.Big);
            idx += 2;
            item.TagName = Encoding.Unicode.GetString(data.SubArray(idx, len));
            idx += len;

            item.AreaId = data.SubArray(idx, 8).To<ulong>(ByteOrder.Big);
            idx += 8;
            len = data.SubArray(idx, 2).To<ushort>(ByteOrder.Big);
            idx += 2;
            item.AreaName = Encoding.Unicode.GetString(data.SubArray(idx, len));
            idx += len;

            item.MapId = data.SubArray(idx, 2).To<ushort>(ByteOrder.Big);
            idx += 2;
            len = data.SubArray(idx, 2).To<ushort>(ByteOrder.Big);
            idx += 2;
            item.MapName = Encoding.Unicode.GetString(data.SubArray(idx, len));
            idx += len;

            item.Status = data[idx];
            idx++;
            item.TimeStamp = data.SubArray(idx, 8).To<ulong>(ByteOrder.Big);
            idx += 8;

            return item;
        }

        public static LsVideoTraceResponse DecodeTagVideoTrace(string jsondata)
        {
            Dictionary<string, object>
                dicResponse = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsondata);
            string res = dicResponse["localsense_video_response"].ToString();

            Dictionary<string, object> response = JsonConvert.DeserializeObject<Dictionary<string, object>>(res);
            LsVideoTraceResponse ret = new LsVideoTraceResponse();
            ret.VideoInfo.TagId = ulong.Parse(response["tagid"].ToString());
            ret.VideoInfo.Ip = response["ip"].ToString();
            ret.VideoInfo.Port = int.Parse(response["port"].ToString());
            ret.VideoInfo.Username = response["user"].ToString();
            ret.VideoInfo.Password = response["pwd"].ToString();
            ret.VideoInfo.DeviceType = int.Parse(response["type"].ToString());
            ret.VideoInfo.StreamType = response["streamtype"].ToString();
            ret.VideoInfo.ProtocolType = response["model"].ToString();
            ret.VideoInfo.Channel = response["channel"].ToString().Length>0 ? int.Parse(response["channel"].ToString()) : 0;
            ret.VideoInfo.DeviceId = response["id"].ToString();
            ret.Videos = new List<LsTagVideoInfo>();
            string videos = response["vidoes"].ToString();
            Newtonsoft.Json.Linq.JArray array = (Newtonsoft.Json.Linq.JArray)JsonConvert.DeserializeObject(videos);

            for (int i = 0; i < array.Count; i++)
            {
                LsTagVideoInfo video = new LsTagVideoInfo();
                video.TagId = ulong.Parse(array[i]["tagid"].ToString());
                video.Ip = array[i]["ip"].ToString();
                video.Port = int.Parse(array[i]["port"].ToString());
                video.Username = array[i]["user"].ToString();
                video.Password = array[i]["pwd"].ToString();
                video.DeviceType = int.Parse(array[i]["type"].ToString());
                video.StreamType = array[i]["streamtype"].ToString();
                video.ProtocolType = array[i]["model"].ToString();
                video.Channel = array[i]["channel"].ToString().Length > 0 ? int.Parse(array[i]["channel"].ToString()) : 0;
                video.DeviceId = array[i]["id"].ToString();
                ret.Videos.Add(video);
            }

            return ret;
        }

        public static LsAreaStatisticsVector DecodeAreaStatistics(byte[] data)
        {
            int idx = 3;
            LsAreaStatisticsVector lis = new LsAreaStatisticsVector();
            lis.FrameTotal = data.SubArray(idx, 2).To<ushort>(ByteOrder.Big);
            idx += 2;
            lis.CurFrameNo = data.SubArray(idx, 2).To<ushort>(ByteOrder.Big);
            idx += 2;
            int areanum = data[idx];
            idx += 1;
            List<LsAreaStatistics> vRet = new List<LsAreaStatistics>();
            for (int i = 0; i < areanum; ++i)
            {
                LsAreaStatistics item = new LsAreaStatistics();
                item.AreaId = data.SubArray(idx, 8).To<ulong>(ByteOrder.Big);
                idx += 8;
                int len = data[idx];
                idx += 1;
                item.AreaName = Encoding.Unicode.GetString(data.SubArray(idx, len));
                idx += len;
                int tagnum = data.SubArray(idx, 2).To<ushort>(ByteOrder.Big);
                idx += 2;
                List<LsTagInAreaInfo> vRetRti = new List<LsTagInAreaInfo>();
                for (int j = 0; j < tagnum; ++j)
                {
                    LsTagInAreaInfo rti = new LsTagInAreaInfo();
                    if (serverType == LsServerType.ServerTypeBlueIot)
                    {
                        rti.TagId = data.SubArray(idx, 8).To<UInt64>(ByteOrder.Big);
                        idx += 8;
                    }
                    else
                    {
                        rti.TagId = data.SubArray(idx, 4).To<uint>(ByteOrder.Big);
                        idx += 4;
                    }

                    len = data[idx];
                    idx += 1;
                    rti.TagName = Encoding.Unicode.GetString(data.SubArray(idx, len));
                    idx += len;

                    len = data[idx];
                    idx += 1;
                    rti.GroupName = Encoding.Unicode.GetString(data.SubArray(idx, len));
                    idx += len;

                    rti.Reserve1 = data[idx];
                    idx++;
                    rti.InTime = data.SubArray(idx, 8).To<ulong>(ByteOrder.Big);
                    idx += 8;
                    rti.Reserve2 = data.SubArray(idx, 8).To<ulong>(ByteOrder.Big);
                    idx += 8;
                    rti.Duration = data.SubArray(idx, 4).To<uint>(ByteOrder.Big);
                    idx += 4;
                    rti.Reserve3 = data.SubArray(idx, 4).To<uint>(ByteOrder.Big);
                    idx += 4;
                    vRetRti.Add(rti);
                }

                item.TagsIn = vRetRti;
                vRet.Add(item);
            }

            lis.AreaStatistics = vRet;
            return lis;
        }

        public static LsTagCountStatistics DecodeTagCountStatistics(byte[] data)
        {
            int idx = 3;
            LsTagCountStatistics psi = new LsTagCountStatistics();
            psi.FrameTotal = data.SubArray(idx, 2).To<ushort>(ByteOrder.Big);
            idx += 2;
            psi.CurFrameNo = data.SubArray(idx, 2).To<ushort>(ByteOrder.Big);
            idx += 2;
            psi.TotalTagCounts = data.SubArray(idx, 2).To<ushort>(ByteOrder.Big);
            idx += 2;
            psi.OnlineTagCounts = data.SubArray(idx, 2).To<ushort>(ByteOrder.Big);
            idx += 2;
            int regionNum = data[idx];
            idx += 1;
            List<LsMapTags> vMpi = new List<LsMapTags>();

            for (int i = 0; i < regionNum; ++i)
            {
                LsMapTags item = new LsMapTags();
                item.MapId = data.SubArray(idx, 2).To<ushort>(ByteOrder.Big);
                idx += 2;
                int len = data[idx];
                idx += 1;
                item.MapName = Encoding.Unicode.GetString(data.SubArray(idx, len));
                idx += len;
                int tagMapNum = data.SubArray(idx, 2).To<ushort>(ByteOrder.Big);
                idx += 2;

                List<ulong> mapTags = new List<ulong>();
                for (int j = 0; j < tagMapNum; ++j)
                {
                    if (serverType == LsServerType.ServerTypeBlueIot)
                    {
                        ulong tagId = data.SubArray(idx, 8).To<ulong>(ByteOrder.Big);
                        idx += 8;
                        mapTags.Add(tagId);
                    }
                    else
                    {
                        uint tagId = data.SubArray(idx, 4).To<uint>(ByteOrder.Big);
                        idx += 4;
                        mapTags.Add(tagId);
                    }
                }

                idx += 2;
                item.Tags = mapTags;
                vMpi.Add(item);
            }

            psi.Maps = vMpi;
            return psi;
        }

        public static List<LsTagCustomIot> DecodeCustomIot(byte[] data)
        {
            int idx = 3;
            int tagCount = data[idx];
            idx++;

            List<LsTagCustomIot> iots = new List<LsTagCustomIot>();

            for (int i = 0; i < tagCount; i++)
            {
                LsTagCustomIot iot;
                iot.TagId = data.SubArray(idx, 8).To<ulong>(ByteOrder.Big);
                idx += 8;
                iot.UploadTime = data.SubArray(idx, 8).To<ulong>(ByteOrder.Big);
                idx += 8;
                iot.DataLen = data[idx];
                idx++;
                iot.Data = new byte[iot.DataLen];
                Array.Copy(data, idx, iot.Data, 0, iot.DataLen);
                idx += iot.DataLen;
                iots.Add(iot);
            }

            return iots;
        }
		
		public static LsVitalSignData DecodeVitalSignData(byte[] data)
        {
            int idx = 3;
            LsVitalSignData vitalSignData = new LsVitalSignData();
            vitalSignData.TagId = data.SubArray(idx, 8).To<ulong>(ByteOrder.Big);
            idx += 8;
            vitalSignData.Type = (LsVitalSignType)data[idx];
            idx ++;
            vitalSignData.Data = data.SubArray(idx, 2).To<short>(ByteOrder.Big);
            idx += 2;
            vitalSignData.Data_Extend = data.SubArray(idx, 2).To<short>(ByteOrder.Big);
            idx += 2;
            vitalSignData.Timestamp = data.SubArray(idx, 8).To<ulong>(ByteOrder.Big);
            idx += 8;
            return vitalSignData;
        }

        // 以下是编码函数
        // Below is the encoding function
        public static string EncodeVideoTraceRequest(ulong tagId, bool op)
        {
            string txt = String.Format("{{\"localsense_video_request\":{{\"tagid\":\"{0}\", \"track\":\"{1}\" }} }}",
                tagId, op ? "true" : "false");

            return txt;
        }

        public static string EncodeTagControl(ulong tagId, LsTagControlType controlType)
        {
            string confType = "tagvibrateandshake";
            switch (controlType)
            {
                case LsTagControlType.ControlTypeBuzzOnly:
                    confType = "tagshake";
                    break;
                case LsTagControlType.ControlTypeVibrateOnly:
                    confType = "tagvibrate";
                    break;

            }
            string txt = String.Format(
                "{{\"localsense_conf_request\":{{\"conf_type\":\"{0}\", \"conf_value\":\"{1}\", \"tag_id\":{2}}} }}",
                confType, controlType != LsTagControlType.ControlTypeStop ? "enable" : "disable", tagId);

            return txt;
        }

        public static byte[] EncodeSubscribeTagIds(List<ulong> tagList)
        {
            var buff = new List<byte>();
            UInt16 head = (UInt16)0xcc5f;
            UInt16 tail = (UInt16)0xaabb;
            Byte type = (Byte)0xa9;
            UInt16 rsstype = (UInt16)0;

            buff.AddRange(head.ToByteArray(ByteOrder.Big));
            buff.Add(type);
            buff.AddRange(rsstype.ToByteArray(ByteOrder.Big));
            UInt16 cnt = (UInt16)tagList.Count();
            buff.AddRange(cnt.ToByteArray(ByteOrder.Big));
            foreach (UInt64 tagId in tagList)
            {
                if (serverType == LsServerType.ServerTypeBlueIot)
                {
                    buff.AddRange(tagId.ToByteArray(ByteOrder.Big));
                }
                else
                {
                    UInt32 id = (UInt32)tagId;
                    buff.AddRange(id.ToByteArray(ByteOrder.Big));
                }
            }

            byte[] tmp = buff.ToArray();
            ushort crcValue = LsCrypto.Crc16(tmp.SubArray(2, tmp.Length - 2), tmp.Length - 2);
            buff.AddRange(((UInt16)(crcValue)).ToByteArray(ByteOrder.Big));
            buff.AddRange(tail.ToByteArray(ByteOrder.Big));

            return buff.ToArray();
        }

        public static byte[] EncodeSubscribeMapIds(List<uint> mapIdList)
        {
            var buff = new List<byte>();
            UInt16 head = (UInt16)0xcc5f;
            UInt16 tail = (UInt16)0xaabb;
            Byte type = (Byte)0xa9;
            UInt16 rsstype = (UInt16)2;

            buff.AddRange(head.ToByteArray(ByteOrder.Big));
            buff.Add(type);
            buff.AddRange(rsstype.ToByteArray(ByteOrder.Big));
            UInt16 cnt = (UInt16)mapIdList.Count();
            buff.AddRange(cnt.ToByteArray(ByteOrder.Big));
            foreach (uint mapid in mapIdList)
            {
                UInt32 id = (UInt32)mapid;
                buff.AddRange(id.ToByteArray(ByteOrder.Big));
            }

            byte[] tmp = buff.ToArray();
            ushort crcValue = LsCrypto.Crc16(tmp.SubArray(2, tmp.Length - 2), tmp.Length - 2);
            buff.AddRange(((UInt16)(crcValue)).ToByteArray(ByteOrder.Big));
            buff.AddRange(tail.ToByteArray(ByteOrder.Big));

            return buff.ToArray();
        }

        public static byte[] EncodeSubscribeGroupIds(List<string> groupIdList)
        {
            var buff = new List<byte>();
            UInt16 head = (UInt16)0xcc5f;
            UInt16 tail = (UInt16)0xaabb;
            Byte type = (Byte)0xa9;
            UInt16 rsstype = (UInt16)1;

            buff.AddRange(head.ToByteArray(ByteOrder.Big));
            buff.Add(type);
            buff.AddRange(rsstype.ToByteArray(ByteOrder.Big));
            UInt16 cnt = (UInt16)groupIdList.Count();
            buff.AddRange(cnt.ToByteArray(ByteOrder.Big));
            foreach (string grpid in groupIdList)
            {
                UInt16 len = (UInt16)grpid.Length;
                buff.AddRange(len.ToByteArray(ByteOrder.Big));
                buff.AddRange(Encoding.ASCII.GetBytes(grpid));
            }

            byte[] tmp = buff.ToArray();
            ushort crcValue = LsCrypto.Crc16(tmp.SubArray(2, tmp.Length - 2), tmp.Length - 2);
            buff.AddRange(((UInt16)(crcValue)).ToByteArray(ByteOrder.Big));
            buff.AddRange(tail.ToByteArray(ByteOrder.Big));

            return buff.ToArray();
        }
    } // end class LsProtocol
}