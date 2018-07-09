using UnityEngine;

public class AttackCollider : MonoBehaviour
{
    public Character characterRef;
    public int playerID;

    public bool enemyHit = false;
    public int damage;

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Player" &&
            collision.gameObject.GetComponent<PhotonView>().owner.GetTeam() != characterRef.photonView.owner.GetTeam() &&
            collision.gameObject.GetComponent<Character>().isAlive == true)
        {
            if (collision.gameObject.GetComponent<Character>().properties.health - damage <= 0 && collision.gameObject.GetComponent<Character>().isAlive == true)
            {
                KingOfTheHill.Instance.netView.RPC("PlayerKilledPlayer", PhotonTargets.All, playerID, collision.gameObject.GetComponent<Character>().playerID);
                characterRef.killStreak++;
                KingOfTheHill.Instance.netView.RPC("IncreaseKillStreak", PhotonTargets.All, characterRef.killStreak, characterRef.player.ID, (int)characterRef.type);
                KingOfTheHill.Instance.netView.RPC("AddPlayerMVPStat", PhotonTargets.All, playerID, 10);
                KingOfTheHill.Instance.netView.RPC("AddPlayerKillStat", PhotonTargets.All, playerID);
            }
            collision.gameObject.GetComponent<PhotonView>().RPC("LoseHealth", PhotonTargets.AllViaServer, damage);
            KingOfTheHill.Instance.netView.RPC("AddPlayerDamageStat", PhotonTargets.All, playerID, damage);
            characterRef.numSkillsHit++;
            enemyHit = true;
            if (characterRef.properties.charges < 2)
            {
                characterRef.properties.charges++;
            }
        }
    }
}
