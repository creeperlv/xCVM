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
                        int OP0 = BitConverter.ToInt32(Registers.data[op0..(op0 + long_size)]);
                        int OP1 = BitConverter.ToInt32(Registers.data[op1..(op1 + long_size)]);
                        BitConverter.GetBytes(OP0 + OP1).CopyTo(Registers.data, op2);
                    }
                    break;
                case (int)Inst.sub:
                    {
                        int op0 = BitConverter.ToInt32(instruct.Op0);
                        int op1 = BitConverter.ToInt32(instruct.Op1);
                        int op2 = BitConverter.ToInt32(instruct.Op2);
                        int OP0 = BitConverter.ToInt32(Registers.data[op0..(op0 + long_size)]);
                        int OP1 = BitConverter.ToInt32(Registers.data[op1..(op1 + long_size)]);
                        BitConverter.GetBytes(OP0 - OP1).CopyTo(Registers.data, op2);
                    }
                    break;
                case (int)Inst.mul:
                    {
                        int op0 = BitConverter.ToInt32(instruct.Op0);
                        int op1 = BitConverter.ToInt32(instruct.Op1);
                        int op2 = BitConverter.ToInt32(instruct.Op2);
                        int OP0 = BitConverter.ToInt32(Registers.data[op0..(op0 + long_size)]);
                        int OP1 = BitConverter.ToInt32(Registers.data[op1..(op1 + long_size)]);
                        BitConverter.GetBytes(OP0 * OP1).CopyTo(Registers.data, op2);
                    }
                    break;
                case (int)Inst.div:
                    {
                        int op0 = BitConverter.ToInt32(instruct.Op0);
                        int op1 = BitConverter.ToInt32(instruct.Op1);
                        int op2 = BitConverter.ToInt32(instruct.Op2);
                        int OP0 = BitConverter.ToInt32(Registers.data[op0..(op0 + long_size)]);
                        int OP1 = BitConverter.ToInt32(Registers.data[op1..(op1 + long_size)]);
                        BitConverter.GetBytes(OP0 / OP1).CopyTo(Registers.data, op2);
                    }
                    break;

                case (int)Inst.addi:
                    {
                        int op0 = BitConverter.ToInt32(instruct.Op0);
                        int op1 = BitConverter.ToInt32(instruct.Op1);
                        int op2 = BitConverter.ToInt32(instruct.Op2);
                        int OP0 = BitConverter.ToInt32(Registers.data[op0..(op0 + long_size)]);
                        BitConverter.GetBytes(OP0 + op1).CopyTo(Registers.data, op2);
                    }
                    break;
                case (int)Inst.subi:
                    {
                        int op0 = BitConverter.ToInt32(instruct.Op0);
                        int op1 = BitConverter.ToInt32(instruct.Op1);
                        int op2 = BitConverter.ToInt32(instruct.Op2);
                        int OP0 = BitConverter.ToInt32(Registers.data[op0..(op0 + long_size)]);
                        BitConverter.GetBytes(OP0 - op1).CopyTo(Registers.data, op2);
                    }
                    break;
                case (int)Inst.muli:
                    {
                        int op0 = BitConverter.ToInt32(instruct.Op0);
                        int op1 = BitConverter.ToInt32(instruct.Op1);
                        int op2 = BitConverter.ToInt32(instruct.Op2);
                        int OP0 = BitConverter.ToInt32(Registers.data[op0..(op0 + long_size)]);
                        BitConverter.GetBytes(OP0 * op1).CopyTo(Registers.data, op2);
                    }
                    break;
                case (int)Inst.divi:
                    {
                        int op0 = BitConverter.ToInt32(instruct.Op0);
                        int op1 = BitConverter.ToInt32(instruct.Op1);
                        int op2 = BitConverter.ToInt32(instruct.Op2);
                        int OP0 = BitConverter.ToInt32(Registers.data[op0..(op0 + long_size)]);
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
    public class xCVMRTProgram
    {
        public xCVMProgram program;
    }
    public class ManagedMem
    {
        public List<xCVMObject> Mem = new List<xCVMObject>();
    }
    public class xCVMem
    {
        public byte[] data;

    }
    public class xCVMProgram
    {
        public Dictionary<int, string> UsingFunctions = new Dictionary<int, string>();
        public List<Instruct> Instructions = new List<Instruct>();
    }
}
