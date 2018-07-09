using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EventTrigger : MonoBehaviour {

    public GameObject teamPointObject;

    public GameObject eventObject;
    public GameObject characterSpriteObject;
    public GameObject eventSpriteObject;
    public GameObject KillEventObject;
    public GameObject KillEventEntry;
    public GameObject eventTextObject;
    public TextMeshProUGUI eventText;

    public Sprite characterSpriteApe;
    public Sprite characterSpritePangulin;
    public Sprite myCharacterSprite;

    public Sprite eventSpriteFirstBlood;
    public Sprite eventSpriteGodLike;
    public Sprite eventSpriteUnstoppable;
    public Sprite eventSpritedoubleKill;
    public Sprite eventSpritetripleKill;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip firstBlood;
    public AudioClip godLike;
    public AudioClip unstoppable;
    public AudioClip doubleKill;
    public AudioClip tripleKill;

    public void Start()
    {
        audioSource.volume = 1;
        characterSpriteObject.GetComponent<Image>().sprite = myCharacterSprite;
        eventTextObject = getChildGameObject(eventObject, "Text");
        eventText = eventTextObject.GetComponent<TextMeshProUGUI>();
    }

    public enum EventType
    {
        firstBlood,
        godLike,
        unstoppable,
        doubleKill,
        tripleKill
    }

    public void TriggerEvent(EventType type, int characterID, string name)
    {
        switch (type)
        {
            case EventType.firstBlood:
                {
                    eventText.text = name + " drew FIRST BLOOD";
                    eventSpriteObject.GetComponent<Image>().sprite = eventSpriteFirstBlood;
                    SetPlayerSprite(characterID);
                    eventObject.GetComponent<Animator>().Play("EventAnim");

                    audioSource.clip = firstBlood;
                    audioSource.Play();

                    break;
                }
            case EventType.godLike:
                {
                    eventTextObject.GetComponent<TextMeshProUGUI>().text = name + " is GODLIKE";
                    eventSpriteObject.GetComponent<Image>().sprite = eventSpriteGodLike;
                    SetPlayerSprite(characterID);
                    eventObject.GetComponent<Animator>().Play("EventAnim");

                    audioSource.clip = godLike;
                    audioSource.Play();

                    break;
                }
            case EventType.unstoppable:
                {
                    eventTextObject.GetComponent<TextMeshProUGUI>().text = name + " is UNSTOPPABLE";
                    eventSpriteObject.GetComponent<Image>().sprite = eventSpriteUnstoppable;
                    SetPlayerSprite(characterID);
                    eventObject.GetComponent<Animator>().Play("EventAnim");

                    audioSource.clip = unstoppable;
                    audioSource.Play();

                    break;
                }
            case EventType.doubleKill:
                {
                    eventTextObject.GetComponent<TextMeshProUGUI>().text = name + " scored a DOUBLE KILL";
                    eventSpriteObject.GetComponent<Image>().sprite = eventSpritedoubleKill;
                    SetPlayerSprite(characterID);
                    eventObject.GetComponent<Animator>().Play("EventAnim");

                    audioSource.clip = doubleKill;
                    audioSource.Play();

                    break;
                }
            case EventType.tripleKill:
                {
                    eventTextObject.GetComponent<TextMeshProUGUI>().text = name + " scored a TRIPLE KILL";
                    eventSpriteObject.GetComponent<Image>().sprite = eventSpriteFirstBlood;
                    SetPlayerSprite(characterID);
                    eventObject.GetComponent<Animator>().Play("EventAnim");

                    audioSource.clip = tripleKill;
                    audioSource.Play();

                    break;
                }
          }
    }

    void SetPlayerSprite(int characterID)
    {
        if (characterID == 0)
        {
            characterSpriteObject.GetComponent<Image>().sprite = characterSpriteApe;
        } else
        {
            characterSpriteObject.GetComponent<Image>().sprite = characterSpritePangulin;
        }
    }

    public void TriggerKillEvent(string killPlayer, string deadPlayer)
    {
        var killEvent = Instantiate(KillEventEntry, KillEventObject.transform);
        Destroy(killEvent, 8f);
        var killEventBackground = getChildGameObject(killEvent, "Background");
        var killEventTextObject = getChildGameObject(killEventBackground, "Text");
        TextMeshProUGUI killEventText = killEventTextObject.GetComponent<TextMeshProUGUI>();
        killEventText.text =  "<color=yellow>" + killPlayer + "</color>" + " killed " + "<color=red>" + deadPlayer + "</color>" + ".";
    }

    public GameObject getChildGameObject(GameObject fromGameObject, string withName)
    {
        Transform[] ts = fromGameObject.transform.GetComponentsInChildren<Transform>();
        foreach (Transform t in ts) if (t.gameObject.name == withName) return t.gameObject;
        return null;
    }

    public void Show(PunTeams.Team team)
    {
        GameObject teamPoint = Instantiate(teamPointObject, GameObject.Find("Canvas").transform);
        if (team == PhotonNetwork.player.GetTeam())
        {
            teamPoint.GetComponent<TextMeshProUGUI>().text = "<color=#00CAA1FF>YOUR TEAM</color> SCORED A POINT!";
        }
        else
        {
            teamPoint.GetComponent<TextMeshProUGUI>().text = "<color=#F44200FF>THE ENEMY TEAM</color> SCORED A POINT!";
        }
    }

}
