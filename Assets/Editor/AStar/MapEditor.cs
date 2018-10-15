using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using Framework.AStar;

[InitializeOnLoad]
[CustomEditor(typeof(GridMap2D))]
public class MapEditor : Editor
{
    static GridMap2D map;

    static Vector3 m_curOverCellPos;
    static IPoint m_curSelectedPoint = IPoint.Unavailable;

    static Vector2 m_from;
    static Vector2 m_to;
    static List<IPoint> m_path;

    public override void OnInspectorGUI()
    {
        map = serializedObject.targetObject as GridMap2D;
        map.m_GridCustom = EditorGUILayout.ObjectField("自定义网格类型", map.m_GridCustom, typeof(GridCustom), true) as GridCustom;
        map.m_rowCount = EditorGUILayout.IntField("行数", map.m_rowCount);
        map.m_colCount = EditorGUILayout.IntField("列数", map.m_colCount);
        map.m_cellWidth = EditorGUILayout.FloatField("单元格宽度", map.m_cellWidth);
        map.m_cellHeight = EditorGUILayout.FloatField("单元格高度", map.m_cellHeight);
        map.m_offsetInWorldSapce = EditorGUILayout.Vector3Field("世界空间偏移", map.m_offsetInWorldSapce);
        map.Fit();
        EditorGUILayout.LabelField("总宽度", map.Length.ToString());
        EditorGUILayout.LabelField("总高度", map.Height.ToString());

        m_from = EditorGUILayout.Vector2Field("从", m_from);
        m_to = EditorGUILayout.Vector2Field("到", m_to);

        if (GUILayout.Button("寻路"))
        {
            FindPath(false);
        }
        if (GUILayout.Button("寻路(忽略对角)"))
        {
            FindPath(true);
        }

        if (m_curSelectedPoint != IPoint.Unavailable)
        {
            ECellType type = (ECellType)map.GetType(m_curSelectedPoint.X, m_curSelectedPoint.Y);
            ECellType s = (ECellType)EditorGUILayout.EnumPopup("单元格类型", type);
            map.SetType(m_curSelectedPoint.X, m_curSelectedPoint.Y, (int)s);
        }

        EditorUtility.SetDirty(map);
    }

    static MapEditor()
    {
        SceneView.onSceneGUIDelegate -= OnSceneGUI;
        SceneView.onSceneGUIDelegate += OnSceneGUI;
    }

    static IPoint lastPoint = new IPoint();

    static void OnSceneGUI(SceneView sceneView)
    {
        if (map == null)
            return;
        if (UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().name != "MapEditor")
            return;

        //画静态的网格
        Vector3 startPos = new Vector3(-map.Length * 0.5f, 0, -map.Height * 0.5f) + map.m_offsetInWorldSapce;
        Vector3 p0, p1 = default(Vector3);
        for (int x = 0; x <= map.m_colCount; x++)
        {
            p0 = startPos + new Vector3(map.m_cellWidth * x, 0, 0);
            p1 = p0 + new Vector3(0, 0, map.Height);
            Handles.DrawLine(p0, p1);
        }

        for (int y = 0; y <= map.m_rowCount; y++)
        {
            p0 = startPos + new Vector3(0, 0, map.m_cellHeight * y);
            p1 = p0 + new Vector3(map.Length, 0, 0);
            Handles.DrawLine(p0, p1);
        }

        startPos = new Vector3(-map.Length * 0.5f + map.m_cellWidth * 0.5f, 0, -map.Height * 0.5f + map.m_cellHeight * 0.5f) + map.m_offsetInWorldSapce;
        for (int x = 0; x < map.m_colCount; x++)
        {
            for (int y = 0; y < map.m_rowCount; y++)
            {
                int type = map.GetType(x, y);
                if (type == (int)ECellType.OBSTACLE)
                {
                    DrawCube(startPos + new Vector3(x * map.m_cellWidth, 0, y * map.m_cellHeight),
                        Quaternion.identity, new Vector3(0.9f * map.m_cellWidth, 0.001f, 0.9f * map.m_cellHeight), map.m_GridCustom.m_CellInfo[type].Color);
                }
            }
        }

        Handles.matrix = Matrix4x4.identity;

        //场景中的拾取
        Vector2 mousePosition = new Vector2(Event.current.mousePosition.x, Event.current.mousePosition.y);
        Ray ray = HandleUtility.GUIPointToWorldRay(mousePosition);
        Plane plane = new Plane(Vector3.up, map.m_offsetInWorldSapce);
        float enter;
        plane.Raycast(ray, out enter);
        Vector3 hitPoint = ray.direction * enter + ray.origin;
        IPoint point = map.WorldSpaceToCellPos(hitPoint);
        bool available = map.IsAvailable(point.X, point.Y);

        if (Event.current.type == EventType.MouseDown)
        {
            if (lastPoint != point && available)
            {
                m_curOverCellPos = map.CellToWorldSpacePos(point.X, point.Y);
                lastPoint = point;
            }
            if (map.IsAvailable(point.X, point.Y))
                m_curSelectedPoint = point;
        }

        if (Event.current.control && available)
        {
            m_from.x = point.X;
            m_from.y = point.Y;
        }

        if (Event.current.keyCode == KeyCode.Space && available)
        {
            m_to.x = point.X;
            m_to.y = point.Y;
        }

        if (Event.current.keyCode == KeyCode.Return)
        {
            FindPath(false);
        }

        //当前选中
        if (m_curOverCellPos != Vector3.zero)
        {
            Handles.color = Color.green;
            Handles.DrawWireCube(m_curOverCellPos, new Vector3(map.m_cellWidth, 0.001f, map.m_cellHeight));
            Handles.color = Color.white;
        }

        Vector3 size = new Vector3(0.5f * map.m_cellWidth, 0.2f, 0.5f * map.m_cellHeight);
        if (m_path != null && m_path.Count > 0)
        {
            for (int i = 0; i < m_path.Count; ++i)
            {
                DrawCube(m_path[i], Quaternion.identity, size, Color.Lerp(map.m_GridCustom.m_EndPointColor, map.m_GridCustom.m_StartPointColor, (i + 1) / (float)m_path.Count));
            }
        }
        DrawCube(new IPoint((int)m_from.x, (int)m_from.y), Quaternion.identity, size, map.m_GridCustom.m_StartPointColor);
        DrawCube(new IPoint((int)m_to.x, (int)m_to.y), Quaternion.identity, size, map.m_GridCustom.m_EndPointColor);
    }

    static void DrawCube(Vector3 pos, Quaternion q, Vector3 s, Color color)
    {
        Handles.color = color;
        Handles.matrix = Matrix4x4.TRS(pos, q, s);
        Handles.CubeCap(0, Vector3.zero, Quaternion.identity, 1);
        Handles.color = Color.white;
    }

    static void DrawCube(IPoint point, Quaternion q, Vector3 s, Color color)
    {
        DrawCube(map.CellToWorldSpacePos(point.X, point.Y), q, s, color);
    }

    static void FindPath(bool ignoreCorners)
    {
        m_path = map.FindPath(new IPoint((int)m_from.x, (int)m_from.y), new IPoint((int)m_to.x, (int)m_to.y), ignoreCorners);
    }
}
