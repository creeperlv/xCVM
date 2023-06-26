namespace Cx.Core.VCParser
{
    public static class ExpressionSymbols
    {
        public static readonly string [ ] Termination = new string [ ] { ","  , ";" , "" };
        public static readonly string [ ] RightHand_Unary_0st = new string [ ] { "!" ,"&","*", "++" , "--" };
        public static readonly string [ ] LeftHand_Unary_0st = new string [ ] { "++" , "--" };
        public static readonly string [ ] Binary_0st = new string [ ] { "&&" , "||" , "&" , "|" };
        public static readonly string [ ] Binary_1st = new string [ ] { "*" , "/" , "%" };
        public static readonly string [ ] Binary_2st = new string [ ] { "+" , "-" };

    }
}