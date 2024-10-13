using Microsoft.EntityFrameworkCore;
using PublisherDomain;

namespace PublisherData;

public class PubContext : DbContext
{
    public DbSet<Author> Authors { get; set; }
    public DbSet<Book> Books { get; set; }
    public DbSet<Artist> Artists { get; set; }
    public DbSet<Cover> Covers { get; set; }
    public DbSet<AuthorByArtist> AuthorsByArtist { get; set; }

    public PubContext(DbContextOptions<PubContext> options) : base(options)
    {   
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AuthorByArtist>().HasNoKey()
            .ToView(nameof(AuthorsByArtist));
        modelBuilder.Entity<Author>().ComplexProperty(x => x.Name);
        modelBuilder.Entity<Artist>().ComplexProperty(x => x.Name);

        var someBooks = new Book[]
        {
            new Book
            {
                BookId = 1, AuthorId = 1, Title = "In God's Ear",
                PublishDate = new DateOnly(1989, 3, 1)
            },
            new Book
            {
                BookId = 2, AuthorId = 2, Title = "A Tale For the Time Being",
                PublishDate = new DateOnly(2013, 12, 31)
            },
            new Book
            {
                BookId = 3, AuthorId = 3, Title = "The Left Hand of Darkness",
                PublishDate = new DateOnly(1969, 3, 1)
            }
        };
        modelBuilder.Entity<Book>().HasData(someBooks);

        var someCovers = new Cover[]
        {
            new Cover
            {
                CoverId = 1, BookId = 3,
                DesignIdeas = "How about a left hand in the dark?", DigitalOnly = false
            },
            new Cover
            {
                CoverId = 2, BookId = 2,
                DesignIdeas = "Should we put a clock?", DigitalOnly = true
            },
            new Cover
            {
                CoverId = 3, BookId = 1,
                DesignIdeas = "A big ear in the clouds?", DigitalOnly = false
            }
        };
        modelBuilder.Entity<Cover>().HasData(someCovers);
    }
}