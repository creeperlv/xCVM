namespace Cx.Core
{
    public static class ASTNodeType
    {
        public const int Root = -1;
        public const int DeclareFunc = 0;
        public const int DeclareStruct = 1;
        public const int DeclareVar = 2;
        public const int Assign = 3;
        public const int Expression = 4;
        public const int BinaryExpression = 5;
        public const int UnaryExpression = 6;
        public const int Arguments = 7;
        public const int Parameters = 8;
        public const int Scope = 9;
        public const int Return = 10;
        public const int ReturnType = 11;
        public const int DataType = 12;
        public const int UseStruct = 13;
        public const int Name = 14;
        public const int Pointer = 15;
        public const int EndNode = 16;
        public const int SingleParameter = 17;
        public const int Enum = 18;
        public const int EnumElement = 19;
    }

}