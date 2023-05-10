﻿using System.IO;
using xCVM.Core;

namespace SystemCalls
{
    public class lseek : ISysCall
    {
        public void Execute(xCVMCore core)
        {
            var ParameterPointer = core.RegisterToInt32(3);
            var block = core.runtimeData.MemoryBlocks.Datas [ ParameterPointer ];
            var parameter_count = block.data.Length / 4;
            if (parameter_count == 3)
            {
                int ResourceID = core.ReadInt32(block.data , 0);
                SEEK seek = (SEEK)core.ReadInt32(block.data , 4);
                int offset = core.ReadInt32(block.data , 8);
                var fs = core.Resources [ ResourceID ] as Stream;
                if (fs is null)
                {
                    core.WriteBytesToRegister(-1 , 3);
                    return;
                }

                var l = (int)fs.Seek(offset , (SeekOrigin)seek);
                core.WriteBytesToRegister(l , 3);
                return;
            }
            core.WriteBytesToRegister(-1 , 3);
            return;
        }
    }
}