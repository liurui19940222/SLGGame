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

        public static IPoint GetPointByInputWord(EInputWord word, int step)
        {
            return GetPointByDir(GetDirectionByInputWord(word), step);
        }
    }
}