using xCVM.Core;

namespace xCVM.VM
{
    internal class Program
    {
        static void Main(string [ ] args)
        {
            if(args.Length == 0)
            {

            }
            else
            {
                xCVMModule module;
                using (var s=File.OpenRead(args [ 0 ]))
                {
                    module = xCVMModule.FromStream(s);
                }
                xCVMCore core = new xCVMCore(new xCVMOption() { RegisterSize = Constants.int_size, RegisterCount= 16 } ,null);
                core.Load(module);
                core.Run();
            }
        }
    }
}