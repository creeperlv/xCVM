using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace xCVM.Core
{
    public class ModuleProvider
    {
        public List<DirectoryInfo> SearchDirectories = new List<DirectoryInfo>();
        public xCVMModule? LoadModule(string name)
        {
            foreach (var item in SearchDirectories)
            {
                var m_p=Path.Combine(item.FullName, name);
                if(File.Exists(m_p))
                {
                    using (var s=File.OpenRead(m_p))
                    {
                        var mmodule=xCVMModule.FromStream(s);
                        if (mmodule != null) return mmodule;
                    }
                }
            }
            return null;
        }
    }
}
