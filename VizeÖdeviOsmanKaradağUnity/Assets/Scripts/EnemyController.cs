using UnityEngine;

public class EnemyController : CellObject
{
    public int Health = 3;
    private int m_CurrentHealth;

    public int AmountGranted = 5;


    private void Awake()
    {
        GameManager.Instance.TurnManager.OnTick += TurnHappened;
    }

    private void OnDestroy()
    {
        GameManager.Instance.TurnManager.OnTick -= TurnHappened;
    }

    public override void Init(Vector2Int coord)
    {
        base.Init(coord);
        m_CurrentHealth = Health;
        transform.position = GameManager.Instance.BoardManager.CellToWorld(coord);
    }

    public override bool PlayerWantsToEnter()
    {
        m_CurrentHealth -= 3;

        if (m_CurrentHealth <= 0)
        {
            Destroy(gameObject);
        }
        GameManager.Instance.ChangeFood(AmountGranted);
        return false;
    }

    bool MoveTo(Vector2Int coord)
    {
        var board = GameManager.Instance.BoardManager;
        var targetCell = board.GetCellData(coord);

        if (targetCell == null || !targetCell.Passable || targetCell.ContainedObject != null)
        {
            return false; // Hareket edilemiyor
        }

        // �u anki h�creyi bo�alt
        var currentCell = board.GetCellData(m_Cell);
        currentCell.ContainedObject = null;

        // Yeni h�creye ge�i�
        targetCell.ContainedObject = this;
        m_Cell = coord; // m_Cell'� g�ncelle
        transform.position = board.CellToWorld(coord); // D�nya pozisyonunu g�ncelle

        return true;
    }

    private void TurnHappened()
    {
        var playerCell = GameManager.Instance.PlayerController.Cell;

        int xDist = playerCell.x - m_Cell.x;
        int yDist = playerCell.y - m_Cell.y;

        int absXDist = Mathf.Abs(xDist);
        int absYDist = Mathf.Abs(yDist);

        // Oyuncu ile ayn� h�crede mi?
        if ((absXDist == 1 && yDist == 0) || (absYDist == 1 && xDist == 0))
        {
            // Oyuncu biti�ikteyse, sald�r
            GameManager.Instance.ChangeFood(-1);
        }
        else
        {
            // Oyuncuya do�ru hareket et
            if (absXDist > absYDist)
            {
                if (!TryMoveInX(xDist))
                {
                    TryMoveInY(yDist);
                }
            }
            else
            {
                if (!TryMoveInY(yDist))
                {
                    TryMoveInX(xDist);
                }
            }
        }
    }

    private bool TryMoveInX(int xDist)
    {
        if (xDist > 0) // Oyuncu sa�daysa
        {
            return MoveTo(m_Cell + Vector2Int.right);
        }
        else // Oyuncu soldaysa
        {
            return MoveTo(m_Cell + Vector2Int.left);
        }
    }

    private bool TryMoveInY(int yDist)
    {
        if (yDist > 0) // Oyuncu yukar�daysa
        {
            return MoveTo(m_Cell + Vector2Int.up);
        }
        else // Oyuncu a�a��daysa
        {
            return MoveTo(m_Cell + Vector2Int.down);
        }


    }
}
