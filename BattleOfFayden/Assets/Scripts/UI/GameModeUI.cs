using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameModeUI : Photon.MonoBehaviour {

    public enum PointGainState
    {
        none,
        leftSide,
        rightSide
    }

    [SerializeField]
    private Image redTeam, blueTeam;

    [SerializeField]
    private RectTransform redTransform, blueTransform, sliderTransform;
    private float distance;
    public float lastPoints = 0.5f;

    public Color leftBarColor, rightBarColor;
    public static Color highlight = new Color(1.0f, 1.0f, 1.0f);

    public PointGainState state = PointGainState.none;

    Character character;

    public GameObject timer;
    public Text timerText;

    public float roundTime = 165.0f; // 3 minutes round time + 5 seconds camera
    public float timerf;

    private void Start()
    {
        distance = redTransform.localPosition.x - blueTransform.localPosition.x;

        timer = GameObject.Find("Timer");
        timerText = timer.GetComponent<Text>();
        timerf = roundTime;
    }

    /*private void OnPhotonSerializeView(
        PhotonStream stream,
        PhotonMessageInfo info)
    {
        if(stream.isWriting)
        {
            if (PhotonNetwork.isMasterClient)
                stream.SendNext(timerf);
        }
        else
        {
            timerf = (float)stream.ReceiveNext();
        }
    }*/

    private void Update()
    {
        if (KingOfTheHill.Instance.gameFinished)
            return;

        if (timerf - Time.deltaTime > 0)
            timerf -= Time.deltaTime;
        else
        {
            if (lastPoints > 0.6f)
            {
                KingOfTheHill.Instance.netView.RPC("StartNextRound", PhotonTargets.All, PunTeams.Team.red);
            }
            else if(lastPoints < 0.4f)
            {
                KingOfTheHill.Instance.netView.RPC("StartNextRound", PhotonTargets.All, PunTeams.Team.blue);
            }
            else
            {
                StartCoroutine(KingOfTheHill.Instance.player.GetComponent<Character>().WaitThenRespawn(3f));
                
                GameObject[] bombs = GameObject.FindGameObjectsWithTag("Bomb");
                foreach (GameObject bomb in bombs)
                {
                    PhotonNetwork.Destroy(bomb);
                }
            }

            timerf = (roundTime - 5.0f);
        }

        string minutes = Mathf.Floor(timerf / 60).ToString("00");
        string seconds = Mathf.Ceil(timerf % 60).ToString("00");

        timerText.text = string.Format("{0:0}:{1:00}", minutes, seconds);
        
        if (character == null)
        {
            character = KingOfTheHill.Instance.player.GetComponent<Character>();

            if(character.teamID == PunTeams.Team.red)
            {
                leftBarColor = character.teamColor;
                rightBarColor = character.enemyColor;

            }
            else if(character.teamID == PunTeams.Team.blue)
            {
                rightBarColor = character.teamColor;
                leftBarColor = character.enemyColor;
            }
        }
        
        float sin = Mathf.Sin(Time.time * 5);
        switch (state)
        {
            case PointGainState.leftSide:
                {
                    var color = redTeam.color;

                    color.r = rightBarColor.r + (rightBarColor.r - (sin * (highlight.r - rightBarColor.r)) / 1f);
                    color.g = rightBarColor.g + (rightBarColor.g - (sin * (highlight.g - rightBarColor.g)) / 1f);
                    color.b = rightBarColor.b + (rightBarColor.b - (sin * (highlight.b - rightBarColor.b)) / 1f);

                    redTeam.color = color;
                    break;
                }

            case PointGainState.rightSide:
                {
                    var color = blueTeam.color;

                    color.r = leftBarColor.r + (leftBarColor.r - (sin * (highlight.r - leftBarColor.r)) / 1f);
                    color.b = leftBarColor.b + (leftBarColor.b - (sin * (highlight.b - leftBarColor.b)) / 1f);
                    color.g = leftBarColor.g + (leftBarColor.g - (sin * (highlight.g - leftBarColor.g)) / 1f);

                    blueTeam.color = color;
                    break;
                }

            default:
                {
                    blueTeam.color = leftBarColor;
                    redTeam.color = rightBarColor;
                    break;
                }
        }
    }

    public void SetPoints(float amount)
    {
        if (amount < 0.0f || amount > 1.0f)
            return;

        if(amount > lastPoints)
        {
            state = PointGainState.leftSide;
        }
        else if(amount < lastPoints)
        {
            state = PointGainState.rightSide;
        }
        else
        {
            state = PointGainState.none;
        }

        //amount = 0.2f * amount * amount + 0.46f * amount + 0.22f; //0.22f + amount * (0.88f - 0.22f));
        amount = 0.1f + amount * 0.8f; // d + amount * (1 - 2 * d)

        //blueTransform.sizeDelta = new Vector2(blueWidth * 2.0f * amount, blueHeight);
        //redTransform.sizeDelta = new Vector2(redWidth * 2.0f * (1.0f - amount), redHeight);

        blueTransform.sizeDelta = new Vector2(distance * amount, blueTransform.sizeDelta.y);
        redTransform.sizeDelta = new Vector2(distance * (1.0f - amount), redTransform.sizeDelta.y); // new Vector2(distance * (1 - (0.22f + amount)), redHeight);

        sliderTransform.localPosition = new Vector3(blueTransform.localPosition.x + blueTransform.sizeDelta.x, sliderTransform.localPosition.y);

        lastPoints = amount;
    }
}
