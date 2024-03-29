﻿using LibCLCC.NET.TextProcessing;
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
        bool CaseSensitiveSegmentationIdentifiers = false;
        public xCVMAssembler(AssemblerDefinition? definition = null)
        {
            AssemblerDefinition = definition;
            if (definition != null)
            {
                WillUseEndMark = definition.UseStatementEndMark;
                EndMark = definition.StatementEndMark;
                AcceptIDAlias = definition.AcceptIDAlias;
                CaseSensitiveSegmentationIdentifiers = definition.CaseSensitiveSegmentationIdentifiers;
            }
        }
        public OperationResult<xCVMModule> Assemble(FileInfo file , ResourceDevDef? resourceDef = null)
        {
            using var sr = file.OpenText();
            var content = sr.ReadToEnd();
            return Assemble(parser.Scan(content , false , file.FullName));
        }
        public OperationResult<xCVMModule> Assemble(List<FileInfo> files , ResourceDevDef? resourceDef = null)
        {
            Segment? s = null;
            foreach (var item in files)
            {
                using var sr = item.OpenText();
                var content = sr.ReadToEnd();
                var _s = parser.Scan(content , false , item.FullName);
                if (s is null) s = _s;
                else s.Concatenate(_s);
            }
            if (s is null) return new OperationResult<xCVMModule>(new xCVMModule());
            return Assemble(s);
        }
        public OperationResult<xCVMModule> Assemble(Stream stream , ResourceDevDef? resourceDef = null)
        {
            using (StreamReader sr = new StreamReader(stream))
            {
                return Assemble(sr.ReadToEnd());
            }
        }
        ASMScanner parser = new ASMScanner();
        void Preprocess(Segment HEAD)
        {
            HEAD.content = Regex.Unescape(HEAD.content);
            if (HEAD.Next != null) Preprocess(HEAD.Next);
        }
        ResourceDevDef? resourceDef = null;
        public OperationResult<xCVMModule> Assemble(Segment segments , ResourceDevDef? resourceDef = null)
        {
            this.resourceDef = resourceDef;
            Preprocess(segments);
            xCVMModule module = new xCVMModule();
            OperationResult<xCVMModule> assembleResult = new OperationResult<xCVMModule>(module);
            var current = segments;
            SegmentContext context = new SegmentContext(current);
            int Sections = 0;
            Dictionary<string , int> Labels = new Dictionary<string , int>();
            Dictionary<string , int> Texts = new Dictionary<string , int>();
            Dictionary<string , int> IDs = new Dictionary<string , int>();
            int _IC = 0;
            while (true)
            {

                if (context.ReachEnd)
                    break;
                {

                    (var mr, var section) =
                        context.MatchCollectionMarchWithMatchNext(":" ,
                                                                  CaseSensitiveSegmentationIdentifiers ,
                                                                  ".module" ,
                                                                  ".text" ,
                                                                  ".ids" ,
                                                                  ".codes" ,
                                                                  ".extern_func" ,
                                                                  ".extern_struct" ,
                                                                  ".extern_var");
                    //Console.WriteLine(mr + ":" + section);
                    if (mr == MatchResult.Match)
                    {
                        Sections = section;
                    }
                    else if (mr == MatchResult.Mismatch)
                    {
                        _IC = BaseAssemble(module ,
                                           assembleResult ,
                                           context ,
                                           Sections ,
                                           Labels ,
                                           Texts ,
                                           IDs ,
                                           _IC);

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
            foreach (var _st in module.ExternStructs)
            {
                foreach (var item in _st.Fields.Values)
                {
                    if (item is IntermediateDataType DT)
                    {
                        PostProcessDT(DT);
                    }
                }
            }
            foreach (IntermediateExternFunction _fn in module.ExternFunctions)
            {
                if (_fn.PseudoLabel != null)
                {
                    if (!NextInt(assembleResult , new SegmentContext(_fn.PseudoLabel.Prev) , true , Labels , Texts , IDs ,
                                 false , out _fn.Label))
                    {
                        assembleResult.AddError(new IntParseError(_fn.PseudoLabel));
                    }
                }
                foreach (var item in _fn.Registers.Values)
                {
                    if (item is IntermediateDataType DT)
                    {
                        PostProcessDT(DT);
                    }

                }
                {
                    if (_fn.ReturnType is IntermediateDataType DT)
                    {
                        PostProcessDT(DT);
                    }
                }
            }
            foreach (IntermediateDataType item in module.ExternVariables)
            {
                PostProcessDT(item);
            }
            void PostProcessDT(IntermediateDataType intermediateDataType)
            {
                if (intermediateDataType.PseudoType == null) return;
                string L = intermediateDataType.PseudoType.content;
                if (AssemblerDefinition?.CaseSensitiveInternalTypeIdentifier ?? true == true)
                {
                    L = L.ToUpper();
                }
                string R = AssemblerDefinition?.InternalTypeIdentifier ?? "internal";
                if (AssemblerDefinition?.CaseSensitiveInternalTypeIdentifier ?? true == true)
                {
                    R = R.ToUpper();
                }
                if (L == R)
                {
                    if (AssemblerDefinition != null)
                    {
                        if (intermediateDataType.PseudoAdditionalType != null)
                        {
                            foreach (var item in AssemblerDefinition.PredefinedTypeMapping)
                            {
                                if (AssemblerDefinition?.CaseSensitiveInternalTypeIdentifier ?? true == true)
                                {
                                    if (intermediateDataType.PseudoAdditionalType.content == item.Key)
                                    {
                                        intermediateDataType.Type = item.Value;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (!(AssemblerDefinition?.StructIdentifiers is null))
                        foreach (var item in AssemblerDefinition.StructIdentifiers)
                        {
                            var _item = item;
                            if (AssemblerDefinition?.CaseSensitiveInternalTypeIdentifier ?? true == true)
                            {
                                _item = _item.ToUpper();
                            }
                            if(L== _item)
                            {
                                if (intermediateDataType.PseudoAdditionalType != null)
                                {
                                    int id = 0;
                                    foreach (var st in module.ExternStructs)
                                    {
                                        if (st.Name == intermediateDataType.PseudoAdditionalType.content)
                                        {
                                            intermediateDataType.Type = -2;
                                            intermediateDataType.AdditionalTypeID = id;
                                            break;
                                        }
                                        id++;
                                    }
                                }
                            }
                        }
                }
            }
            void PostProcess(IntermediateInstruct instruct)
            {

                if (instruct is IntermediateInstruct ii)
                {
                    if (ii.Definition != null)
                    {
                        if (ii.PseudoOp0 != null)
                        {
                            if (NextData(assembleResult , new SegmentContext(ii.PseudoOp0.Prev) , ii.Definition.OP0REG ,
                                         ii.Definition.OP0DT , Labels , Texts , IDs , false , out ii.Op0))
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
                            if (NextData(assembleResult ,
                                         new SegmentContext(ii.PseudoOp1.Prev) ,
                                         ii.Definition.OP1REG ,
                                         ii.Definition.OP1DT ,
                                         Labels ,
                                         Texts ,
                                         IDs ,
                                         false ,
                                         out ii.Op1))
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
                            if (NextData(assembleResult ,
                                         new SegmentContext(ii.PseudoOp2.Prev) ,
                                         ii.Definition.OP2REG ,
                                         ii.Definition.OP2DT ,
                                         Labels ,
                                         Texts ,
                                         IDs ,
                                         false ,
                                         out ii.Op2))
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
        private int BaseAssemble(xCVMModule module ,
                                 OperationResult<xCVMModule> assembleResult ,
                                 SegmentContext context ,
                                 int Sections ,
                                 Dictionary<string , int> Labels ,
                                 Dictionary<string , int> Texts ,
                                 Dictionary<string , int> IDs ,
                                 int _IC)
        {
            switch (Sections)
            {
                case 0:
                    {
                        (var _mr, var _selection) = context.MatchCollectionMarchReturnName("ModuleName" , "Author" , "Copyright" , "Description" , "ModuleVersion" , "TargetVersion");
                        if (_mr == MatchResult.Match)
                        {
                            switch (_selection)
                            {
                                case "ModuleName":
                                    {
                                        module.ModuleMetadata.ModuleName = context.Current!.content;
                                        if (WillUseEndMark)
                                        {
                                            if (context.MatchNext(EndMark) != MatchResult.Match)
                                            {
                                                assembleResult.AddError(new MustEndWithSpecifiedEndMarkError(context.Current , EndMark));
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
                                            if (context.MatchNext(EndMark) != MatchResult.Match)
                                            {
                                                assembleResult.AddError(new MustEndWithSpecifiedEndMarkError(context.Current , EndMark));
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
                                            if (context.MatchNext(EndMark) != MatchResult.Match)
                                            {
                                                assembleResult.AddError(new MustEndWithSpecifiedEndMarkError(context.Current , EndMark));
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
                                            if (context.MatchNext(EndMark) != MatchResult.Match)
                                            {
                                                assembleResult.AddError(new MustEndWithSpecifiedEndMarkError(context.Current , EndMark));
                                            }
                                            else
                                            {
                                            }
                                        }
                                    }
                                    break;
                                case "ModuleVersion":
                                    {
                                        if (Version.TryParse(context.Current!.content , out var result))
                                        {
                                            module.ModuleMetadata.ModuleVersion = result;
                                        }
                                        else assembleResult.AddError(new VersionFormatError(context.Current));
                                        if (WillUseEndMark)
                                        {
                                            if (context.MatchNext(EndMark) != MatchResult.Match)
                                            {
                                                assembleResult.AddError(new MustEndWithSpecifiedEndMarkError(context.Current , EndMark));
                                            }
                                            else
                                            {
                                            }
                                        }
                                    }
                                    break;
                                case "TargetVersion":
                                    {
                                        if (Version.TryParse(context.Current!.content , out var result))
                                        {
                                            module.ModuleMetadata.TargetVersion = result;
                                        }
                                        else assembleResult.AddError(new VersionFormatError(context.Current));
                                        if (WillUseEndMark)
                                        {
                                            if (context.MatchNext(EndMark) != MatchResult.Match)
                                            {
                                                assembleResult.AddError(new MustEndWithSpecifiedEndMarkError(context.Current , EndMark));
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
                        if (int.TryParse(context.Current!.content , out var result))
                        {
                            if (context.GoNext())
                            {
                                module.Texts.Add(result , context.Current.content);
                                if (WillUseEndMark)
                                {
                                    if (context.MatchNext(EndMark) != MatchResult.Match)
                                    {
                                        assembleResult.AddError(new MustEndWithSpecifiedEndMarkError(context.Current , EndMark));
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
                                if (!Texts.TryAdd(context.Current!.content , ID))
                                {
                                    Texts [ context.Current!.content ] = ID;
                                }
                                if (context.GoNext())
                                {
                                    module.Texts.Add(ID , context.Current.content);
                                    if (WillUseEndMark)
                                    {
                                        if (context.MatchNext(EndMark) != MatchResult.Match)
                                        {
                                            assembleResult.AddError(new MustEndWithSpecifiedEndMarkError(context.Current , EndMark));
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
                        if (int.TryParse(context.Current!.content , out var result))
                        {
                            if (context.GoNext())
                            {
                                module.IDs.Add(result , context.Current.content);
                                if (WillUseEndMark)
                                {
                                    if (context.MatchNext(EndMark) != MatchResult.Match)
                                    {
                                        assembleResult.AddError(new MustEndWithSpecifiedEndMarkError(context.Current , EndMark));
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
                                if (!IDs.TryAdd(context.Current!.content , ID))
                                {
                                    IDs [ context.Current!.content ] = ID;
                                }
                                if (context.GoNext())
                                {
                                    module.IDs.Add(ID , context.Current.content);
                                    if (WillUseEndMark)
                                    {
                                        if (context.MatchNext(EndMark) != MatchResult.Match)
                                        {
                                            assembleResult.AddError(new MustEndWithSpecifiedEndMarkError(context.Current , EndMark));
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
                            assembleResult.AddError(new AssemblerNotDefinedError(context.Current));
                            return _IC;
                        }
                        else
                        {
                            _IC = eXtensibleAssemble(module , assembleResult , context , Labels , Texts , IDs , _IC);
                        }
                    }
                    break;
                case 4:
                    {
                        //Func
                        var result = context.MatchMarch((AssemblerDefinition?.FunctionIdentifier) ?? "fn" , true , AssemblerDefinition?.CaseSensitiveExternIdentifiers ?? true);
                        if (result == MatchResult.Match)
                        {
                            if (context.ReachEnd)
                            {
                                assembleResult.AddError(new UnexpectedEndOfFileError(context.Current));
                                return _IC;
                            }
                            var Name = context.Current;
                            if (Name == null)
                            {
                                assembleResult.AddError(new UnexpectedEndOfFileError(context.Current));
                                return _IC;
                            }
                            context.GoNext();
                            if (context.ReachEnd)
                            {
                                assembleResult.AddError(new UnexpectedEndOfFileError(context.Current));
                                return _IC;
                            }
                            var Label = context.Current;
                            module.ExternFunctions.Add(new IntermediateExternFunction { Name = Name.content , PseudoLabel = Label });
                            context.GoNext();
                            if (AssemblerDefinition?.UseExternStartMark ?? true == true)
                            {
                                result = context.MatchMarch((AssemblerDefinition?.ExternStartMark) ?? ":" , true , true);
                                if (result == MatchResult.Match)
                                {
                                    context.GoBack();
                                }
                                else if (result == MatchResult.Mismatch)
                                {
                                    assembleResult.AddError(new ExpectAMarkError(context.Last , (AssemblerDefinition?.ExternStartMark) ?? ":"));
                                }
                                else if (result == MatchResult.ReachEnd)
                                {
                                    assembleResult.AddError(new UnexpectedEndOfFileError(context.Current));
                                }


                            }
                        }
                        else if (result == MatchResult.Mismatch)
                        {
                            //ReturnIdentifier
                            result = context.MatchMarch((AssemblerDefinition?.ReturnIdentifier) ?? "return" , true , AssemblerDefinition?.CaseSensitiveReturnIdentifier ?? true);
                            if (result == MatchResult.Match)
                            {
                                var MainType = context.Current;
                                context.GoNext();
                                var AType = context.Current;
                                context.GoNext();
                                var fn = module.ExternFunctions.Last();
                                fn.ReturnType = (new IntermediateDataType { PseudoType = MainType , PseudoAdditionalType = AType });
                                if (AssemblerDefinition?.UseStatementEndMark ?? true == true)
                                {
                                    result = context.MatchMarch((AssemblerDefinition?.StatementEndMark) ?? ";" , true , true);
                                    if (result == MatchResult.Match)
                                    {
                                        context.GoBack();
                                    }
                                    else if (result == MatchResult.Mismatch)
                                    {
                                        assembleResult.AddError(new ExpectAMarkError(context.Last , (AssemblerDefinition?.StatementEndMark) ?? ";"));
                                    }
                                    else if (result == MatchResult.ReachEnd)
                                    {
                                        assembleResult.AddError(new UnexpectedEndOfFileError(context.Current));
                                    }
                                }

                            }
                            else if (result == MatchResult.Mismatch)
                            {
                                var MainType = context.Current;
                                context.GoNext();
                                var AType = context.Current;
                                context.GoNext();
                                var fn = module.ExternFunctions.Last();
                                fn.Registers.Add(fn.Registers.Count , new IntermediateDataType { PseudoType = MainType , PseudoAdditionalType = AType });
                                if (AssemblerDefinition?.UseStatementEndMark ?? true == true)
                                {
                                    result = context.MatchMarch((AssemblerDefinition?.StatementEndMark) ?? ";" , true , true);
                                    if (result == MatchResult.Match)
                                    {
                                        context.GoBack();
                                    }
                                    else if (result == MatchResult.Mismatch)
                                    {
                                        assembleResult.AddError(new ExpectAMarkError(context.Last , (AssemblerDefinition?.StatementEndMark) ?? ";"));
                                    }
                                    else if (result == MatchResult.ReachEnd)
                                    {
                                        assembleResult.AddError(new UnexpectedEndOfFileError(context.Current));
                                    }
                                }

                            }
                        }

                    }
                    break;
                case 5:
                    {
                        var result = context.MatchCollectionMarch(AssemblerDefinition?.CaseSensitiveExternIdentifiers ?? true ,
                                                                  (AssemblerDefinition?.StructIdentifier) ?? "st" ,
                                                                  (AssemblerDefinition?.RuntimeStructIdentifier) ?? "rst");
                        //var result = context.MatchMarch((AssemblerDefinition?.StructIdentifier) ?? "st" ,
                        //                                true ,
                        //                                AssemblerDefinition?.CaseSensitiveExternIdentifiers ?? true);
                        if (result.Item1 == MatchResult.Match)
                        {
                            if (context.ReachEnd)
                            {
                                assembleResult.AddError(new UnexpectedEndOfFileError(context.Current));
                                return _IC;
                            }
                            var Name = context.Current;
                            if (Name == null)
                            {
                                assembleResult.AddError(new UnexpectedEndOfFileError(context.Current));
                                return _IC;
                            }
                            context.GoNext();
                            if (context.ReachEnd)
                            {
                                assembleResult.AddError(new UnexpectedEndOfFileError(context.Current));
                                return _IC;
                            }
                            module.ExternStructs.Add(new ExternStruct { Name = Name.content });
                            if (result.Item2 == 1) { module.ExternStructs.Last().RuntimeStruct = true; }
                            if (AssemblerDefinition?.UseExternStartMark ?? true == true)
                            {
                                var result0 = context.MatchMarch((AssemblerDefinition?.ExternStartMark) ?? ":" , true , true);
                                if (result0 == MatchResult.Match)
                                {
                                    context.GoBack();
                                }
                                else if (result0 == MatchResult.Mismatch)
                                {
                                    assembleResult.AddError(new ExpectAMarkError(context.Last , (AssemblerDefinition?.ExternStartMark) ?? ":"));
                                }
                                else if (result0 == MatchResult.ReachEnd)
                                {
                                    assembleResult.AddError(new UnexpectedEndOfFileError(context.Current));
                                }


                            }
                        }
                        else if (result.Item1 == MatchResult.Mismatch)
                        {
                            //ReturnIdentifier
                            {
                                var MainType = context.Current;
                                context.GoNext();
                                var AType = context.Current;
                                context.GoNext();
                                var fn = module.ExternStructs.Last();
                                fn.Fields.Add(fn.Fields.Count , new IntermediateDataType { PseudoType = MainType , PseudoAdditionalType = AType });
                                if (AssemblerDefinition?.UseStatementEndMark ?? true == true)
                                {
                                    var result0 = context.MatchMarch((AssemblerDefinition?.StatementEndMark) ?? ";" , true , true);
                                    if (result0 == MatchResult.Match)
                                    {
                                        context.GoBack();
                                    }
                                    else if (result0 == MatchResult.Mismatch)
                                    {
                                        assembleResult.AddError(new ExpectAMarkError(context.Last , (AssemblerDefinition?.StatementEndMark) ?? ";"));
                                    }
                                    else if (result0 == MatchResult.ReachEnd)
                                    {
                                        assembleResult.AddError(new UnexpectedEndOfFileError(context.Current));
                                    }
                                }

                            }
                        }
                    }
                    break;
                case 6:
                    {

                        var MainType = context.Current;
                        context.GoNext();
                        var AType = context.Current;
                        context.GoNext();
                        module.ExternVariables.Add(new IntermediateDataType { PseudoType = MainType , PseudoAdditionalType = AType });
                        if (AssemblerDefinition?.UseStatementEndMark ?? true == true)
                        {
                            var result0 = context.MatchMarch((AssemblerDefinition?.StatementEndMark) ?? ";" , true , true);
                            if (result0 == MatchResult.Match)
                            {
                                context.GoBack();
                            }
                            else if (result0 == MatchResult.Mismatch)
                            {
                                assembleResult.AddError(new ExpectAMarkError(context.Last , (AssemblerDefinition?.StatementEndMark) ?? ";"));
                            }
                            else if (result0 == MatchResult.ReachEnd)
                            {
                                assembleResult.AddError(new UnexpectedEndOfFileError(context.Current));
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
            context.GoNext();
            return _IC;
        }

        private int eXtensibleAssemble(xCVMModule module ,
                                       OperationResult<xCVMModule> assembleResult ,
                                       SegmentContext context ,
                                       Dictionary<string , int> Labels ,
                                       Dictionary<string , int> Texts ,
                                       Dictionary<string , int> IDs ,
                                       int _IC)
        {
            var a = context.MatchCollectionMarchReturnContentable(AssemblerDefinition!.Definitions ,
                                                                   AssemblerDefinition!.CaseSensitiveInstructions);
            if (a.Item1 == MatchResult.Match)
            {
                if (a.Item2 is InstructionDefinition def)
                {
                    IntermediateInstruct instruct = new IntermediateInstruct();
                    instruct.Operation = def.ID;
                    instruct.Definition = def;
                    _3Operators(module ,
                                assembleResult ,
                                context ,
                                instruct ,
                                Labels ,
                                Texts ,
                                IDs ,
                                ref _IC ,
                                def.OP0DT ,
                                def.OP0REG ,
                                def.OP1DT ,
                                def.OP1REG ,
                                def.OP2DT ,
                                def.OP2REG ,
                                true);
                }
            }
            else if (a.Item1 == MatchResult.Mismatch)
            {
                var r = context.MatchNext(":" , true);
                if (r == MatchResult.Match)
                {
                    if (context.Current == null)
                    {
                        assembleResult.AddError(new UnexpectedEndOfFileError(context.Current));
                        return _IC;
                    }
                    string name = context.Current.Prev.content;
                    Labels.Add(name , _IC + 1);
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


        bool WillUseEndMark = true;
        string EndMark = ";";
        private void _3Operators(xCVMModule module ,
                                 OperationResult<xCVMModule> assembleResult ,
                                 SegmentContext context ,
                                 Instruct inst ,
                                 Dictionary<string , int> Labels ,
                                 Dictionary<string , int> Texts ,
                                 Dictionary<string , int> IDs ,
                                 ref int IC ,
                                 int Reg0Data = 0 ,
                                 bool AcceptReg0 = false ,
                                 int Reg1Data = 0 ,
                                 bool AcceptReg1 = false ,
                                 int Reg2Data = 0 ,
                                 bool AcceptReg2 = false ,
                                 bool SupressError = false)
        {
            if (Reg0Data != -2)
            {

                if (NextData(assembleResult , context , AcceptReg0 , Reg0Data , Labels , Texts , IDs , SupressError , out inst.Op0))
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
            }
            if (Reg1Data != -2)
            {
                if (NextData(assembleResult , context , AcceptReg1 , Reg1Data , Labels , Texts , IDs , SupressError , out inst.Op1))
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
            }
            if (Reg2Data != -2)
            {
                if (NextData(assembleResult , context , AcceptReg2 , Reg2Data , Labels , Texts , IDs , SupressError , out inst.Op2))
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
            }

            if (WillUseEndMark)
            {
                var __re = context.MatchNext(EndMark);
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
        private bool NextData(OperationResult<xCVMModule> assembleResult ,
                              SegmentContext context ,
                              bool AcceptRegister ,
                              int dataType ,
                              Dictionary<string , int> Labels ,
                              Dictionary<string , int> Texts ,
                              Dictionary<string , int> IDs ,
                              bool SupressError ,
                              out byte [ ] reg)
        {
            switch (dataType)
            {
                case 1:
                    {
                        if (NextInt(assembleResult , context , AcceptRegister , Labels , Texts , IDs , SupressError , out var reg0))
                        {
                            reg = BitConverter.GetBytes(reg0);
                            return true;
                        }
                    }
                    break;
                case 2:
                    {
                        if (NextLong(assembleResult , context , AcceptRegister , SupressError , out var reg0))
                        {
                            reg = BitConverter.GetBytes(reg0);
                            return true;
                        }
                    }
                    break;
                case 3:
                    {
                        if (NextFloat(assembleResult , context , SupressError , out var reg0))
                        {
                            reg = BitConverter.GetBytes(reg0);
                            return true;
                        }
                    }
                    break;
                case 4:
                    {
                        if (NextDouble(assembleResult , context , SupressError , out var reg0))
                        {
                            reg = BitConverter.GetBytes(reg0);
                            return true;
                        }
                    }
                    break;
                case 5:
                    {
                        if (NextUInt(assembleResult , context , AcceptRegister , Labels , Texts , IDs , SupressError , out var reg0))
                        {
                            reg = BitConverter.GetBytes(reg0);
                            return true;
                        }
                    }
                    break;
                case 6:
                    {
                        if (NextULong(assembleResult , context , AcceptRegister , SupressError , out var reg0))
                        {
                            reg = BitConverter.GetBytes(reg0);
                            return true;
                        }
                    }
                    break;
                case Constants._short:
                    {
                        if (NextShort(assembleResult , context , AcceptRegister , SupressError , out var reg0))
                        {
                            reg = BitConverter.GetBytes(reg0);
                            return true;
                        }
                    }
                    break;
                case Constants._ushort:
                    {
                        if (NextUShort(assembleResult , context , AcceptRegister , SupressError , out var reg0))
                        {
                            reg = BitConverter.GetBytes(reg0);
                            return true;
                        }
                    }
                    break;
                case Constants._byte:
                    {
                        if (NextByte(assembleResult , context , AcceptRegister , SupressError , out var reg0))
                        {
                            reg = new byte [ ] { reg0 };
                            return true;
                        }
                    }
                    break;
                default:
                    break;
            }
            reg = new byte [ 0 ];
            return false;
        }
        private bool NextInt(OperationResult<xCVMModule> assembleResult ,
                             SegmentContext context ,
                             bool AcceptRegister ,
                             Dictionary<string , int> Labels ,
                             Dictionary<string , int> Texts ,
                             Dictionary<string , int> IDs ,
                             bool SupressError ,
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
                        reg0 = Texts [ _int ];
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
                        reg0 = IDs [ _int ];
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
                        reg0 = Labels [ _int ];
                        return true;
                    }
                    else
                    {
                        reg0 = -1;
                        return false;
                    }
                }
                else
                if (_int.StartsWith("res?"))
                {
                    _int = _int.Substring(4);
                    if (resourceDef != null)
                    {
                        if (resourceDef.Mapping.ContainsKey(_int))
                        {
                            reg0 = resourceDef.Mapping [ _int ];
                            return true;
                        }
                        else
                        {
                            reg0 = -1;
                            return false;
                        }
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
                if (int.TryParse(_int , out var data))
                {
                    reg0 = data;
                    return true;
                }
                else
                {
                    if (AssemblerDefinition != null)
                    {
                        if (AssemblerDefinition.PredefinedSymbols.TryGetValue(context.Current!.content , out _int))
                        {
                            if (int.TryParse(_int , out data))
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
        private bool NextUInt(OperationResult<xCVMModule> assembleResult ,
                             SegmentContext context ,
                             bool AcceptRegister ,
                             Dictionary<string , int> Labels ,
                             Dictionary<string , int> Texts ,
                             Dictionary<string , int> IDs ,
                             bool SupressError ,
                             out uint reg0)
        {
            if (context.GoNext())
            {
                var _int = context.Current!.content;
                if (_int.StartsWith("text?"))
                {
                    _int = _int.Substring(5);
                    if (Texts.ContainsKey(_int))
                    {
                        if (Texts [ _int ] > 0)
                        {
                            reg0 = (uint)Texts [ _int ];
                            return true;
                        }
                        else
                        {
                            reg0 = 0;
                            assembleResult.AddError(new UIntParseError(context.Current));
                            return false;
                        }
                    }
                    else
                    {
                        reg0 = 0;
                        return false;
                    }
                }
                else
                if (_int.StartsWith("id?"))
                {
                    _int = _int.Substring(3);
                    if (IDs.ContainsKey(_int))
                    {
                        if (IDs [ _int ] > 0)
                        {
                            reg0 = (uint)IDs [ _int ];
                            return true;
                        }
                        else
                        {
                            reg0 = 0;
                            assembleResult.AddError(new UIntParseError(context.Current));
                            return false;
                        }
                    }
                    else
                    {
                        reg0 = 0;
                        return false;
                    }
                }
                else
                if (_int.StartsWith("lbl?"))
                {
                    _int = _int.Substring(4);
                    if (Labels.ContainsKey(_int))
                    {
                        if (Labels [ _int ] > 0)
                        {
                            reg0 = (uint)Labels [ _int ];
                            return true;
                        }
                        else
                        {
                            reg0 = 0;
                            assembleResult.AddError(new UIntParseError(context.Current));
                            return false;
                        }
                    }
                    else
                    {
                        reg0 = 0;
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
                if (uint.TryParse(_int , out var data))
                {
                    reg0 = data;
                    return true;
                }
                else
                {
                    if (AssemblerDefinition != null)
                    {
                        if (AssemblerDefinition.PredefinedSymbols.TryGetValue(context.Current!.content , out _int))
                        {
                            if (uint.TryParse(_int , out data))
                            {
                                reg0 = data;
                                return true;
                            }
                            else
                            {
                                if (!SupressError)
                                    assembleResult.AddError(new UIntParseError(context.Last));
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

            reg0 = 0;
            return false;
        }
        private bool NextLong(OperationResult<xCVMModule> assembleResult ,
                              SegmentContext context ,
                              bool AcceptRegister ,
                              bool SupressError ,
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
                if (long.TryParse(_long , out var data))
                {
                    reg0 = data;
                    return true;
                }
                else
                {
                    if (AssemblerDefinition != null)
                    {
                        if (AssemblerDefinition.PredefinedSymbols.TryGetValue(context.Current!.content , out _long))
                        {
                            if (long.TryParse(_long , out data))
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
        private bool NextShort(OperationResult<xCVMModule> assembleResult ,
                              SegmentContext context ,
                              bool AcceptRegister ,
                              bool SupressError ,
                              out short reg0)
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
                if (short.TryParse(_long , out var data))
                {
                    reg0 = data;
                    return true;
                }
                else
                {
                    if (AssemblerDefinition != null)
                    {
                        if (AssemblerDefinition.PredefinedSymbols.TryGetValue(context.Current!.content , out _long))
                        {
                            if (short.TryParse(_long , out data))
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
        private bool NextByte(OperationResult<xCVMModule> assembleResult ,
                              SegmentContext context ,
                              bool AcceptRegister ,
                              bool SupressError ,
                              out byte reg0)
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
                if (byte.TryParse(_long , out var data))
                {
                    reg0 = data;
                    return true;
                }
                else
                {
                    if (AssemblerDefinition != null)
                    {
                        if (AssemblerDefinition.PredefinedSymbols.TryGetValue(context.Current!.content , out _long))
                        {
                            if (byte.TryParse(_long , out data))
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

            reg0 = 0;
            return false;
        }
        private bool NextULong(OperationResult<xCVMModule> assembleResult ,
                              SegmentContext context ,
                              bool AcceptRegister ,
                              bool SupressError ,
                              out ulong reg0)
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
                if (ulong.TryParse(_long , out var data))
                {
                    reg0 = data;
                    return true;
                }
                else
                {
                    if (AssemblerDefinition != null)
                    {
                        if (AssemblerDefinition.PredefinedSymbols.TryGetValue(context.Current!.content , out _long))
                        {
                            if (ulong.TryParse(_long , out data))
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

            reg0 = 0;
            return false;
        }
        private bool NextUShort(OperationResult<xCVMModule> assembleResult ,
                              SegmentContext context ,
                              bool AcceptRegister ,
                              bool SupressError ,
                              out ushort reg0)
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
                if (ushort.TryParse(_long , out var data))
                {
                    reg0 = data;
                    return true;
                }
                else
                {
                    if (AssemblerDefinition != null)
                    {
                        if (AssemblerDefinition.PredefinedSymbols.TryGetValue(context.Current!.content , out _long))
                        {
                            if (ushort.TryParse(_long , out data))
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

            reg0 = 0;
            return false;
        }
        private bool NextFloat(OperationResult<xCVMModule> assembleResult , SegmentContext context ,
                              bool SupressError , out float reg0)
        {
            if (context.GoNext())
            {
                var _float = context.Current!.content;
                if (float.TryParse(_float , out var data))
                {
                    reg0 = data;
                    return true;
                }
                else
                {
                    if (AssemblerDefinition != null)
                    {
                        if (AssemblerDefinition.PredefinedSymbols.TryGetValue(context.Current!.content , out _float))
                        {
                            if (float.TryParse(_float , out data))
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
        private bool NextDouble(OperationResult<xCVMModule> assembleResult , SegmentContext context ,
                              bool SupressError , out double reg0)
        {
            if (context.GoNext())
            {
                var _double = context.Current!.content;
                if (double.TryParse(_double , out var data))
                {
                    reg0 = data;
                    return true;
                }
                else
                {
                    if (AssemblerDefinition != null)
                    {
                        if (AssemblerDefinition.PredefinedSymbols.TryGetValue(context.Current!.content , out _double))
                        {
                            if (double.TryParse(_double , out data))
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

        public OperationResult<xCVMModule> Assemble(string content)
        {
            var segments = parser.Scan(content , false);
            return Assemble(segments);
        }
    }
}
