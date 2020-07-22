using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using WebAPI.Models;

namespace WebAPI.DBContexts
{
    public partial class ZMDBContext : DbContext
    {
        public ZMDBContext()
        {
        }

        public ZMDBContext(DbContextOptions<DbContext> options)
            : base(options)
        {
        }

        // public virtual DbSet<IpWhiteList> IpWhiteList { get; set; }
        // public virtual DbSet<SysAction> SysAction { get; set; }
        // public virtual DbSet<SysPrivilege> SysPrivilege { get; set; }
        // public virtual DbSet<SysPrivilegeAction> SysPrivilegeAction { get; set; }
        // public virtual DbSet<SysRole> SysRole { get; set; }
        // public virtual DbSet<SysRolePrivilege> SysRolePrivilege { get; set; }
        // public virtual DbSet<SysUser> SysUser { get; set; }
        // public virtual DbSet<SysUserRole> SysUserRole { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("name=MyDB");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.6-servicing-10079");

            modelBuilder.Entity<IpWhiteList>(entity =>
            {
                entity.Property(e => e.IpwhiteListId)
                    .HasColumnName("IPWhiteListId")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatedOn)
                    .HasColumnName("createdOn")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ModifiedOn)
                    .HasColumnName("modifiedOn")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.UrlList)
                    .HasColumnName("urlList")
                    .HasMaxLength(255)
                    .IsUnicode(false);
                entity.Property(e => e.NameList)
                    .HasColumnName("nameList")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.VerCol)
                    .IsRequired()
                    .IsRowVersion();
            });

            modelBuilder.Entity<IpNameRef>(entity =>
            {
                entity.Property(e => e.IpNameRefId)
                    .HasColumnName("IpNameRefId")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatedOn)
                    .HasColumnName("createdOn")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ModifiedOn)
                    .HasColumnName("modifiedOn")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.StartIP)
                    .HasColumnName("StartIP")
                    .HasMaxLength(20)
                    .IsUnicode(false);
                entity.Property(e => e.EndIP)
                    .HasColumnName("EndIP")
                    .HasMaxLength(20)
                    .IsUnicode(false);
                entity.Property(e => e.Country)
                    .HasColumnName("Country")
                    .HasMaxLength(50)
                    .IsUnicode(false);
                entity.Property(e => e.Local)
                    .HasColumnName("Local")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.VerCol)
                    .IsRequired()
                    .IsRowVersion();
            });

            modelBuilder.Entity<SysAction>(entity =>
            {
                entity.Property(e => e.SysActionId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Code).HasColumnName("code");

                entity.Property(e => e.CreatedOn)
                    .HasColumnName("createdOn")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ModifiedOn)
                    .HasColumnName("modifiedOn")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ParentId).HasColumnName("parentId");

                entity.Property(e => e.VerCol)
                    .IsRequired()
                    .IsRowVersion();
            });

            modelBuilder.Entity<SysPrivilege>(entity =>
            {
                entity.Property(e => e.SysPrivilegeId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatedOn)
                    .HasColumnName("createdOn")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ModifiedOn)
                    .HasColumnName("modifiedOn")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.PrivilegeType).HasColumnName("privilegeType");

                entity.Property(e => e.VerCol)
                    .IsRequired()
                    .IsRowVersion();
            });

            modelBuilder.Entity<SysPrivilegeAction>(entity =>
            {
                entity.Property(e => e.SysPrivilegeActionId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatedOn)
                    .HasColumnName("createdOn")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ModifiedOn)
                    .HasColumnName("modifiedOn")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.VerCol)
                    .IsRequired()
                    .IsRowVersion();
            });

            modelBuilder.Entity<SysRole>(entity =>
            {
                entity.Property(e => e.SysRoleId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatedOn)
                    .HasColumnName("createdOn")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ModifiedOn)
                    .HasColumnName("modifiedOn")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(50)
                    .IsUnicode(false);
                entity.Property(e => e.isAdmin)
                    .IsRequired()
                    .HasColumnName("isAdmin")
                    .HasColumnType("bit");

                entity.Property(e => e.VerCol)
                    .IsRequired()
                    .IsRowVersion();
            });

            modelBuilder.Entity<SysRoleTable>(entity =>
            {
                entity.Property(e => e.SysRoleTableId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatedOn)
                    .HasColumnName("createdOn")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ModifiedOn)
                    .HasColumnName("modifiedOn")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.tableName)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(50)
                    .IsUnicode(false);
                entity.Property(e => e.permissionType)
                    .IsRequired()
                    .HasColumnName("permissionType")
                    .HasColumnType("int");
                entity.Property(e => e.searchSQL)
                    .IsRequired()
                    .HasColumnName("searchSQL")
                    .IsUnicode(true);

                entity.Property(e => e.VerCol)
                    .IsRequired()
                    .IsRowVersion();
            });

            modelBuilder.Entity<SysRolePrivilege>(entity =>
            {
                entity.Property(e => e.SysRolePrivilegeId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatedOn)
                    .HasColumnName("createdOn")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ModifiedOn)
                    .HasColumnName("modifiedOn")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.VerCol)
                    .IsRequired()
                    .IsRowVersion();
            });

            modelBuilder.Entity<SysUser>(entity =>
            {
                entity.Property(e => e.SysUserId)
                    .HasColumnName("SysUserID")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatedOn)
                    .HasColumnName("createdOn")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ModifiedOn)
                    .HasColumnName("modifiedOn")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Password)
                    .HasColumnName("password")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.VerCol)
                    .IsRequired()
                    .IsRowVersion();
            });

            modelBuilder.Entity<SysUserRole>(entity =>
            {
                entity.Property(e => e.SysUserRoleId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatedOn)
                    .HasColumnName("createdOn")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ModifiedOn)
                    .HasColumnName("modifiedOn")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.VerCol)
                    .IsRequired()
                    .IsRowVersion();
            });

            modelBuilder.Entity<AccessInfo>(entity =>
            {
                entity.Property(e => e.AccessInfoId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Url)
                    .HasColumnName("Url")
                    .HasMaxLength(int.MaxValue)
                    .IsUnicode(false);
                entity.Property(e => e.Path)
                    .HasColumnName("Path")
                    .HasMaxLength(500)
                    .IsUnicode(false);
                entity.Property(e => e.Ip)
                    .HasColumnName("Ip")
                    .HasMaxLength(50)
                    .IsUnicode(false);
                entity.Property(e => e.Method)
                    .HasColumnName("Method")
                    .HasMaxLength(10)
                    .IsUnicode(false);
                entity.Property(e => e.IsBlocked)
                    .HasColumnName("IsBlocked")
                    .HasColumnType("bit");
                entity.Property(e => e.RequestKey)
                    .HasColumnName("RequestKey")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedOn)
                    .HasColumnName("createdOn")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ModifiedOn)
                    .HasColumnName("modifiedOn")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.VerCol)
                    .IsRequired()
                    .IsRowVersion();
            });

            modelBuilder.Entity<ActionAccessInfo>(entity =>
            {
                entity.Property(e => e.ActionAccessInfoId).HasDefaultValueSql("(newid())");
                entity.Property(e => e.RequestKey)
                    .HasColumnName("RequestKey")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedOn)
                    .HasColumnName("createdOn")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ModifiedOn)
                    .HasColumnName("modifiedOn")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.VerCol)
                    .IsRequired()
                    .IsRowVersion();
            });
        }
    }
}
