using LibCLCC.NET.Collections;

namespace xCVM.Core.CompilerServices
{
    public class CompileResult<T>
    {
        public T Result;

        public CompileResult(T module)
        {
            this.Result = module;
        }
        public void AddError(CompileError error) => Errors.Add(error);
        public ConnectableList<CompileError> Errors=new ConnectableList<CompileError>();
    }
}
