using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ImaginationOverflow.UniversalDeepLinking;
using TMPro;
using System.Xml;
using System.IO;
using System;
using System.Globalization;
using UnityEngine.Events;
using UnityEngine.Networking;
using System.Text;

/// <summary>
/// This script takes care of...
/// Responding to deep links (that contain User ID)
/// Registering the program on the device (Part of the installation process, to allow deep links to work)
/// Download Private and Public Room lists and populating the appropriate content with modified "UI_SaveLoad" buttons (Which can be viewed and clicked to download an actual room file from the server)
/// </summary>
public class DC_NetworkManager : MonoBehaviour
{
    [Header("Registering Program")]
    [Tooltip("Enabled if not registering")]
    public GameObject _MenuButtons;
    [Tooltip("Disabled if not registering")]
    public GameObject _RegisterProductText;

    [Header("Found DeepLink Data")]
    [Tooltip("User Authentication Token")]
    // Already assigned for editor testing (Token will expire soon though)
    public static string s_UserToken = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJodHRwczpcL1wvbXltdXNldW0uZG9yc2V0Y3JlYXRpdmUudGVjaFwvZ2VuZXJhdGUtand0LXRva2VuIiwiaWF0IjoxNjI5OTgxNzk3LCJleHAiOjE2MzA1ODY1OTcsIm5iZiI6MTYyOTk4MTc5NywianRpIjoiaDJKbXdDYVp4NTFYMlFobiIsInN1YiI6NCwicHJ2IjoiMjNiZDVjODk0OWY2MDBhZGIzOWU3MDFjNDAwODcyZGI3YTU5NzZmNyJ9.O--HaDEiu2IQnYywOuhi-m-AQD-J7BI794wqHW9Jzyo";

    [Header("APIs")]
    public static string _URIPrefix = "https://mymuseum.dorsetcreative.tech/api/";
    public static string _URIPrefixRooms = "https://mymuseum.dorsetcreative.tech/storage/rooms/";
    public static string _APIGetUserProfile = "getUserProfile";
    public static string _APIGetPublicRoomList = "getPublicRooms";
    public static string _APIGetPrivateRoomList = "getPrivateRooms";
    public static string _APIUploadRoom = "uploadRoom";
    public static string _APIDeleteRoom = "deleteRoom";

    [Header("User Profile Data")]
    public static string s_UserName = "";
    public static string s_UserUniqueID = "";

    [Header("Save/Load Button List")]
    [Tooltip("Save/Load button prefab (Instantiated under expandable content fitter)")]
    public UI_SaveLoad _SaveLoadButtonPrefab;
    [Tooltip("Where instantiated Private Save/Load buttons are placed")]
    public Transform _PrivateRoomListContentTransform;
    [Tooltip("Where instantiated Public Save/Load buttons are placed")]
    public Transform _PublicRoomListContentTransform;

    [Header("Testing")]
    public TextMeshProUGUI _LoginText;

    [Serializable]
    public class RoomDetails
    {
        public string _ID;
        public string _Name;
        public string _Creator;
        public string _CreatorID;
        public DateTime _LastModified;
        public string _SaveFileLocation;
    }
    [Header("Room data")]
    [Tooltip("List of rooms (Downloaded from server)")]
    public List<RoomDetails> _PrivateRooms = new List<RoomDetails>();
    public List<RoomDetails> _PublicRooms = new List<RoomDetails>();

    // If the program was launched with -Register then this was only to register the program, so will immeditately close
    private bool _RegisteringOnly = false;

    // Triggered once the lists of rooms has been downloaded and ready for use
    private UnityEvent m_ListsDownloaded;

    // The DC_RoomListPanel is just an empty class used to quickly find the one instance of this in the project
    // The panel that contains the lists of rooms should have this component
    private DC_RoomListPanel m_RoomListPanel;

    private void Awake()
    {
        DeepLinkManager.Instance.LinkActivated += Instance_LinkActivated;

        // Event invoked when room list file has been downloaded and processed
        if (m_ListsDownloaded == null)
            m_ListsDownloaded = new UnityEvent();

        m_ListsDownloaded.AddListener(RoomListsDownloaded);

        // Find the room list panel
        m_RoomListPanel = FindObjectOfType<DC_RoomListPanel>();
    }

