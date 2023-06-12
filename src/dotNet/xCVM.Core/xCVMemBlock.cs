using System;
using System.Collections.Generic;
using System.Linq;

namespace xCVM.Core
{
    public class xCVMemBlock
    {
        public long TotalMemory = 0;
        public Dictionary<int , xCVMem> Datas = new Dictionary<int , xCVMem>();
        public int PUT(byte [ ] data , int ForceKey = -1)
        {
            try
            {
                var mem = new xCVMem(data);
                var K = ForceKey == -1 ? mem.GetHashCode() : ForceKey;
                Datas.Add(K , mem);
                TotalMemory += data.Length;
                return K;

            }
            catch (Exception)
            {
                return -1;
            }
        }
        public void Push(int ID , byte [ ] data , int offset , int len)
        {
            Stack<byte> bytes = new Stack<byte>(Datas [ ID ].data);
            var L = len;
            for (int i = 0 ; i < L ; i++)
            {
                bytes.Push(data [ offset + i ]);
            }
            TotalMemory += data.Length;
            Datas [ ID ].data = bytes.ToArray();
        }
        public void Pop(int ID , int Len , bool IsStack , byte [ ] reciver , int Offset)
        {
            if (IsStack)
            {
                var span = Datas [ ID ].data.AsSpan();
                var poped = span.Slice(span.Length - Len , Len).ToArray();
                poped.CopyTo(reciver , Offset);
                Datas [ ID ].data = span.Slice(0 , span.Length - Len).ToArray();
                TotalMemory -= Len;
                //Stack<byte> bytes = new Stack<byte>(Datas [ ID ].data);
                //for (int i = 0 ; i < Len ; i++)
                //{
                //    reciver [ Offset +Len- i ] = bytes.Pop();
                //}
                //Datas [ ID ].data = bytes.ToArray();
            }
            else
            {
                Queue<byte> bytes = new Queue<byte>(Datas [ ID ].data);
                for (int i = 0 ; i < Len ; i++)
                {
                    reciver [ Offset + i ] = bytes.Dequeue();
                }
                TotalMemory -=Len;
                Datas [ ID ].data = bytes.ToArray();
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
                TotalMemory += Size;
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
                TotalMemory -= Datas [ Key ].data.Length;
                Datas.Remove(Key);
            }
        }
        public int REALLOC(int Key , int NewSize , bool RightAligned)
        {
            try
            {
                if (!Datas.ContainsKey(Key)) return -1;
                TotalMemory -= Datas [ Key ].data.Length;
                var newD = new xCVMem(new byte [ NewSize ]);
                var old = Datas [ Key ].data.AsSpan();

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
                TotalMemory +=NewSize;
                return Key;
            }
            catch (Exception)
            {
                return -1;
            }
        }
    }
}
