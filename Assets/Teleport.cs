using UnityEngine;

public class Teleport : MonoBehaviour
{
    public Transform destination;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TeleportPlayer(other.Transform);
        }
    }
    private void TeleportPlayer(Transform player)
    {
        player.position = destination.position;
    }
}
