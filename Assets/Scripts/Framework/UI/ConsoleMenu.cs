using Framework.Common.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.UI
{
    [System.Serializable]
    public struct Padding
    {
        public float left;
        public float right;
        public float top;
        public float bottom;
    }

    public class ConsoleMenuItem
    {
        private int m_Id;
        private RectTransform m_Transform;
        private GameObject m_GameObject;

        public RectTransform Transform { get { return m_Transform; } }

        public GameObject GameObject { get { return m_GameObject; } }

        public int Id { get { return m_Id; } }

        public float Width { get { return m_Transform.sizeDelta.x; } }

        public float Height { get { return m_Transform.sizeDelta.y; } }

        public ConsoleMenuItem()
        {

        }

        public void CreateInstance(int id, GameObject prefab, Transform parent)
        {
            m_Id = id;
            m_GameObject = GameObject.Instantiate(prefab);
            m_Transform = m_GameObject.GetComponent<RectTransform>();
            m_Transform.SetParent(parent);
            m_Transform.localScale = Vector3.one;
            OnInitialize();
        }

        public void Release()
        {
            if (!m_GameObject)
                return;
            GameObject.Destroy(m_GameObject);
            m_GameObject = null;
            m_Transform = null;
        }

        protected virtual void OnInitialize()
        {

        }
    }

    public class ConsoleMenuItemText : ConsoleMenuItem
    {
        private Text m_Text;

        protected override void OnInitialize()
        {
            base.OnInitialize();
            m_Text = GameObject.GetComponentInChildren<Text>();
        }

        public void SetText(string text)
        {
            m_Text.text = text;
        }
    }

    public class ConsoleMenu : MonoBehaviour 
    {

        public GameObject ItemPrefab;

        public RectTransform BgImage;

        public RectTransform.Axis Axis;

        public Transform Focus;

        public Padding Padding;

        private List<ConsoleMenuItem> m_Items;

        private int m_CurIndex;
        private GridLayoutGroup m_Grid;

        private System.Action<int> m_OnSelected;

        void Awake()
        {
            m_Items = new List<ConsoleMenuItem>();
            m_Grid = GetComponentInChildren<GridLayoutGroup>();
            Show(false);
            if (Axis == RectTransform.Axis.Vertical)
                m_Grid.childAlignment = TextAnchor.MiddleCenter;
        }

        /*int OnceStep = 1;

        void OnGUI()
        {
            if (GUILayout.Button("add item"))
            {
                AddItem<ConsoleMenuItem>(0);
                Show(true);
            }

            if (GUILayout.Button("cursor up"))
            {
                MoveCursor(-OnceStep);
            }

            if (GUILayout.Button("cursor down"))
            {
                MoveCursor(OnceStep);
            }

            OnceStep = (int)GUILayout.HorizontalSlider(OnceStep, 0, 5, GUILayout.Width(300));
        }*/

        void Update()
        {
            UpdateFocus();
        }

        public void MoveCursor(int step = 1)
        {
            m_CurIndex += step;
            if (m_CurIndex < 0)
                m_CurIndex = m_Items.Count + m_CurIndex;
            else if (m_CurIndex > m_Items.Count - 1)
                m_CurIndex = m_CurIndex - (m_Items.Count);
        }

        //处理输入
        public bool OnInput(InputMessage msg)
        {
            if (!msg.IsDown)
                return false;
            switch (msg.Word)
            {
                case Input.EInputWord.DPAD_UP:
                    if (Axis == RectTransform.Axis.Vertical)
                    {
                        MoveCursor(-1);
                        return true;
                    }
                    break;
                case Input.EInputWord.DPAD_DOWN:
                    if (Axis == RectTransform.Axis.Vertical)
                    {
                        MoveCursor(1);
                        return true;
                    }
                    break; 
                case Input.EInputWord.DPAD_LEFT:
                    if (Axis == RectTransform.Axis.Horizontal)
                    {
                        MoveCursor(-1);
                        return true;
                    }
                    break;
                case Input.EInputWord.DPAD_RIGHT:
                    if (Axis == RectTransform.Axis.Horizontal)
                    {
                        MoveCursor(1);
                        return true;
                    }
                    break;
                case Input.EInputWord.A:
                    if (m_OnSelected != null)
                    {
                        m_OnSelected(m_Items[m_CurIndex].Id);
                    }
                    return true;
            }
            return false;
        }

        public void Show(bool isShow)
        {
            BgImage.gameObject.SetActive(isShow);
            Focus.gameObject.SetActive(isShow);
            m_Grid.gameObject.SetActive(isShow);
        }

        public T AddItem<T>(int id) where T : ConsoleMenuItem, new()
        {
            T item = new T();
            item.CreateInstance(id, ItemPrefab, m_Grid.transform);
            m_Items.Add(item);
            Readjust();
            return item;
        }

        public void Clear()
        {
            foreach (ConsoleMenuItem item in m_Items)
            {
                item.Release();
            }
            m_Items.Clear();
        }

        public void Readjust()
        {
            if (m_Items.Count == 0)
                return;
            if (Axis == RectTransform.Axis.Vertical)
            {
                float bgWidth = Padding.left + Padding.right + m_Grid.cellSize.x;
                float bgHeight = Padding.top + Padding.bottom + m_Grid.cellSize.y * m_Items.Count + m_Grid.spacing.y * (m_Items.Count - 1);
                BgImage.sizeDelta = new Vector2(bgWidth, bgHeight);
                BgImage.anchoredPosition = new Vector2(-Padding.left * 0.5f + Padding.right * 0.5f, Padding.top * 0.5f - Padding.bottom * 0.5f);
            }
        }

        public void UpdateFocus()
        {
            if (m_Items.Count == 0)
                return;
            Focus.localPosition = m_Items[m_CurIndex].Transform.localPosition;
        }

        public void ResetCursor()
        {
            m_CurIndex = 0;
        }

        public void SetOnSelectedDelegate(System.Action<int> onSelected)
        {
            m_OnSelected = onSelected;
        }
    }
}