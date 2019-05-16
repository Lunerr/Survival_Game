using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    [SerializeField]
    private Image health_Stats, stamina_Stats;

    [SerializeField]
    private Text ammo_Stats, wood_Stats, kills_Stats;

    public static int wood;
    public static int kills;

    private WeaponManager weaponManager;

    public void Awake()
    {
        weaponManager = GetComponent<WeaponManager>();
    }

    public void Update()
    {
        kills_Stats.text = kills.ToString();
        wood_Stats.text = wood.ToString();
        ammo_Stats.text = weaponManager.GetCurrentSelectedWeapon().bullets.ToString();
    }

    public void Display_HealthStats(float healthValue)
    {
        healthValue /= 100f;

        health_Stats.fillAmount = healthValue;
    }

    public void Display_StaminaStats(float staminaValue)
    {
        staminaValue /= 100f;

        stamina_Stats.fillAmount = staminaValue;
    }
}
