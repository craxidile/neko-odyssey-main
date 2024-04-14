using UnityEngine;
using System.Linq;
using Database.Repository;
using Assets.Scripts.Integration.GRU.Repositories;
using Assets.Scripts.Integration.GRU.DbContexts;
using System;

public class MainManager : MonoBehaviour
{
    #region Repos for Bookshop Example Db

    private IAuthorRepository _authorRepository;
    private IBookRepository _bookRepository;

    #endregion

    #region Inspector Fields

    [SerializeField] private DBHandler _dbHandlerBookShopExampleDb;

    #endregion

    #region Private Fields

    private AuthorFactory _authorFactory;
    private BookFactory _bookFactory;

    #endregion

    #region Lifecycle 

    private void Awake()
    {
        _authorFactory = new AuthorFactory();
        _bookFactory = new BookFactory();
    }

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        TryAutoPopulatingDemoBookshopDbHandler();

        var dbConnection = _dbHandlerBookShopExampleDb.GetConnection();
        var dbContext = new BookshopexampledbDbContext(dbConnection);
        _authorRepository = new AuthorRepository(dbContext);
        _bookRepository = new BookRepository(dbContext);

        OnCreateAuthorBtnClickHandler.OnCreateAuthorBtnClick += OnCreateAuthorTest;
        OnGetAuthorByIdBtnClickHandler.OnGetAuthorByIdBtnClick += OnGetAuthorByIdTest;
        OnGetAuthorByNameBtnClickHandler.OnGetAuthorByNameBtnClick += OnGetAuthorByNameTest;
        OnGetAllAuthorsBtnClickHandler.OnGetAllAuthorsBtnClick += OnGetAllAuthorsTest;
        OnGetAllAuthorsForUpdateBtnClickHandler.OnGetAllAuthorsForUpdateBtnClick += OnGetAllAuthorsForUpdateTest;
        OnAuthorInUpdateListBtnClickHandler.OnAuthorInUpdateListBtnClick += OnAuthorInUpdateListSelected;
        OnUpdateAuthorBtnClickHandler.OnUpdateAuthorBtnClick += OnUpdateAuthorTest;
        OnGetAllAuthorsForDeleteBtnClickHandler.OnGetAllAuthorsForDeleteBtnClick += OnGetAllAuthorsForDeleteTest;
        OnAuthorInDeleteListBtnClickHandler.OnAuthorInDeleteListBtnClick += OnAuthorInDeleteListSelected;
        OnDeleteAuthorBtnClickHandler.OnDeleteAuthorBtnClick += OnDeleteAuthorTest;

