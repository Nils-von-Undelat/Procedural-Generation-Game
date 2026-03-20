using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField] LayerMask WhatIsRoom;

    RoomGenerator s_roomGenerator;

    bool canBeInstantiated = false;

    bool hasAlreadyInstantiatedRooM = false;

    [SerializeField] float lengthOfRayForChecking;

    [SerializeField] Vector2 colliderCheckSize;

    bool functionablePlaceForRoomFound = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        s_roomGenerator = FindAnyObjectByType<RoomGenerator>();
    }

    private void Update()
    {
        if (s_roomGenerator == null) return;

        if (s_roomGenerator.amountOfRoomsInTotal > 0)
        {
            Vector3 positionForRoom = PositionForNextRoom();

            if ((positionForRoom == Vector3.zero))
            {
                positionForRoom = PositionForNextRoom();
            }

            if (positionForRoom != Vector3.zero && !hasAlreadyInstantiatedRooM && canBeInstantiated)
            {
                hasAlreadyInstantiatedRooM = true;

                GameObject gOToInstantiate = s_roomGenerator.roomObjects[Random.Range(0, s_roomGenerator.roomObjects.Length)];

                Instantiate(gOToInstantiate, positionForRoom, Quaternion.identity);
                s_roomGenerator.amountOfRoomsInTotal--;

                s_roomGenerator.currentRooms.Add(gOToInstantiate);

                GameObject[] localGOArray = GameObject.FindGameObjectsWithTag("TestRoom");

                foreach (GameObject go in localGOArray)
                {
                    Destroy(go);
                }

                this.enabled = false;
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

        Vector3 directionToCheckForColliders = new Vector3();

        if (wayToGo == 0)
        {
            direction += transform.up + new Vector3(0, s_roomGenerator.distanceBetweenRooms, 0);
            directionToCheckForColliders = transform.up + new Vector3(0, s_roomGenerator.distanceBetweenRooms, 0);
            s_roomGenerator.previousDirection = 0;
        }
        else if (wayToGo == 1)
        {
            direction += -transform.up + new Vector3(0, -s_roomGenerator.distanceBetweenRooms, 0);
            directionToCheckForColliders = -transform.up + new Vector3(0, -s_roomGenerator.distanceBetweenRooms, 0);
            s_roomGenerator.previousDirection = 1;
        }
        else if (wayToGo == 2)
        {
            direction += -transform.right + new Vector3(-s_roomGenerator.distanceBetweenRooms, 0, 0);
            directionToCheckForColliders = -transform.right + new Vector3(-s_roomGenerator.distanceBetweenRooms, 0, 0);
            s_roomGenerator.previousDirection = 2;
        }
        else
        {
            direction += transform.right + new Vector3(s_roomGenerator.distanceBetweenRooms, 0, 0);
            directionToCheckForColliders = transform.right + new Vector3(s_roomGenerator.distanceBetweenRooms, 0, 0);
            s_roomGenerator.previousDirection = 3;
        }

        if (GOPosToInstantiateAt(direction, wayToGo, directionToCheckForColliders))
        {
            return direction;
        }
        else
        {
            return Vector3.zero;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.IsTouchingLayers(WhatIsRoom) || /*!collision.CompareTag("Room") */ !collision.gameObject)
        {
            functionablePlaceForRoomFound = true;
        }
        else
        {
            functionablePlaceForRoomFound = false;
        }
    }


    private bool GOPosToInstantiateAt(Vector3 positionToInstantiateAt, int direction, Vector3 directionForOverLap)
    {
        GameObject testPositionGameObject = new GameObject();

        testPositionGameObject.tag = "TestRoom";

        testPositionGameObject.AddComponent<BoxCollider2D>().isTrigger = false;

        BoxCollider2D testBoxCollider = testPositionGameObject.GetComponent<BoxCollider2D>();

        Instantiate(testPositionGameObject, positionToInstantiateAt, Quaternion.identity);


        float angle;

        if (direction == 0)
        {// up
            angle = Vector3.Angle(transform.up, directionForOverLap);
        }
        else if (direction == 1)
        { // down
            angle = Vector3.Angle(transform.up, directionForOverLap);
        }
        else if (direction == 2)
        { // left
            angle = Vector3.Angle(transform.up, directionForOverLap);
        }
        else
        { // right
            angle = Vector3.Angle(transform.up, directionForOverLap);
        }

        Debug.Log(angle);

        Collider2D[] localCollider = Physics2D.OverlapBoxAll(testPositionGameObject.transform.position, colliderCheckSize, angle);


        //foreach (Collider2D coll in localCollider)
        //{
        //    OnTriggerEnter2D(coll);
        //}


        OnTriggerEnter2D(testBoxCollider);

        // Debug.Log(localCollider.Length);

        if (gameObject == s_roomGenerator.startRoom || localCollider.Length == 1 && functionablePlaceForRoomFound)
        {
            canBeInstantiated = true;
            foreach (Collider2D coll in localCollider)
            {
                Destroy(coll);
            }
            return true;
        }
        else
        {
            return false;
        }
    }


    //private Collider2D[] CollidersInTheWay(GameObject go, Vector3 direction)
    //{



    //}
}
