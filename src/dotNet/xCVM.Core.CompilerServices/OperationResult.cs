using LibCLCC.NET.Collections;
using LibCLCC.NET.TextProcessing;

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
        public void AddWarning(OperationWarn warn) => Warnings.Add(warn);
        public ConnectableList<OperationWarn> Warnings = new ConnectableList<OperationWarn>();
        public bool CheckAndInheritAbnormalities<V>(OperationResult<V> Old)
        {
            Errors.ConnectAfterEnd(Old.Errors);
            Warnings.ConnectAfterEnd(Old.Warnings);
            return Errors.Count > 0;
        }
        public void InheritAbnormalities<V>(OperationResult<V> Old)
        {
            Errors.ConnectAfterEnd(Old.Errors);
            Warnings.ConnectAfterEnd(Old.Warnings);
        }
        public static implicit operator T(OperationResult<T> result) => result.Result;
        public static implicit operator OperationResult<T>(T d) => new OperationResult<T>(d);
    }
    public class OperationWarn
    {

        public Segment? Segment;

        public OperationWarn(Segment? binded , string? msg = "")
        {
            Segment = binded;
            _msg = msg;
        }

        string? _msg;
        public virtual string? Message { get => _msg; }
        public override string ToString()
        {
            return Message ?? "";
        }
    }
}
