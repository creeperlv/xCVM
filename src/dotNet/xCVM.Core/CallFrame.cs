using System;

namespace xCVM.Core
{
    public struct CallFrame
    {
        public int Module;
        public int IP;
        public int MainStack;
        public CallFrame(int module , int iP,int mainStack)
        {
            Module = module;
            IP = iP;
            MainStack= mainStack;
        }
        public void WriteToBytes(byte [ ] bytes , int Offset , byte [ ] buffer_4_bytes)
        {
            BitConverter.TryWriteBytes(buffer_4_bytes , Module);
            buffer_4_bytes.CopyTo(bytes , Offset);
            BitConverter.TryWriteBytes(buffer_4_bytes , IP);
            buffer_4_bytes.CopyTo(bytes , Offset + 4);
            BitConverter.TryWriteBytes(buffer_4_bytes , MainStack);
            buffer_4_bytes.CopyTo(bytes , Offset + 8);

        }
        public static CallFrame FromBytes(byte [ ] bytes , int Offset)
        {
            int M = BitConverter.ToInt32(bytes , Offset);
            int IP = BitConverter.ToInt32(bytes , Offset + 4);
            int MS = BitConverter.ToInt32(bytes , Offset + 8);
            return new CallFrame(M , IP,MS);
        }
    }
}
