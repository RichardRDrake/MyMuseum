using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowDetail : MonoBehaviour
{
    //Gets its relevant text field, the detail panel, and the text field within
    [SerializeField] private GameObject MyTextField;
    [SerializeField] private GameObject DetailPanel;
    [SerializeField] private GameObject DetailTextField;
    private Text myText;
    private Text detailText;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPress()
    {
        DetailPanel.SetActive(false);
        myText = MyTextField.GetComponent<Text>();
        detailText = DetailTextField.GetComponent<Text>();
        detailText.text = myText.text;
        DetailPanel.SetActive(true);
    }
}
