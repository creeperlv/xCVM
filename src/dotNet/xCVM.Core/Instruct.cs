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
            int OP_LEN = BitConverter.ToInt16(bytes [ Offset..(Offset + Constants.short_size) ]);
            Offset += Constants.short_size;
            if (OP_LEN > 0)
            {
                instruct.Op0 = bytes [ Offset..(Offset + OP_LEN) ];
            }
            OP_LEN = BitConverter.ToInt16(bytes [ Offset..(Offset + Constants.short_size) ]);
            Offset += Constants.short_size;
            if (OP_LEN > 0)
            {
                instruct.Op1 = bytes [ Offset..(Offset + OP_LEN) ];
            }
            OP_LEN = BitConverter.ToInt16(bytes [ Offset..(Offset + Constants.short_size) ]);
            Offset += Constants.short_size;
            if (OP_LEN > 0)
            {
                instruct.Op2 = bytes [ Offset..(Offset + OP_LEN) ];
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
    public enum Inst
    {
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

        cmp = 0x0009,
        fcmp_s = 0x0019,
        fcmp_d = 0x0029,
        cmpi = 0x000A,
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
        /// <summary>
        /// Jump to an absolute instruct.
        /// jmp value
        /// </summary>
        jmp = 0x0040,
        /// <summary>
        /// Jump to an absolute instruct which is in the register.
        /// jmpr $register
        /// </summary>
        jmpr = 0x0066,
        /// <summary>
        /// IFJump
        /// ifj $condition value
        /// </summary>
        ifj= 0x0067,
        /// <summary>
        /// IFJump(Register)
        /// ifjr $condition $pointer
        /// </summary>
        ifjr= 0x0068,
        ret = 0x0041,
        cvt_sf_i = 0x0030, cvt_i_sf = 0x0031, cvt_df_i = 0x0032, cvt_i_df = 0x0033,
        sqrt = 0x000B, fsqrt_s = 0x001B, fsqrt_d = 0x002B,

        call = 0x0042, 
        mv = 0x0043,
        syscall = 0x0044,
        /// <summary>
        /// Copy Resource to new memory area.
        /// rescp resource_type:TEXT|XCVMRES ID $ReciverRegister
        /// </summary>
        rescp = 0x051,
        /// <summary>
        /// malloc $register_contains_size $register_to_store_pointer
        /// </summary>
        malloc = 0x05B,
        /// <summary>
        /// realloc $register_contain_original_pointer $register_contains_new_size $register_to_store_pointer
        /// </summary>
        realloc = 0x05C,
        /// <summary>
        /// free $register_contains_the_pointer $register_to_store_result
        /// </summary>
        free = 0x05D,
        /// <summary>
        /// mlen $pointer_to_memory $register_to_store_len
        /// </summary>
        mlen=0x05E,
        /// <summary>
        /// AND Word full Register
        /// andw $L $R $Save
        /// </summary>
        andwr = 0x0045,
        /// <summary>
        /// OR Word full Register
        /// orwr $L $R $Save
        /// </summary>
        orwr = 0x0045,
        xorwr = 0x0046,
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