using System;
using System.Collections.Generic;
using System.IO;
using xCVM.Core.Utilities;

namespace xCVM.Core
{
    public class xCVMResource
    {
        public Dictionary<int, byte[]> Datas = new Dictionary<int, byte[]>();
        public static xCVMResource FromStream(Stream stream)
        {
            xCVMResource xCVMResource = new xCVMResource();
            List<int> IDs = new List<int>();
            {
                var RC__HEADER = stream.ReadBytes();
                for (int i = 0; i < RC__HEADER.Length;)
                {
                    int KEY = BitConverter.ToInt32(RC__HEADER, i);
                    i += sizeof(int);
                    IDs.Add(KEY);
                }
            }
            foreach (var key in IDs)
            {
                xCVMResource.Datas.Add(key, stream.ReadBytes());
            }
            return xCVMResource;
        }
        public void WriteToStream(Stream stream)
        {
            byte[] Header;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                foreach (var item in Datas)
                {
                    memoryStream.Write(BitConverter.GetBytes(item.Key));
                }
                Header = memoryStream.GetBuffer();
            }
            stream.WriteBytes(Header);
            foreach (var item in Datas)
            {
                stream.WriteBytes(item.Value);
            }
        }
    }
}
