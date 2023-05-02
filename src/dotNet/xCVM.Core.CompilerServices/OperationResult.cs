using LibCLCC.NET.Collections;

namespace xCVM.Core.CompilerServices
{
    public class OperationResult<T>
    {
        public T Result;

        public OperationResult(T module)
        {
            this.Result = module;
        }
        public void AddError(OperationError error) => Errors.Add(error);
        public ConnectableList<OperationError> Errors=new ConnectableList<OperationError>();
    }
}
