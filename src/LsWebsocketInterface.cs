using System;
using System.Reflection;
using System.Text;
using WatsonWebsocket;

namespace LsWebsocketClient
{
    public class LsWebsocketInterface : IDisposable
    {
        private WatsonWsClient _wsRealData;
        private WatsonWsClient _wsCtrl;
        private readonly LsWebsocketParams _lsWebsocketParams;
        private bool _disposed = false;

        public Action<string> Logger;

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool dispose)
        {
            if (_disposed) return;
            if (!dispose) return;
            _disposed = true;
            if (_wsRealData != null)
            {
                _wsRealData.Dispose();
                _wsRealData = null;
            }

            if (_wsCtrl != null)
            {
                _wsCtrl.Dispose();
                _wsCtrl = null;
            }
        }

        public delegate void LsWebsocketMessageHandle(LsFrameType frameType, object data);

        public event LsWebsocketMessageHandle MessageEvent;

        void InvokeMessageEvent(LsFrameType frameType, object data)
        {
            MessageEvent?.Invoke(frameType, data);
            MessageEvent2?.Invoke(this, new LSMessageHandle(frameType, data));
        }

        public struct LSMessageHandle
        {
            public LsFrameType FrameType;
            public object Data;

            public LSMessageHandle(LsFrameType frameType, object data)
            {
                FrameType = frameType;
                Data = data;
            }
        }

        public event EventHandler<LSMessageHandle> MessageEvent2;
        public delegate void LsWebsocketStatusHandle(LsWebsocketStatus status, LsWebsocketSubProtocol subProtocol);

        public event LsWebsocketStatusHandle WebsocketStatusEvent;

        void InvokeWebSocketStatusEvent(LsWebsocketStatus status, LsWebsocketSubProtocol subProtocol)
        {
            WebsocketStatusEvent?.Invoke(status, subProtocol);
            WebsocketStatusEvent2?.Invoke(this, new LSStatusHandle(status, subProtocol));
        }

        public struct LSStatusHandle
        {
            public LsWebsocketStatus Status;
            public LsWebsocketSubProtocol SubProtocol;

            public LSStatusHandle(LsWebsocketStatus status, LsWebsocketSubProtocol subProtocol)
            {
                Status = status;
                SubProtocol = subProtocol;
            }
        }

        public event EventHandler<LSStatusHandle> WebsocketStatusEvent2;

        public LsWebsocketInterface(LsWebsocketParams lsWebsocketParams)
        {
            this._lsWebsocketParams = lsWebsocketParams;
        }

        public void Connect2Server()
        {
            if (((int)_lsWebsocketParams.SubProtocol & (int)LsWebsocketSubProtocol.SubProtocolRealTimeData) ==
                (int)LsWebsocketSubProtocol.SubProtocolRealTimeData)
            {
                _wsRealData = InternalConnect("localSensePush-protocol");
                _wsRealData.Start();
            }

            if (((int)_lsWebsocketParams.SubProtocol & (int)LsWebsocketSubProtocol.SubProtocolCtrl) ==
                (int)LsWebsocketSubProtocol.SubProtocolCtrl)
            {
                _wsCtrl = InternalConnect("localSense-Json");
                _wsCtrl.Start();
            }
        }

        public void DisconnectFromServer()
        {
            if (_wsRealData != null && _wsRealData.Connected)
            {
                _wsRealData.Stop();
            }
            if (_wsCtrl != null && _wsCtrl.Connected)
            {
                _wsCtrl.Stop();
            }
        }

        public void VideoTraceRequest(ulong tagId, bool op)
        {
            string txt = LsProtocol.EncodeVideoTraceRequest(tagId, op);
            _wsCtrl.SendAsync(txt);
        }

        public void TagControlRequest(ulong tagId, LsTagControlType controlType)
        {
            var data = LsProtocol.EncodeTagControl(tagId, controlType);
            _wsCtrl.SendAsync(data);
        }

        public void SubscribeViaTagId(List<ulong> tagIdList)
        {
            var data = LsProtocol.EncodeSubscribeTagIds(tagIdList);
            _wsRealData.SendAsync(data);
        }

        public void SubscribeViaGroupId(List<string> groupIdList)
        {
            var data = LsProtocol.EncodeSubscribeGroupIds(groupIdList);
            _wsRealData.SendAsync(data);
        }

        public void SubscribeViaMapId(List<uint> mapIdList)
        {
            var data = LsProtocol.EncodeSubscribeMapIds(mapIdList);
            _wsRealData.SendAsync(data);
        }

