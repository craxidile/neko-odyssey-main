using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using SpatiumInteractive.Libraries.Unity.Platform.Extensions;

public class ESUIManager : MonoBehaviour
{
    [Header("General")]
    public GameObject ModalWindow;
    public TextMeshProUGUI ModalMessageTxt;

    [Header("1. Create Author Test Form")]
    [SerializeField] private TMP_InputField _createAuthorFirstAndLastNameTxt;
    [SerializeField] private TMP_InputField _createAuthorCityTxt;
    [SerializeField] private TMP_InputField _createAuthorDateOfBirthTxt;

    [Header("2. Get Author Test Form")]
    [SerializeField] private TMP_InputField _getAuthorByIdInputTxt;
    [SerializeField] private TMP_InputField _getAuthorByNameInputTxt;
    [SerializeField] private TMP_Text _getAuthorSearchResultTxt;

    [Header("3. Get All Authors Test Form")]
    [SerializeField] private Transform _authorInListParent;
    [SerializeField] private GameObject _authorInListPrefab;
    [SerializeField] private TMP_Text _authorInListTitleTxt;

    [Header("4. Update Author Test Form")]
    [SerializeField] private Transform _updateAuthorInListParent;
    [SerializeField] private GameObject _updateAuthorInListPrefab;
    [SerializeField] private TMP_Text _updateAuthorInListTitleTxt;

    [Header("4.b Update Author Popup Form")]
    [SerializeField] private GameObject _updateAuthorPopup;
    [SerializeField] private TMP_InputField _updateAuthorPopupIdTxt;
    [SerializeField] private TMP_InputField _updateAuthorPopupFirstAndLastNameTxt;
    [SerializeField] private TMP_InputField _updateAuthorPopupCityTxt;
    [SerializeField] private TMP_InputField _updateAuthorPopupDateOfBirthTxt;

    [Header("5. Delete Author Test Form")]
    [SerializeField] private Transform _deleteAuthorInListParent;
    [SerializeField] private GameObject _deleteAuthorInListPrefab;
    [SerializeField] private TMP_Text _deleteAuthorInListTitleTxt;

    [Header("5.b Delete Author Popup Form")]
    [SerializeField] private GameObject _deleteAuthorPopup;
    [SerializeField] private TMP_Text _deleteAuthorPopupMainMessageTxt;

    [Header("6. Create Book Test Form")]
    [SerializeField] private TMP_InputField _createBookTitleTxt;
    [SerializeField] private TMP_InputField _createBookYearTxt;
    [SerializeField] private TMP_InputField _createBookAuthorIdTxt;

    [Header("7. Get Book Test Form")]
    [SerializeField] private TMP_InputField _getBookByIdInputTxt;
    [SerializeField] private TMP_InputField _getBookByTitleTxt;
    [SerializeField] private TMP_Text _getBookSearchResultTxt;

    [Header("8. Get All Books Test Form")]
    [SerializeField] private Transform _bookInListParent;
    [SerializeField] private GameObject _bookInListPrefab;
    [SerializeField] private TMP_Text _bookInListTitleTxt;

    [Header("9. Update Book Test Form")]
    [SerializeField] private Transform _updateBookInListParent;
    [SerializeField] private GameObject _updateBookInListPrefab;
    [SerializeField] private TMP_Text _updateBookInListTitleTxt;

    [Header("9.b Update Book Popup Form")]
    [SerializeField] private GameObject _updateBookPopup;
    [SerializeField] private TMP_InputField _updateBookPopupIdTxt;
    [SerializeField] private TMP_InputField _updateBookPopupTitleTxt;
    [SerializeField] private TMP_InputField _updateBookPopupYearTxt;
    [SerializeField] private TMP_InputField _updateBookPopupAuthorIdTxt;

    [Header("10. Delete Book Test Form")]
    [SerializeField] private Transform _deleteBookInListParent;
    [SerializeField] private GameObject _deleteBookInListPrefab;
    [SerializeField] private TMP_Text _deleteBookInListTitleTxt;

