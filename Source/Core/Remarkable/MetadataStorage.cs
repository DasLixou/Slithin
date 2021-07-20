﻿using System;
using System.Collections.Generic;

namespace Slithin.Core.Remarkable
{
    public static class MetadataStorage
    {
        private static readonly Dictionary<string?, Metadata?> _storage = new();

        public static void Add(Metadata metadata)
        {
            if (!_storage.ContainsKey(metadata.VisibleName))
            {
                _storage.Add(metadata.VisibleName, metadata);
            }
        }

        public static Metadata Get(string name)
        {
            return _storage[name];
        }

        public static IEnumerable<Metadata> GetByParent(string parent)
        {
            foreach (var item in _storage)
            {
                if (item.Value.Parent.Equals(parent))
                {
                    yield return item.Value;
                }
            }
        }

        public static IEnumerable<string?> GetNames()
        {
            return _storage.Keys;
        }
    }
}
