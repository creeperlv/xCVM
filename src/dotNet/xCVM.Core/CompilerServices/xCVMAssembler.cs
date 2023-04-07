using LibCLCC.NET.TextProcessing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace xCVM.Core.CompilerServices
{
    public class xCVMAssembler
    {
        public AssemblerDefinition? AssemblerDefinition;
        public xCVMAssembler(AssemblerDefinition? definition = null)
        {
            AssemblerDefinition = definition;
            if (definition != null)
            {
                WillUseEndMark = definition.UseStatementEndMark;
                EndMark = definition.StateMentEndMark;
                AcceptIDAlias = definition.AcceptIDAlias;
            }
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
        void Preprocess(Segment HEAD)
        {
            HEAD.content = Regex.Unescape(HEAD.content);
            if (HEAD.Next != null) Preprocess(HEAD.Next);
        }
        public AssembleResult Assemble(Segment segments)
        {
            Preprocess(segments);
            xCVMModule module = new xCVMModule();
            AssembleResult assembleResult = new AssembleResult(module);
            var current = segments;
            SegmentContext context = new SegmentContext(current);
            int Sections = 0;
            Dictionary<string, int> Labels = new Dictionary<string, int>();
            Dictionary<string, int> Texts = new Dictionary<string, int>();
            Dictionary<string, int> IDs = new Dictionary<string, int>();
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
                        _IC = BaseAssemble(module, assembleResult, context, Sections, Labels, Texts, IDs, _IC);

                    }
                    else if (mr == MatchResult.ReachEnd)
                    {

                    }
                }

            }
            foreach (IntermediateInstruct inst in module.Instructions)
            {
                PostProcess(inst);
            }
            void PostProcess(IntermediateInstruct instruct)
            {

                if (instruct is IntermediateInstruct ii)
                {
                    if (ii.Definition != null)
                    {
                        if (ii.PseudoOp0 != null)
                        {
                            if (NextData(assembleResult, new SegmentContext(ii.PseudoOp0.Prev), ii.Definition.OP0REG, ii.Definition.OP0DT, Labels, Texts, IDs, false, out ii.Op0))
                            {

                            }
                            else
                            {
                                assembleResult.AddError(new UnknownParameterError(ii.PseudoOp0));
                                //Fail.
                            }
                        }
                        if (ii.PseudoOp1 != null)
                        {
                            if (NextData(assembleResult, new SegmentContext(ii.PseudoOp1.Prev), ii.Definition.OP1REG, ii.Definition.OP1DT, Labels, Texts, IDs, false, out ii.Op1))
                            {

                            }
                            else
                            {
                                assembleResult.AddError(new UnknownParameterError(ii.PseudoOp1));
                                //Fail.
                            }
                        }
                        if (ii.PseudoOp2 != null)
                        {
                            if (NextData(assembleResult, new SegmentContext(ii.PseudoOp2.Prev), ii.Definition.OP2REG, ii.Definition.OP2DT, Labels, Texts, IDs, false, out ii.Op2))
                            {

                            }
                            else
                            {
                                assembleResult.AddError(new UnknownParameterError(ii.PseudoOp2));
                                //Fail.
                            }
                        }
                    }

                }
            }
            return assembleResult;

        }
        bool AcceptIDAlias = true;
        private int BaseAssemble(xCVMModule module,
                                 AssembleResult assembleResult,
                                 SegmentContext context,
                                 int Sections,
                                 Dictionary<string, int> Labels,
                                 Dictionary<string, int> Texts,
                                 Dictionary<string, int> IDs,
                                 int _IC)
        {
            switch (Sections)
            {
                case 0:
                    {
                        (var _mr, var _selection) = context.MatchCollectionMarchReturnName("ModuleName", "Author", "Copyright", "Description", "ModuleVersion", "TargetVersion");
                        if (_mr == MatchResult.Match)
                        {
                            switch (_selection)
                            {
                                case "ModuleName":
                                    {
                                        module.ModuleMetadata.ModuleName = context.Current!.content;
                                        if (WillUseEndMark)
                                        {
                                            if (context.MatachNext(EndMark) != MatchResult.Match)
                                            {
                                                assembleResult.AddError(new MustEndWithSpecifiedEndMarkError(context.Current, EndMark));
                                            }
                                            else
                                            {

                                            }
                                        }
                                    }
                                    break;
                                case "Author":
                                    {
                                        module.ModuleMetadata.Author = context.Current!.content;
                                        if (WillUseEndMark)
                                        {
                                            if (context.MatachNext(EndMark) != MatchResult.Match)
                                            {
                                                assembleResult.AddError(new MustEndWithSpecifiedEndMarkError(context.Current, EndMark));
                                            }
                                            else
                                            {

                                            }
                                        }
                                    }
                                    break;
                                case "Copyright":
                                    {
                                        module.ModuleMetadata.Copyright = context.Current!.content;
                                        if (WillUseEndMark)
                                        {
                                            if (context.MatachNext(EndMark) != MatchResult.Match)
                                            {
                                                assembleResult.AddError(new MustEndWithSpecifiedEndMarkError(context.Current, EndMark));
                                            }
                                            else
                                            {

                                            }
                                        }
                                    }
                                    break;
                                case "Description":
                                    {
                                        module.ModuleMetadata.Description = context.Current!.content;
                                        if (WillUseEndMark)
                                        {
                                            if (context.MatachNext(EndMark) != MatchResult.Match)
                                            {
                                                assembleResult.AddError(new MustEndWithSpecifiedEndMarkError(context.Current, EndMark));
                                            }
                                            else
                                            {
                                            }
                                        }
                                    }
                                    break;
                                case "ModuleVersion":
                                    {
                                        if (Version.TryParse(context.Current!.content, out var result))
                                        {
                                            module.ModuleMetadata.ModuleVersion = result;
                                        }
                                        else assembleResult.AddError(new VersionFormatError(context.Current));
                                        if (WillUseEndMark)
                                        {
                                            if (context.MatachNext(EndMark) != MatchResult.Match)
                                            {
                                                assembleResult.AddError(new MustEndWithSpecifiedEndMarkError(context.Current, EndMark));
                                            }
                                            else
                                            {
                                            }
                                        }
                                    }
                                    break;
                                case "TargetVersion":
                                    {
                                        if (Version.TryParse(context.Current!.content, out var result))
                                        {
                                            module.ModuleMetadata.TargetVersion = result;
                                        }
                                        else assembleResult.AddError(new VersionFormatError(context.Current));
                                        if (WillUseEndMark)
                                        {
                                            if (context.MatachNext(EndMark) != MatchResult.Match)
                                            {
                                                assembleResult.AddError(new MustEndWithSpecifiedEndMarkError(context.Current, EndMark));
                                            }
                                            else
                                            {
                                            }
                                        }
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
                                if (WillUseEndMark)
                                {
                                    if (context.MatachNext(EndMark) != MatchResult.Match)
                                    {
                                        assembleResult.AddError(new MustEndWithSpecifiedEndMarkError(context.Current, EndMark));
                                    }
                                    else
                                    {
                                    }
                                }
                            }
                            else
                            {
                                assembleResult.AddError(new UnexpectedEndOfFileError(context.Last));
                                break;
                            }
                        }
                        else
                        {

                            if (AcceptIDAlias)
                            {
                                int ID = Texts.Count;
                                if (!Texts.TryAdd(context.Current!.content, ID))
                                {
                                    Texts[context.Current!.content] = ID;
                                }
                                if (context.GoNext())
                                {
                                    module.Texts.Add(ID, context.Current.content);
                                    if (WillUseEndMark)
                                    {
                                        if (context.MatachNext(EndMark) != MatchResult.Match)
                                        {
                                            assembleResult.AddError(new MustEndWithSpecifiedEndMarkError(context.Current, EndMark));
                                        }
                                        else
                                        {
                                        }
                                    }
                                }
                                else
                                {
                                    assembleResult.AddError(new UnexpectedEndOfFileError(context.Last));
                                    break;
                                }

                            }
                            else
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
                                if (WillUseEndMark)
                                {
                                    if (context.MatachNext(EndMark) != MatchResult.Match)
                                    {
                                        assembleResult.AddError(new MustEndWithSpecifiedEndMarkError(context.Current, EndMark));
                                    }
                                    else
                                    {
                                    }
                                }
                            }
                            else
                            {
                                assembleResult.AddError(new UnexpectedEndOfFileError(context.Last));
                                break;
                            }
                        }
                        else
                        {

                            if (AcceptIDAlias)
                            {
                                int ID = IDs.Count;
                                if (!IDs.TryAdd(context.Current!.content, ID))
                                {
                                    IDs[context.Current!.content] = ID;
                                }
                                if (context.GoNext())
                                {
                                    module.IDs.Add(ID, context.Current.content);
                                    if (WillUseEndMark)
                                    {
                                        if (context.MatachNext(EndMark) != MatchResult.Match)
                                        {
                                            assembleResult.AddError(new MustEndWithSpecifiedEndMarkError(context.Current, EndMark));
                                        }
                                        else
                                        {
                                        }
                                    }
                                }
                                else
                                {
                                    assembleResult.AddError(new UnexpectedEndOfFileError(context.Last));
                                    break;
                                }

                            }
                            else
                                assembleResult.AddError(new IntParseError(context.Current));
                            break;
                        }
                    }
                    break;
                case 3:
                    {

                        if (AssemblerDefinition == null)
                        {
                            _IC = EmbeddedAssemble(module, assembleResult, context, Labels, Texts, IDs, _IC);
                        }
                        else
                        {
                            _IC = eXtensibleAssemble(module, assembleResult, context, Labels, Texts, IDs, _IC);
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
                                       Dictionary<string, int> Texts,
                                       Dictionary<string, int> IDs,
                                       int _IC)
        {
            var a = context.MatachCollectionMarchReturnContentable(AssemblerDefinition!.Definitions);
            if (a.Item1 == MatchResult.Match)
            {
                if (a.Item2 is Instruction3OperatorsDefinition def)
                {
                    IntermediateInstruct instruct = new IntermediateInstruct();
                    instruct.Operation = def.ID;
                    instruct.Definition = def;
                    _3Operators(module,
                                assembleResult,
                                context,
                                instruct,
                                Labels,
                                Texts,
                                IDs,
                                ref _IC,
                                def.OP0DT,
                                def.OP0REG,
                                def.OP1DT,
                                def.OP1REG,
                                def.OP2DT,
                                def.OP2REG,
                                true);
                }
            }
            else if (a.Item1 == MatchResult.Mismatch)
            {
                var r = context.MatachNext(":", true);
                if (r == MatchResult.Match)
                {
                    string name = context.Current.Prev.content;
                    Labels.Add(name, _IC + 1);
                }
                else if (r == MatchResult.Mismatch)
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

        private int EmbeddedAssemble(xCVMModule module,
                                     AssembleResult assembleResult,
                                     SegmentContext context,
                                     Dictionary<string, int> Labels,
                                     Dictionary<string, int> Texts,
                                     Dictionary<string, int> IDs,
                                     int _IC)
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
                                _3Operators(module,
                                            assembleResult,
                                            context,
                                            inst,
                                            Labels,
                                            Texts,
                                            IDs,
                                            ref _IC,
                                            1,
                                            true,
                                            1,
                                            true,
                                            1,
                                            true);
                            }
                            break;
                        case "addi":
                            {
                                var inst = new Instruct { Operation = (int)Inst.addi };
                                _3Operators(module, assembleResult, context, inst,
                                Labels,
                                Texts,
                                IDs, ref _IC, 1, true, 1, false, 1, true);
                            }
                            break;
                        case "sub":
                            {
                                var inst = new Instruct { Operation = (int)Inst.sub };
                                _3Operators(module, assembleResult, context, inst, Labels, Texts, IDs, ref _IC, 1, true, 1, true, 1, true);
                            }
                            break;
                        case "subi":
                            {
                                var inst = new Instruct { Operation = (int)Inst.subi };
                                _3Operators(module, assembleResult, context, inst, Labels, Texts, IDs, ref _IC, 1, true, 1, false, 1, true);
                            }
                            break;
                        case "mul":
                            {
                                var inst = new Instruct { Operation = (int)Inst.mul };
                                _3Operators(module, assembleResult, context, inst, Labels, Texts, IDs, ref _IC, 1, true, 1, true, 1, true);
                            }
                            break;
                        case "muli":
                            {
                                var inst = new Instruct { Operation = (int)Inst.muli };
                                _3Operators(module, assembleResult, context, inst, Labels, Texts, IDs, ref _IC, 1, true, 1, false, 1, true);
                            }
                            break;
                        case "div":
                            {
                                var inst = new Instruct { Operation = (int)Inst.div };
                                _3Operators(module, assembleResult, context, inst, Labels, Texts, IDs, ref _IC, 1, true, 1, true, 1, true);
                            }
                            break;
                        case "divi":
                            {
                                var inst = new Instruct { Operation = (int)Inst.divi };
                                _3Operators(module, assembleResult, context, inst, Labels, Texts, IDs, ref _IC, 1, true, 1, false, 1, true);
                            }
                            break;
                        case "ladd":
                            {
                                var inst = new Instruct { Operation = (int)Inst.ladd };
                                _3Operators(module, assembleResult, context, inst, Labels, Texts, IDs, ref _IC, 1, true, 1, true, 1, true);
                            }
                            break;
                        case "laddi":
                            {
                                var inst = new Instruct { Operation = (int)Inst.laddi };
                                _3Operators(module, assembleResult, context, inst, Labels, Texts, IDs, ref _IC, 1, true, 2, false, 1, true);
                            }
                            break;
                        case "lsub":
                            {
                                var inst = new Instruct { Operation = (int)Inst.lsub };
                                _3Operators(module, assembleResult, context, inst, Labels, Texts, IDs, ref _IC, 1, true, 1, true, 1, true);
                            }
                            break;
                        case "lsubi":
                            {
                                var inst = new Instruct { Operation = (int)Inst.lsubi };
                                _3Operators(module, assembleResult, context, inst, Labels, Texts, IDs, ref _IC, 1, true, 2, false, 1, true);
                            }
                            break;
                        case "lmul":
                            {
                                var inst = new Instruct { Operation = (int)Inst.lmul };
                                _3Operators(module, assembleResult, context, inst, Labels, Texts, IDs, ref _IC, 1, true, 1, true, 1, true);
                            }
                            break;
                        case "lmuli":
                            {
                                var inst = new Instruct { Operation = (int)Inst.lmuli };
                                _3Operators(module, assembleResult, context, inst, Labels, Texts, IDs, ref _IC, 1, true, 2, false, 1, true);
                            }
                            break;
                        case "ldiv":
                            {
                                var inst = new Instruct { Operation = (int)Inst.ldiv };
                                _3Operators(module, assembleResult, context, inst, Labels, Texts, IDs, ref _IC, 1, true, 1, true, 1, true);
                            }
                            break;
                        case "ldivi":
                            {
                                var inst = new Instruct { Operation = (int)Inst.ldivi };
                                _3Operators(module, assembleResult, context, inst, Labels, Texts, IDs, ref _IC, 1, true, 2, false, 1, true);
                            }
                            break;
                        case "fadd_s":
                            {
                                var inst = new Instruct { Operation = (int)Inst.fadd_s };
                                _3Operators(module, assembleResult, context, inst, Labels, Texts, IDs, ref _IC, 1, true, 1, true, 1, true);
                            }
                            break;
                        case "faddi_s":
                            {
                                var inst = new Instruct { Operation = (int)Inst.faddi_s };
                                _3Operators(module, assembleResult, context, inst, Labels, Texts, IDs, ref _IC, 1, true, 2, false, 1, true);
                            }
                            break;
                        case "fsub_s":
                            {
                                var inst = new Instruct { Operation = (int)Inst.fsub_s };
                                _3Operators(module, assembleResult, context, inst, Labels, Texts, IDs, ref _IC, 1, true, 1, true, 1, true);
                            }
                            break;
                        case "fsubi_s":
                            {
                                var inst = new Instruct { Operation = (int)Inst.fsubi_s };
                                _3Operators(module, assembleResult, context, inst, Labels, Texts, IDs, ref _IC, 1, true, 2, false, 1, true);
                            }
                            break;
                        case "fmul_s":
                            {
                                var inst = new Instruct { Operation = (int)Inst.fmul_s };
                                _3Operators(module, assembleResult, context, inst, Labels, Texts, IDs, ref _IC, 1, true, 1, true, 1, true);
                            }
                            break;
                        case "fmuli_s":
                            {
                                var inst = new Instruct { Operation = (int)Inst.fmuli_s };
                                _3Operators(module, assembleResult, context, inst, Labels, Texts, IDs, ref _IC, 1, true, 2, false, 1, true);
                            }
                            break;
                        case "fdiv_s":
                            {
                                var inst = new Instruct { Operation = (int)Inst.fdiv_s };
                                _3Operators(module, assembleResult, context, inst, Labels, Texts, IDs, ref _IC, 1, true, 1, true, 1, true);
                            }
                            break;
                        case "fdivi_s":
                            {
                                var inst = new Instruct { Operation = (int)Inst.fdivi_s };
                                _3Operators(module, assembleResult, context, inst, Labels, Texts, IDs, ref _IC, 1, true, 2, false, 1, true);
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
                        Labels.Add(context.Last.Prev.content, _IC + 1);
                    }
                    else
                    {
                        bool willIgnore = false;
                        if (context.Current != null)
                            switch (context.Current.content)
                            {
                                case "":
                                    {
                                        willIgnore = true;
                                    }
                                    break;
                                default:
                                    if (context.Current.content == EndMark)
                                    {
                                        willIgnore = true;
                                    }
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

        bool WillUseEndMark = true;
        string EndMark = ";";
        private void _3Operators(xCVMModule module,
                                 AssembleResult assembleResult,
                                 SegmentContext context,
                                 Instruct inst,
                                 Dictionary<string, int> Labels,
                                 Dictionary<string, int> Texts,
                                 Dictionary<string, int> IDs,
                                 ref int IC,
                                 int Reg0Data = 0,
                                 bool AcceptReg0 = false,
                                 int Reg1Data = 0,
                                 bool AcceptReg1 = false,
                                 int Reg2Data = 0,
                                 bool AcceptReg2 = false,
                                 bool SupressError = false)
        {
            if (NextData(assembleResult, context, AcceptReg0, Reg0Data, Labels, Texts, IDs, SupressError, out inst.Op0))
            {
            }
            else if (SupressError)
            {
                if (inst is IntermediateInstruct ii)
                {
                    ii.PseudoOp0 = context.Current;
                }
            }
            else
            {
                return;
            }

            if (NextData(assembleResult, context, AcceptReg1, Reg1Data, Labels, Texts, IDs, SupressError, out inst.Op1))
            {
            }
            else if (SupressError)
            {
                if (inst is IntermediateInstruct ii)
                {
                    ii.PseudoOp1 = context.Current;
                }
            }
            else
            {
                return;
            }

            if (NextData(assembleResult, context, AcceptReg2, Reg2Data, Labels, Texts, IDs, SupressError, out inst.Op2))
            {
            }
            else if (SupressError)
            {
                if (inst is IntermediateInstruct ii)
                {
                    ii.PseudoOp2 = context.Current;
                }
            }
            else
            {
            }
            if (WillUseEndMark)
            {
                var __re = context.MatachNext(EndMark);
                if (__re == MatchResult.Match)
                {
                    module.Instructions.Add(inst);
                    IC++;
                    //context.GoNext();
                }
                else
                {
                    assembleResult.AddError(new MustEndWithSemicolonError(context.Last));
                }
            }
            else
            {
                module.Instructions.Add(inst);
                IC++;
            }
        }
        private bool NextData(AssembleResult assembleResult,
                              SegmentContext context,
                              bool AcceptRegister,
                              int dataType,
                              Dictionary<string, int> Labels,
                              Dictionary<string, int> Texts,
                              Dictionary<string, int> IDs,
                              bool SupressError,
                              out byte[] reg)
        {
            switch (dataType)
            {
                case 1:
                    {
                        if (NextInt(assembleResult, context, AcceptRegister, Labels, Texts, IDs, SupressError, out var reg0))
                        {
                            reg = BitConverter.GetBytes(reg0);
                            return true;
                        }
                    }
                    break;
                case 2:
                    {
                        if (NextLong(assembleResult, context, AcceptRegister, SupressError, out var reg0))
                        {
                            reg = BitConverter.GetBytes(reg0);
                            return true;
                        }
                    }
                    break;
                case 3:
                    {
                        if (NextFloat(assembleResult, context, SupressError, out var reg0))
                        {
                            reg = BitConverter.GetBytes(reg0);
                            return true;
                        }
                    }
                    break;
                case 4:
                    {
                        if (NextDouble(assembleResult, context, SupressError, out var reg0))
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
        private bool NextInt(AssembleResult assembleResult,
                             SegmentContext context,
                             bool AcceptRegister,
                              Dictionary<string, int> Labels,
                              Dictionary<string, int> Texts,
                              Dictionary<string, int> IDs,
                              bool SupressError,
                             out int reg0)
        {
            if (context.GoNext())
            {
                var _int = context.Current!.content;
                if (_int.StartsWith("text?"))
                {
                    _int = _int.Substring(5);
                    if (Texts.ContainsKey(_int))
                    {
                        reg0 = Texts[_int];
                        return true;
                    }
                    else
                    {
                        reg0 = -1;
                        return false;
                    }
                }
                else
                if (_int.StartsWith("id?"))
                {
                    _int = _int.Substring(3);
                    if (IDs.ContainsKey(_int))
                    {
                        reg0 = IDs[_int];
                        return true;
                    }
                    else
                    {
                        reg0 = -1;
                        return false;
                    }
                }
                else
                if (_int.StartsWith("lbl?"))
                {
                    _int = _int.Substring(4);
                    if (Labels.ContainsKey(_int))
                    {
                        reg0 = Labels[_int];
                        return true;
                    }
                    else
                    {
                        reg0 = -1;
                        return false;
                    }
                }
                if (AcceptRegister)
                {
                    if (_int.StartsWith("$"))
                    {
                        _int = _int.Substring(1);
                    }
                    else
                    {
                        if (!SupressError)
                            assembleResult.AddError(new RegisterFormatError(context.Last));
                    }
                }
                if (int.TryParse(_int, out var data))
                {
                    reg0 = data;
                    return true;
                }
                else
                {
                    if (AssemblerDefinition != null)
                    {
                        if (AssemblerDefinition.PredefinedSymbols.TryGetValue(context.Current!.content, out _int))
                        {
                            if (int.TryParse(_int, out data))
                            {
                                reg0 = data;
                                return true;
                            }
                            else
                            {
                                if (!SupressError)
                                    assembleResult.AddError(new IntParseError(context.Last));
                            }
                        }
                    }
                    else
                        if (!SupressError)
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
        private bool NextLong(AssembleResult assembleResult,
                              SegmentContext context,
                              bool AcceptRegister,
                              bool SupressError,
                              out long reg0)
        {
            if (context.GoNext())
            {
                var _long = context.Current!.content;
                if (AcceptRegister)
                {
                    if (_long.StartsWith("$"))
                    {
                        _long = _long.Substring(1);
                    }
                    else
                    {
                        assembleResult.AddError(new RegisterFormatError(context.Last));
                    }
                }
                if (long.TryParse(_long, out var data))
                {
                    reg0 = data;
                    return true;
                }
                else
                {
                    if (AssemblerDefinition != null)
                    {
                        if (AssemblerDefinition.PredefinedSymbols.TryGetValue(context.Current!.content, out _long))
                        {
                            if (long.TryParse(_long, out data))
                            {
                                reg0 = data;
                                return true;
                            }
                            else
                            {
                                if (!SupressError)
                                    assembleResult.AddError(new IntParseError(context.Last));
                            }
                        }
                    }
                    else
                        if (!SupressError)
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
        private bool NextFloat(AssembleResult assembleResult, SegmentContext context,
                              bool SupressError, out float reg0)
        {
            if (context.GoNext())
            {
                var _float = context.Current!.content;
                if (float.TryParse(_float, out var data))
                {
                    reg0 = data;
                    return true;
                }
                else
                {
                    if (AssemblerDefinition != null)
                    {
                        if (AssemblerDefinition.PredefinedSymbols.TryGetValue(context.Current!.content, out _float))
                        {
                            if (float.TryParse(_float, out data))
                            {
                                reg0 = data;
                                return true;
                            }
                            else
                            {
                                if (!SupressError)
                                    assembleResult.AddError(new IntParseError(context.Last));
                            }
                        }
                    }
                    else
                        if (!SupressError)
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
        private bool NextDouble(AssembleResult assembleResult, SegmentContext context,
                              bool SupressError, out double reg0)
        {
            if (context.GoNext())
            {
                var _double = context.Current!.content;
                if (double.TryParse(_double, out var data))
                {
                    reg0 = data;
                    return true;
                }
                else
                {
                    if (AssemblerDefinition != null)
                    {
                        if (AssemblerDefinition.PredefinedSymbols.TryGetValue(context.Current!.content, out _double))
                        {
                            if (double.TryParse(_double, out data))
                            {
                                reg0 = data;
                                return true;
                            }
                            else
                            {
                                if (!SupressError)
                                    assembleResult.AddError(new IntParseError(context.Last));
                            }
                        }
                    }
                    else
                    {
                        if (!SupressError)
                            assembleResult.AddError(new DoubleParseError(context.Last));
                    }
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
}
