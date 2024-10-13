using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using PublisherData;
using PublisherDomain;
namespace PubAPI;

public static class AuthorEndpoints
{
    public static void MapAuthorEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Author").WithTags(nameof(Author));

        group.MapGet("/WithBooks/", GetAllWithBooks);
        group.MapGet("/{authorId}", GetById);
        group.MapPut("/{authorId}", Update);
        group.MapPost("/", Create);
        group.MapDelete("/{authorId}", Delete);
    }
    private static async Task<List<AuthorWithBookResponse>> GetAllWithBooks(PubContext db)
    {
        return await db.Authors.Include(a => a.Books).AsNoTracking()
            .Select(x => new AuthorWithBookResponse
            {
                AuthorId = x.AuthorId,
                FirstName = x.Name.FirstName,
                LastName = x.Name.LastName,
                Books = x.Books.Select(x => new BookResponse(x.BookId, x.Title, x.PublishDate)).ToList()
            }).ToListAsync();
    }
    private static async Task<Results<Ok<AuthorResponse>, NotFound>> GetById(int authorId, PubContext db)
    {
        return await db.Authors.AsNoTracking()
            .Where(model => model.AuthorId == authorId)
            .Select(a => new AuthorResponse(a.AuthorId, a.Name.FirstName, a.Name.LastName))
            .FirstOrDefaultAsync() is { } model
            ? TypedResults.Ok(model)
            : TypedResults.NotFound();
    }
    private static async Task<Results<Ok, NotFound>> Update(int authorId, AuthorRequest request, PubContext db)
    {
        var affected = await db.Authors.Where(model => model.AuthorId == authorId)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(m => m.Name.FirstName, request.FirstName)
                .SetProperty(m => m.Name.LastName, request.LastName));
        return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
    }
    private static async Task<Created<AuthorResponse>> Create(AuthorRequest request, PubContext db)
    {
        var author = new Author { Name = new PersonName { FirstName = request.FirstName, LastName = request.LastName } };
        db.Authors.Add(author);
        await db.SaveChangesAsync();
        return TypedResults.Created($"/api/Author/{author.AuthorId}", new AuthorResponse(author.AuthorId, author.Name.FirstName, author.Name.LastName));
    }
    private static async Task<Results<Ok, NotFound>> Delete(int authorId, PubContext db)
    {
        var affected = await db.Authors.Where(model => model.AuthorId == authorId)
            .ExecuteDeleteAsync();
        return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
    }
    private record AuthorRequest(string FirstName, string LastName);
    private record AuthorResponse(int AuthorId, string FirstName, string LastName);

    private class AuthorWithBookResponse
    {
        public int AuthorId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<BookResponse> Books { get; set; }
    }

    private record BookResponse(int BookId, string Title, DateOnly PublishDate);
}