using Database.Repository;
using System.Collections.Generic;

public class BookFactory
{
    public Book Map(CreateOrUpdateBookDto dto)
    {
        var entity = new Book(dto.Title, dto.Year, dto.AuthorId, dto.Id);
        return entity;
    }

    public Book Map(GetBookDto dto)
    {
        var entity = new Book(dto.Id, dto.Title);
        return entity;
    }

    public CreateOrUpdateBookDto Map(Book entity)
    {
        var dto = new CreateOrUpdateBookDto(entity.Title, entity.Year, entity.AuthorId, entity.Id);
        return dto;
    }

    public List<IdentifyBookDto> Map(IEnumerable<Book> entities)
    {
        var bookDtos = new List<IdentifyBookDto>() { };
        
        foreach (var entity in entities)
        {
            var bookDto = new IdentifyBookDto(entity.Title, entity.Id);
            bookDtos.Add(bookDto);
        }

        return bookDtos;
    }
}
