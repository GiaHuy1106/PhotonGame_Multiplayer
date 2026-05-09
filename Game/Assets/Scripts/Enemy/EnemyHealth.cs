using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] Image healthBar;
    Transform root;

    private void Awake()
    {
        root = healthBar.transform.parent;
    }
    public void UpdateHP(float hp, float maxHp)
    {
        if(healthBar != null)
        {
            healthBar.fillAmount = hp/maxHp;
        }
    }
    private void Update()
    {
        if(Camera.main != null)
        {
            root.transform.forward = Camera.main.transform.forward;
        }
    }
    public void Visible()
    {
        healthBar.transform.parent.gameObject.SetActive(true);
    }
    public void Invisible()
    {
        healthBar.transform.parent.gameObject.SetActive(false);
    }
}
