using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Scrollbar))]
public class DC_ScrollbarHelper : MonoBehaviour
{
    private Scrollbar m_Scrollbar;

    private void Awake()
    {
        m_Scrollbar = GetComponent<Scrollbar>();
    }

    public void MoveUp(float amount)
    {
        if(m_Scrollbar.value <= 1.0f - amount)
            m_Scrollbar.value += amount;
    }

    public void MoveDown(float amount)
    {
        if(m_Scrollbar.value >= amount)
        m_Scrollbar.value -= amount;
    }
}
