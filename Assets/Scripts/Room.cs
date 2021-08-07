using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public GameObject doorLeft, doorRight, doorUp, doorDown;

    public bool roomLeft, roomRight, roomUp, roomDown;
    public int roomNum;

    // Start is called before the first frame update
    void Start()
    {
        doorLeft.SetActive(roomLeft);
        doorRight.SetActive(roomRight);
        doorUp.SetActive(roomUp);
        doorDown.SetActive(roomDown);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RoomUpdate(){
        if(roomLeft)
            roomNum++;
        if(roomRight)
            roomNum++;
        if(roomUp)
            roomNum++;
        if(roomDown)
            roomNum++;
    }
}
