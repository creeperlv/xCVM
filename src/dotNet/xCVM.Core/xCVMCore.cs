﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace xCVM.Core
{
    public class xCVMOption
    {
        public int RegisterSize;
        public int RegisterCount;
    }
    public struct ProgramPosition
    {
        public int Module;
        public int IP;

        public ProgramPosition(int module , int iP)
        {
            Module = module;
            IP = iP;
        }
        public void WriteToBytes(byte [ ] bytes , int Offset , byte [ ] buffer_4_bytes)
        {
            BitConverter.TryWriteBytes(buffer_4_bytes , Module);
            buffer_4_bytes.CopyTo(bytes , Offset);
            BitConverter.TryWriteBytes(buffer_4_bytes , IP);
            buffer_4_bytes.CopyTo(bytes , Offset + 4);

        }
        public static ProgramPosition FromBytes(byte [ ] bytes , int Offset)
        {
            int M = BitConverter.ToInt32(bytes , Offset);
            int IP = BitConverter.ToInt32(bytes , Offset + 4);
            return new ProgramPosition(M , IP);
        }
    }
    public class xCVMCore
    {
        int RegisterSize = Constants.int_size;
        xCVMRTProgram? program = null;
        xCVMem Registers;
        xCVMemBlock MemoryBlocks;
        ManagedMem ManagedMem;
        xCVMOption xCVMOption;
        Dictionary<int , xCVMModule> LoadedModules = new Dictionary<int , xCVMModule>();
        Dictionary<string , int> ModuleNameIDMap = new Dictionary<string , int>();
        Stack<ProgramPosition> CallStack = new Stack<ProgramPosition>();
        int CurrentModule;
        public xCVMCore(xCVMOption xCVMOption , xCVMemBlock? PredefinedMemories)
        {
            this.xCVMOption = xCVMOption;
            RegisterSize = this.xCVMOption.RegisterSize;
            Registers = new xCVMem() { data = new byte [ this.xCVMOption.RegisterCount * this.xCVMOption.RegisterSize ] };
            MemoryBlocks = new xCVMemBlock();
            ManagedMem = new ManagedMem();
            VM__BUFFER_4_BYTES = new byte [ 4 ];
            VM__BUFFER_8_BYTES = new byte [ 8 ];
            VM__BUFFER_16_BYTES = new byte [ 16 ];
            if (PredefinedMemories != null)
            {
                MemoryBlocks = PredefinedMemories;
            }
            else
            {
                MemoryBlocks = new xCVMemBlock();
                InitMemory();
            }
        }
        void InitMemory()
        {
            MemoryBlocks.MALLOC(10 , 0);
        }
        byte [ ] VM__BUFFER_4_BYTES;
        byte [ ] VM__BUFFER_8_BYTES;
        byte [ ] VM__BUFFER_16_BYTES;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void WriteBytes(int Data , byte [ ] Target , int Offset)
        {
            BitConverter.TryWriteBytes(VM__BUFFER_4_BYTES , Data);
            VM__BUFFER_4_BYTES.CopyTo(Target , Offset);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void WriteBytes(uint Data , byte [ ] Target , int Offset)
        {
            BitConverter.TryWriteBytes(VM__BUFFER_4_BYTES , Data);
            VM__BUFFER_4_BYTES.CopyTo(Target , Offset);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void WriteBytes(float Data , byte [ ] Target , int Offset)
        {
            BitConverter.TryWriteBytes(VM__BUFFER_4_BYTES , Data);
            VM__BUFFER_4_BYTES.CopyTo(Target , Offset);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void WriteBytes(double Data , byte [ ] Target , int Offset)
        {
            BitConverter.TryWriteBytes(VM__BUFFER_8_BYTES , Data);
            VM__BUFFER_8_BYTES.CopyTo(Target , Offset);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void WriteBytes(long Data , byte [ ] Target , int Offset)
        {
            BitConverter.TryWriteBytes(VM__BUFFER_8_BYTES , Data);
            VM__BUFFER_8_BYTES.CopyTo(Target , Offset);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        int RegisterToInt32(byte [ ]? inst_parameter)
        {
            int op_i = BitConverter.ToInt32(inst_parameter) * RegisterSize;
            if (op_i == 0) return 0;
            return BitConverter.ToInt32(Registers.data , op_i);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        float RegisterToSingle(byte [ ]? inst_parameter)
        {
            int op_i = BitConverter.ToInt32(inst_parameter) * RegisterSize;
            if (op_i == 0) return 0;
            return BitConverter.ToSingle(Registers.data , op_i);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        double RegisterToDouble(byte [ ]? inst_parameter)
        {
            int op_i = BitConverter.ToInt32(inst_parameter) * RegisterSize;
            if (op_i == 0) return 0;
            return BitConverter.ToDouble(Registers.data , op_i);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        long RegisterToInt64(byte [ ]? inst_parameter)
        {
            int op_i = BitConverter.ToInt32(inst_parameter) * RegisterSize;
            if (op_i == 0) return 0;
            return BitConverter.ToInt64(Registers.data , op_i);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        uint RegisterToUInt32(byte [ ]? inst_parameter)
        {
            int op_i = BitConverter.ToInt32(inst_parameter) * RegisterSize;
            if (op_i == 0) return 0;
            return BitConverter.ToUInt32(Registers.data , op_i);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        ulong RegisterToUInt64(byte [ ]? inst_parameter)
        {
            int op_i = BitConverter.ToInt32(inst_parameter) * RegisterSize;
            if (op_i == 0) return 0;
            return BitConverter.ToUInt64(Registers.data , op_i);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        int ImmediateToInt32(byte [ ]? inst_parameter)
        {
            return BitConverter.ToInt32(inst_parameter);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        long ImmediateToInt64(byte [ ]? inst_parameter)
        {
            return BitConverter.ToInt64(inst_parameter);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        float ImmediateToSingle(byte [ ]? inst_parameter)
        {
            return BitConverter.ToSingle(inst_parameter);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        double ImmediateToDouble(byte [ ]? inst_parameter)
        {
            return BitConverter.ToDouble(inst_parameter);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        uint ImmediateToUInt32(byte [ ]? inst_parameter)
        {
            return BitConverter.ToUInt32(inst_parameter);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        ulong ImmediateToUInt64(byte [ ]? inst_parameter)
        {
            return BitConverter.ToUInt64(inst_parameter);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        int ToRegisterOffset(byte [ ]? inst_parameter)
        {
            return BitConverter.ToInt32(inst_parameter) * RegisterSize;
        }
        public void Execute(Instruct instruct)
        {
            switch (instruct.Operation)
            {
                case (int)Inst.nop:
                    {
                        return;
                    }
                case (int)Inst.add:
                    {
                        WriteBytes(RegisterToInt32(instruct.Op0!) + RegisterToInt32(instruct.Op1!) , Registers.data , ToRegisterOffset(instruct.Op2!));
                    }
                    break;
                case (int)Inst.sub:
                    {
                        WriteBytes(RegisterToInt32(instruct.Op0!) - RegisterToInt32(instruct.Op1!) , Registers.data , ToRegisterOffset(instruct.Op2!));
                    }
                    break;
                case (int)Inst.mul:
                    {
                        WriteBytes(RegisterToInt32(instruct.Op0!) * RegisterToInt32(instruct.Op1!) , Registers.data , ToRegisterOffset(instruct.Op2!));
                    }
                    break;
                case (int)Inst.div:
                    {
                        WriteBytes(RegisterToInt32(instruct.Op0!) / RegisterToInt32(instruct.Op1!) , Registers.data , ToRegisterOffset(instruct.Op2!));
                    }
                    break;

                case (int)Inst.addi:
                    {
                        WriteBytes(RegisterToInt32(instruct.Op0!) + ImmediateToInt32(instruct.Op1!) , Registers.data , ToRegisterOffset(instruct.Op2!));
                    }
                    break;
                case (int)Inst.subi:
                    {
                        WriteBytes(RegisterToInt32(instruct.Op0!) - ImmediateToInt32(instruct.Op1!) , Registers.data , ToRegisterOffset(instruct.Op2!));
                    }
                    break;
                case (int)Inst.muli:
                    {
                        WriteBytes(RegisterToInt32(instruct.Op0!) * ImmediateToInt32(instruct.Op1!) , Registers.data , ToRegisterOffset(instruct.Op2!));
                    }
                    break;
                case (int)Inst.divi:
                    {
                        WriteBytes(RegisterToInt32(instruct.Op0!) / ImmediateToInt32(instruct.Op1!) , Registers.data , ToRegisterOffset(instruct.Op2!));
                    }
                    break;

                case (int)Inst.uadd:
                    {
                        WriteBytes(RegisterToUInt32(instruct.Op0!) + RegisterToUInt32(instruct.Op1!) , Registers.data , ToRegisterOffset(instruct.Op2!));
                    }
                    break;
                case (int)Inst.usub:
                    {
                        WriteBytes(RegisterToUInt32(instruct.Op0!) - RegisterToUInt32(instruct.Op1!) , Registers.data , ToRegisterOffset(instruct.Op2!));
                    }
                    break;
                case (int)Inst.umul:
                    {
                        WriteBytes(RegisterToUInt32(instruct.Op0!) * RegisterToUInt32(instruct.Op1!) , Registers.data , ToRegisterOffset(instruct.Op2!));
                    }
                    break;
                case (int)Inst.udiv:
                    {
                        WriteBytes(RegisterToUInt32(instruct.Op0!) / RegisterToUInt32(instruct.Op1!) , Registers.data , ToRegisterOffset(instruct.Op2!));
                    }
                    break;

                case (int)Inst.uaddi:
                    {
                        WriteBytes(RegisterToUInt32(instruct.Op0!) + ImmediateToUInt32(instruct.Op1!) , Registers.data , ToRegisterOffset(instruct.Op2!));
                    }
                    break;
                case (int)Inst.usubi:
                    {
                        WriteBytes(RegisterToUInt32(instruct.Op0!) - ImmediateToUInt32(instruct.Op1!) , Registers.data , ToRegisterOffset(instruct.Op2!));
                    }
                    break;
                case (int)Inst.umuli:
                    {
                        WriteBytes(RegisterToUInt32(instruct.Op0!) * ImmediateToUInt32(instruct.Op1!) , Registers.data , ToRegisterOffset(instruct.Op2!));
                    }
                    break;
                case (int)Inst.udivi:
                    {
                        WriteBytes(RegisterToUInt32(instruct.Op0!) / ImmediateToUInt32(instruct.Op1!) , Registers.data , ToRegisterOffset(instruct.Op2!));
                    }
                    break;

                case (int)Inst.ladd:
                    {
                        WriteBytes(RegisterToInt64(instruct.Op0!) + RegisterToInt64(instruct.Op1!) , Registers.data , ToRegisterOffset(instruct.Op2!));
                    }
                    break;
                case (int)Inst.lsub:
                    {
                        WriteBytes(RegisterToInt64(instruct.Op0!) - RegisterToInt64(instruct.Op1!) , Registers.data , ToRegisterOffset(instruct.Op2!));
                    }
                    break;
                case (int)Inst.lmul:
                    {
                        WriteBytes(RegisterToInt64(instruct.Op0!) * RegisterToInt64(instruct.Op1!) , Registers.data , ToRegisterOffset(instruct.Op2!));
                    }
                    break;
                case (int)Inst.ldiv:
                    {
                        WriteBytes(RegisterToInt64(instruct.Op0!) / RegisterToInt64(instruct.Op1!) , Registers.data , ToRegisterOffset(instruct.Op2!));
                    }
                    break;


                case (int)Inst.laddi:
                    {
                        WriteBytes(RegisterToInt64(instruct.Op0!) + ImmediateToInt64(instruct.Op1!) , Registers.data , ToRegisterOffset(instruct.Op2!));
                    }
                    break;
                case (int)Inst.lsubi:
                    {
                        WriteBytes(RegisterToInt64(instruct.Op0!) - ImmediateToInt64(instruct.Op1!) , Registers.data , ToRegisterOffset(instruct.Op2!));
                    }
                    break;
                case (int)Inst.lmuli:
                    {
                        WriteBytes(RegisterToInt64(instruct.Op0!) * ImmediateToInt64(instruct.Op1!) , Registers.data , ToRegisterOffset(instruct.Op2!));
                    }
                    break;
                case (int)Inst.ldivi:
                    {
                        WriteBytes(RegisterToInt64(instruct.Op0!) / ImmediateToInt64(instruct.Op1!) , Registers.data , ToRegisterOffset(instruct.Op2!));
                    }
                    break;

                case (int)Inst.uladd:
                    {
                        WriteBytes(RegisterToUInt64(instruct.Op0!) + RegisterToUInt64(instruct.Op1!) , Registers.data , ToRegisterOffset(instruct.Op2!));
                    }
                    break;
                case (int)Inst.ulsub:
                    {
                        WriteBytes(RegisterToUInt64(instruct.Op0!) - RegisterToUInt64(instruct.Op1!) , Registers.data , ToRegisterOffset(instruct.Op2!));
                    }
                    break;
                case (int)Inst.ulmul:
                    {
                        WriteBytes(RegisterToUInt64(instruct.Op0!) * RegisterToUInt64(instruct.Op1!) , Registers.data , ToRegisterOffset(instruct.Op2!));
                    }
                    break;
                case (int)Inst.uldiv:
                    {
                        WriteBytes(RegisterToUInt64(instruct.Op0!) / RegisterToUInt64(instruct.Op1!) , Registers.data , ToRegisterOffset(instruct.Op2!));
                    }
                    break;


                case (int)Inst.uladdi:
                    {
                        WriteBytes(RegisterToUInt64(instruct.Op0!) + ImmediateToUInt64(instruct.Op1!) , Registers.data , ToRegisterOffset(instruct.Op2!));
                    }
                    break;
                case (int)Inst.ulsubi:
                    {
                        WriteBytes(RegisterToUInt64(instruct.Op0!) - ImmediateToUInt64(instruct.Op1!) , Registers.data , ToRegisterOffset(instruct.Op2!));
                    }
                    break;
                case (int)Inst.ulmuli:
                    {
                        WriteBytes(RegisterToUInt64(instruct.Op0!) * ImmediateToUInt64(instruct.Op1!) , Registers.data , ToRegisterOffset(instruct.Op2!));
                    }
                    break;
                case (int)Inst.uldivi:
                    {
                        WriteBytes(RegisterToUInt64(instruct.Op0!) / ImmediateToUInt64(instruct.Op1!) , Registers.data , ToRegisterOffset(instruct.Op2!));
                    }
                    break;

                case (int)Inst.fadd_s:
                    {
                        WriteBytes(RegisterToSingle(instruct.Op0!) + RegisterToSingle(instruct.Op1!) , Registers.data , ToRegisterOffset(instruct.Op2!));
                    }
                    break;
                case (int)Inst.fsub_s:
                    {
                        WriteBytes(RegisterToSingle(instruct.Op0!) - RegisterToSingle(instruct.Op1!) , Registers.data , ToRegisterOffset(instruct.Op2!));
                    }
                    break;
                case (int)Inst.fmul_s:
                    {
                        WriteBytes(RegisterToSingle(instruct.Op0!) * RegisterToSingle(instruct.Op1!) , Registers.data , ToRegisterOffset(instruct.Op2!));
                    }
                    break;
                case (int)Inst.fdiv_s:
                    {
                        WriteBytes(RegisterToSingle(instruct.Op0!) / RegisterToSingle(instruct.Op1!) , Registers.data , ToRegisterOffset(instruct.Op2!));
                    }
                    break;

                case (int)Inst.faddi_s:
                    {
                        WriteBytes(RegisterToSingle(instruct.Op0!) + ImmediateToSingle(instruct.Op1!) , Registers.data , ToRegisterOffset(instruct.Op2!));
                    }
                    break;
                case (int)Inst.fsubi_s:
                    {
                        WriteBytes(RegisterToSingle(instruct.Op0!) - ImmediateToSingle(instruct.Op1!) , Registers.data , ToRegisterOffset(instruct.Op2!));
                    }
                    break;
                case (int)Inst.fmuli_s:
                    {
                        WriteBytes(RegisterToSingle(instruct.Op0!) * ImmediateToSingle(instruct.Op1!) , Registers.data , ToRegisterOffset(instruct.Op2!));
                    }
                    break;
                case (int)Inst.fdivi_s:
                    {
                        WriteBytes(RegisterToSingle(instruct.Op0!) / ImmediateToSingle(instruct.Op1!) , Registers.data , ToRegisterOffset(instruct.Op2!));
                    }
                    break;

                case (int)Inst.fadd_d:
                    {
                        WriteBytes(RegisterToDouble(instruct.Op0!) + RegisterToDouble(instruct.Op1!) , Registers.data , ToRegisterOffset(instruct.Op2!));
                    }
                    break;
                case (int)Inst.fsub_d:
                    {
                        WriteBytes(RegisterToDouble(instruct.Op0!) - RegisterToDouble(instruct.Op1!) , Registers.data , ToRegisterOffset(instruct.Op2!));
                    }
                    break;
                case (int)Inst.fmul_d:
                    {
                        WriteBytes(RegisterToDouble(instruct.Op0!) * RegisterToDouble(instruct.Op1!) , Registers.data , ToRegisterOffset(instruct.Op2!));
                    }
                    break;
                case (int)Inst.fdiv_d:
                    {
                        WriteBytes(RegisterToDouble(instruct.Op0!) / RegisterToDouble(instruct.Op1!) , Registers.data , ToRegisterOffset(instruct.Op2!));
                    }
                    break;

                case (int)Inst.faddi_d:
                    {
                        WriteBytes(RegisterToDouble(instruct.Op0!) + ImmediateToDouble(instruct.Op1!) , Registers.data , ToRegisterOffset(instruct.Op2!));
                    }
                    break;
                case (int)Inst.fsubi_d:
                    {
                        WriteBytes(RegisterToDouble(instruct.Op0!) - ImmediateToDouble(instruct.Op1!) , Registers.data , ToRegisterOffset(instruct.Op2!));
                    }
                    break;
                case (int)Inst.fmuli_d:
                    {
                        WriteBytes(RegisterToDouble(instruct.Op0!) * ImmediateToDouble(instruct.Op1!) , Registers.data , ToRegisterOffset(instruct.Op2!));
                    }
                    break;
                case (int)Inst.fdivi_d:
                    {
                        WriteBytes(RegisterToDouble(instruct.Op0!) / ImmediateToDouble(instruct.Op1!) , Registers.data , ToRegisterOffset(instruct.Op2!));
                    }
                    break;

                case (int)Inst.malloc:
                    {
                        int OP0 = RegisterToInt32(instruct.Op0!);
                        int op1 = ToRegisterOffset(instruct.Op1!);
                        WriteBytes(MemoryBlocks.MALLOC(OP0) , Registers.data , op1);
                    }
                    break;
                case (int)Inst.realloc:
                    {
                        int OP0 = RegisterToInt32(instruct.Op0);
                        int OP1 = RegisterToInt32(instruct.Op1);
                        int op2 = ToRegisterOffset(instruct.Op2);
                        WriteBytes(MemoryBlocks.REALLOC(OP0 , OP1) , Registers.data , op2);
                    }
                    break;
                case (int)Inst.free:
                    {
                        int OP0 = RegisterToInt32(instruct.Op0);
                        MemoryBlocks.FREE(OP0);
                    }
                    break;
                case (int)Inst.lwr:
                    {
                        int OP1 = RegisterToInt32(instruct.Op1);
                        int OP2 = RegisterToInt32(instruct.Op2);
                        int op0 = ToRegisterOffset(instruct.Op0);
                        MemoryBlocks.Datas [ OP1 ].data [ (OP2 * RegisterSize)..(OP2 * RegisterSize + RegisterSize) ].CopyTo(Registers.data , op0);
                    }
                    break;
                case (int)Inst.lwi:
                    {
                        int op0 = ToRegisterOffset(instruct.Op0);
                        int op2 = ToRegisterOffset(instruct.Op2);
                        int OP1 = RegisterToInt32(instruct.Op1);
                        MemoryBlocks.Datas [ OP1 ].data [ (op2 * RegisterSize)..(op2 * RegisterSize + RegisterSize) ].CopyTo(Registers.data , op0);
                    }
                    break;
                case (int)Inst.swr:
                    {
                        int OP0 = RegisterToInt32(instruct.Op0);
                        int OP1 = RegisterToInt32(instruct.Op1);
                        int OP2 = RegisterToInt32(instruct.Op2);
                        Registers.data [ OP0..(OP0 + RegisterSize) ].CopyTo(MemoryBlocks.Datas [ OP1 ].data , OP2);
                    }
                    break;
                case (int)Inst.swi:
                    {
                        int op2 = ToRegisterOffset(instruct.Op2);
                        int OP0 = RegisterToInt32(instruct.Op0);
                        int OP1 = RegisterToInt32(instruct.Op1);
                        Registers.data [ OP0..(OP0 + RegisterSize) ].CopyTo(MemoryBlocks.Datas [ OP1 ].data , op2);
                    }
                    break;
                case (int)Inst.jmp:
                    {
                        PC = ImmediateToInt32(instruct.Op0) - 1;
                    }
                    break;
                case (int)Inst.jmpr:
                    {
                        PC = RegisterToInt32(instruct.Op0) - 1;
                    }
                    break;
                case (int)Inst.ifj:
                    {
                        var cond = ImmediateToInt32(instruct.Op0);
                        if (cond != 0)
                        {
                            PC = ImmediateToInt32(instruct.Op1) - 1;
                        }
                    }
                    break;
                case (int)Inst.ifjr:
                    {
                        var cond = ImmediateToInt32(instruct.Op0);
                        if (cond != 0)
                        {
                            PC = RegisterToInt32(instruct.Op1) - 1;
                        }
                    }
                    break;
                case (int)Inst.pcs:
                    {
                        CallStack.Push(new ProgramPosition(CurrentModule , PC + 1));
                    }
                    break;
                case (int)Inst.pcso:
                    {
                        CallStack.Push(new ProgramPosition(CurrentModule , PC + ImmediateToInt32(instruct.Op0)));
                    }
                    break;
                case (int)Inst.pcsor:
                    {
                        CallStack.Push(new ProgramPosition(CurrentModule , PC + RegisterToInt32(instruct.Op0)));
                    }
                    break;
                case (int)Inst.cmp:
                    {
                        var comp = ImmediateToInt32(instruct.Op2);
                        var L = RegisterToInt32(instruct.Op0);
                        var R = RegisterToInt32(instruct.Op1);
                        int result = 0;
                        switch (comp)
                        {
                            case (int)CmpOp.eq:
                                {
                                    result = L == R ? 1 : 0;
                                }
                                break;
                            case (int)CmpOp.neq:
                                {
                                    result = L != R ? 1 : 0;
                                }
                                break;
                            case (int)CmpOp.lt:
                                {
                                    result = L < R ? 1 : 0;
                                }
                                break;
                            case (int)CmpOp.lteq:
                                {
                                    result = L <= R ? 1 : 0;
                                }
                                break;
                            case (int)CmpOp.gt:
                                {
                                    result = L > R ? 1 : 0;
                                }
                                break;
                            case (int)CmpOp.gteq:
                                {
                                    result = L >= R ? 1 : 0;
                                }
                                break;
                            default:
                                break;
                        }
                        WriteBytes(result , Registers.data , 3 * RegisterSize);
                    }
                    break;
                case (int)Inst.cmpi:
                    {
                        var comp = ImmediateToInt32(instruct.Op2);
                        var L = RegisterToInt32(instruct.Op0);
                        var R = ImmediateToInt32(instruct.Op1);
                        int result = 0;
                        switch (comp)
                        {
                            case (int)CmpOp.eq:
                                {
                                    result = L == R ? 1 : 0;
                                }
                                break;
                            case (int)CmpOp.neq:
                                {
                                    result = L != R ? 1 : 0;
                                }
                                break;
                            case (int)CmpOp.lt:
                                {
                                    result = L < R ? 1 : 0;
                                }
                                break;
                            case (int)CmpOp.lteq:
                                {
                                    result = L <= R ? 1 : 0;
                                }
                                break;
                            case (int)CmpOp.gt:
                                {
                                    result = L > R ? 1 : 0;
                                }
                                break;
                            case (int)CmpOp.gteq:
                                {
                                    result = L >= R ? 1 : 0;
                                }
                                break;
                            default:
                                break;
                        }
                        WriteBytes(result , Registers.data , 3 * RegisterSize);
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
        public void Load(xCVMModule module)
        {
            program = new xCVMRTProgram();
            program.program = module;
        }
        public void Run()
        {
            if (program == null) return;
            while (true)
            {
                //byte [ ] PCbytes = Registers.data [ 0..3 ];
#if UNSAFE
                unsafe
                {
                    fixed (byte* pc_ptr = PCbytes)
                        PC = *(int*)pc_ptr;
                }
#else
                //PC = BitConverter.ToInt32(PCbytes);
#endif
                if (PC >= program.program.Instructions.Count) break;
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
                //PCbytes = BitConverter.GetBytes(PC);
                //Registers.data [ 0 ] = PCbytes [ 0 ];
                //Registers.data [ 1 ] = PCbytes [ 1 ];
                //Registers.data [ 2 ] = PCbytes [ 2 ];
                //Registers.data [ 3 ] = PCbytes [ 3 ];
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
        public int MALLOC(int Size , int ForceKey = -1)
        {
            var mem = new xCVMem(new byte [ Size + 1 ]);
            var K = ForceKey == -1 ? mem.GetHashCode() : ForceKey;
            Datas.Add(K , mem);
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
