using Framework.AStar;
using Game.Common;
using Game.Component;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Entity
{
    public class GameArrow : EntityBase
    {
        private GameObject[] m_PartObjs;

        private List<ArrowPart> m_Parts;

        public GameArrow(Transform parent) : base(ResourceLoader.COMP_PATH, "Arrow", parent)
        {
            m_Parts = new List<ArrowPart>();
            m_PartObjs = new GameObject[4];
            for (int i = 0; i < m_PartObjs.Length; ++i)
            {
                m_PartObjs[i] = m_Transform.Find("arrow_0" + (i + 1)).gameObject;
                m_PartObjs[i].SetActive(false);
            }
        }

        public void ShowPath(List<IPoint> path)
        {
            if (path.Count < 2)
                return;
            Close();
            int type = 0;
            IPoint prev = IPoint.Unavailable;
            IPoint next = IPoint.Unavailable;
            for (int i = 0; i < path.Count; ++i)
            {
                if (i == 0)
                {
                    prev = IPoint.Unavailable;
                    next = path[i + 1];
                    type = 1;
                }
                else if (i == path.Count - 1)
                {
                    prev = path[i - 1];
                    next = IPoint.Unavailable;
                    type = 4;
                }
                else
                {
                    prev = path[i - 1];
                    next = path[i + 1];
                    type = IPoint.IsStraight(prev, path[i], next) ? 2 : 3;
                }
                CreatePart(type, prev, path[i], next);
            }
        }

        public void Close()
        {
            m_Parts.ForEach((part) =>
            {
                part.Release();
            });
            m_Parts.Clear();
        }

        private ArrowPart CreatePart(int type, IPoint prevPoint, IPoint curPoint, IPoint nextPoint)
        {
            Transform trans = null;
            ArrowPart part = null;
            if (type == 1)
            {
                trans = GameObject.Instantiate(m_PartObjs[0]).transform;
                part = new ArrowOrigin(trans);
            }
            if (type == 2)
            {
                trans = GameObject.Instantiate(m_PartObjs[1]).transform;
                part = new ArrowStraight(trans);
            }
            if (type == 3)
            {
                trans = GameObject.Instantiate(m_PartObjs[2]).transform;
                part = new ArrowCorner(trans);
            }
            if (type == 4)
            {
                trans = GameObject.Instantiate(m_PartObjs[3]).transform;
                part = new ArrowEnding(trans);
            }
            trans.SetParent(m_PartObjs[0].transform.parent);
            part.Show(prevPoint, curPoint, nextPoint);
            m_Parts.Add(part);
            return part;
        }
    }

    internal abstract class ArrowPart
    {
        protected Transform m_Transform;

        public ArrowPart(Transform trans)
        {
            this.m_Transform = trans;
            this.m_Transform.gameObject.SetActive(true);
        }

        public abstract void Show(IPoint prevPoint, IPoint curPoint, IPoint nextPoint);

        protected void SetWorldPos(IPoint curPoint)
        {
            this.m_Transform.position = SLG.SLGGame.Instance.MapData.CellToWorldSpacePos(curPoint.X, curPoint.Y);
        }

        public void Release()
        {
            GameObject.Destroy(this.m_Transform.gameObject);
        }
    }

    internal class ArrowOrigin : ArrowPart
    {
        public ArrowOrigin(Transform trans) : base(trans)
        {

        }

        public override void Show(IPoint prevPoint, IPoint curPoint, IPoint nextPoint)
        {
            Direction dir = GlobalFunctions.GetDirFromPointToAnother(curPoint, nextPoint);
            switch (dir)
            {
                case Direction.North:
                    this.m_Transform.eulerAngles = Vector3.zero;
                    break;
                case Direction.South:
                    this.m_Transform.eulerAngles = new Vector3(0, 180, 0);
                    break;
                case Direction.West:
                    this.m_Transform.eulerAngles = new Vector3(0, 270, 0);
                    break;
                case Direction.East:
                    this.m_Transform.eulerAngles = new Vector3(0, 90, 0);
                    break;
            }
            this.SetWorldPos(curPoint);
        }
    }

    internal class ArrowStraight : ArrowPart
    {
        public ArrowStraight(Transform trans) : base(trans)
        {

        }

        public override void Show(IPoint prevPoint, IPoint curPoint, IPoint nextPoint)
        {
            Direction dir = GlobalFunctions.GetDirFromPointToAnother(curPoint, nextPoint);
            switch (dir)
            {
                case Direction.North:
                    this.m_Transform.eulerAngles = Vector3.zero;
                    break;
                case Direction.South:
                    this.m_Transform.eulerAngles = new Vector3(0, 180, 0);
                    break;
                case Direction.West:
                    this.m_Transform.eulerAngles = new Vector3(0, 270, 0);
                    break;
                case Direction.East:
                    this.m_Transform.eulerAngles = new Vector3(0, 90, 0);
                    break;
            }
            this.SetWorldPos(curPoint);
        }
    }

    internal class ArrowCorner : ArrowPart
    {
        public ArrowCorner(Transform trans) : base(trans)
        {

        }

        public override void Show(IPoint prevPoint, IPoint curPoint, IPoint nextPoint)
        {
            Direction forward = GlobalFunctions.GetDirFromPointToAnother(curPoint, nextPoint);
            Direction back = GlobalFunctions.GetDirFromPointToAnother(curPoint, prevPoint);
            switch (forward)
            {
                case Direction.North:
                    {
                        switch (back)
                        {
                            case Direction.West:
                                this.m_Transform.eulerAngles = new Vector3(0, 180, 0);
                                break;
                            case Direction.East:
                                this.m_Transform.eulerAngles = new Vector3(0, 270, 0);
                                break;
                        }
                    }
                    break;
                case Direction.South:
                    {
                        switch (back)
                        {
                            case Direction.West:
                                this.m_Transform.eulerAngles = new Vector3(0, 90, 0);
                                break;
                            case Direction.East:
                                this.m_Transform.eulerAngles = Vector3.zero;
                                break;
                        }
                    }
                    break;
                case Direction.West:
                    {
                        switch (back)
                        {
                            case Direction.North:
                                this.m_Transform.eulerAngles = new Vector3(0, 180, 0);
                                break;
                            case Direction.South:
                                this.m_Transform.eulerAngles = new Vector3(0, 90, 0);
                                break;
                        }
                    }
                    break;
                case Direction.East:
                    {
                        switch (back)
                        {
                            case Direction.North:
                                this.m_Transform.eulerAngles = new Vector3(0, 270, 0);
                                break;
                            case Direction.South:
                                this.m_Transform.eulerAngles = Vector3.zero;
                                break;
                        }
                    }
                    break;
            }
            this.SetWorldPos(curPoint);
        }
    }

    internal class ArrowEnding : ArrowPart
    {
        public ArrowEnding(Transform trans) : base(trans)
        {

        }

        public override void Show(IPoint prevPoint, IPoint curPoint, IPoint nextPoint)
        {
            Direction dir = GlobalFunctions.GetDirFromPointToAnother(prevPoint, curPoint);
            switch (dir)
            {
                case Direction.North:
                    this.m_Transform.eulerAngles = new Vector3(0, 270, 0);
                    break;
                case Direction.South:
                    this.m_Transform.eulerAngles = new Vector3(0, 90, 0);
                    break;
                case Direction.West:
                    this.m_Transform.eulerAngles = new Vector3(0, 180, 0);
                    break;
                case Direction.East:
                    this.m_Transform.eulerAngles = new Vector3(0, 0, 0);
                    break;
            }
            this.SetWorldPos(curPoint);
        }
    }
}