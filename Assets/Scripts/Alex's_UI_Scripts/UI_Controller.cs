using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Controller : MonoBehaviour
{
    [SerializeField] private GameObject AssetRepository;
    [SerializeField] private GameObject ObjectsHide;
    [SerializeField] private GameObject PageCounter;
    private List<Text> objectDisplay = new List<Text>();
    [SerializeField] private GameObject Object1;
    [SerializeField] private GameObject Object2;
    [SerializeField] private GameObject Object3;
    [SerializeField] private GameObject Object4;
    [SerializeField] private GameObject Object5;
    [SerializeField] private GameObject Object6;
    private TempListScript Resources;
    Text countText;
    int listLength;
    int pageCount;
    int pageCurrent = 1;

    public bool increment = false;
    public bool decrement = false;

    // Start is called before the first frame update
    void Start()
    {
        #region set variables
        objectDisplay.Add(Object1.GetComponent<Text>());
        objectDisplay.Add(Object2.GetComponent<Text>());
        objectDisplay.Add(Object3.GetComponent<Text>());
        objectDisplay.Add(Object4.GetComponent<Text>());
        objectDisplay.Add(Object5.GetComponent<Text>());
        objectDisplay.Add(Object6.GetComponent<Text>());
        countText = PageCounter.GetComponent<Text>();
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

    }

    private void MenuSetup()
    {
        //Creates a total page count based on number of objects in Resources.readFrom
        //Includes all full pages, plus a page for the remainder
        listLength = Resources.readFrom.Count;
        Debug.Log(listLength);
        pageCount = listLength / 6;
        if(listLength % 6 > 0)
        {
            pageCount++;
        }

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
        pageCurrent++;
        pageCurrent = Mathf.Clamp(pageCurrent, 1, pageCount);
        ObjectsHide.SetActive(false);
        DisplayPageDetails();
        ObjectsHide.SetActive(true);
    }

    public void DecrementPage()
    {
        pageCurrent--;
        pageCurrent = Mathf.Clamp(pageCurrent, 1, pageCount);
        ObjectsHide.SetActive(false);
        DisplayPageDetails();
        ObjectsHide.SetActive(true);
    }
}
