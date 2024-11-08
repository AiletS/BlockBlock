using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Rendering;

public enum Directions
{
    Up,
    Down,
    Left,
    Right
}

public class GridManager : MonoBehaviour
{
    public GameObject[,] tiles, bgtiles, answerTiles;
    public List<int[,]> grid_history = new List<int[,]>();
    private Dictionary<Vector2Int, int> answer = new Dictionary<Vector2Int, int>();
    private int now_level = 1;
    private int max_level = 1;
    public int cleard_max_level = 1;
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private List<Color> tileColors;
    [SerializeField] private Camera cam;
    [SerializeField] private GameObject clearText;

    private void Awake()
    {
        now_level = PlayerPrefs.GetInt("now_level", 1);
        cleard_max_level = PlayerPrefs.GetInt("cleard_max_level", 0);
        TextAsset[] problems = Resources.LoadAll<TextAsset>("Problem");
        max_level = problems.Length;
    }

    private void Start()
    {
        GenerateGrid(now_level);
    }

    public void GenerateGrid(int level)
    {
        clearText.SetActive(false);
        LoadGridFile(level);
        LoadAnswerGridFile(level);
        DrawGrid();
        PlayerPrefs.SetInt("now_level", level);
    }

    public void LoadGridFile(int level)
    {
        string filename = level.ToString();
        TextAsset textAsset = Resources.Load<TextAsset>("Problem/" + filename);
        if(textAsset == null)
        {
            Debug.LogError("Problem Grid file not found: " + filename);
        }

        string[] lines = textAsset.text.Split('\n');
        string[] rowcol = lines[0].Split(' ');
        int row = int.Parse(rowcol[0]);
        int col = int.Parse(rowcol[1]);
        int[,] grid = new int[row, col];
        tiles = new GameObject[row, col];
        bgtiles = new GameObject[row, col];

        float camposx = -1f;
        float camposy = col / 2f - (col % 2 == 0 ? 0.5f : 0);
        cam.transform.position = new Vector3(camposx, -camposy, -10);
        int camsize = Mathf.Max(row, col);
        cam.orthographicSize = camsize;

        for(int i = 1; i < lines.Length; i++)
        {
            string[] cells = lines[i].Split(' ');
            for(int j = 0; j < cells.Length; j++)
            {
                grid[i - 1, j] = int.Parse(cells[j]);
            }
        }
        grid_history.Add(grid);
        Debug.Log("Problem Loaded!");
    }

    public void LoadAnswerGridFile(int level)
    {
        string filename = level.ToString();
        TextAsset textAsset = Resources.Load<TextAsset>("Answer/" + filename);
        if(textAsset == null)
        {
            Debug.LogError("Answer Grid file not found: " + filename);
        }
        string[] lines = textAsset.text.Split('\n');
        string[] rowcol = lines[0].Split(' ');
        int row = int.Parse(rowcol[0]);
        int col = int.Parse(rowcol[1]);
        int[,] answer_grid = new int[row, col];
        answer = new Dictionary<Vector2Int, int>();
        for(int i = 1; i < lines.Length; i++)
        {
            string[] cells = lines[i].Split(' ');
            for(int j = 0; j < cells.Length; j++)
            {
                answer_grid[i - 1, j] = int.Parse(cells[j]);
            }
        }
        DrawAnswerGrid(answer_grid);
        List<Vector2Int> connectedTiles = GetConnectedTiles(answer_grid);
        Vector2Int playerPos = new Vector2Int();
        for(int i = 0; i < row; i++) for(int j = 0; j < col; j++)
        {
            if(answer_grid[i, j] == 2) playerPos = new Vector2Int(i, j);
        }
        foreach(var pos in connectedTiles)
        {
            answer[pos - playerPos] = answer_grid[pos.x, pos.y];
        }
        Debug.Log("Answer Loaded!");
        foreach(var item in answer)
        {
            Debug.Log(item.Key + " " + item.Value);
        }
    }

    public void DrawGrid()
    {
        int[,] now_grid = grid_history[grid_history.Count - 1];
        for(int i = 0; i < now_grid.GetLength(0); i++)
        {
            for(int j = 0; j < now_grid.GetLength(1); j++)
            {
                bgtiles[i, j] = Instantiate(tilePrefab, new Vector3(j, -i, 0), Quaternion.identity);
                bgtiles[i, j].GetComponent<SpriteRenderer>().color = tileColors[1];
                bgtiles[i, j].GetComponent<SpriteRenderer>().sortingOrder = -10;
                tiles[i, j] = Instantiate(tilePrefab, new Vector3(j, -i, 0), Quaternion.identity);
                tiles[i, j].GetComponent<SpriteRenderer>().color = tileColors[now_grid[i, j]];
            }
        }
    }

    public void DrawAnswerGrid(int[,] grid)
    {
        int row = grid.GetLength(0);
        int col = grid.GetLength(1);
        answerTiles = new GameObject[row, col];
        for(int i = 0; i < row; i++) for(int j = 0; j < col; j++)
        {
            answerTiles[i, j] = Instantiate(tilePrefab, new Vector3(j - col - 1, -i, 0), Quaternion.identity);
            answerTiles[i, j].GetComponent<SpriteRenderer>().color = tileColors[grid[i, j]];
        }

    }

    public void ClearGrid()
    {
        int[,] grid = grid_history[grid_history.Count - 1];
        for(int i = 0; i < grid.GetLength(0); i++)
        {
            for(int j = 0; j < grid.GetLength(1); j++)
            {
                Destroy(tiles[i, j]);
                Destroy(bgtiles[i, j]);
            }
        }
    }

