using UnityEngine;

public class Projectile : Photon.MonoBehaviour
{

    [SerializeField] private int shootSpeed;
    public Character characterRef;
    Vector3 heading;
    TimedEffect timer;
    PhotonView photonView;
    public int damage = 10;
    public PunTeams.Team teamID;
    public int playerID;

    void Start ()
    {
        photonView = GetComponent<PhotonView>();
        timer = new TimedEffect(2f, 0, true);
        timer.onEffectEnd += () =>
        {
            PhotonNetwork.Destroy(photonView);
        };
        timer.Activate();
        teamID = photonView.owner.GetTeam();
    }
	
	void Update ()
    {
        timer.Update();

        transform.LookAt(heading);
        transform.position += heading.normalized * Time.deltaTime * shootSpeed;
	}

    public void MoveTowardsHitPoint(Vector3 point)
    {
        heading = point - transform.position;
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Player" &&
            collider.gameObject.GetComponent<PhotonView>().owner.GetTeam() != teamID &&
            collider.gameObject.GetComponent<Character>().isAlive == true)
        {
            PhotonNetwork.Instantiate("Projectile_Hit", transform.position + new Vector3(0, 2.5f, 0), Quaternion.identity, 0);
            
            if (collider.gameObject.GetComponent<Character>().properties.health - damage <= 0 && collider.gameObject.GetComponent<Character>().isAlive == true)
            {
                KingOfTheHill.Instance.netView.RPC("PlayerKilledPlayer", PhotonTargets.All, playerID, collider.gameObject.GetComponent<Character>().playerID);
                characterRef.killStreak++;
                KingOfTheHill.Instance.netView.RPC("IncreaseKillStreak", PhotonTargets.All, characterRef.killStreak, characterRef.player.ID, (int)characterRef.type);
                KingOfTheHill.Instance.netView.RPC("AddPlayerMVPStat", PhotonTargets.All, playerID, 10);
                KingOfTheHill.Instance.netView.RPC("AddPlayerKillStat", PhotonTargets.All, playerID);
            }
            KingOfTheHill.Instance.netView.RPC("AddPlayerDamageStat", PhotonTargets.All, playerID, damage);
            collider.gameObject.GetComponent<PhotonView>().RPC("LoseHealth", PhotonTargets.AllViaServer, damage);
            characterRef.numSkillsHit++;
            photonView.RPC("DestroyProjectile", PhotonTargets.AllViaServer);
        }
        if (collider.gameObject.tag == "Bomb")
        {
            PhotonNetwork.Instantiate("Projectile_Hit", transform.position + new Vector3(0, 2.5f, 0), Quaternion.identity, 0);
            collider.gameObject.GetComponent<Bomb>().Explode();
            photonView.RPC("DestroyProjectile", PhotonTargets.AllViaServer);
        }
        if(collider.gameObject.tag == "Obstacle")
        {
            PhotonNetwork.Instantiate("Projectile_Hit", transform.position + new Vector3(0, 2.5f, 0), Quaternion.identity, 0);
            photonView.RPC("DestroyProjectile", PhotonTargets.AllViaServer);
        }
    }

    [PunRPC]
    void DestroyProjectile()
    {
        Destroy(gameObject);
    }
}
