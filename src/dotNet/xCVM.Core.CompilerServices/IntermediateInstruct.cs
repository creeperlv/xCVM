using LibCLCC.NET.TextProcessing;
using System;

namespace xCVM.Core.CompilerServices
{
    public class IntermediateInstruct : Instruct
    {
        [NonSerialized]
        public InstructionDefinition? Definition;
        [NonSerialized]
        public Segment? PseudoOp0;
        [NonSerialized]
        public Segment? PseudoOp1;
        [NonSerialized]
        public Segment? PseudoOp2;
    }
}
