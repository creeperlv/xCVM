namespace xCVM.Core
{
    public class Constants
    {
        public static readonly int long_size = sizeof(long);
        public static readonly int short_size = sizeof(short);
        public static readonly int int_size = sizeof(int);
        public static readonly int float_size = sizeof(float);
        public static readonly int double_size = sizeof(double);
        public static readonly int MainStack = 5;
        //0 - CIL Object; 1 - int; 2 - long; 3 - float ; 4 - double, -1 - void, -2 - not used, 5 - unsigned int, 6 - unsigned long
        public static readonly int _int = 1;
        public static readonly int _long = 2;
        public static readonly int _float = 3;
        public static readonly int _double = 4;
        public static readonly int _void = -1;
        public static readonly int _not_used = -2;
        public static readonly int _uint = 5;
        public static readonly int _ulong = 6;
        public static readonly int _byte= 7;
        public static readonly int _short= 8;
        public static readonly int _ushort= 9;
    }
}
