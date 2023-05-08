namespace xCVM.Core
{
    public class RuntimeData
    {
        public xCVMem Registers;
        public xCVMemBlock MemoryBlocks;

        public RuntimeData(xCVMem registers , xCVMemBlock memoryBlocks)
        {
            Registers = registers;
            MemoryBlocks = memoryBlocks;
        }
    }
}
