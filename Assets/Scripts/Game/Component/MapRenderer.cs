using Framework.AStar;
using Game.Common;
using Game.Data;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Component
{
    public class MapRenderer : MonoBehaviour
    {
        public Material LineMat;

        public Camera Camera;
        public GridMap2D MapData;
        private bool m_NeedDrawGrid;

        public void EnableGridDrawing()
        {
            m_NeedDrawGrid = true;
        }

        public void DisableGridDrawing()
        {
            m_NeedDrawGrid = false;
        }

        void OnRenderObject()
        {
            if (m_NeedDrawGrid)
            {
                DrawGrid();
            }
        }

        private void DrawGrid()
        {
            GL.wireframe = true;
            GL.PushMatrix(); //保存当前Matirx
            LineMat.SetPass(0); //刷新当前材质
            GL.LoadProjectionMatrix(Camera.projectionMatrix);
            GL.Begin(GL.LINES);

            Vector3 startPos = new Vector3(-MapData.Length * 0.5f, 0, -MapData.Height * 0.5f) + MapData.m_offsetInWorldSapce;
            Vector3 p0, p1 = default(Vector3);
            for (int x = 0; x <= MapData.m_colCount; x++)
            {
                p0 = startPos + new Vector3(MapData.m_cellWidth * x, 0, 0);
                p1 = p0 + new Vector3(0, 0, MapData.Height);
                GL.Vertex3(p0.x, p0.y, p0.z);
                GL.Vertex3(p1.x, p1.y, p1.z);
            }

            for (int y = 0; y <= MapData.m_rowCount; y++)
            {
                p0 = startPos + new Vector3(0, 0, MapData.m_cellHeight * y);
                p1 = p0 + new Vector3(MapData.Length, 0, 0);
                GL.Vertex3(p0.x, p0.y, p0.z);
                GL.Vertex3(p1.x, p1.y, p1.z);
            }
            GL.End();
            GL.PopMatrix();
            GL.wireframe = false;
        }

        [ContextMenu("EnableDrawing")]
        void Editor_Enable()
        {
            EnableGridDrawing();
        }

    }

}
