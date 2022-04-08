using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace US
{
    public delegate void MsgHandler(Message message);

    public class DispatchMessageController : MonoSingleton<DispatchMessageController>
    {

        //消息分发
        public int frameMaxDealCount = 30;
        public Queue<Message> recMessages = new Queue<Message>();
        //消息分发
        public Queue<KeyValuePair<Message, Action<Message>>> sendMessages = new Queue<KeyValuePair<Message, Action<Message>>>();

        Dictionary<int, MsgHandler> msgHandlers = new Dictionary<int, MsgHandler>();

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

        public void ReceiveMessage(Message message)
        {
            recMessages.Enqueue(message);
        }

        public void SendMessage(Message message, Action<Message> action, bool isDelay = true)
        {
            if (!isDelay)
            {
                action(message);
                return;
            }
            var keyValue = new KeyValuePair<Message, Action<Message>>(message, action);
            sendMessages.Enqueue(keyValue);
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
            while (sendMessages.Count > 0)
            {
                var keyValue = sendMessages.Dequeue();
                keyValue.Value?.Invoke(keyValue.Key);
                count++;
                if (count > frameMaxDealCount)
                    break;
            }
        }
    }
}