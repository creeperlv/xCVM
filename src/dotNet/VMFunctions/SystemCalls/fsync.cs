using System.IO;
using xCVM.Core;

namespace SystemCalls
{
    public class fsync : ISysCall
    {
        public void Execute(xCVMCore core)
        {
            var ParameterPointer = core.RegisterToInt32(3);
            var block = core.runtimeData.MemoryBlocks.Datas [ ParameterPointer ];
            var parameter_count = block.data.Length / 4;
            if (parameter_count >= 1)
            {
                int ResourceID = core.ReadInt32(block.data , 0);
                var fs = core.Resources [ ResourceID ] as Stream;
                if (fs is null)
                {
                    core.WriteBytesToRegister(-1 , Constants.retv);
                    return;
                }
                fs.Flush();
                core.WriteBytesToRegister(0 , Constants.retv);
                return;
            }
            core.WriteBytesToRegister(0 , Constants.retv);
            return;
        }
    }
}
