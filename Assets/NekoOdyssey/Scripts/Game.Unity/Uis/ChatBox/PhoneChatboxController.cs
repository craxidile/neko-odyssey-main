using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

using DG.Tweening;

public class PhoneChatboxController : MonoBehaviour
{
    #region Move this part to database or file save later
    //public class ChatHistoryElement
    //{
    //    public string npcId, chatId;
    //    public TextAsset chatCSV;
    //}

    Dictionary<string, List<string>> phoneChatHistories = new Dictionary<string, List<string>>(); //string : NpcId or ChatGroupId , string : chatCsvId
    #endregion



    enum PhoneChatBoxMenu { friendList, chatDetail }
    PhoneChatBoxMenu _currentChatBoxMenu = PhoneChatBoxMenu.friendList;

    bool _isMenuTransitioning;


    [Header("Chat List")]
    [SerializeField] CanvasGroup chatListCanvas;
    [SerializeField] Text chatList_headerText;
    [SerializeField] Transform chatList_parent;
    [SerializeField] GameObject chatList_friendPrefab;
    List<PhoneFriendListPresenter> friendListPresenters = new List<PhoneFriendListPresenter>();

    [Header("Chat Detail")]
    [SerializeField] CanvasGroup chatDetailCanvas;
    [SerializeField] TextAsset[] chatDetial_testCsv;
    [SerializeField] Text chatDetail_headerText;
    [SerializeField] ButtonHover chatDetail_answerButton;
    [SerializeField] Text chatDetail_answerText;
    [SerializeField] ButtonHover chatDetail_back;


    [SerializeField] Transform chatboxParent;
    [SerializeField] GameObject chatboxPrefab_text, chatboxPrefab_image, chatboxPrefab_location, chatboxPrefab_line, chatboxPrefab_newTextLine;

    List<PhoneChatboxPresenter> chatboxes_text, chatboxe_image, chatboxes_location;
    List<GameObject> chatboxes_line, chatboxes_newTextLine;



    //Dictionary<string, TextAsset> allChatBoxCSVs;
    public class ChatBoxElement
    {
        public string lineId;
        public string actor;
        public ChatboxType chatboxType;
        public string message;


        public bool isPlayer => actor.ToLower().Equals("player");
    }
    public class ChatBoxElementGroup
    {
        public string chatBoxGroupId;
        public Dictionary<string, List<ChatBoxElement>> chatboxElements = new Dictionary<string, List<ChatBoxElement>>();
    }
    List<ChatBoxElementGroup> chatBoxElementGroups = new List<ChatBoxElementGroup>();
    //Dictionary<string, List<ChatBoxElement>> chatboxElements = new Dictionary<string, List<ChatBoxElement>>();




    //Dictionary<string, TextAsset> phoneChatHistories = new Dictionary<string, List<ChatHistoryElement>>();


    public enum ChatboxType { text, image, location, newTextLine }


    private void Awake()
    {
        chatboxes_text = new List<PhoneChatboxPresenter>();
        chatboxe_image = new List<PhoneChatboxPresenter>();
        chatboxes_location = new List<PhoneChatboxPresenter>();
        chatboxes_line = new List<GameObject>();
        chatboxes_newTextLine = new List<GameObject>();

        //allChatBoxCSVs = new Dictionary<string, TextAsset>();

        //allChatHistoryElement = new List<ChatHistoryElement>();

        //phoneChatHistories.Add(chatDetial_testCsv);

        chatDetail_back.onClick.AddListener(() =>
        {
            ClosePanel_chatDetail();
        });
    }
    // Start is called before the first frame update
    void Start()
    {
        foreach (var csv in chatDetial_testCsv)
        {
            RegisterChatBoxData(csv);
        }
        //allChatBoxCSVs.Add(chatDetial_testCsv.name, chatDetial_testCsv);

        //CSVLoader_Chatbox(chatDetial_testCsv);

        //SetUpPanel(chatDetial_testCsv.name);

        //AddNewMessage(chatDetial_testCsv.name);

        TestingSetUp();
    }
    void TestingSetUp()
    {
        //var owndCSV = chatDetial_testCsv.First();
        //phoneChatHistories.Add(owndCSV.name, );
        AddChatHistory("PhoneChat - quest01_chat1");

        //SetUpChatDetailPanel("npc01");
    }

