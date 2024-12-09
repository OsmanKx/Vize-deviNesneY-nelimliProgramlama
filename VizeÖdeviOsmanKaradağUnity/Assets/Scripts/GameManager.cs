using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance{ get; private set; }
    public int m_FoodAmount = 30;
    public BoardManager BoardManager;
    public PlayerController PlayerController;
    private ExitCellObject ExitCellObject;
    private int m_CurrentLevel = 1;

    public TurnManager TurnManager {  get; private set; }

    public UIDocument UIDoc;
    private Label m_FoodLabel;

    private VisualElement m_GameOverPanel;
    private Label m_GameOverMessage;

    

    

    public class CellData
    {
        public bool Passable;
        public CellObject ContainedObject;
    }

    public void Awake()
    {
        
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

    }

   



    void Start()
    {


        TurnManager = new TurnManager();
        TurnManager.OnTick += OnTurnHappen;

        NewLevel();

        m_FoodLabel = UIDoc.rootVisualElement.Q<Label>("FoodLabel");
        m_FoodLabel.text = "Food: " + m_FoodAmount;


        m_GameOverPanel = UIDoc.rootVisualElement.Q<VisualElement>("GameOverPanel");
        m_GameOverMessage = m_GameOverPanel.Q<Label>("GameOverMessage");

        PlayerController.Spawn(BoardManager, new Vector2Int(1, 1));
        PlayerController.MoveTo(new Vector2Int(1, 1));

        StartNewGame();
    }

    public void StartNewGame()
    {
        m_GameOverPanel.style.visibility = Visibility.Hidden;
  
        m_CurrentLevel = 1;
        m_FoodAmount = 20;
        m_FoodLabel.text = "Food : " + m_FoodAmount;
  
        BoardManager.Clean();
        BoardManager.Init();
  
        PlayerController.Init();
        PlayerController.Spawn(BoardManager, new Vector2Int(1,1));
        PlayerController.MoveTo(new Vector2Int(1, 1));

        
    }


    public void NewLevel()
    {
        BoardManager.Clean();
        BoardManager.Init();
        PlayerController.Spawn(BoardManager, new Vector2Int(1, 1));
        
        PlayerController.MoveTo(new Vector2Int(1, 1));

        m_CurrentLevel++;
    }


    void OnTurnHappen()
    {

        ChangeFood(-1);
        
    }

    public void ChangeFood(int amount)
    {
       int newFoodAmount = m_FoodAmount += amount;
        m_FoodAmount = Mathf.Max(newFoodAmount, 0);
        m_FoodLabel.text = "Food: " + m_FoodAmount;

        if (m_FoodAmount <= 0)
        {
            PlayerController.GameOver();
            m_GameOverPanel.style.visibility = Visibility.Visible;
            m_GameOverMessage.text = "Game Over!\n\nYou traveled through " + m_CurrentLevel + " levels";

        }
    }
    
    void Update()
    {
        
    }
}
