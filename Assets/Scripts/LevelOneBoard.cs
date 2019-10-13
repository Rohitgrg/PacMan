using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelOneBoard : MonoBehaviour
{


    //height of the board
    private static int height = 36;

    //width of the board
    private static int width = 28;



    public GameObject[,] board = new GameObject[width, height];
    // Start is called before the first frame update
    void Start()
    {
        Object[] objects = GameObject.FindObjectsOfType(typeof(GameObject));

        foreach (GameObject item in objects)
        {
            Vector2 position = item.transform.position;
            if (item.name != "pacman" && item.name != "IntersectionNodes" && item.name != "nonIntersectionNodes" && item.name != "borders" && item.name != "palets")
            {
                board[(int)Mathf.Round(position.x), (int)Mathf.Round(position.y)] = item;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
