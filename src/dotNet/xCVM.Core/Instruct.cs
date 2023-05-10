using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using xCVM.Core.Utilities;

namespace xCVM.Core
{
    [Serializable]
    public class Instruct
    {
        public int Operation;
        public byte [ ]? Op0;
        public byte [ ]? Op1;
        public byte [ ]? Op2;
        public List<byte> GetBytes()
        {
            List<byte> bytes = new List<byte>();
            {
                bytes.Concatenate(BitConverter.GetBytes(Operation));
            }
            if (Op0 != null)
                bytes.Concatenate(BitConverter.GetBytes((short)Op0.Length));
            else
                bytes.Concatenate(BitConverter.GetBytes((short)0));
            if (Op1 != null)
                bytes.Concatenate(BitConverter.GetBytes((short)Op1.Length));
            else
                bytes.Concatenate(BitConverter.GetBytes((short)0));
            if (Op2 != null)
                bytes.Concatenate(BitConverter.GetBytes((short)Op2.Length));
            else
                bytes.Concatenate(BitConverter.GetBytes((short)0));
            if (Op0 != null)
                bytes.Concatenate(Op0);
            if (Op1 != null)
                bytes.Concatenate(Op1);
            if (Op2 != null)
                bytes.Concatenate(Op2);
            return bytes;
        }
        public static Instruct FromBytes(byte [ ] bytes)
        {
            Instruct instruct = new Instruct();
            int Offset = 0;
            instruct.Operation = BitConverter.ToInt32(bytes [ Offset..(Offset + Constants.int_size) ]);
            Offset += Constants.int_size;
            int OP_LEN0 = BitConverter.ToInt16(bytes [ Offset..(Offset + Constants.short_size) ]);
            Offset += Constants.short_size;
            var OP_LEN1 = BitConverter.ToInt16(bytes [ Offset..(Offset + Constants.short_size) ]);
            Offset += Constants.short_size;
            var OP_LEN2 = BitConverter.ToInt16(bytes [ Offset..(Offset + Constants.short_size) ]);
            Offset += Constants.short_size;
            if (OP_LEN0 > 0)
            {
                instruct.Op0 = bytes [ Offset..(Offset + OP_LEN0) ];
                Offset += OP_LEN0;
            }
            if (OP_LEN1 > 0)
            {
                instruct.Op1 = bytes [ Offset..(Offset + OP_LEN1) ];
                Offset += OP_LEN1;
            }
            if (OP_LEN2 > 0)
            {
                instruct.Op2 = bytes [ Offset..(Offset + OP_LEN2) ];
                Offset += OP_LEN2;
            }
            return instruct;
        }
        public void WriteToStream(Stream stream)
        {
            var b = GetBytes();
            stream.WriteBytes(b.ToArray());
        }
    }
    public enum ManagedExt
    {
        /// <summary>
        ///  mcall ID:function_name, $reg_contains_arg_0_pointer, int:argument_count
        /// </summary>
        mcall = 0x0801,
        /// <summary>
        /// mset $reg_contains_pointer_to_object, ID:Field_Name, $reg_contains_pointer_to_data # int:data_type is at 
        /// </summary>
        mset = 0x0802, thread = 0x0803, mrd = 0x804
    }
    public enum CmpOp
    {
        eq = 0, neq = 1, lt = 2, lteq = 3, gt = 4, gteq = 5,
    }
    public enum Inst
    {
        nop = 0xFFFF,

        add = 0x0001, addi = 0x0002,
        sub = 0x0003, subi = 0x0004,
        mul = 0x0005, muli = 0x0006,
        div = 0x0007, divi = 0x0008,

        uadd = 0x0071, uaddi = 0x0072,
        usub = 0x0073, usubi = 0x0074,
        umul = 0x0075, umuli = 0x0076,
        udiv = 0x0077, udivi = 0x0078,

        fadd_s = 0x0011, faddi_s = 0x0012,
        fsub_s = 0x0013, fsubi_s = 0x0014,
        fmul_s = 0x0015, fmuli_s = 0x0016,
        fdiv_s = 0x0017, fdivi_s = 0x0018,

        fadd_d = 0x0021, faddi_d = 0x0022,
        fsub_d = 0x0023, fsubi_d = 0x0024,
        fmul_d = 0x0025, fmuli_d = 0x0026,
        fdiv_d = 0x0027, fdivi_d = 0x0028,
        /// <summary>
        /// compare
        /// cmp $L $R compare_method
        /// compare_methods:
        /// equal, not_equal, less, LessOrEqual,Greater,GreaterOrEqual.
        /// </summary>
        cmp = 0x0009,
        ucmp = 0x0079,
        fcmp_s = 0x0019,
        fcmp_d = 0x0029,
        cmpi = 0x000A,
        ucmpi = 0x007A,
        fcmpi_s = 0x001A,
        fcmpi_d = 0x002A,

        ladd = 0x0051, laddi = 0x0052,
        lsub = 0x0053, lsubi = 0x0054,
        lmul = 0x0055, lmuli = 0x0056,
        ldiv = 0x0057, ldivi = 0x0058,

        uladd = 0x0081, uladdi = 0x0082,
        ulsub = 0x0083, ulsubi = 0x0084,
        ulmul = 0x0085, ulmuli = 0x0086,
        uldiv = 0x0087, uldivi = 0x0088,

