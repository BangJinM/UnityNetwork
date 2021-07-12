namespace Network
{
    public interface IConnection
    {
        bool Connect(string ip, int port);
        bool Send(Message message);
        bool Close();
        bool IsConnected();
    }
}