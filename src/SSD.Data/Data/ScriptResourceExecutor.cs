using System;
using System.Data.Entity;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace SSD.Data
{
    public static class ScriptResourceExecutor
    {
        private static readonly Regex CommandRegularExpression = new Regex("^GO", RegexOptions.IgnoreCase | RegexOptions.Multiline);

        public static void ExecuteScript(Database database, string scriptResourceName)
        {
            string scriptFileContent = GetScript(scriptResourceName);
            ExecuteScriptCommands(database, scriptResourceName, scriptFileContent);
        }

        private static string GetScript(string scriptResourceName)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            using (Stream stream = assembly.GetManifestResourceStream(scriptResourceName))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        private static void ExecuteScriptCommands(Database database, string scriptResourceName, string scriptFileContent)
        {
            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "{0} - Beginning execution of '{1}'", DateTime.Now, scriptResourceName), "Information");
            foreach (string command in CommandRegularExpression.Split(scriptFileContent))
            {
                ExecuteCommand(database, command);
            }
            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "{0} - Finished executing '{1}'", DateTime.Now, scriptResourceName), "Information");
        }

        private static void ExecuteCommand(Database database, string command)
        {
            if (!string.IsNullOrWhiteSpace(command))
            {
                Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "{0} - Executing command:\r\n{1}", DateTime.Now, command), "Information");
                database.ExecuteSqlCommand(command);
            }
        }
    }
}
