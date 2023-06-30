using System.Collections.Generic;

namespace xCVM.Core
{
    public class RuntimeData
    {
        public xCVMem Registers;
        public xCVMemBlock MemoryBlocks;

        public Dictionary<int , xCVMem> PrivateData = new Dictionary<int , xCVMem>();
        public RuntimeData(xCVMem registers , xCVMemBlock memoryBlocks , Dictionary<int , xCVMem> privateData)
        {
            Registers = registers;
            MemoryBlocks = memoryBlocks;
            PrivateData = privateData;
        }
    }
}
