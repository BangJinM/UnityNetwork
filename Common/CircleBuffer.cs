using System;

namespace USC
{
    public class CircularBuffer
    {
        //容量
        private int capacity;
        //当前大小
        volatile private int length;
        //读数据位置
        volatile private int readIndex;
        //写数据位置
        volatile private int writeIndex;
        //数据
        volatile private byte[] buffer;


        public CircularBuffer(int capacity)
        {
            length = 0;
            readIndex = 0;
            writeIndex = 0;
            buffer = new byte[capacity];
            this.capacity = capacity;
        }

        //获取或设置缓冲区大小
        public int Capacity
        {
            get { return capacity; }
            set
            {
                if (value == capacity)
                    return;

                if (value < length)
                    throw new ArgumentOutOfRangeException("value", "Capacity to small");

                var dst = new byte[value];
                if (length > 0)
                    CopyTo(dst);
                buffer = dst;

                capacity = value;
            }
        }

        public int GetLength()
        {
            return length;
        }

        public void Clear()
        {
            length = 0;
            readIndex = 0;
            writeIndex = 0;
        }

        public int WriteBytes(byte[] bytes)
        {
            return WriteBytes(bytes, 0, bytes.Length);
        }

        public int WriteBytes(byte[] bytes, int length)
        {
            return WriteBytes(bytes, 0, length);
        }

        /**
	     * 将bytes字节数组从offset开始的count字节写入到此缓存区
	     */
        public int WriteBytes(byte[] bytes, int offset, int count)
        {
            lock (this)
            {
                if (count > capacity - length)
                    throw new InvalidOperationException("Buffer Overflow");

                int srcIndex = offset;
                for (int i = 0; i < count; i++, writeIndex++, srcIndex++)
                {
                    if (writeIndex == capacity)
                        writeIndex = 0;
                    buffer[writeIndex] = bytes[srcIndex];
                }
                length = Math.Min(length + count, capacity);
                return count;
            }
        }

        public void WriteByte(byte b)
        {
            lock (this)
            {
                if (length >= capacity)
                    throw new InvalidOperationException("Buffer Overflow");

                buffer[writeIndex] = b;
                if (++writeIndex == capacity)
                    writeIndex = 0;
                length++;
            }
        }

        /**
	     * 翻转字节数组，如果本地字节序列为低字节序列，则进行翻转以转换为高字节序列
	     */
        private byte[] Flip(byte[] bytes)
        {
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }
            return bytes;
        }

        /**
	     * 写入一个int16数据
	     */
        public void WriteShort(short value)
        {
            WriteBytes(Flip(BitConverter.GetBytes(value)));
        }

        /**
	     * 写入一个uint16数据
	     */
        public void WriteUshort(ushort value)
        {
            WriteBytes(Flip(BitConverter.GetBytes(value)));
        }

        /**
	     * 写入一个int32数据
	     */
        public void WriteInt(int value)
        {
            WriteBytes(Flip(BitConverter.GetBytes(value)));
        }

        /**
	     * 写入一个uint32数据
	     */
        public void WriteUint(uint value)
        {
            WriteBytes(Flip(BitConverter.GetBytes(value)));
        }

        /**
	     * 写入一个int64数据
	     */
        public void WriteLong(long value)
        {
            WriteBytes(Flip(BitConverter.GetBytes(value)));
        }

        /**
	     * 写入一个uint64数据
	     */
        public void WriteUlong(ulong value)
        {
            WriteBytes(Flip(BitConverter.GetBytes(value)));
        }

        /**
	     * 写入一个float数据
	     */
        public void WriteFloat(float value)
        {
            WriteBytes(Flip(BitConverter.GetBytes(value)));
        }


        /**
                 * 写入一个double类型数据
                 */
        public void WriteDouble(double value)
        {
            WriteBytes(Flip(BitConverter.GetBytes(value)));
        }

        /**
	     * 读取一个字节
	     */
        public byte ReadByte()
        {
            return ReadBytes(1)[0];
        }

        /**
	     * 从读取索引位置开始读取count个字节
	     */
        public byte[] ReadBytes(int count)
        {
            lock (this)
            {
                byte[] bytes = new byte[count];
                if (count > length)
                    throw new InvalidOperationException("Buffer Overflow");
                int dstIndex = 0;
                for (int i = 0; i < count; i++, readIndex++, dstIndex++)
                {
                    if (readIndex >= capacity)
                        readIndex = 0;
                    bytes[dstIndex] = buffer[readIndex];
                }
                length -= count;
                return bytes;
            }
        }

        /**
	     * 读取一个uint16数据
	     */
        public ushort ReadUshort()
        {
            return BitConverter.ToUInt16(ReadBytes(2), 0);
        }

        /**
	     * 读取一个int16数据
	     */
        public short ReadShort()
        {
            return BitConverter.ToInt16(ReadBytes(2), 0);
        }

        /**
	     * 读取一个uint32数据
	     */
        public uint ReadUint()
        {
            return BitConverter.ToUInt32(ReadBytes(4), 0);
        }

        /**
	     * 读取一个int32数据
	     */
        public int ReadInt()
        {
            return BitConverter.ToInt32(ReadBytes(4), 0);
        }

        /**
	     * 读取一个uint64数据
	     */
        public ulong ReadUlong()
        {
            return BitConverter.ToUInt64(ReadBytes(8), 0);
        }

        /**
	     * 读取一个long数据
	     */
        public long ReadLong()
        {
            return BitConverter.ToInt64(ReadBytes(8), 0);
        }

        /**
	     * 读取一个float数据
	     */
        public float ReadFloat()
        {
            return BitConverter.ToSingle(ReadBytes(4), 0);
        }

        /**
	     * 读取一个double数据
	     */
        public double ReadDouble()
        {
            return BitConverter.ToDouble(ReadBytes(8), 0);
        }


        public void Skip(int count)
        {
            lock (this)
            {
                readIndex += count;
                if (readIndex >= capacity)
                    readIndex -= capacity;
            }
        }


        public byte[] GetBuffer()
        {
            return buffer;
        }


        public void CopyTo(byte[] array)
        {
            CopyTo(array, 0);
        }

        public void CopyTo(byte[] array, int arrayIndex)
        {
            CopyTo(0, array, arrayIndex, length);
        }
        //拷贝缓冲区的数据到指定数组的指定位置，如果缓冲区的数量不足，则抛出异常
        public void CopyTo(int index, byte[] array, int arrayIndex, int count)
        {
            if (count > length)
                throw new ArgumentOutOfRangeException("count", "Read size to large");

            int bufferIndex = readIndex;
            for (int i = 0; i < count; i++, bufferIndex++, arrayIndex++)
            {
                if (bufferIndex == capacity)
                    bufferIndex = 0;
                array[arrayIndex] = buffer[bufferIndex];
            }
        }
    }
}