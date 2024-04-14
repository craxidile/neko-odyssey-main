using System;
using System.Collections.Generic;
using SQLite4Unity3d;
using SpatiumInteractive.Libraries.Unity.GRU.Base;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;
using SpatiumInteractive.Libraries.Unity.Platform.Extensions;

namespace Database.Repository
{
    /// <summary>
    /// example class of authors that you can use
    /// for testing and playing around with GRU's 
    /// capabilities and features
    /// </summary>
    [Serializable]
    public class Author : EntityBase<int>, IAggregateRoot
    {
        #region Properties (Correspond to Columns in Database)

        [NotNull]
        public string Name { get; set; }

        public string City { get; set; }

        [NotNull]
        public DateTime Birthday { get; set; }

        [Ignore]
        public virtual ICollection<Book> Books { get; set; }

        [ForeignKey(typeof(Author))]
        public int? ColleagueId { get; set; }

        [Ignore]
        public virtual Author Colleague { get; set; }

        [Ignore]
        public virtual ICollection<Coauthor> CoauthoredBooks { get; set; }

        #endregion

        #region Constructors 

        public Author()
        {

        }

        public Author(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public Author(int id, string name, DateTime birthday)
        {
            Id = id;
            Name = name;
            Birthday = birthday;
        }

        public Author(string name, string city, DateTime birthday, int? id = null)
        {
            if (id.HasValue)
            {
                Id = id.Value;
            }
            Name = name;
            City = city;
            Birthday = birthday;
        }

        #endregion

        #region Public Methods

        public bool ValidateForCreateOrUpdate(out string brokenRule)
        {
            brokenRule = string.Empty;

            if (Name.IsNullOrEmpty())
            {
                brokenRule = "Name is a required field.";
                return false;
            }
            else if (Birthday == null || (Birthday.Year == 1))
            {
                brokenRule = "Birthday is a required field and must be in 01.17.2022 OR 01/07/2022 format!";
                return false;
            }

            return true;
        }

        public bool ValidateForGetByName(out string brokenRule)
        {
            brokenRule = string.Empty;

            if (Name.IsNullOrEmpty())
            {
                brokenRule = "No name entered. Please enter an author's name";
                return false;
            }

            return true;
        }

        #endregion
    }
}