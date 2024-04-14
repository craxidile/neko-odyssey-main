using System;
using System.Collections.Generic;
using SQLite4Unity3d;
using SpatiumInteractive.Libraries.Unity.GRU.Base;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;
using SpatiumInteractive.Libraries.Unity.Platform.Extensions;

namespace Database.Repository
{
    /// <summary>
    /// example class of author's books that you can use
    /// for testing and playing around with GRU's 
    /// capabilities and features. A book has one author
    /// and one author can have many books.
    /// </summary>
    [Serializable]
    public class Book: EntityBase<int>, IAggregateRoot
    {
        #region Properties 

        [NotNull]
        [Indexed]
        [ForeignKey(typeof(Author))] 
        public int AuthorId { get; set; }

        [Ignore]
        public virtual Author Author { get; set; }

        [Ignore]
        public virtual ICollection<Coauthor> Coauthors { get; set; }

        [NotNull]
        public string Title { get; set; }

        public int? Year { get; set; }

        #endregion

        #region Constructors 

        public Book()
        {

        }

        public Book(string title)
        {
            Title = title;
        }

        public Book(int id, string name)
        {
            Id = id;
            Title = name;
        }

        public Book(string title, int? year, int authorId, int? id = null)
        {
            if (id.HasValue)
            {
                Id = id.Value;
            }
            AuthorId = authorId;
            Title = title;
            Year = year;
        }

        #endregion

        #region Public Methods

        public bool ValidateForCreateOrUpdate(out string brokenRule)
        {
            brokenRule = string.Empty;

            if (Title.IsNullOrEmpty() || Title.Length < 2)
            {
                brokenRule = "Title is a required field. Min 2 characters required.";
                return false;
            }

            else if (AuthorId == 0)
            {
                brokenRule = "Author ID is a required field. Please enter ID of an author of this book. ID must valid, i.e. exist in table of authors";
                return false;
            }

            return true;
        }

        public bool ValidateForGetByTitle(out string brokenRule)
        {
            brokenRule = string.Empty;

            if (Title.IsNullOrEmpty())
            {
                brokenRule = "No title entered. Please enter book's name";
                return false;
            }

            return true;
        }

        #endregion
    }
}