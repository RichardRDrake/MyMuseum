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
    [Tooltip("MD5 Checksum")]
    public string _UserID = "Not found";

    [Header("Connection Settings")]
    public string _URLPrefix = "www.website.com/User/";
    public string _URLGetRoomListSuffix = "GetRoomList";

    [Header("Save/Load Button List")]
    [Tooltip("Save/Load button prefab (Instantiated under expandable content fitter)")]
    public UI_SaveLoad _SaveLoadButtonPrefab;
    [Tooltip("Where instantiated Private Save/Load buttons are placed")]
    public Transform _PrivateRoomListContentTransform;
    [Tooltip("Where instantiated Public Save/Load buttons are placed")]
    public Transform _PublicRoomListContentTransform;

    [Header("Testing")]
    public TextMeshProUGUI _LoginText;
    public TextMeshProUGUI _LevelText;

    [Serializable]
    public class RoomDetails
    {
        public string _Name;
        public string _Creator;
        public DateTime _LastModified;
        public string _SaveFileLocation;
    }
    [Header("Room data")]
    [Tooltip("List of rooms (Downloaded from server)")]
    public List<RoomDetails> _PrivateRooms = new List<RoomDetails>();
    public List<RoomDetails> _PublicRooms = new List<RoomDetails>();

    // If the program was launched with -Register then this was only to register the program, so will immeditately close
    private bool _RegisteringOnly = false;

    // In case of hiccups in the network, each download/upload will attempt 5 times in total, before giving up
    private int m_NetworkTimesTried = 0;

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
        _UserID = s.QueryString["u"];

        if (_LoginText)
            _LoginText.text = "User ID: " + _UserID + " : URL: " + s.Uri + " : Full:" + s.RawQueryString;

        // Save the key to the registry (in case they want to launch the application locally next time)
        PlayerPrefs.SetString("UserID", _UserID);
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
        if (_UserID.Equals("Not Found"))
        {
            // Try getting existing saved key
            _UserID = PlayerPrefs.GetString("UserID", "Not Found");

            // If the user ID is still "Not Found" then the app was never launched from the website
            if (_UserID.Equals("Not Found"))
            {
                // TODO: Tell the user that they can only save room locally or publicly
                // To save to their personal profile on the website, the app must be launched at least once from the website
            }
        }

        // Test
        UpdateRoomLists();
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

    public void UpdateRoomLists()
    {
        StartCoroutine(DownloadRoomLists());
    }

    private IEnumerator DownloadRoomLists()
    {
        for (int I = 0; I < 2; I++)
        {
            // Try 5 times before giving up
            Retry:

            Uri URI;

            // Private first
            if (I == 0)
                URI = new Uri("c:/" + _UserID + "/PrivateRoomList.xml");
            // Public second
            else
                URI = new Uri("c:/RoomList.xml");

            // Start downloading the list file
            StartCoroutine(DC_Downloader.DownloadText(URI.OriginalString));

            // Wait until the entire text file has been downloaded
            while (DC_Downloader.isDownloading)
                yield return null;

            // Check to see if the generated link was valid,
            if (DC_Downloader.DownloadedStreamFile == null || DC_Downloader.DownloadedTextFile.Contains("Error with config"))
            {
                // Try again 4 times (1 time a second)
                m_NetworkTimesTried++;
                if (m_NetworkTimesTried < 5)
                {
                    yield return new WaitForSeconds(1);
                    goto Retry;
                }

                // TODO: If not display a warning letting the user know that no list could be found
                Debug.LogError("Could not find list of rooms from server @ " + URI.OriginalString);
            }
            else
            {
                // Parse the downloaded XML file, poulating private(0)/public(1) RoomList
                ParseXmlFile(DC_Downloader.DownloadedTextFile, I == 0);

                // Reset times tried as it's used whenever we are trying to download something
                m_NetworkTimesTried = 0;

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
    private void ParseXmlFile(string xmlData, bool bPrivate)
    {
        XmlDocument xmlDoc = new XmlDocument();

        xmlDoc.Load(new StringReader(xmlData));

        string xmlPathPattern = "//Rooms/Room";

        XmlNodeList myNodeList = xmlDoc.SelectNodes(xmlPathPattern);

        CultureInfo provider = CultureInfo.InvariantCulture;

        foreach (XmlNode node in myNodeList)
        {
            XmlNode dateNode = node.FirstChild;
            XmlNode uriNode = dateNode.NextSibling;

            RoomDetails room = new RoomDetails();
            room._LastModified = DateTime.ParseExact(dateNode.InnerXml, "yyyy:MM:dd:HH:mm:ss", provider);

            string roomFullPath = uriNode.InnerXml;

            // Split each part using "\"
            string[] stringParts_L1 = roomFullPath.Split('/', '\\');

            // Split the last part using "."
            string[] stringParts_L2 = stringParts_L1[stringParts_L1.Length - 1].Split('.');

            // Room name is first part
            room._Name = stringParts_L2[0];

            // Creator is second part
            room._Creator = stringParts_L2[1];

            // URI (The path used to download the file is the full path
            room._SaveFileLocation = roomFullPath;

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
        // Populate the UI content field with each room found on the server

        // Private list
        foreach(RoomDetails roomDetails in _PrivateRooms)
        {
            UI_SaveLoad saveLoadButton = Instantiate(_SaveLoadButtonPrefab, _PrivateRoomListContentTransform, false);
            saveLoadButton.AssignDetails(roomDetails._Name, roomDetails._Creator, roomDetails._LastModified.ToString(), roomDetails._SaveFileLocation);
        }

        // Public list
        foreach (RoomDetails roomDetails in _PublicRooms)
        {
            UI_SaveLoad saveLoadButton = Instantiate(_SaveLoadButtonPrefab, _PublicRoomListContentTransform, false);
            saveLoadButton.AssignDetails(roomDetails._Name, roomDetails._Creator, roomDetails._LastModified.ToString(), roomDetails._SaveFileLocation);
        }

        // Open the room list panel
        if (m_RoomListPanel)
            m_RoomListPanel.gameObject.SetActive(true);
    }
}
