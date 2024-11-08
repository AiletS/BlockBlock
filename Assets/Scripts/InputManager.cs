using UnityEngine;

public class InputManager : MonoBehaviour
{
    private GridManager gridManager;

    private void Start()
    {
        gridManager = FindObjectOfType<GridManager>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            gridManager.Move(Directions.Up);
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            gridManager.Move(Directions.Down);
        }
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            gridManager.Move(Directions.Left);
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            gridManager.Move(Directions.Right);
        }
        else if(Input.GetKeyDown(KeyCode.Z))
        {
            gridManager.Undo();
        }
        else if(Input.GetKeyDown(KeyCode.R))
        {
            gridManager.Restart();
        }
        else if(Input.GetKeyDown(KeyCode.Q))
        {
            gridManager.PrevLevel();
        }
        else if(Input.GetKeyDown(KeyCode.E))
        {
            gridManager.NextLevel();
        }
    }
}
