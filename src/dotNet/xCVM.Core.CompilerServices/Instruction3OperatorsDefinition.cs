using System;

namespace xCVM.Core.CompilerServices
{
    [Serializable]
    public class Instruction3OperatorsDefinition : IContentable
    {
        public string Name;
        public int ID;
        public int OP0DT;
        public bool OP0REG;
        public int OP1DT;
        public bool OP1REG;
        public int OP2DT;
        public bool OP2REG;
        public string Content => Name;
    }
}
