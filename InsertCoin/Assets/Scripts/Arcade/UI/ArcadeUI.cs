using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArcadeUI : MonoBehaviour
{
    [SerializeField]
    private TMPro.TMP_Text _scoreCounterText;

    [SerializeField]
    private TMPro.TMP_Text _creditsCounterText;

    [SerializeField]
    private RectTransform _continueUi;
    [SerializeField]
    private TMPro.TMP_Text _continueCounterText;

    [SerializeField]
    private RectTransform _insertCoinUi;

    [SerializeField]
    private string _creditChar;

    public int Score { get; set; }
    public int Credits { get; set; }
    public float ContinueTimer { get; set; }
    public bool InsertCoinUIEnabled { get { return _insertCoinUi.gameObject.activeSelf; } set { _insertCoinUi.gameObject.SetActive(value); } }

    // Update is called once per frame
    void Update()
    {
        _scoreCounterText.text = string.Format("{0:#,###0}", Score);

        _creditsCounterText.text = new string(_creditChar[0], Credits);

        _continueUi.gameObject.SetActive(ContinueTimer >= 0f);
        if (ContinueTimer >= 0f)
        {
            _continueCounterText.text = Mathf.FloorToInt(ContinueTimer).ToString();
        }
    }
}
