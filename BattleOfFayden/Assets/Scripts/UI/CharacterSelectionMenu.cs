using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class CharacterSelectionMenu : MonoBehaviour
{
    const int mainLevel = 1;

    public PhotonView photonView;

    public Vector3 spawnPoint;
    private bool isSpawned;

    public struct CharacterSkill
    {
        public string iconName;
        public string name;
        public string description;
    }
    public struct CharacterInfo
    {
        public string name;
        public string description;
    }

#region ROTATIONMENU VARIABLES
    public enum CharacterID
    {
        Champion01,
        Champion02
    }
    public enum RotationState
    {
        StartRotation,
        Rotating,
        StopRotating
    }
    
    public RotationState    state;
    public Transform        rotationObject;
    public float            targetAngle;
    public float            scrollSpeed;

    private float           startTime;

    public CharacterID selectedChampion;
#endregion

#region DESCRIPTION VARIABLES
    public TextMeshProUGUI characterName;
    public TextMeshProUGUI characterDescription;
    #endregion

#region TEAM VARIABLES
    public GameObject playerName;
    public GameObject roomName;
    public GameObject entryPrefab;
    public GameObject blueTeamUI;
    public GameObject redTeamUI;
    public GameObject backgroundObject;

    public class TeamEntry
    {
        public int playerID;
        public GameObject listItem;
    }
    List<TeamEntry> blueEntries;
    List<TeamEntry> redEntries;

    private TeamEntry localEntry;
    
    public int readyClients;

    List<PhotonPlayer> ingameClients;
    public bool isReady;

    public string localName = "Local Player";
    public CharacterID localCharacter = CharacterID.Champion01;
    #endregion

    private void Awake()
    {
        localName = PlayerPrefs.GetString("PlayerName");
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        SelectCharacter(CharacterID.Champion01);
        state = RotationState.StopRotating;

        blueEntries = new List<TeamEntry>();
        redEntries = new List<TeamEntry>();
        ingameClients = new List<PhotonPlayer>();

        playerName.GetComponent<TMPro.TextMeshProUGUI>().text = localName;
        roomName.GetComponent<TMPro.TextMeshProUGUI>().text = PhotonNetwork.room.Name;

        NetworkManager.onPlayerDisconnected += OnPlayerClosedConnection;
    }

    void FixedUpdate()
    {
        if (isSpawned)
            return;

        if (PhotonNetwork.player.NickName != localName)
            PhotonNetwork.player.NickName = localName;
        
        switch (this.state)
        {
            case RotationState.StartRotation:
                if (Mathf.Round(this.rotationObject.rotation.eulerAngles.y) == targetAngle)
                {
                    this.state = RotationState.StopRotating;
                    return;
                }

                this.state = RotationState.Rotating;
                this.startTime = Time.time;
                break;
            case RotationState.Rotating:

                var rot = this.rotationObject.rotation.eulerAngles.y + 0.1f * scrollSpeed;
                this.rotationObject.rotation = Quaternion.Euler(0, rot, 0);

                if (Mathf.Round(this.rotationObject.rotation.eulerAngles.y) == targetAngle)
                    this.state = RotationState.StopRotating;

                break;
            case RotationState.StopRotating:
                break;
        }
    }
    
    public void OnPlayerClosedConnection(PhotonPlayer player)
    {
        RemoveEntry(player.ID);
    }

    public void OnChangeTeamPressed(int teamID)
    {
        PunTeams.Team team = (PunTeams.Team)teamID;

        if (PhotonNetwork.player.GetTeam() == team)
            return;

        PhotonNetwork.player.SetTeam(team);
        photonView.RPC("SelectTeam", PhotonTargets.AllBuffered, PhotonNetwork.player, team);

        switch (team)
        {
            case PunTeams.Team.blue:
                backgroundObject.GetComponent<Image>().sprite = Resources.Load("teams_xaak", typeof(Sprite)) as Sprite;
                break;
            case PunTeams.Team.red:
                backgroundObject.GetComponent<Image>().sprite = Resources.Load("teams_much_tahal", typeof(Sprite)) as Sprite;
                break;
            default:
                break;
        }

        if (isReady)
            OnReadyButtonClicked();
    }
    public void OnReadyButtonClicked()
    {
        if (PhotonNetwork.player.GetTeam() != PunTeams.Team.none)
        {
            isReady = !isReady;
            photonView.RPC("ClientReady", PhotonTargets.AllBuffered, PhotonNetwork.player.ID, isReady);
        }
    }

    public void SelectCharacter(CharacterID character)
    {
        CharacterInfo apeInfo;
        apeInfo.name = "SEEBAK";
        apeInfo.description = "This guy is a real sniper. Equipped with a musket and some bombs he even floors the biggest enemies in the jungle, which also is his natural habitat.\n\nIf you cross his path watch out! A bullet could already be on the way to you.";

        CharacterInfo pangolinInfo;
        pangolinInfo.name = "MUUK";
        pangolinInfo.description = "This creature is a mighty Pangolin equipped with razor-sharp claws.\n\nMost of the time a few punches are enough for him to finish off his enemies. Because of his size he is really slow which makes him a perfect tank.";

        this.selectedChampion = character;

        switch(character)
        {
            case CharacterID.Champion01:
                this.targetAngle = 310;
                this.state = RotationState.StartRotation;
                this.characterName.text = apeInfo.name;
                this.characterDescription.text = apeInfo.description;
                break;
            case CharacterID.Champion02:
                this.targetAngle = 130;
                this.state = RotationState.StartRotation;
                this.characterName.text = pangolinInfo.name;
                this.characterDescription.text = pangolinInfo.description;
                break;
        }

        this.localCharacter = character;

        if (isReady)
            OnReadyButtonClicked();
    }

    [PunRPC]
    public void SelectTeam(PhotonPlayer player, PunTeams.Team teamID)
    {

        switch (teamID)
        {
            case PunTeams.Team.blue:
            {
                foreach (var e in blueEntries.ToArray())
                    if (e.playerID == player.ID)
                        return;

                RemoveEntry(player.ID);

                var blueTransform = blueTeamUI.GetComponent<RectTransform>().transform;

                localEntry = new TeamEntry();
                localEntry.listItem = Instantiate(entryPrefab, blueTransform);
                localEntry.playerID = player.ID;

                localEntry.listItem.transform.localPosition -= new Vector3(0, 50, 0) * blueEntries.Count;

                var texts = localEntry.listItem.GetComponentsInChildren<TextMeshProUGUI>();
                foreach (var text in texts)
                {
                    if (text.name == "PlayerName")
                    {
                        text.text = player.NickName;
                    }
                }

                blueEntries.Add(localEntry);

                break;
            }
            case PunTeams.Team.red:
            {
                foreach (var e in redEntries.ToArray())
                    if (e.playerID == player.ID)
                        return;

                RemoveEntry(player.ID);

                var redTransform = redTeamUI.GetComponent<RectTransform>().transform;

                localEntry = new TeamEntry();
                localEntry.listItem = Instantiate(entryPrefab, redTransform);
                localEntry.playerID = player.ID;

                localEntry.listItem.transform.localPosition -= new Vector3(0, 50, 0) * redEntries.Count;
                    
                var texts = localEntry.listItem.GetComponentsInChildren<TextMeshProUGUI>();
                foreach (var text in texts)
                {
                    if (text.name == "PlayerName")
                    {
                        text.text = player.NickName;
                    }
                }

                redEntries.Add(localEntry);

                break;
            }
        default:
                break;
        }
    }
    [PunRPC]
    public void ClientReady(int playerID, bool state)
    {
        if (PhotonNetwork.isMasterClient)
        {
            if (state)
                readyClients++;
            else
                readyClients--;

            if (readyClients >= PhotonNetwork.room.PlayerCount)
            {
                PhotonNetwork.room.MaxPlayers = readyClients;
                photonView.RPC("EnterGame", PhotonTargets.AllBuffered);
            }
        }

        RefreshReadyState(blueEntries, playerID, state);
        RefreshReadyState(redEntries, playerID, state);
    }

    void RefreshReadyState(List<TeamEntry> entries, int playerID, bool state)
    {
        foreach (var e in entries)
        {
            if (e.playerID == playerID)
            {
                var texts = e.listItem.GetComponentsInChildren<TextMeshProUGUI>();
                foreach (var text in texts)
                {
                    if (text.name == "ReadyState")
                    {
                        if (state)
                        {
                            text.text = "O";
                            text.color = Color.green;
                        }
                        else
                        {
                            text.text = "X";
                            text.color = Color.red;
                        }
                    }
                }
            }
        }
    }

    [PunRPC]
    private void EnterGame()
    {
        if (isSpawned)
            return;

        LoadingScreenManager.LoadScene((int)SceneAlias.MainWorld);
        SceneManager.LoadSceneAsync((int)SceneAlias.OptionsMenu, LoadSceneMode.Additive);

        isSpawned = true;
    }

    private void RemoveEntry(int playerID)
    {
        var redArray = redEntries.ToArray();
        for (int i = 0; i < redArray.Length; i++)
        {
            if (redArray[i].playerID == playerID)
            {
                for (int j = i; j < redArray.Length; j++)
                    redArray[j].listItem.transform.localPosition += new Vector3(0, 50, 0);
                Destroy(redArray[i].listItem);
                redEntries.Remove(redArray[i]);
            }
        }

        var blueArray = blueEntries.ToArray();
        for (int i = 0; i < blueArray.Length; i++)
        {
            if (blueArray[i].playerID == playerID)
            {
                for (int j = i; j < blueArray.Length; j++)
                    blueArray[j].listItem.transform.localPosition += new Vector3(0, 50, 0);
                Destroy(blueArray[i].listItem);
                blueEntries.Remove(blueArray[i]);
            }
        }
    }

    private void OnLevelWasLoaded(int level)
    {
        if (level == (int)SceneAlias.MainWorld)
        {
            SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex((int)SceneAlias.MainWorld));
            photonView.RPC("OnPlayerIngame", PhotonTargets.MasterClient, PhotonNetwork.player);
        }
    }

    [PunRPC]
    void OnPlayerIngame(PhotonPlayer player)
    {
        if (PhotonNetwork.isMasterClient)
        {
            if (!ingameClients.Contains(player))
                ingameClients.Add(player);

            if (ingameClients.Count >= readyClients)
            {
                photonView.RPC("SendStartGame", PhotonTargets.AllBuffered);
            }
        }
    }

    [PunRPC]
    void SendStartGame()
    {
        if (GameManager.Instance.onStartGame != null)
            GameManager.Instance.onStartGame(localName, (int)localCharacter);
    } 
}
