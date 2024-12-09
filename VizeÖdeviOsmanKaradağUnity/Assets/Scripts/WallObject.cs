using UnityEngine;
using UnityEngine.Tilemaps;

public class WallObject : CellObject
{
    public Tile ObstacleTile;
    public int Wall1MaxHealth = 5;

    private int m_HealthPoint;
    private Tile m_OriginalTile;

    public int AmountGranted = 2;

    public override void Init(Vector2Int cell)
    {
        base.Init(cell);

        m_HealthPoint = Wall1MaxHealth;
        

        m_OriginalTile = GameManager.Instance.BoardManager.GetCellTile(cell);
        GameManager.Instance.BoardManager.SetCellTile(cell, ObstacleTile);
    }

    public override bool PlayerWantsToEnter() 
    {
        m_HealthPoint -= 1;
        

        if (m_HealthPoint > 0)
        {
            return false;
        }

        GameManager.Instance.BoardManager.SetCellTile(m_Cell, m_OriginalTile);
        GameManager.Instance.ChangeFood(AmountGranted);
        Destroy(gameObject);
        return true;
        



    }

    void Start()
    {
        
    }

    

    void Update()
    {
        
    }
}
