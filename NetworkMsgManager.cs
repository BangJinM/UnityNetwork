using System.Collections.Generic;

namespace Network
{
    public class NetworkMsgManager : INetworkManager
    {
        Dictionary<int, MsgHandler> msgHandlers;
        Queue<Message> recMessages;
        Queue<NetworkTask> sendTasks;

        private int frameMaxDealCount = 30;

        public NetworkMsgManager(int frameDealCount)
        {
            frameMaxDealCount = frameDealCount;
            recMessages = new Queue<Message>();
            sendTasks = new Queue<NetworkTask>();
            msgHandlers = new Dictionary<int, MsgHandler>();
        }

        public void RegisterHandler(int msgID, MsgHandler handler)
        {
            msgHandlers[msgID] = handler;
        }
        public void RemoveHandler(int msgID)
        {
            if (msgHandlers.ContainsKey(msgID))
            {
                msgHandlers.Remove(msgID);
            }
        }

        public void Update()
        {
            int count = 0;
            while (recMessages.Count > 0)
            {
                var message = recMessages.Dequeue();
                MsgHandler msgHandler;
                if (msgHandlers.TryGetValue(message.msgID, out msgHandler))
                    msgHandler(message);
                count++;
                if (count > frameMaxDealCount)
                    break;
            }
            count = 0;
            while (sendTasks.Count > 0)
            {
                var task = sendTasks.Dequeue();
                task.Execute();
                count++;
                if (count > frameMaxDealCount)
                    break;
            }
        }

        public void RecevieMsg(Message message)
        {
            lock (recMessages)
            {
                recMessages.Enqueue(message);
            }
        }
        public void SendMsg(NetworkTask task)
        {
            lock (sendTasks)
            {
                sendTasks.Enqueue(task);
            }
        }
    }
}