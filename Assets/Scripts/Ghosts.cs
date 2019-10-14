using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghosts : MonoBehaviour
{
    public float speed = 3.7f;

    public Node startingPosition;

    public int[] scatterModeTimer = { 7, 7, 5, 5 };
    public int[] chaseModeTimer = { 20, 20, 20 };

    // Start is called before the first frame update
    private int counterModeChange = 1;
    private float timerModeChange = 0;

    public enum Mode
    {
        Chase,
        Scatter,
        Frightened
    }

    Mode currentMode = Mode.Scatter;
    // Mode oldMode;

    private Node oldNode, currentNode, desiredNode;
    private Vector2 moveDirection, desiredDirection;

    private GameObject pacman;


    void Start()
    {
        pacman = GameObject.FindGameObjectWithTag("PacMan");

        Node node2 = GetNodeAtPostion(transform.localPosition);

        if (node2 != null)
        {
            currentNode = node2;

        }

        moveDirection = Vector2.right;


        oldNode = currentNode;


        Vector2 pacmanPos = pacman.transform.position;
        Vector2 nextTile = new Vector2(Mathf.RoundToInt(pacmanPos.x), Mathf.RoundToInt(pacmanPos.y));

        desiredNode = GetNodeAtPostion(nextTile);


    }

    // Update is called once per frame
    void Update()
    {
        updateMode();
        move();
    }

    void move()
    {
        if (desiredNode != currentNode && desiredNode != null)
        {
            if (possibleTarget())
            {

                currentNode = desiredNode;

                transform.localPosition = currentNode.transform.position;

                GameObject nextPortal = getPortal(currentNode.transform.position);

                if (nextPortal != null)
                {
                    transform.localPosition = nextPortal.transform.position;
                    currentNode = nextPortal.GetComponent<Node>();
                }
                desiredNode = GetNextNode();
                oldNode = currentNode;
                currentNode = null;
            }
            else
            {
                transform.localPosition += (Vector3)moveDirection * speed * 2 * Time.deltaTime;
            }
        }

    }

    void updateMode()
    {
        if (currentMode != Mode.Frightened)
        {
            timerModeChange += Time.deltaTime;
            if (counterModeChange == 1)
            {
                if (currentMode == Mode.Scatter && timerModeChange > scatterModeTimer[0])
                {
                    changeMode(Mode.Chase);
                    timerModeChange = 0;
                }
                if (currentMode == Mode.Chase && timerModeChange > chaseModeTimer[0])
                {
                    counterModeChange = 2;
                    changeMode(Mode.Scatter);
                    timerModeChange = 0;
                }
            }
            else if (counterModeChange == 2)
            {
                if (currentMode == Mode.Scatter && timerModeChange > scatterModeTimer[1])
                {
                    changeMode(Mode.Chase);
                    timerModeChange = 0;
                }
                if (currentMode == Mode.Chase && timerModeChange > chaseModeTimer[1])
                {
                    counterModeChange = 3;
                    changeMode(Mode.Scatter);
                    timerModeChange = 0;
                }
            }
            else if (counterModeChange == 3)
            {
                if (currentMode == Mode.Scatter && timerModeChange > scatterModeTimer[2])
                {
                    changeMode(Mode.Chase);
                    timerModeChange = 0;
                }
                if (currentMode == Mode.Chase && timerModeChange > chaseModeTimer[2])
                {
                    counterModeChange = 4;
                    changeMode(Mode.Scatter);
                    timerModeChange = 0;
                }
            }
            else if (counterModeChange == 4)
            {
                if (currentMode == Mode.Scatter && timerModeChange > scatterModeTimer[3])
                {
                    changeMode(Mode.Chase);
                    timerModeChange = 0;
                }
            }
        }
        else if (currentMode == Mode.Frightened) { }

    }
    void changeMode(Mode m)
    {
        currentMode = m;
    }


    Node GetNextNode()
    {
        Vector2 nextTile = Vector2.zero;
        Vector2 pacmanPos = pacman.transform.position;
        nextTile = new Vector2(Mathf.RoundToInt(pacmanPos.x), Mathf.RoundToInt(pacmanPos.y));

        Node nextNode = null;
        Node[] availableNodes = new Node[4];
        Vector2[] availDirections = new Vector2[4];

        int counter = 0;
        for (int i = 0; i < currentNode.neighbours.Length; i++)
        {
            if (currentNode.availableDirections[i] != moveDirection * -1)
            {
                availableNodes[counter] = currentNode.neighbours[i];
                availDirections[counter] = currentNode.availableDirections[i];
                counter++;
            }

        }
        if (availableNodes.Length == 1)
        {
            nextNode = availableNodes[0];
            moveDirection = availDirections[0];
        }
        if (availableNodes.Length > 1)
        {
            float highDistance = 100000f;

            for (int i = 0; i < availableNodes.Length; i++)
            {
                if (availDirections[i] != Vector2.zero)
                {

                    float dist = GetDistance(availableNodes[i].transform.position, nextTile);

                    if (dist < highDistance)
                    {
                        highDistance = dist;
                        nextNode = availableNodes[i];
                        moveDirection = availDirections[i];
                    }
                }

            }
        }
        return nextNode;
    }
    Node GetNodeAtPostion(Vector2 position)
    {
        //look into it
        GameObject tile = GameObject.Find("Game").GetComponent<LevelOneBoard>().board[(int)Mathf.RoundToInt(position.x), (int)Mathf.RoundToInt(position.y)];
        if (tile != null)
        {
            if (tile.GetComponent<Node>() != null)
            {
                return tile.GetComponent<Node>();
            }
        }
        return null;
    }

    GameObject getPortal(Vector2 v)
    {
        GameObject tile = GameObject.Find("Game").GetComponent<LevelOneBoard>().board[(int)Mathf.Round(v.x), (int)Mathf.Round(v.y)];

        if (tile != null)
        {
            if (tile.GetComponent<Tile>() != null)
            {

                if (tile.GetComponent<Tile>().isPortal)
                {
                    GameObject nextPortal = tile.GetComponent<Tile>().firstPortal;
                    return nextPortal;
                }
            }
        }
        return null;
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

    float GetDistance(Vector2 first_position, Vector2 second_position)
    {
        return (Mathf.Sqrt(Mathf.Pow((first_position.x - second_position.x), 2) + Mathf.Pow((first_position.y - second_position.y), 2)));
    }


}
