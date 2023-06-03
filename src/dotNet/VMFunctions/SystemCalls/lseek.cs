using System.IO;
using xCVM.Core;

namespace SystemCalls
{
    public class lseek : ISysCall
    {
        public void Execute(xCVMCore core)
        {
            var ParameterPointer = core.RegisterToInt32(3);
            var block = core.runtimeData.MemoryBlocks.Datas [ ParameterPointer ];
            var parameter_count = block.data.Length / 4;
            if (parameter_count == 4)
            {
                int ResourceID = core.ReadInt32(block.data , 0);
                _= core.ReadInt32(block.data , 4);
                SEEK seek = (SEEK)core.ReadInt32(block.data , 8);
                int offset = core.ReadInt32(block.data , 16);
                var fs = core.Resources [ ResourceID ] as Stream;
                if (fs is null)
                {
                    core.WriteBytesToRegister(-1 , Constants.retv);
                    return;
                }

                var l = (int)fs.Seek(offset , (SeekOrigin)seek);
                core.WriteBytesToRegister(l , Constants.retv);
                return;
            }
            core.WriteBytesToRegister(-1 , Constants.retv);
            return;
        }
    }
}
