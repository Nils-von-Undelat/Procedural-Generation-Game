using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField] LayerMask WhatIsRoom;

    RoomGenerator s_roomGenerator;

    bool canBeInstantiated = false;

    bool hasAlreadyInstantiatedRooM = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        s_roomGenerator = FindAnyObjectByType<RoomGenerator>();
    }

    private void Update()
    {
        if (s_roomGenerator == null) return;

        if (s_roomGenerator.amountOfRooms > 0)
        {
            Vector3 positionForRoom = PositionForNextRoom();

            if ((positionForRoom == Vector3.zero))
            {
                positionForRoom = PositionForNextRoom();
            }

            if (GOPosToInstantiateAt(positionForRoom) && positionForRoom != Vector3.zero && !hasAlreadyInstantiatedRooM)
            {
                hasAlreadyInstantiatedRooM = true;

                GameObject gOToInstantiate = s_roomGenerator.roomObject[Random.Range(0, s_roomGenerator.roomObject.Length)];

                Instantiate(gOToInstantiate, positionForRoom, Quaternion.identity);
                s_roomGenerator.amountOfRooms--;

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

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 10, WhatIsRoom);

        //if (gameObject == s_roomGenerator.startRoom || hit.collider == null || hit.collider.gameObject.GetComponent<Room>() == null)
        //{
        //    return direction;
        //}
        if (Physics2D.Raycast(transform.position, direction, 10, WhatIsRoom))
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
        if (!collision.IsTouchingLayers(WhatIsRoom))
        {
            canBeInstantiated = true;
        }
    }

    private bool GOPosToInstantiateAt(Vector3 positionToInstantiateAt)
    {
        GameObject testPositionGameObject = new GameObject();

        testPositionGameObject.tag = "TestRoom";

        testPositionGameObject.AddComponent<BoxCollider2D>().isTrigger = true;

        BoxCollider2D testBoxCollider = testPositionGameObject.GetComponent<BoxCollider2D>();

        Instantiate(testPositionGameObject, positionToInstantiateAt, Quaternion.identity);

        OnTriggerEnter2D(testBoxCollider);

        if (canBeInstantiated)
        {
            return true;
        }

        return false;
    }
}