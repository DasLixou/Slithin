﻿using AuroraModularis.Core;
using Newtonsoft.Json;
using Slithin.Entities.Remarkable;
using Slithin.Modules.Device.Models;
using Slithin.Modules.Repository.Models;

namespace Slithin.Modules.Repository;

public class MetadataRepositoryImpl : IMetadataRepository
{
    private readonly Dictionary<string, Metadata> _storage = new();

    public Metadata Load(string id)
    {
        var pathManager = Container.Current.Resolve<IPathManager>();

        var mdObj = JsonConvert.DeserializeObject<Metadata>(
            File.ReadAllText(Path.Combine(pathManager.NotebooksDir, id + ".metadata")));
        mdObj!.ID = id;

        if (File.Exists(Path.Combine(pathManager.NotebooksDir, id + ".content")))
        {
            mdObj.Content = JsonConvert.DeserializeObject<ContentFile>(
                File.ReadAllText(Path.Combine(pathManager.NotebooksDir, id + ".content")));
        }

        if (File.Exists(Path.Combine(pathManager.NotebooksDir, id + ".pagedata")))
        {
            mdObj.PageData.Parse(File.ReadAllText(Path.Combine(pathManager.NotebooksDir, id + ".pagedata")));
        }
        else
        {
            var data = new[] { "Blank" };
            var pg = new PageData { Data = data };

            mdObj.PageData = pg;
        }

        return mdObj;
    }

    public void AddMetadata(Metadata metadata, out bool alreadyAdded)
    {
        if (_storage.ContainsKey(metadata.ID))
        {
            alreadyAdded = true;
            return;
        }

        _storage.Add(metadata.ID, metadata);
        alreadyAdded = false;
    }

    public void SaveToDisk(Metadata metadata)
    {
        var pathManager = Container.Current.Resolve<IPathManager>();

        File.WriteAllText(Path.Combine(pathManager.NotebooksDir, metadata.ID + ".metadata"),
            JsonConvert.SerializeObject(this, Formatting.Indented));

        File.WriteAllText(Path.Combine(pathManager.NotebooksDir, metadata.ID + ".content"),
            JsonConvert.SerializeObject(metadata.Content, Formatting.Indented));
    }

    public void Clear()
    {
        _storage.Clear();
    }

    public IEnumerable<Metadata> GetAll()
    {
        return _storage.Values;
    }

    public IEnumerable<Metadata> GetByParent(string parent)
    {
        var list = new List<Metadata>();

        foreach (var item in _storage)
        {
            if (item.Value.Parent is not null && item.Value.Parent.Equals(parent))
            {
                list.Add(item.Value);
            }
        }

        return list;
    }

    public Metadata GetMetadata(string id)
    {
        return _storage[id];
    }

    public IEnumerable<string> GetNames()
    {
        return _storage.Keys;
    }

    public void Move(Metadata md, string folder)
    {
        md.Parent = folder;
        md.Version++;

        _storage[md.ID] = md; //replace metadata with changed md

        SaveToDisk(md);

        Upload(md);

        var remarkableDevice = Container.Current.Resolve<IRemarkableDevice>();
        remarkableDevice.Reload();
    }

    public void Upload(Metadata md, bool onlyMetadata = false)
    {
        var scp = Container.Current.Resolve<IRemarkableDevice>();
        var notebooksDir = Container.Current.Resolve<IPathManager>().NotebooksDir;
        var pathList = Container.Current.Resolve<PathList>();

        scp.Upload(new FileInfo(Path.Combine(notebooksDir, md.ID + ".metadata")),
                                pathList.Documents + md.ID + ".metadata");

        if (md.Type == "DocumentType" &&
                                (md.Content.FileType == "pdf" || md.Content.FileType == "epub") && !onlyMetadata)
        {
            scp.Upload(new FileInfo(Path.Combine(notebooksDir, md.ID + "." + md.Content.FileType)),
                pathList.Documents + md.ID + "." + md.Content.FileType);
            scp.Upload(new FileInfo(Path.Combine(notebooksDir, md.ID + ".content")),
                pathList.Documents + md.ID + ".content");
        }
    }

    public void Remove(Metadata tmpl)
    {
        _storage.Remove(tmpl.ID);
    }
}