using Framework.AStar;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Test : MonoBehaviour {

    public Camera camera;
    public LayerMask mask;
    public float height;

	// Use this for initialization
	void Start () {

    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0))
        {
            var ray = camera.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo, 1000, mask))
            {
                Vector3 p = hitInfo.barycentricCoordinate;
                p.y += height;
                transform.position = p;
                transform.forward = hitInfo.normal;
            }
        }
	}

    void OnRenderObject()
    {

    }
}
