using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{

    private int points = 0;

    private void Update()
    {
        if (points >= 10)
        {
            restartLevel();
        }
        Debug.Log(points);
    }

    public void restartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        points= 0;
    }

    public void collectPoint()
    {
        points++;
    }


}
