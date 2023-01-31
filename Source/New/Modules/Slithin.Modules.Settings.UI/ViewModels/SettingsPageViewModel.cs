﻿using System.Windows.Input;
using AuroraModularis.Core;
using AuroraModularis.Logging.Models;
using Slithin.Core.MVVM;
using Slithin.Entities;
using Slithin.Modules.BaseServices.Models;
using Slithin.Modules.Repository.Models;
using Slithin.Modules.Settings.Models;
using Slithin.Modules.Settings.Models.Builder;

namespace Slithin.Modules.Settings.UI.ViewModels;

internal class SettingsPageViewModel : BaseViewModel
{
    private readonly LoginInfo _credential;
    private readonly ILogger _logger;
    private readonly ILoginService _loginService;
    private readonly IPathManager _pathManager;
    private readonly SettingsModel _settings;
    private readonly ISettingsService _settingsService;
    private object _settingsContent;

    public SettingsPageViewModel(ILoginService loginService,
        ISettingsService settingsService,
        IPathManager pathManager,
        ILogger logger)
    {
        _credential = loginService.GetCurrentCredential();
        _loginService = loginService;

        FeedbackCommand = new DelegateCommand(Feedback);

        _settingsService = settingsService;
        _pathManager = pathManager;
        _logger = logger;

        _settings = settingsService.GetSettings();

        _credential = _loginService.GetCurrentCredential();
    }
    
    public object SettingsContent
    {
        get => _settingsContent;
        set => SetValue(ref _settingsContent, value);
    }

    public ICommand FeedbackCommand { get; }

    public bool IsSSHLogin => _loginService.GetCurrentCredential().Key != null;

    private void Feedback(object obj)
    {
        var feedbackWindow = new FeedbackWindow();
        feedbackWindow.Show();
    }

    public override void OnLoad()
    {
        var settingsUiBuilder = ServiceContainer.Current.Resolve<ISettingsUiBuilder>();
        SettingsContent = settingsUiBuilder.Build();
    }
}
