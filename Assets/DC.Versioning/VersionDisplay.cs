using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VersionDisplay : MonoBehaviour {

    public string _Suffix = "";

    [SerializeField] Text VersionText;
    [SerializeField] TextMeshProUGUI VersionTextTMP;

    void Start ()
    {
        if(VersionText)
            VersionText.text = "Version: " + Application.version + " " + _Suffix;
        else if (VersionTextTMP)
            VersionTextTMP.text = "Version: " + Application.version + " " + _Suffix;
    }

}
