using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DACS_WebClothes.Models;

public partial class WebClothesContext : DbContext
{
    public WebClothesContext()
    {
    }

    public WebClothesContext(DbContextOptions<WebClothesContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Blog> Blogs { get; set; }

    public virtual DbSet<Brand> Brands { get; set; }

    public virtual DbSet<Cart> Carts { get; set; }

    public virtual DbSet<CartDetail> CartDetails { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<CommentBlog> CommentBlogs { get; set; }

    public virtual DbSet<CommentProduct> CommentProducts { get; set; }

    public virtual DbSet<CustomerFeedback> CustomerFeedbacks { get; set; }

    public virtual DbSet<Discount> Discounts { get; set; }

    public virtual DbSet<Feedback> Feedbacks { get; set; }

    public virtual DbSet<Inventory> Inventories { get; set; }

    public virtual DbSet<Menu> Menus { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-J6S1QT9;Database=WebClothes;Trusted_Connection=True;TrustServerCertificate=true;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Blog>(entity =>
        {
            entity.HasKey(e => e.BlogId).HasName("PK__Blogs__54379E5065394F0F");

            entity.Property(e => e.BlogId).HasColumnName("BlogID");
            entity.Property(e => e.DateBegin).HasColumnType("datetime");
            entity.Property(e => e.Image).HasMaxLength(255);
            entity.Property(e => e.Link).HasMaxLength(200);
            entity.Property(e => e.Title).HasMaxLength(255);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.Blogs)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Blogs__UserID__4BAC3F29");
        });

        modelBuilder.Entity<Brand>(entity =>
        {
            entity.HasKey(e => e.BrandId).HasName("PK__Brands__DAD4F3BEC6171C4A");

            entity.Property(e => e.BrandId).HasColumnName("BrandID");
            entity.Property(e => e.BrandName).HasMaxLength(100);
            entity.Property(e => e.Link).HasMaxLength(200);
        });

        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => e.CartId).HasName("PK__Carts__51BCD7979CAB1C4A");

            entity.Property(e => e.CartId).HasColumnName("CartID");
            entity.Property(e => e.DateBegin).HasColumnType("datetime");
            entity.Property(e => e.OrderId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("OrderID");
            entity.Property(e => e.Status).HasMaxLength(200);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.Carts)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Carts__UserID__44FF419A");
        });

        modelBuilder.Entity<CartDetail>(entity =>
        {
            entity.HasKey(e => e.CartDetailId).HasName("PK__CartDeta__01B6A6D414C62126");

            entity.Property(e => e.CartDetailId).HasColumnName("CartDetailID");
            entity.Property(e => e.CartId).HasColumnName("CartID");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.SoldNum).HasColumnName("Sold_Num");

            entity.HasOne(d => d.Cart).WithMany(p => p.CartDetails)
                .HasForeignKey(d => d.CartId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CartDetai__CartI__47DBAE45");

            entity.HasOne(d => d.Product).WithMany(p => p.CartDetails)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CartDetai__Produ__48CFD27E");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Categori__19093A2BA0FC5E7F");

            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.CategoryName).HasMaxLength(100);
            entity.Property(e => e.Link).HasMaxLength(200);
        });

        modelBuilder.Entity<CommentBlog>(entity =>
        {
            entity.HasKey(e => e.ComBlogId).HasName("PK__CommentB__795527F194EA029D");

            entity.Property(e => e.ComBlogId).HasColumnName("Com_BlogID");
            entity.Property(e => e.BlogId).HasColumnName("BlogID");
            entity.Property(e => e.ComBlogName).HasColumnName("Com_BlogName");
            entity.Property(e => e.DateBegin).HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Blog).WithMany(p => p.CommentBlogs)
                .HasForeignKey(d => d.BlogId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CommentBl__BlogI__59063A47");

            entity.HasOne(d => d.User).WithMany(p => p.CommentBlogs)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CommentBl__UserI__59FA5E80");
        });

        modelBuilder.Entity<CommentProduct>(entity =>
        {
            entity.HasKey(e => e.ComProId).HasName("PK__CommentP__DC61A774841EEF2C");

            entity.Property(e => e.ComProId).HasColumnName("Com_ProID");
            entity.Property(e => e.ComProName).HasColumnName("Com_ProName");
            entity.Property(e => e.DateBegin).HasColumnType("datetime");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Product).WithMany(p => p.CommentProducts)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CommentPr__Produ__5CD6CB2B");

            entity.HasOne(d => d.User).WithMany(p => p.CommentProducts)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CommentPr__UserI__5DCAEF64");
        });

        modelBuilder.Entity<CustomerFeedback>(entity =>
        {
            entity.HasKey(e => e.CusFeedbackId).HasName("PK__Customer__C84FA11D91DC6405");

            entity.Property(e => e.CusFeedbackId).HasColumnName("Cus_FeedbackID");
            entity.Property(e => e.CusFeedbackName).HasColumnName("Cus_FeedbackName");
            entity.Property(e => e.DateBegin).HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.CustomerFeedbacks)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CustomerF__UserI__5629CD9C");
        });

        modelBuilder.Entity<Discount>(entity =>
        {
            entity.HasKey(e => e.DiscountId).HasName("PK__Discount__E43F6DF63C3D4474");

            entity.Property(e => e.DiscountId).HasColumnName("DiscountID");
            entity.Property(e => e.DiscountName)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.DiscountNum).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.DiscountPercent).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.ExpirationDate).HasColumnType("datetime");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");

            entity.HasOne(d => d.Product).WithMany(p => p.Discounts)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__Discounts__Produ__5070F446");
        });

        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.HasKey(e => e.FeedBackId).HasName("PK__Feedback__E2CB3867F3E40005");

            entity.Property(e => e.FeedBackId).HasColumnName("FeedBackID");
            entity.Property(e => e.DateBegin).HasColumnType("datetime");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Product).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Feedbacks__Produ__412EB0B6");

            entity.HasOne(d => d.User).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Feedbacks__UserI__4222D4EF");
        });

        modelBuilder.Entity<Inventory>(entity =>
        {
            entity.HasKey(e => e.InventoryId).HasName("PK__Inventor__F5FDE6D30AD42556");

            entity.Property(e => e.InventoryId).HasColumnName("InventoryID");
            entity.Property(e => e.ImportDate).HasColumnType("datetime");
            entity.Property(e => e.ImportPrice).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.PlaceOfImport).HasMaxLength(100);
            entity.Property(e => e.ProductId).HasColumnName("ProductID");

            entity.HasOne(d => d.Product).WithMany(p => p.Inventories)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__Inventori__Produ__534D60F1");
        });

        modelBuilder.Entity<Menu>(entity =>
        {
            entity.HasKey(e => e.MenuId).HasName("PK__Menus__C99ED250FE6B41E5");

            entity.Property(e => e.MenuId).HasColumnName("MenuID");
            entity.Property(e => e.Link).HasMaxLength(200);
            entity.Property(e => e.Title).HasMaxLength(100);
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__Payments__9B556A588590E5AA");

            entity.Property(e => e.PaymentId).HasColumnName("PaymentID");
            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CartId).HasColumnName("CartID");
            entity.Property(e => e.DiscountId).HasColumnName("DiscountID");
            entity.Property(e => e.PaymentDate).HasColumnType("datetime");
            entity.Property(e => e.PaymentMethod).HasMaxLength(255);
            entity.Property(e => e.TransportMethod).HasMaxLength(255);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Discount).WithMany(p => p.Payments)
                .HasForeignKey(d => d.DiscountId)
                .HasConstraintName("FK_Payments_Discounts");

            entity.HasOne(d => d.User).WithMany(p => p.Payments)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Payments_Users");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__Products__B40CC6ED2275A899");

            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.BrandId).HasColumnName("BrandID");
            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.DiscountPrice).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.Image1)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Image2)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Image3)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Link).HasMaxLength(200);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Price).HasColumnType("decimal(18, 0)");

            entity.HasOne(d => d.Brand).WithMany(p => p.Products)
                .HasForeignKey(d => d.BrandId)
                .HasConstraintName("FK__Products__BrandI__3C69FB99");

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK__Products__Catego__3B75D760");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCACF19E2ED0");

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.Address).HasMaxLength(150);
            entity.Property(e => e.Email)
                .HasMaxLength(70)
                .IsUnicode(false);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Phone)
                .HasMaxLength(12)
                .IsUnicode(false);
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
