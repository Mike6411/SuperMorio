using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{

    private int points = 0;

    //Score UI is handled in the level manager because it's pretty simple and there's no need  to make a manager for it right now
    [SerializeField]
    public Text scoreText;

    //Points to win, settable through Inspector
    [SerializeField]
    private int wincond;

    private void Update()
    {
        if (points >= wincond)
        {
            restartLevel();
        }
  
        scoreText.text = "Snacks remaining: " + (wincond - points).ToString();
    }

    public void restartLevel()
    {
        //Simple scene reload and point reset
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        points = 0;
    }

    public void collectPoint()
    {
        points++;
    }


}
