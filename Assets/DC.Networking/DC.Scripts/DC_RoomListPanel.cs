using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DC_RoomListPanel : MonoBehaviour
{
    private void OnEnable()
    {
        DC_NetworkManager manager = FindObjectOfType<DC_NetworkManager>();

        if (manager)
            manager.UpdateRoomLists();
    }
}
