using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingTile : MonoBehaviour
{
    public float distance;
    public float turningPoint;
    public float moveSpeed;

    float initPosX;
    bool turnSwitch;
    bool onBoard;


    // Start is called before the first frame update
    void Start()
    {
        initPosX = transform.position.x;
        turningPoint = initPosX - distance;

    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    void Move()
    {
        float currentPosX = transform.position.x;

        if (currentPosX >= initPosX + distance)
            turnSwitch = false;
        else if (currentPosX <= turningPoint)
            turnSwitch = true;

        if (turnSwitch)
            transform.position += new Vector3(1, 0, 0) * moveSpeed * Time.deltaTime;
        else
            transform.position += new Vector3(-1,0,0) * moveSpeed * Time.deltaTime;
    }


    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            onBoard = true;

            Player player = other.gameObject.GetComponent<Player>();

            if (onBoard)
            {
                if (turnSwitch)
                    player.transform.position += new Vector3(1, 0, 0) * moveSpeed * Time.deltaTime;
                else
                    player.transform.position += new Vector3(-1, 0, 0) * moveSpeed * Time.deltaTime;
            }   
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
            onBoard = false;
    }

}
