using Cx.Core;
using Cx.Core.VCParser;
using Cx.HL2VC;
using Cx.HL2VC.Parsers;
using LibCLCC.NET.TextProcessing;

namespace DevTestApp
{
    internal class Program
    {
        static void PrintDepth(int depth , string content)
        {
            for (int i = 0 ; i < depth ; i++)
            {
                Console.Write("\t");
            }
            Console.WriteLine(content);
        }
        static void PrintESTree(TreeNode node , int Depth)
        {
            switch (node.Type)
            {
                case ASTNodeType.Expression:
                    PrintDepth(Depth , "Node: Expression");
                    break;
                case ASTNodeType.BinaryExpression:
                    PrintDepth(Depth , "Node: BinExp");
                    break;
                case ASTNodeType.UnaryExpression:
                    PrintDepth(Depth , "Node: UnExp");
                    break;
                case ASTNodeType.Call:
                    PrintDepth(Depth , "Node: Function Call");
                    break;
                default:
                    PrintDepth(Depth , "Generic Node");
                    break;
            }
            if (node.Segment != null)
            {
                PrintDepth(Depth , $">Content:{node.Segment.content}");
            }
            else
                PrintDepth(Depth , $">Content: NULL");
            foreach (var item in node.Children)
            {
                PrintESTree(item , Depth + 1);
            }
        }
        static void TestExpression(GeneralPurposeScanner scanner , string input)
        {
            ExpressionParser parser = new ExpressionParser();
            var seg = scanner.Scan(input , false);
            var p = HLCParsers.GetProvider();
            TreeNode root = new TreeNode();
            root.Type = ASTNodeType.Root;
            var result = parser.Parse(p , new xCVM.Core.CompilerServices.SegmentContext(seg) , root);
            PrintESTree(root , 0);
            if (result.Errors.Count > 0)
            {
                Console.WriteLine("\x1b[93mError!\x1b[0m");
                foreach (var item in result.Errors)
                {
                    Console.WriteLine("\t\t" + item.Message);
                }
            }
        }
        static void Main(string [ ] args)
        {
            CStyleScanner cStyleScanner = new CStyleScanner();
            {
                var seg = cStyleScanner.Scan("1++2" , false);
                Console.WriteLine(seg.SequentialToString());
            }
            TestExpression(cStyleScanner , "1+2");
            TestExpression(cStyleScanner , "1+2+3");
            TestExpression(cStyleScanner , "1+(2*3)");
            TestExpression(cStyleScanner , "1+2*3");
            TestExpression(cStyleScanner , "(1+2)*3");
            TestExpression(cStyleScanner , "(1+2)*3++");
            TestExpression(cStyleScanner , "1+&a");
        }
    }
}