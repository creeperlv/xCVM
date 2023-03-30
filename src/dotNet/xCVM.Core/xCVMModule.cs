namespace xCVM.Core
{
    public class xCVMModule
    {
        public ModuleMetadata? ModuleMetadata;
        public Dictionary<int, string> IDs=new Dictionary<int, string>();
        public Dictionary<int, string> Texts=new Dictionary<int, string>();
        public Dictionary<int, string> UsingFunctions = new Dictionary<int, string>();
        public List<Instruct> Instructions = new List<Instruct>();
    }
    [Serializable]
    public class ModuleMetadata
    {
        public string ModuleName;
        public string Publisher;
        public string Copyright;
        public Version ModuleVersion;
        public Version TargetVersion;
    }
}
