public class GetBookDto
{
    public int Id { get; set; }
    public string Title { get; set; }

    public GetBookDto(int id)
    {
        Id = id;
    }

    public GetBookDto(string title)
    {
        Title = title;
    }

    public GetBookDto(int id, string title)
    {
        Title = title;
        Id = id;
    }
}
