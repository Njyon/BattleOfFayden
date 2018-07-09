using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class KingOfTheHill : Singleton<KingOfTheHill>
{
    public GameObject teamPointObject;

    public GameObject charGlobal;

    public StatMenuUI.StatUIElement winnerFirst;
    public StatMenuUI.StatUIElement winnerSecond;
    public StatMenuUI.StatUIElement loserFirst;
    public StatMenuUI.StatUIElement loserSecond;

    public GameObject[] middleStones;
    private Renderer[] renderer;
    private Material[] middleStoneMaterials;
    private float ticktime = 10;
    private EventTrigger eventTrigger;

    SphereCollider collider;

    private GameModeUI gameModeUI;
    public PhotonView netView;
    Dictionary<PunTeams.Team, List<Transform>> teamSpawns;
    Dictionary<int, GameObject> characters;
    private float mouseScrollSpeed = 1f;
    [SerializeField] private PunTeams.Team currentCaptor = PunTeams.Team.none;
    [SerializeField] private double leftTeamCapture = 0.5f;
    [SerializeField] private float updateTime = 1.5f;
    [SerializeField] private float pointsPerTick = 1.0f;

    float lastUpdate;
    float currentLerp;

    [SerializeField] int redTeamPlayers;
    [SerializeField] int blueTeamPlayers;

    int redPoints = 0;
    int bluePoints = 0;
    int currentRound = 0;

    [Header("MiddleStoneColors")]
    [SerializeField] private Color colorNormal;
    [SerializeField] private Color colorBlueTeam;
    [SerializeField] private Color colorRedTeam;

    [Header("Point Color")]
    [SerializeField] private Color redPointColor;
    [SerializeField] private Color bluePointColor;

    public GameObject player;
    PunTeams.Team winnerTeam;
    // Intro
    Animator animator;

    [SerializeField] public Dictionary<int, PlayerStats> playerStats;

    GameObject firstPointLeft;
    GameObject secondPointLeft;
    GameObject firstPointRight;
    GameObject secondPointRight;

    Image firstPointLeftImage;
    Image secondPointLeftImage;
    Image firstPointRightImage;
    Image secondPointRightImage;

    // Particle " MiddleStone
    GameObject blueTeamPartilce;
    GameObject bluePulseGo;

    GameObject redTeamParticle;
    GameObject redPulseGo;

    ParticleSystem blueTeamCollecting;
    ParticleSystem bluepulse;

    ParticleSystem redTeamCollecting;
    ParticleSystem redpulse;

    public bool gameFinished = false;
    bool firstBloodDone = false;

    GameObject timerReset;

    GameObject glow;
    Image glowImage;

    Color greenColor, redColor;

    void OnPhotonSerializeView(
       PhotonStream stream,
       PhotonMessageInfo info)
    {
        if (stream.isWriting == true)
        {
            SerializeStatMenu(stream, winnerFirst);
            SerializeStatMenu(stream, winnerSecond);
            SerializeStatMenu(stream, loserFirst);
            SerializeStatMenu(stream, loserSecond);
        }
        else
        {
            DeserializeStatMenu(stream, winnerFirst);
            DeserializeStatMenu(stream, winnerSecond);
            DeserializeStatMenu(stream, loserFirst);
            DeserializeStatMenu(stream, loserSecond);
        }
    }

    void SerializeStatMenu(PhotonStream stream, StatMenuUI.StatUIElement ui)
    {
        stream.SendNext(ui.playerName);
        stream.SendNext(ui.spriteName);
        stream.SendNext(ui.stats.capturePoints);
        stream.SendNext(ui.stats.kills);
        stream.SendNext(ui.stats.deaths);
        stream.SendNext(ui.stats.mvp);
        stream.SendNext(ui.stats.precision);
        stream.SendNext(ui.stats.damage);
    }

    void DeserializeStatMenu(PhotonStream stream, StatMenuUI.StatUIElement ui)
    {
        ui.playerName = (string)stream.ReceiveNext();
        ui.spriteName = (string)stream.ReceiveNext();
        ui.stats.capturePoints = (int)stream.ReceiveNext();
        ui.stats.kills = (int)stream.ReceiveNext();
        ui.stats.deaths = (int)stream.ReceiveNext();
        ui.stats.mvp = (int)stream.ReceiveNext();
        ui.stats.precision = (int)stream.ReceiveNext();
        ui.stats.damage = (int)stream.ReceiveNext();
    }

    private void Awake()
    {
        SceneManager.LoadSceneAsync((int)SceneAlias.OptionsMenu, LoadSceneMode.Additive);

        winnerFirst = new StatMenuUI.StatUIElement();
        winnerSecond = new StatMenuUI.StatUIElement();
        loserFirst = new StatMenuUI.StatUIElement();
        loserSecond = new StatMenuUI.StatUIElement();

        playerStats = new Dictionary<int, PlayerStats>();
        middleStones = GameObject.FindGameObjectsWithTag("MiddleStone");
        renderer = new Renderer[middleStones.Length];
        middleStoneMaterials = new Material[middleStones.Length];
        
        colorNormal = Color.black;
        colorRedTeam = new Color(0.5514706f, 0.07986816f, 0f);
        colorBlueTeam = new Color(0f, 0.3235294f, 0.2833672f);

        greenColor = new Color(124, 255, 206, 255);
        redColor = new Color(255, 108, 41, 255);
        
        collider = gameObject.AddComponent<SphereCollider>();
        collider.radius = 11;
        collider.isTrigger = true;

        gameObject.layer = 2;

        netView = gameObject.AddComponent<PhotonView>();
        netView.viewID = 2;
        netView.synchronization = ViewSynchronization.UnreliableOnChange;

        if (PhotonNetwork.isMasterClient)
            netView.TransferOwnership(PhotonNetwork.player.ID);
        
        gameModeUI = FindObjectOfType<GameModeUI>().GetComponent<GameModeUI>();
        teamSpawns = new Dictionary<PunTeams.Team, List<Transform>>();
        GameManager.Instance.onStartGame += OnStartGame;
        InputManager.Instance.onKeyPressed += OnKeyPressed;
        InputManager.Instance.onMouseMoved += OnMouseMoved;
        
        currentLerp = (float)leftTeamCapture;
    }
    
    private void OnLevelWasLoaded(int level)
    {
        if(level == (int)SceneAlias.MainWorld)
        {
            // 1 Point Left
            firstPointLeft = GameObject.Find("FirstPointLeft");
            firstPointLeftImage = firstPointLeft.GetComponent<Image>();
            // 2 Point Left
            secondPointLeft = GameObject.Find("SecondPointLeft");
            secondPointLeftImage = secondPointLeft.GetComponent<Image>();

            // 1 Point Right
            firstPointRight = GameObject.Find("FirstPointRight");
            firstPointRightImage = firstPointRight.GetComponent<Image>();
            // 2 Point Right
            secondPointRight = GameObject.Find("SecondPointRight");
            secondPointRightImage = secondPointRight.GetComponent<Image>();

            eventTrigger = GameObject.Find("EventTrigger").GetComponent<EventTrigger>();

            // MiddleStone Particle
            blueTeamPartilce = GameObject.Find("blue_Collecting");
            blueTeamCollecting = blueTeamPartilce.GetComponent<ParticleSystem>();
            bluePulseGo = GameObject.Find("bluepulse");
            bluepulse = bluePulseGo.GetComponent<ParticleSystem>();

            redTeamParticle = GameObject.Find("red_Collecting");
            redTeamCollecting = redTeamParticle.GetComponent<ParticleSystem>();
            redPulseGo = GameObject.Find("redpulse");
            redpulse = redPulseGo.GetComponent<ParticleSystem>();

            glow = GameObject.Find("SliderGlow");
            glowImage = glow.GetComponent<Image>();
        }
    }

    public void SetWinnerUIValues()
    {
        bool firstWinnerFilled = false;
        bool firstLoserFilled = false;
        var obj = FindObjectOfType<StatMenuUI>() as StatMenuUI;

        foreach (KeyValuePair<int,PlayerStats> stats in playerStats) {
            PhotonPlayer player = FindPlayerByID(stats.Key);

            GameObject[] chars = GameObject.FindGameObjectsWithTag("Player");
            string spriteName = "";
            for (int i = 0; i < chars.Length; i++)
            {
                if (chars[i].GetComponent<Character>().player.ID == player.ID)
                {
                    if (chars[i].GetComponent<Character>().type == Character.CharacterID.Ape)
                    {
                        spriteName = "ape";
                    } else
                    {
                        spriteName = "pangulin";
                    }
                }
            }

            if (player.GetTeam() == winnerTeam)
            {
                if (firstWinnerFilled == false)
                {
                    firstWinnerFilled = true;
                    winnerFirst.playerName = player.NickName;
                    winnerFirst.spriteName = spriteName;
                    winnerFirst.stats = playerStats[stats.Key];
                }
                else
                {
                    winnerSecond.playerName = player.NickName;
                    winnerSecond.spriteName = spriteName;
                    winnerSecond.stats = playerStats[stats.Key];
                }
            }
            else
            {
                if (firstLoserFilled == false)
                {
                    firstLoserFilled = true;
                    loserFirst.playerName = player.NickName;
                    loserFirst.spriteName = spriteName;
                    loserFirst.stats = playerStats[stats.Key];
                }
                else
                {
                    loserSecond.playerName = player.NickName;
                    loserSecond.spriteName = spriteName;
                    loserSecond.stats = playerStats[stats.Key];
                }
            }
        }

        StatMenuUI menu = GameObject.Find("BackgroundPanel").GetComponent<StatMenuUI>();
        menu.FillWinnerUI(true);
        menu.FillWinnerUI(false);
    }

    private void Start()
    {
        if (netView.ObservedComponents == null)
            netView.ObservedComponents = new List<Component>();

        netView.ObservedComponents.Add(Instance.GetComponent<KingOfTheHill>());

        for (int i = 0; i < middleStones.Length; i++)
        {
            renderer[i] = middleStones[i].GetComponent<Renderer>();
            middleStoneMaterials[i] = renderer[i].material;
            middleStoneMaterials[i].SetColor("_EmissionColor", colorNormal);
        }
    }

    public Transform GetSpawnTransform()
    {
        var spawnList = teamSpawns[PhotonNetwork.player.GetTeam()];
        int spawnIndex = UnityEngine.Random.Range(0, spawnList.Count);
        return spawnList[spawnIndex];
    }
    public void AddSpawn(PunTeams.Team teamID, Transform trafo)
    {
        if (!teamSpawns.ContainsKey(teamID))
            teamSpawns[teamID] = new List<Transform>();

        teamSpawns[teamID].Add(trafo);
    }

    //[PunRPC]
    //void GetPosition(Vector3 position)
    //{
    //    charGlobal.transform.position = position;
    //}

    public void Respawn(GameObject character)
    {
        charGlobal = character;

        //netView.RPC("GetPosition", PhotonTargets.All, trafo.position);
        
        PhotonView pv = character.GetComponent<PhotonView>();

        if (character.GetComponent<Character>().type == 0)
        {
            pv.RPC(
                "SetProperties",
                PhotonTargets.All,
                PhotonNetwork.player.ID,
                ApePrototype.health,
                ApePrototype.mana,
                ApePrototype.damage,
                ApePrototype.charges
            );
        } else
        {
            pv.RPC(
                "SetProperties",
                PhotonTargets.All,
                PhotonNetwork.player.ID,
                PangolinPrototype.health,
                PangolinPrototype.mana,
                PangolinPrototype.damage,
                PangolinPrototype.charges
            );
        }
    }

    public void OnStartGame(string playerName, int characterID)
    {
        GameObject.Find("WaitingText").SetActive(false);

        Transform trafo = GetSpawnTransform();

        string prefabName = "pref_ape_01";

        if (characterID != 0)
            prefabName = "pref_pangolin_01";
        
        player = PhotonNetwork.Instantiate(
            prefabName,
            trafo.position,
            trafo.rotation,
            0
        );
        
        player.GetComponent<PhotonView>().RPC("SetPlayerName", PhotonTargets.All, playerName);

        player.GetComponent<PhotonView>().RPC(
             "SetProperties",
             PhotonTargets.All,
             PhotonNetwork.player.ID,
             ApePrototype.health,
             ApePrototype.mana,
             ApePrototype.damage,
             ApePrototype.charges
         );
        
        if (characterID != 0)
            player.GetComponent<PhotonView>().RPC(
             "SetProperties",
             PhotonTargets.All,
             PhotonNetwork.player.ID,
             PangolinPrototype.health,
             PangolinPrototype.mana,
             PangolinPrototype.damage,
             PangolinPrototype.charges
        );

        redPointColor = player.GetComponent<Character>().enemyColor;
        bluePointColor = player.GetComponent<Character>().teamColor;

        animator = Camera.main.GetComponent<Animator>();
        
        if (Camera.main.isActiveAndEnabled == true && player.GetComponent<PhotonView>().isMine && player != null)
        {
            if (player.GetComponent<PhotonView>().owner.GetTeam() == PunTeams.Team.red)
            {
                StartCoroutine(player.GetComponent<Character>().AnimationLenght());
                animator.SetInteger("team", 1);
            }
            else if (player.GetComponent<PhotonView>().owner.GetTeam() == PunTeams.Team.blue)
            {
                StartCoroutine(player.GetComponent<Character>().AnimationLenght());
                animator.SetInteger("team", 2);
            }
        }

        netView.RPC("AddPlayerStatTracking", PhotonTargets.MasterClient, PhotonNetwork.player.ID);
    }

    void OnKeyPressed(KeyCode code)
    {
        if (code == KeyCode.Space)
        {
            Transform trafo = GameObject.FindGameObjectWithTag("Player").transform;
            Camera.main.gameObject.transform.position = new Vector3(trafo.position.x, Camera.main.transform.position.y, trafo.position.z - 30);
        }
    }

    void OnMouseMoved(Vector2 mousePos)
    {
        int screenHeight = Screen.height;
        int screenWidth = Screen.width;
        
        if (mousePos.x < 100)
        {
            if (Camera.main.transform.position.x > -60.0f)
                Camera.main.gameObject.transform.position += Vector3.left * mouseScrollSpeed;
        }
        else if (mousePos.x > screenWidth - 100)
        {
            if (Camera.main.transform.position.x < 60.0f)
                Camera.main.gameObject.transform.position += Vector3.right * mouseScrollSpeed;
        }

        if (mousePos.y < 100)
        {
            if (Camera.main.transform.position.z > -70.0f)
                Camera.main.gameObject.transform.position += Vector3.back * mouseScrollSpeed;
        }
        else if (mousePos.y > screenHeight - 100)
        {
            if (Camera.main.transform.position.z < 10.0f)
                Camera.main.gameObject.transform.position += Vector3.forward * mouseScrollSpeed;
        }
    }

    private void FixedUpdate()
    {
        //    Debug.Log("Index " + playerStats.Keys);
        //    if (PhotonNetwork.isMasterClient && playerStats[1] != null)
        //        Debug.Log(playerStats[1].damage + " , " + playerStats[1].kills + " , " + playerStats[1].deaths);
        if(player != null)
        {
            if (player.GetComponent<Character>().camSnap == true)
            {
                OnKeyPressed(KeyCode.Space);
            }
        }
        if ((Time.time - lastUpdate) < updateTime)
            return;
         
        if (redTeamPlayers > blueTeamPlayers)
            currentCaptor = PunTeams.Team.red;
        else if (redTeamPlayers < blueTeamPlayers)
            currentCaptor = PunTeams.Team.blue;
        else
            currentCaptor = PunTeams.Team.none;

        if (PhotonNetwork.isMasterClient)
            AddTeamPoints(pointsPerTick, currentCaptor);

        if (leftTeamCapture >= 0.99)
        {
            leftTeamCapture = 1.0f;
            if (PhotonNetwork.isMasterClient)
            {
                netView.RPC("StartNextRound", PhotonTargets.All, PunTeams.Team.red);
            }
        }
        else if (leftTeamCapture <= 0.01)
        {
            leftTeamCapture = 0.0f;
            if (PhotonNetwork.isMasterClient)
            {
                netView.RPC("StartNextRound", PhotonTargets.All, PunTeams.Team.blue);
            }
        }

        if (PhotonNetwork.isMasterClient)
            netView.RPC("SetPercent", PhotonTargets.Others, leftTeamCapture);
        
        if (player != null)
        {
            if (player.GetPhotonView().owner.GetTeam() == PunTeams.Team.red)
            {
                if (redPoints == 1 && firstPointLeftImage.color != bluePointColor)
                {
                    firstPointLeftImage.color = bluePointColor;
                }
                else if (redPoints == 2 && secondPointLeftImage.color != bluePointColor)
                {
                    secondPointLeftImage.color = bluePointColor;
                }
                if (bluePoints == 1 && firstPointRightImage.color != redPointColor)
                {
                    firstPointRightImage.color = redPointColor;
                }
                if (bluePoints == 2 && secondPointRightImage.color != redPointColor)
                {
                    secondPointRightImage.color = redPointColor;
                }
            }
            else if (player.GetPhotonView().owner.GetTeam() == PunTeams.Team.blue)
            {
                if (redPoints == 1 && firstPointLeftImage.color != redPointColor)
                {
                    firstPointLeftImage.color = redPointColor;
                }
                else if (redPoints == 2 && secondPointLeftImage.color != redPointColor)
                {
                    secondPointLeftImage.color = redPointColor;
                }
                if (bluePoints == 1 && firstPointRightImage.color != bluePointColor)
                {
                    firstPointRightImage.color = bluePointColor;
                }
                if (bluePoints == 2 && secondPointRightImage.color != bluePointColor)
                {
                    secondPointRightImage.color = bluePointColor;
                }
            }
        }

        lastUpdate = Time.time;
    }

    public void CheckIfGameIsOver()
    {
        if (gameFinished)
            return;
        
        if (redPoints >= 2 || bluePoints >= 2)
        {
            var fast = player.GetComponent<Character>();
            if (redPoints >= 2)
            {
                winnerTeam = PunTeams.Team.red;

                if(player.GetPhotonView().owner.GetTeam() == PunTeams.Team.red)
                {
                    fast.audioSource.clip = fast.wonGame;
                    fast.audioSource.Play();
                }
                else
                {
                    fast.audioSource.clip = fast.loseGame;
                    fast.audioSource.Play();
                }
            } else
            {
                winnerTeam = PunTeams.Team.blue;

                if (player.GetPhotonView().owner.GetTeam() == PunTeams.Team.red)
                {
                    fast.audioSource.clip = fast.loseGame;
                    fast.audioSource.Play();
                }
                else
                {
                    fast.audioSource.clip = fast.wonGame;
                    fast.audioSource.Play();
                }
            }
            gameFinished = true;

            SceneManager.LoadScene((int)SceneAlias.StatScreen, LoadSceneMode.Additive);

            StopAllCoroutines();
            
            Camera.main.enabled = false;
            InputManager.Instance.blockInput = true;
        }
    }

    [PunRPC]
    public void StartNextRound(PunTeams.Team winnerTeam)
    {
        gameModeUI.timerf = 165f;
        leftTeamCapture = .5f;
        if (PunTeams.Team.red == winnerTeam)
        {
            redPoints++;
            StartCoroutine(player.GetComponent<Character>().WaitThenRespawn(2f));
            eventTrigger.Show(winnerTeam);
        }
        else
        {
            bluePoints++;
            StartCoroutine(player.GetComponent<Character>().WaitThenRespawn(2f));
            eventTrigger.Show(winnerTeam);
        }
        
        GameObject[] bombs = GameObject.FindGameObjectsWithTag("Bomb");
        foreach (GameObject bomb in bombs)
        {
            PhotonNetwork.Destroy(bomb);
        }
    }

    [PunRPC] public void PlayerKilledPlayer(int killerPlayerID, int deadPlayerID)
    {

        string killerPlayerName = FindPlayerByID(killerPlayerID).NickName;
        string deadPlayerName = FindPlayerByID(deadPlayerID).NickName;

        eventTrigger.TriggerKillEvent(killerPlayerName, deadPlayerName);
        if(firstBloodDone == false)
        {
            firstBloodDone = true;
            eventTrigger.TriggerEvent(EventTrigger.EventType.firstBlood, 0, killerPlayerName);
        }
    }

    [PunRPC]
    void IncreaseKillStreak(int killStreak, int playerID, int characterID)
    {
        string playerName = FindPlayerByID(playerID).NickName;
        if (killStreak == 2)
        {
            eventTrigger.TriggerEvent(EventTrigger.EventType.doubleKill, characterID, playerName);
        }
        else if (killStreak == 3)
        {
            eventTrigger.TriggerEvent(EventTrigger.EventType.tripleKill, characterID, playerName);
        }
        else if (killStreak == 5)
        {
            eventTrigger.TriggerEvent(EventTrigger.EventType.unstoppable, characterID, playerName);
        }
        else if (killStreak == 8)
        {
            eventTrigger.TriggerEvent(EventTrigger.EventType.godLike, characterID, playerName);
        }

    }

    PhotonPlayer FindPlayerByID(int ID)
    {
        foreach (PhotonPlayer player in PhotonNetwork.playerList)
        {
            if (player.ID == ID)
            {
                return player;
            }
        }
        return null;
    }

    [PunRPC]
    public void AddPlayerStatTracking(int playerID) //
    {
            if(!playerStats.ContainsKey(playerID))
                playerStats.Add(playerID, new PlayerStats());
    }
    [PunRPC] public void AddPlayerKillStat(int playerID)
    {
            playerStats[playerID].kills += 1;
            Debug.Log("Kill: " + playerStats[playerID].kills);
    }
    [PunRPC] public void AddPlayerDeathStat(int playerID) //
    {
            playerStats[playerID].deaths += 1;
    }
    [PunRPC] public void AddPlayerCapturePoints(int playerID, int points)
    {
            playerStats[playerID].capturePoints += points;
    }
    [PunRPC] public void SetPlayerPrecisionStat(int playerID, int precision)
    {
            playerStats[playerID].precision = precision;
    }
    [PunRPC] public void AddPlayerDamageStat(int playerID, int damage)
    {
            playerStats[playerID].damage += damage;
    }
    [PunRPC] public void AddPlayerMVPStat(int playerID, int mvp)
    {
            playerStats[playerID].mvp += mvp;
    }
    [PunRPC] public void ClearPlayerStats(int playerID)
    {
            var stats = playerStats[playerID];
            stats.capturePoints = 0;
            stats.damage = 0;
            stats.deaths = 0;
            stats.kills = 0;
            stats.mvp = 0;
            stats.precision = 0;
    }

    private void Update()
    {
        if (currentLerp != leftTeamCapture)
        {
            currentLerp = Mathf.Lerp(currentLerp, (float)leftTeamCapture, Time.deltaTime);
            gameModeUI.SetPoints(currentLerp);
        }
        if (player != null)
        {
            var photonView = player.GetComponent<PhotonView>();
            if (photonView == null)
                return;

            if (photonView.owner.GetTeam() != PunTeams.Team.none)
            {
                LerpMaterialColors();
                ActivateParticles();
            }
        }
    }

    void ActivateParticles()
    {
        GameModeUI ui = FindObjectOfType<GameModeUI>() as GameModeUI;
        if (currentCaptor == PunTeams.Team.none)
        {
            glowImage.color = Color.Lerp(glowImage.color, Color.clear, Time.deltaTime * 10);

            ui.state = GameModeUI.PointGainState.none;
            var blue = blueTeamCollecting.emission;
            blue.enabled = false;

            var red = redTeamCollecting.emission;
            red.enabled = false;

            var bPulse = bluepulse.emission;
            bPulse.enabled = false;

            var rPulse = redpulse.emission;
            rPulse.enabled = false;
        }
        else if(currentCaptor == player.GetPhotonView().owner.GetTeam())
        {
            glowImage.color = Color.Lerp(glowImage.color, greenColor, Time.deltaTime * 10);

            var blue = blueTeamCollecting.emission;
            blue.enabled = true;

            var red = redTeamCollecting.emission;
            red.enabled = false;

            var bPulse = bluepulse.emission;
            bPulse.enabled = true;

            var rPulse = redpulse.emission;
            rPulse.enabled = false;
        }
        else
        {
            glowImage.color = Color.Lerp(glowImage.color, redColor, Time.deltaTime * 10);

            var blue = blueTeamCollecting.emission;
            blue.enabled = false;

            var red = redTeamCollecting.emission;
            red.enabled = true;

            var bPulse = bluepulse.emission;
            bPulse.enabled = false;

            var rPulse = redpulse.emission;
            rPulse.enabled = true;
        }

        if(currentCaptor == PunTeams.Team.red)
        {
            ui.state = GameModeUI.PointGainState.rightSide;
        }
        else if(currentCaptor == PunTeams.Team.blue)
        {
            ui.state = GameModeUI.PointGainState.leftSide;
        }
    }

    private void LerpMaterialColors()
    {
        foreach (Material mat in middleStoneMaterials)
        {
            if (currentCaptor == PunTeams.Team.none)
            {
                mat.SetColor("_EmissionColor", Color.Lerp(mat.GetColor("_EmissionColor"), colorNormal, Time.deltaTime * ticktime));
            }
            else if (currentCaptor == player.GetPhotonView().owner.GetTeam())
            {
                mat.SetColor("_EmissionColor", Color.Lerp(mat.GetColor("_EmissionColor"), colorBlueTeam, Time.deltaTime * ticktime));
            }
            else
            {
                mat.SetColor("_EmissionColor", Color.Lerp(mat.GetColor("_EmissionColor"), colorRedTeam, Time.deltaTime * ticktime));
            }
        }
    }

    public void AddTeamPoints(float points, PunTeams.Team team)
    {
        double value = points / 100;

        if (team == PunTeams.Team.red &&
            leftTeamCapture + value <= 1.0f)
            leftTeamCapture += value * redTeamPlayers;
        else if (team == PunTeams.Team.blue &&
            leftTeamCapture - value >= 0.0f)
            leftTeamCapture -= value * blueTeamPlayers;
    }
    
    [PunRPC]
    void SetPercent(double percent)
    {
        leftTeamCapture = percent;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.4f, 0.6f, 0.3f, 0.5f);
        //Gizmos.DrawCube(transform.position, new Vector3(22, 22, 22));
        Gizmos.DrawSphere(transform.position, 11);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other == null)
            return ;

        var team = other.GetComponent<PhotonView>().owner.GetTeam();

        if (other.gameObject.tag != "Player")
            return;

        switch(team)
        {
            case PunTeams.Team.blue:
                blueTeamPlayers++;
                break;
            case PunTeams.Team.red:
                redTeamPlayers++;
                break;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag != "Player")
            return;

        int playerID = other.gameObject.GetComponent<Character>().player.ID;
        netView.RPC("AddPlayerCapturePoints", PhotonTargets.All, playerID, 1);
    }

    private void OnTriggerExit(Collider other)
    {
        var team = other.GetComponent<PhotonView>().owner.GetTeam();

        if (other.gameObject.tag != "Player")
            return;

        switch (team)
        {
            case PunTeams.Team.blue:
                blueTeamPlayers--;
                break;
            case PunTeams.Team.red:
                redTeamPlayers--;
                break;
        }
    }
}
