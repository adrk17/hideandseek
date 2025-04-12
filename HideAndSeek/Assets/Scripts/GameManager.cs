using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Game duration in seconds")]
    public float gameDuration = 60f;
    private float _timer;
    private bool _gameActive = false;

    [Header("UI")]
    public TextMeshProUGUI timerText;
    public KeyCode toggleTimerKey = KeyCode.T;
    public KeyCode startGameKey = KeyCode.Space;

    [Header("Agents")]
    public AgentGroupManager agentGroup; 

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        ResetAllAgents();

        if (timerText != null)
        {
            timerText.gameObject.SetActive(true);
            timerText.text = "Press " + startGameKey + " to start!";
        }
    }

    void Update()
    {
        if (!_gameActive)
        {
            if (Input.GetKeyDown(startGameKey))
            {
                StartGame();
            }
            return;
        }

        UpdateTimer();
    }

    public void StartGame()
    {
        _gameActive = true;
        _timer = gameDuration;

        if (timerText != null)
            timerText.gameObject.SetActive(true);

        ResetAllAgents();
    }

    public void EndGame(bool hiderWon)
    {
        _gameActive = false;
        Debug.Log(hiderWon ? "Hider wins!" : "Seeker wins!");

        ResetAllAgents();

        if (timerText != null)
            timerText.text = (hiderWon ? "Hider wins!" : "Seeker wins!") + "\nPress " + startGameKey + " to restart.";
    }

    private void OnToggleTimer()
    {
        if (Input.GetKeyDown(toggleTimerKey) && timerText != null)
        {
            timerText.gameObject.SetActive(!timerText.gameObject.activeSelf);
        }
    }

    private void UpdateTimer()
    {
        OnToggleTimer();

        _timer -= Time.deltaTime;

        if (timerText != null && timerText.gameObject.activeSelf)
            timerText.text = "Time: " + Mathf.CeilToInt(_timer);

        if (_timer <= 0f)
        {
            EndGame(true); // Hider wins
        }
    }

    private void ResetAllAgents()
    {
        if (agentGroup != null)
        {
            agentGroup.ResetAllAgents();
        }
    }
}
