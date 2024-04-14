using Database.Repository;
using System.Collections.Generic;

public class AuthorFactory
{
    public Author Map(CreateOrUpdateAuthorDto dto)
    {
        var entity = new Author(dto.FirstAndLastName, dto.City, dto.DateOfBirth, dto.Id);
        return entity;
    }

    public Author Map(GetAuthorDto dto)
    {
        var entity = new Author(dto.Id, dto.Name);
        return entity;
    }
 
    public CreateOrUpdateAuthorDto Map(Author entity)
    {
        var dto = new CreateOrUpdateAuthorDto(entity.Name, entity.City, entity.Birthday, entity.Id);
        return dto;
    }

    public List<IdentifyAuthorDto> Map(IEnumerable<Author> entities)
    {
        var authorDtos = new List<IdentifyAuthorDto>() { };
        
        foreach (var entity in entities)
        {
            var authorDto = new IdentifyAuthorDto(entity.Name, entity.Id);
            authorDtos.Add(authorDto);
        }

        return authorDtos;
    }
}
