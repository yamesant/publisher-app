namespace PublisherDomain;

public class Author
{
    public int AuthorId { get; set; }
    public PersonName Name { get; set; }
    public List<Book> Books { get; set; } = new();
}
