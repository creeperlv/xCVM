using xCVM.Core;

namespace SystemCalls
{
    public class close : ISysCall
    {
        public void Execute(xCVMCore core)
        {
            var ParameterPointer = core.RegisterToInt32(3);
            var block = core.runtimeData.MemoryBlocks.Datas [ ParameterPointer ];
            var parameter_count = block.data.Length / 4;
            if (parameter_count >= 1)
            {
                try
                {

                    int ResourceID = core.ReadInt32(block.data , 0);
                    var fs = core.Resources [ ResourceID ];
                    fs.Dispose();
                    core.Resources.Remove(ResourceID);
                    core.WriteBytesToRegister(0 , Constants.retv);
                }
                catch (System.Exception)
                {
                    core.WriteBytesToRegister(-1 , Constants.retv);
                }
                return;
            }
            core.WriteBytesToRegister(0 , Constants.retv);
            return;
        }
    }
}
