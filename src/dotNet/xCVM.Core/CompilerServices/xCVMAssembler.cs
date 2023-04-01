﻿using LibCLCC.NET.TextProcessing;
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
        public AssembleResult Assemble(FileInfo file)
        {
            using var sr = file.OpenText();
            var content = sr.ReadToEnd();
            return Assemble(content);
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
            while (true)
            {

                if (context.ReachEnd)
                    break;
                {
                    (var mr, var section) = context.MatchCollectionMarchWithMatchNext(":", ".module", ".text", ".ids", ".codes");
                    if (mr == MatchResult.Match)
                    {
                        Sections = section;
                    }
                    else if (mr == MatchResult.Mismatch)
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
                                                    module.ModuleMetadata.ModuleName = context.Current.content;
                                                    context.GoNext();
                                                }
                                                break;
                                            case "Author":
                                                {
                                                    module.ModuleMetadata.Author = context.Current.content;
                                                    context.GoNext();
                                                }
                                                break;
                                            case "Copyright":
                                                {
                                                    module.ModuleMetadata.Copyright = context.Current.content;
                                                    context.GoNext();
                                                }
                                                break;
                                            case "ModuleVersion":
                                                {
                                                    if (Version.TryParse(context.Current.content, out var result))
                                                    {
                                                        module.ModuleMetadata.ModuleVersion = result;
                                                    }
                                                    else assembleResult.AddError(new VersionFormatError(context.Current));
                                                    context.GoNext();
                                                }
                                                break;
                                            case "TargetVersion":
                                                {
                                                    if (Version.TryParse(context.Current.content, out var result))
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
                                    if (int.TryParse(context.Current.content, out var result))
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
                                    if (int.TryParse(context.Current.content, out var result))
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
                                    var matched = context.MatchCollectionMarchReturnName(
                                        "add", "addi", "sub", "subi", "mul", "muli", "div", "divi"
                                        );
                                    if (matched.Item1 == MatchResult.Match)
                                    {
                                        switch (matched.Item2)
                                        {
                                            case "add":
                                                {
                                                    var inst = new Instruct { Operation = (int)Inst.add };
                                                    _3Operators(module, assembleResult, context, inst, 1, true, 1, true, 1, true);
                                                }
                                                break;
                                            case "addi":
                                                {
                                                    var inst = new Instruct { Operation = (int)Inst.addi };
                                                    _3Operators(module, assembleResult, context, inst, 1, true, 1, false, 1, true);
                                                }
                                                break;
                                            case "sub":
                                                {
                                                    var inst = new Instruct { Operation = (int)Inst.sub };
                                                    _3Operators(module, assembleResult, context, inst, 1, true, 1, true, 1, true);
                                                }
                                                break;
                                            case "subi":
                                                {
                                                    var inst = new Instruct { Operation = (int)Inst.subi };
                                                    _3Operators(module, assembleResult, context, inst, 1, true, 1, false, 1, true);
                                                }
                                                break;
                                            case "mul":
                                                {
                                                    var inst = new Instruct { Operation = (int)Inst.mul };
                                                    _3Operators(module, assembleResult, context, inst, 1, true, 1, true, 1, true);
                                                }
                                                break;
                                            case "muli":
                                                {
                                                    var inst = new Instruct { Operation = (int)Inst.muli };
                                                    _3Operators(module, assembleResult, context, inst, 1, true, 1, false, 1, true);
                                                }
                                                break;
                                            case "div":
                                                {
                                                    var inst = new Instruct { Operation = (int)Inst.div };
                                                    _3Operators(module, assembleResult, context, inst, 1, true, 1, true, 1, true);
                                                }
                                                break;
                                            case "divi":
                                                {
                                                    var inst = new Instruct { Operation = (int)Inst.divi };
                                                    _3Operators(module, assembleResult, context, inst, 1, true, 1, false, 1, true);
                                                }
                                                break;
                                            case "ladd":
                                                {
                                                    var inst = new Instruct { Operation = (int)Inst.ladd };
                                                    _3Operators(module, assembleResult, context, inst, 1, true, 1, true, 1, true);
                                                }
                                                break;
                                            case "laddi":
                                                {
                                                    var inst = new Instruct { Operation = (int)Inst.laddi };
                                                    _3Operators(module, assembleResult, context, inst, 1, true, 2, false, 1, true);
                                                }
                                                break;
                                            default:
                                                break;
                                        }
                                    }
                                }
                                break;
                            default:
                                break;
                        }
                        context.GoNext();
                    }
                    else if (mr == MatchResult.ReachEnd)
                    {

                    }
                }

            }
            return assembleResult;

        }

        private static void _3Operators(xCVMModule module, AssembleResult assembleResult, SegmentContext context, Instruct inst, int Reg0Data = 0,
            bool AcceptReg0 = false, int Reg1Data = 0, bool AcceptReg1 = false, int Reg2Data = 0, bool AcceptReg2 = false)
        {
            if (NextData(assembleResult, context, AcceptReg0, Reg0Data, out inst.Op0))
            {
                if (NextData(assembleResult, context, AcceptReg2, Reg1Data, out inst.Op1))
                {
                    if (NextData(assembleResult, context, AcceptReg1, Reg2Data, out inst.Op2))
                    {
                        inst.Op3 = null;
                        module.Instructions.Add(inst);
                    }
                }
            }
        }

        private static bool NextData(AssembleResult assembleResult, SegmentContext context, bool AcceptRegister, int dataType, out byte[] reg)
        {
            switch (dataType)
            {
                case 0:
                    {
                        if (NextInt(assembleResult, context, AcceptRegister, out var reg0))
                        {
                            reg = BitConverter.GetBytes(reg0);
                            return true;
                        }
                    }
                    break;
                case 1:
                    {
                        if (NextLong(assembleResult, context, AcceptRegister, out var reg0))
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
                if (int.TryParse(context.Current!.content, out var reg))
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
                if (long.TryParse(context.Current!.content, out var reg))
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

        public AssembleResult Assemble(string content)
        {
            var segments = parser.Parse(content, false);
            return Assemble(segments);
        }
    }
}
