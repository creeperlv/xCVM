using Cx.Core;
using Cx.Core.VCParser;
using Cx.HL2VC.Parsers;
using LibCLCC.NET.TextProcessing;

namespace Cx.HL.Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }
        void PrintDepth(int depth , string content)
        {
            for (int i = 0 ; i < depth ; i++)
            {
                Console.Write("\t");
            }
            Console.WriteLine(content);
        }
        void PrintESTree(TreeNode node , int Depth)
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
        void TestExpression(GeneralPurposeScanner scanner , string input , bool ExpectError)
        {
            ExpressionParser parser = new ExpressionParser();
            var seg = scanner.Scan(input , false);
            var p = HLCParsers.GetProvider();
            TreeNode root = new TreeNode();
            root.Type = ASTNodeType.Root;
            var result = parser.Parse(p , new xCVM.Core.CompilerServices.SegmentContext(seg) , root);
            PrintESTree(root,0);
            if (ExpectError)
            {
                Assert.That(result.Errors , Is.Not.Empty);
            }
            else
                Assert.That(result.Errors , Is.Empty);
        }
        [Test]
        public void Test1()
        {
            CStyleScanner scanner = new CStyleScanner();
            TestExpression(scanner , "1+2" , false);
            TestExpression(scanner , "1+asd" , false);
            TestExpression(scanner , "1++asd" , false);
            TestExpression(scanner , "1+&asd" , false);
            Assert.Pass();
        }
    }
}