using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class GameManager : MonoBehaviour {

    public GameObject astar;
    public Text gameover;
    public Text endGameGoldValue;

void Awake()
    {
        //Time.timeScale = 2;
    }

    void Update () {
		if(Input.GetKey(KeyCode.Escape))
            SceneManager.LoadScene("Menu");

        if (Timer.instance.timelimit <= 0)
        {
            if (Player.instance.training)
            {
                endGameGoldValue.text = "Gold Collected : " + Player.instance.get_player_gold_value();
                QLearning.instance.EnvReset();
            }
            else
            {
                endGameGoldValue.text = "Gold Collected : " + Player.instance.get_player_gold_value();
                gameover.text = "GAME OVER!";
                Time.timeScale = 0;
            }
        }
    }

}
