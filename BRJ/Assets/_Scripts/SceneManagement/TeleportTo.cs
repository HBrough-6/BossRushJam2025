using UnityEngine;

public class TeleportTo : MonoBehaviour
{
    public Transform teleportPoint;

    public AudioClip GolemMusic;
    public AudioSource source;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.position = teleportPoint.position;
            source.Stop();
            source.clip = GolemMusic;
            source.PlayDelayed(1);
        }

    }
}
