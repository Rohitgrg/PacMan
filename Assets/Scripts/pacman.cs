using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pacman : MonoBehaviour
{
    private Node currentNode;
    //The movement speed of Pacman
    public float speed = 4.0f;

    //store the direction Pacman wants to go
    private Vector2 moveDirection = Vector2.zero;
    // Start is called before the first frame update
    void Start()
    {
        Node node = GetNodeAtPostion(transform.position);

        if (node != null)
        {
            currentNode = node;
            Debug.Log(transform.localPosition);
            Debug.Log(currentNode);
        }
        else
        {
            Debug.Log(transform.localPosition);
            Debug.Log("Null");
        }
    }

    // Update is called once per frame
    void Update()
    {
        //checking for the pressed key value
        checkForInput();

        //change the position according to the pressed key
        // transform.localPosition += (Vector3)(moveDirection * speed) * 2 * Time.deltaTime;

        //Change the direction that Pacman is facing
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
            movePacman(moveDirection);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            moveDirection = Vector2.down;
            movePacman(moveDirection);

        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            moveDirection = Vector2.right;
            movePacman(moveDirection);

        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            moveDirection = Vector2.left;
            movePacman(moveDirection);

        }
    }

    //returns Node at a specific position
    Node GetNodeAtPostion(Vector2 position)
    {
        GameObject tile = GameObject.Find("Game").GetComponent<LevelOneBoard>().board[(int)position.x, (int)position.y];
        if (tile != null) return tile.GetComponent<Node>();
        return null;
    }

    Node validMove (Vector2 v){
        Node nextNode = null;
        foreach (Node item in currentNode.neighbours)
        {
            if (currentNode.availableDirections[System.Array.IndexOf(currentNode.neighbours, item)] == v){
                nextNode = item;
                break;
            }
        }
        return nextNode;
    }

    void movePacman (Vector2 v){
        Node nextNode = validMove(v);
        if (nextNode != null ){
            transform.localPosition = nextNode.transform.position;
            currentNode = nextNode;
        }
    }

}
