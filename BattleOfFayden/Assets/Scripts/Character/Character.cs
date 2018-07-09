using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Character : Photon.MonoBehaviour
{
    [SerializeField]
    public GameObject black;

    public string playerNameString;

    public int numSkills = 0;
    public int numSkillsHit = 0;

    public enum CharacterID
    {
        Ape,
        Pangolin
    }

    public struct Properties
    {
       public int health;
       public int mana;
       public int damage;
       public int charges;

        public Properties(int health, int mana, int damage, int charges)
        {
            this.health = health;
            this.mana = mana;
            this.damage = damage;
            this.charges = charges;
        }
    };

    public Properties properties;

    public int killStreak = 0;

    [SerializeField] private string charName = "Character";
    [SerializeField] private string desc = "Character Description";
    [SerializeField] public CharacterID type;
    public int maxHealth;
    public int maxMana;
    [SerializeField] public bool isAlive = true;
    [SerializeField] private float attackHeight;
    public Animator animator;
    public PunTeams.Team teamID;
    public PhotonView photonView;
    private bool isBombSkillActive = false;
    public Ability[] abilities;
    Vector3 skillDestination;
    Movement mov;

    private bool dead = false;
    
    public bool isStunned = false;

    public PhotonPlayer player;
    public PhotonPlayer playerX;
    public string playerName;

    CapsuleCollider playerCollider;

    public bool camSnap = false;

    /////////////           Audio               //////////////
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip wonGame;
    public AudioClip loseGame;


    [Header("ApeShot")]
    public GameObject shotGo;
    bool shotPeBool;

    [Header("Ape Sound Clips")]
    [SerializeField] private AudioClip apeShotClip;
    [SerializeField] private AudioClip bombThrowClip;

    [Header("Pangolin Sound Clips")]
    [SerializeField] private AudioClip pangolinSlashClip1;
    [SerializeField] private AudioClip pangolinSlashClip2;
    [SerializeField] private AudioClip pangolinSlashClip3;
    [SerializeField] private AudioClip pangolinSlashClip4;
    [SerializeField] private AudioClip pangolinSlashClip5;
    [SerializeField] private AudioClip pangolinSlashClip6;
    [SerializeField] private AudioClip pangolinSlashClip7;

    // Der Ring an denn füßen
    GameObject aura;

    [HideInInspector] public bool bombOnCooldwon = false;

    //Pangolin Damage Variable
    int actualDamage;
    //Pangolin HitBox
    GameObject hitBox;

    [Header("Status Bars")]
    public GameObject statusBarObject;
    [SerializeField] private Image healthImage, manaImage, damageIndikator;
    [SerializeField] public Color teamColor, enemyColor;
    private float smoothHealth, smoothMana, smoothIndikator;
    private Vector3 statusBarOffset = new Vector3(0, 5, 0);
    
    [Header("Ape Ability Costs")]
    public int shootCost;
    public int bombCost;
    [Header("Ape Ability Cooldown")]
    public float shotCooldown;
    public float bombCooldown;
    [Header("Pangolin")]
    public int slashCost;
    public int earthquakeCost;
    [Header("Pangolin Ability Cooldown")]
    public float slashCooldown;
    public float earthquakeCooldown;
    [Header("Stun Duration")]
    public float stunDuration;
    [Header("Stun Effect")]
    public GameObject stunEffect;

    bool regMana = true;

    GameObject leftTeam;
    GameObject rightTeam;

    Image leftTeamImage;
    Image rightTeamImage;

    // Animation bool
    [HideInInspector]
    public bool isRunning;

    [HideInInspector]
    public bool isHealing = false;
    GameObject healEffect;

    [HideInInspector]
    public bool isSpawning = false;
    GameObject spawnEffect;

    [Header("PlayerID")]
    public int playerID;
    
    [PunRPC]
    void SetPlayer()
    {
        player = PhotonNetwork.player;
    }

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
        var agent = GetComponent<NavMeshAgent>();
        mov = GetComponent<Movement>();
        animator = GetComponent<Animator>();

        player = photonView.owner;
        KingOfTheHill.Instance.netView.RPC("AddPlayerStatTracking", PhotonTargets.All, player.ID);

        playerX = PhotonNetwork.player;
        playerName = playerX.NickName;

        audioSource = GetComponent<AudioSource>();
        playerCollider = GetComponent<CapsuleCollider>();

        teamID = photonView.owner.GetTeam();

        // Healthbar
        statusBarObject = Instantiate(statusBarObject, GameObject.Find("Canvas2").transform);
        healthImage = getChildGameObject(statusBarObject, "Health").GetComponent<Image>();
        manaImage = getChildGameObject(statusBarObject, "Mana").GetComponent<Image>();
        damageIndikator = getChildGameObject(statusBarObject, "DamageIndikator").GetComponent<Image>();

        healEffect = getChildGameObject(this.gameObject, "PE_heal").gameObject;
        healEffect.SetActive(false);

        spawnEffect = getChildGameObject(this.gameObject, "PE_spawn").gameObject;
        spawnEffect.SetActive(false);

        if (photonView.isMine)
        {
            // Team BalkenSetup
            leftTeam = GameObject.FindGameObjectWithTag("LeftTeam");
            rightTeam = GameObject.FindGameObjectWithTag("RightTeam");

            leftTeamImage = leftTeam.GetComponent<Image>();
            rightTeamImage = rightTeam.GetComponent<Image>();

            if (photonView.owner.GetTeam() == PunTeams.Team.red)
            {
                leftTeamImage.color = teamColor;
                rightTeamImage.color = enemyColor;
            }
            else if (photonView.owner.GetTeam() == PunTeams.Team.blue)
            {
                rightTeamImage.color = teamColor;
                leftTeamImage.color = enemyColor;
            }
        }

        // Healthbar Setup
        if (photonView.owner.GetTeam() == PhotonNetwork.player.GetTeam())
        {
            healthImage.color = teamColor;
        }
        else
        {
            healthImage.color = enemyColor;
        }

        if(type == CharacterID.Ape)
        {
            //Stats
            maxHealth = ApePrototype.health;
            maxMana = ApePrototype.mana;
        }
        else if(type == CharacterID.Pangolin)
        {
            //Stats
            maxHealth = PangolinPrototype.health;
            maxMana = PangolinPrototype.mana;
        }

        aura = getChildGameObject(gameObject, "Aura");

        if (playerX.GetTeam() == photonView.owner.GetTeam())
        {
            if (photonView.isMine)
            {
                //bleibe weiß
            }
            else
            {
                ParticleSystem part = aura.GetComponent<ParticleSystem>();
                var main = part.main;
                main.startColor = teamColor;
            }
        }
        else
        {
            ParticleSystem part = aura.GetComponent<ParticleSystem>();
            var main = part.main;
            main.startColor = enemyColor;
        }
        
        abilities = new Ability[2];

        switch (type)
        {
            ///////////////////////////////////////////////////////////////
            /////////////                APE                  /////////////
            ///////////////////////////////////////////////////////////////
            case CharacterID.Ape:

                ////////////             APE ABILITY 1            ////////////  
                
                shotGo.SetActive(false);

                abilities[0] = new Ability(
                    "Shotgun shot",
                    "This ability shoots a bullet to the specified target.",
                    0f,
                    shotCooldown,
                    false
                );

                int shoot = 0;

                abilities[0].onEffectStart = () =>
                {
                    numSkills++;
                    shoot = Random.Range(1, 3);

                    skillDestination = MousePosition();
                    photonView.RPC("LookAt", PhotonTargets.All, new Vector3(skillDestination.x, transform.position.y, skillDestination.z));
                    photonView.RPC("SetMovementState", PhotonTargets.All, false);

                    animator.SetInteger("shoot", shoot);

                    audioSource.clip = apeShotClip;
                    audioSource.Play();

                    photonView.RPC("LoseEnergie", PhotonTargets.AllViaServer, shootCost);

                    StartCoroutine(Shot(shoot));
                };
                abilities[0].onEffectTick = (float time) => { };
                abilities[0].onEffectEnd = () => { };
                abilities[0].onCooldownDone = () => { };

                ////////////            APE ABILITY 2            ////////////  

                abilities[1] = new Ability(
                    "Bomb",
                    "DOTO",
                    0f,
                    bombCooldown,
                    false
                );

                abilities[1].onEffectStart = () => 
                {
                    numSkills++;

                    photonView.RPC("LookAt", PhotonTargets.All, skillDestination);
                    photonView.RPC("SetMovementState", PhotonTargets.All, false);
                    animator.SetBool("throwBomb", true);

                    StartCoroutine(Bomb());

                    audioSource.clip = bombThrowClip;
                    audioSource.Play();

                    properties.charges--;

                    bombOnCooldwon = true;
                    StopCoroutine(BombOnCooldown(bombCooldown));
                    StartCoroutine(BombOnCooldown(bombCooldown));

                    photonView.RPC("LoseEnergie", PhotonTargets.AllViaServer, bombCost);
                };
                abilities[1].onEffectTick = (float time) =>  { };
                abilities[1].onEffectEnd = () => { };
                abilities[1].onCooldownDone = () => 
                {
                    isBombSkillActive = false;
                };

                InputManager.Instance.onKeyReleased += (KeyCode keyCode) =>
                {
                    if (keyCode == KeyCode.Q && properties.mana >= shootCost && photonView.isMine && isStunned == false)
                        abilities[0].Activate();

                    if (keyCode == KeyCode.W && properties.mana >= bombCost && properties.charges > 0 && photonView.isMine && isStunned == false && bombOnCooldwon == false)
                    {
                        isBombSkillActive = true;
                        skillDestination = MousePosition();
                    }
                };
                break;

            ///////////////////////////////////////////////////////////////
            /////////////              PANGOLIN               /////////////
            ///////////////////////////////////////////////////////////////
            case CharacterID.Pangolin:

                animator.SetInteger("Attack", 4);
                hitBox = getChildGameObject(gameObject, "AttackCollider");
                hitBox.GetComponent<AttackCollider>().characterRef = this;

                hitBox.SetActive(false);

                ////////////            PANGOLIN ABILITY 1            ////////////  

                abilities[0] = new Ability(
                    "A heavy dash",
                    "The pangolin dashes it's victim and deals a huge amount of damage.",
                    0f,
                    slashCooldown,
                    false
                );

                abilities[0].onEffectStart = () => 
                {
                    numSkills++;

                    Vector3 realdestination;

                    skillDestination = MousePosition();
                    realdestination = new Vector3(skillDestination.x, 0, skillDestination.z);

                    //animator.SetInteger("Attack", combo);
                    switch (properties.charges)
                    {
                        case 0:
                            actualDamage = properties.damage * 10;
                            break;
                        case 1:
                            actualDamage = (properties.damage * 15);
                            break;
                        case 2:
                            actualDamage = properties.damage * 20;
                            break;
                    }
                    hitBox.GetComponent<AttackCollider>().damage = actualDamage;
                    
                    photonView.RPC("LookAt", PhotonTargets.All, realdestination);
                    photonView.RPC("SetMovementState", PhotonTargets.All, false);

                    animator.SetInteger("Attack", properties.charges);

                    hitBox.GetComponent<AttackCollider>().playerID = playerID;
                    hitBox.GetComponent<AttackCollider>().characterRef = this;
                    hitBox.SetActive(true);
                    StartCoroutine(Slash());

                    switch(properties.charges)
                    {
                        case 0:
                            realdestination = (realdestination - transform.position).normalized;
                            GameObject slash = PhotonNetwork.Instantiate("Slash", transform.localPosition + 2 * realdestination + new Vector3(0,1,0), Quaternion.identity, 0);
                            slash.transform.LookAt(transform.localPosition + realdestination * 50);

                            break;
                        case 1:
                            realdestination = (realdestination - transform.position).normalized;
                            GameObject slashv2 = PhotonNetwork.Instantiate("Slashv2", transform.localPosition + 2 * realdestination + new Vector3(0, 1, 0), Quaternion.identity, 0);
                            slashv2.transform.LookAt(transform.localPosition + realdestination * 50);
                            
                            break;
                        case 2:
                            realdestination = (realdestination - transform.position).normalized;
                            GameObject slashv3 = PhotonNetwork.Instantiate("Slashv3", transform.localPosition + 2 * realdestination + new Vector3(0, 1, 0), Quaternion.identity, 0);
                            slashv3.transform.LookAt(transform.localPosition + realdestination * 50);

                            break;

                    }

                    int randomSoundInt;
                    randomSoundInt = Random.Range(1, 8);
                    switch(randomSoundInt)
                    {
                        case 1:
                            audioSource.clip = pangolinSlashClip1;
                            audioSource.Play();
                            break;
                        case 2:
                            audioSource.clip = pangolinSlashClip2;
                            audioSource.Play();
                            break;
                        case 3:
                            audioSource.clip = pangolinSlashClip3;
                            audioSource.Play();
                            break;
                        case 4:
                            audioSource.clip = pangolinSlashClip4;
                            audioSource.Play();
                            break;
                        case 5:
                            audioSource.clip = pangolinSlashClip5;
                            audioSource.Play();
                            break;
                        case 6:
                            audioSource.clip = pangolinSlashClip6;
                            audioSource.Play();
                            break;
                        case 7:
                            audioSource.clip = pangolinSlashClip7;
                            audioSource.Play();
                            break;

                    }

                    photonView.RPC("LoseEnergie", PhotonTargets.AllViaServer, slashCost);
                };
                abilities[0].onEffectTick = (float time) => { };
                abilities[0].onEffectEnd = () => { };
                abilities[0].onCooldownDone = () => { };

                ////////////             PANGOLIN ABILITY 2            ////////////  

                abilities[1] = new Ability(
                    "Earthquake",
                    "TODO!",
                    0f,
                    earthquakeCooldown,
                    false
                );

                abilities[1].onEffectStart = () =>
                {
                    numSkills++;

                    skillDestination = MousePosition();

                    photonView.RPC("LookAt", PhotonTargets.All, skillDestination);
                    photonView.RPC("SetMovementState", PhotonTargets.All, false);

                    animator.SetBool("stunAttack", true);
                    StartCoroutine(Stun());

                    photonView.RPC("LoseEnergie", PhotonTargets.AllViaServer, earthquakeCost);
                };
                abilities[1].onEffectTick = (float time) => { };
                abilities[1].onEffectEnd = () => { };
                abilities[1].onCooldownDone = () => { };

                InputManager.Instance.onKeyReleased += (KeyCode keyCode) =>
                {
                    if (keyCode == KeyCode.Q && properties.mana >= slashCost && photonView.isMine && isStunned == false)
                        abilities[0].Activate();

                    if (keyCode == KeyCode.W && properties.mana >= earthquakeCost && photonView.isMine && isStunned == false)
                        abilities[1].Activate();
                };
                break;

            default:
                break;
        }

    }

    private void Start()
    {
        playerID = player.ID;
    }

    ///////////////////////////////////////////////////////////////
    /////////////               UPDATE                /////////////
    ///////////////////////////////////////////////////////////////

    private void Update()
    {
        SetStatusBarPosition();
        SetHealthbar();

        myBombChange();

        if(isSpawning == true)
        {
            spawnEffect.SetActive(true);
        }
        else
        {
            spawnEffect.SetActive(false);
        }

        if (isHealing == true)
        {
            healEffect.SetActive(true);
        }
        else
        {
            healEffect.SetActive(false);
        }

        if(type == CharacterID.Ape)
        {
            if (shotPeBool == true)
            {
                shotGo.SetActive(true);
            }
            else
            {
                shotGo.SetActive(false);
            }
        }

        if (photonView.isMine)
        {

            ///////////////////    ISALIVE OR NOT    ////////////////////////

            if (properties.health <= 0)
            {
                isAlive = false;
            }
            else
            {
                isAlive = true;
            }
            if (isAlive == false)
            {
                animator.SetBool("isAlive", false);
            }

            if (dead)
                return;

            CheckIfDead();

            abilities[0].Update();
            abilities[1].Update();

            ///////////////////    CAP RESSOURCES    ////////////////////////

            if (type == CharacterID.Ape)
            {
                if (properties.health > ApePrototype.health)
                {
                    photonView.RPC("SetHealth", PhotonTargets.AllViaServer, ApePrototype.health);
                }
                if (properties.mana > ApePrototype.mana)
                {
                    photonView.RPC("SetMana", PhotonTargets.AllViaServer, ApePrototype.mana);
                }
            }
            else if (type == CharacterID.Pangolin)
            {
                if (properties.health > PangolinPrototype.health)
                {
                    photonView.RPC("SetHealth", PhotonTargets.AllViaServer, PangolinPrototype.health);
                }
                if (properties.mana > PangolinPrototype.mana)
                {
                    photonView.RPC("SetMana", PhotonTargets.AllViaServer, PangolinPrototype.mana);
                }
            }

            ///////////////////    MANA REGENARATION    ////////////////////////

            if (properties.mana < maxMana && regMana == true)
            {
                regMana = false;
                StartCoroutine(RegEnergie());
            }
            
            ///////////////////    APE: KANN BOMBE WERFEN ODER MUSS LAUFEN    ////////////////////////

            if (isBombSkillActive)
            {
                if (mov.IsCloseToPos(skillDestination, 20))
                {
                    abilities[1].Activate();
                    photonView.RPC("MoveToTarget", PhotonTargets.All, PhotonNetwork.player.ID, mov.destination);
                }
                else if(Input.GetMouseButtonDown(1))
                {
                    isBombSkillActive = false;
                    return;
                }
                else
                {
                    photonView.RPC("MoveToTarget", PhotonTargets.All, PhotonNetwork.player.ID, skillDestination);
                }
            }

            ///////////////////    PANGOLIN: COMBO UP ODER DOWN    ////////////////////////

            if (type == CharacterID.Pangolin)
            {
                if (hitBox.GetComponent<AttackCollider>().enemyHit == true)
                {
                    StopCoroutine(ComboDown());
                    StartCoroutine(ComboDown());
                }
            }
        }
    }

    ///////////////////////////////////////////////////////////////
    /////////////             FUNCTIONS               /////////////
    ///////////////////////////////////////////////////////////////


    void OnPhotonSerializeView(
        PhotonStream stream,
        PhotonMessageInfo info)
    {
        if (stream.isWriting == true)
        {
            stream.SendNext(isAlive);
            stream.SendNext(playerID);
            stream.SendNext(shotPeBool);
            stream.SendNext(killStreak);
            stream.SendNext(playerName);
            stream.SendNext(isHealing);
            stream.SendNext(isSpawning);
        }
        else
        {
            isAlive = (bool)stream.ReceiveNext();
            playerID = (int)stream.ReceiveNext();
            shotPeBool = (bool)stream.ReceiveNext();
            killStreak = (int)stream.ReceiveNext();
            playerName = (string)stream.ReceiveNext();
            isHealing = (bool)stream.ReceiveNext();
            isSpawning = (bool)stream.ReceiveNext();
        }
    }

    private void CheckIfDead()
    {
        if (isAlive == false && dead == false)
        {
            dead = true;

            InputManager.Instance.blockInput = true;

            killStreak = 0;
            StartCoroutine(WaitThenRespawn(5f));
            KingOfTheHill.Instance.netView.RPC("AddPlayerDeathStat", PhotonTargets.All, player.ID);
        }
    }
        
    private void SetStatusBarPosition()
    {
        if (KingOfTheHill.Instance.gameFinished == true)
            return;
        var statusBarRect = statusBarObject.GetComponent<RectTransform>();
        Vector2 screenPoint = Camera.main.WorldToScreenPoint(transform.position + statusBarOffset);
        statusBarRect.anchoredPosition = screenPoint - (new Vector2(Screen.width, Screen.height) / 2);
    }
    
    private void SetHealthbar()
    {
        if (type == CharacterID.Ape)
        {
            if (smoothHealth > properties.health)
            {
                smoothHealth -= Time.deltaTime * 50;
                healthImage.fillAmount = smoothHealth / (float)ApePrototype.health;
            }
            if (smoothHealth < properties.health)
            {
                smoothHealth += Time.deltaTime * 70;
                healthImage.fillAmount = smoothHealth / (float)ApePrototype.health;
            }
            if (smoothMana > properties.mana)
            {
                smoothMana -= Time.deltaTime * 50;
                manaImage.fillAmount = smoothMana / (float)ApePrototype.mana;
            }
            if (smoothMana < properties.mana)
            {
                smoothMana += Time.deltaTime * 70;
                manaImage.fillAmount = smoothMana / (float)ApePrototype.mana;
            }
            if(smoothIndikator > properties.health)
            {
                smoothIndikator -= Time.deltaTime * 20;
                damageIndikator.fillAmount = smoothIndikator / (float)ApePrototype.health;
            }
            if (smoothIndikator < properties.health)
            {
                smoothIndikator += Time.deltaTime * 70;
                damageIndikator.fillAmount = smoothIndikator / (float)ApePrototype.health;
            }
        }
        else
        {
            if (smoothHealth > properties.health)
            {
                smoothHealth -= Time.deltaTime * 50;
                healthImage.fillAmount = smoothHealth / (float)PangolinPrototype.health;
            }
            if (smoothHealth < properties.health)
            {
                smoothHealth += Time.deltaTime * 70;
                healthImage.fillAmount = smoothHealth / (float)PangolinPrototype.health;
            }
            if (smoothMana > properties.mana)
            {
                smoothMana -= Time.deltaTime * 50;
                manaImage.fillAmount = smoothMana / (float)PangolinPrototype.mana;
            }
            if (smoothMana < properties.mana)
            {
                smoothMana += Time.deltaTime * 70;
                manaImage.fillAmount = smoothMana / (float)PangolinPrototype.mana;
            }
            if (smoothIndikator > properties.health)
            {
                smoothIndikator -= Time.deltaTime * 20;
                damageIndikator.fillAmount = smoothIndikator / (float)PangolinPrototype.health;
            }
            if (smoothIndikator < properties.health)
            {
                smoothIndikator += Time.deltaTime * 70;
                damageIndikator.fillAmount = smoothIndikator / (float)PangolinPrototype.health;
            }
        }
    }

    public GameObject getChildGameObject(GameObject fromGameObject, string withName)
    {
        Transform[] ts = fromGameObject.transform.GetComponentsInChildren<Transform>();
        foreach (Transform t in ts) if (t.gameObject.name == withName) return t.gameObject;
        return null;
    }

    void ShootProjectile(Vector3 target)
    {
        if (photonView.isMine)
        {
            GameObject projectileObj = PhotonNetwork.Instantiate("pref_ape_projectile_01", transform.position + new Vector3(0, attackHeight, 0), Quaternion.identity, 0);
            projectileObj.transform.LookAt(target);
            
            Projectile projectile = projectileObj.GetComponent<Projectile>();
            Vector3 realHitPoint = new Vector3(target.x, transform.position.y + attackHeight, target.z);
            projectile.MoveTowardsHitPoint(realHitPoint);
            projectile.teamID = teamID;
            projectile.playerID = playerID;
            projectile.characterRef = this;
        }
    }
    void BombThrow(Vector3 target)
    {
        if(photonView.isMine)
        {
            float shootHeight = 1.5f;

            GameObject bombObj = PhotonNetwork.Instantiate("pref_bomb_01", transform.position + new Vector3(0, shootHeight, 0), Quaternion.identity, 0);
            bombObj.transform.LookAt(target);

            Bomb bombRef = bombObj.GetComponent<Bomb>();
            Vector3 realHitPoint = new Vector3(target.x, transform.position.y + shootHeight, target.z);
            bombRef.MoveTowardsHitPoint(realHitPoint);
            bombRef.teamID = teamID;
            bombRef.playerID = playerID;
            bombRef.characterRef = this;
        }
    }
    void Stun(Vector3 target)
    {
        if (photonView.isMine)
        {
            GameObject stunObj = PhotonNetwork.Instantiate("pref_stun_01", transform.position + new Vector3(0, 0, 0), Quaternion.identity, 0);
            stunObj.transform.LookAt(target);

            Stun stunRef = stunObj.GetComponent<Stun>();
            Vector3 realHitPoint = new Vector3(target.x, 0, target.z);
            stunRef.MoveTowardsHitPoint(realHitPoint);
            stunRef.teamID = teamID;
            stunRef.playerID = playerID;
            stunRef.characterRef = this;
        }
    }

    public Vector3 MousePosition()
    {
        RaycastHit mousePosition;

        Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out mousePosition, 100);
        return mousePosition.point;
    }

    public void myBombChange()
    {
        GameObject[] bombs = GameObject.FindGameObjectsWithTag("Bomb");

        GameObject newBomb = null;
        ParticleSystem.MainModule pS;
        foreach (GameObject bomb in bombs)
        {
            pS = bomb.GetComponentInChildren<ParticleSystem>().main;
            if (pS.startColor.color == Color.white)
            {
                newBomb = bomb;
                break;
            }
        }

        if (newBomb == null)
            return;

        PunTeams.Team b = newBomb.GetComponent<Bomb>().teamID;
        if (b == teamID)
        {
            pS.startColor = teamColor;
        }
        else
        {
            pS.startColor = enemyColor;
        }
    }

    ///////////////////////////////////////////////////////////////
    /////////////               RPC's                 /////////////
    ///////////////////////////////////////////////////////////////

    [PunRPC]
    public void SetPlayerName(string playerName)
    {
        var playerNameText = getChildGameObject(statusBarObject, "PlayerName").GetComponent<TextMeshProUGUI>();
        playerNameText.text = playerName;
    }

    [PunRPC]
    public void BombIndikatorColor()
    {
        myBombChange();
    }
    
    [PunRPC]
    public void SetProperties(int playerID, int health, int mana, int damage, int charges)
    {
        if (playerID == photonView.owner.ID)
            this.properties = new Properties(health, mana, damage, charges);
    }

    [PunRPC]
    void LoseHealth(int value)
    {
        properties.health -= value;
    }
    
    [PunRPC]
    void LoseEnergie(int value)
    {
        properties.mana -= value;
    }

    [PunRPC]
    void SetHealth(int value)
    {
        properties.health = value;
    }

    [PunRPC]
    void SetMana(int value)
    {
        properties.mana = value;
    }

    [PunRPC]
    void Stun(bool stunned)
    {
        Instantiate(stunEffect, new Vector3(transform.position.x, 2, transform.position.z), Quaternion.identity);
        isStunned = stunned;
        photonView.RPC("SetMovementState", PhotonTargets.All, false);
        StopCoroutine(StunDuration(stunDuration));
        StartCoroutine(StunDuration(stunDuration));
    }

    ///////////////////////////////////////////////////////////////
    /////////////            IEnumarator              /////////////
    ///////////////////////////////////////////////////////////////

    public IEnumerator WaitThenRespawn(float time)
    {
        statusBarObject.SetActive(false);
        KingOfTheHill.Instance.CheckIfGameIsOver();

        InputManager.Instance.blockInput = true;

        var trafo = KingOfTheHill.Instance.GetSpawnTransform();

        if(mov.agent.enabled == true)
            mov.agent.destination = trafo.position;

        photonView.RPC("SetMovementState", PhotonTargets.All, false);
        yield return new WaitForSeconds(time);
        KingOfTheHill.Instance.Respawn(this.gameObject);
        photonView.RPC("Teleport", PhotonTargets.All, trafo.position);
        yield return new WaitForSeconds(1f);
        animator.SetBool("isAlive", true);

        animator.SetBool("isSelected", true);

        camSnap = true;
        yield return new WaitForSeconds(.5f);
        photonView.RPC("SetMovementState", PhotonTargets.All, true);
        camSnap = false;

        animator.SetBool("isSelected", false);

        isSpawning = true;
        yield return new WaitForSeconds(2f);
        isSpawning = false;

        InputManager.Instance.blockInput = false;
        statusBarObject.SetActive(true);
        dead = false;
    }

    IEnumerator Bomb()
    {
        yield return new WaitForSeconds(0.8f);
        BombThrow(skillDestination);
        yield return new WaitForSeconds(0.3f);
        animator.SetBool("throwBomb", false);
        photonView.RPC("SetMovementState", PhotonTargets.All, true);
        photonView.RPC("MoveToTarget", PhotonTargets.All, PhotonNetwork.player.ID, mov.destination);
        isBombSkillActive = false;
    }

    IEnumerator Shot(int shoot)
    {
        yield return new WaitForSeconds(0.1f);
        ShootProjectile(skillDestination);
        yield return new WaitForSeconds(0.1f);
        shotPeBool = true;
        shoot = 0;

        animator.SetInteger("shoot", shoot);
        
        photonView.RPC("SetMovementState", PhotonTargets.All, true);
        photonView.RPC("MoveToTarget", PhotonTargets.All, PhotonNetwork.player.ID, mov.destination);
        yield return new WaitForSeconds(1.5f);
        shotPeBool = false;
    }

    IEnumerator Slash()
    {
        yield return new WaitForSeconds(0.2f);
        hitBox.SetActive(false);
        yield return new WaitForSeconds(0.3f);
        animator.SetInteger("Attack", 4);
        photonView.RPC("SetMovementState", PhotonTargets.All, true);
        photonView.RPC("MoveToTarget", PhotonTargets.All, PhotonNetwork.player.ID, mov.destination);
    }

    public IEnumerator Stun()
    {
        yield return new WaitForSeconds(0.4f);
        Stun(skillDestination);
        animator.SetBool("stunAttack", false);

        photonView.RPC("SetMovementState", PhotonTargets.All, true);
        photonView.RPC("MoveToTarget", PhotonTargets.All, PhotonNetwork.player.ID, mov.destination);
    }

    IEnumerator BombOnCooldown(float cooldown)
    {
        yield return new WaitForSeconds(cooldown);
        bombOnCooldwon = false;
    }
    
    IEnumerator RegEnergie()
    {
        photonView.RPC("LoseEnergie", PhotonTargets.All, -15);
        yield return new WaitForSeconds(2f);
        regMana = true;
    }

    public IEnumerator StunDuration(float duration)
    {
        yield return new WaitForSeconds(duration);
        isStunned = false;
        photonView.RPC("SetMovementState", PhotonTargets.All, true);
    }
    
    IEnumerator ComboDown()
    {
        yield return new WaitForSeconds(5f);
        properties.charges = 0;
    }



    public IEnumerator AnimationLenght()
    {
        isStunned = true;
        GameObject BLACK = Instantiate(black, GameObject.Find("Canvas").transform);
        Destroy(BLACK, BLACK.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length-1f);
        yield return new WaitForSeconds(5f);
        Camera.main.GetComponent<Animator>().SetInteger("team", 0);
        Camera.main.GetComponent<Animator>().enabled = false;

        var res = GetComponent<RessourceUI>();
        StartCoroutine(res.UI());

        yield return new WaitForSeconds(1f);
        isStunned = false;
    }
}
