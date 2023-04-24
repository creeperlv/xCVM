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
    public class IntermediateExternFunction: ExternFunction
    {
        [NonSerialized]
        public Segment? PseudoLabel;
    }
    public class IntermediateExternStruct : ExternStruct
    {
        [NonSerialized]
        public Segment? PseudoName;
    }
    public class IntermediateDataType : DataType {

        [NonSerialized]
        public Segment? PseudoType;

        [NonSerialized]
        public Segment? PseudoAdditionalType;
    }

}
