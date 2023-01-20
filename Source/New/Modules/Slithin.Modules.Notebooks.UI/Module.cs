﻿using AuroraModularis.Core;
using Slithin.Core.FeatureToggle;
using Slithin.Modules.Notebooks.UI.ViewModels;

namespace Slithin.Modules.Notebooks.UI;

[Priority(ModulePriority.Max)]
internal class Module : AuroraModularis.Module
{
    public override Task OnStart(Container container)
    {
        return Task.CompletedTask;
    }

    public override void OnInit()
    {
        Features.Collect();
        Features.EnableAll();
    }

    public override void RegisterServices(Container container)
    {
        container.Register<NotebooksPageViewModel>();
    }
}
