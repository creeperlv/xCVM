using System.Collections.Generic;
using xCVM.Core;
using xCVM.Core.CompilerServices;

namespace xCVMc.Data
{
    public static class PredefiniedAssemblerDefinition
    {
        public static AssemblerDefinition GetDefinition()
        {
            AssemblerDefinition assemblerDefinition = new AssemblerDefinition { StateMentEndMark = ";", UseStatementEndMark = true };
            assemblerDefinition.PredefinedSymbols = new Dictionary<string, string> { 
                { "stdin", "0" },
                { "stdout", "1" },
            };
            assemblerDefinition.Definitions = new List<Instruction3OperatorsDefinition>
            {
                new Instruction3OperatorsDefinition{ Name="add",  ID=(int)Inst.add, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=true,OP2DT=1, OP2REG=true},
                new Instruction3OperatorsDefinition{ Name="addi", ID=(int)Inst.addi, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=false,OP2DT=1, OP2REG=true},
                new Instruction3OperatorsDefinition{ Name="sub",  ID=(int)Inst.sub, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=true,OP2DT=1, OP2REG=true},
                new Instruction3OperatorsDefinition{ Name="subi", ID=(int)Inst.subi, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=false,OP2DT=1, OP2REG=true},
                new Instruction3OperatorsDefinition{ Name="mul",  ID=(int)Inst.mul, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=true,OP2DT=1, OP2REG=true},
                new Instruction3OperatorsDefinition{ Name="muli", ID=(int)Inst.muli, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=false,OP2DT=1, OP2REG=true},
                new Instruction3OperatorsDefinition{ Name="div",  ID=(int)Inst.div, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=true,OP2DT=1, OP2REG=true},
                new Instruction3OperatorsDefinition{ Name="divi", ID=(int)Inst.divi, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=false,OP2DT=1, OP2REG=true},

                new Instruction3OperatorsDefinition{ Name="ladd",  ID=(int)Inst.ladd, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=true,OP2DT=1, OP2REG=true},
                new Instruction3OperatorsDefinition{ Name="laddi", ID=(int)Inst.laddi, OP0DT=1, OP0REG=true, OP1DT=2,OP1REG=false,OP2DT=1, OP2REG=true},
                new Instruction3OperatorsDefinition{ Name="lsub",  ID=(int)Inst.lsub, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=true,OP2DT=1, OP2REG=true},
                new Instruction3OperatorsDefinition{ Name="lsubi", ID=(int)Inst.lsubi, OP0DT=1, OP0REG=true, OP1DT=2,OP1REG=false,OP2DT=1, OP2REG=true},
                new Instruction3OperatorsDefinition{ Name="lmul",  ID=(int)Inst.lmul, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=true,OP2DT=1, OP2REG=true},
                new Instruction3OperatorsDefinition{ Name="lmuli", ID=(int)Inst.lmuli, OP0DT=1, OP0REG=true, OP1DT=2,OP1REG=false,OP2DT=1, OP2REG=true},
                new Instruction3OperatorsDefinition{ Name="ldiv",  ID=(int)Inst.ldiv, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=true,OP2DT=1, OP2REG=true},
                new Instruction3OperatorsDefinition{ Name="ldivi", ID=(int)Inst.ldivi, OP0DT=1, OP0REG=true, OP1DT=2,OP1REG=false,OP2DT=1, OP2REG=true},

                new Instruction3OperatorsDefinition{ Name="fadd_s",  ID=(int)Inst.fadd_s, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=true,OP2DT=1, OP2REG=true},
                new Instruction3OperatorsDefinition{ Name="faddi_s", ID=(int)Inst.faddi_s, OP0DT=1, OP0REG=true, OP1DT=3,OP1REG=false,OP2DT=1, OP2REG=true},
                new Instruction3OperatorsDefinition{ Name="fsub_s",  ID=(int)Inst.fsub_s, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=true,OP2DT=1, OP2REG=true},
                new Instruction3OperatorsDefinition{ Name="fsubi_s", ID=(int)Inst.fsubi_s, OP0DT=1, OP0REG=true, OP1DT=3,OP1REG=false,OP2DT=1, OP2REG=true},
                new Instruction3OperatorsDefinition{ Name="fmul_s",  ID=(int)Inst.fmul_s, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=true,OP2DT=1, OP2REG=true},
                new Instruction3OperatorsDefinition{ Name="fmuli_s", ID=(int)Inst.fmuli_s, OP0DT=1, OP0REG=true, OP1DT=3,OP1REG=false,OP2DT=1, OP2REG=true},
                new Instruction3OperatorsDefinition{ Name="fdiv_s",  ID=(int)Inst.fdiv_s, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=true,OP2DT=1, OP2REG=true},
                new Instruction3OperatorsDefinition{ Name="fdivi_s", ID=(int)Inst.fdivi_s, OP0DT=1, OP0REG=true, OP1DT=3,OP1REG=false,OP2DT=1, OP2REG=true},

                new Instruction3OperatorsDefinition{ Name="fadd_d",  ID=(int)Inst.fadd_d, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=true,OP2DT=1, OP2REG=true},
                new Instruction3OperatorsDefinition{ Name="faddi_d", ID=(int)Inst.faddi_d, OP0DT=1, OP0REG=true, OP1DT=4,OP1REG=false,OP2DT=1, OP2REG=true},
                new Instruction3OperatorsDefinition{ Name="fsub_d",  ID=(int)Inst.fsub_d, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=true,OP2DT=1, OP2REG=true},
                new Instruction3OperatorsDefinition{ Name="fsubi_d", ID=(int)Inst.fsubi_d, OP0DT=1, OP0REG=true, OP1DT=4,OP1REG=false,OP2DT=1, OP2REG=true},
                new Instruction3OperatorsDefinition{ Name="fmul_d",  ID=(int)Inst.fmul_d, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=true,OP2DT=1, OP2REG=true},
                new Instruction3OperatorsDefinition{ Name="fmuli_d", ID=(int)Inst.fmuli_d, OP0DT=1, OP0REG=true, OP1DT=4,OP1REG=false,OP2DT=1, OP2REG=true},
                new Instruction3OperatorsDefinition{ Name="fdiv_d",  ID=(int)Inst.fdiv_d, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=true,OP2DT=1, OP2REG=true},
                new Instruction3OperatorsDefinition{ Name="fdivi_d", ID=(int)Inst.fdivi_d, OP0DT=1, OP0REG=true, OP1DT=4,OP1REG=false,OP2DT=1, OP2REG=true},
            };
            return assemblerDefinition;
        }
    }
}