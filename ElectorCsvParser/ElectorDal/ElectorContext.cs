using System;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace ElectorDal
{
    public class ElectorContext : DbContext
    {
        public virtual DbSet<City> Citys { get; set; }
        public virtual DbSet<Street> Streets { get; set; }
        public virtual DbSet<House> Houses { get; set; }
        public virtual DbSet<Flat> Flats { get; set; }
        public virtual DbSet<Problem> Problems { get; set; }
        public virtual DbSet<Area> Areas { get; set; }
        public virtual DbSet<AreaStreetHouse> AreaStreetHouses { get; set; }
    }
}
