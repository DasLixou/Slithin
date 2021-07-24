﻿using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Slithin.Core.Remarkable;

namespace Slithin.UI
{
    public class NotebookDataTemplate : IDataTemplate
    {
        public IControl Build(object param)
        {
            var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();

            var container = new StackPanel();

            var img = new Image();
            img.MinWidth = 25;
            img.MinHeight = 25;

            if (param is Metadata md && md.Type == MetadataType.DocumentType)
            {
                img.Source = new Bitmap(assets.Open(new Uri("avares://Slithin/Resources/image.png")));
            }
            else if (param is Metadata md2 && md2.Type == MetadataType.CollectionType)
            {
                img.Source = new Bitmap(assets.Open(new Uri("avares://Slithin/Resources/folder.png")));
            }

            container.Children.Add(img);

            var title = new TextBlock
            {
                [!TextBlock.TextProperty] = new Binding("VisibleName")
            };
            title.TextAlignment = Avalonia.Media.TextAlignment.Center;
            title.TextWrapping = Avalonia.Media.TextWrapping.Wrap;

            container.Children.Add(title);

            return container;
        }

        public bool Match(object data)
        {
            return data is Metadata md;
        }
    }
}