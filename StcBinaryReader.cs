using NLog;
using NLog.Fluent;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFDecompress
{
    public class StcBinaryReader
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public byte[] _buf;
        public int _offset;

        // constructor
        public StcBinaryReader(byte[] buf)
        {
            _buf = buf;
            _offset = 0;
        }

        public byte ReadByte()
        {
            byte output = _buf[_offset];
            log.Trace("offset: {0} | byte: {1}", _offset, output);

            _offset += 1;

            return output;
        }

        public int ReadShort()
        {
            int output = BitConverter.ToInt16(_buf, _offset);
            log.Trace("offset: {0} | short: {1}", _offset, output);

            _offset += 2;

            return output;
        }

        public int ReadUShort()
        {
            int output = BitConverter.ToUInt16(_buf, _offset);
            log.Trace("offset: {0} | u_short: {1}", _offset, output);

            _offset += 2;

            return output;
        }

        public int ReadInt()
        {
            int output = BitConverter.ToInt32(_buf, _offset);
            log.Trace("offset: {0} | int: {1}", _offset, output);

            _offset += 4;

            return output;
        }

        public long ReadLong()
        {
            long output = BitConverter.ToInt64(_buf, _offset);
            log.Trace("offset: {0} | long: {1}", _offset, output);

            _offset += 8;

            return output;
        }

        public string ReadString()
        {
            _offset += 1;   // 오프셋 +1 해야 위치 맞음

            int length = ReadUShort();
            byte[] bytes = new byte[length];
            Array.Copy(_buf, _offset, bytes, 0, length);
            string output = Encoding.UTF8.GetString(bytes);
            output = output.Replace("\\u0000", "").Replace("\u0000", "");
            log.Trace("offset: {0} | length: {1} | string: {2}", _offset, length, output);

            _offset += length;

            return output;
        }
    }
}
