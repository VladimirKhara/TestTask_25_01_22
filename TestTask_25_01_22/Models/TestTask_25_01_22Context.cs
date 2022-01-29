using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Collections.Generic;
using System.Linq;
#nullable disable

namespace TestTask_25_01_22.Models
{
    public partial class TestTask_25_01_22Context : DbContext
    {
        public TestTask_25_01_22Context()
        {
        }

        public TestTask_25_01_22Context(DbContextOptions<TestTask_25_01_22Context> options)
            : base(options)
        {
            try
            {
                Database.EnsureCreated();
            }
            catch
            {

            }
        }

        public virtual DbSet<SwitchDevice> SwitchDevices { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=TestTask_28_01_22;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<SwitchDevice>(entity =>
            {
                entity.HasKey(e => e.SerialNumber)
                    .HasName("PK__SwitchDe__048A0009B690495F");

                entity.Property(e => e.SerialNumber)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.Comment)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.InstallationDate).HasColumnType("date");

                entity.Property(e => e.InventoryNumber)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.Ipv4)
                    .IsRequired()
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("IPv4")
                    .IsFixedLength(true);

                entity.Property(e => e.Mac)
                    .IsRequired()
                    .HasMaxLength(17)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.MainVlan)
                    .IsRequired()
                    .HasMaxLength(17)
                    .IsUnicode(false)
                    .HasColumnName("MainVLAN")
                    .IsFixedLength(true);

                entity.Property(e => e.ModelName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.PurchaseDate).HasColumnType("date");
            });



            List<SwitchDevice> switchDevices = new List<SwitchDevice>();
            Random random = new Random();
            for (int i = 0; i < 10; i++)
            {
                switchDevices.Add(new SwitchDevice($"testName { i }", $"{random.Next(0, 256)}.{random.Next(0, 256)}.{random.Next(0, 256)}.{random.Next(0, 256)}",
                    $"{GenerateMac(random)}", $"{random.Next(0, 256)}.{random.Next(0, 256)}.{random.Next(0, 256)}.{random.Next(0, 256)}", $"{random.Next(0, 100000000)}",
                    $"{random.Next(0, 100000)}", GenerateRandomDate(random), GenerateRandomDate(random)));
            }

            modelBuilder.Entity<SwitchDevice>().HasData(switchDevices);
            OnModelCreatingPartial(modelBuilder);
        }

        private static string GenerateMac(Random random)
        {
            byte[] buffer = new byte[1];
            string mac = string.Empty;
            for (int i = 0; i < 6; i++)
            {
                random.NextBytes(buffer);
                mac = mac + string.Concat(buffer.Select(x => x.ToString("X2")).ToArray()) + ":";
            }
            return mac.Remove(mac.Length-1);
        }

        private DateTime GenerateRandomDate(Random random)
        {
            return DateTime.Today.AddDays(-random.Next(0, 3650));
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

    }
}
