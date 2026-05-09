using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] Image healthBar;  



    public void UpdateHP(float hp, float maxHp)
    {
        if(healthBar != null)
        {
            healthBar.fillAmount = hp/maxHp;
        }
    }

}
