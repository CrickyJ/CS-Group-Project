using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//simple directional enum... cmon unity why isn't this stock
public enum Direction {
    Right,
    Up,
    Left,
    Down,
}

//this class holds shortcut layermask names so you don't have to write up the masks every time
public static class LAYERMASK {
    public static int SOLIDS = 1 << 8;
    public static int EDGE_TRIGGER = 1 << 9;
    public static int PROJECTILE = 1 << 10;
}
public static class LAYER {
    public static int SOLIDS = 8;
    public static int EDGE_TRIGGER = 9;
    public static int PROJECTILE = 10;
}

public static class Extensions {
    public static void DrawDebugRect2D(Vector2 center, Vector2 size, Color color, float duration = 0, bool depthTest = true) {
        Debug.DrawRay(center, Vector2.one, color, duration);
        Debug.DrawRay(center, new Vector2(size.x, 0), color, duration);
        Debug.DrawRay(center, new Vector2(-size.x, 0), color, duration);
        Debug.DrawRay(center, new Vector2(0, size.y), color, duration);
        Debug.DrawRay(center, new Vector2(0, -size.y), color, duration);

        /*
        for (int n = 0; n < 2; n++) {
            Debug.DrawLine(min + n * max, min + new Vector2(max.x, 0), color, duration, depthTest);
            Debug.DrawLine(min + n * max, min + new Vector2(0, max.y), color, duration, depthTest);
            Debug.DrawLine(min + n * max, min + max, color, duration, depthTest);
        }
        */
    }
}