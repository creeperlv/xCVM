using Cx.Core.DataTools;
using LibCLCC.NET.TextProcessing;

namespace Cx.Core.VCParser
{
    public class ExpressionSegment
    {
        public ExpressionSegment? Prev = null;
        public ExpressionSegment? Next = null;
        public Segment? Segment = null;
        public TreeNode? Node = null;
        public ExpressionSegmentContext? Context;
        public ExpressionSegment Duplicate() => new ExpressionSegment { Prev=null,Next=null, Segment = Segment , Node = Node , Context = Context , ID = ID };
        public ExpressionSegmentType SegmentType
        {
            get
            {

                if (Node != null)
                {
                    if (Node is ExpressionTreeNode)
                    {
                        return ExpressionSegmentType.ESTreeNode;

                    }
                    return ExpressionSegmentType.TreeNode;
                }
                if (Segment != null) return ExpressionSegmentType.PlainContent;
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
        public ExpressionSegment GetEnd()
        {
            ExpressionSegment segment = this;
            while (true)
            {
                if (segment.Next == null) break;
                segment = segment.Next;
            }
            return segment;
        }
        public void AttachNext(ExpressionSegment next)
        {
            Next = next;
            next.Prev = this;
        }
    }
}