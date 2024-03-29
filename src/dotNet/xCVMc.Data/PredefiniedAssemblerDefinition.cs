﻿using System.Collections.Generic;
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
                { "NULL", "-1" },
                { "null", "-1" },
                { "stdout", "1" },

                { "retv", "1" },
                { "error", "2" },

                { "func_parameter_pointer", "3" },
                { "fpp", "3" },

                { "cond", "4" },
                { "condition", "4" },
                { "cnd", "4" },

                { "mainstack", "5" },
                { "p_ms", "5" },
                { "mem_mainstack", "0" },
                { "current_stack", "6" },

                { "t0", "7" },
                { "t1", "8" },
                { "t2", "9" },
                { "t3", "10" },
                { "t4", "11" },
                { "t5", "12" },
                { "t6", "13" },
                { "t7", "14" },
                { "erc", "15" },
                { "err_code", "15" },

                { "lt0", "7" },
                { "lt1", "9" },
                { "lt2", "11" },
                { "lt3", "13" },

                { "equal", "0" },
                { "eq", "0" },

                { "notequal", "1" },
                { "neq", "1" },
                { "less", "2" },
                { "lt", "2" },
                { "lessorequal", "3" },
                { "lesseq", "3" },
                { "lteq", "3" },
                { "greater", "4" },
                { "gt", "4" },
                { "greaterorequal", "5" },
                { "gteq", "5" },



                { "read", "3" },
                { "write", "4" },
                { "open", "5" },
                { "close", "6" },
                { "lseek", "19" },
                { "fsync", "118" },
                { "uname", "122" },
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
                new InstructionDefinition{ Name="jmp",  ID=(int)Inst.jmp, OP0DT=1, OP0REG=false, OP1DT=-2,OP1REG=false,OP2DT=-2, OP2REG=false},
                new InstructionDefinition{ Name="jmpr",  ID=(int)Inst.jmpr, OP0DT=1, OP0REG=true, OP1DT=-2,OP1REG=false,OP2DT=-2, OP2REG=false},
                new InstructionDefinition{ Name=$"{Inst.funccall}",  ID=(int)Inst.funccall, OP0DT=1, OP0REG=false, OP1DT=1,OP1REG=false,OP2DT=-2, OP2REG=false},
                new InstructionDefinition{ Name=$"{Inst.funccallr}",  ID=(int)Inst.funccallr, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=true,OP2DT=-2, OP2REG=false},
                new InstructionDefinition{ Name=$"{Inst.syscall}",  ID=(int)Inst.syscall, OP0DT=1, OP0REG=false, OP1DT=-2,OP1REG=false,OP2DT=-2, OP2REG=false},
                new InstructionDefinition{ Name=$"{Inst.syscallr}",  ID=(int)Inst.syscallr, OP0DT=1, OP0REG=true, OP1DT=-2,OP1REG=false,OP2DT=-2, OP2REG=false},
                new InstructionDefinition{ Name=$"{Inst.call}",  ID=(int)Inst.call, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=true,OP2DT=-2, OP2REG=false},
                new InstructionDefinition{ Name=$"{Inst.ifj}",  ID=(int)Inst.ifj, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=false,OP2DT=-2, OP2REG=false},
                new InstructionDefinition{ Name=$"{Inst.ifjr}",  ID=(int)Inst.ifjr, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=true,OP2DT=-2, OP2REG=false},
                new InstructionDefinition{ Name=$"{Inst.malloc}",  ID=(int)Inst.malloc, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=true,OP2DT=-2, OP2REG=true},
                new InstructionDefinition{ Name=$"{Inst.pushw}",  ID=(int)Inst.pushw, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=true,OP2DT=-2, OP2REG=true},
                new InstructionDefinition{ Name=$"{Inst.pushd}",  ID=(int)Inst.pushd, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=true,OP2DT=-2, OP2REG=true},
                new InstructionDefinition{ Name=$"{Inst.pushb}",  ID=(int)Inst.pushb, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=true,OP2DT=-2, OP2REG=true},
                new InstructionDefinition{ Name=$"{Inst.lpopw}",  ID=(int)Inst.lpopw, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=true,OP2DT=-2, OP2REG=true},
                new InstructionDefinition{ Name=$"{Inst.lpopd}",  ID=(int)Inst.lpopd, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=true,OP2DT=-2, OP2REG=true},
                new InstructionDefinition{ Name=$"{Inst.lpopb}",  ID=(int)Inst.lpopb, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=true,OP2DT=-2, OP2REG=true},
                new InstructionDefinition{ Name=$"{Inst.rpopw}",  ID=(int)Inst.rpopw, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=true,OP2DT=-2, OP2REG=true},
                new InstructionDefinition{ Name=$"{Inst.rpopd}",  ID=(int)Inst.rpopd, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=true,OP2DT=-2, OP2REG=true},
                new InstructionDefinition{ Name=$"{Inst.rpopb}",  ID=(int)Inst.rpopb, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=true,OP2DT=-2, OP2REG=true},
                new InstructionDefinition{ Name=$"{Inst.mlen}",  ID=(int)Inst.mlen, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=true,OP2DT=-2, OP2REG=true},
                new InstructionDefinition{ Name=$"{Inst.realloc}",  ID=(int)Inst.realloc, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=true,OP2DT=1, OP2REG=true},
                new InstructionDefinition{ Name=$"{Inst.reallocr}",  ID=(int)Inst.reallocr, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=true,OP2DT=1, OP2REG=true},
                new InstructionDefinition{ Name=$"{Inst.free}",  ID=(int)Inst.free, OP0DT=1, OP0REG=true, OP1DT=-2,OP1REG=true,OP2DT=-2, OP2REG=true},
                new InstructionDefinition{ Name=$"{Inst.cptxtr}",  ID=(int)Inst.cptxtr, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=true,OP2DT=-2, OP2REG=true},
                new InstructionDefinition{ Name=$"{Inst.cptxt}",  ID=(int)Inst.cptxt, OP0DT=1, OP0REG=false, OP1DT=1,OP1REG=true,OP2DT=-2, OP2REG=true},
                new InstructionDefinition{ Name=$"{Inst.swi}",  ID=(int)Inst.swi, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=true,OP2DT=1, OP2REG=false},
                new InstructionDefinition{ Name=$"{Inst.swr}",  ID=(int)Inst.swr, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=true,OP2DT=1, OP2REG=true},
                new InstructionDefinition{ Name=$"{Inst.lwi}",  ID=(int)Inst.lwi, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=true,OP2DT=1, OP2REG=false},
                new InstructionDefinition{ Name=$"{Inst.lwr}",  ID=(int)Inst.lwr, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=true,OP2DT=1, OP2REG=true},
                new InstructionDefinition{ Name=$"{Inst.sbi}",  ID=(int)Inst.sbi, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=true,OP2DT=1, OP2REG=false},
                new InstructionDefinition{ Name=$"{Inst.sbr}",  ID=(int)Inst.sbr, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=true,OP2DT=1, OP2REG=true},
                new InstructionDefinition{ Name=$"{Inst.lbi}",  ID=(int)Inst.lbi, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=true,OP2DT=1, OP2REG=false},
                new InstructionDefinition{ Name=$"{Inst.lbr}",  ID=(int)Inst.lbr, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=true,OP2DT=1, OP2REG=true},

                new InstructionDefinition{ Name=$"{Inst.pcs}", ID=(int)Inst.pcs, OP0DT=-2, OP0REG=false, OP1DT=-2,OP1REG=false,OP2DT=-2, OP2REG=false},
                new InstructionDefinition{ Name=$"{Inst.pcso}", ID=(int)Inst.pcso, OP0DT=1, OP0REG=false, OP1DT=-2,OP1REG=false,OP2DT=-2, OP2REG=false},
                new InstructionDefinition{ Name=$"{Inst.pcsor}", ID=(int)Inst.pcsor, OP0DT=1, OP0REG=true, OP1DT=-2,OP1REG=false,OP2DT=-2, OP2REG=false},

                new InstructionDefinition{ Name=$"{Inst.nop}",  ID=(int)Inst.nop, OP0DT=-2, OP0REG=true, OP1DT=-2,OP1REG=true,OP2DT=-2, OP2REG=true},
                new InstructionDefinition{ Name=$"{Inst.ret}",  ID=(int)Inst.ret, OP0DT=-2, OP0REG=true, OP1DT=-2,OP1REG=true,OP2DT=-2, OP2REG=true},

                new InstructionDefinition{ Name=$"{Inst.cmp}",  ID=(int)Inst.cmp, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=true,OP2DT=1, OP2REG=false},
                new InstructionDefinition{ Name=$"{Inst.bcmp}",  ID=(int)Inst.bcmp, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=true,OP2DT=1, OP2REG=false},
                new InstructionDefinition{ Name=$"{Inst.bcmpi}",  ID=(int)Inst.bcmpi, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=true,OP2DT=1, OP2REG=false},
                new InstructionDefinition{ Name=$"{Inst.scmp}",  ID=(int)Inst.scmp, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=true,OP2DT=1, OP2REG=false},
                new InstructionDefinition{ Name=$"{Inst.scmpi}",  ID=(int)Inst.scmpi, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=true,OP2DT=1, OP2REG=false},
                new InstructionDefinition{ Name=$"{Inst.uscmp}",  ID=(int)Inst.uscmp, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=true,OP2DT=1, OP2REG=false},
                new InstructionDefinition{ Name=$"{Inst.uscmpi}",  ID=(int)Inst.uscmpi, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=true,OP2DT=1, OP2REG=false},

                new InstructionDefinition{ Name=$"{Inst.ucmp}",  ID=(int)Inst.ucmp, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=true,OP2DT=1, OP2REG=false},
                new InstructionDefinition{ Name=$"{Inst.lcmp}",  ID=(int)Inst.lcmp, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=true,OP2DT=1, OP2REG=false},
                new InstructionDefinition{ Name=$"{Inst.ulcmp}",  ID=(int)Inst.ulcmp, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=true,OP2DT=1, OP2REG=false},
                new InstructionDefinition{ Name=$"{Inst.fcmp_s}",  ID=(int)Inst.fcmp_s, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=true,OP2DT=1, OP2REG=false},
                new InstructionDefinition{ Name=$"{Inst.fcmp_d}",  ID=(int)Inst.fcmp_d, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=true,OP2DT=1, OP2REG=false},

                new InstructionDefinition{ Name=$"{Inst.cmpi}",  ID=(int)Inst.cmpi, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=false,OP2DT=1, OP2REG=false},
                new InstructionDefinition{ Name=$"{Inst.ucmpi}",  ID=(int)Inst.ucmpi, OP0DT=1, OP0REG=true, OP1DT=5,OP1REG=false,OP2DT=1, OP2REG=false},
                new InstructionDefinition{ Name=$"{Inst.ulcmpi}",  ID=(int)Inst.ulcmpi, OP0DT=1, OP0REG=true, OP1DT=6,OP1REG=false,OP2DT=1, OP2REG=false},
                new InstructionDefinition{ Name=$"{Inst.lcmpi}",  ID=(int)Inst.lcmpi, OP0DT=1, OP0REG=true, OP1DT=2,OP1REG=false,OP2DT=1, OP2REG=false},
                new InstructionDefinition{ Name=$"{Inst.fcmpi_s}",  ID=(int)Inst.fcmpi_s, OP0DT=1, OP0REG=true, OP1DT=3,OP1REG=false,OP2DT=1, OP2REG=false},
                new InstructionDefinition{ Name=$"{Inst.fcmpi_d}",  ID=(int)Inst.fcmpi_d, OP0DT=1, OP0REG=true, OP1DT=4,OP1REG=false,OP2DT=1, OP2REG=false},

                new InstructionDefinition{ Name=$"{Inst.cvt}",  ID=(int)Inst.cvt, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=true,OP2DT=1, OP2REG=true},
                new InstructionDefinition{ Name=$"{Inst.pdw}",  ID=(int)Inst.pdw, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=true,OP2DT=1, OP2REG=true},
                new InstructionDefinition{ Name=$"{Inst.pdr}",  ID=(int)Inst.pdr, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=true,OP2DT=1, OP2REG=true},
                new InstructionDefinition{ Name=$"{Inst.pdint}",  ID=(int)Inst.pdint, OP0DT=-2, OP0REG=true, OP1DT=-2,OP1REG=true,OP2DT=-2, OP2REG=true},
                new InstructionDefinition{ Name=$"{Inst.pdintm}",  ID=(int)Inst.pdintm, OP0DT=1, OP0REG=true, OP1DT=-2,OP1REG=true,OP2DT=-2, OP2REG=true},

                new InstructionDefinition{ Name=$"{Inst.lmx}",  ID=(int)Inst.lmx, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=true,OP2DT=1, OP2REG=true},

                new InstructionDefinition{ Name=$"{Inst.add}",  ID=(int)Inst.add, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=true,OP2DT=1, OP2REG=true},
                new InstructionDefinition{ Name=$"{Inst.addi}", ID=(int)Inst.addi, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=false,OP2DT=1, OP2REG=true},
                new InstructionDefinition{ Name=$"{Inst.sub}",  ID=(int)Inst.sub, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=true,OP2DT=1, OP2REG=true},
                new InstructionDefinition{ Name=$"{Inst.subi}", ID=(int)Inst.subi, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=false,OP2DT=1, OP2REG=true},
                new InstructionDefinition{ Name=$"{Inst.mul}",  ID=(int)Inst.mul, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=true,OP2DT=1, OP2REG=true},
                new InstructionDefinition{ Name=$"{Inst.muli}", ID=(int)Inst.muli, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=false,OP2DT=1, OP2REG=true},
                new InstructionDefinition{ Name=$"{Inst.div}",  ID=(int)Inst.div, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=true,OP2DT=1, OP2REG=true},
                new InstructionDefinition{ Name=$"{Inst.divi}", ID=(int)Inst.divi, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=false,OP2DT=1, OP2REG=true},

                new InstructionDefinition{ Name="badd",  ID=(int)Inst.badd, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=true,OP2DT=1, OP2REG=true},
                new InstructionDefinition{ Name="baddi", ID=(int)Inst.baddi, OP0DT=1, OP0REG=true, OP1DT=Constants._byte,OP1REG=false,OP2DT=1, OP2REG=true},
                new InstructionDefinition{ Name="bsub",  ID=(int)Inst.bsub, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=true,OP2DT=1, OP2REG=true},
                new InstructionDefinition{ Name="bsubi", ID=(int)Inst.bsubi, OP0DT=1, OP0REG=true, OP1DT=Constants._byte,OP1REG=false,OP2DT=1, OP2REG=true},
                new InstructionDefinition{ Name="bmul",  ID=(int)Inst.bmul, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=true,OP2DT=1, OP2REG=true},
                new InstructionDefinition{ Name="bmuli", ID=(int)Inst.bmuli, OP0DT=1, OP0REG=true, OP1DT=Constants._byte,OP1REG=false,OP2DT=1, OP2REG=true},
                new InstructionDefinition{ Name="bdiv",  ID=(int)Inst.bdiv, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=true,OP2DT=1, OP2REG=true},
                new InstructionDefinition{ Name="bdivi", ID=(int)Inst.bdivi, OP0DT=1, OP0REG=true, OP1DT=Constants._byte,OP1REG=false,OP2DT=1, OP2REG=true},

                new InstructionDefinition{ Name="sadd",  ID=(int)Inst.sadd, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=true,OP2DT=1, OP2REG=true},
                new InstructionDefinition{ Name="saddi", ID=(int)Inst.saddi, OP0DT=1, OP0REG=true, OP1DT=Constants._short,OP1REG=false,OP2DT=1, OP2REG=true},
                new InstructionDefinition{ Name="ssub",  ID=(int)Inst.ssub, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=true,OP2DT=1, OP2REG=true},
                new InstructionDefinition{ Name="ssubi", ID=(int)Inst.ssubi, OP0DT=1, OP0REG=true, OP1DT=Constants._short,OP1REG=false,OP2DT=1, OP2REG=true},
                new InstructionDefinition{ Name="smul",  ID=(int)Inst.smul, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=true,OP2DT=1, OP2REG=true},
                new InstructionDefinition{ Name="smuli", ID=(int)Inst.smuli, OP0DT=1, OP0REG=true, OP1DT=Constants._short,OP1REG=false,OP2DT=1, OP2REG=true},
                new InstructionDefinition{ Name="sdiv",  ID=(int)Inst.sdiv, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=true,OP2DT=1, OP2REG=true},
                new InstructionDefinition{ Name="sdivi", ID=(int)Inst.sdivi, OP0DT=1, OP0REG=true, OP1DT=Constants._short,OP1REG=false,OP2DT=1, OP2REG=true},

                new InstructionDefinition{ Name="usadd",  ID=(int)Inst.usadd, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=true,OP2DT=1, OP2REG=true},
                new InstructionDefinition{ Name="usaddi", ID=(int)Inst.usaddi, OP0DT=1, OP0REG=true, OP1DT=Constants._ushort,OP1REG=false,OP2DT=1, OP2REG=true},
                new InstructionDefinition{ Name="ussub",  ID=(int)Inst.ussub, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=true,OP2DT=1, OP2REG=true},
                new InstructionDefinition{ Name="ussubi", ID=(int)Inst.ussubi, OP0DT=1, OP0REG=true, OP1DT=Constants._ushort,OP1REG=false,OP2DT=1, OP2REG=true},
                new InstructionDefinition{ Name="usmul",  ID=(int)Inst.usmul, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=true,OP2DT=1, OP2REG=true},
                new InstructionDefinition{ Name="usmuli", ID=(int)Inst.usmuli, OP0DT=1, OP0REG=true, OP1DT=Constants._ushort,OP1REG=false,OP2DT=1, OP2REG=true},
                new InstructionDefinition{ Name="usdiv",  ID=(int)Inst.usdiv, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=true,OP2DT=1, OP2REG=true},
                new InstructionDefinition{ Name="usdivi", ID=(int)Inst.usdivi, OP0DT=1, OP0REG=true, OP1DT=Constants._ushort,OP1REG=false,OP2DT=1, OP2REG=true},

                new InstructionDefinition{ Name="uadd",  ID=(int)Inst.uadd, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=true,OP2DT=1, OP2REG=true},
                new InstructionDefinition{ Name="uaddi", ID=(int)Inst.uaddi, OP0DT=1, OP0REG=true, OP1DT=5,OP1REG=false,OP2DT=1, OP2REG=true},
                new InstructionDefinition{ Name="usub",  ID=(int)Inst.usub, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=true,OP2DT=1, OP2REG=true},
                new InstructionDefinition{ Name="usubi", ID=(int)Inst.usubi, OP0DT=1, OP0REG=true, OP1DT=5,OP1REG=false,OP2DT=1, OP2REG=true},
                new InstructionDefinition{ Name="umul",  ID=(int)Inst.umul, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=true,OP2DT=1, OP2REG=true},
                new InstructionDefinition{ Name="umuli", ID=(int)Inst.umuli, OP0DT=1, OP0REG=true, OP1DT=5,OP1REG=false,OP2DT=1, OP2REG=true},
                new InstructionDefinition{ Name="udiv",  ID=(int)Inst.udiv, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=true,OP2DT=1, OP2REG=true},
                new InstructionDefinition{ Name="udivi", ID=(int)Inst.udivi, OP0DT=1, OP0REG=true, OP1DT=5,OP1REG=false,OP2DT=1, OP2REG=true},

                new InstructionDefinition{ Name="uladd",  ID=(int)Inst.uladd, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=true,OP2DT=1, OP2REG=true},
                new InstructionDefinition{ Name="uladdi", ID=(int)Inst.uladdi, OP0DT=1, OP0REG=true, OP1DT=6,OP1REG=false,OP2DT=1, OP2REG=true},
                new InstructionDefinition{ Name="ulsub",  ID=(int)Inst.ulsub, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=true,OP2DT=1, OP2REG=true},
                new InstructionDefinition{ Name="ulsubi", ID=(int)Inst.ulsubi, OP0DT=1, OP0REG=true, OP1DT=6,OP1REG=false,OP2DT=1, OP2REG=true},
                new InstructionDefinition{ Name="ulmul",  ID=(int)Inst.ulmul, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=true,OP2DT=1, OP2REG=true},
                new InstructionDefinition{ Name="ulmuli", ID=(int)Inst.ulmuli, OP0DT=1, OP0REG=true, OP1DT=6,OP1REG=false,OP2DT=1, OP2REG=true},
                new InstructionDefinition{ Name="uldiv",  ID=(int)Inst.uldiv, OP0DT=1, OP0REG=true, OP1DT=1,OP1REG=true,OP2DT=1, OP2REG=true},
                new InstructionDefinition{ Name="uldivi", ID=(int)Inst.uldivi, OP0DT=1, OP0REG=true, OP1DT=6,OP1REG=false,OP2DT=1, OP2REG=true},

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