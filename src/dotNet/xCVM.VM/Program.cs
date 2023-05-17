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
                    module = xCVMModule.FromStream(s);
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
                core.Load(module);
                core.Run();
            }
        }
    }
}