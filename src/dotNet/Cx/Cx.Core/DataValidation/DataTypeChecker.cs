using LibCLCC.NET.TextProcessing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Xml.Linq;

namespace Cx.Core.DataTools
{
    public static class DataTypeChecker
    {
        public static CStyleScanner CStyleParser = new CStyleScanner();
        public static bool ValidateLong(string input)
        {
            if (input.StartsWith("0x"))
            {
                var ch = input [ 2.. ].ToUpper();
                for (int i = 0 ; i < ch.Length ; i++)
                {
                    if ((ch [ i ] >= '0' && ch [ i ] <= '9') || (ch [ i ] >= 'A' && ch [ i ] <= 'F') || ch [ i ] == '_')
                    {

                    }
                    else return false;
                }
                return true;
            }
            else if (input.StartsWith("0b"))
            {
                var ch = input [ 2.. ];
                for (int i = 0 ; i < ch.Length ; i++)
                {
                    if (ch [ i ] != '0' && ch [ i ] != '1' && ch [ i ] != '_')
                    {
                        return false;
                    }
                }
                return true;
            }
            else if (input.StartsWith("0o"))
            {
                var ch = input [ 2.. ];
                for (int i = 0 ; i < ch.Length ; i++)
                {
                    if ((ch [ i ] >= '0' && ch [ i ] <= '7') || ch [ i ] == '_')
                    {
                    }
                    else return false;
                }
                return true;
            }
            else
            {
                for (int i = 0 ; i < input.Length ; i++)
                {
                    if ((input [ i ] >= '0' && input [ i ] <= '9') || input [ i ] == '_')
                    {

                    }
                    else return false;
                }
                return true;
            }
        }
        public static bool ValidateDouble(string input)
        {
            return (double.TryParse(input , NumberStyles.Float | NumberStyles.AllowThousands , CultureInfo.InvariantCulture , out _));
        }
        public static bool ValidateULong(string input)
        {
            return false;
        }
        public static DataType DetermineDataType(string content)
        {
            if (content.Length == 1)
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
            if (ValidateLong(c) && (Suffix == '\0' || Suffix == 'l' || Suffix == 'L'))
            {
                return DataType.IntegerAny;
            }
            else if (ValidateDouble(c) && (Suffix == '\0' || Suffix == 'f' || Suffix == 'F' || Suffix == 'd' || Suffix == 'F'))
            {
                return DataType.DecimalAny;
            }
            //if (TypeDescriptor.GetConverter(typeof(long)).IsValid(c) && (Suffix == '\0' || Suffix == 'l' || Suffix == 'L'))
            //    return DataType.IntegerAny;
            //if (TypeDescriptor.GetConverter(typeof(double)).IsValid(c) && (Suffix == '\0' || Suffix == 'f' || Suffix == 'F'))
            //    return DataType.DecimalAny;
            return DataType.String;
        }
        public static DataTypeEx DetermineDataTypePrecisely(string content)
        {
            if (content.Length == 1)
                foreach (var item in CStyleParser.PredefinedSegmentCharacters)
                {
                    if (item == content [ 0 ]) return DataTypeEx.Symbol;
                }
            foreach (var item in CStyleParser.PredefinedSegmentTemplate)
            {
                if (item == content) return DataTypeEx.Symbol;
            }
            string c = content;
            char Suffix = '\0';
            char Suffix2 = '\0';
            if (c.EndsWith("f") || c.EndsWith("l") || c.EndsWith("L"))
            {
                Suffix = c [ ^1 ];
                c = c [ ..^1 ];
                Suffix2 = c [ ^1 ];
            }
            if (ValidateLong(c) && (Suffix == '\0' || Suffix == 'l' || Suffix == 'L' || Suffix == 'u' || Suffix == 'U'))
            {
                if (Suffix == 'l' || Suffix == 'L')
                    return DataTypeEx.Int64;
                if (DataConverter.TryParse(c , out long d))
                {
                    if (d <= int.MaxValue && d >= int.MinValue)
                        return DataTypeEx.Int32;
                    return DataTypeEx.Int64;
                }
                return DataTypeEx.Int32;
            }
            else if (ValidateDouble(c) && (Suffix == '\0' || Suffix == 'f' || Suffix == 'F' || Suffix == 'd' || Suffix == 'F'))
            {
                if (Suffix == 'd' || Suffix == 'D')
                    return DataTypeEx.Double;
                if (Suffix == 'f' || Suffix == 'F')
                    return DataTypeEx.Double;
                if (double.TryParse(c , out double d))
                {
                    if (d >= float.MinValue && d <= float.MaxValue)
                    {
                        return DataTypeEx.Single;
                    }
                    else
                        return DataTypeEx.Double;
                }
                return DataTypeEx.Single;
            }
            //if (TypeDescriptor.GetConverter(typeof(long)).IsValid(c) && (Suffix == '\0' || Suffix == 'l' || Suffix == 'L'))
            //    return DataType.IntegerAny;
            //if (TypeDescriptor.GetConverter(typeof(double)).IsValid(c) && (Suffix == '\0' || Suffix == 'f' || Suffix == 'F'))
            //    return DataType.DecimalAny;
            return DataTypeEx.String;
        }
    }
    public enum DataType
    {
        String, IntegerAny, Symbol, DecimalAny
    }
    public enum DataTypeEx
    {
        String, Int32, Int64, Symbol, Single, Double
    }
}
