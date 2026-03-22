using System.Collections.Generic;
using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
    public GameObject startRoom;

    public GameObject[] roomObjects;

    public List<GameObject> currentRooms;

    public int amountOfRoomsToBeGenerated;

    public int amountOfRoomsInTotal;

    public int distanceBetweenRooms;

    public Sprite spriteForTesy;

    public void ResetTheCurrentRooms()
    {
        foreach (GameObject go in currentRooms)
        {
            if (go != startRoom)
            {
                currentRooms.Remove(go);
            }

        }





    }
}