using System;
using System.Linq;
using XLua;

namespace US
{
    public class Message
    {
        public int msgID = 0;
        public int param = 1;
        public int param1 = 2;
        public int param2 = 3;
        public int param3 = 40;
        public int param4 = 50;
        public int recog = -1;
        public string data = "1111111111111";

        public override string ToString()
        {
            return string.Format("msgID = {0}\nparam = {1}\nparam1 = {2}\nparam2 = {3}\nparam3 = {4}\nparam4 = {5}\nrecog = {6}\ndata = {7}\n",
             msgID, param, param1, param2, param3, param4, recog, data);
        }

        public static Message Decoder(ByteBuffer byteBuffer)
        {
            Message message = new Message();

            message.msgID = byteBuffer.ReadInt();
            message.param = byteBuffer.ReadInt();
            message.param1 = byteBuffer.ReadInt();
            message.param2 = byteBuffer.ReadInt();
            message.param3 = byteBuffer.ReadInt();
            message.param4 = byteBuffer.ReadInt();
            message.recog = byteBuffer.ReadInt();

            int strLength = byteBuffer.GetLength() - sizeof(Int32) * 7;
            byte[] str = new byte[strLength];
            byteBuffer.ReadBytes(str, 0, strLength);

            message.data = System.Text.Encoding.Default.GetString(str);
            return message;
        }
        public static ByteBuffer Encoder(Message message)
        {
            int size = 0;
            size += (sizeof(Int32) * 7);
            size += message.data.Length;

            byte[] str = System.Text.Encoding.Default.GetBytes(message.data);
            ByteBuffer byteBuffer = ByteBuffer.Allocate(size);

            byteBuffer.WriteInt(message.msgID);
            byteBuffer.WriteInt(message.param);
            byteBuffer.WriteInt(message.param1);
            byteBuffer.WriteInt(message.param2);
            byteBuffer.WriteInt(message.param3);
            byteBuffer.WriteInt(message.param4);
            byteBuffer.WriteInt(message.recog);
            byteBuffer.WriteBytes(str);

            return byteBuffer;
        }
    }
}