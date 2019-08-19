using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class PlayerLifeManager : MonoBehaviour
{
    public GameObject playerUIPanel;

    public int initialNumberOfLives = 3;
    [SerializeField] private int currentNumberOfLives;
    [SerializeField] private int currentScore = 0;
    public int CurrentNumberOfLives
    {
        get { return currentNumberOfLives; }
    }

    void Awake()
    {
        currentNumberOfLives = initialNumberOfLives;
    }

    private void Start()
    {
        UpdateUIText();
    }

    public void DecrementNumberOfLives()
    {
        currentNumberOfLives = Mathf.Max(currentNumberOfLives - 1, 0);
        UpdateUIText();
    }

    public void AddTenPoints()
    {
        currentScore += 10;
        UpdateUIText();
    }

    public void AddTwentyFivePoints()
    {
        currentScore += 25;
        UpdateUIText();
    }

    public bool IsGameOver()
    {
        return currentNumberOfLives <= 0;
    }

    private void UpdateUIText()
    {
        var textObjs = playerUIPanel.GetComponentsInChildren<Text>();
        foreach (var textObj in textObjs)
        {
            if (textObj.name.Equals("LivesText"))
            {
                textObj.text = $"Lives: {currentNumberOfLives}";
            }
            else if (textObj.name.Equals("ScoreText"))
            {
                textObj.text = $"Score: {currentScore}";
            }
        }
    }
}
