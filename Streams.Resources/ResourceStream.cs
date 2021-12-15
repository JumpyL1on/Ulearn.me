using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Streams.Resources
{
    public class ResourceReaderStream : Stream
    {
        private Stream UnderLayingStream { get; }
        private string Value { get; }
        private List<byte> ValueRepresentation { get; } = new List<byte>();
        private List<byte> FieldValueRepresentation { get; } = new List<byte>();
        private int RVP { get; set; } // RVP - ReadValuePosition
        private int RVC { get; set; } // RVC - ReadValuesCount
        private bool IsValueFound { get; set; }
        private bool IsFieldValueFound { get; set; }

        public ResourceReaderStream(Stream stream, string key)
        {
            UnderLayingStream = stream;
            Value = key;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            SeekValue(offset, IsValueFound);
            if (!IsValueFound) return default;
            ReadFieldValueWithoutGainedBuffer(offset, IsFieldValueFound);
            if (RVP == FieldValueRepresentation.Count) return default;
            RVC = 0;
            for (var i = 0; i < buffer.Length && RVP < FieldValueRepresentation.Count; i++, RVP++, RVC++)
                buffer[i] = FieldValueRepresentation[RVP];
            return RVC;
        }

        private bool CheckValue(IReadOnlyList<byte> bufferForSeekValue, ref int i)
        {
            if (Value != Encoding.ASCII.GetString(ValueRepresentation.ToArray()))
            {
                i += 2;
                ValueRepresentation.Clear();
                return false;
            }
            IsValueFound = true;
            var bufferForReadFieldValue = new List<byte>();
            for (var j = i + 3; j < bufferForSeekValue.Count; j++)
                bufferForReadFieldValue.Add(bufferForSeekValue[j]);
            ReadFieldValueWithGainedBuffer(bufferForReadFieldValue);
            return true;
        }

        private void SeekValue(int offset, bool isValueFound)
        {
            if (isValueFound) return;
            while (true)
            {
                var bufferForSeekValue = new byte[Constants.BufferSize];
                var read = UnderLayingStream.Read(bufferForSeekValue, offset, Constants.BufferSize);
                for (var i = 0; i < bufferForSeekValue.Length - 2; i++)
                {
                    ValueRepresentation.Add(bufferForSeekValue[i]);
                    if (bufferForSeekValue[i + 1] == 0 && bufferForSeekValue[i + 2] == 1)
                        if (CheckValue(bufferForSeekValue, ref i)) return;
                }
                for (var i = bufferForSeekValue.Length - 2; i < bufferForSeekValue.Length; i++)
                    ValueRepresentation.Add(bufferForSeekValue[i]);
                if (read < Constants.BufferSize) break;
            }
        }

        private void ReadFieldValueWithGainedBuffer(IReadOnlyList<byte> bufferForReadFieldValue)
        {
            for (var i = 0; i < bufferForReadFieldValue.Count - 2; i++)
            {
                FieldValueRepresentation.Add(bufferForReadFieldValue[i]);
                if (bufferForReadFieldValue[i + 1] == 0 && bufferForReadFieldValue[i + 2] == 0)
                {
                    i++;
                    continue;;
                }
                if (bufferForReadFieldValue[i + 1] == 0 && bufferForReadFieldValue[i + 2] == 1)
                {
                    IsFieldValueFound = true;
                    return;
                }
            }
            for (var i = bufferForReadFieldValue.Count - 2; i < bufferForReadFieldValue.Count; i++)
                FieldValueRepresentation.Add(bufferForReadFieldValue[i]);
        }

        private void ReadFieldValueWithoutGainedBuffer(int offset, bool isFieldValueFound)
        {
            if (isFieldValueFound) return;
            while (!IsFieldValueFound)
            {
                var bufferForReadFieldValue = new byte[Constants.BufferSize];
                var read = UnderLayingStream.Read(bufferForReadFieldValue, offset, Constants.BufferSize);
                for (var i = 0; i < bufferForReadFieldValue.Length - 2; i++)
                {
                    FieldValueRepresentation.Add(bufferForReadFieldValue[i]);
                    if (bufferForReadFieldValue[i + 1] == 0 && bufferForReadFieldValue[i + 2] == 0)
                    {
                        i++;
                        continue; 
                    }
                    if (bufferForReadFieldValue[i + 1] == 0 && bufferForReadFieldValue[i + 2] == 1)
                    {
                        IsFieldValueFound = true;
                        return;
                    }
                }
                for (var i = bufferForReadFieldValue.Length - 2; i < bufferForReadFieldValue.Length; i++)
                    FieldValueRepresentation.Add(bufferForReadFieldValue[i]);
                if (read < Constants.BufferSize) break;
            }
        }

        public override void Write(byte[] buffer, int offset, int count) => 
            throw new NotImplementedException();

        public override bool CanRead => 
            UnderLayingStream.CanRead;

        public override bool CanSeek => 
            UnderLayingStream.CanSeek;

        public override bool CanWrite => 
            UnderLayingStream.CanWrite;

        public override void Flush() => 
            UnderLayingStream.Flush();

        public override long Length => 
            UnderLayingStream.Length;

        public override long Position
        {
            get => 
                UnderLayingStream.Position;
            set => 
                UnderLayingStream.Position = value;
        }

        public override long Seek(long offset, SeekOrigin origin) 
            => UnderLayingStream.Seek(offset, origin);

        public override void SetLength(long value) 
            => UnderLayingStream.SetLength(value);
    }
}
