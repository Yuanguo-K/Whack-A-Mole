using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
  [SerializeField] private List<Mole> moles;

  [Header("UI objects")]
  [SerializeField] private GameObject playButton;
  [SerializeField] private GameObject gameUI;
  // [SerializeField] private GameObject outOfTimeText;
  // // [SerializeField] private GameObject bombText;
  // [SerializeField] private Image timeBar; // The countdown bar
  [SerializeField] private TMPro.TextMeshProUGUI timeText;
  [SerializeField] private TMPro.TextMeshProUGUI scoreText;
  [Header("Game Over UI")]
  [SerializeField] private GameObject gameOverPanel;
  [SerializeField] private TMPro.TextMeshProUGUI finalScoreText;

  // Hardcoded variables you may want to tune.
  private float startingTime = 30f;
  private int maxActiveMoles = 5; // Adjust this number to have more moles on screen

  // Global variables
  private float timeRemaining;
  private HashSet<Mole> currentMoles = new HashSet<Mole>();
  private int score;
  private bool playing = false;

  // This is public so the play button can see it.
  public void StartGame() {
    // Hide/show the UI elements we don't/do want to see.
    playButton.SetActive(false);
    gameOverPanel.SetActive(false);
    // outOfTimeText.SetActive(false);
    // bombText.SetActive(false);
    gameUI.SetActive(true);
    // Hide all the visible moles.
    for (int i = 0; i < moles.Count; i++) {
      moles[i].Hide();
      moles[i].SetIndex(i);
    }
    // Remove any old game state.
    currentMoles.Clear();
    // Start with 30 seconds.
    timeRemaining = startingTime;
    score = 0;
    scoreText.text = "0";
    playing = true;
  }

  public void GameOver() {
    // Show the message.
    // if (type == 0) {
    //   outOfTimeText.SetActive(true);
    // } else {
    //   bombText.SetActive(true);
    // }
    // bombText.SetActive(true);

    gameOverPanel.SetActive(true);
    gameUI.SetActive(false);
    finalScoreText.text = $"Your Final Score: {score}";
    // Hide all moles.
    foreach (Mole mole in moles) {
      mole.StopGame();
    }
    // Stop the game and show the start UI.
    playing = false;
    playButton.SetActive(true);
  }

  // Update is called once per frame
  void Update() {
    if (playing) {
      // Update time.
      timeRemaining -= Time.deltaTime;
      if (timeRemaining <= 0) {
        timeRemaining = 0;
        GameOver();
      }
      UpdateTimeUI();
      // timeText.text = $"{(int)timeRemaining / 60}:{(int)timeRemaining % 60:D2}";
      // Check if we need to start any more moles.
      while (currentMoles.Count <= maxActiveMoles) {
        // Choose a random mole.
        int index = Random.Range(0, moles.Count);
        // Doesn't matter if it's already doing something, we'll just try again next frame.
        if (!currentMoles.Contains(moles[index])) {
          currentMoles.Add(moles[index]);
          moles[index].Activate(score / 10);
        }
      }
    }
  }
  private void UpdateTimeUI() {
        // Update time text
        timeText.text = $"{(int)timeRemaining / 60}:{(int)timeRemaining % 60:D2}";

        // Change color to red if time is less than 10 seconds
        if (timeRemaining <= 10f) {
            timeText.color = Color.red;
        } else {
            timeText.color = Color.white;
        }

        // Update the time bar
        // UpdateTimeBar();
    }
  // private void UpdateTimeBar() {
  //       // Assuming the timeBar is an Image with a filled type of Horizontal
  //       float fillAmount = timeRemaining / startingTime;
  //       timeBar.fillAmount = fillAmount;
  //   }
  public void AddScore(int moleIndex, int amount) {
    // Add and update score.
    score += amount;
    scoreText.text = $"{score}";
    // Increase time by a little bit.
    // timeRemaining += 1;
    // Remove from active moles.
    currentMoles.Remove(moles[moleIndex]);
  }

  public void Missed(int moleIndex, bool isMole) {
    // if (isMole) {
    //   // Decrease time by a little bit.
    //   timeRemaining -= 2;
    // }
    // Remove from active moles.
    currentMoles.Remove(moles[moleIndex]);
  }
}
