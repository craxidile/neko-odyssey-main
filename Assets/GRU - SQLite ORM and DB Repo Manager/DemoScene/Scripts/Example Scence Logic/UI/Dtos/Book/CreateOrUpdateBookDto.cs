public class CreateOrUpdateBookDto
{
    public int? Id { get; set; }

    public string Title { get; set; }

    public int? Year { get; set; }

    public int AuthorId { get; set; }

    public CreateOrUpdateBookDto(string title, int? year, int authorId, int? id = null)
    {
        Title = title;
        Year = year;
        Id = id;
        AuthorId = authorId;
    }
}
