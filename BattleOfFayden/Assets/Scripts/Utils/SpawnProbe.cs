using UnityEngine;

public class SpawnProbe : MonoBehaviour
{
    public PunTeams.Team team;

    private void Awake()
    {
        KingOfTheHill.Instance.AddSpawn(team, this.transform);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 1, 1, 0.5f);
        Gizmos.DrawCube(transform.position, new Vector3(5, 5, 5));
    }
}
