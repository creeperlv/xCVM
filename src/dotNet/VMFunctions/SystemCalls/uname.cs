using System;
using System.Text;
using xCVM.Core;

namespace SystemCalls
{
    public class uname : ISysCall
    {
        public void Execute(xCVMCore core)
        {
            var ParameterPointer = core.RegisterToInt32(3);
            var block = core.runtimeData.MemoryBlocks.Datas [ ParameterPointer ];
            var parameter_count = block.data.Length / 4;
            if (parameter_count >= 1)
            {
                int UNAME_STRUCT_ID = core.ReadInt32(block.data , 0);
                var OS = "xCVMK";
                var node = Environment.MachineName;
                var release = "dev";
                var ver = typeof(xCVMCore).Assembly.GetName().Version.ToString();
                var machine = "xCVM";
                var B_OS = Encoding.UTF8.GetBytes(OS);
                var B_NODE = Encoding.UTF8.GetBytes(node);
                var B_RELEASE = Encoding.UTF8.GetBytes(release);
                var B_VER = Encoding.UTF8.GetBytes(ver);
                var B_MACHINE = Encoding.UTF8.GetBytes(machine);

                var I_OS = core.runtimeData.MemoryBlocks.PUT(B_OS);
                var I_NODE = core.runtimeData.MemoryBlocks.PUT(B_NODE);
                var I_RELEASE = core.runtimeData.MemoryBlocks.PUT(B_RELEASE);
                var I_VER = core.runtimeData.MemoryBlocks.PUT(B_VER);
                var I_MACHINE = core.runtimeData.MemoryBlocks.PUT(B_MACHINE);
                core.WriteBytesToMem(I_OS , UNAME_STRUCT_ID , 0);
                core.WriteBytesToMem(I_NODE , UNAME_STRUCT_ID , 4);
                core.WriteBytesToMem(I_RELEASE , UNAME_STRUCT_ID , 8);
                core.WriteBytesToMem(I_VER , UNAME_STRUCT_ID , 12);
                core.WriteBytesToMem(I_MACHINE , UNAME_STRUCT_ID , 16);
                //core.WriteBytesToMem(-1 , UNAME_STRUCT_ID , 20);
            }
            core.WriteBytesToRegister(-1 , 3);
            return;
        }
    }
}
