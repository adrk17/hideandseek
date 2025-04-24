using System.Collections;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Game duration in seconds")]
    public float gameDuration = 60f;
    private float _timer;
    private bool _gameActive = false;
  
    public bool IsGameActive => _gameActive;

    [Header("UI")]
    public TextMeshProUGUI timerText;
    public KeyCode toggleTimerKey = KeyCode.T;
    public KeyCode startGameKey = KeyCode.Space;

    [Header("Agents")]
    public AgentGroupManager agentGroup; 
    
    [Header("Agent Position Control")]
    public bool randomizeStartPositionsOnGameStart = true;


    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        InitializeAndResetAgents();
    }

    void Update()
    {
        if (Input.GetKeyDown(startGameKey))
        {
            if (!_gameActive)
            {
                StartGame();
            }
            else
            {
                ResetGame();
            }
        }
        
        UpdateTimer();
    }
    
    ////// Game state management functions //////
    public void StartGame()
    {
        _gameActive = true;
        _timer = gameDuration;

        if (timerText != null)
            timerText.gameObject.SetActive(true);

        if (agentGroup != null)
        {
            if (randomizeStartPositionsOnGameStart && agentGroup.randomPositions)
            {
                agentGroup.SetRandomStartPositions();
            }

            agentGroup.ResetAllAgents();
        }
    }
    
    public void EndGame(bool hiderWon)
    {
        _gameActive = false;
        Debug.Log(hiderWon ? "Hider wins!" : "Seeker wins!");

        ResetAllAgents();

        if (timerText != null)
            timerText.text = (hiderWon ? "Hider wins!" : "Seeker wins!") + "\nPress " + startGameKey + " to restart.";
    }
    
    public void SeekerCaughtHider()
    {
        if (!_gameActive)
        {
            Debug.LogWarning("Seeker tried to end game while it wasn't active.");
            return;
        }

        EndGame(false); // Seeker wins
    }

    public void ResetGame()
    {
        _gameActive = false;
        _timer = gameDuration;
        
        if (timerText != null)
            timerText.gameObject.SetActive(false);
        ResetAllAgents();
        
        Debug.Log("Game reset.");
    }

    ////// Timer functions //////
    
    private void OnToggleTimer()
    {
        if (Input.GetKeyDown(toggleTimerKey) && timerText != null)
        {
            timerText.gameObject.SetActive(!timerText.gameObject.activeSelf);
        }
    }

    private void UpdateTimer()
    {
        if(!_gameActive)
            return;
        
        OnToggleTimer();

        _timer -= Time.deltaTime;

        if (timerText != null && timerText.gameObject.activeSelf)
            timerText.text = "Time: " + Mathf.CeilToInt(_timer);

        if (_timer <= 0f)
        {
            EndGame(true); // Hider wins
        }
    }

    ////// Agent positioning functions //////
    
    private void ResetAllAgents()
    {
        if (agentGroup != null && agentGroup.AgentsInitialized)
        {
            agentGroup.ResetAllAgents();
        }
        else
        {
            Debug.LogWarning("Tried to reset agents before they were initialized.");
        }
    }

    private void InitializeAndResetAgents()
    {
        StartCoroutine(WaitForAgentInitAndReset());
    }

    private IEnumerator WaitForAgentInitAndReset()
    {
        if (agentGroup == null)
        {
            Debug.LogWarning("AgentGroupManager is not assigned.");
            yield break;
        }

        while (!agentGroup.AgentsInitialized)
        {
            yield return null; // wait one frame
        }

        ResetAllAgents();

        if (timerText != null)
        {
            timerText.gameObject.SetActive(true);
            timerText.text = "Press " + startGameKey + " to start!";
        }
    }
}
