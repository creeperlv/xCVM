namespace Cx.Core
{
    public enum ASTNodeType
    {
        DeclareFunc,
        DeclareStruct,
        DeclareVar, 
        Assign, 
        Expression, 
        BinaryExpression, 
        UnaryExpression, 
        Arguments, 
        Parameters,
        Scope, 
        Return, 
        ReturnType,
        DataType,
        UseStruct,
        Name,
        Pointer
    }

}