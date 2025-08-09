using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    [Header("Game Settings")]
    public float gameTime = 60f;
    public int playerScore = 0;
    public int playerHealth = 100;
    public int maxHealth = 100;
    
    [Header("UI")]
    public bool isPaused = false;
    public bool gameEnded = false;
    
    private float currentGameTime;
    private int highScore;
    private string hitFeedbackText = "";
    private Color hitFeedbackColor = Color.white;
    private float hitFeedbackTimer = 0f;
    
    void Awake()
    {
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            Destroy(gameObject);
            return;
        }
        
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        Time.timeScale = 1f;
        isPaused = false;
        gameEnded = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        currentGameTime = gameTime;
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        playerScore = 0;
    }
    
    void Update()
    {
        if (gameEnded) return;
        
        // Game timer
        currentGameTime -= Time.deltaTime;
        if (currentGameTime <= 0)
        {
            EndGame();
            return;
        }
        
        // Pause functionality
        if (SceneManager.GetActiveScene().name == "GameScene")
        {
            if (Input.GetKeyDown(KeyCode.Escape) && !gameEnded)
            {
                TogglePause();
            }
        }
        
        // Update hit feedback timer
        if (hitFeedbackTimer > 0)
        {
            hitFeedbackTimer -= Time.deltaTime;
        }
    }
    
    public void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;
        Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isPaused;
    }
    
    public void AddScore(int points)
    {
        playerScore += points;
        
        // Check and update high score
        if (playerScore > highScore)
        {
            highScore = playerScore;
            PlayerPrefs.SetInt("HighScore", highScore);
        }
    }
    
    public void ShowHitFeedback(string text, Color color)
    {
        hitFeedbackText = text;
        hitFeedbackColor = color;
        hitFeedbackTimer = 1f; // Show for 1 second
    }
    
    void EndGame()
    {
        gameEnded = true;
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        // Save high score
        if (playerScore > PlayerPrefs.GetInt("HighScore", 0))
        {
            PlayerPrefs.SetInt("HighScore", playerScore);
        }
    }
    
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("GameScene");
    }
    
    public void ReturnToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
    
    void OnGUI()
    {
        if (SceneManager.GetActiveScene().name != "GameScene")
            return;
            
    // Game UI (bold, black, bigger)
    GUIStyle uiBoldStyle = new GUIStyle(GUI.skin.label);
    uiBoldStyle.fontStyle = FontStyle.Bold;
    uiBoldStyle.normal.textColor = Color.black;
    uiBoldStyle.fontSize = 16;
    GUI.Label(new Rect(Screen.width - 200, 10, 190, 28), "Score: " + playerScore, uiBoldStyle);
    GUI.Label(new Rect(Screen.width - 200, 38, 190, 28), "High Score: " + highScore, uiBoldStyle);
    GUI.Label(new Rect(Screen.width - 200, 66, 190, 28), "Time: " + Mathf.Max(0, Mathf.FloorToInt(currentGameTime)), uiBoldStyle);
        
        // Hit feedback
        if (hitFeedbackTimer > 0)
        {
            GUI.color = hitFeedbackColor;
            GUI.Label(new Rect(Screen.width / 2 - 50, Screen.height / 2 + 50, 100, 30), hitFeedbackText);
            GUI.color = Color.white;
        }
        
    // Instructions (bold)
    GUIStyle boldStyle = new GUIStyle(GUI.skin.label);
    boldStyle.fontStyle = FontStyle.Bold;
    boldStyle.normal.textColor = Color.black;
    GUI.Label(new Rect(10, Screen.height - 60, 200, 20), "ESC - Pause Menu", boldStyle);
    GUI.Label(new Rect(10, Screen.height - 40, 200, 20), "WASD - Move, Mouse - Look", boldStyle);
    GUI.Label(new Rect(10, Screen.height - 20, 200, 20), "Left Click - Shoot, R - Reload", boldStyle);
        
        // Crosshair (only when not paused and game not ended)
        if (!isPaused && !gameEnded)
        {
            float crosshairSize = 20f;
            float centerX = Screen.width / 2f;
            float centerY = Screen.height / 2f;
            
            GUI.DrawTexture(new Rect(centerX - 1, centerY - crosshairSize/2, 2, crosshairSize), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(centerX - crosshairSize/2, centerY - 1, crosshairSize, 2), Texture2D.whiteTexture);
        }

        // Pause menu
        if (isPaused && !gameEnded)
        {
            GUI.color = new Color(0, 0, 0, 0.8f);
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
            GUI.color = Color.white;

            float menuWidth = 400f;
            float menuHeight = 400f;
            float menuX = Screen.width / 2 - menuWidth / 2;
            float menuY = Screen.height / 2 - menuHeight / 2;

            GUI.Box(new Rect(menuX, menuY, menuWidth, menuHeight), "GAME PAUSED");

            float buttonWidth = 215f;
            float buttonHeight = 50f;
            float buttonX = Screen.width / 2 - buttonWidth / 2;

            if (GUI.Button(new Rect(buttonX, menuY + 60, buttonWidth, buttonHeight), "Resume"))
            {
                TogglePause();
            }

            if (GUI.Button(new Rect(buttonX, menuY + 125, buttonWidth, buttonHeight), "Restart Game"))
            {
                RestartGame();
            }

            if (GUI.Button(new Rect(buttonX, menuY + 190, buttonWidth, buttonHeight), "Main Menu"))
            {
                ReturnToMenu();
            }
            if (GUI.Button(new Rect(buttonX, menuY + 255, buttonWidth, buttonHeight), "Quit Game"))
            {
                QuitGame();
            }
           
        }
        
        // Game Over menu
        if (gameEnded)
        {
            GUI.color = new Color(0, 0, 0, 0.9f);
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
            GUI.color = Color.white;
            
            float menuWidth = 350f;
            float menuHeight = 300f;
            float menuX = Screen.width/2 - menuWidth/2;
            float menuY = Screen.height/2 - menuHeight/2;
            
            GUI.Box(new Rect(menuX, menuY, menuWidth, menuHeight), "TIME UP!");
            
            // Game over stats
            GUI.Label(new Rect(menuX + 20, menuY + 40, menuWidth - 40, 30), "Final Score: " + playerScore);
            GUI.Label(new Rect(menuX + 20, menuY + 70, menuWidth - 40, 30), "High Score: " + PlayerPrefs.GetInt("HighScore", 0));
            
            if (playerScore == PlayerPrefs.GetInt("HighScore", 0) && playerScore > 0)
            {
                GUI.color = Color.yellow;
                GUI.Label(new Rect(menuX + 20, menuY + 100, menuWidth - 40, 30), "NEW HIGH SCORE!");
                GUI.color = Color.white;
            }
            
            float buttonWidth = 120f;
            float buttonHeight = 35f;
            float buttonX = Screen.width/2 - buttonWidth/2;
            
            if (GUI.Button(new Rect(buttonX, menuY + 150, buttonWidth, buttonHeight), "Play Again"))
            {
                RestartGame();
            }
            
            if (GUI.Button(new Rect(buttonX, menuY + 195, buttonWidth, buttonHeight), "Main Menu"))
            {
                ReturnToMenu();
            }
            
            if (GUI.Button(new Rect(buttonX, menuY + 240, buttonWidth, buttonHeight), "Quit Game"))
            {
                Application.Quit();
            }
        }
    }
}
