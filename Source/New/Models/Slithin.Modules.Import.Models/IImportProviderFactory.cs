﻿namespace Slithin.Modules.Import.Models;

public interface IImportProviderFactory
{
    IImportProvider? GetImportProvider(string baseExtension, string filename);

    void Init();
}
