using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace FCS.Models;

public partial class FCSContext : DbContext
{
    public FCSContext()
    {
    }

    public FCSContext(DbContextOptions<FCSContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Application> Applications { get; set; }

    public virtual DbSet<Area> Areas { get; set; }

    public virtual DbSet<City> Cities { get; set; }

    public virtual DbSet<Code> Codes { get; set; }

    public virtual DbSet<EntryAttempt> EntryAttempts { get; set; }

    public virtual DbSet<FuelMeasure> FuelMeasures { get; set; }

    public virtual DbSet<FuelType> FuelTypes { get; set; }

    public virtual DbSet<GasStation> GasStations { get; set; }

    public virtual DbSet<GasStationEntry> GasStationEntries { get; set; }

    public virtual DbSet<GasStationFuelEntry> GasStationFuelEntries { get; set; }

    public virtual DbSet<GasStationSchedule> GasStationSchedules { get; set; }

    public virtual DbSet<Gender> Genders { get; set; }

    public virtual DbSet<Language> Languages { get; set; }

    public virtual DbSet<Menu> Menus { get; set; }

    public virtual DbSet<MenuCategory> MenuCategories { get; set; }

    public virtual DbSet<MenuType> MenuTypes { get; set; }

    public virtual DbSet<Organization> Organizations { get; set; }

    public virtual DbSet<PlateNumberCategory> PlateNumberCategories { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<RolesMenu> RolesMenus { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserLogo> UserLogos { get; set; }

    public virtual DbSet<UserOrganization> UserOrganizations { get; set; }

    public virtual DbSet<UserPassword> UserPasswords { get; set; }

    public virtual DbSet<UserPasswordReset> UserPasswordResets { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    public virtual DbSet<Vehicle> Vehicles { get; set; }

    public virtual DbSet<VehicleStatus> VehicleStatuses { get; set; }

    public virtual DbSet<VehicleType> VehicleTypes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("name=FCSConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Application>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Systems");

            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<Area>(entity =>
        {
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<City>(entity =>
        {
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsDeleted).HasDefaultValue(true);
        });

        modelBuilder.Entity<Code>(entity =>
        {
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<EntryAttempt>(entity =>
        {
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.GasStation).WithMany(p => p.EntryAttempts)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EntryAttempts_GasStations");

            entity.HasOne(d => d.User).WithMany(p => p.EntryAttempts)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EntryAttempts_Users");

            entity.HasOne(d => d.Vehicle).WithMany(p => p.EntryAttempts)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EntryAttempts_Vehicles");
        });

        modelBuilder.Entity<FuelMeasure>(entity =>
        {
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.ActionByNavigation).WithMany(p => p.FuelMeasureActionByNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FuelMeasures_Users");

            entity.HasOne(d => d.ApprovedByNavigation).WithMany(p => p.FuelMeasureApprovedByNavigations).HasConstraintName("FK_FuelMeasures_Users1");

            entity.HasOne(d => d.FuelType).WithMany(p => p.FuelMeasures)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FuelMeasures_FuelTypes");

            entity.HasOne(d => d.GasStation).WithMany(p => p.FuelMeasures)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FuelMeasures_GasStations");
        });

        modelBuilder.Entity<FuelType>(entity =>
        {
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<GasStation>(entity =>
        {
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.City).WithMany(p => p.GasStations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GasStations_Cities");
        });

        modelBuilder.Entity<GasStationEntry>(entity =>
        {
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.FuelType).WithMany(p => p.GasStationEntries)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GasStationEntries_FuelTypes");

            entity.HasOne(d => d.GasStation).WithMany(p => p.GasStationEntries)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GasStationEntries_GasStations");

            entity.HasOne(d => d.User).WithMany(p => p.GasStationEntries)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GasStationEntries_Users");

            entity.HasOne(d => d.Vehicle).WithMany(p => p.GasStationEntries)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GasStationEntries_Vehicles");
        });

        modelBuilder.Entity<GasStationFuelEntry>(entity =>
        {
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.ActionByNavigation).WithMany(p => p.GasStationFuelEntries)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GasStationFuelEntries_Users");

            entity.HasOne(d => d.FuelType).WithMany(p => p.GasStationFuelEntries)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GasStationFuelEntries_FuelTypes");

            entity.HasOne(d => d.GasStation).WithMany(p => p.GasStationFuelEntries)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GasStationFuelEntries_GasStations");
        });

        modelBuilder.Entity<GasStationSchedule>(entity =>
        {
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.ActionByNavigation).WithMany(p => p.GasStationSchedules)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GasStationSchedules_Users");

            entity.HasOne(d => d.City).WithMany(p => p.GasStationSchedules)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GasStationSchedules_Cities");

            entity.HasOne(d => d.FuelType).WithMany(p => p.GasStationSchedules)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GasStationSchedules_FuelTypes");

            entity.HasOne(d => d.GasStation).WithMany(p => p.GasStationSchedules)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GasStationSchedules_GasStations");

            entity.HasOne(d => d.PlateNumberCategory).WithMany(p => p.GasStationSchedules)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GasStationSchedules_PlateNumberCategories");

            entity.HasOne(d => d.VehicleType).WithMany(p => p.GasStationSchedules)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GasStationSchedules_VehicleTypes");
        });

        modelBuilder.Entity<Gender>(entity =>
        {
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<Language>(entity =>
        {
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<Menu>(entity =>
        {
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.MenuCategory).WithMany(p => p.Menus)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Menus_MenuCategory");
        });

        modelBuilder.Entity<MenuCategory>(entity =>
        {
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.Application).WithMany(p => p.MenuCategories).HasConstraintName("FK_MenuCategory_Applications");

            entity.HasOne(d => d.MenuType).WithMany(p => p.MenuCategories)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MenuCategory_MenuTypes");

            entity.HasOne(d => d.Parent).WithMany(p => p.InverseParent).HasConstraintName("FK_MenuCategory_MenuCategory");
        });

        modelBuilder.Entity<MenuType>(entity =>
        {
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<Organization>(entity =>
        {
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<PlateNumberCategory>(entity =>
        {
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<RolesMenu>(entity =>
        {
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.Menu).WithMany(p => p.RolesMenus).HasConstraintName("FK_RolesMenus_Menus");

            entity.HasOne(d => d.Role).WithMany(p => p.RolesMenus).HasConstraintName("FK_RolesMenus_Roles");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.Gender).WithMany(p => p.Users)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Users_Genders");

            entity.HasOne(d => d.Language).WithMany(p => p.Users)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Users_Languages");
        });

        modelBuilder.Entity<UserLogo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_UserLogons");

            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.User).WithMany(p => p.UserLogos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserLogos_Users");
        });

        modelBuilder.Entity<UserOrganization>(entity =>
        {
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.ActionByNavigation).WithMany(p => p.UserOrganizationActionByNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserOrganizations_Users1");

            entity.HasOne(d => d.Organization).WithMany(p => p.UserOrganizations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserOrganizations_UserOrganizations");

            entity.HasOne(d => d.User).WithMany(p => p.UserOrganizationUsers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserOrganizations_Users");
        });

        modelBuilder.Entity<UserPassword>(entity =>
        {
            entity.HasOne(d => d.User).WithMany(p => p.UserPasswords)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserPasswords_Users");
        });

        modelBuilder.Entity<UserPasswordReset>(entity =>
        {
            entity.HasOne(d => d.User).WithMany(p => p.UserPasswordResets)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserPasswordResets_Users");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.Role).WithMany(p => p.UserRoles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserRoles_Roles");

            entity.HasOne(d => d.User).WithMany(p => p.UserRoles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserRoles_Users");
        });

        modelBuilder.Entity<Vehicle>(entity =>
        {
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.Area).WithMany(p => p.Vehicles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Vehicles_Areas");

            entity.HasOne(d => d.City).WithMany(p => p.Vehicles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Vehicles_Cities");

            entity.HasOne(d => d.Code).WithMany(p => p.Vehicles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Vehicles_Codes");

            entity.HasOne(d => d.FuelType).WithMany(p => p.Vehicles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Vehicles_FuelTypes");

            entity.HasOne(d => d.VehicleStatus).WithMany(p => p.Vehicles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Vehicles_VehicleStatuses");

            entity.HasOne(d => d.VehicleType).WithMany(p => p.Vehicles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Vehicles_VehicleTypes");
        });

        modelBuilder.Entity<VehicleStatus>(entity =>
        {
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<VehicleType>(entity =>
        {
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.Code).WithMany(p => p.VehicleTypes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_VehicleTypes_Codes");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