    [Header("10.b Delete Book Popup Form")]
    [SerializeField] private GameObject _deleteBookPopup;
    [SerializeField] private TMP_Text _deleteBookPopupMainMessageTxt;

    private static ESUIManager _instance;
    private IdentifyAuthorDto _lastClickedAuthorDtoInDeleteList;
    private IdentifyBookDto _lastClickedBookDtoInDeleteList;

    #region Lifecycle

    private void Awake()
    {
        _instance = this;
    }

    #endregion

    #region API - Get Forms' Data

    public static void DisplayModal(string message)
    {
        _instance.ModalMessageTxt.text = message;
        _instance.ModalWindow.SetActive(true);
    }

    public static CreateOrUpdateAuthorDto GetCreateAuthorFormData()
    {
        string name = _instance._createAuthorFirstAndLastNameTxt.text;
        string city = _instance._createAuthorCityTxt.text;
        DateTime? birthday = _instance._createAuthorDateOfBirthTxt.text.ToDate();
        birthday = birthday != null ? birthday : new DateTime(1, 1, 1);
        var formData = new CreateOrUpdateAuthorDto(name, city, birthday.Value);
        return formData;
    }

    public static CreateOrUpdateAuthorDto GetUpdateAuthorFormData()
    {
        int id = Convert.ToInt32(_instance._updateAuthorPopupIdTxt.text);
        string name = _instance._updateAuthorPopupFirstAndLastNameTxt.text;
        string city = _instance._updateAuthorPopupCityTxt.text;
        DateTime? birthday = _instance._updateAuthorPopupDateOfBirthTxt.text.ToDate();
        birthday = birthday != null ? birthday : new DateTime(1, 1, 1);
        var formData = new CreateOrUpdateAuthorDto(name, city, birthday.Value, id);
        return formData;
    }

    public static GetAuthorDto GetAuthorFormData(bool fetchById = true, bool fetchByName = false)
    {
        int idInput = default;
        var formData = new GetAuthorDto(idInput);

        if (fetchById)
        {
            try
            {
                idInput = Convert.ToInt32(_instance._getAuthorByIdInputTxt.text);
                formData = new GetAuthorDto(idInput);
            }
            catch
            {
                DisplayModal("Id must be a number and greater than zero.");
            }

        }
        else if (fetchByName)
        {
            string nameInput = _instance._getAuthorByNameInputTxt.text;
            if (!nameInput.IsNullOrWhiteSpace())
            {
                formData = new GetAuthorDto(nameInput);
            }
        }
        return formData;
    }

    public static IdentifyAuthorDto GetAuthorFromDeleteAuthorPopup()
    {
        return _instance._lastClickedAuthorDtoInDeleteList;
    }

    public static void RenderAuthorGetSearchResult(string fetchedAutorName)
    {
        _instance._getAuthorSearchResultTxt.text = "Fetched Author: " + fetchedAutorName;
    }

    public static void RenderAllAuthorsInGetAllAuthors(List<string> authors)
    {
        _instance._authorInListTitleTxt.text = "3.Get All Authors \n Total Authors found: " + authors.Count;

        for (int i = 0; i < _instance._authorInListParent.childCount; i++)
        {
            var existingAuthorInList = _instance._authorInListParent.GetChild(i);
            Destroy(existingAuthorInList.gameObject);
        }

        foreach (var authorName in authors)
        {
            var authorObj = Instantiate(_instance._authorInListPrefab, _instance._authorInListParent, false) as GameObject;
            authorObj.GetComponent<Text>().text = authorName;
        }
    }

