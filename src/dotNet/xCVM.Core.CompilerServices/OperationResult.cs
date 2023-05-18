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
        public ConnectableList<OperationError> Errors = new ConnectableList<OperationError>();
        public static implicit operator T(OperationResult<T> result) => result.Result;
        public static implicit operator OperationResult<T>(T d) => new OperationResult<T>(d);
    }
}
