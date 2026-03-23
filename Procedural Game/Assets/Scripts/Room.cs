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

        GameObject bridgeGoal = Instantiate(gOToInstantiate, positionToInstantiateAt, Quaternion.identity, s_roomGenerator.roomParentGO.transform);

        BridgeBetweenRooms(positionToInstantiateAt, bridgeGoal);

        s_roomGenerator.amountOfRoomsInTotal--;

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

    private GameObject BridgeBetweenRooms(Vector3 positionForNextRoom, GameObject nextRoom)
    {
        GameObject bridgeGO = new()
        {
            tag = "Bridge",
            name = "BridgeBetweenRoom"
        };

        bridgeGO.AddComponent<SpriteRenderer>().color = Color.green;

        bridgeGO.GetComponent<SpriteRenderer>().sprite = s_roomGenerator.spriteForTesy;


        BoxCollider2D box = gameObject.GetComponent<BoxCollider2D>();
        BoxCollider2D box2 = nextRoom.GetComponent<BoxCollider2D>();


        Vector3 middleBetweenRooms;

        Vector3 rotationForBridge;

        if (DirectionAndSizeOfBridge(positionForNextRoom, out rotationForBridge) == transform.up)
        {
            middleBetweenRooms = DirectionAndSizeOfBridge(positionForNextRoom, out rotationForBridge) + new Vector3(0, Halving(s_roomGenerator.distanceBetweenRooms), 0);
        }
        else if (DirectionAndSizeOfBridge(positionForNextRoom, out rotationForBridge) == -transform.up)
        {
            middleBetweenRooms = DirectionAndSizeOfBridge(positionForNextRoom, out rotationForBridge) + new Vector3(0, -Halving(s_roomGenerator.distanceBetweenRooms), 0);
        }
        else if (DirectionAndSizeOfBridge(positionForNextRoom, out rotationForBridge) == transform.right)
        {
            middleBetweenRooms = DirectionAndSizeOfBridge(positionForNextRoom, out rotationForBridge) + new Vector3(Halving(s_roomGenerator.distanceBetweenRooms), 0, 0);
        }
        else
        {
            middleBetweenRooms = DirectionAndSizeOfBridge(positionForNextRoom, out rotationForBridge) + new Vector3(-Halving(s_roomGenerator.distanceBetweenRooms), 0, 0);
        }

        middleBetweenRooms += gameObject.transform.position;

        bridgeGO.transform.position = middleBetweenRooms;

        bridgeGO.transform.rotation = Quaternion.Euler(rotationForBridge * 90);

        bridgeGO.transform.localScale = new Vector3(1, 2, 1);

        bridgeGO.transform.SetParent(s_roomGenerator.bridgeParent.transform, true);


        return bridgeGO;
    }

    private Vector3 DirectionAndSizeOfBridge(Vector3 direction, out Vector3 rotationForBridge)
    {
        Vector3 bridgeDirection;

        if (direction.x > gameObject.transform.position.x)
        {
            bridgeDirection = transform.right;
            rotationForBridge = new Vector3(0, 0, transform.right.magnitude);

        }
        else if (direction.x < gameObject.transform.position.x)
        {
            bridgeDirection = -transform.right;
            rotationForBridge = new Vector3(0, 0, -transform.right.magnitude);

        }
        else if (direction.y > gameObject.transform.position.y)
        {
            bridgeDirection = transform.up;
            rotationForBridge = new Vector3(0, 0, 2);
        }
        else
        {
            bridgeDirection = -transform.up;
            rotationForBridge = new Vector3(0, 0, -2);
        }
        return bridgeDirection;
    }

    private int Halving(int numberToBeHalved)
    {
        return numberToBeHalved / 2;
    }
}
