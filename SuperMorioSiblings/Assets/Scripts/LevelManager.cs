using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{

    private int points = 0;

    [SerializeField]
    private int wincond;

    private void Update()
    {
        if (points >= wincond)
        {
            restartLevel();
        }
    }

    public void restartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        points = 0;
    }

    public void collectPoint()
    {
        points++;
    }


}
