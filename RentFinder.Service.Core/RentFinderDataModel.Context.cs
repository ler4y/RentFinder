﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace RentFinder.Service.Core
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class RentFinderEntities : DbContext
    {
        public RentFinderEntities()
            : base("name=RentFinderEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Ad> Ads { get; set; }
        public virtual DbSet<Area> Areas { get; set; }
        public virtual DbSet<City> Cities { get; set; }
        public virtual DbSet<PhoneNumber> PhoneNumbers { get; set; }
        public virtual DbSet<RealtyType> RealtyTypes { get; set; }
        public virtual DbSet<RentalType> RentalTypes { get; set; }
        public virtual DbSet<ActualAd> ActualAds { get; set; }
        public virtual DbSet<BlackNumbersView> BlackNumbersViews { get; set; }
    }
}
