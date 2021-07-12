using System;
namespace Network
{
    public class ProtoMsgProtocol
    {
        public static Message Decoder(byte[] b)
        {
            Message message = new Message();
            int intSize = sizeof(int);
            byte[] bInt = new byte[intSize];
            int start = 0;

            Array.Copy(b, start, bInt, 0, intSize);
            start += intSize;
            message.msgID = BitConverter.ToInt32(bInt, 0);

            Array.Copy(b, start, bInt, 0, intSize);
            start += intSize;
            message.param = BitConverter.ToInt32(bInt, 0);

            Array.Copy(b, start, bInt, 0, intSize);
            start += intSize;
            message.param1 = BitConverter.ToInt32(bInt, 0);

            Array.Copy(b, start, bInt, 0, intSize);
            start += intSize;
            message.param2 = BitConverter.ToInt32(bInt, 0);

            Array.Copy(b, start, bInt, 0, intSize);
            start += intSize;
            message.param3 = BitConverter.ToInt32(bInt, 0);

            Array.Copy(b, start, bInt, 0, intSize);
            start += intSize;
            message.param4 = BitConverter.ToInt32(bInt, 0);

            Array.Copy(b, start, bInt, 0, intSize);
            start += intSize;
            message.recog = BitConverter.ToInt32(bInt, 0);

            int strLength = b.Length - start;
            byte[] str = new byte[strLength];
            Array.Copy(b, start, str, 0, strLength);
            message.data = System.Text.Encoding.Default.GetString(str);
            return message;
        }
        public static byte[] Encoder(Message message)
        {
            byte[] bmsgID = BitConverter.GetBytes(message.msgID);
            byte[] bParam = BitConverter.GetBytes(message.param);
            byte[] bParam1 = BitConverter.GetBytes(message.param1);
            byte[] bParam2 = BitConverter.GetBytes(message.param2);
            byte[] bParam3 = BitConverter.GetBytes(message.param3);
            byte[] bParam4 = BitConverter.GetBytes(message.param4);
            byte[] bRecog = BitConverter.GetBytes(message.recog);
            byte[] str = System.Text.Encoding.Default.GetBytes(message.data);

            byte[] sendbuff = new byte[bmsgID.Length + bParam.Length + bParam1.Length + bParam2.Length + bParam3.Length + bParam4.Length + bRecog.Length + str.Length];
            int start = 0;
            Array.Copy(bmsgID, sendbuff, bmsgID.Length);
            start += bmsgID.Length;

            Array.Copy(bParam, 0, sendbuff, start, bParam.Length);
            start += bParam.Length;

            Array.Copy(bParam1, 0, sendbuff, start, bParam1.Length);
            start += bParam1.Length;

            Array.Copy(bParam2, 0, sendbuff, start, bParam2.Length);
            start += bParam2.Length;

            Array.Copy(bParam3, 0, sendbuff, start, bParam3.Length);
            start += bParam3.Length;

            Array.Copy(bParam4, 0, sendbuff, start, bParam4.Length);
            start += bParam4.Length;

            Array.Copy(bRecog, 0, sendbuff, start, bRecog.Length);
            start += bRecog.Length;
            
            Array.Copy(str, 0, sendbuff, start, str.Length);
            return sendbuff;
        }
    }
}