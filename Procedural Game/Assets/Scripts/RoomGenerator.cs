using System.Collections.Generic;
using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
    public GameObject startRoom;

    public GameObject roomParentGO;

    public GameObject[] roomObjects;

    public GameObject[] treasureRoomObjects;

    public GameObject bossRoom;

    public int amountOfRoomsToBeGenerated;

    public int amountOfRoomsInTotal;

    public int distanceBetweenRooms;

    public Sprite spriteForTesy;

    public GameObject bridgeParent;

    public int totalTreasureRoomsAllowed;

    public int treasureRoomsInScene;

}