using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace RnD.Controller.Test
{
    public class TestRunner
    {
        private List<string> _tests;

        public TestRunner()
        {
            var path = @"C:\Users\sdimitrov\Documents\Visual Studio 2013\Projects\TestAutomationMock\NunitUnitTestProject\bin\Debug";
            ReadTests(path, "NunitUnitTestProject.dll");


        }

        //[SD] This method should return the methdInfo so categories attributes can be extracted later on [for  ms test there is enable disable attribute]
        //Refractor this in reflection helper or utils    
        public void ReadTests(string rootDirectory, string nunitExecutionArguments)
        {
            var nunutTestClassAttributeName = "TestFixtureAttribute";
            var nunutTestMethodAttributeName = "TestAttribute";

            var executionInfos = TestCommandsParser.ParseNunitCommands(nunitExecutionArguments);

            var testAssembliesNames = executionInfos.Select(x => x.AssemblyName);

            foreach (var file in Directory.GetFiles(rootDirectory, "*.dll"))
            {

                //if (assembly.GetReferencedAssemblies().FirstOrDefault(x => x.Name == "nunit.framework") == null)
                //    continue;

                if (!testAssembliesNames.Contains(Path.GetFileName(file)))
                    continue;

                var assembly = Assembly.LoadFrom(file);

                Type[] types = assembly.GetTypes();

                List<MethodInfo> testMethods = new List<MethodInfo>();

                foreach (var type in types)
                {
                    var isTestClass = type.CustomAttributes.FirstOrDefault(x => x.AttributeType.Name == nunutTestClassAttributeName) != null;

                    if (isTestClass)
                    {

                        foreach (var method in type.GetMethods())
                        {
                            var isTestMethod = method.CustomAttributes.FirstOrDefault(x => x.AttributeType.Name == nunutTestMethodAttributeName) != null;

                            if (isTestMethod)
                                testMethods.Add(method);
                        }
                    }

                }



                var testNames = testMethods.Select(x => x.ReflectedType.FullName + "." + x.Name).ToList();

            }
        }



    }
}
