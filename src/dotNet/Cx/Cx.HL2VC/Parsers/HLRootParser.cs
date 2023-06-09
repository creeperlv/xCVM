using Cx.Core.VCParser;
using System.Collections.Generic;

namespace Cx.HL2VC.Parsers
{
    public class HLRootParser : RootParser
    {
        public HLRootParser()
        {
            var old_list = ConcernedParsers;
            ConcernedParsers = new List<int>
            {
                HLASTNodeType.Namespace ,
                HLASTNodeType.Using
            };
            foreach (var item in old_list)
            {
                ConcernedParsers.Add(item);
            }
        }
    }
}