    public List<Vector2Int> GetConnectedTiles(int[,] grid)
    {
        List<Vector2Int> connectedTiles = new List<Vector2Int>();
        int row = grid.GetLength(0);
        int col = grid.GetLength(1);
        for(int i = 0; i < row; i++) for(int j = 0; j < col; j++)
        {
            if(grid[i, j] == 2) connectedTiles.Add(new Vector2Int(i, j));
        }

        int[] dr = new int[] {0, 0, 1, -1};
        int[] dc = new int[] {1, -1, 0, 0};
        for(int i = 0; i < row; i++) for(int j = 0; j < col; j++)
        {
            for(int di = 0; di < row; di++) for(int dj = 0; dj < col; dj++)
            {
                if(grid[di, dj] < 2) continue;
                if(connectedTiles.Contains(new Vector2Int(di, dj))) continue;
                for(int k = 0; k < 4; k++)
                {
                    int ni = di + dr[k];
                    int nj = dj + dc[k];
                    if(ni < 0 || ni >= row || nj < 0 || nj >= col) continue;
                    if(connectedTiles.Contains(new Vector2Int(ni, nj)))
                    {
                        connectedTiles.Add(new Vector2Int(di, dj));
                        break;
                    }
                }
            }
        }

        return connectedTiles;
    }

    public bool CanMove(Directions directions)
    {
        int[,] grid = grid_history[grid_history.Count - 1];
        int row = grid.GetLength(0);
        int col = grid.GetLength(1);
        List<Vector2Int> connectedTiles = GetConnectedTiles(grid);
        foreach(Vector2Int tile in connectedTiles)
        {
            int i = tile.x;
            int j = tile.y;
            if(directions == Directions.Up)
            {
                if(i == 0) return false;
                if(grid[i - 1, j] == 0) return false;
            }
            else if(directions == Directions.Down)
            {
                if(i == row - 1) return false;
                if(grid[i + 1, j] == 0) return false;
            }
            else if(directions == Directions.Left)
            {
                if(j == 0) return false;
                if(grid[i, j - 1] == 0) return false;
            }
            else if(directions == Directions.Right)
            {
                if(j == col - 1) return false;
                if(grid[i, j + 1] == 0) return false;
            }
        }

        return true;
    }

    public void Move(Directions directions)
    {
        if(!CanMove(directions)) return;
        int[,] grid = grid_history[grid_history.Count - 1];
        int[,] new_grid = grid.Clone() as int[,];
        List<Vector2Int> connectedTiles = GetConnectedTiles(grid);
        foreach(Vector2Int tile in connectedTiles)
        {
            int i = tile.x;
            int j = tile.y;
            new_grid[i, j] = 1;
        }
        foreach(Vector2Int tile in connectedTiles)
        {
            int i = tile.x;
            int j = tile.y;
            if(directions == Directions.Up)
            {
                new_grid[i - 1, j] = grid[i, j];
            }
            else if(directions == Directions.Down)
            {
                new_grid[i + 1, j] = grid[i, j];
            }
            else if(directions == Directions.Left)
            {
                new_grid[i, j - 1] = grid[i, j];
            }
            else if(directions == Directions.Right)
            {
                new_grid[i, j + 1] = grid[i, j];
            }
        }

        grid_history.Add(new_grid);
        ClearGrid();
        DrawGrid();
        if(CheckAnswer())
        {
            clearText.SetActive(true);
            if(now_level > cleard_max_level)
            {
                cleard_max_level = now_level;
                PlayerPrefs.SetInt("cleard_max_level", cleard_max_level);
            }
        }
    }

    public void Undo()
    {
        if(grid_history.Count <= 1) return;
        grid_history.RemoveAt(grid_history.Count - 1);
        ClearGrid();
        DrawGrid();
    }

    public void Restart()
    {
        Reset();
        GenerateGrid(now_level);
    }

    public bool CheckAnswer()
    {
        int[,] grid = grid_history[grid_history.Count - 1];
        int row = grid.GetLength(0);
        int col = grid.GetLength(1);
        List<Vector2Int> connectedTiles = GetConnectedTiles(grid);
        Vector2Int playerPos = new Vector2Int();
        for(int i = 0; i < row; i++) for(int j = 0; j < col; j++)
        {
            if(grid[i, j] == 2) playerPos = new Vector2Int(i, j);
        }
        if(connectedTiles.Count != answer.Count) return false;
        foreach(Vector2Int tile in connectedTiles)
        {
            if(answer.ContainsKey(tile - playerPos))
            {
                if(answer[tile - playerPos] != grid[tile.x, tile.y]) return false;
            }
            else
            {
                return false;
            }
        }

        Debug.Log("Answer Correct!");
        return true;
    }

    public void NextLevel()
    {
        if(now_level == max_level) return;
        if(now_level > cleard_max_level) return;
        now_level++;
        Reset();
        GenerateGrid(now_level);
    }

    public void PrevLevel()
    {
        if(now_level == 1) return;
        now_level--;
        Reset();
        GenerateGrid(now_level);
    }

    public void Reset()
    {
        foreach(var obj in answerTiles) Destroy(obj);
        foreach(var obj in bgtiles) Destroy(obj);
        foreach(var obj in tiles) Destroy(obj);
        grid_history.Clear();
        answer.Clear();
    }
}
