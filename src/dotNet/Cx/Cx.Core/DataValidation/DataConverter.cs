using System.Linq;

namespace Cx.Core.DataTools
{
    public static class DataConverter
    {
        public static bool TryParse(string input , out int data)
        {
            bool Negative = input.StartsWith("-");
            input = input.ToUpper();
            int Start = 0;
            int Length = input.Length;
            if (Negative)
            {
                Start = 1;
            }
            if (input.EndsWith("L"))
            {
                data = -1;
                return false;
            }
            if (input.StartsWith("0X"))
            {
                Start += 2;
                Length -= 2;
                int _data = 0;
                for (int i = Start ; i < Length ; i++)
                {
                    _data *= 16;
                    if ((input [ i ] >= '0' && input [ i ] <= '9'))
                    {
                        _data += input [ i ] - '0';
                    }
                    else
                    if ((input [ i ] >= 'A' && input [ i ] <= 'F'))
                    {
                        _data += input [ i ] - 'A' + 10;
                    }
                    else if ((input [ i ] == '_')) continue;
                    else
                    {
                        data = -1;
                        return false;
                    }
                }
                data = _data * (Negative ? -1 : 1);
                return true;
            }
            else if (input.StartsWith("0O"))
            {
                Start += 2;
                Length -= 2;
                int _data = 8;
                for (int i = Start ; i < Length ; i++)
                {
                    _data *= 16;
                    if ((input [ i ] >= '0' && input [ i ] <= '7'))
                    {
                        _data += input [ i ] - '0';
                    }
                    else if ((input [ i ] == '_')) continue;
                    else
                    {
                        data = -1;
                        return false;
                    }
                }
                data = _data * (Negative ? -1 : 1);
                return true;
            }
            else
            if (input.StartsWith("0B"))
            {
                Start += 2;
                Length -= 2;
                int _data = 0;
                for (int i = Start ; i < Length ; i++)
                {
                    _data *= 2;
                    if (input [ i ] == '0')
                    {
                        _data += 0;
                    }
                    else
                    if (input [ i ] == '1')
                    {
                        _data += 1;
                    }
                    else if ((input [ i ] == '_')) continue;
                    else
                    {
                        data = -1;
                        return false;
                    }
                }
                data = _data * (Negative ? -1 : 1);
                return true;
            }
            else
            {

                int _data = 0;
                for (int i = Start ; i < Length ; i++)
                {
                    _data *= 10;
                    if ((input [ i ] >= '0' && input [ i ] <= '9'))
                    {
                        _data += (input [ i ] - '0');
                    }
                    else if ((input [ i ] == '_')) continue;
                    else
                    {
                        data = -1;
                        return false;
                    }
                }
                data = _data * (Negative ? -1 : 1);
                return true;
            }
        }
        public static bool TryParse(string input , out uint data)
        {
            bool Negative = input.StartsWith("-");
            input = input.ToUpper();
            int Start = 0;
            int Length = input.Length;
            if (Negative)
            {
                Start = 1;
            }
            if (input.EndsWith("L"))
            {
                data = 0;
                return false;
            }
            if (input.EndsWith("U"))
            {
                Length -= 1;
            }
            if (input.StartsWith("0X"))
            {
                Start += 2;
                Length -= 2;
                uint _data = 0;
                for (int i = Start ; i < Length ; i++)
                {
                    _data *= 16;
                    if ((input [ i ] >= '0' && input [ i ] <= '9'))
                    {
                        _data += (uint)(input [ i ] - '0');
                    }
                    else
                    if ((input [ i ] >= 'A' && input [ i ] <= 'F'))
                    {
                        _data += (uint)(input [ i ] - 'A') + 10;
                    }
                    else
                    if ((input [ i ] >= 'a' && input [ i ] <= 'f'))
                    {
                        _data += (uint)(input [ i ] - 'a') + 10;
                    }
                    else if ((input [ i ] == '_')) continue;
                    else
                    {
                        data = 0;
                        return false;
                    }
                }
                data = _data;
                return true;
            }
            else if (input.StartsWith("0O"))
            {
                Start += 2;
                Length -= 2;
                uint _data = 8;
                for (int i = Start ; i < Length ; i++)
                {
                    _data *= 16;
                    if ((input [ i ] >= '0' && input [ i ] <= '7'))
                    {
                        _data += (uint)input [ i ] - '0';
                    }
                    else if ((input [ i ] == '_')) continue;
                    else
                    {
                        data = 0;
                        return false;
                    }
                }
                data = _data;
                return true;
            }
            else
            if (input.StartsWith("0B"))
            {
                Start += 2;
                Length -= 2;
                uint _data = 0;
                for (int i = Start ; i < Length ; i++)
                {
                    _data *= 2;
                    if (input [ i ] == '0')
                    {
                        _data += 0;
                    }
                    else
                    if (input [ i ] == '1')
                    {
                        _data += 1;
                    }
                    else if ((input [ i ] == '_')) continue;
                    else
                    {
                        data = 0;
                        return false;
                    }
                }
                data = _data;
                return true;
            }
            else
            {

                uint _data = 0;
                for (int i = Start ; i < Length ; i++)
                {
                    _data *= 10;
                    if ((input [ i ] >= '0' && input [ i ] <= '9'))
                    {
                        _data +=(uint) (input [ i ] - '0');
                    }
                    else if ((input [ i ] == '_')) continue;
                    else
                    {
                        data = 0;
                        return false;
                    }
                }
                data = _data;
                return true;
            }
        }
        public static bool TryParse(string input , out long data)
        {
            bool Negative = input.StartsWith("-");
            input = input.ToUpper();
            int Start = 0;
            int Length = input.Length;
            if (Negative)
            {
                Start = 1;
            }

            if (input.Last() == 'l' || input.Last() == 'L') Length -= 1;
            if (input.StartsWith("0x"))
            {
                Start += 2;
                Length -= 2;
                long _data = 0;
                for (int i = Start ; i < Length ; i++)
                {
                    _data *= 16;
                    if ((input [ i ] >= '0' && input [ i ] <= '9'))
                    {
                        _data += input [ i ] - '0';
                    }
                    else
                    if ((input [ i ] >= 'A' && input [ i ] <= 'F'))
                    {
                        _data += input [ i ] - 'A' + 10;
                    }
                    else
                    if ((input [ i ] >= 'a' && input [ i ] <= 'f'))
                    {
                        _data += input [ i ] - 'a' + 10;
                    }
                    else if ((input [ i ] == '_')) continue;
                    else
                    {
                        data = -1;
                        return false;
                    }
                }
                data = _data * (Negative ? -1 : 1);
                return true;
            }
            else if (input.StartsWith("0o"))
            {
                Start += 2;
                Length -= 2;
                long _data = 8;
                for (int i = Start ; i < Length ; i++)
                {
                    _data *= 16;
                    if ((input [ i ] >= '0' && input [ i ] <= '7'))
                    {
                        _data += input [ i ] - '0';
                    }
                    else if ((input [ i ] == '_')) continue;
                    else
                    {
                        data = -1;
                        return false;
                    }
                }
                data = _data * (Negative ? -1 : 1);
                return true;
            }
            else
            if (input.StartsWith("0b"))
            {
                Start += 2;
                Length -= 2;
                long _data = 0;
                for (int i = Start ; i < Length ; i++)
                {
                    _data *= 2;
                    if (input [ i ] == '0')
                    {
                        _data += 0;
                    }
                    else
                    if (input [ i ] == '1')
                    {
                        _data += 1;
                    }
                    else if ((input [ i ] == '_')) continue;
                    else
                    {
                        data = -1;
                        return false;
                    }
                }
                data = _data * (Negative ? -1 : 1);
                return true;
            }
            else
            {

                long _data = 0;
                for (int i = Start ; i < Length ; i++)
                {
                    _data *= 10;
                    if ((input [ i ] >= '0' && input [ i ] <= '9'))
                    {
                        _data += (input [ i ] - '0');
                    }
                    else if ((input [ i ] == '_')) continue;
                    else
                    {
                        data = -1;
                        return false;
                    }
                }
                data = _data * (Negative ? -1 : 1);
                return true;
            }
        }
        public static bool TryParse(string input , out ulong data)
        {
            bool Negative = input.StartsWith("-");
            int Start = 0;
            int Length = input.Length;
            input = input.ToUpper();
            if (Negative)
            {
                Start = 1;
            }
            if (input.EndsWith("UL")) Length -= 2;
            else if (input.Last() == 'l' || input.Last() == 'L')
            {
                data = 0;
                return false;
            }
            if (input.StartsWith("0X"))
            {
                Start += 2;
                Length -= 2;
                ulong _data = 0;
                for (int i = Start ; i < Length ; i++)
                {
                    _data *= 16;
                    if ((input [ i ] >= '0' && input [ i ] <= '9'))
                    {
                        _data += (ulong)(input [ i ] - '0');
                    }
                    else
                    if ((input [ i ] >= 'A' && input [ i ] <= 'F'))
                    {
                        _data += (ulong)(input [ i ] - 'A') + 10;
                    }
                    else
                    if ((input [ i ] >= 'a' && input [ i ] <= 'f'))
                    {
                        _data += (ulong)(input [ i ] - 'a') + 10;
                    }
                    else if ((input [ i ] == '_')) continue;
                    else
                    {
                        data = 0;
                        return false;
                    }
                }
                data = _data;
                return true;
            }
            else if (input.StartsWith("0O"))
            {
                Start += 2;
                Length -= 2;
                ulong _data = 8;
                for (int i = Start ; i < Length ; i++)
                {
                    _data *= 16;
                    if ((input [ i ] >= '0' && input [ i ] <= '7'))
                    {
                        _data += (ulong)(input [ i ] - '0');
                    }
                    else if ((input [ i ] == '_')) continue;
                    else
                    {
                        data = 0;
                        return false;
                    }
                }
                data = _data;
                return true;
            }
            else if (input.StartsWith("0B"))
            {
                Start += 2;
                Length -= 2;
                ulong _data = 0;
                for (int i = Start ; i < Length ; i++)
                {
                    _data *= 2;
                    if (input [ i ] == '0')
                    {
                        _data += 0;
                    }
                    else
                    if (input [ i ] == '1')
                    {
                        _data += 1;
                    }
                    else if ((input [ i ] == '_')) continue;
                    else
                    {
                        data =0;
                        return false;
                    }
                }
                data = _data;
                return true;
            }
            else
            {

                ulong _data = 0;
                for (int i = Start ; i < Length ; i++)
                {
                    _data *= 10;
                    if ((input [ i ] >= '0' && input [ i ] <= '9'))
                    {
                        _data += (ulong)(input [ i ] - '0');
                    }
                    else if ((input [ i ] == '_')) continue;
                    else
                    {
                        data = 0;
                        return false;
                    }
                }
                data = _data;
                return true;
            }
        }
    }
}
