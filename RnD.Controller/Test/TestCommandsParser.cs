
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;


namespace RnD.Controller.Test
{
    public class TestCommandsParser
    {
        /// <summary>
        /// Read the arguments meant for nunit-console. 
        /// </summary>
        /// <param name="arguments"></param>
        public static TestExecutionInfo ParseNunitCommands(string[] arguments)
        {
            var testExecutionInfo = new TestExecutionInfo();
            var agregatedComands = new StringBuilder();

            foreach (var argument in arguments)
            {
                if (argument.EndsWith(".dll"))
                {
                    testExecutionInfo.AssemblyNames.Add(argument);
                }
                else if (argument.StartsWith("/run:"))
                {
                    testExecutionInfo.RunItems = argument.Substring(5).Split(',').ToList();
                }
                else if (argument.StartsWith("/include:"))
                {
                    testExecutionInfo.IncludeCategories = argument.Substring(5).Split(',').ToList();
                }
                else if (argument.StartsWith("/exclude:"))
                {
                    testExecutionInfo.ExcludeCategories = argument.Substring(5).Split(',').ToList();
                }
                else
                {
                    agregatedComands.Append(argument + " ");
                }

            }

            if (agregatedComands.Length > 0)
            {
                //Remove the last empty space
                agregatedComands.Remove(agregatedComands.Length - 1, 1);

                testExecutionInfo.AgregatedArguments = agregatedComands.ToString();
            }

            return testExecutionInfo;
        }
    }


    /// <summary>
    /// Represents a wildcard running on the
    /// <see cref="System.Text.RegularExpressions"/> engine.
    /// </summary>
    public class Wildcard : Regex
    {
        private static char[] validWildcardChars = new char[] { '*', '#', '?' };

        /// <summary>
        /// Initializes a wildcard with the given search pattern.
        /// </summary>
        /// <param name="pattern">The wildcard pattern to match.</param>
        public Wildcard(string pattern)
            : base(WildcardToRegex(pattern))
        {
        }

        /// <summary>
        /// Initializes a wildcard with the given search pattern and options.
        /// </summary>
        /// <param name="pattern">The wildcard pattern to match.</param>
        /// <param name="options">A combination of one or more
        /// <see cref="System.Text.RegexOptions"/>.</param>
        public Wildcard(string pattern, RegexOptions options)
            : base(WildcardToRegex(pattern), options)
        {
        }

        /// <summary>
        /// Converts a wildcard to a regex.
        /// </summary>
        /// <param name="pattern">The wildcard pattern to convert.</param>
        /// <returns>A regex equivalent of the given wildcard.</returns>
        public static string WildcardToRegex(string pattern)
        {
            if (pattern.IndexOfAny(validWildcardChars) != -1)
                return "^" + Regex.Escape(pattern).Replace("\\*", ".*").Replace("\\!", "^").Replace("\\?", ".").Replace("\\#", "[0-9]") + "$";

            pattern = HandleSquareBrackets(pattern);


            return pattern;
        }

        private static string HandleSquareBrackets(string pattern)
        {
            var result = new StringBuilder();
            var startFlag = false;
            var endFlag = false;
            var containsSquareBrackets = false;

            for (int i = 0; i < pattern.Length; i++)
            {
                result.Append(pattern[i]);

                if (pattern[i] == '[')
                    startFlag = true;

                if (pattern[i] == ']' && startFlag)
                    endFlag = true;

                if (startFlag && endFlag)
                {
                    containsSquareBrackets = true;

                    if (i + 1 == pattern.Length)
                    {
                        result.Append('$');
                    }
                    else
                    {
                        startFlag = false;
                        endFlag = false;
                    }
                }
            }

            if (result.Length == pattern.Length && containsSquareBrackets)
                result.Append('$');

            if (containsSquareBrackets)
                return result.ToString().Replace("!", "^");

            return pattern;
        }
    }
}