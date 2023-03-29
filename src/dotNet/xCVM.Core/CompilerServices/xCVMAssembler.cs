using LibCLCC.NET.TextProcessing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xCVM.Core.CompilerServices
{
    public class xCVMAssembler
    {
        public xCVMAssembler() { }
        public xCVMModule Assemble(FileInfo file)
        {
            using var sr = file.OpenText();
            var content = sr.ReadToEnd();
            return Assemble(content);
        }
        public xCVMModule Assemble(Stream stream)
        {
            using (StreamReader sr = new StreamReader(stream))
            {
                return Assemble(sr.ReadToEnd());
            }
        }
        ASMParser parser = new ASMParser();
        public xCVMModule Assemble(Segment segments)
        {
            xCVMModule program = new xCVMModule();
            var current = segments;
            while (true)
            {
                current = segments.Next;
                if (current == null) break;
                if (current.content == "text")
                {

                }
                else
                if (current.content == "text")
                {

                }
            }
            return program;

        }
        public xCVMModule Assemble(string content)
        {
            var segments = parser.Parse(content, false);
            return Assemble(segments);
        }
    }
}
