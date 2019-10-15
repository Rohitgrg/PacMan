using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghosts : MonoBehaviour
{

    public RuntimeAnimatorController ghostUp;
    public RuntimeAnimatorController ghostDown;
    public RuntimeAnimatorController ghostLeft;
    public RuntimeAnimatorController ghostRight;




    public Vector2 orientation;
    public float speed = 3.7f;

    public Node startingPosition;
    public Node homePosition;

    public float ghostsReleaseTime = 0;
    public float pinkyReleaseTime = 5;
    public float inkyReleaseTime = 14;
    public float clydeReleaseTime = 21;

    public bool inJail = false;

    public enum GhostType
    {
        Blue,
        Pink,
        Orange,
        Red
    }

    public GhostType ghostType = GhostType.Red;



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

    private GameObject pinkGhost;

    private GameObject blueGhost;

    void Start()
    {
        pacman = GameObject.FindGameObjectWithTag("PacMan");
        pinkGhost = GameObject.FindGameObjectWithTag("Pinky");
        blueGhost = GameObject.FindGameObjectWithTag("Inky");

        Node node2 = GetNodeAtPostion(transform.localPosition);

        if (node2 != null)
        {
            currentNode = node2;

        }

        orientation = Vector2.right;

        oldNode = currentNode;


        if (inJail)
        {
            moveDirection = Vector2.up;
            desiredNode = currentNode.neighbours[0];
        }
        else
        {
            moveDirection = Vector2.left;
            desiredNode = GetNextNode();
        }
        updateAnimatorController();

    }

    // Update is called once per frame
    void Update()
    {
        updateMode();
        move();
        releaseGhosts();
    }

    void updateAnimatorController()
    {

        if (moveDirection == Vector2.down)
        {
            transform.GetComponent<Animator>().runtimeAnimatorController = ghostDown;
        }
        else if (moveDirection == Vector2.up)
        {
            transform.GetComponent<Animator>().runtimeAnimatorController = ghostUp;
        }
        else if (moveDirection == Vector2.left)
        {
            transform.GetComponent<Animator>().runtimeAnimatorController = ghostLeft;
        }
        else if (moveDirection == Vector2.right)
        {
            transform.GetComponent<Animator>().runtimeAnimatorController = ghostRight;
        }
        else
        {
            transform.GetComponent<Animator>().runtimeAnimatorController = ghostRight;
        }

    }

    Vector2 GetNextTile()
    {
        Vector2 nextTile = Vector2.zero;
        switch (ghostType)
        {
            case GhostType.Red:
                Vector2 pacmanPos = pacman.transform.position;
                nextTile = new Vector2(Mathf.RoundToInt(pacmanPos.x), Mathf.RoundToInt(pacmanPos.y));
                return nextTile;
            case GhostType.Blue:
                Vector2 blueGhostPos = blueGhost.transform.position;
                Vector2 pacmanTilePos = pacman.transform.position;
                Vector2 homeTilePos = homePosition.transform.position;

                float distanceInkyToPac = GetDistance(blueGhostPos, pacmanTilePos);
                float distancePacToHome = GetDistance(homeTilePos, pacmanTilePos);
                float distanceInkyToHome = GetDistance(homeTilePos, blueGhostPos);

                Vector2 pacmanOrientation = pacman.GetComponent<Pacman>().orientation;


                if (distanceInkyToPac < 8)
                {
                    nextTile = pacmanTilePos * -1;
                }
                else
                {
                    nextTile = pacmanTilePos + (10 * pacmanOrientation);
                }

                return nextTile;

            case GhostType.Pink:
                Vector2 pinkGhostPos = pinkGhost.transform.position;
                Vector2 pinkGhostOrientation = pinkGhost.GetComponent<Ghosts>().orientation;
                // pinkGhostOrientation = pinkGhostOrientation.right;
                Vector2 pinkyTile = new Vector2(Mathf.RoundToInt(pinkGhostPos.x), Mathf.RoundToInt(pinkGhostPos.y));
                nextTile = pinkyTile + (10 * pinkGhostOrientation);
                updateOrientation();
                return nextTile;
            case GhostType.Orange:
                Vector2 pacmanPosition = pinkGhost.transform.position;
                Vector2 blueGhostOrientation = pinkGhost.GetComponent<Ghosts>().orientation;
                Vector2 clydeTile = new Vector2(Mathf.RoundToInt(pacmanPosition.x), Mathf.RoundToInt(pacmanPosition.y));
                int rand = Random.Range(1, 25);
                nextTile = clydeTile + (rand * blueGhostOrientation);
                return nextTile;
            default:
                return nextTile;
        }
    }

    void releasePinkGhost()
    {
        if (inJail && ghostType == GhostType.Pink)
        {
            inJail = false;

        }
    }
    void releaseBlueGhost()
    {
        if (inJail && ghostType == GhostType.Blue)
        {
            inJail = false;

        }
    }
    void releaseOrangeGhost()
    {
        if (inJail && ghostType == GhostType.Orange)
        {
            inJail = false;

        }
    }

    void releaseGhosts()
    {
        ghostsReleaseTime += Time.deltaTime;
        if (ghostsReleaseTime > pinkyReleaseTime) { releasePinkGhost(); }
        if (ghostsReleaseTime > inkyReleaseTime) { releaseBlueGhost(); }
        if (ghostsReleaseTime > clydeReleaseTime) { releaseOrangeGhost(); }

    }

    void move()
    {
        if (desiredNode != currentNode && desiredNode != null && !inJail)
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

                updateAnimatorController();
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
        // (currentMode == Mode.Chase)

        switch (currentMode)
        {
            case Mode.Chase:
                nextTile = GetNextTile();
                break;
            case Mode.Scatter:
                nextTile = homePosition.transform.position;
                break;
            default:
                break;
        }


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


    void updateOrientation()
    {
        //Change the direction that Pacman is facing
        if (moveDirection == Vector2.up)
        {
            orientation = Vector2.up;
        }
        else if (moveDirection == Vector2.down)
        {
            orientation = Vector2.down;
        }
        else if (moveDirection == Vector2.left)
        {
            orientation = Vector2.right;
        }

        else if (moveDirection == Vector2.right)
        {
            orientation = Vector2.right;


        }
    }


    void updateInkyOrientation(Vector2 direction)
    {
        //Change the direction that Pacman is facing
        if (direction == Vector2.up)
        {
            orientation = Vector2.down;
        }
        else if (direction == Vector2.down)
        {
            orientation = Vector2.up;
        }
        else if (direction == Vector2.left)
        {
            orientation = Vector2.right;
        }

        else if (direction == Vector2.right)
        {
            orientation = Vector2.left;


        }
    }

}
