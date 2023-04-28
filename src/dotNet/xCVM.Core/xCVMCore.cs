using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xCVM.Core
{
    public class xCVMOption
    {
        public int RegisterSize;
        public int RegisterCount;
    }
    public class xCVMCore
    {
        int RegisterSize = Constants.int_size;
        xCVMRTProgram? program = null;
        xCVMem Registers;
        xCVMemBlock MemoryBlocks;
        ManagedMem ManagedMem;
        xCVMOption xCVMOption;
        public xCVMCore(xCVMOption xCVMOption)
        {
            this.xCVMOption = xCVMOption;
            RegisterSize = xCVMOption.RegisterSize;
            Registers = new xCVMem() { data = new byte [ xCVMOption.RegisterCount * xCVMOption.RegisterSize ] };
            MemoryBlocks = new xCVMemBlock();
            ManagedMem = new ManagedMem();
        }
        public void Execute(Instruct instruct)
        {
            switch (instruct.Operation)
            {
                case (int)Inst.add:
                    {
                        int op0 = BitConverter.ToInt32(instruct.Op0) * RegisterSize;
                        int op1 = BitConverter.ToInt32(instruct.Op1) * RegisterSize;
                        int op2 = BitConverter.ToInt32(instruct.Op2) * RegisterSize;
                        int OP0 = BitConverter.ToInt32(Registers.data [ op0..(op0 + Constants.int_size) ]);
                        int OP1 = BitConverter.ToInt32(Registers.data [ op1..(op1 + Constants.int_size) ]);
                        BitConverter.GetBytes(OP0 + OP1).CopyTo(Registers.data , op2);
                    }
                    break;
                case (int)Inst.sub:
                    {
                        int op0 = BitConverter.ToInt32(instruct.Op0) * RegisterSize;
                        int op1 = BitConverter.ToInt32(instruct.Op1) * RegisterSize;
                        int op2 = BitConverter.ToInt32(instruct.Op2) * RegisterSize;
                        int OP0 = BitConverter.ToInt32(Registers.data [ op0..(op0 + Constants.int_size) ]);
                        int OP1 = BitConverter.ToInt32(Registers.data [ op1..(op1 + Constants.int_size) ]);
                        BitConverter.GetBytes(OP0 - OP1).CopyTo(Registers.data , op2);
                    }
                    break;
                case (int)Inst.mul:
                    {
                        int op0 = BitConverter.ToInt32(instruct.Op0) * RegisterSize;
                        int op1 = BitConverter.ToInt32(instruct.Op1) * RegisterSize;
                        int op2 = BitConverter.ToInt32(instruct.Op2) * RegisterSize;
                        int OP0 = BitConverter.ToInt32(Registers.data [ op0..(op0 + Constants.int_size) ]);
                        int OP1 = BitConverter.ToInt32(Registers.data [ op1..(op1 + Constants.int_size) ]);
                        BitConverter.GetBytes(OP0 * OP1).CopyTo(Registers.data , op2);
                    }
                    break;
                case (int)Inst.div:
                    {
                        int op0 = BitConverter.ToInt32(instruct.Op0) * RegisterSize;
                        int op1 = BitConverter.ToInt32(instruct.Op1) * RegisterSize;
                        int op2 = BitConverter.ToInt32(instruct.Op2) * RegisterSize;
                        int OP0 = BitConverter.ToInt32(Registers.data [ op0..(op0 + Constants.int_size) ]);
                        int OP1 = BitConverter.ToInt32(Registers.data [ op1..(op1 + Constants.int_size) ]);
                        BitConverter.GetBytes(OP0 / OP1).CopyTo(Registers.data , op2);
                    }
                    break;

                case (int)Inst.addi:
                    {
                        int op0 = BitConverter.ToInt32(instruct.Op0) * RegisterSize;
                        int op1 = BitConverter.ToInt32(instruct.Op1);
                        int op2 = BitConverter.ToInt32(instruct.Op2) * RegisterSize;
                        int OP0 = BitConverter.ToInt32(Registers.data [ op0..(op0 + Constants.int_size) ]);
                        BitConverter.GetBytes(OP0 + op1).CopyTo(Registers.data , op2);
                    }
                    break;
                case (int)Inst.subi:
                    {
                        int op0 = BitConverter.ToInt32(instruct.Op0) * RegisterSize;
                        int op1 = BitConverter.ToInt32(instruct.Op1);
                        int op2 = BitConverter.ToInt32(instruct.Op2) * RegisterSize;
                        int OP0 = BitConverter.ToInt32(Registers.data [ op0..(op0 + Constants.int_size) ]);
                        BitConverter.GetBytes(OP0 - op1).CopyTo(Registers.data , op2);
                    }
                    break;
                case (int)Inst.muli:
                    {
                        int op0 = BitConverter.ToInt32(instruct.Op0) * RegisterSize;
                        int op1 = BitConverter.ToInt32(instruct.Op1);
                        int op2 = BitConverter.ToInt32(instruct.Op2) * RegisterSize;
                        int OP0 = BitConverter.ToInt32(Registers.data [ op0..(op0 + Constants.int_size) ]);
                        BitConverter.GetBytes(OP0 * op1).CopyTo(Registers.data , op2);
                    }
                    break;
                case (int)Inst.divi:
                    {
                        int op0 = BitConverter.ToInt32(instruct.Op0) * RegisterSize;
                        int op1 = BitConverter.ToInt32(instruct.Op1);
                        int op2 = BitConverter.ToInt32(instruct.Op2) * RegisterSize;
                        int OP0 = BitConverter.ToInt32(Registers.data [ op0..(op0 + Constants.int_size) ]);
                        BitConverter.GetBytes(OP0 / op1).CopyTo(Registers.data , op2);
                    }
                    break;
                case (int)Inst.uadd:
                    {
                        int op0 = BitConverter.ToInt32(instruct.Op0) * RegisterSize;
                        int op1 = BitConverter.ToInt32(instruct.Op1) * RegisterSize;
                        int op2 = BitConverter.ToInt32(instruct.Op2) * RegisterSize;
                        uint OP0 = BitConverter.ToUInt32(Registers.data [ op0..(op0 + Constants.int_size) ]);
                        uint OP1 = BitConverter.ToUInt32(Registers.data [ op1..(op1 + Constants.int_size) ]);
                        BitConverter.GetBytes(OP0 + OP1).CopyTo(Registers.data , op2);
                    }
                    break;
                case (int)Inst.usub:
                    {
                        int op0 = BitConverter.ToInt32(instruct.Op0) * RegisterSize;
                        int op1 = BitConverter.ToInt32(instruct.Op1) * RegisterSize;
                        int op2 = BitConverter.ToInt32(instruct.Op2) * RegisterSize;
                        uint OP0 = BitConverter.ToUInt32(Registers.data [ op0..(op0 + Constants.int_size) ]);
                        uint OP1 = BitConverter.ToUInt32(Registers.data [ op1..(op1 + Constants.int_size) ]);
                        BitConverter.GetBytes(OP0 - OP1).CopyTo(Registers.data , op2);
                    }
                    break;
                case (int)Inst.umul:
                    {
                        int op0 = BitConverter.ToInt32(instruct.Op0) * RegisterSize;
                        int op1 = BitConverter.ToInt32(instruct.Op1) * RegisterSize;
                        int op2 = BitConverter.ToInt32(instruct.Op2) * RegisterSize;
                        uint OP0 = BitConverter.ToUInt32(Registers.data [ op0..(op0 + Constants.int_size) ]);
                        uint OP1 = BitConverter.ToUInt32(Registers.data [ op1..(op1 + Constants.int_size) ]);
                        BitConverter.GetBytes(OP0 * OP1).CopyTo(Registers.data , op2);
                    }
                    break;
                case (int)Inst.udiv:
                    {
                        int op0 = BitConverter.ToInt32(instruct.Op0) * RegisterSize;
                        int op1 = BitConverter.ToInt32(instruct.Op1) * RegisterSize;
                        int op2 = BitConverter.ToInt32(instruct.Op2) * RegisterSize;
                        uint OP0 = BitConverter.ToUInt32(Registers.data [ op0..(op0 + Constants.int_size) ]);
                        uint OP1 = BitConverter.ToUInt32(Registers.data [ op1..(op1 + Constants.int_size) ]);
                        BitConverter.GetBytes(OP0 / OP1).CopyTo(Registers.data , op2);
                    }
                    break;

                case (int)Inst.uaddi:
                    {
                        int op0 = BitConverter.ToInt32(instruct.Op0) * RegisterSize;
                        uint op1 = BitConverter.ToUInt32(instruct.Op1);
                        int op2 = BitConverter.ToInt32(instruct.Op2) * RegisterSize;
                        uint OP0 = BitConverter.ToUInt32(Registers.data [ op0..(op0 + Constants.int_size) ]);
                        BitConverter.GetBytes(OP0 + op1).CopyTo(Registers.data , op2);
                    }
                    break;
                case (int)Inst.usubi:
                    {
                        int op0 = BitConverter.ToInt32(instruct.Op0) * RegisterSize;
                        uint op1 = BitConverter.ToUInt32(instruct.Op1);
                        int op2 = BitConverter.ToInt32(instruct.Op2) * RegisterSize;
                        uint OP0 = BitConverter.ToUInt32(Registers.data [ op0..(op0 + Constants.int_size) ]);
                        BitConverter.GetBytes(OP0 - op1).CopyTo(Registers.data , op2);
                    }
                    break;
                case (int)Inst.umuli:
                    {
                        int op0 = BitConverter.ToInt32(instruct.Op0) * RegisterSize;
                        uint op1 = BitConverter.ToUInt32(instruct.Op1);
                        int op2 = BitConverter.ToInt32(instruct.Op2) * RegisterSize;
                        uint OP0 = BitConverter.ToUInt32(Registers.data [ op0..(op0 + Constants.int_size) ]);
                        BitConverter.GetBytes(OP0 * op1).CopyTo(Registers.data , op2);
                    }
                    break;
                case (int)Inst.udivi:
                    {
                        int op0 = BitConverter.ToInt32(instruct.Op0) * RegisterSize;
                        uint op1 = BitConverter.ToUInt32(instruct.Op1);
                        int op2 = BitConverter.ToInt32(instruct.Op2) * RegisterSize;
                        uint OP0 = BitConverter.ToUInt32(Registers.data [ op0..(op0 + Constants.int_size) ]);
                        BitConverter.GetBytes(OP0 / op1).CopyTo(Registers.data , op2);
                    }
                    break;
                case (int)Inst.ladd:
                    {
                        int op0 = BitConverter.ToInt32(instruct.Op0) * RegisterSize;
                        int op1 = BitConverter.ToInt32(instruct.Op1) * RegisterSize;
                        int op2 = BitConverter.ToInt32(instruct.Op2) * RegisterSize;
                        long OP0 = BitConverter.ToInt64(Registers.data [ op0..(op0 + Constants.long_size) ]);
                        long OP1 = BitConverter.ToInt64(Registers.data [ op1..(op1 + Constants.long_size) ]);
                        BitConverter.GetBytes(OP0 + OP1).CopyTo(Registers.data , op2);
                    }
                    break;
                case (int)Inst.lsub:
                    {
                        int op0 = BitConverter.ToInt32(instruct.Op0) * RegisterSize;
                        int op1 = BitConverter.ToInt32(instruct.Op1) * RegisterSize;
                        int op2 = BitConverter.ToInt32(instruct.Op2) * RegisterSize;
                        long OP0 = BitConverter.ToInt64(Registers.data [ op0..(op0 + Constants.long_size) ]);
                        long OP1 = BitConverter.ToInt64(Registers.data [ op1..(op1 + Constants.long_size) ]);
                        BitConverter.GetBytes(OP0 - OP1).CopyTo(Registers.data , op2);
                    }
                    break;
                case (int)Inst.lmul:
                    {
                        int op0 = BitConverter.ToInt32(instruct.Op0) * RegisterSize;
                        int op1 = BitConverter.ToInt32(instruct.Op1) * RegisterSize;
                        int op2 = BitConverter.ToInt32(instruct.Op2) * RegisterSize;
                        long OP0 = BitConverter.ToInt64(Registers.data [ op0..(op0 + Constants.long_size) ]);
                        long OP1 = BitConverter.ToInt64(Registers.data [ op1..(op1 + Constants.long_size) ]);
                        BitConverter.GetBytes(OP0 * OP1).CopyTo(Registers.data , op2);
                    }
                    break;
                case (int)Inst.ldiv:
                    {
                        int op0 = BitConverter.ToInt32(instruct.Op0) * RegisterSize;
                        int op1 = BitConverter.ToInt32(instruct.Op1) * RegisterSize;
                        int op2 = BitConverter.ToInt32(instruct.Op2) * RegisterSize;
                        long OP0 = BitConverter.ToInt64(Registers.data [ op0..(op0 + Constants.long_size) ]);
                        long OP1 = BitConverter.ToInt64(Registers.data [ op1..(op1 + Constants.long_size) ]);
                        BitConverter.GetBytes(OP0 / OP1).CopyTo(Registers.data , op2);
                    }
                    break;


                case (int)Inst.laddi:
                    {
                        int op0 = BitConverter.ToInt32(instruct.Op0) * RegisterSize;
                        long op1 = BitConverter.ToInt64(instruct.Op1);
                        int op2 = BitConverter.ToInt32(instruct.Op2) * RegisterSize;
                        long OP0 = BitConverter.ToInt64(Registers.data [ op0..(op0 + Constants.long_size) ]);
                        BitConverter.GetBytes(OP0 + op1).CopyTo(Registers.data , op2);
                    }
                    break;
                case (int)Inst.lsubi:
                    {
                        int op0 = BitConverter.ToInt32(instruct.Op0) * RegisterSize;
                        long op1 = BitConverter.ToInt64(instruct.Op1);
                        int op2 = BitConverter.ToInt32(instruct.Op2) * RegisterSize;
                        long OP0 = BitConverter.ToInt64(Registers.data [ op0..(op0 + Constants.long_size) ]);
                        BitConverter.GetBytes(OP0 - op1).CopyTo(Registers.data , op2);
                    }
                    break;
                case (int)Inst.lmuli:
                    {
                        int op0 = BitConverter.ToInt32(instruct.Op0) * RegisterSize;
                        long op1 = BitConverter.ToInt64(instruct.Op1);
                        int op2 = BitConverter.ToInt32(instruct.Op2) * RegisterSize;
                        long OP0 = BitConverter.ToInt64(Registers.data [ op0..(op0 + Constants.long_size) ]);
                        BitConverter.GetBytes(OP0 * op1).CopyTo(Registers.data , op2);
                    }
                    break;
                case (int)Inst.ldivi:
                    {
                        int op0 = BitConverter.ToInt32(instruct.Op0) * RegisterSize;
                        long op1 = BitConverter.ToInt64(instruct.Op1);
                        int op2 = BitConverter.ToInt32(instruct.Op2) * RegisterSize;
                        long OP0 = BitConverter.ToInt64(Registers.data [ op0..(op0 + Constants.long_size) ]);
                        BitConverter.GetBytes(OP0 / op1).CopyTo(Registers.data , op2);
                    }
                    break;

                case (int)Inst.uladd:
                    {
                        int op0 = BitConverter.ToInt32(instruct.Op0) * RegisterSize;
                        int op1 = BitConverter.ToInt32(instruct.Op1) * RegisterSize;
                        int op2 = BitConverter.ToInt32(instruct.Op2) * RegisterSize;
                        ulong OP0 = BitConverter.ToUInt64(Registers.data [ op0..(op0 + Constants.long_size) ]);
                        ulong OP1 = BitConverter.ToUInt64(Registers.data [ op1..(op1 + Constants.long_size) ]);
                        BitConverter.GetBytes(OP0 + OP1).CopyTo(Registers.data , op2);
                    }
                    break;
                case (int)Inst.ulsub:
                    {
                        int op0 = BitConverter.ToInt32(instruct.Op0) * RegisterSize;
                        int op1 = BitConverter.ToInt32(instruct.Op1) * RegisterSize;
                        int op2 = BitConverter.ToInt32(instruct.Op2) * RegisterSize;
                        ulong OP0 = BitConverter.ToUInt64(Registers.data [ op0..(op0 + Constants.long_size) ]);
                        ulong OP1 = BitConverter.ToUInt64(Registers.data [ op1..(op1 + Constants.long_size) ]);
                        BitConverter.GetBytes(OP0 - OP1).CopyTo(Registers.data , op2);
                    }
                    break;
                case (int)Inst.ulmul:
                    {
                        int op0 = BitConverter.ToInt32(instruct.Op0) * RegisterSize;
                        int op1 = BitConverter.ToInt32(instruct.Op1) * RegisterSize;
                        int op2 = BitConverter.ToInt32(instruct.Op2) * RegisterSize;
                        ulong OP0 = BitConverter.ToUInt64(Registers.data [ op0..(op0 + Constants.long_size) ]);
                        ulong OP1 = BitConverter.ToUInt64(Registers.data [ op1..(op1 + Constants.long_size) ]);
                        BitConverter.GetBytes(OP0 * OP1).CopyTo(Registers.data , op2);
                    }
                    break;
                case (int)Inst.uldiv:
                    {
                        int op0 = BitConverter.ToInt32(instruct.Op0) * RegisterSize;
                        int op1 = BitConverter.ToInt32(instruct.Op1) * RegisterSize;
                        int op2 = BitConverter.ToInt32(instruct.Op2) * RegisterSize;
                        ulong OP0 = BitConverter.ToUInt64(Registers.data [ op0..(op0 + Constants.long_size) ]);
                        ulong OP1 = BitConverter.ToUInt64(Registers.data [ op1..(op1 + Constants.long_size) ]);
                        BitConverter.GetBytes(OP0 / OP1).CopyTo(Registers.data , op2);
                    }
                    break;


                case (int)Inst.uladdi:
                    {
                        int op0 = BitConverter.ToInt32(instruct.Op0) * RegisterSize;
                        ulong op1 = BitConverter.ToUInt64(instruct.Op1);
                        int op2 = BitConverter.ToInt32(instruct.Op2) * RegisterSize;
                        ulong OP0 = BitConverter.ToUInt64(Registers.data [ op0..(op0 + Constants.long_size) ]);
                        BitConverter.GetBytes(OP0 + op1).CopyTo(Registers.data , op2);
                    }
                    break;
                case (int)Inst.ulsubi:
                    {
                        int op0 = BitConverter.ToInt32(instruct.Op0) * RegisterSize;
                        ulong op1 = BitConverter.ToUInt64(instruct.Op1);
                        int op2 = BitConverter.ToInt32(instruct.Op2) * RegisterSize;
                        ulong OP0 = BitConverter.ToUInt64(Registers.data [ op0..(op0 + Constants.long_size) ]);
                        BitConverter.GetBytes(OP0 - op1).CopyTo(Registers.data , op2);
                    }
                    break;
                case (int)Inst.ulmuli:
                    {
                        int op0 = BitConverter.ToInt32(instruct.Op0) * RegisterSize;
                        ulong op1 = BitConverter.ToUInt64(instruct.Op1);
                        int op2 = BitConverter.ToInt32(instruct.Op2) * RegisterSize;
                        ulong OP0 = BitConverter.ToUInt64(Registers.data [ op0..(op0 + Constants.long_size) ]);
                        BitConverter.GetBytes(OP0 * op1).CopyTo(Registers.data , op2);
                    }
                    break;
                case (int)Inst.uldivi:
                    {
                        int op0 = BitConverter.ToInt32(instruct.Op0) * RegisterSize;
                        ulong op1 = BitConverter.ToUInt64(instruct.Op1);
                        int op2 = BitConverter.ToInt32(instruct.Op2) * RegisterSize;
                        ulong OP0 = BitConverter.ToUInt64(Registers.data [ op0..(op0 + Constants.long_size) ]);
                        BitConverter.GetBytes(OP0 / op1).CopyTo(Registers.data , op2);
                    }
                    break;

                case (int)Inst.fadd_s:
                    {
                        int op0 = BitConverter.ToInt32(instruct.Op0) * RegisterSize;
                        int op1 = BitConverter.ToInt32(instruct.Op1) * RegisterSize;
                        int op2 = BitConverter.ToInt32(instruct.Op2) * RegisterSize;
                        var OP0 = BitConverter.ToSingle(Registers.data [ op0..(op0 + Constants.float_size) ]);
                        var OP1 = BitConverter.ToSingle(Registers.data [ op1..(op1 + Constants.float_size) ]);
                        BitConverter.GetBytes(OP0 + OP1).CopyTo(Registers.data , op2);
                    }
                    break;
                case (int)Inst.fsub_s:
                    {
                        int op0 = BitConverter.ToInt32(instruct.Op0) * RegisterSize;
                        int op1 = BitConverter.ToInt32(instruct.Op1) * RegisterSize;
                        int op2 = BitConverter.ToInt32(instruct.Op2) * RegisterSize;
                        float OP0 = BitConverter.ToSingle(Registers.data [ op0..(op0 + Constants.float_size) ]);
                        float OP1 = BitConverter.ToSingle(Registers.data [ op1..(op1 + Constants.float_size) ]);
                        BitConverter.GetBytes(OP0 - OP1).CopyTo(Registers.data , op2);
                    }
                    break;
                case (int)Inst.fmul_s:
                    {
                        int op0 = BitConverter.ToInt32(instruct.Op0) * RegisterSize;
                        int op1 = BitConverter.ToInt32(instruct.Op1) * RegisterSize;
                        int op2 = BitConverter.ToInt32(instruct.Op2) * RegisterSize;
                        float OP0 = BitConverter.ToSingle(Registers.data [ op0..(op0 + Constants.float_size) ]);
                        float OP1 = BitConverter.ToSingle(Registers.data [ op1..(op1 + Constants.float_size) ]);
                        BitConverter.GetBytes(OP0 * OP1).CopyTo(Registers.data , op2);
                    }
                    break;
                case (int)Inst.fdiv_s:
                    {
                        int op0 = BitConverter.ToInt32(instruct.Op0) * RegisterSize;
                        int op1 = BitConverter.ToInt32(instruct.Op1) * RegisterSize;
                        int op2 = BitConverter.ToInt32(instruct.Op2) * RegisterSize;
                        float OP0 = BitConverter.ToSingle(Registers.data [ op0..(op0 + Constants.float_size) ]);
                        float OP1 = BitConverter.ToSingle(Registers.data [ op1..(op1 + Constants.float_size) ]);
                        BitConverter.GetBytes(OP0 / OP1).CopyTo(Registers.data , op2);
                    }
                    break;

                case (int)Inst.faddi_s:
                    {
                        int op0 = BitConverter.ToInt32(instruct.Op0) * RegisterSize;
                        float OP1 = BitConverter.ToSingle(instruct.Op1);
                        int op2 = BitConverter.ToInt32(instruct.Op2) * RegisterSize;
                        float OP0 = BitConverter.ToSingle(Registers.data [ op0..(op0 + Constants.float_size) ]);
                        BitConverter.GetBytes(OP0 + OP1).CopyTo(Registers.data , op2);
                    }
                    break;
                case (int)Inst.fsubi_s:
                    {
                        int op0 = BitConverter.ToInt32(instruct.Op0) * RegisterSize;
                        float OP1 = BitConverter.ToSingle(instruct.Op1);
                        int op2 = BitConverter.ToInt32(instruct.Op2) * RegisterSize;
                        float OP0 = BitConverter.ToSingle(Registers.data [ op0..(op0 + Constants.float_size) ]);
                        BitConverter.GetBytes(OP0 - OP1).CopyTo(Registers.data , op2);
                    }
                    break;
                case (int)Inst.fmuli_s:
                    {
                        int op0 = BitConverter.ToInt32(instruct.Op0) * RegisterSize;
                        float OP1 = BitConverter.ToSingle(instruct.Op1);
                        int op2 = BitConverter.ToInt32(instruct.Op2) * RegisterSize;
                        float OP0 = BitConverter.ToSingle(Registers.data [ op0..(op0 + Constants.float_size) ]);
                        BitConverter.GetBytes(OP0 * OP1).CopyTo(Registers.data , op2);
                    }
                    break;
                case (int)Inst.fdivi_s:
                    {
                        int op0 = BitConverter.ToInt32(instruct.Op0) * RegisterSize;
                        float OP1 = BitConverter.ToSingle(instruct.Op1);
                        int op2 = BitConverter.ToInt32(instruct.Op2) * RegisterSize;
                        float OP0 = BitConverter.ToSingle(Registers.data [ op0..(op0 + Constants.float_size) ]);
                        BitConverter.GetBytes(OP0 / OP1).CopyTo(Registers.data , op2);
                    }
                    break;
                case (int)Inst.fadd_d:
                    {
                        int op0 = BitConverter.ToInt32(instruct.Op0) * RegisterSize;
                        int op1 = BitConverter.ToInt32(instruct.Op1) * RegisterSize;
                        int op2 = BitConverter.ToInt32(instruct.Op2) * RegisterSize;
                        var OP0 = BitConverter.ToDouble(Registers.data [ op0..(op0 + Constants.double_size) ]);
                        var OP1 = BitConverter.ToDouble(Registers.data [ op1..(op1 + Constants.double_size) ]);
                        BitConverter.GetBytes(OP0 + OP1).CopyTo(Registers.data , op2);
                    }
                    break;
                case (int)Inst.fsub_d:
                    {
                        int op0 = BitConverter.ToInt32(instruct.Op0) * RegisterSize;
                        int op1 = BitConverter.ToInt32(instruct.Op1) * RegisterSize;
                        int op2 = BitConverter.ToInt32(instruct.Op2) * RegisterSize;
                        double OP0 = BitConverter.ToDouble(Registers.data [ op0..(op0 + Constants.double_size) ]);
                        double OP1 = BitConverter.ToDouble(Registers.data [ op1..(op1 + Constants.double_size) ]);
                        BitConverter.GetBytes(OP0 - OP1).CopyTo(Registers.data , op2);
                    }
                    break;
                case (int)Inst.fmul_d:
                    {
                        int op0 = BitConverter.ToInt32(instruct.Op0) * RegisterSize;
                        int op1 = BitConverter.ToInt32(instruct.Op1) * RegisterSize;
                        int op2 = BitConverter.ToInt32(instruct.Op2) * RegisterSize;
                        double OP0 = BitConverter.ToDouble(Registers.data [ op0..(op0 + Constants.double_size) ]);
                        double OP1 = BitConverter.ToDouble(Registers.data [ op1..(op1 + Constants.double_size) ]);
                        BitConverter.GetBytes(OP0 * OP1).CopyTo(Registers.data , op2);
                    }
                    break;
                case (int)Inst.fdiv_d:
                    {
                        int op0 = BitConverter.ToInt32(instruct.Op0) * RegisterSize;
                        int op1 = BitConverter.ToInt32(instruct.Op1) * RegisterSize;
                        int op2 = BitConverter.ToInt32(instruct.Op2) * RegisterSize;
                        double OP0 = BitConverter.ToDouble(Registers.data [ op0..(op0 + Constants.double_size) ]);
                        double OP1 = BitConverter.ToDouble(Registers.data [ op1..(op1 + Constants.double_size) ]);
                        BitConverter.GetBytes(OP0 / OP1).CopyTo(Registers.data , op2);
                    }
                    break;
                case (int)Inst.faddi_d:
                    {
                        int op0 = BitConverter.ToInt32(instruct.Op0) * RegisterSize;
                        double OP1 = BitConverter.ToInt32(instruct.Op1);
                        int op2 = BitConverter.ToInt32(instruct.Op2) * RegisterSize;
                        double OP0 = BitConverter.ToDouble(Registers.data [ op0..(op0 + Constants.double_size) ]);
                        BitConverter.GetBytes(OP0 + OP1).CopyTo(Registers.data , op2);
                    }
                    break;
                case (int)Inst.fsubi_d:
                    {
                        int op0 = BitConverter.ToInt32(instruct.Op0) * RegisterSize;
                        double OP1 = BitConverter.ToInt32(instruct.Op1);
                        int op2 = BitConverter.ToInt32(instruct.Op2) * RegisterSize;
                        double OP0 = BitConverter.ToDouble(Registers.data [ op0..(op0 + Constants.double_size) ]);
                        BitConverter.GetBytes(OP0 - OP1).CopyTo(Registers.data , op2);
                    }
                    break;
                case (int)Inst.fmuli_d:
                    {
                        int op0 = BitConverter.ToInt32(instruct.Op0) * RegisterSize;
                        double OP1 = BitConverter.ToInt32(instruct.Op1);
                        int op2 = BitConverter.ToInt32(instruct.Op2) * RegisterSize;
                        double OP0 = BitConverter.ToDouble(Registers.data [ op0..(op0 + Constants.double_size) ]);
                        BitConverter.GetBytes(OP0 * OP1).CopyTo(Registers.data , op2);
                    }
                    break;
                case (int)Inst.fdivi_d:
                    {
                        int op0 = BitConverter.ToInt32(instruct.Op0) * RegisterSize;
                        int op1 = BitConverter.ToInt32(instruct.Op1) * RegisterSize;
                        int op2 = BitConverter.ToInt32(instruct.Op2) * RegisterSize;
                        double OP0 = BitConverter.ToDouble(Registers.data [ op0..(op0 + Constants.double_size) ]);
                        double OP1 = BitConverter.ToDouble(Registers.data [ op1..(op1 + Constants.double_size) ]);
                        BitConverter.GetBytes(OP0 / OP1).CopyTo(Registers.data , op2);
                    }
                    break;
                case (int)Inst.malloc:
                    {
                        int op0 = BitConverter.ToInt32(instruct.Op0) * RegisterSize;
                        int op1 = BitConverter.ToInt32(instruct.Op1) * RegisterSize;
                        int OP0 = BitConverter.ToInt32(Registers.data [ op0..(op0 + Constants.int_size) ]);
                        BitConverter.GetBytes(MemoryBlocks.MALLOC(OP0)).CopyTo(Registers.data , op1);
                    }
                    break;
                case (int)Inst.realloc:
                    {
                        int op0 = BitConverter.ToInt32(instruct.Op0) * RegisterSize;
                        int op1 = BitConverter.ToInt32(instruct.Op1) * RegisterSize;
                        int op2 = BitConverter.ToInt32(instruct.Op2) * RegisterSize;
                        int OP0 = BitConverter.ToInt32(Registers.data [ op0..(op0 + Constants.int_size) ]);
                        int OP1 = BitConverter.ToInt32(Registers.data [ op1..(op1 + Constants.int_size) ]);
                        BitConverter.GetBytes(MemoryBlocks.REALLOC(OP0 , OP1)).CopyTo(Registers.data , op2);
                    }
                    break;
                case (int)Inst.free:
                    {
                        int op0 = BitConverter.ToInt32(instruct.Op0) * RegisterSize;
                        int OP0 = BitConverter.ToInt32(Registers.data [ op0..(op0 + Constants.int_size) ]);
                        MemoryBlocks.FREE(OP0);
                    }
                    break;
                case (int)Inst.lwr:
                    {
                        int op0 = BitConverter.ToInt32(instruct.Op0) * RegisterSize;
                        int op1 = BitConverter.ToInt32(instruct.Op1) * RegisterSize;
                        int op2 = BitConverter.ToInt32(instruct.Op2) * RegisterSize;
                        int OP1 = BitConverter.ToInt32(Registers.data [ op1..(op1 + Constants.int_size) ]);
                        int OP2 = BitConverter.ToInt32(Registers.data [ op2..(op2 + Constants.int_size) ]);
                        MemoryBlocks.Datas [ OP1 ].data [ (OP2 * RegisterSize)..(OP2 * RegisterSize + RegisterSize) ].CopyTo(Registers.data , op0);
                    }
                    break;
                case (int)Inst.lwi:
                    {
                        int op0 = BitConverter.ToInt32(instruct.Op0) * RegisterSize;
                        int op1 = BitConverter.ToInt32(instruct.Op1) * RegisterSize;
                        int op2 = BitConverter.ToInt32(instruct.Op2) * RegisterSize;
                        int OP1 = BitConverter.ToInt32(Registers.data [ op1..(op1 + Constants.int_size) ]);
                        MemoryBlocks.Datas [ OP1 ].data [ (op2 * RegisterSize)..(op2 * RegisterSize + RegisterSize) ].CopyTo(Registers.data , op0);
                    }
                    break;
                case (int)Inst.swr:
                    {
                        int op0 = BitConverter.ToInt32(instruct.Op0) * RegisterSize;
                        int op1 = BitConverter.ToInt32(instruct.Op1) * RegisterSize;
                        int op2 = BitConverter.ToInt32(instruct.Op2) * RegisterSize;
                        int OP0 = BitConverter.ToInt32(Registers.data [ op0..(op0 + Constants.int_size) ]);
                        int OP1 = BitConverter.ToInt32(Registers.data [ op1..(op1 + Constants.int_size) ]);
                        int OP2 = BitConverter.ToInt32(Registers.data [ op2..(op2 + Constants.int_size) ]);
                        Registers.data [ OP0..(OP0 + RegisterSize) ].CopyTo(MemoryBlocks.Datas [ OP1 ].data , OP2);
                    }
                    break;
                case (int)Inst.swi:
                    {
                        int op0 = BitConverter.ToInt32(instruct.Op0) * RegisterSize;
                        int op1 = BitConverter.ToInt32(instruct.Op1) * RegisterSize;
                        int op2 = BitConverter.ToInt32(instruct.Op2) * RegisterSize;
                        int OP0 = BitConverter.ToInt32(Registers.data [ op0..(op0 + Constants.int_size) ]);
                        int OP1 = BitConverter.ToInt32(Registers.data [ op1..(op1 + Constants.int_size) ]);
                        Registers.data [ OP0..(OP0 + RegisterSize) ].CopyTo(MemoryBlocks.Datas [ OP1 ].data , op2);
                    }
                    break;
                case (int)Inst.jmp:
                    {
                        int op0 = BitConverter.ToInt32(instruct.Op0) * RegisterSize;
                        PC = op0;
                    }
                    break;
                case (int)Inst.jmpr:
                    {
                        int op0 = BitConverter.ToInt32(instruct.Op0) * RegisterSize;
                        int OP0 = BitConverter.ToInt32(Registers.data [ op0..(op0 + Constants.int_size) ]);
                        PC = OP0;
                    }
                    break;
                case (int)ManagedExt.mcall:
                    {

                    }
                    break;
                default:
                    break;
            }
        }
        int PC = 0;
        public void Run()
        {
            if (program == null) return;
            while (true)
            {
                byte [ ] PCbytes = Registers.data [ 0..3 ];
#if UNSAFE
                unsafe
                {
                    fixed (byte* pc_ptr = PCbytes)
                        PC = *(int*)pc_ptr;
                }
#else
                PC = BitConverter.ToInt32(PCbytes);
#endif
                {
                    //Execute instruct.
                    Execute(program.program.Instructions [ PC ]);
                }
                PC++;
#if UNSAFE
                unsafe
                {
                    byte* pc_ptr = (byte*)PC;
                Registers.data[0] = pc_ptr[0];
                Registers.data[1] = pc_ptr[1];
                Registers.data[2] = pc_ptr[2];
                Registers.data[3] = pc_ptr[3];
                }
#else
                PCbytes = BitConverter.GetBytes(PC);
                Registers.data [ 0 ] = PCbytes [ 0 ];
                Registers.data [ 1 ] = PCbytes [ 1 ];
                Registers.data [ 2 ] = PCbytes [ 2 ];
                Registers.data [ 3 ] = PCbytes [ 3 ];
#endif
            }
        }
    }
    public class ManagedMem
    {
        public List<xCVMObject> Mem = new List<xCVMObject>();
    }
    public class xCVMem
    {
        public byte [ ] data;

        public xCVMem() { data = new byte [ 0 ]; }
        public xCVMem(byte [ ] data)
        {
            this.data = data;
        }
    }
    public class xCVMemBlock
    {
        public Dictionary<int , xCVMem> Datas = new Dictionary<int , xCVMem>();
        public int MALLOC(int Size)
        {
            int K = Datas.Count + 1;
            Datas.Add(K , new xCVMem(new byte [ Size ]));
            return K;
        }
        public void FREE(int Key)
        {
            if (Datas.ContainsKey(Key))
            {
                Datas.Remove(Key);
            }
        }
        public int REALLOC(int Key , int NewSize)
        {
            var newD = new xCVMem(new byte [ NewSize ]);
            if (!Datas.ContainsKey(Key)) return 0;
            var old = Datas [ Key ];
            var L = Math.Min(old.data.Length , NewSize);
            for (int i = 0 ; i < L ; i++)
            {
                newD.data [ i ] = old.data [ i ];
            }
            return Key;
        }
    }
}
