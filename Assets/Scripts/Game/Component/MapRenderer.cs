using Framework.AStar;
using Game.Common;
using Game.Data;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Component
{
    public class MapRenderer : MonoBehaviour
    {
        public GridMap2D MapData;

        private Transform m_Grid;

        private Material m_GridMat;

        private void Awake()
        {
            m_Grid = transform.Find("Grid");
            m_GridMat = m_Grid.GetComponent<MeshRenderer>().sharedMaterial;   
        }

        public void InitWithMapData(GridMap2D MapData)
        {
            this.MapData = MapData;
            Vector3 scale = new Vector3(MapData.m_colCount * MapData.m_cellWidth, MapData.m_rowCount * MapData.m_cellHeight, 1.0f);
            m_Grid.localScale = scale;
            m_Grid.position = MapData.m_offsetInWorldSapce;
            m_GridMat.SetTextureScale("_MainTex", new Vector2(MapData.m_colCount, MapData.m_rowCount));
        }

        public void EnableGridDrawing()
        {
            this.m_Grid.gameObject.SetActive(true);
        }

        public void DisableGridDrawing()
        {
            this.m_Grid.gameObject.SetActive(false);
        }

        [ContextMenu("EnableDrawing")]
        void Editor_Enable()
        {
            InitWithMapData(MapData);
        }

    }

}
