using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pacman : MonoBehaviour
{
    //The movement speed of Pacman
    public float speed = 4.0f;

    //store the direction pacman wants to go
    private Vector2 moveDirection = Vector2.zero;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //checking for the pressed key value
        checkForInput();

        //change the position according to the pressed key
        transform.localPosition += (Vector3)(moveDirection * speed) * 2 * Time.deltaTime;

        //Change the direction that pacman is facing
        if (moveDirection == Vector2.up)
        {
            transform.localScale = new Vector3(1, 1, 1);
            transform.localRotation = Quaternion.Euler(0, 0, 90);
        }
        else if (moveDirection == Vector2.down)
        {
            transform.localScale = new Vector3(1, 1, 1);
            transform.localRotation = Quaternion.Euler(0, 0, 270);
        }
        else if (moveDirection == Vector2.left)
        {
            transform.localRotation = Quaternion.Euler(0, 0, 0);

            transform.localScale = new Vector3(-1, 1, 1);
        }

        else if (moveDirection == Vector2.right)
        {
            transform.localRotation = Quaternion.Euler(0, 0, 0);

            transform.localScale = new Vector3(1, 1, 1);

        }

    }

    void checkForInput()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            moveDirection = Vector2.up;
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            moveDirection = Vector2.down;
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            moveDirection = Vector2.right;

        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            moveDirection = Vector2.left;
        }
    }
}
