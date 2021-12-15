using System.Collections.Generic;
using System.IO;

namespace Streams.Compression
{
    public class CustomCompressionStream : Stream
    {
        private Stream UnderLayingStream { get; }
        private bool IsReader { get; }
        private List<byte> RemainingValues { get; } = new List<byte>();
        private int Offset { get; set; }

        public CustomCompressionStream(Stream underLayingStream, bool isReader)
        {
            UnderLayingStream = underLayingStream;
            IsReader = isReader;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (!IsReader) return default;
            var values = new List<byte>();
            values.AddRange(RemainingValues);
            RemainingValues.Clear();
            UnderLayingStream.Position = Offset;
            var read = UnderLayingStream.Read(buffer, offset, count);
            for (var i = 1; i < offset + read; i += 2)
            {
                Offset += 2;
                if (values.Count + buffer[i] > buffer.Length)
                {
                    for (var j = 0; j < values.Count + buffer[i] - buffer.Length; j++)
                        RemainingValues.Add(buffer[i - 1]);
                    for (var j = 0; j < buffer[i] - RemainingValues.Count; j++)
                        values.Add(buffer[i - 1]);
                    break;
                }
                for (var j = 0; j < buffer[i]; j++)
                    values.Add(buffer[i - 1]);
            }
            for (var j = 0; j < values.Count; j++)
                buffer[j] = values[j];
            return values.Count;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (IsReader)
                return;
            var toWrite = new List<byte>();
            var byteToAdd = buffer[0];
            var countOfByteToAdd = (byte) 1;
            for (var i = offset + 1; i < offset + count; i++)
            {
                if (buffer[i] == byteToAdd)
                    countOfByteToAdd++;
                else
                {
                    toWrite.Add(byteToAdd);
                    toWrite.Add(countOfByteToAdd);
                    byteToAdd = buffer[i];
                    countOfByteToAdd = 1;
                }
            }
            toWrite.Add(byteToAdd);
            toWrite.Add(countOfByteToAdd);
            UnderLayingStream.Write(toWrite.ToArray(), offset, toWrite.Count);
        }

        public override bool CanRead => UnderLayingStream.CanRead;

        public override bool CanSeek => UnderLayingStream.CanSeek;

        public override bool CanWrite => UnderLayingStream.CanWrite;

        public override void Flush() => UnderLayingStream.Flush();

        public override long Length => UnderLayingStream.Length;

        public override long Position
        {
            get => UnderLayingStream.Position;
            set => UnderLayingStream.Position = value;
        }

        public override long Seek(long offset, SeekOrigin origin) => UnderLayingStream.Seek(offset, origin);

        public override void SetLength(long value) => UnderLayingStream.SetLength(value);
    }
}