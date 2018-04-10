using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Q: MonoBehaviour {

    public double[,] Qmatrix = new double[5,2];
    public static Q instance;

    public Q()
    {
        instance = this;
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                Qmatrix[i, j] = 0;
            }
        }   
    }

    //private void Start()
    //{
    //    instance = this;    
    //    for (int i = 0; i < 5; i++)
    //    {
    //        for (int j = 0; j < 2; j++)
    //        {
    //            Qmatrix[i, j] = 0;
    //        }
    //    }

    //}
}
