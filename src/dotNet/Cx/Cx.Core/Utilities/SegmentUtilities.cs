using LibCLCC.NET.TextProcessing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using xCVM.Core.Utilities;

namespace Cx.Core.Utilities
{
    public static class SegmentUtilities
    {
        public static Segment? ReadSegment(this Stream stream)
        {
            var main = stream.ReadBytes();
            if(main.Length == 0)
            {
                return null;
            }
            using (MemoryStream ms = new MemoryStream(main))
            {
                byte [ ] byte4 = new byte [ 4 ];
                byte [ ] charb = new byte [ sizeof(char) ];
                byte [ ] boolb = new byte [ sizeof(bool) ];
                Segment segment = new Segment();

                ms.Read(byte4 , 0 , 4);
                segment.LineNumber = BitConverter.ToInt32(byte4);
                ms.Read(byte4 , 0 , 4);
                segment.Index = BitConverter.ToInt32(byte4);
                ms.Read(boolb , 0 , boolb.Length);
                segment.isEncapsulated = BitConverter.ToBoolean(boolb);
                if (segment.isEncapsulated)
                {
                    SegmentEncapsulationIdentifier segmentEncapsulationIdentifier = new SegmentEncapsulationIdentifier();
                    ms.Read(charb , 0 , charb.Length);
                    segmentEncapsulationIdentifier.L = BitConverter.ToChar(charb);
                    ms.Read(charb , 0 , charb.Length);
                    segmentEncapsulationIdentifier.R = BitConverter.ToChar(charb);
                    segment.EncapsulationIdentifier = segmentEncapsulationIdentifier;
                }
                var b = ms.ReadBytes();
                segment.content = Encoding.UTF8.GetString(b);
                return segment;
            }
        }
        public static void WriteSegment(this Stream stream , Segment? seg)
        {
            if (seg == null)
            {
                stream.Write(BitConverter.GetBytes(0));
                return;
            }
            using (MemoryStream ms = new MemoryStream())
            {
                {
                    var BUF = BitConverter.GetBytes(seg.LineNumber);
                    ms.Write(BUF);
                }
                {
                    var BUF = BitConverter.GetBytes(seg.Index);
                    ms.Write(BUF);
                }
                {
                    var BUF = BitConverter.GetBytes(seg.isEncapsulated);
                    ms.Write(BUF);
                }
                if (seg.isEncapsulated)
                {
                    {
                        var BUF = BitConverter.GetBytes(seg.EncapsulationIdentifier.L);
                        ms.Write(BUF);
                    }
                    {
                        var BUF = BitConverter.GetBytes(seg.EncapsulationIdentifier.R);
                        ms.Write(BUF);
                    }

                }
                {
                    var BUF = Encoding.UTF8.GetBytes(seg.content);
                    ms.WriteBytes(BUF);
                }
                ms.Flush();
                var buf = ms.GetBuffer();
                stream.WriteBytes(buf);
            }
        }
    }
}
