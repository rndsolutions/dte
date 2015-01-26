using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RnD.Controller.Reflection
{
    public class ReflectionUtilities
    {
        public static List<MethodInfo> GetTestMethods(Type type, string testClassAttributeName, string testMethodAttributeName)
        {
            if (CheckIsTestClass(type, testClassAttributeName))
            {
                return GetTestMethods(type, testMethodAttributeName);

            }

            return new List<MethodInfo>();
        }

        public static List<MethodInfo> GetAllTestMethods(Assembly assembly, string testClassAttributeName, string testMethodAttributeName)
        {
            List<MethodInfo> testMethods = new List<MethodInfo>();
            Type[] types = assembly.GetTypes();

            foreach (var type in types)
            {
                testMethods.AddRange(GetTestMethods(type, testClassAttributeName, testMethodAttributeName));
            }

            return new List<MethodInfo>();
        }

        public static List<MethodInfo> GetAllTestMethods(Assembly assembly, string testClassAttributeName, string testMethodAttributeName, string categoryAttributeName, List<string> includeCategoreis, List<string> excludeCAtegories)
        {


            List<MethodInfo> testMethods = GetAllTestMethods(assembly, testClassAttributeName, testMethodAttributeName);

            foreach (var testMethod in testMethods)
            {
                var category = testMethod.CustomAttributes.FirstOrDefault(x => x.AttributeType.Name == categoryAttributeName);

                if (category != null)
                {

                }
            }

            return testMethods;
        }

        public static bool CheckIsTestClass(Type type, string testClassAttributeName)
        {
            return type.CustomAttributes.FirstOrDefault(x => x.AttributeType.Name == testClassAttributeName) != null;
        }

        public static List<MethodInfo> GetTestMethods(Type type, string testMethodAttributeName)
        {
            List<MethodInfo> testMethods = new List<MethodInfo>();

            foreach (var method in type.GetMethods())
            {
                var isTestMethod = method.CustomAttributes.FirstOrDefault(x => x.AttributeType.Name == testMethodAttributeName) != null;

                if (isTestMethod)
                    testMethods.Add(method);
            }

            return testMethods;
        }

        public static string ReduceNamespace(string input)
        {
            return input.Substring(0, input.LastIndexOf('.'));
        }


        public static List<MethodInfo> GetTestMethodsFromNamespace(Assembly assembly, string testClassAttributeName, string testMethodAttributeName, string namespaceString)
        {
            List<MethodInfo> testMethods = new List<MethodInfo>();
            Type[] types = assembly.GetTypes();

            foreach (var type in types)
            {
                if (type.Namespace == namespaceString)
                    testMethods.AddRange(GetTestMethods(type, testClassAttributeName, testMethodAttributeName));
            }

            return testMethods;
        }

    }
}
