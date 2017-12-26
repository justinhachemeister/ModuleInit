﻿using System.Collections.Generic;
using System.Linq;
using Fody;

public class ModuleWeaver : BaseModuleWeaver
{
    public override void Execute()
    {
        var finder = new InitializeMethodFinder
        {
            ModuleWeaver = this
        };
        finder.Execute();
        var importer = new ModuleLoaderImporter
        {
            InitializeMethodFinder = finder,
            ModuleWeaver = this,
            TypeSystem = ModuleDefinition.TypeSystem
        };
        importer.Execute();
        CleanReferences();
    }

    public override IEnumerable<string> GetAssembliesForScanning()
    {
        return Enumerable.Empty<string>();
    }

    public void CleanReferences()
    {
        var referenceToRemove = ModuleDefinition.AssemblyReferences.FirstOrDefault(x => x.Name == "ModuleInit");
        if (referenceToRemove == null)
        {
            LogDebug("\tNo reference to 'ModuleInit' found. References not modified.");
            return;
        }

        ModuleDefinition.AssemblyReferences.Remove(referenceToRemove);
        LogInfo("\tRemoving reference to 'ModuleInit'.");
    }
}