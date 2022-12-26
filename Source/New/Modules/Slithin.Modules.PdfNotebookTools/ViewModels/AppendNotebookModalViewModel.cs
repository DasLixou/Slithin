﻿using System.Collections.ObjectModel;
using System.Windows.Input;
using AuroraModularis.Core;
using PdfSharpCore;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf.IO;
using Slithin.Core.MVVM;
using Slithin.Entities.Remarkable;
using Slithin.Entities.Remarkable.Rendering;
using Slithin.Modules.I18N.Models;
using Slithin.Modules.PdfNotebookTools.Models;
using Slithin.Modules.Repository.Models;
using Slithin.Modules.UI.Models;

namespace Slithin.Modules.PdfNotebookTools.ViewModels;

public class AppendNotebookModalViewModel : ModalBaseViewModel
{
    private readonly ILoadingService _loadingService;
    private readonly IDialogService _dialogService;
    private readonly ILocalisationService _localisationService;
    private readonly IPathManager _pathManager;
    private readonly ITemplateStorage _templateStorage;
    private string _customTemplateFilename;
    private string _pageCount;
    private Template _selectedTemplate;
    private ObservableCollection<Template> _templates = new();

    public AppendNotebookModalViewModel(IPathManager pathManager, ITemplateStorage templateStorage,
        ILoadingService loadingService, IDialogService dialogService,
        ILocalisationService localisationService)
    {
        _pathManager = pathManager;
        _templateStorage = templateStorage;
        _loadingService = loadingService;
        _dialogService = dialogService;
        _localisationService = localisationService;
        AddPagesCommand = new DelegateCommand(AddPages);
        OKCommand = new DelegateCommand(OK);
    }

    public ICommand AddPagesCommand { get; set; }

    public string CustomTemplateFilename
    {
        get => _customTemplateFilename;
        set => SetValue(ref _customTemplateFilename, value);
    }

    public string ID
    {
        get;
        set;
    }

    public ICommand OKCommand { get; set; }

    public string PageCount
    {
        get => _pageCount;
        set => SetValue(ref _pageCount, value);
    }

    public ObservableCollection<object> Pages { get; set; } = new();

    public Template SelectedTemplate
    {
        get => _selectedTemplate;
        set => SetValue(ref _selectedTemplate, value);
    }

    public ObservableCollection<Template> Templates
    {
        get => _templates;
        set => SetValue(ref _templates, value);
    }

    public override void OnLoad()
    {
        base.OnLoad();

        Templates = new ObservableCollection<Template>(_templateStorage.Templates);
    }

    private async void AddPages(object obj)
    {
        if (int.TryParse(PageCount, out var pcount) &&
            (SelectedTemplate != null || !string.IsNullOrEmpty(CustomTemplateFilename)))
        {
            if (!string.IsNullOrEmpty(CustomTemplateFilename))
            {
                Pages.Add(new NotebookCustomPage(CustomTemplateFilename, pcount));
            }
            else
            {
                Pages.Add(new NotebookPage(SelectedTemplate, pcount));
            }

            SelectedTemplate = null;
            PageCount = null;
            CustomTemplateFilename = null;
        }
        else
        {
            await _dialogService.Show(
                _localisationService.GetString("Page Count must be a number and a template need to be selected"));
        }
    }

    private void OK(object obj)
    {
        /* var validationResult = _validator.Validate(this);

         if (!validationResult.IsValid)
         {
             DialogService.OpenError(validationResult.Errors.First().ToString());
             return;
         }
        */
        var mdStorage = Container.Current.Resolve<IMetadataRepository>();

        var document = PdfReader.Open(Path.Combine(_pathManager.NotebooksDir, ID + ".pdf"));
        var md = mdStorage.GetMetadata(ID);
        var pages = new List<string>(md.Content.Pages);
        var pageCount = md.Content.PageCount;

        foreach (var p in Pages)
        {
            XImage image = null;
            var count = 0;

            pageCount++;

            switch (p)
            {
                case NotebookPage nbp:
                    count = nbp.Count;
                    image = XImage.FromFile($"{_pathManager.TemplatesDir}\\{nbp.Template.Filename}.png");
                    break;

                case NotebookCustomPage nbcp:
                    image = XImage.FromFile(nbcp.Filename);
                    count = nbcp.Count;
                    break;
            }

            for (var i = 0; i < count; i++)
            {
                var page = document.AddPage();
                page.Size = PageSize.A4;

                var gfx = XGraphics.FromPdfPage(page);

                gfx.DrawImage(image, 0, 0, page.Width, page.Height);

                var pageID = Guid.NewGuid();
                pages.Add(pageID.ToString());
            }
        }

        var content = md.Content;
        content.PageCount = pageCount;
        content.Pages = pages.ToArray();

        md.Content = content;

        mdStorage.SaveToDisk(md);

        document.Save(_pathManager.NotebooksDir + $"\\{md.ID}.pdf");

        Notebook.UploadDocument(md);
    }
}
