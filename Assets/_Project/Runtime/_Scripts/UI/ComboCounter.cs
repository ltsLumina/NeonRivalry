#region
using System.Collections;
using TMPro;
using UnityEngine;
#endregion

public class ComboCounter : MonoBehaviour
{
    TextMeshProUGUI comboText;
    Animator comboAnimator;

    [SerializeField] float comboTime;
    [SerializeField] float comboTimeReset;
    [SerializeField] float comboTimeChange;
    [SerializeField] int comboCount;

    void Start()
    {
        comboText     = GetComponent<TextMeshProUGUI>();
        comboAnimator = GetComponent<Animator>();
    }

    void Update()
    {
        comboTime -= Time.deltaTime;
        ChangeComboText();

        if (comboTime <= 0f) ResetCombo();
    }

    public void ChangeComboText()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            comboCount++;
            comboText.text = $"Combo\n{comboCount}";
            comboTime      = comboTimeChange;
            comboAnimator.SetBool("Appear", comboCount > 1);
            comboAnimator.SetBool("Disappear", false);
            StartCoroutine(ComboPop());
        }
    }

    void ResetCombo()
    {
        comboCount     = 0;
        comboText.text = string.Empty;
        comboAnimator.SetBool("Appear", false);
        comboAnimator.SetBool("Disappear", true);
    }

    public void ResetDisappearAnimation() => comboAnimator.SetBool("Disappear", false);

    IEnumerator ComboPop()
    {
        if (comboCount < 2) yield break;

        comboAnimator.Play("ComboPop");
        yield return new WaitForSeconds(1f);
    }
}
