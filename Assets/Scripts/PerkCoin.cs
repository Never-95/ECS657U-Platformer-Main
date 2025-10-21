using UnityEngine;

public class PerkCoin : MonoBehaviour
{
    public enum PerkType { SpeedBoost, DoubleJump }
    public PerkType perkType;
    public float effectDuration = 5f;
    public float speedMultiplier = 2f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();

            if (player != null)
            {
                switch (perkType)
                {
                    case PerkType.SpeedBoost:
                        player.ActivateSpeedBoost(speedMultiplier, effectDuration);
                        break;
                    case PerkType.DoubleJump:
                        player.EnableDoubleJump(effectDuration);
                        break;
                }

                Destroy(gameObject, 0.1f);
            }
        }
    }
}
