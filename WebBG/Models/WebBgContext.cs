using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace WebBG.Models;

public partial class WebBgContext : DbContext
{
    public WebBgContext()
    {
    }

    public WebBgContext(DbContextOptions<WebBgContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Blog> Blogs { get; set; }

    public virtual DbSet<BoardGame> BoardGames { get; set; }

    public virtual DbSet<Cart> Carts { get; set; }

    public virtual DbSet<CartDetail> CartDetails { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Feedback> Feedbacks { get; set; }

    public virtual DbSet<Menu> Menus { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Publisher> Publishers { get; set; }

    public virtual DbSet<Slider> Sliders { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Promotion> Promotion { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=CHIBAO;Database=WebBG;Trusted_Connection=True;TrustServerCertificate=true;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Blog>(entity =>
        {
            entity.HasKey(e => e.BlogId).HasName("PK__Blog__54379E5019E239E4");

            entity.ToTable("Blog");

            entity.Property(e => e.BlogId).HasColumnName("BlogID");
            entity.Property(e => e.Datebegin).HasColumnType("datetime");
            entity.Property(e => e.Image)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Link).HasMaxLength(255);
            entity.Property(e => e.Title).HasMaxLength(100);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.Blogs)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Blog_User");
        });

        modelBuilder.Entity<BoardGame>(entity =>
        {
            entity.HasKey(e => e.BoardGameId).HasName("PK__BoardGam__D2B424272939980B");
            entity.Property(e => e.BoardGameId).HasColumnName("BoardGameID");
            entity.Property(e => e.Age).HasMaxLength(50);
            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.Image1)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Image2)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Image3)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Link).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.DiscountPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PublisherId).HasColumnName("PublisherID");

            entity.HasOne(d => d.Category).WithMany(p => p.BoardGames)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK__BoardGame__Categ__3B75D760");

            entity.HasOne(d => d.Publisher).WithMany(p => p.BoardGames)
                .HasForeignKey(d => d.PublisherId)
                .HasConstraintName("FK__BoardGame__Publi__3C69FB99");
        });

        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => e.CartId).HasName("PK__Cart__51BCD797D7163A3C");

            entity.ToTable("Cart");

            entity.Property(e => e.CartId).HasColumnName("CartID");
            entity.Property(e => e.DateCreated).HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.Carts)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Cart_User");
        });

        modelBuilder.Entity<CartDetail>(entity =>
        {
            entity.HasKey(e => e.CartDetailId).HasName("PK__CartDeta__01B6A6D4E41608DE");

            entity.ToTable("CartDetail");

            entity.Property(e => e.CartDetailId).HasColumnName("CartDetailID");
            entity.Property(e => e.BoardGameId).HasColumnName("BoardGameID");
            entity.Property(e => e.CartId).HasColumnName("CartID");

            entity.HasOne(d => d.Product).WithMany(p => p.CartDetails)
                .HasForeignKey(d => d.BoardGameId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CartDetail_BoardGame");

            entity.HasOne(d => d.Cart).WithMany(p => p.CartDetails)
                .HasForeignKey(d => d.CartId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CartDetail_Cart");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Categori__19093A2B16F94FA9");

            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.CategoryName).HasMaxLength(100);
            entity.Property(e => e.Link).HasMaxLength(255);
        });

        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.HasKey(e => e.FeedbackId).HasName("PK__Feedback__6A4BEDF6090BBFCD");

            entity.ToTable("Feedback");

            entity.Property(e => e.FeedbackId).HasColumnName("FeedbackID");
            entity.Property(e => e.BoardGameId).HasColumnName("BoardGameID");
            entity.Property(e => e.Datebegin).HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            

            entity.HasOne(d => d.BoardGame).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.BoardGameId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Feedback_BoardGame");

            entity.HasOne(d => d.User).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Feedback_User");
        });

        modelBuilder.Entity<Menu>(entity =>
        {
            entity.HasKey(e => e.IdMenu).HasName("PK__MENU__7F28421A1FB9CF6F");

            entity.ToTable("MENU");

            entity.Property(e => e.IdMenu).HasColumnName("ID_Menu");
            entity.Property(e => e.Link).HasMaxLength(255);
            entity.Property(e => e.Title).HasMaxLength(50);
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__Payment__9B556A5807A1B903");

            entity.ToTable("Payment");

            entity.Property(e => e.PaymentId).HasColumnName("PaymentID");
            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CartId).HasColumnName("CartID");
            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.PaymentDate).HasColumnType("datetime");
            entity.Property(e => e.TransactionId)
                .HasMaxLength(100)
                .HasColumnName("TransactionID");

            entity.HasOne(d => d.Cart).WithMany(p => p.Payments)
                .HasForeignKey(d => d.CartId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Payment_Cart");

            entity.HasOne(d => d.Promotion).WithMany(p => p.Payments)
                .HasForeignKey(d => d.PromoId)
                .HasConstraintName("FK__Payment__Promotion");

            entity.HasOne(d => d.User).WithMany(p => p.Payments)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Payment_User");
        });
        modelBuilder.Entity<Promotion>(entity =>
        {
            entity.HasKey(e => e.PromoId).HasName("PK__Promo__4C657E4B938129C8");

            entity.Property(e => e.PromoId).HasColumnName("PromoID");
            entity.Property(e => e.PromoCode).HasMaxLength(10);
            entity.Property(e => e.DiscountValue).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.DiscountPercent).HasColumnType("decimal(3, 2)");
            entity.Property(e => e.ExpirationDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<Publisher>(entity =>
        {
            entity.HasKey(e => e.PublisherId).HasName("PK__Publishe__4C657E4B938129C8");

            entity.Property(e => e.PublisherId).HasColumnName("PublisherID");
            entity.Property(e => e.Link).HasMaxLength(255);
            entity.Property(e => e.PublisherName).HasMaxLength(100);
        });

        modelBuilder.Entity<Slider>(entity =>
        {
            entity.HasKey(e => e.IdSlider).HasName("PK__SLIDER__709130846B8A9547");

            entity.ToTable("SLIDER");

            entity.Property(e => e.IdSlider).HasColumnName("ID_Slider");
            entity.Property(e => e.Image)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Link).HasMaxLength(255);
            entity.Property(e => e.Title).HasMaxLength(50);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCACE1DF769C");

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.Password)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Username)
                .HasMaxLength(30)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
