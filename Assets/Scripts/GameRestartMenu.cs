using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameRestartMenu : MonoBehaviour {
    [Header("UI Pages")] public GameObject gameOverMenu;

    [Header("GameOver Menu Buttons")] public Button restartButton;
    public Button quitButton;

    public List<Button> returnButtons;

    // Start is called before the first frame update
    private void Start() {
        EnableGameOverMenu();

        //Hook events
        restartButton.onClick.AddListener(RestartGame);
        quitButton.onClick.AddListener(QuitGame);
    }

    public void QuitGame() {
        Application.Quit();
    }

    public void RestartGame() {
        HideAll();
        SceneTransitionManager.singleton.GoToSceneAsync(1);
    }

    public void HideAll() {
        gameOverMenu.SetActive(false);
    }

    public void EnableGameOverMenu() {
        gameOverMenu.SetActive(true);
    }
}