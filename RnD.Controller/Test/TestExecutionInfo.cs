using System.Collections.Generic;

namespace RnD.Controller.Test
{
    public class TestExecutionInfo
    {
        public string OriginalArguments { get; set; }
        public string AgregatedArguments { get; set; }

        public List<string> AssemblyNames { get; set; }
        public List<string> IncludeCategories { get; set; }
        public List<string> ExcludeCategories { get; set; }
        public List<string> RunItems { get; set; }

        public TestExecutionInfo()
        {
            AssemblyNames = new List<string>();
            IncludeCategories = new List<string>();
            ExcludeCategories = new List<string>();
            RunItems = new List<string>();
        }

        public bool ShouldExecuteAll()
        {
            return RunItems.Count == 0 && IncludeCategories.Count == 0 && ExcludeCategories.Count == 0;
        }
    }
}
