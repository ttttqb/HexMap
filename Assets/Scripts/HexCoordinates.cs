using UnityEngine;

[System.Serializable]
public struct HexCoordinates
{
    [SerializeField]
    private int x, z;

    public int X => x;

    public int Z => z;

    public HexCoordinates (int x, int z) {
        this.x = x;
        this.z = z;
    }
    public int Y => -X - Z;//立方体坐标，3个可能的运动方向

    public static HexCoordinates FromOffsetCoordinates (int x, int z) {
        return new HexCoordinates(x - z / 2, z);
    }
    
    public override string ToString () {
        return "(" + X.ToString() + ", " + Y.ToString() + ", " + Z.ToString() + ")";
    }

    public string ToStringOnSeparateLines () {
        return X.ToString() + "\n" + Y.ToString() + "\n" + Z.ToString();
    }
    
    public static HexCoordinates FromPosition (Vector3 position) {
        // z=0时的坐标
        float x = position.x / (HexMetrics.innerRadius * 2f);
        float y = -x;
        // 添加z偏移量
        float offset = position.z / (HexMetrics.outerRadius * 3f);
        x -= offset;
        y -= offset;
        // 四舍五入
        int iX = Mathf.RoundToInt(x);
        int iY = Mathf.RoundToInt(y);
        int iZ = Mathf.RoundToInt(-x - y);
        if (iX + iY + iZ != 0) {
            // 舍弃四舍五入时增量最大的坐标，并从其他两个坐标中重构它
            float dX = Mathf.Abs(x - iX);
            float dY = Mathf.Abs(y - iY);
            float dZ = Mathf.Abs(-x -y - iZ);

            if (dX > dY && dX > dZ) {
                iX = -iY - iZ;
            }
            else if (dZ > dY) {
                iZ = -iX - iY;
            }
        }

        return new HexCoordinates(iX, iZ);
    }
}
