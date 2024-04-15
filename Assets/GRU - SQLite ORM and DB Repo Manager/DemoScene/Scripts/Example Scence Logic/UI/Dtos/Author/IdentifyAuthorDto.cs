public class IdentifyAuthorDto
{
    public int Id { get; set; }

    public string FirstAndLastName { get; set; }

    public IdentifyAuthorDto(string firstAndLastName, int id)
    {
        FirstAndLastName = firstAndLastName;
        Id = id;
    }
}
