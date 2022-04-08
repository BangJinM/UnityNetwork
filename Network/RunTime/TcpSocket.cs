using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace US
{
    public enum TCPSocketStatus
    {
        Undefined = -1,
        Connected,
        Close,
    }

    public abstract class SocketBase
    {
        protected const int BUFFER_SIZE = 1024 * 1024 * 4;

        public Action<TCPSocketStatus> SocketStatusChangedImpl;
        protected Int32 msgLength = 0;
        protected int buffCount = 0;
        protected byte[] readBuff = new byte[BUFFER_SIZE];
        protected byte[] lenBytes = new byte[sizeof(Int32)];

        public void SetSocketStatusChangedImpl(Action<TCPSocketStatus> action)
        {
            this.SocketStatusChangedImpl = action;
        }
        public abstract bool Connect(string ip, int port);
        public abstract bool Close();
        public abstract bool SendMsg(Message message, bool isDelay = true);
        public abstract bool IsConnected();
    }

    public class TcpSocket : SocketBase
    {
        //Socket
        private Socket socket;

        private ManualResetEvent connectDone = new ManualResetEvent(false);

        public IPAddress GetIPAddress(string ip, bool isIP4)
        {
            IPAddress ipAddress = null;
            bool isIp = false;
            try
            {
                isIp = IPAddress.TryParse(ip, out ipAddress);
            }
            catch (Exception error)
            {
                Debug.Log(error.Message.ToString());
            }

            if (!isIp)
            {
                ipAddress = Dns.GetHostEntry(ip).AddressList[0];
                if (!isIP4)
                {
                    IPHostEntry host = Dns.GetHostEntry(ip);
                    foreach (IPAddress ipv6 in host.AddressList)
                    {
                        if (ipv6.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                        {
                            ipAddress = ipv6;
                            break;
                        }
                    }
                }
                return ipAddress;
            }
            return ipAddress;
        }

        //连接服务端
        public override bool Connect(string ip, int port)
        {
            try
            {
                Close();

                IPAddress[] address = Dns.GetHostAddresses(ip);
                bool mIsInIp4 = address[0].AddressFamily == AddressFamily.InterNetwork;
                if (mIsInIp4)
                    socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                else
                    socket = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
                socket.Blocking = true;
                socket.NoDelay = true;

                IPAddress ipAddress = GetIPAddress(ip, mIsInIp4);
                if (ipAddress == null)
                {
                    return false;
                }

                IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, (int)port);

                IAsyncResult result = socket.BeginConnect(ipEndPoint, ConnectedCallback, socket);
                return true;
            }
            catch (Exception e)
            {
                Close();
                Debug.Log("连接失败:" + e.Message);
                return false;
            }
        }

        public void ConnectedCallback(IAsyncResult ar)
        {
            try
            {
                Socket s = (Socket)ar.AsyncState;
                s.EndConnect(ar);
                socket.BeginReceive(readBuff, buffCount, BUFFER_SIZE - buffCount, SocketFlags.None, ReceiveMsg, readBuff);
                Debug.Log("BeginReceive");
                SocketStatusChangedImpl?.Invoke(TCPSocketStatus.Connected);
            }
            catch (Exception e)
            {
                Close();
                Debug.Log("连接失败:" + e.Message);
            }
        }

        //关闭连接
        public override bool Close()
        {
            if (IsConnected())
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
            SocketStatusChangedImpl?.Invoke(TCPSocketStatus.Close);
            socket = null;
            return true;
        }

        //接收回调
        private void ReceiveMsg(IAsyncResult ar)
        {
            try
            {
                int count = socket.EndReceive(ar);
                buffCount = buffCount + count;
                DealMessage();
                socket.BeginReceive(readBuff, buffCount, BUFFER_SIZE - buffCount, SocketFlags.None, ReceiveMsg, readBuff);
            }
            catch (Exception e)
            {
                Debug.Log("ReceiveMsg失败:" + e.Message);
                Close();
            }
        }

        //消息处理
        private void DealMessage()
        {
            //沾包分包处理
            if (buffCount < sizeof(Int32))
                return;
            //包体长度
            Array.Copy(readBuff, lenBytes, sizeof(Int32));
            if (BitConverter.IsLittleEndian)
                Array.Reverse(lenBytes);
            msgLength = BitConverter.ToInt32(lenBytes, 0);
            if (buffCount < msgLength + sizeof(Int32))
                return;
            Debug.Log("Message message = new Message();:");
            byte[] b = new byte[msgLength];
            Array.Copy(readBuff, sizeof(Int32), b, 0, msgLength);
            ByteBuffer byteBuffer = ByteBuffer.Allocate(b);

            var message = Message.Decoder(byteBuffer);
            DispatchMessageController.Instance.ReceiveMessage(message);

            //清除已处理的消息
            int count = buffCount - msgLength - sizeof(Int32);
            Array.Copy(readBuff, sizeof(Int32) + msgLength, readBuff, 0, count);
            buffCount = count;
            if (buffCount > 0)
            {
                DealMessage();
            }
        }

        public override bool SendMsg(Message message, bool isDelay = true)
        {
            if (!IsConnected())
            {
                Debug.LogError("is Not Connect!");
                return false;
            }

            DispatchMessageController.Instance.SendMessage(message, (Message sendMsg) =>
            {
                if (!IsConnected())
                {
                    Debug.LogError("is Not Connect!");
                    return;
                }

                ByteBuffer b = Message.Encoder(sendMsg);
                byte[] length = length = BitConverter.GetBytes(b.GetLength());
                if (BitConverter.IsLittleEndian)
                    Array.Reverse(length);
                byte[] sendbuff = length.Concat(b.Read(b.GetLength())).ToArray();
                socket.Send(sendbuff, sendbuff.Count(), SocketFlags.None);
            }, isDelay);

            return true;
        }

        public override bool IsConnected()
        {
            return socket != null ? socket.Connected : false;
        }

    }
}