using Cx.Core.DataTools;
using LibCLCC.NET.TextProcessing;

namespace Cx.Core.VCParser
{
    public class ExpressionSegment
    {
        public ExpressionSegment? Prev;
        public ExpressionSegment? Next;
        public Segment? Segment;
        public TreeNode? Node;
        public ExpressionSegmentContext? Context;
        public ExpressionSegmentType SegmentType
        {
            get
            {

                if (Segment != null) return ExpressionSegmentType.PlainContent;
                if (Node != null)
                {
                    if(Node is ExpressionTreeNode)
                    {
                    return ExpressionSegmentType.ESTreeNode;

                    }
                    return ExpressionSegmentType.TreeNode;
                }
                if (Context != null) return ExpressionSegmentType.Closure;
                return ExpressionSegmentType.NULL;
            }
        }
        public int ID;
        public bool IsOkayForExpressionPart
        {
            get
            {
                if (Segment != null)
                {
                    var dt = DataTypeChecker.DetermineDataType(Segment.content);
                    if (dt == DataType.String || dt == DataType.IntegerAny || dt == DataType.DecimalAny) return true;
                }
                if (Node != null) return true;
                return false;
            }
        }
        public void AttachNext(ExpressionSegment next)
        {
            Next = next;
            next.Prev = this;
        }
    }
}