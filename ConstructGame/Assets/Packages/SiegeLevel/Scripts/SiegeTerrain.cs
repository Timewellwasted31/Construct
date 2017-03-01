using UnityEngine;
using System.Collections;

public static class SiegeTerrain {

    static byte[,] m_grid;

    public static void CreateBlankMap(int x, int y) {
        m_grid = new byte[x, y];
    }

    public static void UpdateTile(float x, float y, byte b) {
        UpdateTile((int)x, (int)y, b);
    }

    public static void UpdateTile(int x, int y, byte b) {
        if (x >= m_grid.GetLength(0) || y >= m_grid.GetLength(1)) return;

        m_grid[x, y] = b;
    }

    public static byte GetTile(float x, float y) {
        return GetTile((int)x, (int)y);
    }

    public static byte GetTile(int x, int y) {
        if (x >= m_grid.GetLength(0) || y >= m_grid.GetLength(1)) return 0;
        return m_grid[x, y];
    }

}
