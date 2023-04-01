using xCVM.Core;
using xCVM.Core.CompilerServices;

namespace xCVM.Compiler;
public class Program
{
    public static void Main(string[] args)
    {
        Arguments arguments = Arguments.FromStringArray(args);
        List<FileInfo> files = new List<FileInfo>();
        foreach (var item in arguments.Inputs)
        {
            files.Add(new FileInfo(item));
        }

        {
            xCVMAssembler xCVMAssembler = new xCVMAssembler();
            var result = xCVMAssembler.Assemble(files);
            if (result.Errors.Count > 0)
            {
                foreach (var err in result.Errors)
                {
                    Console.Write("Error:");
                    Console.WriteLine(err.Message);
                    Console.Write("\tAt:");
                    Console.Write(err.Segment.ID);
                    Console.Write(":");
                    if (err.Segment is not null)
                        Console.Write(err.Segment.LineNumber);
                    Console.WriteLine();
                }
            }
            else
            {
            }
        }
    }
}
public class Arguments
{
    public List<string> Inputs;
    public string Output;
    public bool IgnoreError;
    public static Arguments FromStringArray(string[] strings)
    {
        Arguments arguments = new Arguments();
        return arguments;
    }
}