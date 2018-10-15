using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Data
{
    public class ActionCellData
    {
        public int X { get; set; }
        public int Y { get; set; }

        public ActionCellData(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    public class ActionRangeData
    {
        public List<ActionCellData> AttackingList { get; set; }

        public List<ActionCellData> MovingList { get; set; }

        public ActionRangeData()
        {
            AttackingList = new List<ActionCellData>();
            MovingList = new List<ActionCellData>();
        }
    }
}