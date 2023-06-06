using System.Collections.Generic;

namespace Cx.Core.VCParser
{
    public class ParserProvider
    {
        Dictionary<int, ContextualParser> parsers = new Dictionary<int, ContextualParser>();
        public void RegisterParser(int ID, ContextualParser parser)
        {
            if (parsers.ContainsKey(ID))
            {
                parsers[ID] = parser;
            }
            else parsers.Add(ID, parser);
        }
        public ContextualParser? GetParser(int ID)
        {
            if (parsers.ContainsKey(ID))
            {
                return parsers[ID];
            }
            return null;
        }
    }
}