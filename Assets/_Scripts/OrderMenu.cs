using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
// using UnityEngine.UIElements;

public class OrderMenu : MonoBehaviour
{
    CanvasGroup canvasGroup;
    [SerializeField]
    TMP_Text timeText, weightText, collisionsText, scoreText, coinDisplay;
    public static OrderMenu instance;
    int coins = 0;
    void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(this);
            return;
        }

        instance = this;
        canvasGroup = GetComponent<CanvasGroup>();

    }

    void Start()
    {
        HideMenu();
    }
    public void Display(Delivery delivery)
    {
        ShowMenu();
        SetCursorToUI();

        timeText.text = "+ Time " + floatToTime(delivery.time);
        weightText.text = "+ package weight " + delivery.weight + " kg";
        collisionsText.text = "- collision endured " + delivery.collisions + " Ns";
        coins += Mathf.RoundToInt(calculateScore(delivery));
        scoreText.text = "Delivery score " + calculateScore(delivery);
        UpdateCoinsDisplay();
    }

    public bool hasCoins(int amount)
    {
        return coins >= amount;
    }

    public void RemoveCoins(int amount)
    {
        coins -= amount;
        UpdateCoinsDisplay();
    }

    void UpdateCoinsDisplay()
    {
        coinDisplay.text = "" + coins;
    }

    String floatToTime(float input)
    {
        int minutes  = (int) input/60;
        String minutesText  = minutes.ToString("D2");
        float secondsAndLess = input - minutes * 60;
        int seconds = (int) secondsAndLess;
        String secondsText = seconds.ToString("D2");
        String lessThanSeconds = "" + (secondsAndLess - seconds);
        String output = "" + minutesText + ":" + secondsText;
        return output;
    }

    void ShowMenu()
    {
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }
    void HideMenu()
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    void SetCursorToUI()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    void SetCursorToGame()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    float calculateScore(Delivery delivery)
    {
        float baseScore = 100 * delivery.weight;
        float collisionPenalty = delivery.collisions / -10;
        float timePenalty = -delivery.time * 10;
        float score = baseScore + collisionPenalty + timePenalty;
        score = Mathf.Max(200f, score);
        return score;
    }

    public void Close()
    {
        HideMenu();
        SetCursorToGame();
        Time.timeScale = 1;
        OrderManager.instance.SpawnOrder();
    }
}

public class Delivery
{
    public float time, weight, collisions;

    public Delivery(float time, float weight, float collisions)
    {
        this.time = time;
        this.weight = weight;
        this.collisions = collisions;
    }
}