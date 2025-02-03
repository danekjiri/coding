using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace BTCWallet.DataModels
{
    public partial class WalletDbContext : DbContext
    {
        public WalletDbContext()
        {
        }

        public WalletDbContext(DbContextOptions<WalletDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Address> Addresses { get; set; } = null!;
        public virtual DbSet<Input> Inputs { get; set; } = null!;
        public virtual DbSet<Output> Outputs { get; set; } = null!;
        public virtual DbSet<Profile> Profiles { get; set; } = null!;
        public virtual DbSet<PublicKey> PublicKeys { get; set; } = null!;
        public virtual DbSet<Transaction> Transactions { get; set; } = null!;
        public virtual DbSet<Wallet> Wallets { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("data source=../../../WalletDb.db");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Address>(entity =>
            {
                entity.Property(e => e.AddressId).HasColumnName("AddressID");

                entity.Property(e => e.PublicKeyId).HasColumnName("PublicKeyID");

                entity.HasOne(d => d.PublicKey)
                    .WithMany(p => p.Addresses)
                    .HasForeignKey(d => d.PublicKeyId);
            });

            modelBuilder.Entity<Input>(entity =>
            {
                entity.Property(e => e.InputId).HasColumnName("InputID");

                entity.Property(e => e.TransactionId).HasColumnName("TransactionID");

                entity.HasOne(d => d.Transaction)
                    .WithMany(p => p.Inputs)
                    .HasForeignKey(d => d.TransactionId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Output>(entity =>
            {
                entity.Property(e => e.OutputId).HasColumnName("OutputID");

                entity.Property(e => e.AddressId).HasColumnName("AddressID");

                entity.Property(e => e.InputId).HasColumnName("InputID");

                entity.Property(e => e.TransactionId).HasColumnName("TransactionID");

                entity.HasOne(d => d.Address)
                    .WithMany(p => p.Outputs)
                    .HasForeignKey(d => d.AddressId);

                entity.HasOne(d => d.Input)
                    .WithMany(p => p.Outputs)
                    .HasForeignKey(d => d.InputId);

                entity.HasOne(d => d.Transaction)
                    .WithMany(p => p.Outputs)
                    .HasForeignKey(d => d.TransactionId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Profile>(entity =>
            {
                entity.Property(e => e.ProfileId).HasColumnName("ProfileID");

                entity.Property(e => e.WalletId).HasColumnName("WalletID");

                entity.HasOne(d => d.Wallet)
                    .WithMany(p => p.Profiles)
                    .HasForeignKey(d => d.WalletId);
            });

            modelBuilder.Entity<PublicKey>(entity =>
            {
                entity.Property(e => e.PublicKeyId).HasColumnName("PublicKeyID");

                entity.Property(e => e.WalletId).HasColumnName("WalletID");

                entity.HasOne(d => d.Wallet)
                    .WithMany(p => p.PublicKeys)
                    .HasForeignKey(d => d.WalletId);
            });

            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.Property(e => e.TransactionId).HasColumnName("TransactionID");
            });

            modelBuilder.Entity<Wallet>(entity =>
            {
                entity.Property(e => e.WalletId).HasColumnName("WalletID");

                entity.Property(e => e.XpubWif).HasColumnName("XPubWif");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
