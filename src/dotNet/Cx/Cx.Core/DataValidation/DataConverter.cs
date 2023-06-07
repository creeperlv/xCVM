using System.Linq;

namespace Cx.Core.DataTools
{
    public static class DataConverter
    {
        public static bool TryParse(string input , out int data)
        {
            if (input.Last() == 'l' || input.Last() == 'L')
            {
                data = -1;
                return false;
            }
            if (input.StartsWith("0x"))
            {
                var ch = input [ 2.. ].ToUpper();
                int _data = 0;
                for (int i = 0 ; i < ch.Length ; i++)
                {
                    _data *= 16;
                    if ((ch [ i ] >= '0' && ch [ i ] <= '9'))
                    {
                        _data += ch [ i ] - '0';
                    }
                    else
                    if ((ch [ i ] >= 'A' && ch [ i ] <= 'F'))
                    {
                        _data += ch [ i ] - 'A' + 10;
                    }
                    else
                    if ((ch [ i ] >= 'a' && ch [ i ] <= 'f'))
                    {
                        _data += ch [ i ] - 'a' + 10;
                    }
                    else
                    {
                        data = -1;
                        return false;
                    }
                }
                data = _data;
                return true;
            }
            else if (input.StartsWith("0o"))
            {
                var ch = input [ 2.. ].ToUpper();
                int _data = 8;
                for (int i = 0 ; i < ch.Length ; i++)
                {
                    _data *= 16;
                    if ((ch [ i ] >= '0' && ch [ i ] <= '7'))
                    {
                        _data += ch [ i ] - '0';
                    }
                    else
                    {
                        data = -1;
                        return false;
                    }
                }
                data = _data;
                return true;
            }
            else
            if (input.StartsWith("0b"))
            {
                var ch = input [ 2.. ].ToUpper();
                int _data = 0;
                for (int i = 0 ; i < ch.Length ; i++)
                {
                    _data *= 2;
                    if (ch [ i ] == '0')
                    {
                        _data += 0;
                    }
                    else
                    if (ch [ i ] == '1')
                    {
                        _data += 1;
                    }
                    else
                    {
                        data = -1;
                        return false;
                    }
                }
                data = _data;
                return true;
            }
            data = 0;
            return false;
        }
        public static bool TryParse(string input , out long data)
        {
            if (input.Last() == 'l' || input.Last() == 'L') input = input[ ..^1 ];
            if (input.StartsWith("0x"))
            {
                var ch = input [ 2.. ].ToUpper();
                long _data = 0;
                for (int i = 0 ; i < ch.Length ; i++)
                {
                    _data *= 16;
                    if ((ch [ i ] >= '0' && ch [ i ] <= '9'))
                    {
                        _data += ch [ i ] - '0';
                    }
                    else
                    if ((ch [ i ] >= 'A' && ch [ i ] <= 'F'))
                    {
                        _data += ch [ i ] - 'A' + 10;
                    }
                    else
                    if ((ch [ i ] >= 'a' && ch [ i ] <= 'f'))
                    {
                        _data += ch [ i ] - 'a' + 10;
                    }
                    else
                    {
                        data = -1;
                        return false;
                    }
                }
                data = _data;
                return true;
            }
            else if (input.StartsWith("0o"))
            {
                var ch = input [ 2.. ].ToUpper();
                long _data = 8;
                for (int i = 0 ; i < ch.Length ; i++)
                {
                    _data *= 16;
                    if ((ch [ i ] >= '0' && ch [ i ] <= '7'))
                    {
                        _data += ch [ i ] - '0';
                    }
                    else
                    {
                        data = -1;
                        return false;
                    }
                }
                data = _data;
                return true;
            }
            else
            if (input.StartsWith("0b"))
            {
                var ch = input [ 2.. ].ToUpper();
                long _data = 0;
                for (int i = 0 ; i < ch.Length ; i++)
                {
                    _data *= 2;
                    if (ch [ i ] == '0')
                    {
                        _data += 0;
                    }
                    else
                    if (ch [ i ] == '1')
                    {
                        _data += 1;
                    }
                    else
                    {
                        data = -1;
                        return false;
                    }
                }
                data = _data;
                return true;
            }
            data = 0;
            return false;
        }
    }
}
