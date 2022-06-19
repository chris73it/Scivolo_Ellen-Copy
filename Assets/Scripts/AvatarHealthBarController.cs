using ScriptableObjectArchitecture;
using UnityEngine;
using UnityEngine.UI;

public class AvatarHealthBarController : MonoBehaviour
{
    [SerializeField] Image healthBarImage;
    [SerializeField] FloatReference avatarCurrentHealth = null;
    [SerializeField] FloatReference avatarMaxHealth = null;

    void Awake()
    {
        avatarCurrentHealth.Value = avatarMaxHealth.Value;
    }

    public void UpdateHealthBar()
    {
        if (healthBarImage.fillAmount != avatarCurrentHealth.Value / avatarMaxHealth.Value)
        {
            healthBarImage.fillAmount = avatarCurrentHealth.Value / avatarMaxHealth.Value;
        }
    }
}