        lcmp = 0x0059,
        lcmpi = 0x005A,
        ulcmp = 0x0089,
        ulcmpi = 0x008A,

        /// <summary>
        /// Copy Resource to new memory area.
        /// <br/>
        /// cpres resID $ReciverRegister
        /// </summary>
        cpres = 0x0090,
        /// <summary>
        /// Copy Text to new memory area.
        /// cptxt id $register_to_receive_memory_location
        /// </summary>
        cptxt = 0x0091,
        cpid = 0x0092,
        /// <summary>
        /// load module
        /// <br/>
        /// lm id?module_id
        /// </summary>
        lm = 0x0095,
        /// <summary>
        /// Load Module which name given by register.
        /// <br/>
        /// lmr $register_to_name
        /// </summary>
        lmr = 0x0096,
        /// <summary>
        /// Load Module from resource.
        /// <br/>
        /// lmres res_id
        /// </summary>
        lmres = 0x0097,
        /// <summary>
        /// Load Module from resource where id is given in register
        /// <br/>
        /// lmresr $register_to_id
        /// </summary>
        lmresr = 0x0098,
        /// <summary>
        /// Push to call stack
        /// <br/>
        /// pcs
        /// </summary>
        pcs = 0x0099,
        /// <summary>
        /// Push to call stack with offset
        /// <br/>
        /// pcso offset
        /// </summary>
        pcso = 0x009A,
        /// <summary>
        /// Push to call stack with offset which is in register.
        /// <br/>
        /// pcsor $register_to_offset
        /// </summary>
        pcsor = 0x009B,
        /// <summary>
        /// system call with id which is given by register
        /// <br/>
        /// syscallr $register_to_the_call
        /// </summary>
        syscallr = 0x009C,
        /// <summary>
        /// Copy text where the ID is given by register.
        /// <br/>
        /// cptxtr $register_to_text $register_to_receive_ID.
        /// </summary>
        cptxtr =0x009D,
        /// <summary>
        /// Jump to an absolute instruct.
        /// <br/>
        /// jmp value
        /// </summary>
        jmp = 0x0040,
        /// <summary>
        /// Jump to an absolute instruct which is in the register.
        /// <br/>
        /// jmpr $register
        /// </summary>
        jmpr = 0x0066,
        /// <summary>
        /// IFJump
        /// <br/>
        /// ifj $condition value
        /// </summary>
        ifj = 0x0067,
        /// <summary>
        /// IFJump(Register)
        /// <br/>
        /// ifjr $condition $pointer
        /// </summary>
        ifjr = 0x0068,
        /// <summary>
        /// Return to last call stack
        /// <br/>
        /// ret
        /// </summary>
        ret = 0x0041,
        cvt_sf_i = 0x0030, cvt_i_sf = 0x0031, cvt_df_i = 0x0032, cvt_i_df = 0x0033,
        sqrt = 0x000B, fsqrt_s = 0x001B, fsqrt_d = 0x002B,
        /// <summary>
        /// Jump to target function in given module.
        /// <br/>
        /// call $register_to_module $register_to_id
        /// </summary>
        call = 0x0042,
        mv = 0x0043,
        /// <summary>
        /// system call
        /// syscall id
        /// </summary>
        syscall = 0x0044,
        /// <summary>
        /// malloc $register_contains_size $register_to_store_pointer
        /// </summary>
        malloc = 0x005B,
        /// <summary>
        /// realloc $register_contain_original_pointer $register_contains_new_size $register_to_store_pointer
        /// </summary>
        realloc = 0x005C,
        /// <summary>
        /// Re-Allocate Right aligned.
        /// reallocl $register_contain_original_pointer $register_contains_new_size $register_to_store_pointer
        /// </summary>
        reallocl = 0x005F,
        /// <summary>
        /// free $register_contains_the_pointer $register_to_store_result
        /// </summary>
        free = 0x005D,
        /// <summary>
        /// mlen $pointer_to_memory $register_to_store_len
        /// </summary>
        mlen = 0x05E,
        /// <summary>
        /// AND Word full Register
        /// <br/>
        /// andw $L $R $Save
        /// </summary>
        andwr = 0x0045,
        /// <summary>
        /// OR Word full Register
        /// orwr $L $R $Save
        /// </summary>
        orwr = 0x0046,
        xorwr = 0x0047,
        notwr = 0x0048,
        /// <summary>
        /// ws - word, second (second 4 bytes)
        /// </summary>
        andwi = 0x0049,
        orwi = 0x004A,
        xorwi = 0x004B,
        notwi = 0x004C,
        /// <summary>
        /// Load Word full Register.
        /// lwr $register_save_value $register_to_memory $register_to_offset
        /// </summary>
        lwr = 0x0060,
        /// <summary>
        /// Load Word Immediate
        /// lwi $register_save_value $register_to_memory offset
        /// </summary>
        lwi = 0x0061,
        /// <summary>
        /// Save word full Register
        /// swr $register_read_value $register_to_memory $register_to_offset
        /// </summary>
        swr = 0x0064,
        /// <summary>
        /// Save word Immediate
        /// swi $register_read_value $register_to_memory offset
        /// </summary>
        swi = 0x0065,

    }
}