public class IdentifyBookDto
{
    public int Id { get; set; }

    public string Title { get; set; }

    public IdentifyBookDto(string title, int id)
    {
        Title = title;
        Id = id;
    }
}
