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
        AssemblerDefinition? AssemblerDefinition;
        public xCVMAssembler(AssemblerDefinition? definition = null)
        {
            AssemblerDefinition = definition;
        }
        public AssembleResult Assemble(FileInfo file)
        {
            using var sr = file.OpenText();
            var content = sr.ReadToEnd();
            return Assemble(parser.Parse(content, false, file.FullName));
        }
        public AssembleResult Assemble(List<FileInfo> files)
        {
            Segment? s = null;
            foreach (var item in files)
            {
                using var sr = item.OpenText();
                var content = sr.ReadToEnd();
                var _s = parser.Parse(content, false, item.FullName);
                if (s is null) s = _s;
                else s.Concatenate(_s);
            }
            if (s is null) return new AssembleResult(new xCVMModule());
            return Assemble(s);
        }
        public AssembleResult Assemble(Stream stream)
        {
            using (StreamReader sr = new StreamReader(stream))
            {
                return Assemble(sr.ReadToEnd());
            }
        }
        ASMParser parser = new ASMParser();
        public AssembleResult Assemble(Segment segments)
        {
            xCVMModule module = new xCVMModule();
            AssembleResult assembleResult = new AssembleResult(module);
            var current = segments;
            SegmentContext context = new SegmentContext(current);
            int Sections = 0;
            Dictionary<string, int> Labels = new Dictionary<string, int>();
            int _IC = 0;
            while (true)
            {

                if (context.ReachEnd)
                    break;
                {

                    (var mr, var section) = context.MatchCollectionMarchWithMatchNext(":", ".module", ".text", ".ids", ".codes");
                    //Console.WriteLine(mr + ":" + section);
                    if (mr == MatchResult.Match)
                    {
                        Sections = section;
                    }
                    else if (mr == MatchResult.Mismatch)
                    {
                        _IC = BaseAssemble(module, assembleResult, context, Sections, Labels, _IC);

                    }
                    else if (mr == MatchResult.ReachEnd)
                    {

                    }
                }

            }
            return assembleResult;

        }

        private int BaseAssemble(xCVMModule module,
                                 AssembleResult assembleResult,
                                 SegmentContext context,
                                 int Sections,
                                 Dictionary<string, int> Labels,
                                 int _IC)
        {
            switch (Sections)
            {
                case 0:
                    {
                        (var _mr, var _selection) = context.MatchCollectionMarchReturnName("ModuleName", "Author", "Copyright", "ModuleVersion", "TargetVersion");
                        if (_mr == MatchResult.Match)
                        {
                            switch (_selection)
                            {
                                case "ModuleName":
                                    {
                                        module.ModuleMetadata.ModuleName = context.Current!.content;
                                        context.GoNext();
                                    }
                                    break;
                                case "Author":
                                    {
                                        module.ModuleMetadata.Author = context.Current!.content;
                                        context.GoNext();
                                    }
                                    break;
                                case "Copyright":
                                    {
                                        module.ModuleMetadata.Copyright = context.Current!.content;
                                        context.GoNext();
                                    }
                                    break;
                                case "ModuleVersion":
                                    {
                                        if (Version.TryParse(context.Current!.content, out var result))
                                        {
                                            module.ModuleMetadata.ModuleVersion = result;
                                        }
                                        else assembleResult.AddError(new VersionFormatError(context.Current));
                                        context.GoNext();
                                    }
                                    break;
                                case "TargetVersion":
                                    {
                                        if (Version.TryParse(context.Current!.content, out var result))
                                        {
                                            module.ModuleMetadata.TargetVersion = result;
                                        }
                                        else assembleResult.AddError(new VersionFormatError(context.Current));
                                        context.GoNext();
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    break;
                case 1:
                    {
                        if (int.TryParse(context.Current!.content, out var result))
                        {
                            if (context.GoNext())
                            {
                                module.Texts.Add(result, context.Current.content);
                            }
                            else
                            {
                                assembleResult.AddError(new UnexpectedEndOfFileError(context.Last));
                                break;
                            }
                        }
                        else
                        {
                            assembleResult.AddError(new IntParseError(context.Current));
                            break;
                        }
                    }
                    break;
                case 2:
                    {
                        if (int.TryParse(context.Current!.content, out var result))
                        {
                            if (context.GoNext())
                            {
                                module.IDs.Add(result, context.Current.content);
                            }
                            else
                            {
                                assembleResult.AddError(new UnexpectedEndOfFileError(context.Last));
                                break;
                            }
                        }
                        else
                        {
                            assembleResult.AddError(new IntParseError(context.Current));
                            break;
                        }
                    }
                    break;
                case 3:
                    {

                        if (AssemblerDefinition == null)
                        {
                            _IC = EmbeddedAssemble(module, assembleResult, context, Labels, _IC);
                        }
                        else
                        {
                            _IC = eXtensibleAssemble(module, assembleResult, context, Labels, _IC);
                        }
                    }
                    break;
                default:
                    break;
            }
            context.GoNext();
            return _IC;
        }

        private int eXtensibleAssemble(xCVMModule module,
                                       AssembleResult assembleResult,
                                       SegmentContext context,
                                       Dictionary<string, int> Labels,
                                       int _IC)
        {
            var a = context.MatachCollectionMarchReturnContentable(AssemblerDefinition!.Definitions);
            if (a.Item1 == MatchResult.Match)
            {
                if (a.Item2 is Instruction3OperatorsDefinition def)
                {
                    Instruct instruct = new Instruct();
                    instruct.Operation = def.ID;
                    _3Operators(module,
                                assembleResult,
                                context,
                                instruct,
                                ref _IC,
                                def.OP0DT,
                                def.OP0REG,
                                def.OP1DT,
                                def.OP1REG,
                                def.OP2DT,
                                def.OP2REG);
                }
            }
            else if (a.Item1 == MatchResult.Mismatch)
            {
                if (context.MatachNext(":", true) == MatchResult.Match)
                {
                    //Label
                    _IC++;
                    Labels.Add(context.Last.Prev.content, _IC);
                }
                else
                {
                    bool willIgnore = false;
                    if (context.Current != null)
                        switch (context.Current.content)
                        {
                            case "":
                            case ";":
                                {
                                    willIgnore = true;
                                }
                                break;
                            default:
                                break;
                        }
                    if (!willIgnore)
                        assembleResult.AddError(new UnknownInstructionError(context.Current));
                    context.GoNext();
                }
            }

            return _IC;
        }

        private static int EmbeddedAssemble(xCVMModule module, AssembleResult assembleResult, SegmentContext context, Dictionary<string, int> Labels, int _IC)
        {
            {
                var matched = context.MatchCollectionMarchReturnName(
                    "add", "addi", "sub", "subi", "mul", "muli", "div", "divi",
                    "ladd", "laddi", "lsub", "lsubi", "lmul", "lmuli", "ldiv", "ldivi",
                    "fadd_s", "faddi_s", "fsub_s", "fsubi_s", "fmul_s", "fmuli_s", "fdiv_s", "fdivi_s"
                    );
                //Console.WriteLine( context.Current.content);
                context.GoBack();
                //Console.WriteLine( context.Current.content);
                if (matched.Item1 == MatchResult.Match)
                {

                    switch (matched.Item2)
                    {
                        case "add":
                            {
                                var inst = new Instruct { Operation = (int)Inst.add };
                                _3Operators(module, assembleResult, context, inst, ref _IC, 1, true, 1, true, 1, true);
                            }
                            break;
                        case "addi":
                            {
                                var inst = new Instruct { Operation = (int)Inst.addi };
                                _3Operators(module, assembleResult, context, inst, ref _IC, 1, true, 1, false, 1, true);
                            }
                            break;
                        case "sub":
                            {
                                var inst = new Instruct { Operation = (int)Inst.sub };
                                _3Operators(module, assembleResult, context, inst, ref _IC, 1, true, 1, true, 1, true);
                            }
                            break;
                        case "subi":
                            {
                                var inst = new Instruct { Operation = (int)Inst.subi };
                                _3Operators(module, assembleResult, context, inst, ref _IC, 1, true, 1, false, 1, true);
                            }
                            break;
                        case "mul":
                            {
                                var inst = new Instruct { Operation = (int)Inst.mul };
                                _3Operators(module, assembleResult, context, inst, ref _IC, 1, true, 1, true, 1, true);
                            }
                            break;
                        case "muli":
                            {
                                var inst = new Instruct { Operation = (int)Inst.muli };
                                _3Operators(module, assembleResult, context, inst, ref _IC, 1, true, 1, false, 1, true);
                            }
                            break;
                        case "div":
                            {
                                var inst = new Instruct { Operation = (int)Inst.div };
                                _3Operators(module, assembleResult, context, inst, ref _IC, 1, true, 1, true, 1, true);
                            }
                            break;
                        case "divi":
                            {
                                var inst = new Instruct { Operation = (int)Inst.divi };
                                _3Operators(module, assembleResult, context, inst, ref _IC, 1, true, 1, false, 1, true);
                            }
                            break;
                        case "ladd":
                            {
                                var inst = new Instruct { Operation = (int)Inst.ladd };
                                _3Operators(module, assembleResult, context, inst, ref _IC, 1, true, 1, true, 1, true);
                            }
                            break;
                        case "laddi":
                            {
                                var inst = new Instruct { Operation = (int)Inst.laddi };
                                _3Operators(module, assembleResult, context, inst, ref _IC, 1, true, 2, false, 1, true);
                            }
                            break;
                        case "lsub":
                            {
                                var inst = new Instruct { Operation = (int)Inst.lsub };
                                _3Operators(module, assembleResult, context, inst, ref _IC, 1, true, 1, true, 1, true);
                            }
                            break;
                        case "lsubi":
                            {
                                var inst = new Instruct { Operation = (int)Inst.lsubi };
                                _3Operators(module, assembleResult, context, inst, ref _IC, 1, true, 2, false, 1, true);
                            }
                            break;
                        case "lmul":
                            {
                                var inst = new Instruct { Operation = (int)Inst.lmul };
                                _3Operators(module, assembleResult, context, inst, ref _IC, 1, true, 1, true, 1, true);
                            }
                            break;
                        case "lmuli":
                            {
                                var inst = new Instruct { Operation = (int)Inst.lmuli };
                                _3Operators(module, assembleResult, context, inst, ref _IC, 1, true, 2, false, 1, true);
                            }
                            break;
                        case "ldiv":
                            {
                                var inst = new Instruct { Operation = (int)Inst.ldiv };
                                _3Operators(module, assembleResult, context, inst, ref _IC, 1, true, 1, true, 1, true);
                            }
                            break;
                        case "ldivi":
                            {
                                var inst = new Instruct { Operation = (int)Inst.ldivi };
                                _3Operators(module, assembleResult, context, inst, ref _IC, 1, true, 2, false, 1, true);
                            }
                            break;
                        case "fadd_s":
                            {
                                var inst = new Instruct { Operation = (int)Inst.fadd_s };
                                _3Operators(module, assembleResult, context, inst, ref _IC, 1, true, 1, true, 1, true);
                            }
                            break;
                        case "faddi_s":
                            {
                                var inst = new Instruct { Operation = (int)Inst.faddi_s };
                                _3Operators(module, assembleResult, context, inst, ref _IC, 1, true, 2, false, 1, true);
                            }
                            break;
                        case "fsub_s":
                            {
                                var inst = new Instruct { Operation = (int)Inst.fsub_s };
                                _3Operators(module, assembleResult, context, inst, ref _IC, 1, true, 1, true, 1, true);
                            }
                            break;
                        case "fsubi_s":
                            {
                                var inst = new Instruct { Operation = (int)Inst.fsubi_s };
                                _3Operators(module, assembleResult, context, inst, ref _IC, 1, true, 2, false, 1, true);
                            }
                            break;
                        case "fmul_s":
                            {
                                var inst = new Instruct { Operation = (int)Inst.fmul_s };
                                _3Operators(module, assembleResult, context, inst, ref _IC, 1, true, 1, true, 1, true);
                            }
                            break;
                        case "fmuli_s":
                            {
                                var inst = new Instruct { Operation = (int)Inst.fmuli_s };
                                _3Operators(module, assembleResult, context, inst, ref _IC, 1, true, 2, false, 1, true);
                            }
                            break;
                        case "fdiv_s":
                            {
                                var inst = new Instruct { Operation = (int)Inst.fdiv_s };
                                _3Operators(module, assembleResult, context, inst, ref _IC, 1, true, 1, true, 1, true);
                            }
                            break;
                        case "fdivi_s":
                            {
                                var inst = new Instruct { Operation = (int)Inst.fdivi_s };
                                _3Operators(module, assembleResult, context, inst, ref _IC, 1, true, 2, false, 1, true);
                            }
                            break;
                        default:

                            break;
                    }
                }
                else
                {
                    if (context.MatachNext(":", true) == MatchResult.Match)
                    {
                        //Label
                        _IC++;
                        Labels.Add(context.Last.Prev.content, _IC);
                    }
                    else
                    {
                        bool willIgnore = false;
                        if (context.Current != null)
                            switch (context.Current.content)
                            {
                                case "":
                                case ";":
                                    {
                                        willIgnore = true;
                                    }
                                    break;
                                default:
                                    break;
                            }
                        if (!willIgnore)
                            assembleResult.AddError(new UnknownInstructionError(context.Current));
                        context.GoNext();
                    }
                }
            }

            return _IC;
        }

        private static void _3Operators(xCVMModule module,
                                        AssembleResult assembleResult,
                                        SegmentContext context,
                                        Instruct inst,
                                        ref int IC,
                                        int Reg0Data = 0,
                                        bool AcceptReg0 = false,
                                        int Reg1Data = 0,
                                        bool AcceptReg1 = false,
                                        int Reg2Data = 0,
                                        bool AcceptReg2 = false)
        {
            if (NextData(assembleResult, context, AcceptReg0, Reg0Data, out inst.Op0))
            {
                if (NextData(assembleResult, context, AcceptReg1, Reg1Data, out inst.Op1))
                {
                    if (NextData(assembleResult, context, AcceptReg2, Reg2Data, out inst.Op2))
                    {
                        var __re = context.MatachNext(";");
                        if (__re == MatchResult.Match)
                        {
                            module.Instructions.Add(inst);
                            IC++;
                        }
                        else
                        {
                            assembleResult.AddError(new MustEndWithSemicolonError(context.Last));
                        }
                    }
                    else
                    {
                    }
                }
            }
        }

        private static bool NextData(AssembleResult assembleResult, SegmentContext context, bool AcceptRegister, int dataType, out byte[] reg)
        {
            switch (dataType)
            {
                case 1:
                    {
                        if (NextInt(assembleResult, context, AcceptRegister, out var reg0))
                        {
                            reg = BitConverter.GetBytes(reg0);
                            return true;
                        }
                    }
                    break;
                case 2:
                    {
                        if (NextLong(assembleResult, context, AcceptRegister, out var reg0))
                        {
                            reg = BitConverter.GetBytes(reg0);
                            return true;
                        }
                    }
                    break;
                case 3:
                    {
                        if (NextFloat(assembleResult, context, out var reg0))
                        {
                            reg = BitConverter.GetBytes(reg0);
                            return true;
                        }
                    }
                    break;
                case 4:
                    {
                        if (NextDouble(assembleResult, context, out var reg0))
                        {
                            reg = BitConverter.GetBytes(reg0);
                            return true;
                        }
                    }
                    break;
                default:
                    break;
            }
            reg = new byte[0];
            return false;
        }
        private static bool NextInt(AssembleResult assembleResult, SegmentContext context, bool AcceptRegister, out int reg0)
        {
            if (context.GoNext())
            {
                var _int = context.Current!.content;
                if (AcceptRegister)
                {
                    if (_int.StartsWith("$"))
                    {
                        _int = _int.Substring(1);
                    }
                    else
                    {
                        assembleResult.AddError(new RegisterFormatError(context.Last));
                    }
                }
                if (int.TryParse(_int, out var reg))
                {
                    reg0 = reg;
                    return true;
                }
                else
                {
                    assembleResult.AddError(new IntParseError(context.Last));
                }
            }
            else
            {
                assembleResult.AddError(new UnexpectedEndOfFileError(context.Last));
            }

            reg0 = -1;
            return false;
        }
        private static bool NextLong(AssembleResult assembleResult, SegmentContext context, bool AcceptRegister, out long reg0)
        {
            if (context.GoNext())
            {
                var _int = context.Current!.content;
                if (AcceptRegister)
                {
                    if (_int.StartsWith("$"))
                    {
                        _int = _int.Substring(1);
                    }
                    else
                    {
                        assembleResult.AddError(new RegisterFormatError(context.Last));
                    }
                }
                if (long.TryParse(_int, out var reg))
                {
                    reg0 = reg;
                    return true;
                }
                else
                {
                    assembleResult.AddError(new LongParseError(context.Last));
                }
            }
            else
            {
                assembleResult.AddError(new UnexpectedEndOfFileError(context.Last));
            }

            reg0 = -1;
            return false;
        }
        private static bool NextFloat(AssembleResult assembleResult, SegmentContext context, out float reg0)
        {
            if (context.GoNext())
            {
                var _int = context.Current!.content;
                if (float.TryParse(_int, out var reg))
                {
                    reg0 = reg;
                    return true;
                }
                else
                {
                    assembleResult.AddError(new FloatParseError(context.Last));
                }
            }
            else
            {
                assembleResult.AddError(new UnexpectedEndOfFileError(context.Last));
            }

            reg0 = -1;
            return false;
        }
        private static bool NextDouble(AssembleResult assembleResult, SegmentContext context, out double reg0)
        {
            if (context.GoNext())
            {
                var _int = context.Current!.content;
                if (double.TryParse(_int, out var reg))
                {
                    reg0 = reg;
                    return true;
                }
                else
                {
                    assembleResult.AddError(new DoubleParseError(context.Last));
                }
            }
            else
            {
                assembleResult.AddError(new UnexpectedEndOfFileError(context.Last));
            }

            reg0 = -1;
            return false;
        }

        public AssembleResult Assemble(string content)
        {
            var segments = parser.Parse(content, false);
            return Assemble(segments);
        }
    }
    [Serializable]
    public class AssemblerDefinition
    {
        public List<Instruction3OperatorsDefinition> Definitions = new List<Instruction3OperatorsDefinition>();
    }
    [Serializable]
    public class Instruction3OperatorsDefinition : IContentable
    {
        public string Name;
        public int ID;
        public int OP0DT;
        public bool OP0REG;
        public int OP1DT;
        public bool OP1REG;
        public int OP2DT;
        public bool OP2REG;
        public string Content => Name;
    }
}
