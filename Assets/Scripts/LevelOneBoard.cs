using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelOneBoard : MonoBehaviour
{


    //height of the board
    private static int height = 36;

    //width of the board
    private static int width = 28;

    public int noOfPalets = 0;

    public int score = 0;

    public GameObject[,] board = new GameObject[width, height];
    // Start is called before the first frame update
    void Start()
    {
        Object[] objects = GameObject.FindObjectsOfType(typeof(GameObject));

        foreach (GameObject item in objects)
        {
            Vector2 position = item.transform.position;
            Tile tile = item.GetComponent<Tile>();
            if (item.name != "pacman" && item.name != "IntersectionNodes" && item.name != "nonIntersectionNodes" && item.name != "borders" && item.name != "palets" && item.tag != "Ghosts" && item.tag != "Pinky")
            {
                if (tile != null)
                {
                    if (tile.isBigPallet || tile.isBigPallet)
                    {
                        noOfPalets++;
                    }
                    board[(int)Mathf.Round(position.x), (int)Mathf.Round(position.y)] = item;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
