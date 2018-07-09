using System.Collections;
using UnityEngine;

public class SpawnHeal : MonoBehaviour
{
    public PunTeams.Team teamID;
    public float healCycle;

    bool heal = true;

    void OnTriggerStay(Collider collision)
    {
        if (collision.gameObject.tag == "Player" &&
            collision.gameObject.GetComponent<PhotonView>().owner.GetTeam() == teamID)
        {
            if (collision.gameObject.GetComponent<Character>().properties.health < collision.gameObject.GetComponent<Character>().maxHealth)
            {
                collision.gameObject.GetComponent<Character>().isHealing = true;
            }
            else
            {
                collision.gameObject.GetComponent<Character>().isHealing = false;
            }

            if (heal == true)
            {
                heal = false;
                
                collision.gameObject.GetComponent<PhotonView>().RPC("LoseHealth", PhotonTargets.AllViaServer, -10);
                collision.gameObject.GetComponent<PhotonView>().RPC("LoseEnergie", PhotonTargets.AllViaServer, -10);

                StartCoroutine(HealCycle(healCycle));
            }
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if(collider.gameObject.tag == "Player" &&
            collider.gameObject.GetComponent<PhotonView>().owner.GetTeam() == teamID)
        {
            collider.gameObject.GetComponent<Character>().isHealing = false;
        }
    }
    IEnumerator HealCycle(float timeDelay)
    {
        yield return new WaitForSeconds(timeDelay);
        heal = true;
    }
}
