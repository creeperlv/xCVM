namespace xCVM.Core
{
    public class xCVMObject
    {
        /// <summary>
        /// 0 - CIL Object; 1 - int; 2 - long; 3 - float ; 4 - double, -1 - void, -2 - not used, 5 - unsigned int, unsigned long
        /// </summary>
        public int ObjType;
        public object Man;
        public byte[] Mem;
    }
}
