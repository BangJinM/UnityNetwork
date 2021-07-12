using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Network
{
    //网络链接
    public class TCPClient : IConnection
    {
        //常量
        const int BUFFER_SIZE = 1024 * 1024 * 4;
        //Socket
        private Socket socket;
        //Buff
        private byte[] readBuff = new byte[BUFFER_SIZE];
        private int buffCount = 0;
        //沾包分包
        private Int32 msgLength = 0;
        private byte[] lenBytes = new byte[sizeof(Int32)];
        //消息分发
        INetworkManager networkManager;

        public TCPClient(INetworkManager manager)
        {
            networkManager = manager;
        }

        //连接服务端
        public bool Connect(string ip, int port)
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
                    if (!mIsInIp4)
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
                }

                Debug.Log("ConnectServer ip:" + ip + " port:" + port + " isIp:" + isIp + "  mIsInIp4：" + mIsInIp4 + " ipAddress:" + ipAddress.ToString());
                IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, (int)port);

                SocketAsyncEventArgs e = new SocketAsyncEventArgs()
                {
                    RemoteEndPoint = (EndPoint)ipEndPoint
                };
                e.Completed += (EventHandler<SocketAsyncEventArgs>)((sender, eventArgs) =>
                {
                    if (eventArgs.SocketError == SocketError.Success)
                        return;
                    Debug.Log((object)eventArgs.SocketError.ToString());
                });
                socket.ConnectAsync(e);
                //BeginReceive
                socket.BeginReceive(readBuff, buffCount,
                          BUFFER_SIZE - buffCount, SocketFlags.None,
                          ReceiveCb, readBuff);
                Debug.Log("连接成功");
                return true;
            }
            catch (Exception e)
            {
                Debug.Log("连接失败:" + e.Message);
                return false;
            }
        }

        //关闭连接
        public bool Close()
        {
            try
            {
                if (IsConnected())
                {
                    socket.Close();
                }
                socket = null;
                return true;
            }
            catch (Exception e)
            {
                Debug.Log("关闭失败:" + e.Message);
                return false;
            }
        }

        //接收回调
        private void ReceiveCb(IAsyncResult ar)
        {
            try
            {
                int count = socket.EndReceive(ar);
                buffCount = buffCount + count;
                ProcessData();
                socket.BeginReceive(readBuff, buffCount,
                         BUFFER_SIZE - buffCount, SocketFlags.None,
                         ReceiveCb, readBuff);
            }
            catch (Exception e)
            {
                Debug.Log("ReceiveCb失败:" + e.Message);
                Close();
            }
        }

        //消息处理
        private void ProcessData()
        {
            //沾包分包处理
            if (buffCount < sizeof(Int32))
                return;
            //包体长度
            Array.Copy(readBuff, lenBytes, sizeof(Int32));
            msgLength = BitConverter.ToInt32(lenBytes, 0);
            if (buffCount < msgLength + sizeof(Int32))
                return;
            Debug.Log("Message message = new Message();:");
            //协议解码
            byte[] b = new byte[msgLength];
            Array.Copy(readBuff, sizeof(Int32), b, 0, msgLength);
            var message = ProtoMsgProtocol.Decoder(b);
            Debug.Log(message.ToString());
            networkManager.RecevieMsg(message);

            //清除已处理的消息
            int count = buffCount - msgLength - sizeof(Int32);
            Array.Copy(readBuff, sizeof(Int32) + msgLength, readBuff, 0, count);
            buffCount = count;
            if (buffCount > 0)
            {
                ProcessData();
            }
        }

        public bool SendImmediately(Message message)
        {
            if (!IsConnected())
            {
                Debug.LogError("is Not Connect!");
                Close();
                return true;
            }

            byte[] b = ProtoMsgProtocol.Encoder(message);
            byte[] length = BitConverter.GetBytes(b.Length);

            byte[] sendbuff = length.Concat(b).ToArray();
            socket.Send(sendbuff, sendbuff.Count(), SocketFlags.None);
            return true;
        }

        public bool Send(Message message)
        {
            SendTask task = new SendTask();
            task.client = this;
            task.message = message;
            networkManager.SendMsg(task);
            return true;
        }


        public bool IsConnected()
        {
            if (socket != null)
                return socket.Connected;
            return false;
        }
    }
}