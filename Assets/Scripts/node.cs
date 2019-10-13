using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{

    //The Nodes around the Nodes
    public Node[] neighbours;

    //The directions that pacman can go
    public Vector2[] availableDirections;
    // Start is called before the first frame update
    void Start()
    {
        //directions for each neighbour so the size are the length of the neighbours
        availableDirections = new Vector2[neighbours.Length];

        for (int i = 0; i < neighbours.Length; i++)
        {
            Node neighbour = neighbours[i];
            Vector2 tempVector = neighbour.transform.localPosition - transform.localPosition;
            tempVector = new Vector2(Mathf.Round(tempVector.x), Mathf.Round(tempVector.y));
            availableDirections[i] = tempVector.normalized;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
