using System;
using System.IO;

namespace xCVM.Core.Utilities
{
    public static class StreamUtilities
    {
        public static void WriteBytes(this Stream stream, byte[] bytes)
        {
            stream.Write(BitConverter.GetBytes(bytes.Length));
            stream.Write(bytes);
        }
        public static byte[] ReadBytes(this Stream stream)
        {
            byte[] b=new byte[4];
            stream.Read(b,0,4);
            int L = BitConverter.ToInt32(b);
            byte[] result = new byte[L];
            stream.Read(result,0,L);
            return result;
        }
    }
}
