using TMPro;
using UnityEngine;

public class ComboCounter : MonoBehaviour
{
    TextMeshProUGUI comboText;

    [SerializeField] float comboTime;
    [SerializeField] float comboTimeLimit;
    [SerializeField] readonly float currentCombo;
    [SerializeField] int comboCount;

    Animator comboAnimator;

    private void Start()
    {
        comboText = GetComponent<TextMeshProUGUI>();
        comboAnimator = GetComponent<Animator>();
    }

    void Update()
    {
        comboTime -= Time.deltaTime;
        ChangeComboText();
        if (comboTime <= comboTimeLimit) { comboTime = 0; comboCount = 0; comboText.text = " "; comboAnimator.SetBool("Disappear", true);
        }
    }

    private void ChangeComboText()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            comboAnimator.SetBool("Appear", true);
            comboCount += 1;
            comboText.text = "Combo " + comboCount;
            comboTime = 1f;
        }
    }
}
