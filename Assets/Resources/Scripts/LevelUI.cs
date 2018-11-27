using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelUI : MonoBehaviour {
    public Sprite[] HealthSprites;
    public Image HealthUI;
    private PlayerController player;
    public GameObject settingsMenu;
    private bool isActive = false;


    // Use this for initialization
    void Start () {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
	}
	
	// Update is called once per frame
	void Update () {
        HealthUI.sprite = HealthSprites[(100-player.getHealth())/10];

        if (Input.GetKeyDown("escape"))
        {
            if (!isActive)
            {
                PauseGame();
            }
            else
            {
                ContinueGame();
            }
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
        isActive = !isActive;
        settingsMenu.SetActive(isActive);
    }

    public void ContinueGame()
    {
        Time.timeScale = 1;
        isActive = !isActive;
        settingsMenu.SetActive(isActive);
    }

    public void QuitGame()
    {
        Debug.Log("Quit!");
        Application.Quit();
    }
}
