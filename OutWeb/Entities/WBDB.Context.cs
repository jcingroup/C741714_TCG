﻿//------------------------------------------------------------------------------
// <auto-generated>
//     這個程式碼是由範本產生。
//
//     對這個檔案進行手動變更可能導致您的應用程式產生未預期的行為。
//     如果重新產生程式碼，將會覆寫對這個檔案的手動變更。
// </auto-generated>
//------------------------------------------------------------------------------

namespace OutWeb.Entities
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class WBDBEntities : DbContext
    {
        public WBDBEntities()
            : base("name=WBDBEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<WBAGENT> WBAGENT { get; set; }
        public virtual DbSet<WBLOGERR> WBLOGERR { get; set; }
        public virtual DbSet<WBNEWS> WBNEWS { get; set; }
        public virtual DbSet<WBPIC> WBPIC { get; set; }
        public virtual DbSet<WBPRODUCT> WBPRODUCT { get; set; }
        public virtual DbSet<WBUSR> WBUSR { get; set; }
        public virtual DbSet<WBWORKS> WBWORKS { get; set; }
        public virtual DbSet<WBPRODUCTTYPE> WBPRODUCTTYPE { get; set; }
    }
}
