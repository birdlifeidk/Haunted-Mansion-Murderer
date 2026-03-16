using UnityEngine;

public class Warp : MonoBehaviour
{
    public Transform target;
    public AudioSource loopSound; // The sound that plays continuously
    public AudioSource teleportEffect; // Optional: A one-time "zap" sound

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 1. Stop the continuous sound
            if (loopSound != null && loopSound.isPlaying)
            {
                loopSound.Stop();
            }

            // 2. Play the one-time teleport sound (optional)
            if (teleportEffect != null)
            {
                teleportEffect.Play();
            }

            // 3. Move the player
            other.transform.position = target.position;
        }
    }
}