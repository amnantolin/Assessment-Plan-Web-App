﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DROP.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class projectEntities : DbContext
    {
        public projectEntities()
            : base("name=projectEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<user> users { get; set; }
        public virtual DbSet<usertype> usertypes { get; set; }
        public virtual DbSet<at> ats { get; set; }
        public virtual DbSet<outcome> outcomes { get; set; }
        public virtual DbSet<pi> pis { get; set; }
        public virtual DbSet<process> processes { get; set; }
        public virtual DbSet<so> soes { get; set; }
        public virtual DbSet<course> courses { get; set; }
        public virtual DbSet<copiatt> copiatts { get; set; }
        public virtual DbSet<atcase> atcases { get; set; }
    }
}