    private void Instance_LinkActivated(LinkActivation s)
    {
        //  Example <a href="mymuseum://?u=MD5CodeOrName"></a>
        s_UserToken = s.QueryString["u"];

        if (_LoginText)
            _LoginText.text = "User ID: " + s_UserToken + " : URL: " + s.Uri + " : Full:" + s.RawQueryString;

        // Save the key to the registry (in case they want to launch the application locally next time)
        PlayerPrefs.SetString("UserID", s_UserToken);
        PlayerPrefs.Save();
    }

    private void OnDestroy()
    {
        DeepLinkManager.Instance.LinkActivated -= Instance_LinkActivated;
    }

    private void Start()
    {
        // Check command line arguments
        string[] args = System.Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length; i++)
        {
            Debug.Log("ARG " + i + ": " + args[i]);

            // If batch file has -Register then it is only for registering the program, and will close down immediately
            if (args[i].Equals("-Register"))
            {
                _RegisteringOnly = true;
                return;
            }
        }

        // If no user ID was sent, this app was launched locally (Try finding the user key from the registry)
        if (s_UserToken.Equals("Not Found"))
        {
            // Try getting existing saved key
            s_UserToken = PlayerPrefs.GetString("UserID", "Not Found");

            // If the user ID is still "Not Found" then the app was never launched from the website
            if (s_UserToken.Equals("Not Found"))
            {
                // TODO: Tell the user that they can only save room locally or publicly
                // To save to their personal profile on the website, the app must be launched at least once from the website
            }
        }

