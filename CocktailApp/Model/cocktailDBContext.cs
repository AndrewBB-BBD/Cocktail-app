using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace CocktailApp.Model
{
    public partial class cocktailDBContext : DbContext
    {
        public cocktailDBContext()
        {
        }

        public cocktailDBContext(DbContextOptions<cocktailDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Category> Categories { get; set; } = null!;
        public virtual DbSet<Difficulty> Difficulties { get; set; } = null!;
        public virtual DbSet<FlavourProfile> FlavourProfiles { get; set; } = null!;
        public virtual DbSet<Ingredient> Ingredients { get; set; } = null!;
        public virtual DbSet<IngredientMeasurement> IngredientMeasurements { get; set; } = null!;
        public virtual DbSet<Measurement> Measurements { get; set; } = null!;
        public virtual DbSet<Rating> Ratings { get; set; } = null!;
        public virtual DbSet<Recipe> Recipes { get; set; } = null!;
        public virtual DbSet<RecipeType> RecipeTypes { get; set; } = null!;
        public virtual DbSet<UserLogin> UserLogins { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=ANDREWB\\SQLEXPRESS;Database=cocktailDB;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Category");

                entity.Property(e => e.CategoryId).HasColumnName("categoryID");

                entity.Property(e => e.CategoryName)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("categoryName");
            });

            modelBuilder.Entity<Difficulty>(entity =>
            {
                entity.ToTable("Difficulty");

                entity.Property(e => e.DifficultyId).HasColumnName("difficultyID");

                entity.Property(e => e.DifficultyName)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("difficultyName");
            });

            modelBuilder.Entity<FlavourProfile>(entity =>
            {
                entity.HasKey(e => e.FlavourId)
                    .HasName("PK__FlavourP__EE1A92DDA0D72F6D");

                entity.ToTable("FlavourProfile");

                entity.Property(e => e.FlavourId).HasColumnName("flavourID");

                entity.Property(e => e.FlavourName)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("flavourName");
            });

            modelBuilder.Entity<Ingredient>(entity =>
            {
                entity.ToTable("Ingredient");

                entity.Property(e => e.IngredientId).HasColumnName("ingredientID");

                entity.Property(e => e.CategoryId).HasColumnName("categoryID");

                entity.Property(e => e.IngredientName)
                    .HasMaxLength(25)
                    .IsUnicode(false)
                    .HasColumnName("ingredientName");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Ingredients)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Ingredien__categ__2D27B809");
            });

            modelBuilder.Entity<IngredientMeasurement>(entity =>
            {
                entity.HasKey(e => new { e.IngredientId, e.RecipeId })
                    .HasName("PK_IngredientMesaurement");

                entity.ToTable("IngredientMeasurement");

                entity.Property(e => e.IngredientId).HasColumnName("ingredientID");

                entity.Property(e => e.RecipeId).HasColumnName("recipeID");

                entity.Property(e => e.MeasurementAmount)
                    .HasMaxLength(6)
                    .IsUnicode(false)
                    .HasColumnName("measurementAmount");

                entity.Property(e => e.MeasurementId).HasColumnName("measurementID");

                entity.HasOne(d => d.Ingredient)
                    .WithMany(p => p.IngredientMeasurements)
                    .HasForeignKey(d => d.IngredientId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Ingredien__ingre__35BCFE0A");

                entity.HasOne(d => d.Measurement)
                    .WithMany(p => p.IngredientMeasurements)
                    .HasForeignKey(d => d.MeasurementId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Ingredien__measu__37A5467C");

                entity.HasOne(d => d.Recipe)
                    .WithMany(p => p.IngredientMeasurements)
                    .HasForeignKey(d => d.RecipeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Ingredien__recip__36B12243");
            });

            modelBuilder.Entity<Measurement>(entity =>
            {
                entity.ToTable("Measurement");

                entity.Property(e => e.MeasurementId).HasColumnName("measurementID");

                entity.Property(e => e.MeasurementName)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("measurementName");
            });

            modelBuilder.Entity<Rating>(entity =>
            {
                entity.HasKey(e => new { e.UserEmail, e.RecipeId });

                entity.ToTable("Rating");

                entity.Property(e => e.UserEmail)
                    .HasMaxLength(320)
                    .IsUnicode(false)
                    .HasColumnName("userEmail");

                entity.Property(e => e.RecipeId).HasColumnName("recipeID");

                entity.Property(e => e.NumStars)
                    .HasColumnType("decimal(2, 1)")
                    .HasColumnName("numStars");

                entity.Property(e => e.RatingComment)
                    .HasMaxLength(300)
                    .IsUnicode(false)
                    .HasColumnName("ratingComment");

                entity.HasOne(d => d.Recipe)
                    .WithMany(p => p.Ratings)
                    .HasForeignKey(d => d.RecipeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Rating__recipeID__3D5E1FD2");

                entity.HasOne(d => d.UserEmailNavigation)
                    .WithMany(p => p.Ratings)
                    .HasForeignKey(d => d.UserEmail)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Rating__ratingCo__3C69FB99");
            });

            modelBuilder.Entity<Recipe>(entity =>
            {
                entity.ToTable("Recipe");

                entity.Property(e => e.RecipeId).HasColumnName("recipeID");

                entity.Property(e => e.ContaintsAlcohol).HasColumnName("containtsAlcohol");

                entity.Property(e => e.DifficultyId).HasColumnName("difficultyID");

                entity.Property(e => e.FlavourId).HasColumnName("flavourID");

                entity.Property(e => e.RecipeDescription)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("recipeDescription");

                entity.Property(e => e.RecipeImage)
                    .IsUnicode(false)
                    .HasColumnName("recipeImage");

                entity.Property(e => e.RecipeMethod)
                    .IsUnicode(false)
                    .HasColumnName("recipeMethod");

                entity.Property(e => e.RecipeName)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("recipeName");

                entity.Property(e => e.RecipeTime)
                    .HasColumnName("recipeTime")
                    .HasDefaultValueSql("((15))");

                entity.Property(e => e.TypeId).HasColumnName("typeID");

                entity.HasOne(d => d.Difficulty)
                    .WithMany(p => p.Recipes)
                    .HasForeignKey(d => d.DifficultyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Recipe__difficul__31EC6D26");

                entity.HasOne(d => d.Flavour)
                    .WithMany(p => p.Recipes)
                    .HasForeignKey(d => d.FlavourId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Recipe__flavourI__30F848ED");

                entity.HasOne(d => d.Type)
                    .WithMany(p => p.Recipes)
                    .HasForeignKey(d => d.TypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Recipe__typeID__32E0915F");
            });

            modelBuilder.Entity<RecipeType>(entity =>
            {
                entity.HasKey(e => e.TypeId)
                    .HasName("PK__RecipeTy__F04DF11A2F0B52A2");

                entity.ToTable("RecipeType");

                entity.Property(e => e.TypeId).HasColumnName("typeID");

                entity.Property(e => e.TypeName)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("typeName");
            });

            modelBuilder.Entity<UserLogin>(entity =>
            {
                entity.HasKey(e => e.UserEmail)
                    .HasName("PK__UserLogi__D54ADF54E7391756");

                entity.ToTable("UserLogin");

                entity.Property(e => e.UserEmail)
                    .HasMaxLength(320)
                    .IsUnicode(false)
                    .HasColumnName("userEmail");

                entity.Property(e => e.Salt).HasColumnName("salt");

                entity.Property(e => e.UserPassword)
                    .HasMaxLength(64)
                    .HasColumnName("userPassword")
                    .IsFixedLength();

                entity.Property(e => e.Username)
                    .HasMaxLength(40)
                    .IsUnicode(false)
                    .HasColumnName("username");

                entity.HasMany(d => d.Recipes)
                    .WithMany(p => p.UserEmails)
                    .UsingEntity<Dictionary<string, object>>(
                        "Favourite",
                        l => l.HasOne<Recipe>().WithMany().HasForeignKey("RecipeId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK__Favourite__recip__412EB0B6"),
                        r => r.HasOne<UserLogin>().WithMany().HasForeignKey("UserEmail").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK__Favourite__userE__403A8C7D"),
                        j =>
                        {
                            j.HasKey("UserEmail", "RecipeId");

                            j.ToTable("Favourite");

                            j.IndexerProperty<string>("UserEmail").HasMaxLength(320).IsUnicode(false).HasColumnName("userEmail");

                            j.IndexerProperty<int>("RecipeId").HasColumnName("recipeID");
                        });
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