    // Update is called once per frame
    void Update()
    {

    }

    //[ContextMenu("ShowPanel")]
    public void ShowPanel_friendList()
    {

    }
    public void ClosePanel_friendList()
    {

    }

    public void ShowPanel_chatDetail(string chatGroupId)
    {
        SetUpChatDetailPanel(chatGroupId);

        //chatDetailCanvas.DOFade(1)

        if (_isMenuTransitioning) return;
        if (_currentChatBoxMenu == PhoneChatBoxMenu.chatDetail) return;


        _isMenuTransitioning = true;
        chatListCanvas.interactable = false;
        chatListCanvas.blocksRaycasts = false;
        chatListCanvas.DOFade(0, 0.3f)
           .OnComplete(() =>
           {
               //fade out complete
               _currentChatBoxMenu = PhoneChatBoxMenu.chatDetail;
               chatDetailCanvas.DOFade(1, 0.3f)
               .OnComplete(() =>
               {
                   //fade out complete
                   chatDetailCanvas.interactable = true;
                   chatDetailCanvas.blocksRaycasts = true;

                   _isMenuTransitioning = false;
               });
           });
    }
    public void ClosePanel_chatDetail()
    {
        if (_isMenuTransitioning) return;
        if (_currentChatBoxMenu != PhoneChatBoxMenu.chatDetail) return;


        _isMenuTransitioning = true;
        chatDetailCanvas.interactable = false;
        chatDetailCanvas.blocksRaycasts = false;
        chatDetailCanvas.DOFade(0, 0.3f)
           .OnComplete(() =>
           {
               //fade out complete
               _currentChatBoxMenu = PhoneChatBoxMenu.friendList;
               chatListCanvas.DOFade(1, 0.3f)
               .OnComplete(() =>
               {
                   //fade out complete
                   chatListCanvas.interactable = true;
                   chatListCanvas.blocksRaycasts = true;

                   _isMenuTransitioning = false;
               });
           });



        HideAllChatBoxes();
    }

    void SetUpChatDetailPanel(string chatGroupId)
    {
        //Debug.Log($"SetUpChatDetailPanel 0 ({chatGroupId})");
        var chatBoxElementGroup = chatBoxElementGroups.FirstOrDefault(group => group.chatBoxGroupId == chatGroupId);
        if (chatBoxElementGroup == null) return;
        //if (!chatboxElements.ContainsKey(chatId)) return;
        //Debug.Log("SetUpChatDetailPanel 1");
        //var chatboxDatas = chatboxElements[chatId];

        for (int i = 0; i < chatBoxElementGroup.chatboxElements.Count; i++)
        {
            var chatElementGroup = chatBoxElementGroup.chatboxElements.ElementAt(i);

            if (i > 0) CreateNewChatbox_Line();
            //}
            //foreach (var chatElementGroup in chatBoxElementGroup.chatboxElements)
            //{
            foreach (var chatBoxElement in chatElementGroup.Value)
            {
                var chatBoxPresenter = GetChatbox(chatBoxElement.chatboxType);
                if (chatBoxElement.isPlayer) //is player
                {
                    chatBoxPresenter.SetChatBoxSide(true);

                }
                else //is npc
                {
                    chatBoxPresenter.SetChatBoxSide(false);

                    var npcAsset = ScriptableHolder.Instance.GetNpcAsset(chatBoxElement.actor);
                    chatBoxPresenter.SetAvatar(npcAsset.phoneChatProfileIcon);

                    chatDetail_headerText.text = npcAsset.name;
                }


                //var message = chatBoxElement.message;
                if (chatBoxElement.chatboxType == ChatboxType.text)
                    chatBoxPresenter.SetText(chatBoxElement.message);


                if (chatBoxElement.chatboxType == ChatboxType.location || chatBoxElement.chatboxType == ChatboxType.image)
                {
                    var chatSprite = ScriptableHolder.Instance.GetImage(chatBoxElement.message);
                    chatBoxPresenter.SetImageMessage(chatSprite);
                }

            }



        }
    }

