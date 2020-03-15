using DevExtreme.AspNet.Mvc.FileManagement;
using System;
using System.Collections.Generic;

namespace CZDMS.Models
{
    public class DbRechercheFileSystemItem
    {
        public long Id { get; set; }
        public object Key { get; set; }
        public string Name { get; set; }
        public DateTime DateModified { get; set; }
        public string Type { get; set; }
        public long Size { get; set; }
    }

    public class DbFileSystemItem : IClientFileSystemItem
    {
        public object Key { get; set; }
        public long ParentId { get; set; }
        public string Name { get; set; }
        public DateTime DateModified { get; set; }
        public bool IsDirectory { get; set; }
        public byte[] FileData { get; set; }
        public long Size { get; set; }
        public IDictionary<string, object> CustomFields { get; set; }
        public bool HasSubDirectories { get; set; }

        public DbFileSystemItem()
        {
            CustomFields = new Dictionary<string, object>();
        }
    }
}
