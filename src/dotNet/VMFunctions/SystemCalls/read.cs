using System;
using System.IO;
using xCVM.Core;

namespace SystemCalls
{
    public class read : ISysCall
    {
        public void Execute(xCVMCore core)
        {
            var ParameterPointer = core.RegisterToInt32(3);
            var block = core.runtimeData.MemoryBlocks.Datas [ ParameterPointer ];
            var parameter_count = block.data.Length / 4;
            if (parameter_count == 3)
            {
                int ResourceID = core.ReadInt32(block.data , 0);
                int data = core.ReadInt32(block.data , 4);
                int count = core.ReadInt32(block.data , 8);
                var fs = core.Resources [ ResourceID ] as Stream;
                if (fs is null)
                {
                    core.WriteBytesToRegister(-1 , Constants.retv);
                    return;
                }
                var c = fs.Read(core.runtimeData.MemoryBlocks.Datas [ data ].data , 0 , count);
                core.WriteBytesToRegister(c , Constants.retv);
                return;
            }
            core.WriteBytesToRegister(-1 , Constants.retv);
            return;
        }
    }
    public enum SEEK
    {
        SEEK_SET = 0, SEEK_CUR = 1, SEEK_END = 2
    }
}
