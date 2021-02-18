using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Controller : MonoBehaviour
{
    //Window showing list of placeable objects
    [SerializeField] private GameObject AssetRepository;

    //Panel for detailed view/confirming selection
    [SerializeField] private GameObject DetailPanel;

    //List of buttons relating to each category
    //(This form of sorting is for testing's sake and is definitely not considered final)
    [SerializeField] private GameObject FloorBased;
    [SerializeField] private GameObject FloorOrWall;
    [SerializeField] private GameObject Planar;
    [SerializeField] private GameObject Small;

    //The six objects, and a list in which to contain them
    private List<Text> objectDisplay = new List<Text>();
    [SerializeField] private GameObject Object1;
    [SerializeField] private GameObject Object2;
    [SerializeField] private GameObject Object3;
    [SerializeField] private GameObject Object4;
    [SerializeField] private GameObject Object5;
    [SerializeField] private GameObject Object6;

    //Visual representation of the objects themselves
    //So they can be hidden when loading a new page
    [SerializeField] private GameObject ObjectsHide;

    //Page number object
    [SerializeField] private GameObject PageCounter;
    //Page number text
    Text countText;

    private TempListScript Resources;
    //Length of Resources
    int listLength;

    //Used to determine which list to read from
    public int switchLists = 0;

    //Signals whether a menu needs to be displayed
    public bool displayMenu = false;

    int pageCount;
    int pageCurrent = 1;

    // Start is called before the first frame update
    void Start()
    {
        #region set variables
        //Gets the six inventory panes, and a resource list
        objectDisplay.Add(Object1.GetComponent<Text>());
        objectDisplay.Add(Object2.GetComponent<Text>());
        objectDisplay.Add(Object3.GetComponent<Text>());
        objectDisplay.Add(Object4.GetComponent<Text>());
        objectDisplay.Add(Object5.GetComponent<Text>());
        objectDisplay.Add(Object6.GetComponent<Text>());
        countText = PageCounter.GetComponent<Text>();

        //This debug TempListScript is NECESSARY. 
        Resources = GetComponent<TempListScript>();
        if (!Resources)
        {
            Debug.Log("Something went wrong");
        }
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown("space"))
        {
            MenuSetup();
        }

        if(displayMenu == true)
        {
            displayMenu = false;
            MenuSetup();
        }

    }

    private void MenuSetup()
    {
        //Hides listing and detail panel while values are refreshed
        DetailPanel.SetActive(false);
        AssetRepository.SetActive(false);

        //Determines that it's reading from the correct Resources.readFrom
        switch (switchLists)
        {
            case 0:
                Resources = FloorBased.GetComponent<TempListScript>();
                break;
            case 1:
                Resources = FloorOrWall.GetComponent<TempListScript>();
                break;
            case 2:
                Resources = Planar.GetComponent<TempListScript>();
                break;
            case 3:
                Resources = Small.GetComponent<TempListScript>();
                break;
        }

        //Creates a total page count based on number of objects in Resources.readFrom
        //Includes all full pages, plus a page for the remainder
        listLength = Resources.readFrom.Count;
        Debug.Log(listLength);
        pageCount = listLength / 6;
        if(listLength % 6 > 0)
        {
            pageCount++;
        }

        //Makes sure there is always one page
        if(listLength == 0)
        {
            pageCount = 1;
        }

        pageCurrent = 1;
        DisplayPageDetails();
        
        //Displays the AssetsRepository
        //(And 6 assets panes, if they were independently set inactive for any reason)
        AssetRepository.SetActive(true);
        ObjectsHide.SetActive(true);
    }

    private void DisplayPageDetails()
    {
        //Displays values in Resources.readFrom, in objectDisplay
        //Starts at the relevant point for each page, should scale indefinitely.
        countText.text = pageCurrent.ToString() + " / " + pageCount.ToString();
        for (int i = 0; i <= 5; i++)
        {
            if (((pageCurrent - 1) * 6) + i > (Resources.readFrom.Count - 1))
            {
                objectDisplay[i].text = " ";
            }
            else
            {
                objectDisplay[i].text = Resources.readFrom[((pageCurrent - 1) * 6) + i];
            }
        }
    }

    public void IncrementPage()
    {
        //Cycles pages upward
        pageCurrent++;
        pageCurrent = Mathf.Clamp(pageCurrent, 1, pageCount);
        ObjectsHide.SetActive(false);
        DisplayPageDetails();
        ObjectsHide.SetActive(true);
    }

    public void DecrementPage()
    {
        //Cycles pages downward
        pageCurrent--;
        pageCurrent = Mathf.Clamp(pageCurrent, 1, pageCount);
        ObjectsHide.SetActive(false);
        DisplayPageDetails();
        ObjectsHide.SetActive(true);
    }

    public void HideAll()
    {
        DetailPanel.SetActive(false);
        AssetRepository.SetActive(false);
    }
}
