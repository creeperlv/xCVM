using System;
using System.IO;
using System.Text;
using xCVM.Core;

namespace SystemCalls
{
    public class write : ISysCall
    {
        public void Execute(xCVMCore core)
        {
            var ParameterPointer = core.RegisterToInt32(Constants.fpp);
            var block = core.runtimeData.MemoryBlocks.Datas [ ParameterPointer ];
            var parameter_count = block.data.Length / 4;
            if (parameter_count == 4)
            {
                int ResourceID = core.ReadInt32(block.data , 0);
                int data = core.ReadInt32(block.data , 4);
                int offset = core.ReadInt32(block.data , 8);
                int count = core.ReadInt32(block.data , 12);
                var fs = core.Resources [ ResourceID ] as Stream;
                if (fs is null)
                {
                    core.WriteBytesToRegister(-1 , 1);
                    return;
                }
                var d = core.runtimeData.MemoryBlocks.Datas [ data ].data;
                if (count >= 0)
                {
                    fs.Write(d , offset , count);
                    core.WriteBytesToRegister(count , Constants.retv);

                }
                else
                {
                    fs.Write(d , offset , d.Length);
                    core.WriteBytesToRegister(d.Length , Constants.retv);
                }
                return;
            }
            else
            {
                Console.WriteLine("???");
            }
            core.WriteBytesToRegister(-1 , Constants.retv);
            return;
        }
    }
}
