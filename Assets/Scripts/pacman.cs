using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pacman : MonoBehaviour
{
    private Node oldNode, currentNode, desiredNode;
    //The movement speed of Pacman
    public float speed = 4.0f;

    public Sprite idle;

    //store the direction Pacman wants to go
    private Vector2 moveDirection = Vector2.zero;
    private Vector2 desiredDirection;
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

        changePosition(moveDirection);
    }

    // Update is called once per frame
    void Update()
    {
        //checking for the pressed key value
        checkForInput();

        //change the position according to the pressed key
        if (desiredNode != currentNode && desiredNode != null)
        {

            if (possibleTarget())
            {

                currentNode = desiredNode;

                transform.localPosition = currentNode.transform.position;

                Node nextNode = validMove(desiredDirection);

                if (nextNode != null) moveDirection = desiredDirection;
                else nextNode = validMove(desiredDirection);

                if (nextNode != null)
                {
                    desiredNode = nextNode;
                    oldNode = currentNode;
                    currentNode = null;
                }
                else
                {
                    moveDirection = Vector2.zero;
                }

            }
            else
            {
                transform.localPosition += (Vector3)(moveDirection * speed) * 2 * Time.deltaTime;
            }
        }

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

        changeAnimationState();

    }

    void changeAnimationState(){
        if (moveDirection == Vector2.zero){
            GetComponent<Animator> ().enabled = false;
            GetComponent<SpriteRenderer>().sprite = idle;
        } else GetComponent<Animator> ().enabled = true;
    }

    void checkForInput()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            // moveDirection = Vector2.up;
            // movePacman(moveDirection);
            changePosition(Vector2.up);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            // moveDirection = Vector2.down;
            // movePacman(moveDirection);
            changePosition(Vector2.down);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            // moveDirection = Vector2.right;
            // movePacman(moveDirection);
            changePosition(Vector2.right);

        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            // moveDirection = Vector2.left;
            // movePacman(moveDirection);
            changePosition(Vector2.left);

        }
    }

    //returns Node at a specific position
    Node GetNodeAtPostion(Vector2 position)
    {
        GameObject tile = GameObject.Find("Game").GetComponent<LevelOneBoard>().board[(int)position.x, (int)position.y];
        if (tile != null) return tile.GetComponent<Node>();
        return null;
    }

    Node validMove(Vector2 v)
    {
        Node nextNode = null;
        foreach (Node item in currentNode.neighbours)
        {
            if (currentNode.availableDirections[System.Array.IndexOf(currentNode.neighbours, item)] == v)
            {
                nextNode = item;
                break;
            }
        }
        return nextNode;
    }

    void movePacman(Vector2 v)
    {
        Node nextNode = validMove(v);
        if (nextNode != null)
        {
            transform.localPosition = nextNode.transform.position;
            currentNode = nextNode;
        }
    }

    void changePosition(Vector2 d)
    {
        if (d != moveDirection)
        {
            desiredDirection = d;
        }

        if (currentNode != null)
        {
            Node nextNode = validMove(d);

            if (nextNode != null)
            {
                moveDirection = d;
                desiredNode = nextNode;
                oldNode = currentNode;
                currentNode = null;
            }
        }
    }

    float spaceBetweenNodes(Vector2 desiredDirection)
    {
        Vector2 v = desiredDirection - (Vector2)oldNode.transform.position;
        return v.sqrMagnitude;
    }

    bool possibleTarget()
    {
        float nodeToTarget = spaceBetweenNodes(desiredNode.transform.position);

        float nodeToCurrent = spaceBetweenNodes(transform.localPosition);

        return nodeToCurrent > nodeToTarget;
    }
}
