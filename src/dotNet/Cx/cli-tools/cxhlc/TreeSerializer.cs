using Cx.Core;
using Cx.Core.VCParser;
using LibCLCC.NET.IO;
using LibCLCC.NET.TextProcessing;
using System.Linq;
using System.Text.Json.Serialization;
using xCVM.Core.Utilities;
using Cx.Core.Utilities;
namespace cxhlc
{
    public static class TreeSerializer
    {
        public static void Serialize(Stream stream , TreeNode Root)
        {
            var _obj = DiscreteObject.DiscreteNode(Root);
            {
                int len = _obj.SerializedIndices.Count;
                var len_b = BitConverter.GetBytes(len);
                stream.Write(len_b);
            }
            {
                foreach (var item in _obj.SerializedIndices)
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
#if DEBUG
                        Console.WriteLine("Serialize Index:");
#endif
                        {

#if DEBUG
                            Console.WriteLine($"\tID:{item.ID}");
#endif
                            var ID_B = BitConverter.GetBytes(item.ID);
                            stream.Write(ID_B);
                        }
                        {
#if DEBUG
                            Console.WriteLine($"\tPID:{item.ParentID}");
#endif
                            var ID_B = BitConverter.GetBytes(item.ParentID);
                            stream.Write(ID_B);
                        }
                        {
#if DEBUG
                            Console.WriteLine($"\tChildren Count:{item.Children.Count}");
#endif
                            var ID_B = BitConverter.GetBytes(item.Children.Count);
                            stream.Write(ID_B);
                        }
                        foreach (var _id in item.Children)
                        {
                            {
#if DEBUG
                                Console.WriteLine($"\tCH ID:{_id}");
#endif
                                var ID_B = BitConverter.GetBytes(_id);
                                stream.Write(ID_B);
                            }
                        }
                        ms.Flush();
                        var b = ms.GetBuffer();
                        stream.WriteBytes(b);
                    }
                }
            }
            {
                foreach (var item in _obj.SerializedNodes)
                {
                    using (MemoryStream ms = new MemoryStream())
                    {

                        {
                            var ID_B = BitConverter.GetBytes(item.ID);
                            stream.Write(ID_B);
                        }
                        var node = item.Node;
                        if (node == null)
                            continue;
                        {
                            var ID_B = BitConverter.GetBytes(node.Type);
                            stream.Write(ID_B);
                        }

                        {
                            stream.WriteSegment(node.Segment);
                        }
                        ms.Flush();
                        var b = ms.GetBuffer();
                        stream.WriteBytes(b);
                    }
                }
            }
        }
    }
    public class DiscreteObject
    {
        public List<SerializedIndex> SerializedIndices = new List<SerializedIndex>();
        public List<SerializedNode> SerializedNodes = new List<SerializedNode>();
#if DEBUG
        public static void Process(TreeNode node , DiscreteObject discreteObject , int Depth)
#else
        public static void Process(TreeNode node , DiscreteObject discreteObject)
#endif
        {
            SerializedNode s_node = new SerializedNode();
            s_node.ID = discreteObject.SerializedNodes.Count;
            s_node.Node = node;
            discreteObject.SerializedNodes.Add(s_node);
#if DEBUG
            for (int i = 0 ; i < Depth ; i++)
            {
                Console.Write("\t");
            }
            Console.WriteLine("\x1b[92m[Tree Node]\x1b[0m");
#endif

#if DEBUG
            for (int i = 0 ; i < Depth ; i++)
            {
                Console.Write("\t");
            }
            Console.WriteLine($" -ID:{s_node.ID}");
#endif
#if DEBUG
            for (int i = 0 ; i < Depth ; i++)
            {
                Console.Write("\t");
            }
            Console.WriteLine($" -Type:{node.Type}");
#endif
#if DEBUG
            for (int i = 0 ; i < Depth ; i++)
            {
                Console.Write("\t");
            }
            Console.WriteLine($" -Segment:{node.Segment?.content ?? "\x1b[93mnull\x1b[0m"}");
#endif
#if DEBUG
            if (node.Children.Count > 0)
            {
                for (int i = 0 ; i < Depth ; i++)
                {
                    Console.Write("\t");
                }
                Console.WriteLine($" -Children:");
            }
            else
            {
                for (int i = 0 ; i < Depth ; i++)
                {
                    Console.Write("\t");
                }
                Console.WriteLine($"\x1b[94m=Child End Point\x1b[0m");
            }
#endif
            foreach (var item in node.Children)
            {
#if DEBUG
                Process(item , discreteObject , Depth + 1);
#else
                Process(item , discreteObject);
#endif
            }
        }
        public static void BuildIndex(SerializedNode node , DiscreteObject discreteObject)
        {
            SerializedIndex serializedIndex = new SerializedIndex();
            if (node.Node == null)
            {
                return;
            }
            serializedIndex.ID = node.ID;
            foreach (var item in discreteObject.SerializedNodes)
            {
                if (item.Node == node.Node.Parent)
                {
                    serializedIndex.ParentID = item.ID;
                    break;
                }
            }
            foreach (var item in node.Node.Children)
            {

                foreach (var _item in discreteObject.SerializedNodes)
                {
                    if (_item.Node == item)
                    {
                        serializedIndex.ParentID = _item.ID;
                        BuildIndex(_item , discreteObject);
                        break;
                    }
                }
            }
            discreteObject.SerializedIndices.Add(serializedIndex);

        }
        public static DiscreteObject DiscreteNode(TreeNode node)
        {
            DiscreteObject serializedObject = new DiscreteObject();
#if DEBUG
            Process(node , serializedObject , 0);
#else
            Process(node , serializedObject);
#endif
            BuildIndex(serializedObject.SerializedNodes [ 0 ] , serializedObject);
            return serializedObject;
        }
    }
    public class SerializedNode
    {
        public int ID;
        public TreeNode? Node;
    }
    public class SerializedIndex
    {
#if DEBUG
        public int Depth;
#endif
        public int ID;
        public int ParentID = -1;
        public List<int> Children = new List<int>();
    }
}