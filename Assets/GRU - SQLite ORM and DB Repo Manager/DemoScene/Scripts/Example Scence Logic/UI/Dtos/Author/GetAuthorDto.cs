public class GetAuthorDto
{
    public int Id { get; set; }
    public string Name { get; set; }

    public GetAuthorDto(int id)
    {
        Id = id; 
    }

    public GetAuthorDto(string name)
    {
        Name = name;
    }

    public GetAuthorDto(int id, string name)
    {
        Name = name;
        Id = id;
    }
}
