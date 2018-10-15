using Framework.AStar;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {

    public Material mat;
    public Camera camera;
    public GridMap2D map;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnRenderObject()
    {
        if (!mat)
        {
            Debug.LogError("Please Assign a material on the inspector");
            return;
        }
        GL.PushMatrix(); //保存当前Matirx
        mat.SetPass(0); //刷新当前材质
        GL.LoadProjectionMatrix(camera.projectionMatrix);
        GL.Color(Color.yellow);
        GL.Begin(GL.LINES);

        Vector3 startPos = new Vector3(-map.Length * 0.5f, 0, -map.Height * 0.5f) + map.m_offsetInWorldSapce;
        Vector3 p0, p1 = default(Vector3);
        for (int x = 0; x <= map.m_colCount; x++)
        {
            p0 = startPos + new Vector3(map.m_cellWidth * x, 0, 0);
            p1 = p0 + new Vector3(0, 0, map.Height);
            GL.Vertex3(p0.x, p0.y, p0.z);
            GL.Vertex3(p1.x, p1.y, p1.z);
        }

        for (int y = 0; y <= map.m_rowCount; y++)
        {
            p0 = startPos + new Vector3(0, 0, map.m_cellHeight * y);
            p1 = p0 + new Vector3(map.Length, 0, 0);
            GL.Vertex3(p0.x, p0.y, p0.z);
            GL.Vertex3(p1.x, p1.y, p1.z);
        }
        GL.End();
        GL.PopMatrix();
    }

}