    public static void RenderAllAuthorsInUpdateAuthor(List<IdentifyAuthorDto> authorDtos)
    {
        _instance._updateAuthorInListTitleTxt.text = "4.Update Author \n Total Authors found: " + authorDtos.Count;

        for (int i = 0; i < _instance._updateAuthorInListParent.childCount; i++)
        {
            var existingAuthorInList = _instance._updateAuthorInListParent.GetChild(i);
            Destroy(existingAuthorInList.gameObject);
        }

        foreach (var authorDto in authorDtos)
        {
            var authorObj = Instantiate(_instance._updateAuthorInListPrefab, _instance._updateAuthorInListParent, false) as GameObject;
            authorObj.GetComponent<OnAuthorInUpdateListBtnClickHandler>().InitHandler(authorDto);
            authorObj.transform.GetChild(0).GetComponent<TMP_Text>().text = authorDto.FirstAndLastName;
        }
    }

    public static void RenderAllAuthorsInDeleteAuthor(List<IdentifyAuthorDto> authorDtos)
    {
        _instance._deleteAuthorInListTitleTxt.text = "5.Delete Author \n Total Authors found: " + authorDtos.Count;

        for (int i = 0; i < _instance._deleteAuthorInListParent.childCount; i++)
        {
            var existingAuthorInList = _instance._deleteAuthorInListParent.GetChild(i);
            Destroy(existingAuthorInList.gameObject);
        }

        foreach (var authorDto in authorDtos)
        {
            var authorObj = Instantiate(_instance._deleteAuthorInListPrefab, _instance._deleteAuthorInListParent, false) as GameObject;
            authorObj.GetComponent<OnAuthorInDeleteListBtnClickHandler>().InitHandler(authorDto);
            authorObj.transform.GetChild(0).GetComponent<TMP_Text>().text = authorDto.FirstAndLastName;
        }
    }

    public static void RenderUpdateAuthorPopup(CreateOrUpdateAuthorDto authorDto)
    {
        _instance._updateAuthorPopup.SetActive(true);
        _instance._updateAuthorPopupIdTxt.text = authorDto.Id.ToString();
        _instance._updateAuthorPopupFirstAndLastNameTxt.text = authorDto.FirstAndLastName;
        _instance._updateAuthorPopupCityTxt.text = authorDto.City;
        _instance._updateAuthorPopupDateOfBirthTxt.text = authorDto.DateOfBirth.ToString("dd.MM.yyyy");
    }

    public static void RenderDeleteAuthorPopup(IdentifyAuthorDto authorDto)
    {
        _instance._deleteAuthorPopup.SetActive(true);
        _instance._deleteAuthorPopupMainMessageTxt.text = "Are you sure you want to delete \n " + authorDto.FirstAndLastName + " ?";
        _instance._lastClickedAuthorDtoInDeleteList = authorDto;
    }

    public static CreateOrUpdateBookDto GetCreateBookFormData()
    {
        string title = _instance._createBookTitleTxt.text;
        int? year = _instance._createBookYearTxt.text.GetValueOrNull<int>();  
        int? authorIdParsed = _instance._createBookAuthorIdTxt.text.GetValueOrNull<int>();
        int authorId = authorIdParsed ?? 0;
        var formData = new CreateOrUpdateBookDto(title, year, authorId);
        return formData;
    }

    public static CreateOrUpdateBookDto GetUpdateBookFormData()
    {
        int id = Convert.ToInt32(_instance._updateBookPopupIdTxt.text);
        string title = _instance._updateBookPopupTitleTxt.text;
        int year = Convert.ToInt32(_instance._updateBookPopupYearTxt.text);
        int authorId = Convert.ToInt32(_instance._updateBookPopupAuthorIdTxt.text);
        var formData = new CreateOrUpdateBookDto(title, year, authorId, id);
        return formData;
    }

    public static GetBookDto GetBookFormData(bool fetchById = true, bool fetchByTitle = false)
    {
        int idInput = default;
        var formData = new GetBookDto(idInput);

        if (fetchById)
        {
            try
            {
                idInput = Convert.ToInt32(_instance._getBookByIdInputTxt.text);
                formData = new GetBookDto(idInput);
            }
            catch
            {
                DisplayModal("Id must be a number and greater than zero.");
            }

        }
        else if (fetchByTitle)
        {
            string nameInput = _instance._getBookByTitleTxt.text;
            if (!nameInput.IsNullOrWhiteSpace())
            {
                formData = new GetBookDto(nameInput);
            }
        }
        return formData;
    }

