using System;
using System.Collections.Generic;

namespace Cx.Preprocessor
{
    public class Preprocessor
    {
        public Dictionary<string , string> Symbols = new Dictionary<string , string>();
        public void Define(string symbol , string value)
        {
            if (Symbols.ContainsKey(symbol)) { Symbols [ symbol ] = value; }
            else { Symbols.Add(symbol , value); }
        }
        public bool ifdef(string name)
        {
            return Symbols.ContainsKey(name);   
        }
        
    }
}
