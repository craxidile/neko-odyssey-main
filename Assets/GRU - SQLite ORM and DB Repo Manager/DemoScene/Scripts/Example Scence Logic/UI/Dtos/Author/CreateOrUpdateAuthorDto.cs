using System;

public class CreateOrUpdateAuthorDto
{
    public int? Id { get; set; }

    public string FirstAndLastName { get; set; }

    public string City { get; set; }

    public DateTime DateOfBirth { get; set; }

    public CreateOrUpdateAuthorDto(string firstAndLastName, string city, DateTime dateOfBirth, int? id = null)
    {
        FirstAndLastName = firstAndLastName;
        City = city;
        DateOfBirth = dateOfBirth;
        Id = id;
    }
}
