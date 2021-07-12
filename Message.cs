using System;
using System.Linq;

namespace Network
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
        public string data = "";

        public override string ToString()
        {
            return string.Format("msgID = {0}\nparam = {1}\nparam1 = {2}\nparam2 = {3}\nparam3 = {4}\nparam4 = {5}\nrecog = {6}\ndata = {7}\n",
             msgID, param, param1, param2, param3, param4, recog, data);
        }
    }
}