        public string WebsocketSdkVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        private WatsonWsClient InternalConnect(string subProtocol)
        {
            string url = String.Format("ws://{0}:{1}", _lsWebsocketParams.Domain, _lsWebsocketParams.Port);
            if (_lsWebsocketParams.BWss)
            {
                url = String.Format("wss://{0}:{1}", _lsWebsocketParams.Domain, _lsWebsocketParams.Port);
            }
            Uri.TryCreate(url, UriKind.Absolute, out var uri);

            var ws = new WatsonWsClient(uri);
            ws.ConfigureOptions(o  => { o.AddSubProtocol(subProtocol); });

            ws.Logger = (s) => { Logger?.Invoke(s); };

            ws.ServerConnected += (sender, e) =>
                HandleOpen(sender, e);

            ws.ServerDisconnected += (sender, e) => 
                HandleClose(sender, e);

            ws.MessageReceived += (sender, e) =>
                HandleMessage(e.Data.ToArray());

            return ws;
        }

        private void HandleOpen(object sender, EventArgs eventArgs)
        {
            if (sender == _wsCtrl)
            {
                InvokeWebSocketStatusEvent(LsWebsocketStatus.WebsocketStatusConnected,
                    LsWebsocketSubProtocol.SubProtocolCtrl);
            }

            if (sender == _wsRealData)
            {
                InvokeWebSocketStatusEvent(LsWebsocketStatus.WebsocketStatusConnected,
                    LsWebsocketSubProtocol.SubProtocolRealTimeData);
            }

            if (sender != null)
                ((WatsonWsClient)sender).SendAsync(LsProtocol.GetAuth(_lsWebsocketParams.Username,
                _lsWebsocketParams.Password, _lsWebsocketParams.Salt));
        }

        private void HandleClose(object sender, EventArgs eventArgs)
        {
            if (sender == _wsCtrl)
            {
                InvokeWebSocketStatusEvent(LsWebsocketStatus.WebsocketStatusClosed,
                    LsWebsocketSubProtocol.SubProtocolCtrl);
            }

            if (sender == _wsRealData )
            {
                InvokeWebSocketStatusEvent(LsWebsocketStatus.WebsocketStatusClosed,
                    LsWebsocketSubProtocol.SubProtocolRealTimeData);
            }

            if (_disposed || !_lsWebsocketParams.BReconnect)
            {
                return;
            }
            Thread.Sleep(_lsWebsocketParams.ReconnectInterval * 1000);
            if (sender != null && !((WatsonWsClient)sender).Connected)
            {
                ((WatsonWsClient)sender).Start();
            }
        }

        private void HandleError(object sender, ErrorEventArgs eventArgs)
        {
            if (sender == _wsCtrl)
            {
                InvokeWebSocketStatusEvent(LsWebsocketStatus.WebsocketStatusError,
                    LsWebsocketSubProtocol.SubProtocolCtrl);
            }

            if (sender == _wsRealData)
            {
                InvokeWebSocketStatusEvent(LsWebsocketStatus.WebsocketStatusError,
                    LsWebsocketSubProtocol.SubProtocolRealTimeData);
            }
        }

        private void HandleMessage(byte[] data)
        {
            var type = LsProtocol.GetFrameType(data);
            switch (type)
            {
                case LsFrameType.FrameTypePosition:
                    InvokeMessageEvent(type, LsProtocol.DecodePosition(data));
                    break;

                case LsFrameType.FrameTypeWgsPosition:
                    InvokeMessageEvent(type, LsProtocol.DecodeWgsPosition(data));
                    break;

                case LsFrameType.FrameTypeGlobalPosition:
                    InvokeMessageEvent(type, LsProtocol.DecodePosition(data));
                    break;

                case LsFrameType.FrameTypeAlarmEx:
                    InvokeMessageEvent(type, LsProtocol.DecodeAlarmInfo(data));
                    break;

                case LsFrameType.FrameTypeJsonVideoTraceRes:
                    InvokeMessageEvent(type, 
                        LsProtocol.DecodeTagVideoTrace(Encoding.ASCII.GetString(data)));
                    break;

                case LsFrameType.FrameTypeAreaInOut:
                    InvokeMessageEvent(type,
                        LsProtocol.DecodeAreaInOut(data));
                    break;

                case LsFrameType.FrameTypeBattery:
                    InvokeMessageEvent(type,
                        LsProtocol.DecodeBattery(data));
                    break;

                case LsFrameType.FrameTypeBaseStatus:
                    InvokeMessageEvent(type,
                        LsProtocol.DecodeBaseStatus(data));
                    break;

                case LsFrameType.FrameTypeExternData:
                    InvokeMessageEvent(type,
                        LsProtocol.DecodeTagExtendData(data));
                    break;

                case LsFrameType.FrameTypeAreaStatistics:
                    InvokeMessageEvent(type,
                        LsProtocol.DecodeAreaStatistics(data));
                    break;

                case LsFrameType.FrameTypeTagCountStatistics:
                    InvokeMessageEvent(type,
                        LsProtocol.DecodeTagCountStatistics(data));
                    break;
                case LsFrameType.FrameTypeCustomIot:
                    InvokeMessageEvent(type,
                        LsProtocol.DecodeCustomIot(data));
                    break;
				case LsFrameType.FrameTypeVitalSignData:
                    MessageEvent(type, LsProtocol.DecodeVitalSignData(data));
                    break;
            }
        }
    }
}