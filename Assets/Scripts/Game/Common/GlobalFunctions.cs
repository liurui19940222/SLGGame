using Framework.AStar;
using Framework.Input;
using Game.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Common
{
    public class GlobalFunctions
    {
        // 根据输入得到方向
        public static Direction GetDirectionByInputWord(EInputWord word)
        {
            switch (word)
            {
                case EInputWord.DPAD_UP:
                    return Direction.North;
                case EInputWord.DPAD_DOWN:
                    return Direction.South;
                case EInputWord.DPAD_LEFT:
                    return Direction.West;
                case EInputWord.DPAD_RIGHT:
                    return Direction.East;
            }
            return Direction.Center;
        }

        // 根据方向得到整形向量
        public static IPoint GetPointByDir(Direction dir, int step)
        {
            switch (dir)
            {
                case Direction.North:
                    return new IPoint(0, step);
                case Direction.South:
                    return new IPoint(0, -step);
                case Direction.West:
                    return new IPoint(-step, 0);
                case Direction.East:
                    return new IPoint(step, 0);
            }
            return new IPoint();
        }

        // 根据输入得到整形向量
        public static IPoint GetPointByInputWord(EInputWord word, int step)
        {
            return GetPointByDir(GetDirectionByInputWord(word), step);
        }

        // 得到一个点到另一个点的方向
        public static Direction GetDirFromPointToAnother(IPoint org, IPoint target)
        {
            Direction dir = Direction.Center;
            if (target.X > org.X)
                dir = Direction.East;
            else if (target.X < org.X)
                dir = Direction.West;
            else if (target.Y > org.Y)
                dir = Direction.North;
            else if (target.Y < org.Y)
                dir = Direction.South;
            return dir;
        }
    }
}