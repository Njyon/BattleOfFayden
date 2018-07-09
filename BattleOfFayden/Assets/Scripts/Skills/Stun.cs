using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Stun : MonoBehaviour
{
    public Character characterRef;

    public float shootSpeed;
    public PunTeams.Team teamID;
    Vector3 heading;
    TimedEffect timer;
    public Collider colider;
    public PhotonView photonView;

    public int damage;

    public int playerID;

    void Start ()
    {
        colider = GetComponent<Collider>();
        StartCoroutine(TurnOffColider());
        photonView = GetComponent<PhotonView>();
        teamID = photonView.owner.GetTeam();
    }
	
	void Update ()
    {
        transform.position += heading.normalized * Time.deltaTime * shootSpeed;
    }

    public void MoveTowardsHitPoint(Vector3 point)
    {
        heading = point - transform.position;
    }


    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Player" &&
            collision.gameObject.GetComponent<PhotonView>().owner.GetTeam() != teamID &&
            collision.gameObject.GetComponent<Character>().isAlive == true)
        {
            if (collision.gameObject.GetComponent<Character>().properties.health - damage <= 0 && collision.gameObject.GetComponent<Character>().isAlive == true)
            {
                KingOfTheHill.Instance.netView.RPC("AddPlayerKillStat", PhotonTargets.All, playerID);
                KingOfTheHill.Instance.netView.RPC("PlayerKilledPlayer", PhotonTargets.All, playerID, collision.gameObject.GetComponent<Character>().playerID);
                characterRef.killStreak++;
                KingOfTheHill.Instance.netView.RPC("IncreaseKillStreak", PhotonTargets.All, characterRef.killStreak, characterRef.player.ID, (int)characterRef.type);
                KingOfTheHill.Instance.netView.RPC("AddPlayerMVPStat", PhotonTargets.All, playerID, 10);
                KingOfTheHill.Instance.netView.RPC("AddPlayerKillStat", PhotonTargets.All, playerID);
            }

            KingOfTheHill.Instance.netView.RPC("AddPlayerDamageStat", PhotonTargets.All, collision.gameObject.GetComponent<Character>().player.ID, damage);
            collision.gameObject.GetComponent<PhotonView>().RPC("LoseHealth", PhotonTargets.All, damage);

            characterRef.numSkillsHit++;

            collision.gameObject.GetComponent<PhotonView>().RPC("Stun", PhotonTargets.All, true);
        }
    }

    IEnumerator TurnOffColider()
    {
        yield return new WaitForSeconds(0.5f);
        colider.enabled = false;
    }
}