        UpdateUserProfile();
    }

    private void Update()
    {
        // If only registering the program, close immediately (Part of the installation process)
        if (_RegisteringOnly)
            Application.Quit();
        // Otherwise enable the menu buttons and disable the "Registering Product" text
        else
        {
            _MenuButtons.SetActive(true);
            _RegisterProductText.SetActive(false);
        }
    }

    public void UpdateUserProfile()
    {
        StartCoroutine(DownloadUserProfile());
    }

    private IEnumerator DownloadUserProfile()
    {
        // Start downloading the list file
        StartCoroutine(DC_Downloader.DownloadText(_URIPrefix + _APIGetUserProfile, s_UserToken));

        // Wait until the entire text file has been downloaded
        while (DC_Downloader.isDownloading)
            yield return null;

        // Check to see if the generated link was valid,
        if (DC_Downloader.DownloadedTextFile != null )
        {
            Debug.Log(DC_Downloader.DownloadedTextFile);

            XmlDocument xmlDoc = new XmlDocument();

            xmlDoc.Load(new StringReader(DC_Downloader.DownloadedTextFile));

            XmlNode nameNode = xmlDoc.SelectSingleNode("//response/user/name");
            XmlNode idNode = xmlDoc.SelectSingleNode("//response/user/id");

            s_UserName = nameNode.InnerText;
            s_UserUniqueID = idNode.InnerText;

            if (_LoginText)
                _LoginText.text = "Username: " + s_UserName + " / ID: " + s_UserUniqueID; 
        }
    }

    public void UpdateRoomLists()
    {
        StartCoroutine(DownloadRoomLists());
    }

    private IEnumerator DownloadRoomLists()
    {
        _PrivateRooms.Clear();
        _PublicRooms.Clear();

        for (int I = 0; I < 2; I++)
        {
            Uri URI;

            // Private first
            if (I == 0)
                URI = new Uri(_URIPrefix + _APIGetPrivateRoomList);
            // Public second
            else
                URI = new Uri(_URIPrefix + _APIGetPublicRoomList);

            // Start downloading the list file
            StartCoroutine(DC_Downloader.DownloadText(URI.OriginalString, s_UserToken));

            // Wait until the entire text file has been downloaded
            while (DC_Downloader.isDownloading)
                yield return null;

            // Check to see if the generated link was valid,
            if (DC_Downloader.DownloadedStreamFile == null || DC_Downloader.DownloadedTextFile.Contains("Error with config"))
            {
                // TODO: If not display a warning letting the user know that no list could be found
                Debug.LogError("Could not find list of rooms from server @ " + URI.OriginalString);
            }
            else
            {
                Debug.Log(DC_Downloader.DownloadedTextFile);

                // Parse the downloaded XML file, poulating private(0)/public(1) RoomList
                ParseRoomListXmlFile(DC_Downloader.DownloadedTextFile, I == 0);

                // Invoke event to say room list has been downloaded and ready for use
                // When processing final list (Public)
                if (I == 1)
                    m_ListsDownloaded.Invoke();
            }
        }
    }

    /// <summary>
    /// Reads the XML document and stores the Room data into either a Private list of room details or public ones
    /// </summary>
    /// <param name="xmlData"></param>
    /// <param name="bPrivate"></param>
    private void ParseRoomListXmlFile(string xmlData, bool bPrivate)
    {
        XmlDocument xmlDoc = new XmlDocument();

        xmlDoc.Load(new StringReader(xmlData));

        string xmlPathPattern = "//response/rooms/room";

        XmlNodeList myNodeList = xmlDoc.SelectNodes(xmlPathPattern);

        CultureInfo provider = CultureInfo.InvariantCulture;

        foreach (XmlNode node in myNodeList)
        {
            XmlNode idNode = node.FirstChild;
            XmlNode uriNode = idNode.NextSibling;
            XmlNode dateNode = uriNode.NextSibling;
            XmlNode nameNode = dateNode.NextSibling;

            RoomDetails room = new RoomDetails();
            room._LastModified = DateTime.ParseExact(dateNode.InnerXml, "yyyy-MM-dd HH:mm:ss", provider);

            string roomFullPath = uriNode.InnerXml;

            // Split each part using "\"
            string[] stringParts_L1 = roomFullPath.Split('/', '\\');

            // Split the last part using "."
            string[] stringParts_L2 = stringParts_L1[stringParts_L1.Length - 1].Split('.');

            // Room name is first part
            room._Name = stringParts_L2[0];

            // Creator is in the xml file
            room._Creator = bPrivate? s_UserName : nameNode.InnerXml;

            // Creator ID is part of the save file
            room._CreatorID = stringParts_L2[1];

            // URI (The path used to download the file is the full path
            room._SaveFileLocation = _URIPrefixRooms + roomFullPath;

            // Unique ID of room on server (So it can be deleted)
            room._ID = idNode.InnerXml;

            // If loading a private list add to Private Rooms
            if (bPrivate)
                _PrivateRooms.Add(room);
            else
                _PublicRooms.Add(room);

            Debug.Log("Room: " + room._Name + " Created By: " + room._Creator + " Last Modified: " + room._LastModified.ToString() + " Room URI: " + roomFullPath);
        }
    }

    /// <summary>
    /// Invoked once the room lists have been downloaded and is ready for use
    /// </summary>
    private void RoomListsDownloaded()
    {
        // Remove all content from before (if there is any)
        foreach(Transform buttonObject in _PrivateRoomListContentTransform.GetComponentsInChildren<Transform>())
        {
            if(buttonObject != _PrivateRoomListContentTransform)
            {
                Destroy(buttonObject.gameObject);
            }
        }

        foreach (Transform buttonObject in _PublicRoomListContentTransform.GetComponentsInChildren<Transform>())
        {
            if (buttonObject != _PublicRoomListContentTransform)
            {
                Destroy(buttonObject.gameObject);
            }
        }

        // Populate the UI content field with each room found on the server

        // Private list
        foreach (RoomDetails roomDetails in _PrivateRooms)
        {
            UI_SaveLoad saveLoadButton = Instantiate(_SaveLoadButtonPrefab, _PrivateRoomListContentTransform, false);
            saveLoadButton.AssignDetails(roomDetails);
        }

        // Public list
        foreach (RoomDetails roomDetails in _PublicRooms)
        {
            UI_SaveLoad saveLoadButton = Instantiate(_SaveLoadButtonPrefab, _PublicRoomListContentTransform, false);
            saveLoadButton.AssignDetails(roomDetails);
        }

        // Open the room list panel
        if (m_RoomListPanel)
            m_RoomListPanel.gameObject.SetActive(true);
    }    
}
