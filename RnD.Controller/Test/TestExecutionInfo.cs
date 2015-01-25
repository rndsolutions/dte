using System.Collections.Generic;

namespace RnD.Controller.Test
{
    public class TestExecutionInfo
    {
        public string AssemblyName { get; set; }
        public List<string> IncludeCategories { get; set; }
        public List<string> ExcludeCategories { get; set; }
        public List<string> Namespaces { get; set; }
        public List<string> TestClases { get; set; }
        public List<string> TestMethods { get; set; }
    }
}
