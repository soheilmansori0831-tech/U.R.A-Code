using System.Collections;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public float maxHealth;
    // private HealthBarControl healthBarControl;
    private float currentHealth;

    [Header("Flash")]
    [SerializeField] private float floashDuration;    
    [SerializeField, Range(0, 1)] private float floashStrength;
    [SerializeField] private Color flashColor;
    [SerializeField] private Material flashMaterial;
    [SerializeField] private SpriteRenderer spriteRenderer;
    private Material defaultMaterial;

    public bool canTakeDamage = true;
    public float GetCurrentHealth() => currentHealth;
    public bool CanTakeDamage() => canTakeDamage;

    private void Start()
    {
        // healthBarControl = FindFirstObjectByType<HealthBarControl>();
        currentHealth = maxHealth;
        // healthBarControl.SetSliderValue(currentHealth, maxHealth);
        defaultMaterial = spriteRenderer.material;
    }

    public void DamagePlayer(float damage)
    {
        if (canTakeDamage == false) return;

        currentHealth -= damage;
        // healthBarControl.SetSliderValue(currentHealth, maxHealth);
        StartCoroutine(Flash());
        if (currentHealth <= 0)
        {
            if (Player.instance.stateMachine.currentState != PlayerStates.State.KnockBack
              && Player.instance.stateMachine.currentState != PlayerStates.State.Death)
            {
                Player.instance.stateMachine.ChangeState(PlayerStates.State.Death);
            }
        }
    }

    private IEnumerator Flash()
    {
        canTakeDamage = false;
        spriteRenderer.material = flashMaterial;
        flashMaterial.SetColor("_Flash_Color", flashColor);
        flashMaterial.SetFloat("_FlashAmount", floashStrength);
        yield return new WaitForSeconds(floashDuration);
        spriteRenderer.material = defaultMaterial;
        if (currentHealth > 0) canTakeDamage = true;
    }
}
