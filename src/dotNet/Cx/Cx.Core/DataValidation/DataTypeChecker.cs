using LibCLCC.NET.TextProcessing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Cx.Core.DataValidation
{
    public static class DataTypeChecker
    {
        public static CStyleScanner CStyleParser = new CStyleScanner();
        public static DataType DetermineDataType(string content)
        {
            if (content.Length == 0)
                foreach (var item in CStyleParser.PredefinedSegmentCharacters)
                {
                    if (item == content [ 0 ]) return DataType.Symbol;
                }
            foreach (var item in CStyleParser.PredefinedSegmentTemplate)
            {
                if (item == content) return DataType.Symbol;
            }
            string c = content;
            char Suffix = '\0';
            if (c.EndsWith("f") || c.EndsWith("l") || c.EndsWith("L"))
            {
                Suffix = c [ ^1 ];
                c = c [ ..^1 ];
            }
            if (TypeDescriptor.GetConverter(typeof(long)).IsValid(c) && (Suffix == '\0' || Suffix == 'l' || Suffix == 'L'))
                return DataType.DecimalNumber;
            if (TypeDescriptor.GetConverter(typeof(double)).IsValid(c) && (Suffix == '\0' || Suffix == 'f' || Suffix == 'F'))
                return DataType.FloatingNumber;
            return DataType.String;
        }
    }
    public enum DataType
    {
        String, DecimalNumber, Symbol, FloatingNumber
    }
}