    void RegisterChatBoxData(TextAsset textAsset)
    {
        List<ChatBoxElement> newChatList = new List<ChatBoxElement>();
        string chatGroupId = "";
        //csv loader
        string[] lines = textAsset.text.Split('\n');

        for (int i = 1; i < lines.Length; i++)
        {
            var line = lines[i];
            string[] values = line.Trim().Split(',');
            List<string> row = new List<string>(values);

            if (string.IsNullOrEmpty(row.FirstOrDefault())) continue;

            if (row[0] != "-") chatGroupId = row[0];

            System.Enum.TryParse(row[3], true, out ChatboxType chatboxType);

            var lineId = row[1];
            var actor = row[2];
            var message = row[5].Trim(); //FIX LATER to use language manager
            if (chatboxType == ChatboxType.location || chatboxType == ChatboxType.image) message = row[4];

            bool extendLine = false;
            if (newChatList.Count > 0) //Check for Extened text message
            {
                var lastestChatBoxElement = newChatList.Last();

                if (lastestChatBoxElement.lineId == lineId)
                    extendLine = true;

                if (!string.IsNullOrEmpty(message) && message[0] == '_')
                {
                    extendLine = true;
                    message = message.Substring(1);
                }

                if (extendLine)
                {
                    lastestChatBoxElement.message += "\n" + message;
                }

            }

            if (!extendLine) //Normal text message
            {
                var newChatBoxElement = new ChatBoxElement
                {
                    lineId = lineId,
                    actor = actor,
                    chatboxType = chatboxType,
                    message = message,
                };
                newChatList.Add(newChatBoxElement);
            }

        }

        //chatboxElements.Add(textAsset.name, newChatList);
        var newChatGroup = GetChatBoxElementGroup(chatGroupId);
        newChatGroup.chatboxElements.Add(textAsset.name, newChatList);
    }
    ChatBoxElementGroup GetChatBoxElementGroup(string chatGroupId)
    {
        var chatBoxElementGroup = chatBoxElementGroups.FirstOrDefault(group => group.chatBoxGroupId == chatGroupId);
        if (chatBoxElementGroup == null)
        {
            var newGroup = new ChatBoxElementGroup { chatBoxGroupId = chatGroupId };
            chatBoxElementGroups.Add(newGroup);
            return newGroup;
        }
        else
        {
            return chatBoxElementGroup;
        }
    }


    PhoneChatboxPresenter GetChatbox(ChatboxType chatboxType)
    {
        switch (chatboxType)
        {
            case ChatboxType.text:
                foreach (var chatbox in chatboxes_text)
                {
                    if (!chatbox.gameObject.activeSelf)
                    {
                        chatbox.transform.SetAsLastSibling();
                        chatbox.gameObject.SetActive(true);
                        return chatbox;
                    }
                }
                return CreateNewChatbox_Text();
            //if (chatboxElement.actor.ToLower().Equals("player"))
            //{
            //    foreach (var chatbox in chatboxes_player)
            //    {
            //        if (chatbox.gameObject.activeSelf)
            //        {
            //            return chatbox;
            //        }
            //    }
            //    return CreateNewChatbox_Text();
            //}
            //else
            //{
            //    foreach (var chatbox in chatboxe_npcText)
            //    {
            //        if (chatbox.gameObject.activeSelf)
            //        {
            //            return chatbox;
            //        }
            //    }
            //    return CreateNewChatbox_Image();
            //}


            case ChatboxType.image:
                foreach (var chatbox in chatboxe_image)
                {
                    if (!chatbox.gameObject.activeSelf)
                    {
                        chatbox.transform.SetAsLastSibling();
                        chatbox.gameObject.SetActive(true);
                        return chatbox;
                    }
                }
                return CreateNewChatbox_Image();
            case ChatboxType.location:
                foreach (var chatbox in chatboxes_location)
                {
                    if (!chatbox.gameObject.activeSelf)
                    {
                        chatbox.transform.SetAsLastSibling();
                        chatbox.gameObject.SetActive(true);
                        return chatbox;
                    }
                }
                return CreateNewChatbox_Location();
            default:
                return null;
        }
    }
    PhoneChatboxPresenter CreateNewChatbox_Text()
    {
        var newObject = Instantiate(chatboxPrefab_text, chatboxParent);
        newObject.SetActive(true);

        var chatboxPresenter = newObject.GetComponent<PhoneChatboxPresenter>();
        chatboxes_text.Add(chatboxPresenter);

        return chatboxPresenter;
    }

