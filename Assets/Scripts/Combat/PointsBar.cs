using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PointsBar : MonoBehaviour
{
    public Animator DamageTextAnimator;
    public TextMeshProUGUI DamageTextField;
    public TextMeshProUGUI OriginalDamageTextField;
    public Image BarUI;
    private float CurrentValue;
    public float NewValue;
    public float MaxValue;
    private const float Speed = 2.0f;
    
    [SerializeField] private Transform _inputUIAnchor;
    public Transform InputUIAnchor => _inputUIAnchor;

    void Start()
    {
        BarUI = GetComponent<Image>();
    }

    void Update()
    {
        BarUI.fillAmount = CurrentValue / MaxValue;
    }

    void LateUpdate()
    {
        CurrentValue = Mathf.Lerp(CurrentValue, NewValue, Time.deltaTime * Speed);
    }

    public void PlayDamageTextField(int damage, bool hasAttackMissed, bool isResistant, bool isWeakness)
    {
        DamageTextField.gameObject.SetActive(true);
        DamageTextField.color = OriginalDamageTextField.color;
        DamageTextField.fontSize = OriginalDamageTextField.fontSize;
        DamageTextField.fontStyle = OriginalDamageTextField.fontStyle;

        if (isResistant)
        {
            DamageTextField.fontSize /= 1.2f;
            DamageTextField.color = Color.blue;
        }
        else if (isWeakness)
        {
            DamageTextField.fontSize *= 1.8f;
            DamageTextField.color = Color.red;
            DamageTextField.fontStyle = FontStyles.Bold;
        }
        else if (hasAttackMissed)
        {
            DamageTextField.color = Color.gray;
            DamageTextField.fontSize *= 1.2f;
            DamageTextField.fontStyle = FontStyles.Strikethrough;
            DamageTextField.fontStyle = FontStyles.Normal;
        }

        DamageTextField.text = $"-{damage.ToString()}";
        DamageTextAnimator.enabled = true;
        DamageTextAnimator.Play("Move");
    }

    public void PlayAttackResultTextField(string text, bool hitWeakPoint)
    {
        DamageTextField.gameObject.SetActive(true);
        DamageTextField.fontSize *= 1.14f;
        DamageTextField.color = hitWeakPoint ? Color.red : Color.gray;
        DamageTextField.fontStyle = FontStyles.Bold;
        DamageTextField.text = $"*{text}*";
        DamageTextAnimator.enabled = true;
        DamageTextAnimator.Play("Move");
    }
}
