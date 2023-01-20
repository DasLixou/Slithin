﻿using Slithin.Core;
using Slithin.Modules.Tools.Models;

namespace Slithin.Modules.Tools;

internal class ToolInvokerServiceImpl : IToolInvokerService
{
    private static readonly Dictionary<string, ITool> _tools = new();

    public Dictionary<string, ITool> Tools => _tools;

    public void Init()
    {
        foreach (var tool in Utils.Find<ITool>())
        {
            Tools.Add(tool.Info.ID, tool);
        }
    }

    public void Invoke(string id, ToolProperties props)
    {
        if (Tools.TryGetValue(id, out var value))
        {
            value.Invoke(props);
        }
    }
}
