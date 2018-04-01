using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class GameManager : MonoBehaviour {

    public bool AutoMove;
    public GameObject astar;
    public Text gameover;

    void Update () {
		if(Input.GetKey(KeyCode.Escape))
            SceneManager.LoadScene("Menu");

        if (Timer.instance.timelimit <= 0)
        {
            gameover.text = "GAME OVER!";
            Time.timeScale = 0;
        }

        if (AutoMove)
            astar.SetActive(true);
        else
            astar.SetActive(false);
    }

}
