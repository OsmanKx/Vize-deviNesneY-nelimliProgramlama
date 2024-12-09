using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BoardManager : MonoBehaviour
{
    

    private Grid m_Grid;
    private CellData[,] m_BoardData;
    private Tilemap m_Tilemap;
    private List<Vector2Int> m_EmptyCellList;

    
    public int Width;
    public int Height;
    public Tile[] GroundTiles;
    public Tile[] WallTiles;
    public PlayerController Player;


    public FoodObject[] FoodPreFabs; // Rastgele Yiyecek Prefabý
    
    public int MinFoodCount = 3;
    public int MaxFoodCount = 9;

    public WallObject[] WallPrefabs; //Duvar Prefabý


    public ExitCellObject ExitCell;

    public EnemyController[] EnemyPreFabs;
    public int MinEnemyCount = 3;
    public int MaxEnemyCount = 5;


    public class CellData
    {
        public bool Passable;
        public CellObject ContainedObject;
    }

    void Start()
    {



        m_Tilemap = GetComponentInChildren<Tilemap>();

        m_BoardData = new CellData[Width, Height];

        m_Grid = GetComponentInChildren<Grid>();


        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                Tile tile;
                m_BoardData[x, y] = new CellData();

                if (x == 0 || y == 0 || x == Width - 1 || y == Height - 1)
                {
                    tile = WallTiles[Random.Range(0, WallTiles.Length)];
                    m_BoardData[x, y].Passable = false;
                }
                else
                {
                    tile = GroundTiles[Random.Range(0, GroundTiles.Length)];
                    m_BoardData[x, y].Passable = true;
                }

                m_Tilemap.SetTile(new Vector3Int(x, y, 0), tile);
            }
        }
        Player.Spawn(this, new Vector2Int(1, 1));
    }

    private void SpawnEnemies()
    {
        // Rastgele düþman sayýsýný belirle
        int enemyCount = Random.Range(MinEnemyCount, MaxEnemyCount + 1);

        for (int i = 0; i < enemyCount; i++)
        {
            if (m_EmptyCellList.Count == 0) break;

            // Rastgele bir hücre seç
            int randomIndex = Random.Range(0, m_EmptyCellList.Count);
            Vector2Int randomCell = m_EmptyCellList[randomIndex];

            // Hücreyi kullanýldý olarak iþaretle
            m_EmptyCellList.RemoveAt(randomIndex);

            // Düþmaný oluþtur
            EnemyController selectedEnemyPrefab = EnemyPreFabs[Random.Range(0, EnemyPreFabs.Length)];
            GameObject enemy = Instantiate(selectedEnemyPrefab.gameObject, CellToWorld(randomCell), Quaternion.identity);
            enemy.GetComponent<EnemyController>().Init(randomCell);


            // Enemy script'ini baþlat
            enemy.GetComponent<EnemyController>().Init(randomCell);
        }
    }



    public void Init()
    {
        
        m_Tilemap = GetComponentInChildren<Tilemap>();
        m_Grid = GetComponentInChildren<Grid>();
        m_EmptyCellList = new List<Vector2Int>();


        m_BoardData = new CellData[Width, Height];



        m_EmptyCellList.Remove(new Vector2Int(1, 1));

        

        for (int y = 0; y < Height; ++y)
        {
            for (int x = 0; x < Width; ++x)
            {
                Tile tile;
                m_BoardData[x, y] = new CellData();

                if (x == 0 || y == 0 || x == Width - 1 || y == Height - 1)
                {
                    tile = WallTiles[Random.Range(0, WallTiles.Length)];
                    m_BoardData[x, y].Passable = false;
                }
                else
                {
                    tile = GroundTiles[Random.Range(0, GroundTiles.Length)];
                    m_BoardData[x, y].Passable = true;

                    m_EmptyCellList.Add(new Vector2Int(x, y));
                }

                m_Tilemap.SetTile(new Vector3Int(x, y, 0 ), tile);
    
            }

            

        }
        m_EmptyCellList.Remove(new Vector2Int(1, 1));

        Vector2Int coord = new Vector2Int(Width - 2, Height - 2);
        AddObject(Instantiate(ExitCell), coord);
        m_EmptyCellList.Remove(coord);

        GenerateWall();
        GenerateFood();
        SpawnEnemies();
    }


    public Vector3 CellToWorld(Vector2Int cellIndex)
    {
        return m_Grid.GetCellCenterWorld((Vector3Int)cellIndex);
    }

    public CellData GetCellData(Vector2Int cellIndex)
    {
        if (cellIndex.x < 0 || cellIndex.x >= Width
            || cellIndex.y < 0 || cellIndex.y >= Height)
        {
            return null;
        }

        return m_BoardData[cellIndex.x, cellIndex.y];
    }

    public void Clean()
    {
        if (m_BoardData == null)
            return;

        for (int y = 0; y < Height; ++y)
        {
            for (int x = 0; x < Width; ++x)
            {
                var cellData = m_BoardData[x, y];

                if (cellData.ContainedObject != null)
                {
                    Destroy(cellData.ContainedObject.gameObject); // Hücredeki nesneyi yok et
                }

                SetCellTile(new Vector2Int(x, y), null); // Hücreyi boþ býrak
            }
        }
    }

    void AddObject(CellObject obj, Vector2Int coord)
    {
        CellData data = m_BoardData[coord.x, coord.y];
        obj.transform.position = CellToWorld(coord);
        data.ContainedObject = obj;
        obj.Init(coord);
    }



    void GenerateFood()
    {
        
        int foodCount = Random.Range(MinFoodCount, MaxFoodCount + 1); //Rastgele Yiyecek Sayýsý
        for (int i = 0; i < foodCount; ++i)
        {
            //Rastgele yer seç
            int randomIndex = Random.Range(0, m_EmptyCellList.Count);
            Vector2Int coord = m_EmptyCellList[randomIndex];

            m_EmptyCellList.RemoveAt(randomIndex);
            
            // FoodPrefabs dizisinden rastgele bir prefab seç
            FoodObject randomFoodPrefab = FoodPreFabs[Random.Range(0, FoodPreFabs.Length)];
            FoodObject newFood = Instantiate(randomFoodPrefab); // Prefab'ý instantiate et
            newFood.transform.position = CellToWorld(coord); // Dünya koordinatýna yerleþtir
            AddObject(newFood, coord);

        }
    }

    void GenerateWall()
    {
        int wallCount = Random.Range(6, 10);
        for (int i = 0; i < wallCount; ++i)
        {
            int randomIndex = Random.Range(0, m_EmptyCellList.Count);
            Vector2Int coord = m_EmptyCellList[randomIndex];

            m_EmptyCellList.RemoveAt(randomIndex);
            WallObject randomWallPrefabs = WallPrefabs[Random.Range(0, WallPrefabs.Length)];
            WallObject newWall = Instantiate(randomWallPrefabs);
            newWall.transform.position = CellToWorld(coord);

            AddObject(newWall, coord);
        }
    }

    public void SetCellTile(Vector2Int cellIndex, Tile tile)
    {
        m_Tilemap.SetTile(new Vector3Int(cellIndex.x, cellIndex.y, 0), tile);
    }

    public Tile GetCellTile(Vector2Int cellIndex)
    {
        return m_Tilemap.GetTile<Tile>(new Vector3Int(cellIndex.x, cellIndex.y, 0));
    }

    

    
    void Update()
    {
        
    }
}
