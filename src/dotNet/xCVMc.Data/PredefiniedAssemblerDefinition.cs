using System.Collections.Generic;
using xCVM.Core;
using xCVM.Core.CompilerServices;

namespace xCVMc.Data
{
    public static class PredefiniedAssemblerDefinition
    {
        public static AssemblerDefinition GetDefinition()
        {
            AssemblerDefinition assemblerDefinition = new AssemblerDefinition { StatementEndMark = ";" , UseStatementEndMark = true };
            assemblerDefinition.PredefinedSymbols = new Dictionary<string , string> {
                { "stdin", "0" },
                { "stdout", "1" },
            };
            assemblerDefinition.PredefinedTypeMapping = new Dictionary<string , int>
            {
                { "struct" , -2 },
                { "void" , -1 },
                { "cli" , 0 },
                { "int" , 1 },
                { "long" , 2 },
                { "float" , 3 },
                { "double" , 4 },
                { "uint" , 5 },
                { "unsigned_int" , 5 },
                { "ulong" , 6 },
            };
            assemblerDefinition.Definitions = new List<InstructionDefinition>
            {
                new InstructionDefinition{ Name="malloc",  ID=(int)Inst.malloc, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=true,OP2DT=-2, OP2REG=true},
                new InstructionDefinition{ Name="realloc",  ID=(int)Inst.realloc, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=true,OP2DT=1, OP2REG=true},
                new InstructionDefinition{ Name="free",  ID=(int)Inst.free, OP0DT=1, OP0REG=true, OP1DT=-2,OP1REG=true,OP2DT=-2, OP2REG=true},

                new InstructionDefinition{ Name="add",  ID=(int)Inst.add, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=true,OP2DT=1, OP2REG=true},
                new InstructionDefinition{ Name="addi", ID=(int)Inst.addi, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=false,OP2DT=1, OP2REG=true},
                new InstructionDefinition{ Name="sub",  ID=(int)Inst.sub, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=true,OP2DT=1, OP2REG=true},
                new InstructionDefinition{ Name="subi", ID=(int)Inst.subi, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=false,OP2DT=1, OP2REG=true},
                new InstructionDefinition{ Name="mul",  ID=(int)Inst.mul, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=true,OP2DT=1, OP2REG=true},
                new InstructionDefinition{ Name="muli", ID=(int)Inst.muli, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=false,OP2DT=1, OP2REG=true},
                new InstructionDefinition{ Name="div",  ID=(int)Inst.div, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=true,OP2DT=1, OP2REG=true},
                new InstructionDefinition{ Name="divi", ID=(int)Inst.divi, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=false,OP2DT=1, OP2REG=true},
                new InstructionDefinition{ Name="uadd",  ID=(int)Inst.uadd, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=true,OP2DT=1, OP2REG=true},
                new InstructionDefinition{ Name="uaddi", ID=(int)Inst.uaddi, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=false,OP2DT=1, OP2REG=true},
                new InstructionDefinition{ Name="usub",  ID=(int)Inst.usub, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=true,OP2DT=1, OP2REG=true},
                new InstructionDefinition{ Name="usubi", ID=(int)Inst.usubi, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=false,OP2DT=1, OP2REG=true},
                new InstructionDefinition{ Name="umul",  ID=(int)Inst.umul, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=true,OP2DT=1, OP2REG=true},
                new InstructionDefinition{ Name="umuli", ID=(int)Inst.umuli, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=false,OP2DT=1, OP2REG=true},
                new InstructionDefinition{ Name="udiv",  ID=(int)Inst.udiv, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=true,OP2DT=1, OP2REG=true},
                new InstructionDefinition{ Name="udivi", ID=(int)Inst.udivi, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=false,OP2DT=1, OP2REG=true},

                new InstructionDefinition{ Name="ladd",  ID=(int)Inst.ladd, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=true,OP2DT=1, OP2REG=true},
                new InstructionDefinition{ Name="laddi", ID=(int)Inst.laddi, OP0DT=1, OP0REG=true, OP1DT=2,OP1REG=false,OP2DT=1, OP2REG=true},
                new InstructionDefinition{ Name="lsub",  ID=(int)Inst.lsub, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=true,OP2DT=1, OP2REG=true},
                new InstructionDefinition{ Name="lsubi", ID=(int)Inst.lsubi, OP0DT=1, OP0REG=true, OP1DT=2,OP1REG=false,OP2DT=1, OP2REG=true},
                new InstructionDefinition{ Name="lmul",  ID=(int)Inst.lmul, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=true,OP2DT=1, OP2REG=true},
                new InstructionDefinition{ Name="lmuli", ID=(int)Inst.lmuli, OP0DT=1, OP0REG=true, OP1DT=2,OP1REG=false,OP2DT=1, OP2REG=true},
                new InstructionDefinition{ Name="ldiv",  ID=(int)Inst.ldiv, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=true,OP2DT=1, OP2REG=true},
                new InstructionDefinition{ Name="ldivi", ID=(int)Inst.ldivi, OP0DT=1, OP0REG=true, OP1DT=2,OP1REG=false,OP2DT=1, OP2REG=true},

                new InstructionDefinition{ Name="fadd_s",  ID=(int)Inst.fadd_s, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=true,OP2DT=1, OP2REG=true},
                new InstructionDefinition{ Name="faddi_s", ID=(int)Inst.faddi_s, OP0DT=1, OP0REG=true, OP1DT=3,OP1REG=false,OP2DT=1, OP2REG=true},
                new InstructionDefinition{ Name="fsub_s",  ID=(int)Inst.fsub_s, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=true,OP2DT=1, OP2REG=true},
                new InstructionDefinition{ Name="fsubi_s", ID=(int)Inst.fsubi_s, OP0DT=1, OP0REG=true, OP1DT=3,OP1REG=false,OP2DT=1, OP2REG=true},
                new InstructionDefinition{ Name="fmul_s",  ID=(int)Inst.fmul_s, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=true,OP2DT=1, OP2REG=true},
                new InstructionDefinition{ Name="fmuli_s", ID=(int)Inst.fmuli_s, OP0DT=1, OP0REG=true, OP1DT=3,OP1REG=false,OP2DT=1, OP2REG=true},
                new InstructionDefinition{ Name="fdiv_s",  ID=(int)Inst.fdiv_s, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=true,OP2DT=1, OP2REG=true},
                new InstructionDefinition{ Name="fdivi_s", ID=(int)Inst.fdivi_s, OP0DT=1, OP0REG=true, OP1DT=3,OP1REG=false,OP2DT=1, OP2REG=true},

                new InstructionDefinition{ Name="fadd_d",  ID=(int)Inst.fadd_d, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=true,OP2DT=1, OP2REG=true},
                new InstructionDefinition{ Name="faddi_d", ID=(int)Inst.faddi_d, OP0DT=1, OP0REG=true, OP1DT=4,OP1REG=false,OP2DT=1, OP2REG=true},
                new InstructionDefinition{ Name="fsub_d",  ID=(int)Inst.fsub_d, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=true,OP2DT=1, OP2REG=true},
                new InstructionDefinition{ Name="fsubi_d", ID=(int)Inst.fsubi_d, OP0DT=1, OP0REG=true, OP1DT=4,OP1REG=false,OP2DT=1, OP2REG=true},
                new InstructionDefinition{ Name="fmul_d",  ID=(int)Inst.fmul_d, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=true,OP2DT=1, OP2REG=true},
                new InstructionDefinition{ Name="fmuli_d", ID=(int)Inst.fmuli_d, OP0DT=1, OP0REG=true, OP1DT=4,OP1REG=false,OP2DT=1, OP2REG=true},
                new InstructionDefinition{ Name="fdiv_d",  ID=(int)Inst.fdiv_d, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=true,OP2DT=1, OP2REG=true},
                new InstructionDefinition{ Name="fdivi_d", ID=(int)Inst.fdivi_d, OP0DT=1, OP0REG=true, OP1DT=4,OP1REG=false,OP2DT=1, OP2REG=true},
            };
            return assemblerDefinition;
        }
    }
}