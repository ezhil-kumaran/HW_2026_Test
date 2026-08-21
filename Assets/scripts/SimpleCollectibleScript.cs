using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SimpleCollectibleScript : MonoBehaviour
{
    public enum CollectibleTypes { Star, SpeedBoost, BonusScore };
    public CollectibleTypes CollectibleType = CollectibleTypes.Star;

    public bool rotate = true;
    public float rotationSpeed = 100f;
    public AudioClip collectSound;
    public GameObject collectEffect;
    public int bonusScore = 5;

    private bool collected = false;

    private void Update()
    {
        if (rotate)
        {
            transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.World);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (collected) return;

        if (other.CompareTag("Player"))
        {
            collected = true;
            Collect();
        }
    }

    public void Collect()
    {
        if (collectSound != null)
            AudioSource.PlayClipAtPoint(collectSound, transform.position);

        if (collectEffect != null)
        {
            // 1. Spawn effect
            GameObject effect = Instantiate(collectEffect, transform.position, Quaternion.identity);

            // 2. Automatically destroy the effect after 2 seconds
            Destroy(effect, 2f);
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddBonusScore(bonusScore);
        }

        Destroy(gameObject);
    }
}