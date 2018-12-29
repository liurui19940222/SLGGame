using Framework.AStar;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Test : MonoBehaviour {


	void Start () {
        Debug.Log(IPoint.IsStraight(new IPoint(5, 6), new IPoint(5, 7), new IPoint(5, 8)));
        Debug.Log(IPoint.IsStraight(new IPoint(5, 6), new IPoint(5, 7), new IPoint(6, 7)));
        Debug.Log(IPoint.IsStraight(new IPoint(5, 6), new IPoint(6, 7), new IPoint(8, 8)));
    }

	void Update () {

	}

    void OnRenderObject()
    {

    }
}