        OnCreateBookBtnClickHandler.OnCreateBookBtnClick += OnCreateBookTest;
        OnGetBookByIdBtnClickHandler.OnGetBookByIdBtnClick += OnGetBookByIdTest;
        OnGetBookByNameBtnClickHandler.OnGetBookByNameBtnClick += OnGetBookByNameTest;
        OnGetAllBooksBtnClickHandler.OnGetAllBooksBtnClick += OnGetAllBooksTest;
        OnGetAllBooksForUpdateBtnClickHandler.OnGetAllBooksForUpdateBtnClick += OnGetAllBooksForUpdateTest;
        OnBookInUpdateListBtnClickHandler.OnBookInUpdateListBtnClick += OnBookInUpdateListSelected;
        OnUpdateBookBtnClickHandler.OnUpdateBookBtnClick += OnUpdateBookTest;
        OnGetAllBooksForDeleteBtnClickHandler.OnGetAllBooksForDeleteBtnClick += OnGetAllBooksForDeleteTest;
        OnBookInDeleteListBtnClickHandler.OnBookInDeleteListBtnClick += OnBookInDeleteListSelected;
        OnDeleteBookBtnClickHandler.OnDeleteBookBtnClick += OnDeleteBookTest;
    }

    #endregion

    #region Event Handlers (Called on Button Clicks in UI)

    private void OnCreateAuthorTest()
    {
        CreateAuthorTest();
    }

    private void OnGetAuthorByIdTest()
    {
        GetAuthorByIdTest();
    }

    private void OnGetAuthorByNameTest()
    {
        GetAuthorByNameTest();
    }

    private void OnGetAllAuthorsTest()
    {
        GetAllAuthorsTest();
    }

    private void OnGetAllAuthorsForUpdateTest()
    {
        GetAllAuthorsForUpdateTest();
    }

    private void OnAuthorInUpdateListSelected(IdentifyAuthorDto authorDto)
    {
        LoadUpdateAuthorPopup(authorDto);
    }

    private void OnUpdateAuthorTest()
    {
        UpdateAuthorTest();
    }

    private void OnGetAllAuthorsForDeleteTest()
    {
        GetAllAuthorsForDeleteTest();
    }

    private void OnAuthorInDeleteListSelected(IdentifyAuthorDto authorDto)
    {
        LoadDeleteAuthorPopup(authorDto);
    }

    private void OnDeleteAuthorTest()
    {
        DeleteAuthorTest();
    }

    private void OnCreateBookTest()
    {
        CreateBookTest();
    }

    private void OnGetBookByIdTest()
    {
        GetBookByIdTest();
    }

    private void OnGetBookByNameTest()
    {
        GetBookByTitleTest();
    }

    private void OnGetAllBooksTest()
    {
        GetAllBooksTest();
    }

    private void OnGetAllBooksForUpdateTest()
    {
        GetAllBooksForUpdateTest();
    }

    private void OnBookInUpdateListSelected(IdentifyBookDto authorDto)
    {
        LoadUpdateBookPopup(authorDto);
    }

    private void OnUpdateBookTest()
    {
        UpdateBookTest();
    }

    private void OnGetAllBooksForDeleteTest()
    {
        GetAllBooksForDeleteTest();
    }

    private void OnBookInDeleteListSelected(IdentifyBookDto authorDto)
    {
        LoadDeleteBookPopup(authorDto);
    }

    private void OnDeleteBookTest()
    {
        DeleteBookTest();
    }

    #endregion

    #region Private Methods

    private void CreateAuthorTest()
    {
        var formData = ESUIManager.GetCreateAuthorFormData();
        var entity = _authorFactory.Map(formData);
        string errorMessage = string.Empty;
        bool isValid = entity.ValidateForCreateOrUpdate(out errorMessage);
        if (isValid)
        {
            _authorRepository.Add(entity);
            ESUIManager.DisplayModal("Author Created Successfully");
        }
        else
        {
            ESUIManager.DisplayModal(errorMessage);
        }
    }

    private void GetAuthorByIdTest()
    {
        var formData = ESUIManager.GetAuthorFormData();
        var entity = _authorFactory.Map(formData);

        var author = _authorRepository.TestGetAuthorWithIdAndNameSelectedOnly(entity.Id);
        
        if (author != null)
        {
            ESUIManager.DisplayModal("Author Fetched");
            ESUIManager.RenderAuthorGetSearchResult(author.Name);
        }
        else
        {
            ESUIManager.DisplayModal("No author found in the db under such Id");
        }
    }

    private void GetAuthorByNameTest()
    {
        var formData = ESUIManager.GetAuthorFormData(false, true);
        var entity = _authorFactory.Map(formData);
        string errorMessage = string.Empty;
        bool isValid = entity.ValidateForGetByName(out errorMessage);
        if (isValid)
        {
            var author = _authorRepository.TestGetAllWhereNameStartsWithCharacter(entity.Name[0]).FirstOrDefault();
            if (author != null)
            {
                ESUIManager.DisplayModal("Author Fetched Successfully!");
                ESUIManager.RenderAuthorGetSearchResult(author.Name);
            }
            else
            {
                ESUIManager.DisplayModal("No author found in the db under such name");
                ESUIManager.RenderAuthorGetSearchResult("[doesn't exist]");
            }
        }
        else
        {
            ESUIManager.DisplayModal(errorMessage);
        }

    }

    private void GetAllAuthorsTest()
    {
        var entities = _authorRepository.GetAll();
        //var entities = _authorRepository.GetAllDistinctByName(); //in case you want to fetch them distincted by name

        if (entities == null || !entities.Any())
        {
            ESUIManager.DisplayModal("No authors currently in the database. Create one to get started ('1. Create Author Test' button )");
        }
        else
        {
            var authorNames = entities.Select(x => x.Name).ToList();
            ESUIManager.RenderAllAuthorsInGetAllAuthors(authorNames);
        }
    }

    private void GetAllAuthorsForUpdateTest()
    {
        var entities = _authorRepository.GetAll();
        if (entities == null || !entities.Any())
        {
            ESUIManager.DisplayModal("No authors currently in the database. Create one to get started ('1. Create Author Test' button )");
        }
        else
        {
            var authorDtos = _authorFactory.Map(entities);
            ESUIManager.RenderAllAuthorsInUpdateAuthor(authorDtos);
        }
    }

    private void LoadUpdateAuthorPopup(IdentifyAuthorDto authorDto)
    {
        var entity = _authorRepository.Get(authorDto.Id);
        CreateOrUpdateAuthorDto updateAuthorDto = _authorFactory.Map(entity);
        ESUIManager.RenderUpdateAuthorPopup(updateAuthorDto);
    }

    private void UpdateAuthorTest()
    {
        var formData = ESUIManager.GetUpdateAuthorFormData();
        var entity = _authorFactory.Map(formData);
        string errorMessage = string.Empty;
        bool isValid = entity.ValidateForCreateOrUpdate(out errorMessage);
        if (isValid)
        {
            _authorRepository.Update(entity);
            ESUIManager.DisplayModal("Author Updated Successfully");
        }
        else
        {
            ESUIManager.DisplayModal(errorMessage);
        }
    }

    private void GetAllAuthorsForDeleteTest()
    {
        var entities = _authorRepository.GetAll();
        if (entities == null || !entities.Any())
        {
            ESUIManager.DisplayModal("No authors currently in the database. Create one to get started ('1. Create Author Test' button )");
        }
        else
        {
            var authorDtos = _authorFactory.Map(entities);
            ESUIManager.RenderAllAuthorsInDeleteAuthor(authorDtos);
        }
    }

    private void LoadDeleteAuthorPopup(IdentifyAuthorDto authorDto)
    {
        ESUIManager.RenderDeleteAuthorPopup(authorDto);
    }

    private void DeleteAuthorTest()
    {
        var authorDto = ESUIManager.GetAuthorFromDeleteAuthorPopup();
        int id = authorDto.Id;
        _authorRepository.Remove(id);
        GetAllAuthorsForDeleteTest();
        ESUIManager.DisplayModal("Author " + authorDto.FirstAndLastName + " deleted successfully from the database");
    }

    private void CreateBookTest()
    {
        var formData = ESUIManager.GetCreateBookFormData();
        var entity = _bookFactory.Map(formData);
        bool doesAuthorIdExist = _authorRepository.Get(entity.AuthorId) != null;
        if (!doesAuthorIdExist)
        {
            entity.AuthorId = default;
        }
        string errorMessage = string.Empty;
        bool isValid = entity.ValidateForCreateOrUpdate(out errorMessage);
        if (isValid)
        {
            _bookRepository.Add(entity);
            ESUIManager.DisplayModal("Book Created Successfully");
        }
        else
        {
            ESUIManager.DisplayModal(errorMessage);
        }
    }

    private void GetBookByIdTest()
    {
        var formData = ESUIManager.GetBookFormData();
        var entity = _bookFactory.Map(formData);
        var book = _bookRepository.Get(entity.Id);
        if (book != null)
        {
            ESUIManager.DisplayModal("Book Fetched");
            ESUIManager.RenderBookGetSearchResult(book.Title);
        }
        else
        {
            ESUIManager.DisplayModal("No book found in the db under such Id");
        }
    }

    private void GetBookByTitleTest()
    {
        var formData = ESUIManager.GetBookFormData(false, true);
        var entity = _bookFactory.Map(formData);
        string errorMessage = string.Empty;
        bool isValid = entity.ValidateForGetByTitle(out errorMessage);
        if (isValid)
        {
            var book = _bookRepository.GetAll().SingleOrDefault(x => x.Title.Contains(entity.Title));
            if (book != null)
            {
                ESUIManager.DisplayModal("Book Fetched Successfully!");
                ESUIManager.RenderBookGetSearchResult(book.Title);
            }
            else
            {
                ESUIManager.DisplayModal("No book found in the db under such title");
                ESUIManager.RenderBookGetSearchResult("[doesn't exist]");
            }
        }
        else
        {
            ESUIManager.DisplayModal(errorMessage);
        }
    }

    private void GetAllBooksTest()
    {
        var entities = _bookRepository.GetAll();
        if (entities == null || !entities.Any())
        {
            ESUIManager.DisplayModal("No books currently in the database. Create one to get started ('6. Create Book Test' button )");
        }
        else
        {
            var bookTitles = entities.Select(x => x.Title).ToList();
            ESUIManager.RenderAllBooksInGetAllBooks(bookTitles);
        }
    }

    private void GetAllBooksForUpdateTest()
    {
        var entities = _bookRepository.GetAll();
        if (entities == null || !entities.Any())
        {
            ESUIManager.DisplayModal("No books currently in the database. Create one to get started ('6. Create Book Test' button )");
        }
        else
        {
            var bookDtos = _bookFactory.Map(entities);
            ESUIManager.RenderAllBooksInUpdateBook(bookDtos);
        }
    }

    private void LoadUpdateBookPopup(IdentifyBookDto authorDto)
    {
        var entity = _bookRepository.Get(authorDto.Id);
        CreateOrUpdateBookDto updateBookDto = _bookFactory.Map(entity);
        ESUIManager.RenderUpdateBookPopup(updateBookDto);
    }

    private void UpdateBookTest()
    {
        var formData = ESUIManager.GetUpdateBookFormData();
        var entity = _bookFactory.Map(formData);
        string errorMessage = string.Empty;
        bool isValid = entity.ValidateForCreateOrUpdate(out errorMessage);
        if (isValid)
        {
            _bookRepository.Update(entity);
            ESUIManager.DisplayModal("Book Updated Successfully");
        }
        else
        {
            ESUIManager.DisplayModal(errorMessage);
        }
    }

    private void GetAllBooksForDeleteTest()
    {
        var entities = _bookRepository.GetAll();
        if (entities == null || !entities.Any())
        {
            ESUIManager.DisplayModal("No books currently in the database. Create one to get started ('6. Create Book Test' button )");
        }
        else
        {
            var bookDtos = _bookFactory.Map(entities);
            ESUIManager.RenderAllBooksInDeleteBook(bookDtos);
        }
    }

    private void LoadDeleteBookPopup(IdentifyBookDto authorDto)
    {
        ESUIManager.RenderDeleteBookPopup(authorDto);
    }

    private void DeleteBookTest()
    {
        var bookDto = ESUIManager.GetBookFromDeleteBookPopup();
        int id = bookDto.Id;
        _bookRepository.Remove(id);
        GetAllBooksForDeleteTest();
        ESUIManager.DisplayModal("Book " + bookDto.Title + " deleted successfully from the database");
    }

    #endregion

    #region Helpers

    private void OnValidate()
    {
        TryAutoPopulatingDemoBookshopDbHandler();
    }

    /// <summary>
    /// if you plan of using only one database in your project,
    /// then you could use something like this in your consumer class 
    /// as well, in case you like to have everything automated.
    /// you would just have to adjust the db handler GO (and method) name
    /// to match yours.
    /// </summary>
    /// <exception cref="Exception"></exception>
    private void TryAutoPopulatingDemoBookshopDbHandler()
    {
        if (_dbHandlerBookShopExampleDb == null)
        {
            var dbHandlerInScene = GameObject.Find("DbHandler_BookshopExampleDb1");
            if (dbHandlerInScene == null)
            {
                throw new Exception("Game Object named DbHandler_BookshopExampleDb1 not found in the scene hierarchy. DbHandler_BookshopExampleDb1 is needed in example scene both for unity tests to run properly and for example scene logic to execute without errors when in play mode. Please add it back to the scene or manually create a new one (same name) and attach a db handler.cs script to it .");
            }
            else
            {
                _dbHandlerBookShopExampleDb = dbHandlerInScene.GetComponent<DBHandler>();
            }
        }
    }

    #endregion
}
