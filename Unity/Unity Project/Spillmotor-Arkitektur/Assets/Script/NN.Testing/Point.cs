using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Point
{
    [SerializeField] public int x, y, label;

    public void Setup()
    {
        x = Random.Range(0, 100);
        y = Random.Range(0, 100);

        if (x > y)
        {
            label = 1;
        }
        else
        {
            label = -1;
        }
    }

    


}
