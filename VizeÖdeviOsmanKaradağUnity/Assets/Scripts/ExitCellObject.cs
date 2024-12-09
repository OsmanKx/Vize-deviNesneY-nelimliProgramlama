using UnityEngine;
using UnityEngine.Tilemaps;

public class ExitCellObject : CellObject
{
    public Tile EndTile;
    

    public override void Init(Vector2Int coord)
    {
        base.Init(coord);
        GameManager.Instance.BoardManager.SetCellTile(coord, EndTile);

    }

    public override void PlayerEntered()
    {
        Debug.Log("Oyuncu ��k��a ula�t�");
        GameManager.Instance.NewLevel();
    }

    void Start()
    {
        
    }

    
    void Update()
    {
        
    }
}
