using UnityEngine;

public class Bomb : MonoBehaviour
{
    public Character characterRef;

    bool isExploded = false;
    float bombRadius = 10f;
    
    PhotonView photonView;
    TimedEffect timer;

    public int damage;
    public int playerID;

    public PunTeams.Team teamID;

    [Header("Audio")]
    AudioSource audioSource;
    [SerializeField] private AudioClip bombExplosionClip;
    [SerializeField] private AudioClip bombLandingClip;
    
    void Awake ()
    {
        photonView = GetComponent<PhotonView>();
        audioSource = GetComponent<AudioSource>();
        timer = new TimedEffect(25f, 0, true);
        timer.onEffectEnd += () =>
        {
            Destroy(this);
        };
        teamID = photonView.owner.GetTeam();
    }

	void Update ()
    {

    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Enviroment")
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.useGravity = false;
            rb.isKinematic = true;

            audioSource.clip = bombLandingClip;
            audioSource.Play();

            Destroy(rb);
        }
    }

    public void MoveTowardsHitPoint(Vector3 point)
    {
        Vector3 target = point;
        Vector3 pos = transform.position;

        float dist = Vector3.Distance(pos, target) / 4.5f;

        Physics.gravity = new Vector3(0, -200, 0);
        float Vi = Mathf.Sqrt(dist * -Physics.gravity.y / (Mathf.Sin(Mathf.Deg2Rad * 45 * 2)));
        float Vy, Vz;

        Vy = Vi * Mathf.Sin(Mathf.Deg2Rad * 45);
        Vz = Vi * Mathf.Cos(Mathf.Deg2Rad * 45);

        Vector3 localVelocity = new Vector3(0f, Vy, Vz);
        Vector3 globalVelocity = transform.TransformVector(localVelocity);
        GetComponent<Rigidbody>().velocity = globalVelocity;
    }
    
    public void Explode()
    {
        PhotonNetwork.Instantiate("BombParticles", new Vector3(transform.position.x,0,transform.position.z), Quaternion.identity, 0);
        isExploded = true;

        audioSource.clip = bombExplosionClip;
        audioSource.Play();

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            if (Vector3.Distance(transform.position, player.transform.position) < bombRadius && Vector3.Distance(transform.position, player.transform.position) > -bombRadius &&
                player.GetComponent<PhotonView>().owner.GetTeam() != gameObject.GetComponent<PhotonView>().owner.GetTeam() &&
                player.GetComponent<Character>().isAlive == true)
            {
                if(player.GetComponent<Character>().properties.health - damage <= 0 && player.GetComponent<Character>().isAlive == true)
                {
                    KingOfTheHill.Instance.netView.RPC("PlayerKilledPlayer", PhotonTargets.All, playerID, player.GetComponent<Character>().playerID);
                    characterRef.killStreak++;
                    KingOfTheHill.Instance.netView.RPC("IncreaseKillStreak", PhotonTargets.All, characterRef.killStreak, characterRef.player.ID, (int)characterRef.type);
                    KingOfTheHill.Instance.netView.RPC("AddPlayerMVPStat", PhotonTargets.All, playerID, 10);
                    KingOfTheHill.Instance.netView.RPC("AddPlayerKillStat", PhotonTargets.All, playerID);
                }
                player.GetComponent<PhotonView>().RPC("LoseHealth", PhotonTargets.AllViaServer, damage);
                KingOfTheHill.Instance.netView.RPC("AddPlayerDamageStat", PhotonTargets.All, playerID, damage);
                characterRef.numSkillsHit++;
            }
        }
        GameObject[] bombs = GameObject.FindGameObjectsWithTag("Bomb");
        foreach (GameObject bomb in bombs)
        {
            if (Vector3.Distance(transform.position, bomb.transform.position) < bombRadius && Vector3.Distance(transform.position, bomb.transform.position) > -bombRadius)
            {
                Bomb bombRef = bomb.GetComponent<Bomb>();
                if (bombRef.isExploded == false)
                {
                    bombRef.Explode();
                }
            }
        }
        photonView.RPC("DestroyBomb", PhotonTargets.AllViaServer);
    }
    
    [PunRPC]
    void DestroyBomb()
    {
        Destroy(gameObject);
    }
}
