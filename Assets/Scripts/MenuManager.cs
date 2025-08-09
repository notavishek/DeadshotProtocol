using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("Audio")]
    public AudioClip buttonClickSound;
    private AudioSource audioSource;
    
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 1f;
        
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }
    
    public void PlayGame()
    {
        if (buttonClickSound != null && audioSource != null)
            audioSource.PlayOneShot(buttonClickSound);
        
        Time.timeScale = 1f;
        SceneManager.LoadScene("GameScene");
    }
    
    public void QuitGame()
    {
        if (buttonClickSound != null && audioSource != null)
            audioSource.PlayOneShot(buttonClickSound);
            
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
    
    void OnGUI()
    {
        // ONLY draw GUI if we're in the MainMenu scene
        if (SceneManager.GetActiveScene().name != "MainMenu")
            return;
            
        // Title
        GUIStyle titleStyle = new GUIStyle(GUI.skin.label);
        titleStyle.fontSize = 36;
        titleStyle.alignment = TextAnchor.MiddleCenter;
        GUI.Label(new Rect(Screen.width/2 - 150, 50, 300, 50), "Deadshot Protocol", titleStyle);
        
        // High Score Display
        int highScore = PlayerPrefs.GetInt("HighScore", 0);
        GUIStyle scoreStyle = new GUIStyle(GUI.skin.label);
        scoreStyle.fontSize = 16;
        scoreStyle.alignment = TextAnchor.MiddleCenter;
        GUI.Label(new Rect(Screen.width/2 - 75, 120, 150, 30), "High Score: " + highScore, scoreStyle);
        
        // Instructions
        GUIStyle instructionStyle = new GUIStyle(GUI.skin.label);
        instructionStyle.fontSize = 16;
        instructionStyle.alignment = TextAnchor.MiddleCenter;
        GUI.Label(new Rect(Screen.width/2 - 150, 160, 300, 30), "Shoot targets for 60 seconds!", instructionStyle);
        GUI.Label(new Rect(Screen.width/2 - 150, 180, 300, 70), "Headshots = 50 points, Body shots = 25 points", instructionStyle);
        
        // Buttons
        if (GUI.Button(new Rect(Screen.width/2 - 60, 270, 120, 40), "PLAY GAME"))
        {
            PlayGame();
        }

        if (GUI.Button(new Rect(Screen.width/2 - 60, 330, 120, 40), "QUIT GAME"))
        {
            QuitGame();
        }
    }
}
