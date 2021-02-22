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
    private TextMeshProUGUI myText;
    private TextMeshProUGUI detailText;
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
        myText = MyTextField.GetComponent<TextMeshProUGUI>();
        detailText = DetailTextField.GetComponent<TextMeshProUGUI>();
        detailText.text = myText.text;
        DetailPanel.SetActive(true);
    }
}