    PhoneChatboxPresenter CreateNewChatbox_Image()
    {
        var newObject = Instantiate(chatboxPrefab_image, chatboxParent);
        newObject.SetActive(true);

        var chatboxPresenter = newObject.GetComponent<PhoneChatboxPresenter>();
        chatboxe_image.Add(chatboxPresenter);

        return chatboxPresenter;
    }
    PhoneChatboxPresenter CreateNewChatbox_Location()
    {
        var newObject = Instantiate(chatboxPrefab_location, chatboxParent);
        newObject.SetActive(true);

        var chatboxPresenter = newObject.GetComponent<PhoneChatboxPresenter>();
        chatboxes_location.Add(chatboxPresenter);

        return chatboxPresenter;
    }
    void CreateNewChatbox_Line()
    {
        foreach (var line in chatboxes_line)
        {
            if (!line.activeSelf)
            {
                line.transform.SetAsLastSibling();
                line.SetActive(true);
                return;
            }
        }

        var newLine = Instantiate(chatboxPrefab_line, chatboxParent);
        chatboxes_line.Add(newLine);
        newLine.SetActive(true);

    }
    void CreateNewChatbox_NewTextLine()
    {
        foreach (var line in chatboxes_newTextLine)
        {
            if (!line.activeSelf)
            {
                line.transform.SetAsLastSibling();
                line.SetActive(true);
                return;
            }
        }

        var newLine = Instantiate(chatboxPrefab_newTextLine, chatboxParent);
        chatboxes_newTextLine.Add(newLine);
        newLine.SetActive(true);

    }

    void HideAllChatBoxes()
    {
        foreach (var item in chatboxes_text)
        {
            item.gameObject.SetActive(false);
        }
        foreach (var item in chatboxe_image)
        {
            item.gameObject.SetActive(false);
        }
        foreach (var item in chatboxes_location)
        {
            item.gameObject.SetActive(false);
        }
        foreach (var item in chatboxes_line)
        {
            item.gameObject.SetActive(false);
        }
        foreach (var item in chatboxes_newTextLine)
        {
            item.gameObject.SetActive(false);
        }


    }

    public void AddChatHistory(string chatId)
    {
        var chatBoxElementGroup = chatBoxElementGroups.FirstOrDefault(group => group.chatboxElements.ContainsKey(chatId));
        //if (chatBoxElementGroup == null)
        //{
        //    var newChatBoxElementGroup = new ChatBoxElementGroup();
        //    chatBoxElementGroups.Add(newChatBoxElementGroup);
        //    chatBoxElementGroup = newChatBoxElementGroup;
        //}
        var chatBoxGroupId = chatBoxElementGroup.chatBoxGroupId;

        phoneChatHistories.TryAdd(chatBoxGroupId, new List<string>());

        phoneChatHistories[chatBoxGroupId].Add(chatId);


        var friendPresenter = GetFriend(chatBoxGroupId);
        var lastestChatElement = chatBoxElementGroup.chatboxElements[chatId].Last();
        var lastestMessage = lastestChatElement.message;
        switch (lastestChatElement.chatboxType)
        {
            case ChatboxType.image:
                lastestMessage = "Send image";
                break;
            case ChatboxType.location:
                lastestMessage = "Send location";
                break;
            default:
                break;
        }
        if (lastestChatElement.isPlayer)
        {
            lastestMessage = lastestMessage.Insert(0, "you : ");
        }
        friendPresenter.UpdateMessage(lastestMessage);
    }


    //public void RegisterChatBoxCSV(TextAsset textAsset)
    //{

    //}


    public PhoneFriendListPresenter GetFriend(string chatGroupId)
    {
        var friendPresenter = friendListPresenters.FirstOrDefault(friend => friend.chatGroupId == chatGroupId);
        if (friendPresenter == null)
        {
            var newFriend = Instantiate(chatList_friendPrefab, chatList_parent);
            newFriend.gameObject.SetActive(true);

            friendPresenter = newFriend.GetComponent<PhoneFriendListPresenter>();
            friendPresenter.chatGroupId = chatGroupId;
            friendPresenter.SetupAppearance(chatGroupId);

            friendListPresenters.Add(friendPresenter);

            friendPresenter.button.onClick.AddListener(() =>
            {
                ShowPanel_chatDetail(chatGroupId);
            });
        }

        return friendPresenter;
    }
}
