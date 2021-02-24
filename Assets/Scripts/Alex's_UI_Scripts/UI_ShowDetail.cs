using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_ShowDetail : MonoBehaviour
{
    //Gets its relevant text field, the detail panel, and the text field within
    [SerializeField] private GameObject MyTextField;
    [SerializeField] private GameObject DetailPanel;
    [SerializeField] private GameObject DetailTextField;
    [SerializeField] private GameObject UIControllerHost;
    private TextMeshProUGUI myText;
    private TextMeshProUGUI detailText;
    private UI_Controller UI_Controller;
    // Start is called before the first frame update
    void Start()
    {
        UI_Controller = UIControllerHost.GetComponent<UI_Controller>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPress()
    {
        DetailPanel.SetActive(false);
        myText = MyTextField.GetComponent<TextMeshProUGUI>();
        detailText = DetailTextField.GetComponent<TextMeshProUGUI>();
        detailText.text = myText.text;
        DetailPanel.SetActive(true);
        UI_Controller.windowCurrent = UI_Controller.windowFinder.Detail;
        UI_Controller.detailCurrent = UI_Controller.detailFinder.Null;
    }
}
