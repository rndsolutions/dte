using RnD.Business;
using RnD.Controller.Reflection;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace RnD.Controller.Test
{
    public class TestRunner
    {
        private List<string> _tests;

        private List<string> _notExecutedTests;
        public List<string> NotExecutedTests
        {
            get { return _notExecutedTests; }
        }

        private List<string> _executedTests;
        public List<string> ExecutedTests
        {
            get { return _executedTests; }
        }

        private List<string> _pendingTests;
        public List<string> PendingTests
        {
            get { return _pendingTests; }
        }


        public List<string> Tests
        {
            get { return _tests; }
        }

        public TestRunner()
        {
            var path = @"C:\Users\sdimitrov\Documents\Visual Studio 2013\Projects\TestAutomationMock\NunitUnitTestProject\bin\Debug";
            ReadTests(path, "NunitUnitTestProject.dll sdsdsd.dll /run:CalculatorLib.Tests");


        }

        public void ReadTests(string rootDirectory, string nunitExecutionArguments)
        {
            var nunutTestClassAttributeName = "TestFixtureAttribute";
            var nunutTestMethodAttributeName = "TestAttribute";

            var executionInfo = TestCommandsParser.ParseNunitCommands(nunitExecutionArguments.Split());

            List<MethodInfo> testMethods = new List<MethodInfo>();

            foreach (var file in Directory.GetFiles(rootDirectory, "*.dll"))
            {

                if (!executionInfo.AssemblyNames.Contains(Path.GetFileName(file)))
                    continue;

                var assembly = Assembly.LoadFrom(file);

                if (executionInfo.ShouldExecuteAll())
                {
                    testMethods.AddRange(ReflectionUtilities.GetAllTestMethods(assembly, nunutTestClassAttributeName, nunutTestMethodAttributeName));
                }
                else
                {
                    if (executionInfo.RunItems.Count > 0)
                    {
                        foreach (var item in executionInfo.RunItems)
                        {
                            var type = assembly.GetType(item, false);

                            if (type != null)
                            {
                                //If this is a test class search it for test methods
                                testMethods.AddRange(ReflectionUtilities.GetTestMethods(type, nunutTestClassAttributeName, nunutTestMethodAttributeName));
                            }
                            else
                            {
                                var itemParent = ReflectionUtilities.ReduceNamespace(item);
                                var parentType = assembly.GetType(item, false);

                                if (parentType != null)
                                {
                                    //The item was a method
                                    testMethods.AddRange(ReflectionUtilities.GetTestMethods(parentType, nunutTestClassAttributeName, nunutTestMethodAttributeName));
                                }
                                else
                                {
                                    //TODO: This is temp code. We must not re-query the assembly types multiple times
                                    testMethods.AddRange(ReflectionUtilities.GetTestMethodsFromNamespace(assembly, nunutTestClassAttributeName, nunutTestMethodAttributeName, item));
                                }
                            }
                        }
                    }
                }


                this._tests = testMethods.Select(x => x.ReflectedType.FullName + "." + x.Name).ToList();
                this._notExecutedTests = testMethods.Select(x => x.ReflectedType.FullName + "." + x.Name).ToList();
            }
        }

        public List<string> GetTestsForExecution(int bucketSize)
        {
            List<string> pendingTests = new List<string>();

            for (int i = 0; i < bucketSize; i++)
            {
                if (this._notExecutedTests.Count > 0)
                {
                    this._pendingTests.Add(this._notExecutedTests[0]);
                    this._notExecutedTests.RemoveAt(0);
                    pendingTests.Add(this._notExecutedTests[0]);
                }
            }

            return pendingTests;
        }

        public void MarkTestsAsExecuted(List<string> executedTests)
        {
            foreach (var test in executedTests)
            {
                if (this._pendingTests.Contains(test))
                {
                    this._pendingTests.Remove(test);
                    this._executedTests.Add(test);
                }
            }
        }

        public void MarkTestsAsNotExecuted(List<string> notExecutedTests)
        {
            foreach (var test in notExecutedTests)
            {
                if (this._pendingTests.Contains(test))
                {
                    this._pendingTests.Remove(test);
                    this._notExecutedTests.Add(test);
                }
            }
        }
    }

    public struct TestInfo
    {
        public string Name;
        public TestStatus Status;
    }
}
