using System;
using SQLite4Unity3d;
using SpatiumInteractive.Libraries.Unity.GRU.Base;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;

namespace Database.Repository
{
    /// <summary>
    /// Coauthor example class. Used for giving examples of
    /// <i>many to many </i> relationships.
    /// <br/>
    /// (<i> One author can be a coauthor
    /// of many different books and one book can have many different
    /// coauthors </i>).
    /// </summary>
    [Serializable]
    public class Coauthor : EntityBase<int>, IAggregateRoot
    {
        #region Properties

        [NotNull]
        [Indexed]
        [ForeignKey(typeof(Author))]
        public int AuthorId { get; set; }

        [Ignore]
        public virtual Author Author { get; set; }

        [NotNull]
        [Indexed]
        [ForeignKey(typeof(Book))]
        public int BookId { get; set; }

        [Ignore]
        public virtual Book Book { get; set; }

        #endregion

        #region Constructors

        public Coauthor()
        {

        }

        public Coauthor(int authorId, int bookId)
        {
            AuthorId = authorId;
            BookId = bookId;
        }

        #endregion
    }
}
