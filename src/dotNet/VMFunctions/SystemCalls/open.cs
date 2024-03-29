﻿using System;
using System.IO;
using System.Text;
using System.Threading;
using xCVM.Core;

namespace SystemCalls
{
    public enum ACCESS_FLAG
    {
        O_RDONLY = 0b0000,
        O_WRONLY = 0b0001,
        O_RDWR = 0b0010,
    }
    public enum CREATE_FLAG
    {
        O_DEFAULT = 000000,
        O_CREAT = 0b0110_0100,
        O_TRUNC = 00001000,
        O_APPEND = 00002000
    }
    public class open : ISysCall
    {
        public void Execute(xCVMCore core)
        {
            var ParameterPointer = core.RegisterToInt32(3);
            var block = core.runtimeData.MemoryBlocks.Datas [ ParameterPointer ];
            var parameter_count = block.data.Length / 4;
            if (parameter_count == 3)
            {
                var name = core.ReadInt32(block.data , 0);
                var pointer_offset = core.ReadInt32(block.data , 4);
                if (core.runtimeData.MemoryBlocks.Datas.ContainsKey(name) || pointer_offset < 0)
                {
                    core.WriteBytesToRegister(ErrorCodes.Error_Bad_Address , Constants.errno);
                    core.WriteBytesToRegister(-1 , Constants.retv);
                    return;
                }
                var str_buf = core.runtimeData.MemoryBlocks.Datas [ name ].data;
                var str = Encoding.UTF8.GetString(str_buf , pointer_offset , str_buf.Length - pointer_offset);
                if (!File.Exists(str))
                {
                    core.WriteBytesToRegister(ErrorCodes.Error_File_Or_Folder_Not_Exist , Constants.errno);
                    core.WriteBytesToRegister(-1 , Constants.retv);
                    return;
                }
                var flag = core.ReadInt32(block.data , 8);
                var accflg = (ACCESS_FLAG)(flag & 0b0000_0000_0000_0011);
                FileMode fm = FileMode.Open;
                var cflg = (CREATE_FLAG)(flag & 0b1111_1111_1111_1100);
                switch (cflg)
                {
                    case CREATE_FLAG.O_DEFAULT:
                        break;
                    case CREATE_FLAG.O_CREAT:
                        fm = FileMode.Create;
                        break;
                    case CREATE_FLAG.O_TRUNC:
                        fm = FileMode.Truncate;
                        break;
                    case CREATE_FLAG.O_APPEND:
                        fm = FileMode.Append;
                        break;
                    default:
                        break;
                }
                try
                {
                    FileStream fs;
                    switch (accflg)
                    {
                        case ACCESS_FLAG.O_RDONLY:
                            {
                                fs = File.Open(str , fm , FileAccess.Read);
                            }
                            break;
                        case ACCESS_FLAG.O_WRONLY:
                            {
                                fs = File.Open(str , fm , FileAccess.Write);
                            }
                            break;
                        case ACCESS_FLAG.O_RDWR:
                            {
                                fs = File.Open(str , fm , FileAccess.ReadWrite , FileShare.ReadWrite);
                            }
                            break;
                        default:
                            fs = File.Open(str , fm , FileAccess.ReadWrite);
                            break;
                    }
                    var id = core.AddResource(fs);
                    core.WriteBytesToRegister(id , Constants.retv);
                    core.WriteBytesToRegister(0 , Constants.retv + Constants.int_size);
                }
                catch (Exception e)
                {
                    if(e is UnauthorizedAccessException)
                    {
                        core.WriteBytesToRegister(ErrorCodes.Error_No_Access , Constants.errno);
                        core.WriteBytesToRegister(-1 , Constants.retv);
                        return;
                    }else if(e is FileNotFoundException|| e is DirectoryNotFoundException)
                    {
                        core.WriteBytesToRegister(ErrorCodes.Error_File_Or_Folder_Not_Exist , Constants.errno);
                        core.WriteBytesToRegister(-1 , Constants.retv);
                        return;
                    }
                }
            }
        }
    }
}
