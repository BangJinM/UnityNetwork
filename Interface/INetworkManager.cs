namespace Network
{
    public delegate void MsgHandler(Message message);
    public interface INetworkManager
    {
        void RegisterHandler(int msgID, MsgHandler handler);
        void RemoveHandler(int msgID);
        void Update();
        void RecevieMsg(Message message);
        void SendMsg(NetworkTask message);
    }
}