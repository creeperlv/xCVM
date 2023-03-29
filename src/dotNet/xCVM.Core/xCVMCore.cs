using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xCVM.Core
{
    public class xCVMCore
    {
        readonly int long_size = sizeof(long);
        readonly int int_size = sizeof(int);
        readonly int float_size = sizeof(float);
        readonly int double_size = sizeof(double);
        xCVMRTProgram? program = null;
        xCVMem Registers;
        xCVMem NativeMemory;
        ManagedMem ManagedMem;
        public void Execute(Instruct instruct)
        {
            switch (instruct.Operation)
            {
                case (int)Inst.add:
                    {
                        int op0 = BitConverter.ToInt32(instruct.Op0);
                        int op1 = BitConverter.ToInt32(instruct.Op1);
                        int op2 = BitConverter.ToInt32(instruct.Op2);
                        int OP0 = BitConverter.ToInt32(Registers.data[op0..(op0 + int_size)]);
                        int OP1 = BitConverter.ToInt32(Registers.data[op1..(op1 + int_size)]);
                        BitConverter.GetBytes(OP0 + OP1).CopyTo(Registers.data, op2);
                    }
                    break;
                case (int)Inst.sub:
                    {
                        int op0 = BitConverter.ToInt32(instruct.Op0);
                        int op1 = BitConverter.ToInt32(instruct.Op1);
                        int op2 = BitConverter.ToInt32(instruct.Op2);
                        int OP0 = BitConverter.ToInt32(Registers.data[op0..(op0 + int_size)]);
                        int OP1 = BitConverter.ToInt32(Registers.data[op1..(op1 + int_size)]);
                        BitConverter.GetBytes(OP0 - OP1).CopyTo(Registers.data, op2);
                    }
                    break;
                case (int)Inst.mul:
                    {
                        int op0 = BitConverter.ToInt32(instruct.Op0);
                        int op1 = BitConverter.ToInt32(instruct.Op1);
                        int op2 = BitConverter.ToInt32(instruct.Op2);
                        int OP0 = BitConverter.ToInt32(Registers.data[op0..(op0 + int_size)]);
                        int OP1 = BitConverter.ToInt32(Registers.data[op1..(op1 + int_size)]);
                        BitConverter.GetBytes(OP0 * OP1).CopyTo(Registers.data, op2);
                    }
                    break;
                case (int)Inst.div:
                    {
                        int op0 = BitConverter.ToInt32(instruct.Op0);
                        int op1 = BitConverter.ToInt32(instruct.Op1);
                        int op2 = BitConverter.ToInt32(instruct.Op2);
                        int OP0 = BitConverter.ToInt32(Registers.data[op0..(op0 + int_size)]);
                        int OP1 = BitConverter.ToInt32(Registers.data[op1..(op1 + int_size)]);
                        BitConverter.GetBytes(OP0 / OP1).CopyTo(Registers.data, op2);
                    }
                    break;

                case (int)Inst.addi:
                    {
                        int op0 = BitConverter.ToInt32(instruct.Op0);
                        int op1 = BitConverter.ToInt32(instruct.Op1);
                        int op2 = BitConverter.ToInt32(instruct.Op2);
                        int OP0 = BitConverter.ToInt32(Registers.data[op0..(op0 + int_size)]);
                        BitConverter.GetBytes(OP0 + op1).CopyTo(Registers.data, op2);
                    }
                    break;
                case (int)Inst.subi:
                    {
                        int op0 = BitConverter.ToInt32(instruct.Op0);
                        int op1 = BitConverter.ToInt32(instruct.Op1);
                        int op2 = BitConverter.ToInt32(instruct.Op2);
                        int OP0 = BitConverter.ToInt32(Registers.data[op0..(op0 + int_size)]);
                        BitConverter.GetBytes(OP0 - op1).CopyTo(Registers.data, op2);
                    }
                    break;
                case (int)Inst.muli:
                    {
                        int op0 = BitConverter.ToInt32(instruct.Op0);
                        int op1 = BitConverter.ToInt32(instruct.Op1);
                        int op2 = BitConverter.ToInt32(instruct.Op2);
                        int OP0 = BitConverter.ToInt32(Registers.data[op0..(op0 + int_size)]);
                        BitConverter.GetBytes(OP0 * op1).CopyTo(Registers.data, op2);
                    }
                    break;
                case (int)Inst.divi:
                    {
                        int op0 = BitConverter.ToInt32(instruct.Op0);
                        int op1 = BitConverter.ToInt32(instruct.Op1);
                        int op2 = BitConverter.ToInt32(instruct.Op2);
                        int OP0 = BitConverter.ToInt32(Registers.data[op0..(op0 + int_size)]);
                        BitConverter.GetBytes(OP0 / op1).CopyTo(Registers.data, op2);
                    }
                    break;
                case (int)Inst.ladd:
                    {
                        int op0 = BitConverter.ToInt32(instruct.Op0);
                        int op1 = BitConverter.ToInt32(instruct.Op1);
                        int op2 = BitConverter.ToInt32(instruct.Op2);
                        long OP0 = BitConverter.ToInt64(Registers.data[op0..(op0 + long_size)]);
                        long OP1 = BitConverter.ToInt64(Registers.data[op1..(op1 + long_size)]);
                        BitConverter.GetBytes(OP0 + OP1).CopyTo(Registers.data, op2);
                    }
                    break;
                case (int)Inst.lsub:
                    {
                        int op0 = BitConverter.ToInt32(instruct.Op0);
                        int op1 = BitConverter.ToInt32(instruct.Op1);
                        int op2 = BitConverter.ToInt32(instruct.Op2);
                        long OP0 = BitConverter.ToInt64(Registers.data[op0..(op0 + long_size)]);
                        long OP1 = BitConverter.ToInt64(Registers.data[op1..(op1 + long_size)]);
                        BitConverter.GetBytes(OP0 - OP1).CopyTo(Registers.data, op2);
                    }
                    break;
                case (int)Inst.lmul:
                    {
                        int op0 = BitConverter.ToInt32(instruct.Op0);
                        int op1 = BitConverter.ToInt32(instruct.Op1);
                        int op2 = BitConverter.ToInt32(instruct.Op2);
                        long OP0 = BitConverter.ToInt64(Registers.data[op0..(op0 + long_size)]);
                        long OP1 = BitConverter.ToInt64(Registers.data[op1..(op1 + long_size)]);
                        BitConverter.GetBytes(OP0 * OP1).CopyTo(Registers.data, op2);
                    }
                    break;
                case (int)Inst.ldiv:
                    {
                        int op0 = BitConverter.ToInt32(instruct.Op0);
                        int op1 = BitConverter.ToInt32(instruct.Op1);
                        int op2 = BitConverter.ToInt32(instruct.Op2);
                        long OP0 = BitConverter.ToInt64(Registers.data[op0..(op0 + long_size)]);
                        long OP1 = BitConverter.ToInt64(Registers.data[op1..(op1 + long_size)]);
                        BitConverter.GetBytes(OP0 / OP1).CopyTo(Registers.data, op2);
                    }
                    break;


                case (int)Inst.laddi:
                    {
                        int op0 = BitConverter.ToInt32(instruct.Op0);
                        int op1 = BitConverter.ToInt32(instruct.Op1);
                        int op2 = BitConverter.ToInt32(instruct.Op2);
                        long OP0 = BitConverter.ToInt64(Registers.data[op0..(op0 + long_size)]);
                        BitConverter.GetBytes(OP0 + op1).CopyTo(Registers.data, op2);
                    }
                    break;
                case (int)Inst.lsubi:
                    {
                        int op0 = BitConverter.ToInt32(instruct.Op0);
                        int op1 = BitConverter.ToInt32(instruct.Op1);
                        int op2 = BitConverter.ToInt32(instruct.Op2);
                        long OP0 = BitConverter.ToInt64(Registers.data[op0..(op0 + long_size)]);
                        BitConverter.GetBytes(OP0 - op1).CopyTo(Registers.data, op2);
                    }
                    break;
                case (int)Inst.lmuli:
                    {
                        int op0 = BitConverter.ToInt32(instruct.Op0);
                        int op1 = BitConverter.ToInt32(instruct.Op1);
                        int op2 = BitConverter.ToInt32(instruct.Op2);
                        long OP0 = BitConverter.ToInt64(Registers.data[op0..(op0 + long_size)]);
                        BitConverter.GetBytes(OP0 * op1).CopyTo(Registers.data, op2);
                    }
                    break;
                case (int)Inst.ldivi:
                    {
                        int op0 = BitConverter.ToInt32(instruct.Op0);
                        int op1 = BitConverter.ToInt32(instruct.Op1);
                        int op2 = BitConverter.ToInt32(instruct.Op2);
                        long OP0 = BitConverter.ToInt64(Registers.data[op0..(op0 + long_size)]);
                        BitConverter.GetBytes(OP0 / op1).CopyTo(Registers.data, op2);
                    }
                    break;


                case (int)Inst.fadd_s:
                    {
                        int op0 = BitConverter.ToInt32(instruct.Op0);
                        int op1 = BitConverter.ToInt32(instruct.Op1);
                        int op2 = BitConverter.ToInt32(instruct.Op2);
                        var OP0 = BitConverter.ToSingle(Registers.data[op0..(op0 + float_size)]);
                        var OP1 = BitConverter.ToSingle(Registers.data[op1..(op1 + float_size)]);
                        BitConverter.GetBytes(OP0 + OP1).CopyTo(Registers.data, op2);
                    }
                    break;
                case (int)Inst.fsub_s:
                    {
                        int op0 = BitConverter.ToInt32(instruct.Op0);
                        int op1 = BitConverter.ToInt32(instruct.Op1);
                        int op2 = BitConverter.ToInt32(instruct.Op2);
                        float OP0 = BitConverter.ToSingle(Registers.data[op0..(op0 + float_size)]);
                        float OP1 = BitConverter.ToSingle(Registers.data[op1..(op1 + float_size)]);
                        BitConverter.GetBytes(OP0 - OP1).CopyTo(Registers.data, op2);
                    }
                    break;
                case (int)Inst.fmul_s:
                    {
                        int op0 = BitConverter.ToInt32(instruct.Op0);
                        int op1 = BitConverter.ToInt32(instruct.Op1);
                        int op2 = BitConverter.ToInt32(instruct.Op2);
                        float OP0 = BitConverter.ToSingle(Registers.data[op0..(op0 + float_size)]);
                        float OP1 = BitConverter.ToSingle(Registers.data[op1..(op1 + float_size)]);
                        BitConverter.GetBytes(OP0 * OP1).CopyTo(Registers.data, op2);
                    }
                    break;
                case (int)Inst.fdiv_s:
                    {
                        int op0 = BitConverter.ToInt32(instruct.Op0);
                        int op1 = BitConverter.ToInt32(instruct.Op1);
                        int op2 = BitConverter.ToInt32(instruct.Op2);
                        float OP0 = BitConverter.ToSingle(Registers.data[op0..(op0 + float_size)]);
                        float OP1 = BitConverter.ToSingle(Registers.data[op1..(op1 + float_size)]);
                        BitConverter.GetBytes(OP0 / OP1).CopyTo(Registers.data, op2);
                    }
                    break;

                case (int)Inst.fadd_d:
                    {
                        int op0 = BitConverter.ToInt32(instruct.Op0);
                        int op1 = BitConverter.ToInt32(instruct.Op1);
                        int op2 = BitConverter.ToInt32(instruct.Op2);
                        var OP0 = BitConverter.ToDouble(Registers.data[op0..(op0 + double_size)]);
                        var OP1 = BitConverter.ToDouble(Registers.data[op1..(op1 + double_size)]);
                        BitConverter.GetBytes(OP0 + OP1).CopyTo(Registers.data, op2);
                    }
                    break;
                case (int)Inst.fsub_d:
                    {
                        int op0 = BitConverter.ToInt32(instruct.Op0);
                        int op1 = BitConverter.ToInt32(instruct.Op1);
                        int op2 = BitConverter.ToInt32(instruct.Op2);
                        double OP0 = BitConverter.ToDouble(Registers.data[op0..(op0 + double_size)]);
                        double OP1 = BitConverter.ToDouble(Registers.data[op1..(op1 + double_size)]);
                        BitConverter.GetBytes(OP0 - OP1).CopyTo(Registers.data, op2);
                    }
                    break;
                case (int)Inst.fmul_d:
                    {
                        int op0 = BitConverter.ToInt32(instruct.Op0);
                        int op1 = BitConverter.ToInt32(instruct.Op1);
                        int op2 = BitConverter.ToInt32(instruct.Op2);
                        double OP0 = BitConverter.ToDouble(Registers.data[op0..(op0 + double_size)]);
                        double OP1 = BitConverter.ToDouble(Registers.data[op1..(op1 + double_size)]);
                        BitConverter.GetBytes(OP0 * OP1).CopyTo(Registers.data, op2);
                    }
                    break;
                case (int)Inst.fdiv_d:
                    {
                        int op0 = BitConverter.ToInt32(instruct.Op0);
                        int op1 = BitConverter.ToInt32(instruct.Op1);
                        int op2 = BitConverter.ToInt32(instruct.Op2);
                        double OP0 = BitConverter.ToDouble(Registers.data[op0..(op0 + double_size)]);
                        double OP1 = BitConverter.ToDouble(Registers.data[op1..(op1 + double_size)]);
                        BitConverter.GetBytes(OP0 / OP1).CopyTo(Registers.data, op2);
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
        public void Run()
        {
            if (program == null) return;
            while (true)
            {
                byte[] PCbytes = Registers.data[0..3];
                int PC = 0;
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
                    Execute(program.program.Instructions[PC]);
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
                Registers.data[0] = PCbytes[0];
                Registers.data[1] = PCbytes[1];
                Registers.data[2] = PCbytes[2];
                Registers.data[3] = PCbytes[3];
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
        public byte[] data;

    }
}
