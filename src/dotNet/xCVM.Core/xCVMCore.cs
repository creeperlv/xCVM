﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace xCVM.Core
{
    public class xCVMCore
    {
        int RegisterSize = Constants.int_size;
        xCVMOption xCVMOption;
        xCVMem Registers;
        xCVMemBlock MemoryBlocks;
        public RuntimeData runtimeData;
        Dictionary<int , xCVMModule> LoadedModules = new Dictionary<int , xCVMModule>();
        Stack<CallFrame> CallStack = new Stack<CallFrame>();
        Dictionary<int , ISysCall> SysCalls = new Dictionary<int , ISysCall>();
        Dictionary<int , Dictionary<int , ISysCall>> FunctionTable = new Dictionary<int , Dictionary<int , ISysCall>>();
        Dictionary<int , xCVMem> PrivateData = new Dictionary<int , xCVMem>();
        public Dictionary<int , IDisposable> Resources = new Dictionary<int , IDisposable>();
        int CurrentModule = -1;
        public int AddResource(IDisposable disposable)
        {
            var id = disposable.GetHashCode();
            Resources.Add(id , disposable);
            return id;
        }
        public void SetResource(int ID , IDisposable disposable)
        {
            Resources.Add(ID , disposable);
        }
        public void RegisterSysCall(int ID , ISysCall call)
        {
            if (SysCalls.ContainsKey(ID))
                SysCalls [ ID ] = call;
            else
                SysCalls.Add(ID , call);
        }
        public void RegisterFunctionCall(int FunctionCollectionID , int FuncID , ISysCall sysCall)
        {
            if (FunctionTable.ContainsKey(FunctionCollectionID))
            {
                if (FunctionTable [ FunctionCollectionID ].ContainsKey(FuncID))
                {
                    FunctionTable [ FunctionCollectionID ] [ FuncID ] = sysCall;
                }
                else
                    FunctionTable [ FunctionCollectionID ].Add(FuncID , sysCall);
            }
            else
            {
                Dictionary<int , ISysCall> functionCollection = new Dictionary<int , ISysCall>
                {
                    { FuncID , sysCall }
                };
                FunctionTable.Add(FunctionCollectionID , functionCollection);
            }
        }
        ModuleProvider? mprovider;
        public xCVMCore(xCVMOption xCVMOption , xCVMemBlock? PredefinedMemories , ModuleProvider? provider)
        {
            this.xCVMOption = xCVMOption;
            this.mprovider = provider;
            RegisterSize = this.xCVMOption.RegisterSize;
            {
                var size = this.xCVMOption.RegisterCount * this.xCVMOption.RegisterSize;
                var barray = new byte [ size ];
                Array.Fill<byte>(barray , 0);
                Registers = new xCVMem() { data = barray };

            }
            MemoryBlocks = new xCVMemBlock();
            //ManagedMem = new ManagedMem();
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
            PrivateData = new Dictionary<int , xCVMem>();
            runtimeData = new RuntimeData(Registers , MemoryBlocks , PrivateData);
        }
        public void SetEnvironmentVariable(List<string> Variables)
        {
            var EID = MemoryBlocks.MALLOC(Variables.Count * Constants.long_size);
            int C = 0;
            foreach (var item in Variables)
            {
                var LB = BitConverter.GetBytes(item.Length);
                var SB = Encoding.UTF8.GetBytes(item);
                var Final = new byte [ LB.Length + SB.Length ];
                LB.CopyTo(Final , 0);
                SB.CopyTo(Final , LB.Length);
                var KID = MemoryBlocks.PUT(Final);
                WriteBytes(KID , MemoryBlocks.Datas [ EID ].data , C);
                C += Constants.int_size;
                WriteBytes(0 , MemoryBlocks.Datas [ EID ].data , C);
                C += Constants.int_size;
            }
            WriteBytes(EID , MemoryBlocks.Datas [ 0 ].data , 0);
            WriteBytes(0 , MemoryBlocks.Datas [ 0 ].data , Constants.int_size);
        }
        public void SetArguments(string [ ] args)
        {
            var AID = MemoryBlocks.MALLOC(args.Length * Constants.int_size);
            int C = 0;
            foreach (var item in args)
            {
                var KID = MemoryBlocks.PUT(Encoding.UTF8.GetBytes(item));
                WriteBytes(KID , MemoryBlocks.Datas [ AID ].data , C);
                C += Constants.int_size;
            }
            WriteBytes(args.Length , MemoryBlocks.Datas [ 0 ].data , Constants.int_size);
            WriteBytes(AID , MemoryBlocks.Datas [ 0 ].data , Constants.int_size * 2);
        }
        void InitMemory()
        {
            MemoryBlocks.MALLOC(3 * RegisterSize , 0);
        }
        byte [ ] VM__BUFFER_4_BYTES;
        byte [ ] VM__BUFFER_8_BYTES;
        byte [ ] VM__BUFFER_16_BYTES;
        #region Dara RW

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ReadInt32(byte [ ] Target , int offset)
        {
            return BitConverter.ToInt32(Target , offset);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteBytes(int Data , byte [ ] Target , int Offset)
        {
            BitConverter.TryWriteBytes(VM__BUFFER_4_BYTES , Data);
            VM__BUFFER_4_BYTES.CopyTo(Target , Offset);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteBytes(short Data , byte [ ] Target , int Offset)
        {
            BitConverter.TryWriteBytes(VM__BUFFER_4_BYTES , Data);
            VM__BUFFER_4_BYTES.CopyTo(Target , Offset);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteBytes(ushort Data , byte [ ] Target , int Offset)
        {
            BitConverter.TryWriteBytes(VM__BUFFER_4_BYTES , Data);
            VM__BUFFER_4_BYTES.CopyTo(Target , Offset);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteBytes(byte Data , byte [ ] Target , int Offset)
        {
            Target [ Offset ] = Data;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteBytesToRegister(int Data , int RegisterID)
        {
            BitConverter.TryWriteBytes(VM__BUFFER_4_BYTES , Data);
            VM__BUFFER_4_BYTES.CopyTo(Registers.data , RegisterID * RegisterSize);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteBytesToMem(int Data , int Pointer , int Offset)
        {
            BitConverter.TryWriteBytes(VM__BUFFER_4_BYTES , Data);
            VM__BUFFER_4_BYTES.CopyTo(MemoryBlocks.Datas [ Pointer ].data , Offset);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteBytes(uint Data , byte [ ] Target , int Offset)
        {
            BitConverter.TryWriteBytes(VM__BUFFER_4_BYTES , Data);
            VM__BUFFER_4_BYTES.CopyTo(Target , Offset);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteBytes(float Data , byte [ ] Target , int Offset)
        {
            BitConverter.TryWriteBytes(VM__BUFFER_4_BYTES , Data);
            VM__BUFFER_4_BYTES.CopyTo(Target , Offset);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteBytes(double Data , byte [ ] Target , int Offset)
        {
            BitConverter.TryWriteBytes(VM__BUFFER_8_BYTES , Data);
            VM__BUFFER_8_BYTES.CopyTo(Target , Offset);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteBytes(long Data , byte [ ] Target , int Offset)
        {
            BitConverter.TryWriteBytes(VM__BUFFER_8_BYTES , Data);
            VM__BUFFER_8_BYTES.CopyTo(Target , Offset);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int RegisterToInt32(byte [ ]? inst_parameter)
        {
            int op_i = BitConverter.ToInt32(inst_parameter) * RegisterSize;
            if (op_i == 0) return 0;
            return BitConverter.ToInt32(Registers.data , op_i);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public short RegisterToInt16(byte [ ]? inst_parameter)
        {
            int op_i = BitConverter.ToInt32(inst_parameter) * RegisterSize;
            if (op_i == 0) return 0;
            return BitConverter.ToInt16(Registers.data , op_i);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ushort RegisterToUInt16(byte [ ]? inst_parameter)
        {
            int op_i = BitConverter.ToInt32(inst_parameter) * RegisterSize;
            if (op_i == 0) return 0;
            return BitConverter.ToUInt16(Registers.data , op_i);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte RegisterToByte(byte [ ]? inst_parameter)
        {
            int op_i = BitConverter.ToInt32(inst_parameter) * RegisterSize;
            if (op_i == 0) return 0;
            return Registers.data [ op_i ];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int RegisterToInt32(int RegisterID)
        {
            int op_i = RegisterID * RegisterSize;
            if (op_i == 0) return 0;
            return BitConverter.ToInt32(Registers.data , op_i);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float RegisterToSingle(byte [ ]? inst_parameter)
        {
            int op_i = BitConverter.ToInt32(inst_parameter) * RegisterSize;
            if (op_i == 0) return 0;
            return BitConverter.ToSingle(Registers.data , op_i);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double RegisterToDouble(byte [ ]? inst_parameter)
        {
            int op_i = BitConverter.ToInt32(inst_parameter) * RegisterSize;
            if (op_i == 0) return 0;
            return BitConverter.ToDouble(Registers.data , op_i);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long RegisterToInt64(byte [ ]? inst_parameter)
        {
            int op_i = BitConverter.ToInt32(inst_parameter) * RegisterSize;
            if (op_i == 0) return 0;
            return BitConverter.ToInt64(Registers.data , op_i);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint RegisterToUInt32(byte [ ]? inst_parameter)
        {
            int op_i = BitConverter.ToInt32(inst_parameter) * RegisterSize;
            if (op_i == 0) return 0;
            return BitConverter.ToUInt32(Registers.data , op_i);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ulong RegisterToUInt64(byte [ ]? inst_parameter)
        {
            int op_i = BitConverter.ToInt32(inst_parameter) * RegisterSize;
            if (op_i == 0) return 0;
            return BitConverter.ToUInt64(Registers.data , op_i);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ImmediateToInt32(byte [ ]? inst_parameter)
        {
            return BitConverter.ToInt32(inst_parameter);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public short ImmediateToInt16(byte [ ]? inst_parameter)
        {
            return BitConverter.ToInt16(inst_parameter);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ushort ImmediateToUInt16(byte [ ]? inst_parameter)
        {
            return BitConverter.ToUInt16(inst_parameter);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte ImmediateToByte(byte [ ]? inst_parameter)
        {
            return inst_parameter? [ 0 ] ?? 0;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long ImmediateToInt64(byte [ ]? inst_parameter)
        {
            return BitConverter.ToInt64(inst_parameter);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float ImmediateToSingle(byte [ ]? inst_parameter)
        {
            return BitConverter.ToSingle(inst_parameter);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double ImmediateToDouble(byte [ ]? inst_parameter)
        {
            return BitConverter.ToDouble(inst_parameter);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint ImmediateToUInt32(byte [ ]? inst_parameter)
        {
            return BitConverter.ToUInt32(inst_parameter);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ulong ImmediateToUInt64(byte [ ]? inst_parameter)
        {
            return BitConverter.ToUInt64(inst_parameter);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ToRegisterOffset(byte [ ]? inst_parameter)
        {
            return BitConverter.ToInt32(inst_parameter) * RegisterSize;
        }
        #endregion
        bool Running;
        public void Execute(Instruct instruct)
        {
            switch (instruct.Operation)
            {
                case (int)Inst.nop:
                    {
                        return;
                    }
                case (int)Inst.ret:
                    {
                        if (CallStack.Count > 0)
                        {
                            var pp = CallStack.Pop();
                            CurrentModule = pp.Module;
                            PC = pp.IP;
                            WriteBytesToRegister(pp.MainStack , Constants.MainStack);
                        }
                        else { Running = false; }
                        return;
                    }
                case (int)Inst.cvt:
                    {
                        int RegisterID = ToRegisterOffset(instruct.Op0!);
                        int OT = ImmediateToInt32(instruct.Op1);
                        int TT = ImmediateToInt32(instruct.Op2);
                        switch (OT)
                        {
                            case Constants._int:
                                {
                                    var odata = RegisterToInt32(instruct.Op0);
                                    cvt(odata);
                                }
                                break;
                            case Constants._uint:
                                {
                                    var odata = RegisterToUInt32(instruct.Op0);
                                    cvt(odata);
                                }
                                break;
                            case Constants._short:
                                {
                                    var odata = RegisterToInt16(instruct.Op0);
                                    cvt(odata);
                                }
                                break;
                            case Constants._ushort:
                                {
                                    var odata = RegisterToUInt16(instruct.Op0);
                                    cvt(odata);
                                }
                                break;
                            case Constants._long:
                                {
                                    var odata = RegisterToInt64(instruct.Op0);
                                    cvt(odata);
                                }
                                break;
                            case Constants._ulong:
                                {
                                    var odata = RegisterToUInt64(instruct.Op0);
                                    cvt(odata);
                                }
                                break;
                            case Constants._byte:
                                {
                                    var odata = RegisterToByte(instruct.Op0);
                                    cvt(odata);
                                }
                                break;
                            case Constants._float:
                                {
                                    var odata = RegisterToSingle(instruct.Op0);
                                    cvt(odata);
                                }
                                break;
                            case Constants._double:
                                {
                                    var odata = RegisterToDouble(instruct.Op0);
                                    cvt(odata);
                                }
                                break;
                            default:
                                return;
                        }
                        void cvt(object odata)
                        {
                            switch (TT)
                            {
                                case Constants._uint:
                                    WriteBytes(Convert.ToUInt32(odata) , Registers.data , Constants.retv * RegisterSize);
                                    break;
                                case Constants._int:
                                    WriteBytes(Convert.ToInt32(odata) , Registers.data , Constants.retv * RegisterSize);
                                    break;
                                case Constants._short:
                                    WriteBytes(Convert.ToInt16(odata) , Registers.data , Constants.retv * RegisterSize);
                                    break;
                                case Constants._ushort:
                                    WriteBytes(Convert.ToUInt16(odata) , Registers.data , Constants.retv * RegisterSize);
                                    break;
                                case Constants._float:
                                    WriteBytes(Convert.ToSingle(odata) , Registers.data , Constants.retv * RegisterSize);
                                    break;
                                case Constants._long:
                                    WriteBytes(Convert.ToInt64(odata) , Registers.data , Constants.retv * RegisterSize);
                                    break;
                                case Constants._double:
                                    WriteBytes(Convert.ToDouble(odata) , Registers.data , Constants.retv * RegisterSize);
                                    break;
                                case Constants._byte:
                                    WriteBytes(Convert.ToByte(odata) , Registers.data , Constants.retv * RegisterSize);
                                    break;
                                case Constants._ulong:
                                    WriteBytes(Convert.ToUInt64(odata) , Registers.data , Constants.retv * RegisterSize);
                                    break;
                                default:
                                    break;
                            }

                        }
                        WriteBytes(RegisterToInt32(instruct.Op0!) + RegisterToInt32(instruct.Op1!) , Registers.data , ToRegisterOffset(instruct.Op2!));
                    }
                    break;
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

                case (int)Inst.sadd:
                    {
                        WriteBytes((short)(RegisterToInt16(instruct.Op0!) + RegisterToInt16(instruct.Op1!)) , Registers.data , ToRegisterOffset(instruct.Op2!));
                    }
                    break;
                case (int)Inst.ssub:
                    {
                        WriteBytes((short)(RegisterToInt16(instruct.Op0!) - RegisterToInt16(instruct.Op1!)) , Registers.data , ToRegisterOffset(instruct.Op2!));
                    }
                    break;
                case (int)Inst.smul:
                    {
                        WriteBytes((short)(RegisterToInt16(instruct.Op0!) * RegisterToInt16(instruct.Op1!)) , Registers.data , ToRegisterOffset(instruct.Op2!));
                    }
                    break;
                case (int)Inst.sdiv:
                    {
                        WriteBytes((short)(RegisterToInt16(instruct.Op0!) / RegisterToInt16(instruct.Op1!)) , Registers.data , ToRegisterOffset(instruct.Op2!));
                    }
                    break;

                case (int)Inst.saddi:
                    {
                        WriteBytes((short)(RegisterToInt16(instruct.Op0!) + ImmediateToInt16(instruct.Op1!)) , Registers.data , ToRegisterOffset(instruct.Op2!));
                    }
                    break;
                case (int)Inst.ssubi:
                    {
                        WriteBytes((short)(RegisterToInt16(instruct.Op0!) - ImmediateToInt16(instruct.Op1!)) , Registers.data , ToRegisterOffset(instruct.Op2!));
                    }
                    break;
                case (int)Inst.smuli:
                    {
                        WriteBytes((short)(RegisterToInt16(instruct.Op0!) * ImmediateToInt16(instruct.Op1!)) , Registers.data , ToRegisterOffset(instruct.Op2!));
                    }
                    break;
                case (int)Inst.sdivi:
                    {
                        WriteBytes((short)(RegisterToInt16(instruct.Op0!) / ImmediateToInt16(instruct.Op1!)) , Registers.data , ToRegisterOffset(instruct.Op2!));
                    }
                    break;

                case (int)Inst.usadd:
                    {
                        WriteBytes((ushort)(RegisterToUInt16(instruct.Op0!) + RegisterToUInt16(instruct.Op1!)) , Registers.data , ToRegisterOffset(instruct.Op2!));
                    }
                    break;
                case (int)Inst.ussub:
                    {
                        WriteBytes((ushort)(RegisterToUInt16(instruct.Op0!) - RegisterToUInt16(instruct.Op1!)) , Registers.data , ToRegisterOffset(instruct.Op2!));
                    }
                    break;
                case (int)Inst.usmul:
                    {
                        WriteBytes((ushort)(RegisterToUInt16(instruct.Op0!) * RegisterToUInt16(instruct.Op1!)) , Registers.data , ToRegisterOffset(instruct.Op2!));
                    }
                    break;
                case (int)Inst.usdiv:
                    {
                        WriteBytes((ushort)(RegisterToUInt16(instruct.Op0!) / RegisterToUInt16(instruct.Op1!)) , Registers.data , ToRegisterOffset(instruct.Op2!));
                    }
                    break;

                case (int)Inst.usaddi:
                    {
                        WriteBytes((ushort)(RegisterToUInt16(instruct.Op0!) + ImmediateToUInt16(instruct.Op1!)) , Registers.data , ToRegisterOffset(instruct.Op2!));
                    }
                    break;
                case (int)Inst.ussubi:
                    {
                        WriteBytes((ushort)(RegisterToUInt16(instruct.Op0!) - ImmediateToUInt16(instruct.Op1!)) , Registers.data , ToRegisterOffset(instruct.Op2!));
                    }
                    break;
                case (int)Inst.usmuli:
                    {
                        WriteBytes((ushort)(RegisterToUInt16(instruct.Op0!) * ImmediateToUInt16(instruct.Op1!)) , Registers.data , ToRegisterOffset(instruct.Op2!));
                    }
                    break;
                case (int)Inst.usdivi:
                    {
                        WriteBytes((ushort)(RegisterToUInt16(instruct.Op0!) / ImmediateToUInt16(instruct.Op1!)) , Registers.data , ToRegisterOffset(instruct.Op2!));
                    }
                    break;

                case (int)Inst.badd:
                    {
                        WriteBytes((byte)(RegisterToByte(instruct.Op0!) + RegisterToByte(instruct.Op1!)) , Registers.data , ToRegisterOffset(instruct.Op2!));
                    }
                    break;
                case (int)Inst.bsub:
                    {
                        WriteBytes((byte)(RegisterToByte(instruct.Op0!) - RegisterToByte(instruct.Op1!)) , Registers.data , ToRegisterOffset(instruct.Op2!));
                    }
                    break;
                case (int)Inst.bmul:
                    {
                        WriteBytes((byte)(RegisterToByte(instruct.Op0!) * RegisterToByte(instruct.Op1!)) , Registers.data , ToRegisterOffset(instruct.Op2!));
                    }
                    break;
                case (int)Inst.bdiv:
                    {
                        WriteBytes((byte)(RegisterToByte(instruct.Op0!) / RegisterToByte(instruct.Op1!)) , Registers.data , ToRegisterOffset(instruct.Op2!));
                    }
                    break;

                case (int)Inst.baddi:
                    {
                        WriteBytes((byte)(RegisterToByte(instruct.Op0!) + ImmediateToByte(instruct.Op1!)) , Registers.data , ToRegisterOffset(instruct.Op2!));
                    }
                    break;
                case (int)Inst.bsubi:
                    {
                        WriteBytes((byte)(RegisterToByte(instruct.Op0!) - ImmediateToByte(instruct.Op1!)) , Registers.data , ToRegisterOffset(instruct.Op2!));
                    }
                    break;
                case (int)Inst.bmuli:
                    {
                        WriteBytes((byte)(RegisterToByte(instruct.Op0!) * ImmediateToByte(instruct.Op1!)) , Registers.data , ToRegisterOffset(instruct.Op2!));
                    }
                    break;
                case (int)Inst.bdivi:
                    {
                        WriteBytes((byte)(RegisterToByte(instruct.Op0!) / ImmediateToByte(instruct.Op1!)) , Registers.data , ToRegisterOffset(instruct.Op2!));
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
                        var id = MemoryBlocks.MALLOC(OP0);
                        WriteBytes(id , Registers.data , op1);
                    }
                    break;
                case (int)Inst.pushw:
                    {
                        int OP0 = ToRegisterOffset(instruct.Op0!);
                        int op1 = RegisterToInt32(instruct.Op1!);
                        MemoryBlocks.Push(op1 , Registers.data , OP0 , 4);
                    }
                    break;
                case (int)Inst.pushd:
                    {
                        int OP0 = ToRegisterOffset(instruct.Op0!);
                        int op1 = RegisterToInt32(instruct.Op1!);
                        MemoryBlocks.Push(op1 , Registers.data , OP0 , 2);
                    }
                    break;
                case (int)Inst.pushb:
                    {
                        int OP0 = ToRegisterOffset(instruct.Op0!);
                        int op1 = RegisterToInt32(instruct.Op1!);
                        MemoryBlocks.Push(op1 , Registers.data , OP0 , 1);
                    }
                    break;

                case (int)Inst.lpopw:
                    {
                        int OP0 = ToRegisterOffset(instruct.Op0!);
                        int op1 = RegisterToInt32(instruct.Op1!);
                        MemoryBlocks.Pop(op1 , 4 , false , Registers.data , OP0);
                    }
                    break;
                case (int)Inst.rpopw:
                    {
                        int OP0 = ToRegisterOffset(instruct.Op0!);
                        int op1 = RegisterToInt32(instruct.Op1!);
                        MemoryBlocks.Pop(op1 , 4 , true , Registers.data , OP0);
                    }
                    break;
                case (int)Inst.lpopd:
                    {
                        int OP0 = ToRegisterOffset(instruct.Op0!);
                        int op1 = RegisterToInt32(instruct.Op1!);
                        MemoryBlocks.Pop(op1 , 2 , false , Registers.data , OP0);
                    }
                    break;
                case (int)Inst.rpopd:
                    {
                        int OP0 = ToRegisterOffset(instruct.Op0!);
                        int op1 = RegisterToInt32(instruct.Op1!);
                        MemoryBlocks.Pop(op1 , 2 , true , Registers.data , OP0);
                    }
                    break;
                case (int)Inst.lpopb:
                    {
                        int OP0 = ToRegisterOffset(instruct.Op0!);
                        int op1 = RegisterToInt32(instruct.Op1!);
                        MemoryBlocks.Pop(op1 , 1 , false , Registers.data , OP0);
                    }
                    break;
                case (int)Inst.rpopb:
                    {
                        int OP0 = ToRegisterOffset(instruct.Op0!);
                        int op1 = RegisterToInt32(instruct.Op1!);
                        MemoryBlocks.Pop(op1 , 1 , true , Registers.data , OP0);
                    }
                    break;
                case (int)Inst.mlen:
                    {
                        int OP0 = RegisterToInt32(instruct.Op0!);
                        int op1 = RegisterToInt32(instruct.Op1!);
                        if (MemoryBlocks.Datas.ContainsKey(OP0))
                            WriteBytes(-1 , Registers.data , op1);
                        else
                        {
                            WriteBytes(MemoryBlocks.Datas [ OP0 ].data.Length , Registers.data , op1);
                        }
                    }
                    break;
                case (int)Inst.realloc:
                    {
                        int OP0 = RegisterToInt32(instruct.Op0);
                        int OP1 = RegisterToInt32(instruct.Op1);
                        int op2 = ToRegisterOffset(instruct.Op2);
                        WriteBytes(MemoryBlocks.REALLOC(OP0 , OP1 , false) , Registers.data , op2);
                    }
                    break;
                case (int)Inst.reallocr:
                    {
                        int OP0 = RegisterToInt32(instruct.Op0);
                        int OP1 = RegisterToInt32(instruct.Op1);
                        int op2 = ToRegisterOffset(instruct.Op2);
                        WriteBytes(MemoryBlocks.REALLOC(OP0 , OP1 , true) , Registers.data , op2);
                    }
                    break;
                case (int)Inst.free:
                    {
                        int OP0 = RegisterToInt32(instruct.Op0);
                        MemoryBlocks.FREE(OP0);
                    }
                    break;
                case (int)Inst.lm:
                    {
                        int OP0 = ImmediateToInt32(instruct.Op0);
                        var ID = LoadedModules [ CurrentModule ].IDs [ OP0 ];
                        var __id = ID.GetHashCode();
                        if (LoadedModules.ContainsKey(__id))
                        {

                        }
                        else
                        {
                            if (mprovider != null)
                            {
                                var module = mprovider.LoadModule(ID);
                                if (module != null)
                                {
                                    LoadedModules.Add(__id , module);
                                }
                            }
                        }
                    }
                    break;
                case (int)Inst.lmx:
                    {
                        short OP0 = RegisterToInt16(instruct.Op0);
                        int OP1 = RegisterToInt32(instruct.Op1);
                        int OP2 = RegisterToInt32(instruct.Op2);
                        var ID = LoadedModules [ CurrentModule ].IDs [ OP2 ];
                        var __id = ID.GetHashCode();
                        if (LoadedModules.ContainsKey(__id))
                        {

                        }
                        else
                        {
                            switch (OP0)
                            {
                                case Constants.lm_mem:
                                    {
                                        using MemoryStream ms = new MemoryStream(MemoryBlocks.Datas [ OP1 ].data);
                                        var module = xCVMModule.FromStream(ms);
                                        if (module != null)
                                        {
                                            LoadedModules.Add(__id , module);
                                        }
                                    }
                                    break;
                                case Constants.lm_df:
                                    {
                                        var path = Encoding.UTF8.GetString(MemoryBlocks.Datas [ OP1 ].data);
                                        using Stream stream = File.OpenRead(path);
                                        var module = xCVMModule.FromStream(stream);
                                        if (module != null)
                                        {
                                            LoadedModules.Add(__id , module);
                                        }
                                    }
                                    break;
                                default:
                                    break;
                            }
                            if (mprovider != null)
                            {
                                var module = mprovider.LoadModule(ID);
                                if (module != null)
                                {
                                    LoadedModules.Add(__id , module);
                                }
                            }
                        }
                    }
                    break;
                case (int)Inst.lmr:
                    {
                        int OP0 = RegisterToInt32(instruct.Op0);
                        var ID = LoadedModules [ CurrentModule ].IDs [ OP0 ];
                        var __id = ID.GetHashCode();
                        if (LoadedModules.ContainsKey(__id))
                        {

                        }
                        else
                        {
                            if (mprovider != null)
                            {
                                var module = mprovider.LoadModule(ID);
                                if (module != null)
                                {
                                    LoadedModules.Add(__id , module);
                                }
                            }
                        }
                    }
                    break;
                case (int)Inst.lwr:
                    {
                        int OP1 = RegisterToInt32(instruct.Op1);
                        int OP2 = RegisterToInt32(instruct.Op2);
                        int op0 = ToRegisterOffset(instruct.Op0);
                        MemoryBlocks.Datas [ OP1 ].data [ (OP2)..(OP2 + RegisterSize) ].CopyTo(Registers.data , op0);
                    }
                    break;
                case (int)Inst.lbr:
                    {
                        int OP1 = RegisterToInt32(instruct.Op1);
                        int OP2 = RegisterToInt32(instruct.Op2);
                        int op0 = ToRegisterOffset(instruct.Op0);
                        Registers.data [ op0 ] = MemoryBlocks.Datas [ OP1 ].data [ OP2 ];
                    }
                    break;
                case (int)Inst.lbi:
                    {
                        int OP1 = RegisterToInt32(instruct.Op1);
                        int OP2 = ImmediateToInt32(instruct.Op2);
                        int op0 = ToRegisterOffset(instruct.Op0);
                        Registers.data [ op0 ] = MemoryBlocks.Datas [ OP1 ].data [ OP2 ];
                    }
                    break;
                case (int)Inst.lwi:
                    {
                        int op0 = ToRegisterOffset(instruct.Op0);
                        int op2 = ImmediateToInt32(instruct.Op2);
                        int OP1 = RegisterToInt32(instruct.Op1);
                        MemoryBlocks.Datas [ OP1 ].data [ (op2)..(op2 + RegisterSize) ].CopyTo(Registers.data , op0);
                    }
                    break;
                case (int)Inst.swr:
                    {
                        int OP0 = ToRegisterOffset(instruct.Op0);
                        int OP1 = RegisterToInt32(instruct.Op1);
                        int OP2 = RegisterToInt32(instruct.Op2);
                        Registers.data [ OP0..(OP0 + RegisterSize) ].CopyTo(MemoryBlocks.Datas [ OP1 ].data , OP2);
                    }
                    break;
                case (int)Inst.swi:
                    {
                        int OP0 = ToRegisterOffset(instruct.Op0);
                        int OP1 = RegisterToInt32(instruct.Op1);
                        int op2 = ImmediateToInt32(instruct.Op2);
                        var d = MemoryBlocks.Datas [ OP1 ].data;
                        Registers.data [ OP0..(OP0 + RegisterSize) ].CopyTo(d , op2);
                    }
                    break;
                case (int)Inst.sbi:
                    {
                        int OP0 = ToRegisterOffset(instruct.Op0);
                        int OP1 = RegisterToInt32(instruct.Op1);
                        int op2 = ImmediateToInt32(instruct.Op2);
                        var d = MemoryBlocks.Datas [ OP1 ].data;
                        d [ op2 ] = Registers.data [ OP0 ];
                    }
                    break;
                case (int)Inst.sbr:
                    {
                        int OP0 = ToRegisterOffset(instruct.Op0);
                        int OP1 = RegisterToInt32(instruct.Op1);
                        int op2 = RegisterToInt32(instruct.Op2);
                        var d = MemoryBlocks.Datas [ OP1 ].data;
                        d [ op2 ] = Registers.data [ OP0 ];
                    }
                    break;
                case (int)Inst.cptxt:
                    {
                        int op0 = ImmediateToInt32(instruct.Op0);
                        int OP1 = ToRegisterOffset(instruct.Op1);
                        var b = Encoding.UTF8.GetBytes(LoadedModules [ this.CurrentModule ].Texts [ op0 ]);
                        var id = MemoryBlocks.PUT(b);
                        WriteBytes(id , Registers.data , OP1);
                    }
                    break;
                case (int)Inst.cptxtr:
                    {
                        int op0 = RegisterToInt32(instruct.Op0);
                        int OP1 = ToRegisterOffset(instruct.Op1);
                        var b = Encoding.UTF8.GetBytes(
                        LoadedModules [ this.CurrentModule ].Texts [ op0 ]);
                        var id = MemoryBlocks.PUT(b);
                        WriteBytes(id , Registers.data , OP1);
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
                case (int)Inst.funccall:
                    {
                        int FCID = ImmediateToInt32(instruct.Op0);
                        int FID = ImmediateToInt32(instruct.Op1);
                        if (FCID == 0)
                        {
                            SysCalls [ FID ].Execute(this);
                        }
                        else
                        {
                            FunctionTable [ FCID ] [ FID ].Execute(this);
                        }
                    }
                    break;
                case (int)Inst.funccallr:
                    {
                        int FCID = RegisterToInt32(instruct.Op0);
                        int FID = RegisterToInt32(instruct.Op1);
                        if (FCID == 0)
                        {
                            SysCalls [ FID ].Execute(this);
                        }
                        else
                        {
                            FunctionTable [ FCID ] [ FID ].Execute(this);
                        }
                    }
                    break;
                case (int)Inst.syscall:
                    {
                        SysCalls [ ImmediateToInt32(instruct.Op0) ].Execute(this);
                    }
                    break;
                case (int)Inst.syscallr:
                    {
                        SysCalls [ RegisterToInt32(instruct.Op0) ].Execute(this);
                    }
                    break;
                case (int)Inst.ifj:
                    {
                        var cond = RegisterToInt32(instruct.Op0);
                        if (cond != 0)
                        {
                            PC = ImmediateToInt32(instruct.Op1) - 1;
                        }
                    }
                    break;
                case (int)Inst.ifjr:
                    {
                        var cond = RegisterToInt32(instruct.Op0);
                        if (cond != 0)
                        {
                            PC = RegisterToInt32(instruct.Op1) - 1;
                        }
                    }
                    break;
                case (int)Inst.call:
                    {

                        CallStack.Push(new CallFrame(CurrentModule , PC + 1 , RegisterToInt32(Constants.MainStack)));
                        var m_id = RegisterToInt32(instruct.Op0);
                        var fun_ID = RegisterToInt32(instruct.Op1);
                        var Name = LoadedModules [ CurrentModule ].IDs [ m_id ];
                        CurrentModule = Name.GetHashCode();
                        var func = LoadedModules [ CurrentModule ].ExternFunctions [ fun_ID ];
                        PC = func.Label - 1;
                    }
                    break;
                case (int)Inst.pcs:
                    {
                        CallStack.Push(new CallFrame(CurrentModule , PC + 1 , RegisterToInt32(Constants.MainStack)));
                    }
                    break;
                case (int)Inst.pcso:
                    {
                        CallStack.Push(new CallFrame(CurrentModule , PC + ImmediateToInt32(instruct.Op0) , RegisterToInt32(Constants.MainStack)));
                    }
                    break;
                case (int)Inst.pcsor:
                    {
                        CallStack.Push(new CallFrame(CurrentModule , PC + RegisterToInt32(instruct.Op0) , RegisterToInt32(Constants.MainStack)));
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
                        WriteBytes(result , Registers.data , 4 * RegisterSize);
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
                        WriteBytes(result , Registers.data , 4 * RegisterSize);
                    }
                    break;
                case (int)Inst.bcmp:
                    {
                        var comp = ImmediateToInt32(instruct.Op2);
                        var L = RegisterToByte(instruct.Op0);
                        var R = RegisterToByte(instruct.Op1);
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
                        WriteBytes(result , Registers.data , 4 * RegisterSize);
                    }
                    break;
                case (int)Inst.bcmpi:
                    {
                        var comp = ImmediateToInt32(instruct.Op2);
                        var L = RegisterToByte(instruct.Op0);
                        var R = ImmediateToByte(instruct.Op1);
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
                        WriteBytes(result , Registers.data , 4 * RegisterSize);
                    }
                    break;
                case (int)Inst.scmp:
                    {
                        var comp = ImmediateToInt32(instruct.Op2);
                        var L = RegisterToSingle(instruct.Op0);
                        var R = RegisterToSingle(instruct.Op1);
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
                        WriteBytes(result , Registers.data , 4 * RegisterSize);
                    }
                    break;
                case (int)Inst.scmpi:
                    {
                        var comp = ImmediateToInt32(instruct.Op2);
                        var L = RegisterToSingle(instruct.Op0);
                        var R = ImmediateToSingle(instruct.Op1);
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
                        WriteBytes(result , Registers.data , 4 * RegisterSize);
                    }
                    break;
                case (int)Inst.uscmp:
                    {
                        var comp = ImmediateToInt32(instruct.Op2);
                        var L = RegisterToUInt16(instruct.Op0);
                        var R = RegisterToUInt16(instruct.Op1);
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
                        WriteBytes(result , Registers.data , 4 * RegisterSize);
                    }
                    break;
                case (int)Inst.uscmpi:
                    {
                        var comp = ImmediateToInt32(instruct.Op2);
                        var L = RegisterToUInt16(instruct.Op0);
                        var R = ImmediateToUInt16(instruct.Op1);
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
                        WriteBytes(result , Registers.data , 4 * RegisterSize);
                    }
                    break;
                case (int)Inst.ucmp:
                    {
                        var comp = ImmediateToInt32(instruct.Op2);
                        var L = RegisterToUInt32(instruct.Op0);
                        var R = RegisterToUInt32(instruct.Op1);
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
                        WriteBytes(result , Registers.data , 4 * RegisterSize);
                    }
                    break;
                case (int)Inst.ucmpi:
                    {
                        var comp = ImmediateToInt32(instruct.Op2);
                        var L = RegisterToUInt32(instruct.Op0);
                        var R = ImmediateToUInt32(instruct.Op1);
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
                        WriteBytes(result , Registers.data , 4 * RegisterSize);
                    }
                    break;
                case (int)Inst.lcmp:
                    {
                        var comp = ImmediateToInt32(instruct.Op2);
                        var L = RegisterToInt64(instruct.Op0);
                        var R = RegisterToInt64(instruct.Op1);
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
                        WriteBytes(result , Registers.data , 4 * RegisterSize);
                    }
                    break;
                case (int)Inst.lcmpi:
                    {
                        var comp = ImmediateToInt32(instruct.Op2);
                        var L = RegisterToInt64(instruct.Op0);
                        var R = ImmediateToInt64(instruct.Op1);
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
                        WriteBytes(result , Registers.data , 4 * RegisterSize);
                    }
                    break;
                case (int)Inst.ulcmp:
                    {
                        var comp = ImmediateToInt32(instruct.Op2);
                        var L = RegisterToUInt64(instruct.Op0);
                        var R = RegisterToUInt64(instruct.Op1);
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
                        WriteBytes(result , Registers.data , 4 * RegisterSize);
                    }
                    break;
                case (int)Inst.ulcmpi:
                    {
                        var comp = ImmediateToInt32(instruct.Op2);
                        var L = RegisterToUInt64(instruct.Op0);
                        var R = ImmediateToUInt64(instruct.Op1);
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
                        WriteBytes(result , Registers.data , 4 * RegisterSize);
                    }
                    break;
                case (int)Inst.fcmp_s:
                    {
                        var comp = ImmediateToInt32(instruct.Op2);
                        var L = RegisterToSingle(instruct.Op0);
                        var R = RegisterToSingle(instruct.Op1);
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
                        WriteBytes(result , Registers.data , 4 * RegisterSize);
                    }
                    break;
                case (int)Inst.fcmpi_s:
                    {
                        var comp = ImmediateToInt32(instruct.Op2);
                        var L = RegisterToSingle(instruct.Op0);
                        var R = ImmediateToSingle(instruct.Op1);
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
                        WriteBytes(result , Registers.data , 4 * RegisterSize);
                    }
                    break;
                case (int)Inst.fcmp_d:
                    {
                        var comp = ImmediateToInt32(instruct.Op2);
                        var L = RegisterToDouble(instruct.Op0);
                        var R = RegisterToDouble(instruct.Op1);
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
                        WriteBytes(result , Registers.data , 4 * RegisterSize);
                    }
                    break;
                case (int)Inst.fcmpi_d:
                    {
                        var comp = ImmediateToInt32(instruct.Op2);
                        var L = RegisterToDouble(instruct.Op0);
                        var R = ImmediateToDouble(instruct.Op1);
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
                        WriteBytes(result , Registers.data , 4 * RegisterSize);
                    }
                    break;
                case (int)Inst.pdint:
                    {
                        var cm = CurrentModule;
                        int Size = sizeof(long);
                        var module = LoadedModules [ cm ];
                        foreach (var item in module.ExternVariables)
                        {
                            Size += DataLength(item , cm);
                        }
                        xCVMem xCVMem = new xCVMem(new byte [ Size ]);
                        if (PrivateData.ContainsKey(cm))
                            PrivateData [ cm ] = xCVMem;
                        else PrivateData.Add(cm , xCVMem);
                    }
                    break;
                case (int)Inst.pdintm:
                    {
                        var cm = //CurrentModule;
                        RegisterToInt32(instruct.Op0);
                        int Size = sizeof(long);
                        var module = LoadedModules [ cm ];

                        foreach (var item in module.ExternVariables)
                        {
                            Size += DataLength(item , cm);
                        }
                        xCVMem xCVMem = new xCVMem(new byte [ Size ]);
                        if (PrivateData.ContainsKey(cm))
                            PrivateData [ cm ] = xCVMem;
                        else PrivateData.Add(cm , xCVMem);
                    }
                    break;
                case (int)Inst.pdr:
                    {
                        int MemInfo = ImmediateToInt32(instruct.Op0);
                        int mem = RegisterToInt32(MemInfo);
                        int offset = RegisterToInt32(MemInfo + 4);
                        int PDInfo = ImmediateToInt32(instruct.Op1);
                        int DataLength = RegisterToInt32(instruct.Op2);
                        int ModuleID = RegisterToInt32(PDInfo);
                        int Offset_PD = RegisterToInt32(PDInfo + 4);
                        PrivateData [ ModuleID ].data [ Offset_PD..(Offset_PD + DataLength) ].CopyTo(MemoryBlocks.Datas [ mem ].data , offset);
                    }
                    break;
                case (int)Inst.pdw:
                    {
                        int MemInfo = ImmediateToInt32(instruct.Op0);
                        int mem = RegisterToInt32(MemInfo);
                        int offset = RegisterToInt32(MemInfo + 4);
                        int PDInfo = ImmediateToInt32(instruct.Op1);
                        int DataLength = RegisterToInt32(instruct.Op2);
                        int ModuleID = RegisterToInt32(PDInfo);
                        int Offset_PD = RegisterToInt32(PDInfo + 4);
                        MemoryBlocks.Datas [ mem ].data [ offset..(offset + DataLength) ].CopyTo(PrivateData [ ModuleID ].data , Offset_PD);
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
        public int DataLength(DataType type , int Module)
        {
            var l = InternalDataType.Length((InternalDataTypes)type.Type);
            if (l >= 0)
            {
                return l;
            }
            else
            {
                int size = 0;
                var md = LoadedModules [ Module ];
                var st = md.ExternStructs [ type.AdditionalTypeID ];
                foreach (var item in st.Fields)
                {
                    size += DataLength(item.Value , Module);
                }
                return size;
            }
        }
        int PC = 0;
        public void Load(xCVMModule module)
        {
            LoadedModules.Add(module.GetHashCode() , module);
            CurrentModule = module.GetHashCode();
        }
        public void Destory()
        {
            var L = this.Resources.Count;
            Running = false;
            for (int i = 0 ; i < L ; i++)
            {
                var R = Resources.ElementAt(i);
                try
                {
                    R.Value.Dispose();
                }
                catch (Exception)
                {
                }
            }
            LoadedModules.Clear();

        }
        public void Run()
        {
            if (CurrentModule == -1) return;
            Running = true;
            while (Running)
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
                var ins = LoadedModules [ CurrentModule ].Instructions;
                if (PC >= ins.Count)
                {
                    Execute(new Instruct { Operation = (int)Inst.ret });
                }
                else
                {
                    //Execute instruct.
                    Execute(ins [ PC ]);
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
}
