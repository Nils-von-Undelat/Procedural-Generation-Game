using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField] LayerMask WhatIsRoom;

    RoomGenerator s_roomGenerator;

    bool canBeInstantiated = false;

    bool hasAlreadyInstantiatedRooM = false;

    [SerializeField] float lengthOfRayForChecking;

    [SerializeField] Vector2 colliderCheckSize;

    GameObject testGo;

    int roomsAbleToBeGenerated;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        s_roomGenerator = FindAnyObjectByType<RoomGenerator>();

        roomsAbleToBeGenerated = Random.Range(1, 4);
    }

    private void Update()
    {
        if (s_roomGenerator == null) return;

        if (s_roomGenerator.amountOfRoomsInTotal <= 0)
            return;

        Vector3 positionForRoom = PositionForNextRoom();

        if (positionForRoom == Vector3.zero) return;

        if (!canBeInstantiated) return;

        if (roomsAbleToBeGenerated != 1)
        {
            CreateTheActualRoom(positionForRoom);
        }

        else if (!hasAlreadyInstantiatedRooM)
        {
            hasAlreadyInstantiatedRooM = true;

            CreateTheActualRoom(positionForRoom);

            FindAndRemoveAllTestObjects();

            this.enabled = false;
        }

    }

    private void CreateTheActualRoom(Vector3 positionToInstantiateAt)
    {
        roomsAbleToBeGenerated--;

        GameObject gOToInstantiate = s_roomGenerator.roomObjects[Random.Range(0, s_roomGenerator.roomObjects.Length)];

        GameObject roomGO = Instantiate(gOToInstantiate, positionToInstantiateAt, Quaternion.identity);

        roomGO.transform.SetParent(s_roomGenerator.roomParentGO.transform, true);

        s_roomGenerator.amountOfRoomsInTotal--;

        s_roomGenerator.currentRooms.Add(gOToInstantiate);
    }

    private void FindAndRemoveAllTestObjects()
    {
        GameObject[] localGOArray = GameObject.FindGameObjectsWithTag("TestRoom");

        foreach (GameObject go in localGOArray)
        {
            if (go != testGo)
            {
                Destroy(go);
            }
        }
    }

    private Vector3 PositionForNextRoom()
    {
        // Defines a random direction for the next room 
        int wayToGo = Random.Range(0, 4);
        // 0 = up
        // 1 = down
        // 2 = left
        // 3 = right

        Vector3 direction = gameObject.transform.position;

        #region Determening the way for the object

        if (wayToGo == 0)
        {
            direction += transform.up + new Vector3(0, s_roomGenerator.distanceBetweenRooms, 0);
        }
        else if (wayToGo == 1)
        {
            direction += -transform.up + new Vector3(0, -s_roomGenerator.distanceBetweenRooms, 0);
        }
        else if (wayToGo == 2)
        {
            direction += -transform.right + new Vector3(-s_roomGenerator.distanceBetweenRooms, 0, 0);
        }
        else
        {
            direction += transform.right + new Vector3(s_roomGenerator.distanceBetweenRooms, 0, 0);
        }
        #endregion

        if (GOPosToInstantiateAt(direction))
        {
            return direction;
        }
        return Vector3.zero;
    }

    private bool GOPosToInstantiateAt(Vector3 positionToInstantiateAt)
    {
        GameObject testPositionGameObject = new()
        {
            tag = "TestRoom"
        };

        testPositionGameObject.transform.position = positionToInstantiateAt;

        testGo = testPositionGameObject;

        if (HitRoom(testPositionGameObject, positionToInstantiateAt))
            return false;

        canBeInstantiated = true;
        return true;
    }

    private bool HitRoom(GameObject testPosition, Vector3 direction)
    {
        RaycastHit2D hit = Physics2D.BoxCast(testPosition.transform.position, colliderCheckSize, 0, direction, s_roomGenerator.distanceBetweenRooms, WhatIsRoom);

        if (hit && hit.collider.gameObject.GetComponent<Room>() != null)
        {
            FindAndRemoveAllTestObjects();
            return true;
        }

        return false;
    }

    private void OnDrawGizmos()
    {
        if (testGo != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(testGo.transform.position, colliderCheckSize);
        }
    }
}
