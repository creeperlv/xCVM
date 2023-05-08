using System;

namespace xCVM.Core
{
    public struct ProgramPosition
    {
        public int Module;
        public int IP;

        public ProgramPosition(int module , int iP)
        {
            Module = module;
            IP = iP;
        }
        public void WriteToBytes(byte [ ] bytes , int Offset , byte [ ] buffer_4_bytes)
        {
            BitConverter.TryWriteBytes(buffer_4_bytes , Module);
            buffer_4_bytes.CopyTo(bytes , Offset);
            BitConverter.TryWriteBytes(buffer_4_bytes , IP);
            buffer_4_bytes.CopyTo(bytes , Offset + 4);

        }
        public static ProgramPosition FromBytes(byte [ ] bytes , int Offset)
        {
            int M = BitConverter.ToInt32(bytes , Offset);
            int IP = BitConverter.ToInt32(bytes , Offset + 4);
            return new ProgramPosition(M , IP);
        }
    }
}
