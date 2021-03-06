﻿using System.IO;
using Calamari.Integration.FileSystem;
using Calamari.Integration.Processes;

namespace Calamari.Integration.Scripting.Bash
{
    public class BashScriptEngine : IScriptEngine
    {
        public ScriptType[] GetSupportedTypes()
        {
            return new[] {ScriptType.Bash};
        }

        public CommandResult Execute(Script script, CalamariVariableDictionary variables, ICommandLineRunner commandLineRunner)
        {
            var workingDirectory = Path.GetDirectoryName(script.File);
            var configurationFile = BashScriptBootstrapper.PrepareConfigurationFile(workingDirectory, variables);
            var boostrapFile = BashScriptBootstrapper.PrepareBootstrapFile(script, configurationFile, workingDirectory);

            using (new TemporaryFile(configurationFile))
            using (new TemporaryFile(boostrapFile))
            {
                return commandLineRunner.Execute(new CommandLineInvocation(
                    BashScriptBootstrapper.FindBashExecutable(),
                    BashScriptBootstrapper.FormatCommandArguments(boostrapFile), workingDirectory));
            }
        }
    }
}