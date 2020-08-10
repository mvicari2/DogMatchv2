using DogMatch.Domain.Data.Models;
using IdentityServer4.EntityFramework.Options;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DogMatch.Domain.Data
{
    public class DogMatchDbContext : ApiAuthorizationDbContext<DogMatchUser>
    {
        public DogMatchDbContext(
            DbContextOptions options,
            IOptions<OperationalStoreOptions> operationalStoreOptions) : base(options, operationalStoreOptions)
        {
        }

        public DbSet<Addresses> Addresses { get; set; }        
        public DbSet<Biography> Biography { get; set; }
        public DbSet<Color> Color { get; set; }
        public DbSet<Dogs> Dogs { get; set; }
        public DbSet<DogImages> DogImages { get; set; }
        public DbSet<UserImages> UserImages { get; set; }
        public DbSet<Temperament> Temperament { get; set; }
        public DbSet<DogMatchUser> DogMatchUser { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Addresses>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnType("int");
                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasColumnType("nvarchar(450)");
                entity.Property(e => e.Address1)
                    .HasColumnType("nvarchar(200)");
                entity.Property(e => e.Address2)
                    .HasColumnType("nvarchar(200)");
                entity.Property(e => e.Apt)
                    .HasColumnType("nvarchar(50)");
                entity.Property(e => e.City)
                    .HasColumnType("nvarchar(200)");
                entity.Property(e => e.State)
                    .HasColumnType("nvarchar(100)");
                entity.Property(e => e.Zip)
                    .HasColumnType("nvarchar(20)");
                entity.Property(e => e.IsDeleted)
                    .HasColumnType("bit");
                entity.Property(e => e.Created)
                    .HasColumnType("datetime");
                entity.Property(e => e.CreatedBy)
                    .HasColumnType("nvarchar(450)");
                entity.Property(e => e.LastModified)
                    .HasColumnType("datetime");
                entity.Property(e => e.LastModifiedBy)
                    .HasColumnType("nvarchar(450)");
                entity.Property(e => e.AddressGUID)
                    .ValueGeneratedOnAdd()
                    .HasDefaultValueSql("NEWID()");

                entity.HasIndex("UserId");
                entity.HasIndex("CreatedBy");
                entity.HasIndex("LastModifiedBy");

                entity.HasOne(e => e.User)
                    .WithMany(e => e.Addresses)
                    .HasPrincipalKey(e => e.Id)
                    .HasForeignKey(e => e.UserId)
                    .HasConstraintName("FK_Addresses_User");

                entity.HasOne(e => e.CreatedByUser)
                    .WithMany(e => e.AddressesCreatedByUser)
                    .HasPrincipalKey(e => e.Id)
                    .HasForeignKey(e => e.CreatedBy)
                    .HasConstraintName("FK_Addresses_CreatedByUser");

                entity.HasOne(e => e.ModifiedByUser)
                    .WithMany(e => e.AddressesModifiedByUser)
                    .HasPrincipalKey(e => e.Id)
                    .HasForeignKey(e => e.LastModifiedBy)
                    .HasConstraintName("FK_Addresses_ModifiedByUser");

                entity.ToTable("Addresses");
            });            

            modelBuilder.Entity<Biography>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnType("int");
                entity.Property(e => e.DogId)
                    .HasColumnType("int");
                entity.Property(e => e.AboutDoggo)
                    .HasColumnType("nvarchar(max)");
                entity.Property(e => e.FavoriteMemory)
                    .HasColumnType("nvarchar(max)");
                entity.Property(e => e.FavoriteFoods)
                    .HasColumnType("nvarchar(max)");
                entity.Property(e => e.FavoriteToy)
                    .HasColumnType("nvarchar(max)");
                entity.Property(e => e.FavoriteWalkLocation)
                    .HasColumnType("nvarchar(max)");
                entity.Property(e => e.FavoriteMemory)
                    .HasColumnType("nvarchar(max)");
                entity.Property(e => e.Created)
                    .HasColumnType("datetime");
                entity.Property(e => e.CreatedBy)
                    .HasColumnType("nvarchar(450)");
                entity.Property(e => e.LastModified)
                    .HasColumnType("datetime");
                entity.Property(e => e.LastModifiedBy)
                    .HasColumnType("nvarchar(450)");
                entity.Property(e => e.BiographyGUID)
                    .ValueGeneratedOnAdd()
                    .HasDefaultValueSql("NEWID()");

                entity.HasIndex("DogId")
                    .IsUnique();
                entity.HasIndex("CreatedBy");
                entity.HasIndex("LastModifiedBy");

                entity.HasOne(e => e.CreatedByUser)
                    .WithMany(e => e.BiographiesCreatedByUser)
                    .HasPrincipalKey(e => e.Id)
                    .HasForeignKey(e => e.CreatedBy)
                    .HasConstraintName("FK_Biographies_CreatedByUser");

                entity.HasOne(e => e.ModifiedByUser)
                    .WithMany(e => e.BiographiesModifiedByUser)
                    .HasPrincipalKey(e => e.Id)
                    .HasForeignKey(e => e.LastModifiedBy)
                    .HasConstraintName("FK_Biographies_ModifiedByUser");

                entity.ToTable("Biography");
            });

            modelBuilder.Entity<Color>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnType("int");
                entity.Property(e => e.DogId)
                    .IsRequired()
                    .HasColumnType("int");
                entity.Property(e => e.ColorString)
                    .IsRequired()
                    .HasColumnType("nvarchar(100)");
                entity.Property(e => e.ColorGUID)
                    .ValueGeneratedOnAdd()
                    .HasDefaultValueSql("NEWID()");

                entity.HasKey("DogId");

                entity.HasOne(e => e.Dog)
                    .WithMany(e => e.Colors)
                    .HasPrincipalKey(e => e.Id)
                    .HasForeignKey(e => e.DogId)
                    .HasConstraintName("FK_Color_Dog");                

                entity.ToTable("Color");
            });

            modelBuilder.Entity<DogImages>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnType("int");
                entity.Property(e => e.DogId)
                    .IsRequired()
                    .HasColumnType("int");
                entity.Property(e => e.Filename)
                    .HasColumnType("nvarchar(max)");
                entity.Property(e => e.IsProfileImage)
                    .HasColumnType("bit");
                entity.Property(e => e.IsDeleted)
                    .HasColumnType("bit");
                entity.Property(e => e.Created)
                    .HasColumnType("datetime");
                entity.Property(e => e.CreatedBy)
                    .HasColumnType("nvarchar(450)");
                entity.Property(e => e.DogImageGUID)
                    .ValueGeneratedOnAdd()
                    .HasDefaultValueSql("NEWID()");

                entity.HasIndex("DogId");
                entity.HasIndex("CreatedBy");

                entity.HasOne(e => e.Dog)
                    .WithMany(e => e.AlbumImages)
                    .HasPrincipalKey(e => e.Id)
                    .HasForeignKey(e => e.DogId)
                    .HasConstraintName("FK_AlbumImage_Dog");

                entity.HasOne(e => e.CreatedByUser)
                    .WithMany(e => e.DogAlbumImages)
                    .HasPrincipalKey(e => e.Id)
                    .HasForeignKey(e => e.CreatedBy)
                    .HasConstraintName("FK_Album_CreatedByUser");

                entity.ToTable("DogImages");
            });

            modelBuilder.Entity<DogMatchUser>(entity =>
            {
                entity.Property(e => e.FirstName)
                    .HasColumnType("nvarchar(100)");
                entity.Property(e => e.MI)
                    .HasColumnType("nvarchar(20)");
                entity.Property(e => e.LastName)
                    .HasColumnType("nvarchar(100)");
                entity.Property(e => e.PrimaryAddressId)
                    .HasColumnType("int");
                entity.Property(e => e.Birthday)
                    .HasColumnType("datetime");
                entity.Property(e => e.UserImageId)
                    .HasColumnType("int");
                entity.Property(e => e.LastLogin)
                    .HasColumnType("datetime");
                entity.Property(e => e.IsDeleted)
                    .HasColumnType("bit");
                entity.Property(e => e.Created)
                    .HasColumnType("datetime");
                entity.Property(e => e.LastModified)
                    .HasColumnType("datetime");
                entity.Property(e => e.LastModifiedBy)
                    .HasColumnType("nvarchar(450)");
                entity.Property(e => e.UserGUID)
                    .ValueGeneratedOnAdd()
                    .HasDefaultValueSql("NEWID()");

                entity.HasIndex("PrimaryAddressId");

                entity.HasOne(e => e.PrimaryAddress)
                    .WithOne(e => e.PrimaryAddressUser)
                    .HasPrincipalKey<Addresses>(e => e.Id)
                    .HasForeignKey<DogMatchUser>(e => e.PrimaryAddressId)
                    .HasConstraintName("FK_User_PrimaryAddress");

                entity.HasOne(e => e.UserProfileImage)
                    .WithOne(e => e.ProfileImageUser)
                    .HasPrincipalKey<UserImages>(e => e.Id)
                    .HasForeignKey<DogMatchUser>(e => e.UserImageId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DogMatchUser_UserProfileImage");
            });

            modelBuilder.Entity<Dogs>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnType("int");
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("nvarchar(200)");
                entity.Property(e => e.Breed)
                    .HasColumnType("nvarchar(200)");
                entity.Property(e => e.Birthday)
                    .HasColumnType("datetime");
                entity.Property(e => e.Gender)
                    .HasColumnType("char(1)");
                entity.Property(e => e.Weight)
                    .HasColumnType("int");
                entity.Property(e => e.ProfileImageId)
                    .HasColumnType("int");
                entity.Property(e => e.TemperamentId)
                    .HasColumnType("int");
                entity.Property(e => e.BiographyId)
                    .HasColumnType("int");
                entity.Property(e => e.OwnerId)
                    .HasColumnType("nvarchar(450)");
                entity.Property(e => e.AddressId)
                    .HasColumnType("int");
                entity.Property(e => e.IsDeleted)
                    .HasColumnType("bit");
                entity.Property(e => e.Created)
                    .HasColumnType("datetime");
                entity.Property(e => e.CreatedBy)
                    .HasColumnType("nvarchar(450)");
                entity.Property(e => e.LastModified)
                    .HasColumnType("datetime");
                entity.Property(e => e.LastModifiedBy)
                    .HasColumnType("nvarchar(450)");
                entity.Property(e => e.DogGUID)
                    .ValueGeneratedOnAdd()
                    .HasDefaultValueSql("NEWID()");

                entity.HasIndex("TemperamentId");
                entity.HasIndex("BiographyId");
                entity.HasIndex("OwnerId");
                entity.HasIndex("AddressId");
                entity.HasIndex("CreatedBy");
                entity.HasIndex("LastModifiedBy");

                entity.HasOne(e => e.Address)
                    .WithMany(e => e.Dogs)
                    .HasPrincipalKey(e => e.Id)
                    .HasForeignKey(e => e.AddressId)
                    .HasConstraintName("FK_Dogs_Address");

                entity.HasOne(e => e.Biography)
                    .WithOne(e => e.Dog)
                    .HasPrincipalKey<Biography>(e => e.Id)
                    .HasForeignKey<Dogs>(e => e.BiographyId)
                    .HasConstraintName("FK_Dog_Biography");

                entity.HasOne(e => e.DogProfileImage)
                    .WithOne(e => e.ProfileImageDog)
                    .HasPrincipalKey<DogImages>(e => e.Id)
                    .HasForeignKey<Dogs>(e => e.ProfileImageId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Dog_ProfileImage");

                entity.HasOne(e => e.Temperament)
                    .WithOne(e => e.Dog)
                    .HasPrincipalKey<Temperament>(e => e.Id)
                    .HasForeignKey<Dogs>(e => e.TemperamentId)
                    .HasConstraintName("FK_Dog_Temperament");

                entity.HasOne(e => e.Owner)
                    .WithMany(e => e.Dogs)
                    .HasPrincipalKey(e => e.Id)
                    .HasForeignKey(e => e.OwnerId)
                    .HasConstraintName("FK_Dogs_Owner");

                entity.HasOne(e => e.CreatedByUser)
                    .WithMany(e => e.DogsCreatedByUser)
                    .HasPrincipalKey(e => e.Id)
                    .HasForeignKey(e => e.CreatedBy)
                    .HasConstraintName("FK_Dogs_CreatedByUser");

                entity.HasOne(e => e.ModifiedByUser)
                    .WithMany(e => e.DogsModifiedByUser)
                    .HasPrincipalKey(e => e.Id)
                    .HasForeignKey(e => e.LastModifiedBy)
                    .HasConstraintName("FK_Dogs_ModifiedByUser");

                entity.ToTable("Dogs");
            });            

            modelBuilder.Entity<Temperament>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd();
                entity.Property(e => e.DogId)
                    .HasColumnType("int");
                entity.Property(e => e.Empathetic)
                    .HasColumnType("int");
                entity.Property(e => e.Anxiety)
                    .HasColumnType("int");
                entity.Property(e => e.Fearful)
                    .HasColumnType("int");
                entity.Property(e => e.IsAfraidFireworks)
                    .HasColumnType("int");
                entity.Property(e => e.FriendlinessOverall)
                    .HasColumnType("int");
                entity.Property(e => e.GoodWithPeople)
                    .HasColumnType("int");
                entity.Property(e => e.GoodWithOtherDogs)
                    .HasColumnType("int");
                entity.Property(e => e.GoodWithCats)
                    .HasColumnType("int");
                entity.Property(e => e.GoodWithOtherAnimals)
                    .HasColumnType("int");
                entity.Property(e => e.GoodWithChildren)
                    .HasColumnType("int");
                entity.Property(e => e.Playfulness)
                    .HasColumnType("int");
                entity.Property(e => e.LikesPlayingHumans)
                    .HasColumnType("int");
                entity.Property(e => e.LikesPlayingDogs)
                    .HasColumnType("int");
                entity.Property(e => e.PlaysFetch)
                    .HasColumnType("int");
                entity.Property(e => e.LikesToys)
                    .HasColumnType("int");
                entity.Property(e => e.LikesTreats)
                    .HasColumnType("int");
                entity.Property(e => e.AthleticLevel)
                    .HasColumnType("int");
                entity.Property(e => e.LikesExcersize)
                    .HasColumnType("int");
                entity.Property(e => e.TrainingLevel)
                    .HasColumnType("int");
                entity.Property(e => e.Trainability)
                    .HasColumnType("int");
                entity.Property(e => e.Stubbornness)
                    .HasColumnType("int");
                entity.Property(e => e.Intelligence)
                    .HasColumnType("int");
                entity.Property(e => e.SenseOfSmell)
                    .HasColumnType("int");
                entity.Property(e => e.PreyDrive)
                    .HasColumnType("int");
                entity.Property(e => e.AggressionLevel)
                    .HasColumnType("int");
                entity.Property(e => e.Protectiveness)
                    .HasColumnType("int");
                entity.Property(e => e.DistinguishThreatening)
                    .HasColumnType("int");
                entity.Property(e => e.BalanceStability)
                    .HasColumnType("int");
                entity.Property(e => e.Confidence)
                    .HasColumnType("int");
                entity.Property(e => e.IsPickyEater)
                    .HasColumnType("int");
                entity.Property(e => e.Shedding)
                    .HasColumnType("int");
                entity.Property(e => e.Barking)
                    .HasColumnType("int");
                entity.Property(e => e.SmellRating)
                    .HasColumnType("int");
                entity.Property(e => e.HairOrFur)
                    .HasColumnType("bit");
                entity.Property(e => e.Housebroken)
                    .HasColumnType("bit");
                entity.Property(e => e.OutsideOrInside)
                    .HasColumnType("bit");
                entity.Property(e => e.IsFixed)
                    .HasColumnType("bit");
                entity.Property(e => e.Created)
                    .HasColumnType("datetime");
                entity.Property(e => e.CreatedBy)
                    .HasColumnType("nvarchar(450)");
                entity.Property(e => e.LastModified)
                    .HasColumnType("datetime");
                entity.Property(e => e.LastModifiedBy)
                    .HasColumnType("nvarchar(450)");
                entity.Property(e => e.TemperamentGUID)
                    .ValueGeneratedOnAdd()
                    .HasDefaultValueSql("NEWID()");

                entity.HasIndex("DogId")
                    .IsUnique();
                entity.HasIndex("CreatedBy");
                entity.HasIndex("LastModifiedBy");

                entity.HasOne(e => e.CreatedByUser)
                    .WithMany(e => e.TemperamentsCreatedByUser)
                    .HasPrincipalKey(e => e.Id)
                    .HasForeignKey(e => e.CreatedBy)
                    .HasConstraintName("FK_Temperaments_CreatedByUser");

                entity.HasOne(e => e.ModifiedByUser)
                    .WithMany(e => e.TemperamentsModifiedByUser)
                    .HasPrincipalKey(e => e.Id)
                    .HasForeignKey(e => e.LastModifiedBy)
                    .HasConstraintName("FK_Temperaments_ModifiedByUser");

                entity.ToTable("Temperament");
            });

            modelBuilder.Entity<UserImages>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnType("int");
                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasColumnType("nvarchar(450)");
                entity.Property(e => e.Filename)
                    .HasColumnType("nvarchar(max)");
                entity.Property(e => e.IsProfileImage)
                    .HasColumnType("bit");
                entity.Property(e => e.IsDeleted)
                    .HasColumnType("bit");
                entity.Property(e => e.Created)
                    .HasColumnType("datetime");
                entity.Property(e => e.CreatedBy)
                    .HasColumnType("nvarchar(450)");
                entity.Property(e => e.UserImageGUID)
                    .ValueGeneratedOnAdd()
                    .HasDefaultValueSql("NEWID()");

                entity.HasIndex("UserId");
                entity.HasIndex("CreatedBy");

                entity.HasOne(e => e.User)
                    .WithMany(e => e.UserImages)
                    .HasPrincipalKey(e => e.Id)
                    .HasForeignKey(e => e.UserId)
                    .HasConstraintName("FK_UserImages_User");

                entity.HasOne(e => e.CreatedByUser)
                    .WithMany(e => e.UserImagesCreatedBy)
                    .HasPrincipalKey(e => e.Id)
                    .HasForeignKey(e => e.CreatedBy)
                    .HasConstraintName("FK_UserImages_CreatedByUser");

                entity.ToTable("UserImages");
            });            
        }
    }
}
