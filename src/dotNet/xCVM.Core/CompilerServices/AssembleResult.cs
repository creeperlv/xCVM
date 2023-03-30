using LibCLCC.NET.Collections;

namespace xCVM.Core.CompilerServices
{
    public class AssembleResult
    {
        public xCVMModule module;

        public AssembleResult(xCVMModule module)
        {
            this.module = module;
        }
        public void AddError(AssemblerError error) => Errors.Add(error);
        public ConnectableList<AssemblerError> Errors=new ConnectableList<AssemblerError>();
    }
}
