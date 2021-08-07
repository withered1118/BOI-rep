using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : MonoBehaviour
{

    private Animator am;
    // Start is called before the first frame update
    void Start()
    {
        am = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.DownArrow)){
            Debug.Log("1");
            am.SetBool("isShooting", true);
        }
        if(Input.GetKeyUp(KeyCode.DownArrow))
            am.SetBool("isShooting", false);
    }
}
