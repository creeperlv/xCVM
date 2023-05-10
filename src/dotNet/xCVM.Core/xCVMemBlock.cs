using System;
using System.Collections.Generic;

namespace xCVM.Core
{
    public class xCVMemBlock
    {
        public Dictionary<int , xCVMem> Datas = new Dictionary<int , xCVMem>();
        public int PUT(byte [ ] data , int ForceKey = -1)
        {
            try
            {
                var mem = new xCVMem(data);
                var K = ForceKey == -1 ? mem.GetHashCode() : ForceKey;
                Datas.Add(K , mem);
                return K;

            }
            catch (Exception)
            {
                return -1;
            }
        }
        public int MALLOC(int Size , int ForceKey = -1)
        {
            try
            {
                var b = new byte [ Size ];
                Array.Fill<byte>(b , 0);
                var mem = new xCVMem(b);
                var K = ForceKey == -1 ? mem.GetHashCode() : ForceKey;
                Datas.Add(K , mem);
                return K;
            }
            catch (Exception)
            {
                return -1;
            }
        }
        public void FREE(int Key)
        {
            if (Datas.ContainsKey(Key))
            {
                Datas.Remove(Key);
            }
        }
        public int REALLOC(int Key , int NewSize , bool RightAligned)
        {
            try
            {
                if (!Datas.ContainsKey(Key)) return -1;
                var newD = new xCVMem(new byte [ NewSize ]);
                var old = new Span<byte>(Datas [ Key ].data);
                var L = Math.Min(old.Length , NewSize);
                var OL = old.Length - 1;
                if (RightAligned)
                {
                    for (int i = 0 ; i < L ; i++)
                    {
                        newD.data [ i ] = old [ OL - i ];
                    }
                }
                else
                {
                    for (int i = 0 ; i < L ; i++)
                    {
                        newD.data [ i ] = old [ i ];
                    }
                }
                return Key;
            }
            catch (Exception)
            {
                return -1;
            }
        }
    }
}