    public static IdentifyBookDto GetBookFromDeleteBookPopup()
    {
        return _instance._lastClickedBookDtoInDeleteList;
    }

    public static void RenderBookGetSearchResult(string fetchedAutorName)
    {
        _instance._getBookSearchResultTxt.text = "Fetched Book: " + fetchedAutorName;
    }

    public static void RenderAllBooksInGetAllBooks(List<string> authors)
    {
        _instance._bookInListTitleTxt.text = "3.GET ALL BOOKS \n Total Books found: " + authors.Count;

        for (int i = 0; i < _instance._bookInListParent.childCount; i++)
        {
            var existingBookInList = _instance._bookInListParent.GetChild(i);
            Destroy(existingBookInList.gameObject);
        }

        foreach (var authorName in authors)
        {
            var authorObj = Instantiate(_instance._bookInListPrefab, _instance._bookInListParent, false) as GameObject;
            authorObj.GetComponent<Text>().text = authorName;
        }
    }

    public static void RenderAllBooksInUpdateBook(List<IdentifyBookDto> bookDtos)
    {
        _instance._updateBookInListTitleTxt.text = "4.Update Book \n Total Books found: " + bookDtos.Count;

        for (int i = 0; i < _instance._updateBookInListParent.childCount; i++)
        {
            var existingBookInList = _instance._updateBookInListParent.GetChild(i);
            Destroy(existingBookInList.gameObject);
        }

        foreach (var bookDto in bookDtos)
        {
            var bookObj = Instantiate(_instance._updateBookInListPrefab, _instance._updateBookInListParent, false) as GameObject;
            bookObj.GetComponent<OnBookInUpdateListBtnClickHandler>().InitHandler(bookDto);
            bookObj.transform.GetChild(0).GetComponent<TMP_Text>().text = bookDto.Title;
        }
    }

    public static void RenderAllBooksInDeleteBook(List<IdentifyBookDto> bookDtos)
    {
        _instance._deleteBookInListTitleTxt.text = "5.Delete Book \n Total Books found: " + bookDtos.Count;

        for (int i = 0; i < _instance._deleteBookInListParent.childCount; i++)
        {
            var existingBookInList = _instance._deleteBookInListParent.GetChild(i);
            Destroy(existingBookInList.gameObject);
        }

        foreach (var bookDto in bookDtos)
        {
            var bookObj = Instantiate(_instance._deleteBookInListPrefab, _instance._deleteBookInListParent, false) as GameObject;
            bookObj.GetComponent<OnBookInDeleteListBtnClickHandler>().InitHandler(bookDto);
            bookObj.transform.GetChild(0).GetComponent<TMP_Text>().text = bookDto.Title;
        }
    }

    public static void RenderUpdateBookPopup(CreateOrUpdateBookDto bookDto)
    {
        _instance._updateBookPopup.SetActive(true);
        _instance._updateBookPopupIdTxt.text = bookDto.Id.ToString();
        _instance._updateBookPopupTitleTxt.text = bookDto.Title;
        _instance._updateBookPopupYearTxt.text = bookDto.Year.ToString();
        _instance._updateBookPopupAuthorIdTxt.text = bookDto.AuthorId.ToString();
    }

    public static void RenderDeleteBookPopup(IdentifyBookDto bookDto)
    {
        _instance._deleteBookPopup.SetActive(true);
        _instance._deleteBookPopupMainMessageTxt.text = "Are you sure you want to delete \n " + bookDto.Title + " ?";
        _instance._lastClickedBookDtoInDeleteList = bookDto;
    }

    #endregion
}
