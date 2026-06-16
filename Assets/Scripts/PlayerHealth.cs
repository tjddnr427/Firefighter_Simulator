using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float maxHP = 100f;
    [SerializeField] private Slider hpSlider;

    private float currentHP;

    void Start()
    {
        currentHP = maxHP;
        if (hpSlider != null) hpSlider.value = 1f;
    }

    public void TakeDamage(float amount)
    {
        currentHP -= amount;
        currentHP = Mathf.Clamp(currentHP, 0f, maxHP);
        if (hpSlider != null) hpSlider.value = currentHP / maxHP;

        if (currentHP <= 0f)
            GameManager.Instance.GameOver();
    }
}
