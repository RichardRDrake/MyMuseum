using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UI_SaveLoad : MonoBehaviour
{
    //UI SaveLoad is used by the three buttons associated with save files.
    //Found under UI_MainMenu/UI_SaveLoad/UI_ObjectsHide

    #region variables
    //Connects to the 
    //Or the canvas in the main scene
    private UI_MenuController mainMenuController;
    private UI_StartMenu startMenuController;

    [Header("Self Identifier")]
    public int identifier;

    [Header("Download Button Text Details")]
    public TextMeshProUGUI _FileName;
    public TextMeshProUGUI _Creator;
    public TextMeshProUGUI _DateModified;

    // Where to download the save file from the server
    private string m_DownloadURL;

    // The DC_RoomListPanel is just an empty class used to quickly find the one instance of this in the project
    // The panel that contains the lists of rooms should have this component
    private DC_RoomListPanel m_RoomListPanel;

    // Added Save/Load controller so that a room can be made and saved locally upon being downloaded
    private SaveLoadRoom m_SaveLoadRoomController;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        //Should find this if it's within the game scene 
        mainMenuController = FindObjectOfType<UI_MenuController>();
        if (!mainMenuController)
        {
            //Otherwise, it'll assume it's in the main menu
            //Debug.Log("Couldn't find mainMenuController.");
            //Debug.Log("Searching for startMenuController");
            startMenuController = FindObjectOfType<UI_StartMenu>();
            if (!startMenuController)
            {
                //Debug.Log("Couldn't find startMenuController");
            }
            else
            {
                //Debug.Log("Found startMenuController");
            }
        }

        // Find the room list panel
        m_RoomListPanel = FindObjectOfType<DC_RoomListPanel>();

        // Find the Save/Load room controller
        m_SaveLoadRoomController = FindObjectOfType<SaveLoadRoom>();
    }

    public void OnPressed()
    {
        if (!mainMenuController)
        {
            //Behaviour in the main menu
            startMenuController.saveLoadIdentity = identifier;
            startMenuController.paneCurrent = identifier;
            startMenuController.DisplayConfirmation();
        }
        else
        {
            //Behaviour in-game
            mainMenuController.saveLoadIdentity = identifier;
            if (startMenuController)
            {
                startMenuController.paneCurrent = identifier;
            }            
            Debug.Log("Changed to" + mainMenuController.saveLoadIdentity);
            mainMenuController.ShowConfirmation();
        }
    }

    /// <summary>
    /// Assign everything the button needs to display the room details and where to download the room from when clicked
    /// </summary>
    /// <param name="filename"></param>
    /// <param name="creator"></param>
    /// <param name="dateModified"></param>
    /// <param name="downloadURL"></param>
    public void AssignDetails(string filename, string creator, string dateModified, string downloadURL)
    {
        _FileName.text = filename;
        _Creator.text = creator;
        _DateModified.text = dateModified;

        // Where to download the room from
        m_DownloadURL = downloadURL;
    }

    /// <summary>
    /// This UI button can now be used to download a room rather than just Save/Load
    /// Apon downloading the room, it is made locally and saved locally
    /// Afterwhich the approriate menu is updated and the panel for showing the room lists is closed
    /// </summary>
    public void OnPressedDownload()
    {
        // Download the save file from the server
        StartCoroutine(AsyncDownloadFile());
    }

    private IEnumerator AsyncDownloadFile()
    {

        Uri URI = new Uri(m_DownloadURL);

        // Start downloading the room file
        StartCoroutine(DC_Downloader.DownloadRoom(URI.OriginalString, DC_NetworkManager.s_UserToken));

        // Wait until the entire text file has been downloaded
        while (DC_Downloader.isDownloading)
            yield return null;

        // Check to see if the generated link was valid,
        if (DC_Downloader.DownloadedRoomFile == null)
        {
            // TODO: If not display a warning letting the user know that no list could be found
            Debug.LogError("Could not find room on server @ " + URI.OriginalString);
        }
        else
        {
            // Now it has downloaded, make locally and save it
            if (m_SaveLoadRoomController)
            {
                // Make the room locally
                m_SaveLoadRoomController.MakeRoom(DC_Downloader.DownloadedRoomFile.roomString, DC_Downloader.DownloadedRoomFile.Assets, false);

                // Save it locally
                m_SaveLoadRoomController.Save(_FileName.text);

                // Refresh the list in the current menu
                if (!mainMenuController)
                    startMenuController.MenuSetup();
                else
                    mainMenuController.MenuSetup();
            }

            // Close the room list panel
            if (m_RoomListPanel)
                m_RoomListPanel.gameObject.SetActive(false);
        }
    }
}
