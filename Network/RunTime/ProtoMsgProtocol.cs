using System;
using XLua;

namespace US.Net
{
    public class ProtoMsgProtocol
    {
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
            byteBuffer.WriteBytes(str);

            return byteBuffer;
        }
    }
}