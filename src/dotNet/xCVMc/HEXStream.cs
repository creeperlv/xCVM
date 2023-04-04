namespace xCVM.Compiler
{
    public class HEXStream : Stream
    {
        public TextWriter UnderlyingWriter;
        public TextReader UnderlyingReader;
        public override bool CanRead => true;

        public override bool CanSeek => throw new NotImplementedException();

        public override bool CanWrite => true;

        public override long Length => throw new NotImplementedException();

        public override long Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override void Flush()
        {
            UnderlyingWriter.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            char[] chars = new char[count];
            var c = UnderlyingReader.ReadBlock(chars, 0, count);
            var b = Convert.FromHexString(chars);
            for (int i = 0; i < c; i++)
            {
                buffer[i] = b[i];
            }
            return c;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            UnderlyingWriter.Write(Convert.ToHexString(buffer, offset, count));
        }
    }
}