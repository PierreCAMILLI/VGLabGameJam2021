using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField]
    private RectTransform _tutorialTransform;

    [SerializeField]
    private RectTransform[] _tutorialTextsTransform;

    private int _tutorialIndex;


    public void PlayButton()
    {
        GameManager.Instance.LaunchGame();
    }

    public void TutorialButton()
    {
        _tutorialIndex = 0;
        _tutorialTransform.gameObject.SetActive(true);

        _tutorialTextsTransform[0].gameObject.SetActive(true);
        for (int i = 1; i < _tutorialTextsTransform.Length; ++i)
        {
            _tutorialTextsTransform[i].gameObject.SetActive(false);
        }
    }

    public void MenuButton()
    {
        _tutorialTransform.gameObject.SetActive(false);
    }

    public void PreviousButton()
    {
        _tutorialTextsTransform[_tutorialIndex].gameObject.SetActive(false);
        _tutorialIndex = Mathf.Max(0, _tutorialIndex - 1);
        _tutorialTextsTransform[_tutorialIndex].gameObject.SetActive(true);
    }

    public void NextButton()
    {
        _tutorialTextsTransform[_tutorialIndex].gameObject.SetActive(false);
        _tutorialIndex = Mathf.Min(_tutorialTextsTransform.Length - 1, _tutorialIndex + 1);
        _tutorialTextsTransform[_tutorialIndex].gameObject.SetActive(true);
    }

    public void QuitButton()
    {
        Application.Quit();
    }
}
