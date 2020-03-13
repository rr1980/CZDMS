using System;
using System.Collections.Generic;
using System.Text;

namespace CZDMS.Db.Entities
{
    public abstract class BaseEntity
    {
        public int Id { get; set; }
    }

    public partial class User : BaseEntity
    {
        /// <summary>
        /// The name of the User
        /// </summary>
        /// <example>Mustermann</example>
        public string Name { get; set; }
        public string Vorname { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public partial class FileItem : BaseEntity
    {
        public DateTime? LastWriteTime { get; set; }
        public string Name { get; set; }
        public string Key { get; set; }
        public int? ParentId { get; set; }
        public bool? IsFolder { get; set; }
        public byte[] Data { get; set; }
        public int? OptimisticLockField { get; set; }
        public int? Gcrecord { get; set; }
        public byte[] SsmaTimeStamp { get; set; }
    }
}
