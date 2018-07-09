using UnityEngine;
using UnityEngine.AI;

public class Movement : MonoBehaviour
{
    public Vector3 destination;

    public Character character;
    Animator animator;
    public NavMeshAgent agent;
    PhotonView netView = null;
    GameObject[] players;
    public GameObject ping;

    public bool isStunned = false;
    bool isAlive = true;

    void Start ()
    {
        netView = GetComponent<PhotonView>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        character = GetComponent<Character>();

        InputManager.Instance.onMousePressed += (Vector3 mousePosition) =>
        {
            if (!netView.isMine || isStunned == true || isAlive == false)
                return;
            
            destination = mousePosition;


            Instantiate(ping, destination += new Vector3(0, 0, 0), Quaternion.identity);

            ///////////         Pangolin Animation States              ////////////

            if (character.type == Character.CharacterID.Pangolin)
            {
                players = GameObject.FindGameObjectsWithTag("Player");

                foreach (GameObject player in players)
                {
                    if (Vector3.Distance(transform.position, player.transform.position) < 20 &&
                        player.GetComponent<PhotonView>().owner.GetTeam() != gameObject.GetComponent<PhotonView>().owner.GetTeam())
                    {
                        animator.SetBool("enemyNear", true);
                    }
                    else if (Vector3.Distance(transform.position, player.transform.position) > 20 &&
                        player.GetComponent<PhotonView>().owner.GetTeam() != gameObject.GetComponent<PhotonView>().owner.GetTeam())
                    {
                        animator.SetBool("enemyNear", false);
                    }
                    else
                    {
                        animator.SetBool("isMoving", true);
                    }
                }
            }
            ///////////         Ape Animation State                 ////////////
            else
            {
                animator.SetBool("isMoving", true);
            }
            netView.RPC("MoveToTarget", PhotonTargets.All, PhotonNetwork.player.ID, mousePosition);
        };
    }
    
    ///////////         is Player at his Destination              ////////////

    private void Update()
    {
        if(character.type == Character.CharacterID.Pangolin)
        {
            if (agent.enabled && !agent.pathPending)
            {
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                    {
                        animator.SetBool("enemyNear", false);
                        animator.SetBool("isMoving", false);
                    }
                }
            }
        }
        else if (agent.enabled && !agent.pathPending)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    animator.SetBool("isMoving", false);
                }
            }
        }
        if(character.isStunned == true)
        {
            isStunned = true;

            animator.SetBool("isMoving", false);
        }
        else
        {
            isStunned = false;
        }
        isAlive = character.isAlive;
    }

    [PunRPC]
    void MoveToTarget(int playerID, Vector3 target)
    {
        if (playerID != netView.ownerId || !agent.enabled || isStunned == true || isAlive == false)
            return;

        animator.SetBool("isMoving", true);
        agent.destination = target;
        Quaternion newRotation = Quaternion.LookRotation(agent.steeringTarget - transform.position);
        newRotation.x = 0f;
        newRotation.z = 0f;
        transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime * 10);
    }

    [PunRPC]
    void SetMovementState(bool state)
    {
        agent.enabled = state;
    }

    [PunRPC]
    void LookAt(Vector3 lookDirection)
    {
        transform.LookAt(lookDirection);
    }

    [PunRPC]
    void Teleport(Vector3 position)
    {
        agent.Warp(position);
    }

    public bool IsCloseToPos(Vector3 destination, float margin)
    {
        if (Vector3.Distance(transform.position, destination) <= margin)
            return true;

        return false;
    }
}
