// <auto-generated />
using FriendOrganizer.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace FriendOrganizer.DataAccess.Migrations
{
    [DbContext(typeof(FriendOrganizerDbContext))]
    [Migration("20230130140211_updateFriendKey")]
    partial class updateFriendKey
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("FriendOrganizer.Model.Friend", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Email")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("LastName")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.ToTable("Friends");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            FirstName = "Thomas",
                            LastName = "Huber"
                        },
                        new
                        {
                            Id = 2,
                            FirstName = "Andreas",
                            LastName = "Boehler"
                        },
                        new
                        {
                            Id = 3,
                            FirstName = "Julia",
                            LastName = "Huber"
                        },
                        new
                        {
                            Id = 4,
                            FirstName = "Chrissi",
                            LastName = "Egin"
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
