using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLevel : MonoBehaviour {
    public GameObject LevelCompleteUI;

    //TODO: When player collides with NextLevelTrigger, change scene to next in build

    //void OnCollisionEnter(Collision collision)
    //{
    //   Debug.Log("NEXT LEVEL?");
    //    LevelCompleteUI.SetActive(true);
    //}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("NEXT LEVEL?");
            LevelCompleteUI.SetActive(true);
        }
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void GoNextLevel()
    {
            Debug.Log("Next Level!");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
