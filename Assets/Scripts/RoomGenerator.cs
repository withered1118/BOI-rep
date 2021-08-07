using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomGenerator : MonoBehaviour
{
    public enum Direction {up, down, left, right};
    public Direction direction;

    [Header("Room Info")]
    public GameObject roomPrefab;
    public GameObject treasureRoomPrefab;
    public GameObject bossRoomPrefab;
    public GameObject shopPrefab;
    private int roomNumber;
    private GameObject endRoom;
    private GameObject bossRoom;
    private GameObject treasureRoom;
    private GameObject shop;

    [Header("Location Control")]
    public Transform generatePoint;
    public float xOffest;
    public float yOffest;
    public LayerMask roomLayer;
    private int[] coordinates = {-1, 0, 1, 0, -1};
    public List<Room> roomList = new List<Room>();
    private List<Vector3> bossCoordList = new List<Vector3>();
    private List<Vector3> treasureCoordList = new List<Vector3>();
    public WallType wallType;


    // Start is called before the first frame update
    void Start()
    {
        roomNumber = Random.Range(6, 11);
        for(int i = 0; i < roomNumber; i++){
            //change position of point
            roomList.Add(Instantiate(roomPrefab, generatePoint.position, Quaternion.identity).GetComponent<Room>());

            changePointPosition();
        }

        SetUpBossRoom();
        SetUpTreasureRoom();
        SetUpShop();

        roomList.Add(bossRoom.GetComponent<Room>());
        roomList.Add(treasureRoom.GetComponent<Room>());
        roomList.Add(shop.GetComponent<Room>());

        foreach(var room in roomList){
            SetupRoom(room, room.transform.position + new Vector3(0.5f,  0.5f, 0));
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.anyKeyDown){
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public void changePointPosition(){
        do{
            direction  = (Direction)Random.Range(0, 4);

            switch(direction){
                case Direction.up:
                    generatePoint.position += new Vector3(0, yOffest, 0);
                    break;
                case Direction.down:
                    generatePoint.position += new Vector3(0, -yOffest, 0);
                    break;
                case Direction.left:
                    generatePoint.position += new Vector3(-xOffest, 0, 0);
                    break;
                case Direction.right:
                    generatePoint.position += new Vector3(xOffest, 0, 0);
                    break;
            }
        }while(Physics2D.OverlapCircle(generatePoint.position, 0.2f, roomLayer));
    }

    public void SetupRoom(Room newRoom, Vector3 roomPosition){
        newRoom.roomUp = Physics2D.OverlapCircle(roomPosition + new Vector3(0, yOffest, 0), 0.2f, roomLayer);
        newRoom.roomDown = Physics2D.OverlapCircle(roomPosition + new Vector3(0, -yOffest, 0), 0.2f, roomLayer);
        newRoom.roomLeft = Physics2D.OverlapCircle(roomPosition + new Vector3(-xOffest, 0, 0), 0.2f, roomLayer);
        newRoom.roomRight = Physics2D.OverlapCircle(roomPosition + new Vector3(xOffest, 0, 0), 0.2f, roomLayer);

        newRoom.RoomUpdate();

        switch(newRoom.roomNum){
            case 1:
                if(newRoom.roomUp)
                    Instantiate(wallType.singleUp, roomPosition, Quaternion.identity);
                if(newRoom.roomDown)
                    Instantiate(wallType.singleBottom, roomPosition, Quaternion.identity);
                if(newRoom.roomLeft)
                    Instantiate(wallType.singleLeft, roomPosition, Quaternion.identity);
                if(newRoom.roomRight)
                    Instantiate(wallType.singleRight, roomPosition, Quaternion.identity);
                break;
            case 2:
                if(newRoom.roomLeft && newRoom.roomUp)
                    Instantiate(wallType.doubleLU, roomPosition, Quaternion.identity);
                if(newRoom.roomLeft && newRoom.roomRight)
                    Instantiate(wallType.doubleLR, roomPosition, Quaternion.identity);
                if(newRoom.roomLeft && newRoom.roomDown)
                    Instantiate(wallType.doubleLB, roomPosition, Quaternion.identity);
                if(newRoom.roomUp && newRoom.roomRight)
                    Instantiate(wallType.doubleUR, roomPosition, Quaternion.identity);
                if(newRoom.roomUp && newRoom.roomDown)
                    Instantiate(wallType.doubleUB, roomPosition, Quaternion.identity);
                if(newRoom.roomRight && newRoom.roomDown)
                    Instantiate(wallType.doubleRB, roomPosition, Quaternion.identity);
                break;
            case 3:
                if(newRoom.roomLeft && newRoom.roomUp && newRoom.roomRight)
                    Instantiate(wallType.tripleLUR, roomPosition, Quaternion.identity);
                if(newRoom.roomLeft && newRoom.roomUp && newRoom.roomDown)
                    Instantiate(wallType.tripleLUB, roomPosition, Quaternion.identity);
                if(newRoom.roomLeft && newRoom.roomRight && newRoom.roomDown)
                    Instantiate(wallType.tripleLRB, roomPosition, Quaternion.identity);
                if(newRoom.roomUp && newRoom.roomRight && newRoom.roomDown)
                    Instantiate(wallType.tripleURB, roomPosition, Quaternion.identity);
                break;
            case 4:
                Instantiate(wallType.foorDoors, roomPosition, Quaternion.identity);
                break;
        }
    }

    public int AdjacentDetector(Vector3 roomPos){
        int count = 0;
        for(int i = 0; i < 4; i++){
            if(Physics2D.OverlapCircle(roomPos + new Vector3(coordinates[i] * xOffest, coordinates[i + 1] * yOffest, 0), 0.2f, roomLayer))
                count++;
        }
        return count;
    }

    public void SetUpBossRoom(){
        endRoom = roomList[0].gameObject;
        
        foreach(var room in roomList){
            if(room.transform.position.sqrMagnitude > endRoom.transform.position.sqrMagnitude){
                endRoom = room.gameObject;
            }
        }

        for(int i = 0; i < 4; i++){
            if(Physics2D.OverlapCircle(endRoom.transform.position + new Vector3(coordinates[i] * xOffest, coordinates[i + 1] * yOffest, 0), 0.2f, roomLayer)) continue; //if curRoom is adjacent to another room on one direction, continue
            else{
                if(AdjacentDetector(endRoom.transform.position + new Vector3(coordinates[i] * xOffest, coordinates[i + 1] * yOffest, 0)) == 1){ //if the adjacent pos is only adjacent to one room(end room), the pos is avail to be used as bossRoom
                    bossCoordList.Add(endRoom.transform.position + new Vector3(coordinates[i] * xOffest, coordinates[i + 1] * yOffest, 0)); //add the coordinates
                }
            }
        }

        bossRoom = Instantiate(roomPrefab, bossCoordList[Random.Range(0, bossCoordList.Count)], Quaternion.identity);
        bossRoom.gameObject.tag = "bossRoom";
        bossCoordList.Clear();
    }

    public void SetUpTreasureRoom(){
        foreach(var room in roomList){
            for(int i = 0; i < 4; i++){
                if(Physics2D.OverlapCircle(room.transform.position + new Vector3(coordinates[i] * xOffest, coordinates[i + 1] * yOffest, 0), 0.2f, roomLayer)) continue; //if curRoom is adjacent to another room on one direction, continue
                if(AdjacentDetector(room.transform.position + new Vector3(coordinates[i] * xOffest, coordinates[i + 1] * yOffest, 0)) == 1){ //if the adjacent pos is only adjacent to one room(end room), the pos is avail to be used as bossRoom
                    float manDisX = Mathf.Abs(room.transform.position.x - bossRoom.transform.position.x);
                    float manDisY = Mathf.Abs(room.transform.position.y - bossRoom.transform.position.y);
                    if(manDisX > xOffest || manDisY > yOffest){
                        treasureCoordList.Add(room.transform.position + new Vector3(coordinates[i] * xOffest, coordinates[i + 1] * yOffest, 0));
                    }
                }
            }
        }
        treasureRoom = Instantiate(treasureRoomPrefab, treasureCoordList[Random.Range(0, treasureCoordList.Count)], Quaternion.identity);
        treasureRoom.gameObject.tag = "treasureRoom";
        treasureCoordList.Clear();
    }

    public void SetUpShop(){
        foreach(var room in roomList){
            for(int i = 0; i < 4; i++){
                if(Physics2D.OverlapCircle(room.transform.position + new Vector3(coordinates[i] * xOffest, coordinates[i + 1] * yOffest, 0), 0.2f, roomLayer)) continue; //if curRoom is adjacent to another room on one direction, continue
                if(AdjacentDetector(room.transform.position + new Vector3(coordinates[i] * xOffest, coordinates[i + 1] * yOffest, 0)) == 1){ //if the adjacent pos is only adjacent to one room(end room), the pos is avail to be used as bossRoom
                    float manDisX = Mathf.Abs(room.transform.position.x - bossRoom.transform.position.x);
                    float manDisY = Mathf.Abs(room.transform.position.y - bossRoom.transform.position.y);
                    if(manDisX > xOffest || manDisY > yOffest){
                        treasureCoordList.Add(room.transform.position + new Vector3(coordinates[i] * xOffest, coordinates[i + 1] * yOffest, 0));
                    }
                }
            }
        }
        shop = Instantiate(roomPrefab, treasureCoordList[Random.Range(0, treasureCoordList.Count)], Quaternion.identity);
        shop.gameObject.tag = "shop";
        treasureCoordList.Clear();
    }
}

[System.Serializable]
public class WallType{
    public GameObject singleLeft, singleRight, singleUp, singleBottom,
                      doubleLU, doubleLR, doubleLB, doubleUR, doubleUB, doubleRB,
                      tripleLUR, tripleLUB, tripleURB, tripleLRB,
                      foorDoors;
}
