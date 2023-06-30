using LibCLCC.NET.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using xCVM.Core.Utilities;

namespace xCVM.Core
{
    public class xCVMModule
    {
        public ModuleMetadata ModuleMetadata = new ModuleMetadata();

        public ModuleSecurity moduleSecurity = new ModuleSecurity();
        public byte [ ] Keys = new byte [ 0 ];
        public byte [ ] Hash = new byte [ 0 ];

        public Dictionary<int , string> IDs = new Dictionary<int , string>();
        public Dictionary<int , string> Texts = new Dictionary<int , string>();
        public Dictionary<int , string> UsingFunctions = new Dictionary<int , string>();
        public List<Instruct> Instructions = new List<Instruct>();
        public List<ExternFunction> ExternFunctions = new List<ExternFunction>();
        public List<ExternStruct> ExternStructs = new List<ExternStruct>();
        public List<DataType> ExternVariables = new List<DataType>();
        public xCVMResource? xCVMResource;
        public void WriteBrinary(Stream stream)
        {
            {
                //Header
                var arr = BinaryUtilities.ToByteArray('x' , 'C' , 'V' , 'M');
                stream.Write(arr);
            }
            {
                switch (moduleSecurity)
                {
                    case ModuleSecurity.NoSecurity:
                        {
                            var arr = BinaryUtilities.ToByteArray('N' , 'O' , 'S' , 'E' , 'C');
                            stream.WriteBytes(arr);
                        }
                        break;
                    case ModuleSecurity.Signed_SHA256:
                        {
                            var arr = BinaryUtilities.ToByteArray('S' , 'I' , 'G' , 'N' , 'S' , '2' , '5' , '6');
                            stream.WriteBytes(arr);
                        }
                        break;
                    default:
                        break;
                }

            }
            var MetaBytes = ModuleMetadata.GetBytes();
            stream.WriteBytes(MetaBytes);
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    ms.Write(BitConverter.GetBytes(ExternFunctions.Count));
                    foreach (var __func in ExternFunctions)
                    {
                        using (MemoryStream funcMem = new MemoryStream())
                        {
                            __func.WriteToStream(funcMem);
                            ms.WriteBytes(funcMem.GetBuffer());
                        }
                    }
                    stream.WriteBytes(ms.GetBuffer());
                }
            }
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    ms.Write(BitConverter.GetBytes(ExternStructs.Count));
                    foreach (var __struct in ExternStructs)
                    {
                        using (MemoryStream funcMem = new MemoryStream())
                        {
                            __struct.WriteToStream(funcMem);
                            ms.WriteBytes(funcMem.GetBuffer());
                        }
                    }
                    stream.WriteBytes(ms.GetBuffer());
                }
            }
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    ms.Write(BitConverter.GetBytes(ExternVariables.Count));
                    foreach (var __variable in ExternVariables)
                    {
                        using (MemoryStream funcMem = new MemoryStream())
                        {
                            __variable.WriteToStream(funcMem);
                            ms.WriteBytes(funcMem.GetBuffer());
                        }
                    }
                    stream.WriteBytes(ms.GetBuffer());
                }
            }
            {
                stream.Write(BitConverter.GetBytes(Texts.Count));
                foreach (var item in Texts)
                {
                    stream.Write(BitConverter.GetBytes(item.Key));
                    stream.WriteBytes(Encoding.UTF8.GetBytes(item.Value));
                }
            }
            {

                stream.Write(BitConverter.GetBytes(IDs.Count));
                foreach (var item in IDs)
                {
                    stream.Write(BitConverter.GetBytes(item.Key));
                    stream.WriteBytes(Encoding.UTF8.GetBytes(item.Value));
                }
            }
            stream.Write(BitConverter.GetBytes(Instructions.Count));
            foreach (var item in Instructions)
            {
                item.WriteToStream(stream);
            }
            {
                xCVMResource?.WriteToStream(stream);
            }
        }
        public static xCVMModule? FromStream(Stream stream)
        {
            xCVMModule xCVMModule = new xCVMModule();
            {
                //Verify.
                var b0 = stream.ReadByte();
                var b1 = stream.ReadByte();
                var b2 = stream.ReadByte();
                var b3 = stream.ReadByte();
                bool Pass = false;
                if (b0 == 'x')
                    if (b1 == 'C')
                        if (b2 == 'V')
                            if (b3 == 'M')
                            {
                                Pass = true;
                            }
                if (!Pass)
                {
                    //Should throw error.
                    return null;
                }
            }
            {
                var b = stream.ReadBytes();
                var SEC = Encoding.ASCII.GetString(b);
            }
            {
                var b = stream.ReadBytes();
                xCVMModule.ModuleMetadata = ModuleMetadata.FromBytes(b);
            }
            {
                var b = stream.ReadBytes();
                using (MemoryStream ms = new MemoryStream(b))
                {
                    byte [ ] bytes = new byte [ 4 ];
                    ms.Read(bytes , 0 , 4);
                    int Count = BitConverter.ToInt32(bytes , 0);
                    for (int i = 0 ; i < Count ; i++)
                    {
                        var bb = ms.ReadBytes();
                        xCVMModule.ExternFunctions.Add(ExternFunction.FromBytes(bb));
                    }
                }
            }
            {
                var b = stream.ReadBytes();
                using (MemoryStream ms = new MemoryStream(b))
                {
                    byte [ ] bytes = new byte [ 4 ];
                    ms.Read(bytes , 0 , 4);
                    int Count = BitConverter.ToInt32(bytes , 0);
                    for (int i = 0 ; i < Count ; i++)
                    {
                        var bb = ms.ReadBytes();
                        xCVMModule.ExternStructs.Add(ExternStruct.FromBytes(bb));
                    }
                }
            }
            {
                var b = stream.ReadBytes();
                using (MemoryStream ms = new MemoryStream(b))
                {
                    byte [ ] bytes = new byte [ 4 ];
                    ms.Read(bytes , 0 , 4);
                    int Count = BitConverter.ToInt32(bytes , 0);
                    for (int i = 0 ; i < Count ; i++)
                    {
                        xCVMModule.ExternVariables.Add(DataType.FromStream(ms));
                    }
                }
            }
            {
                byte [ ] bytes = new byte [ 4 ];
                stream.Read(bytes , 0 , 4);
                int Count = BitConverter.ToInt32(bytes , 0);
                for (int i = 0 ; i < Count ; i++)
                {

                    stream.Read(bytes , 0 , 4);
                    int ID = BitConverter.ToInt32(bytes , 0);
                    var b = stream.ReadBytes();
                    string t = Encoding.UTF8.GetString(b);
                    xCVMModule.Texts.Add(ID , t);
                }
            }
            {
                byte [ ] bytes = new byte [ 4 ];
                stream.Read(bytes , 0 , 4);
                int Count = BitConverter.ToInt32(bytes , 0);
                for (int i = 0 ; i < Count ; i++)
                {

                    stream.Read(bytes , 0 , 4);
                    int ID = BitConverter.ToInt32(bytes , 0);
                    var b = stream.ReadBytes();
                    string t = Encoding.UTF8.GetString(b);
                    xCVMModule.IDs.Add(ID , t);
                }
            }
            {
                byte [ ] bytes = new byte [ 4 ];
                stream.Read(bytes , 0 , 4);
                int Count = BitConverter.ToInt32(bytes , 0);
                for (int i = 0 ; i < Count ; i++)
                {
                    var b = stream.ReadBytes();
                    xCVMModule.Instructions.Add(Instruct.FromBytes(b));
                }
            }
            if (stream.Position + 1 < stream.Length)
                xCVMModule.xCVMResource = xCVMResource.FromStream(stream);
            return xCVMModule;
        }
    }

    public enum ModuleSecurity
    {
        NoSecurity, Signed_SHA256,
    }
    public class ExternStruct
    {
        public string Name = "";
        public bool RuntimeStruct;
        public Dictionary<int , DataType> Fields = new Dictionary<int , DataType>();
        public void WriteToStream(Stream stream)
        {
            stream.WriteBytes(Encoding.UTF8.GetBytes(Name));
            stream.Write(BitConverter.GetBytes(RuntimeStruct));
            stream.Write(BitConverter.GetBytes(Fields.Count));
            foreach (var item in Fields)
            {
                stream.Write(BitConverter.GetBytes(item.Key));
                item.Value.WriteToStream(stream);
            }
        }
        public static ExternStruct FromStream(Stream stream)
        {
            byte [ ] byte4 = new byte [ 4 ];
            ExternStruct __result = new ExternStruct();
            {
                int Len = stream.ReadInt32(byte4);
                byte [ ] bytes = new byte [ Len ];
                stream.Read(bytes , 0 , Len);
                __result.Name = Encoding.UTF8.GetString(bytes);
            }
            {
                byte [ ] bo = new byte [ sizeof(bool) ];
                stream.Read(byte4 , 0 , sizeof(bool));
                __result.RuntimeStruct = BitConverter.ToBoolean(bo);
            }
            {
                int Len = stream.ReadInt32(byte4);
                for (int i = 0 ; i < Len ; i++)
                {
                    var ID = stream.ReadInt32(byte4);
                    __result.Fields.Add(ID , DataType.FromStream(stream));
                }
            }
            return __result;
        }
        public static ExternStruct FromBytes(byte [ ] bytes)
        {
            int Offset = 0;
            ExternStruct externStruct = new ExternStruct();
            {
                int Len = BitConverter.ToInt32(bytes , Offset);
                Offset += 4;
                string Name = Encoding.UTF8.GetString(bytes , Offset , Len);
                Offset += Len;
                externStruct.Name = Name;
            }
            {
                bool RUNTIM = BitConverter.ToBoolean(bytes , Offset);
                Offset += sizeof(bool);
                externStruct.RuntimeStruct = RUNTIM;
            }
            {
                int Len = BitConverter.ToInt32(bytes , Offset);
                Offset += 4;
                for (int i = 0 ; i < Len ; i++)
                {
                    int ID = BitConverter.ToInt32(bytes , Offset);
                    Offset += 4;
                    DataType dataType = DataType.FromBuffer(bytes , Offset);
                    Offset += 8;
                    externStruct.Fields.Add(ID , dataType);
                }
            }
            return externStruct;
        }
    }
    public class ExternFunction
    {
        public string Name = "";
        public int Label;
        public DataType ReturnType = new DataType { Type = -1 };
        public Dictionary<int , DataType> Registers = new Dictionary<int , DataType>();
        public void WriteToStream(Stream stream)
        {
            stream.WriteBytes(Encoding.UTF8.GetBytes(Name));
            stream.Write(Label.GetBytes());
            ReturnType.WriteToStream(stream);
            stream.Write(Registers.Count.GetBytes());
            foreach (var item in Registers)
            {
                stream.Write(item.Key.GetBytes());
                item.Value.WriteToStream(stream);
            }
        }
        public static ExternFunction FromStream(Stream stream)
        {
            byte [ ] buffer4bytes = new byte [ 4 ];
            stream.Read(buffer4bytes , 0 , 4);
            int Length = BitConverter.ToInt32(buffer4bytes);
            byte [ ] bytes = new byte [ Length ];
            stream.Read(bytes , 0 , Length);
            string Name = Encoding.UTF8.GetString(bytes);
            stream.Read(buffer4bytes , 0 , 4);
            int Label = BitConverter.ToInt32(buffer4bytes);
            ExternFunction func = new ExternFunction();
            func.Name = Name;
            func.Label = Label;
            {
                DataType dataType = DataType.FromStream(stream);
                func.ReturnType = dataType;
            }
            stream.Read(buffer4bytes , 0 , 4);
            int RegLength = BitConverter.ToInt32(buffer4bytes);
            for (int i = 0 ; i < RegLength ; i++)
            {
                stream.Read(buffer4bytes , 0 , 4);
                int ID = BitConverter.ToInt32(buffer4bytes);
                DataType dataType = DataType.FromStream(stream);
                func.Registers.Add(ID , dataType);
            }
            return func;
        }
        public static ExternFunction FromBytes(byte [ ] bytes)
        {
            int OFFSET = 0;
            int Length = BitConverter.ToInt32(bytes , 0);
            OFFSET += 4;
            ExternFunction externFunction = new ExternFunction();
            var Name = Encoding.UTF8.GetString(bytes , OFFSET , Length);
            externFunction.Name = Name;
            OFFSET += Length;
            int Label = BitConverter.ToInt32(bytes , OFFSET);
            OFFSET += 4;
            DataType __dataType = DataType.FromBuffer(bytes , OFFSET);
            OFFSET += 8;
            externFunction.ReturnType = __dataType;
            externFunction.Label = Label;
            int RegLength = BitConverter.ToInt32(bytes , OFFSET);
            OFFSET += 4;
            for (int i = 0 ; i < RegLength ; i++)
            {
                int ID = BitConverter.ToInt32(bytes , OFFSET);
                OFFSET += 4;
                DataType dataType = DataType.FromBuffer(bytes , OFFSET);
                OFFSET += 8;
                externFunction.Registers.Add(ID , dataType);
            }
            return externFunction;
        }
    }
    public class InternalDataType
    {
        public static int Length(InternalDataTypes dataType)
        {
            switch (dataType)
            {
                case InternalDataTypes.CIL_Object:
                    return 0;
                case InternalDataTypes._int:
                    return sizeof(int);
                case InternalDataTypes._long:
                    return sizeof(long);
                case InternalDataTypes._float:
                    return sizeof(float);
                case InternalDataTypes._double:
                    return sizeof(double);
                case InternalDataTypes._void:
                    return 0;
                case InternalDataTypes._struct:
                    return -2;
                case InternalDataTypes._unsigned_int:
                    return sizeof(uint);
                case InternalDataTypes.unsigned_long:
                    return sizeof(ulong);
                case InternalDataTypes._byte:
                    return sizeof(byte);
                case InternalDataTypes._short:
                    return sizeof(short);
                case InternalDataTypes._ushort:
                    return sizeof(ushort);
                default:
                    return 0;
            }
        }
    }
    public enum InternalDataTypes
    {
        CIL_Object = 0, _int = 1, _long = 2, _float = 3, _double = 4, _void = -1, _struct = -2, _unsigned_int = 5, unsigned_long = 6,
        _byte = 7,
        _short = 8,
        _ushort = 9,
    }
    public class DataType
    {
        public int Type;
        public int AdditionalTypeID;
        public void WriteToStream(Stream stream)
        {
            stream.Write(BitConverter.GetBytes(Type));
            stream.Write(BitConverter.GetBytes(AdditionalTypeID));
        }
        public static DataType FromBuffer(byte [ ] buffer , int offset)
        {
            int T = BitConverter.ToInt32(buffer , offset);
            int A = BitConverter.ToInt32(buffer , offset + 4);
            return new DataType { Type = T , AdditionalTypeID = A };
        }
        public static DataType FromStream(Stream stream)
        {
            byte [ ] byte4 = new byte [ 4 ];
            stream.Read(byte4 , 0 , 4);
            int T = BitConverter.ToInt32(byte4);
            stream.Read(byte4 , 0 , 4);
            int A = BitConverter.ToInt32(byte4);
            return new DataType { Type = T , AdditionalTypeID = A };
        }
    }
}
