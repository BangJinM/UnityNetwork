using System.Collections.Generic;

namespace US.Net
{
    public enum NetState
    {
        Undefined = -1,
        Connected,
        Close,
    }
    public delegate void NetStateChanged(NetState state);
    public delegate void MsgHandler(Message message);
    public class SocketClient
    {
        public SocketBase tcpSocket;

        Dictionary<int, MsgHandler> msgHandlers = new Dictionary<int, MsgHandler>();

        public SocketClient()
        {
            tcpSocket = new TcpSocket();
        }

        public bool Connect(string ip, int port)
        {
            return tcpSocket.Connect(ip, port);
        }

        public bool Close()
        {
            return tcpSocket.Close();
        }

        public void SetNetStateChanged(NetStateChanged netStateChanged)
        {
            tcpSocket.NetStateChangedImpl += netStateChanged;
        }

        public void RegisterMsgHandler(int msgID, MsgHandler handler)
        {
            if (msgHandlers.ContainsKey(msgID))
            {
                msgHandlers[msgID] += handler;
                return;
            }
            msgHandlers[msgID] = handler;
        }

        public void RemoveMsgID(int msgID)
        {
            if (msgHandlers.ContainsKey(msgID))
            {
                msgHandlers.Remove(msgID);
            }
        }

        public void RemoveMsgHandler(int msgID, MsgHandler handler)
        {
            if (msgHandlers.ContainsKey(msgID))
            {
                msgHandlers[msgID] -= handler;
            }
        }

        public void SendMsg(Message msg, bool delay = true)
        {
            if (msg == null)
                return;
            if (delay)
                tcpSocket.sendMessages.Enqueue(msg);
            else
                tcpSocket.SendMsg(msg);
        }

        public void Update()
        {
            int count = 0;
            while (tcpSocket.recMessages.Count > 0)
            {
                var message = tcpSocket.recMessages.Dequeue();
                MsgHandler msgHandler;
                if (msgHandlers.TryGetValue(message.msgID, out msgHandler))
                    msgHandler(message);
                count++;
                if (count > tcpSocket.frameMaxDealCount)
                    break;
            }
            count = 0;
            while (tcpSocket.sendMessages.Count > 0)
            {
                var message = tcpSocket.sendMessages.Dequeue();
                tcpSocket.SendMsg(message);
                count++;
                if (count > tcpSocket.frameMaxDealCount)
                    break;
            }
        }
    }
}