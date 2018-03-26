using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public bool AutoMove;
    public GameObject astar;

    // Update is called once per frame
    void Update () {
		if(Input.GetKey(KeyCode.Escape))
            SceneManager.LoadScene("Menu");

        if (AutoMove)
            astar.SetActive(true);
        else
            astar.SetActive(false);
    }
}
