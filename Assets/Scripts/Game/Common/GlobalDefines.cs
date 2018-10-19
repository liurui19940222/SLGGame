using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Common
{
    public class GlobalDefines
    {
        // 角色的Y坐标分量值
        public const float CHAR_Y = -0.485f;

        // 范围格子的Y坐标分量值
        public const float RANGE_CELL_Y = -0.5f;

        // 光标的Y坐标分量值
        public const float CURSOR_Y = -0.485f;

        // 箭头的Y坐标分量值
        public const float ARROW_Y = -0.49f;

        // 光标的移动速度
        public const float CURSOR_SPEED = 15.0f;

        // 光标连续移动阈值
        public const float CURSOR_MOVING_THRESHOLD = 0.98f;

        // 按键按下多少时间之后，光标开始算连续移动
        public const float CURSOR_HOLDON_THRESHOLD = 0.15f;

        // 世界相机欧拉角度
        public static readonly Vector3 WORLD_CAMERA_EULER = new Vector3(90.0f, 0.0f, 0.0f);

        // 世界相机Y坐标分量值
        public const float WORLD_CAMERA_Y = 10.0f;

        // 世界相机正交投影大小
        public const float WORLD_CAMERA_SIZE = 6.0f;

        // 世界相机能够超出网格最大范围的部分
        public const float WORLD_CAMERA_MAX_OUTOF_RANGE = 0.5f;

        // 地图单元格状态
        public const int CELL_STATE_CHAR = 1 << 0;      //角色占领
    }

    public enum Direction
    {
        Center,
        North,
        South,
        West,
        East,
    }

    public enum ETurnType
    {
        System,
        OwnSide,
        Friendly,
        Opposite,
    }

}