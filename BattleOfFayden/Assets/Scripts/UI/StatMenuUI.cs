using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class StatMenuUI : MonoBehaviour
{
    public Sprite pangulin;
    public Sprite ape;

    public GameObject winnerBackground;
    public GameObject loserBackground;

    public class StatUIElement
    {
        public string playerName;
        public string spriteName;
        public PlayerStats stats;

        public StatUIElement()
        {
            playerName = "";
            spriteName = "";
            stats = new PlayerStats();
        }
    }

    private void Start()
    {
        KingOfTheHill.Instance.SetWinnerUIValues();
    }

    public void FillWinnerUI(bool isWinner)
    {
        TextMeshProUGUI[] elements;
        Image[] icons;

        StatUIElement firstPlayer = null;
        StatUIElement secondPlayer = null;

        if (isWinner)
        {
            elements = winnerBackground.GetComponentsInChildren<TextMeshProUGUI>();
            icons = winnerBackground.GetComponentsInChildren<Image>();
            firstPlayer = KingOfTheHill.Instance.winnerFirst;
            secondPlayer = KingOfTheHill.Instance.winnerSecond;
        }
        else
        {
            elements = loserBackground.GetComponentsInChildren<TextMeshProUGUI>();
            icons = loserBackground.GetComponentsInChildren<Image>();
            firstPlayer = KingOfTheHill.Instance.loserFirst;
            secondPlayer = KingOfTheHill.Instance.loserSecond;
        }

        foreach (var element in elements)
        {
            if (firstPlayer != null)
            {
                if (element.name == "firstPlayerName")
                {
                    element.text = firstPlayer.playerName;
                }
                else if (element.name == "firstPlayerStats")
                {
                    element.text =
                        firstPlayer.stats.kills + "\n" + // Kills
                        firstPlayer.stats.deaths + "\n" + // Deaths
                        firstPlayer.stats.capturePoints + "\n" + // CapturePoints
                        firstPlayer.stats.damage + "\n" + // Damage
                        firstPlayer.stats.mvp + "";               // MVP
                }
            }
            
            if (secondPlayer != null)
            {
                if (element.name == "secondPlayerName")
                {
                    element.text = secondPlayer.playerName;
                }
                else if (element.name == "secondPlayerStats")
                {
                    element.text =
                        secondPlayer.stats.kills + "\n" +           // Kills
                        secondPlayer.stats.deaths + "\n" +          // Deaths
                        secondPlayer.stats.capturePoints + "\n" +   // CapturePoints
                        secondPlayer.stats.damage + "\n" +          // Damage
                        secondPlayer.stats.mvp + "";                // MVP
                }
            }
        }

        foreach (var element in icons)
        {
            if (element.name == "firstPlayerIcon")
            {
                Sprite thisSprite;
                if (firstPlayer.spriteName == "pangulin")
                {
                    thisSprite = pangulin;
                }
                else
                {
                    thisSprite = ape;
                }
                element.sprite = thisSprite;
            }
            if (element.name == "secondPlayerIcon")
            {
                Sprite thisSprite;
                if (secondPlayer.spriteName == "pangulin")
                {
                    thisSprite = pangulin;
                } else
                {
                    thisSprite = ape;
                }
                element.sprite = thisSprite;
            }
        }
    }
}