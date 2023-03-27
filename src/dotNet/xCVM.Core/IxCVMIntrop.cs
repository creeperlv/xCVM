namespace xCVM.Core
{
    public interface IxCVMIntrop
    {
        xCVMObject Ctor();
        xCVMObject Ctor(params xCVMObject[] arguments);
        void SetData(xCVMObject THIS, xCVMObject Value);
        xCVMObject Call(string name, params xCVMObject[] arguments);
    }
}
