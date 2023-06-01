using System.Collections;
using SystemCalls;
using xCVM.Core;

namespace xCVM.VM
{
    internal class Program
    {
        static void Main(string [ ] args)
        {
            if (args.Length == 0)
            {

            }
            else
            {
                xCVMModule module;
                using (var s = File.OpenRead(args [ 0 ]))
                {
                    var m = xCVMModule.FromStream(s);
                    if (m == null)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("error:");
                        Console.ResetColor();
                        Console.WriteLine("Cannot Load Module.");
                        Environment.Exit(1);
                    }
                    module = m;
                }

                xCVMCore core = new xCVMCore(new xCVMOption() { RegisterSize = Constants.int_size , RegisterCount = 16 } , null,null);
                {
                    //Setup Syscall
                    core.RegisterSysCall(3 , new read());
                    core.RegisterSysCall(4 , new write());
                    core.RegisterSysCall(5 , new open());
                    core.RegisterSysCall(6 , new close());
                    core.RegisterSysCall(19 , new lseek());
                    core.RegisterSysCall(118 , new fsync());
                    core.RegisterSysCall(122 , new uname());
                }
                {
                    //Setup Resources.
                    core.SetResource(0 , Console.OpenStandardInput());
                    core.SetResource(1 , Console.OpenStandardOutput());
                }
                {
                    var variables=Environment.GetEnvironmentVariables();
                    List<string> EnvVar=new List<string>();
                    foreach (DictionaryEntry item in variables)
                    {
                        EnvVar.Add($"{item.Key}={item.Value}");
                    }
                    core.SetEnvironmentVariable(EnvVar);
                }
                core.Load(module);
                core.Run();
            }
        }
    }
}