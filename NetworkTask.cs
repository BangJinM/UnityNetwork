namespace Network
{
    public abstract class NetworkTask
    {
        public Message message;
        public abstract void Execute();
    }

    public class SendTask : NetworkTask
    {
        public TCPClient client;
        public override void Execute()
        {
            client.SendImmediately(message);
        }
    }
}