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

        GameObject gOToInstantiate = GOToInstantiate();

        GameObject bridgeGoal = Instantiate(gOToInstantiate, positionToInstantiateAt, Quaternion.identity, s_roomGenerator.roomParentGO.transform);

        BridgeBetweenRooms(positionToInstantiateAt, bridgeGoal);

        s_roomGenerator.amountOfRoomsInTotal--;

    }

    private GameObject GOToInstantiate()
    {
        GameObject gObject;


        if (!s_roomGenerator.bossRoomHasBeenInstantiated)
        {
            if (Random.Range(0, s_roomGenerator.amountOfRoomsToBeGenerated) == 0)
            {
                s_roomGenerator.bossRoomHasBeenInstantiated = true;
                gObject = s_roomGenerator.bossRoom;
            }
            else if (s_roomGenerator.totalTreasureRoomsAllowed >= s_roomGenerator.treasureRoomsInScene)
            {

                if (Random.Range(0, 4) != 0)
                {
                    gObject = s_roomGenerator.roomObjects[Random.Range(0, s_roomGenerator.roomObjects.Length)];
                }
                else
                {
                    s_roomGenerator.treasureRoomsInScene++;
                    gObject = s_roomGenerator.treasureRoomObjects[Random.Range(0, s_roomGenerator.treasureRoomObjects.Length)];
                }
            }
            else
                gObject = s_roomGenerator.roomObjects[Random.Range(0, s_roomGenerator.roomObjects.Length)];
        }
        else if (s_roomGenerator.totalTreasureRoomsAllowed >= s_roomGenerator.treasureRoomsInScene)
        {

            if (Random.Range(0, 4) != 0)
            {
                gObject = s_roomGenerator.roomObjects[Random.Range(0, s_roomGenerator.roomObjects.Length)];
            }
            else
            {
                s_roomGenerator.treasureRoomsInScene++;
                gObject = s_roomGenerator.treasureRoomObjects[Random.Range(0, s_roomGenerator.treasureRoomObjects.Length)];
            }
        }
        else

            gObject = s_roomGenerator.roomObjects[Random.Range(0, s_roomGenerator.roomObjects.Length)];

        return gObject;
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

        Vector3 directionForRacyast;

        if (DirectionAndSizeOfBridge(positionForNextRoom) == transform.up)
            directionForRacyast = transform.up;

        else if (DirectionAndSizeOfBridge(positionForNextRoom) == -transform.up)
            directionForRacyast = -transform.up;

        else if (DirectionAndSizeOfBridge(positionForNextRoom) == transform.right)
            directionForRacyast = transform.right;

        else
            directionForRacyast = -transform.right;



        Vector3 midPos = (transform.position + nextRoom.transform.position) / 2;

        RaycastForTheBridgePosition(out RaycastHit2D hitForNextRoom, out RaycastHit2D hitForThisGO, midPos, directionForRacyast);

        midPos = (hitForThisGO.point + hitForNextRoom.point) / 2;

        RaycastForTheBridgePosition(out hitForNextRoom, out hitForThisGO, midPos, directionForRacyast);

        Vector2 referenceDifference = hitForThisGO.point - hitForNextRoom.point;
        Vector2 scale = new(referenceDifference.x, referenceDifference.y);

        if (scale.x == 0)
            scale.x = 1;

        if (scale.y == 0)
            scale.y = 1;

        bridgeGO.transform.position = midPos;

        bridgeGO.transform.localScale = scale;

        bridgeGO.transform.SetParent(s_roomGenerator.bridgeParent.transform, true);

        return bridgeGO;
    }

    private void RaycastForTheBridgePosition(out RaycastHit2D hitForGoalObject, out RaycastHit2D hitForcurrentObject, Vector3 originPos, Vector3 directionForRaycast)
    {
        hitForGoalObject = Physics2D.Raycast(originPos, directionForRaycast, Halving(s_roomGenerator.distanceBetweenRooms), WhatIsRoom);

        hitForcurrentObject = Physics2D.Raycast(originPos, -directionForRaycast, Halving(s_roomGenerator.distanceBetweenRooms), WhatIsRoom);

    }

    private Vector3 DirectionAndSizeOfBridge(Vector3 direction)
    {
        Vector3 bridgeDirection;

        if (direction.x > gameObject.transform.position.x)
        {
            bridgeDirection = transform.right;

        }
        else if (direction.x < gameObject.transform.position.x)
        {
            bridgeDirection = -transform.right;

        }
        else if (direction.y > gameObject.transform.position.y)
        {
            bridgeDirection = transform.up;
        }
        else
        {
            bridgeDirection = -transform.up;
        }
        return bridgeDirection;
    }

    private int Halving(int numberToBeHalved)
    {
        return numberToBeHalved / 2;
    }
}
