using UnityEngine;

public static class HexMetrics
{
    // 六边形内径外径
    public const float outerRadius = 10f;
    public const float innerRadius = outerRadius * 0.866025404f;
    
    // 混合区域和纯色区域
    public const float solidFactor = 0.8f;
    public const float blendFactor = 1f - solidFactor;

    // 海拔高度层次
    public const float elevationStep = 3f;
    
    // 梯田参数
    public const int terracesPerSlope = 2;
    public const int terraceSteps = terracesPerSlope * 2 + 1;
    public const float horizontalTerraceStepSize = 1f / terraceSteps;
    public const float verticalTerraceStepSize = 1f / (terracesPerSlope + 1);
    
    // 噪声纹理和扰动范围
    public static Texture2D noiseSource;
    public const float cellPerturbStrength = 4f;
    public const float elevationPerturbStrength = 1.5f;
    public const float noiseScale = 0.003f;//扩大纹理范围以产生局部连贯性
    
    // 网格分块
    public const int chunkSizeX = 5, chunkSizeZ = 5;

    public static Vector3 TerraceLerp (Vector3 a, Vector3 b, int step) {
        float h = step * HexMetrics.horizontalTerraceStepSize;
        a.x += (b.x - a.x) * h;
        a.z += (b.z - a.z) * h;
        float v = ((step + 1) / 2) * HexMetrics.verticalTerraceStepSize;//只在奇数步改变y值以生成平坦面
        a.y += (b.y - a.y) * v;
        return a;
    }
    
    public static Color TerraceLerp (Color a, Color b, int step) {
        float h = step * HexMetrics.horizontalTerraceStepSize;
        return Color.Lerp(a, b, h);
    }
    
    public static Vector3[] corners = {
        new Vector3(0f, 0f, outerRadius),
        new Vector3(innerRadius, 0f, 0.5f * outerRadius),
        new Vector3(innerRadius, 0f, -0.5f * outerRadius),
        new Vector3(0f, 0f, -outerRadius),
        new Vector3(-innerRadius, 0f, -0.5f * outerRadius),
        new Vector3(-innerRadius, 0f, 0.5f * outerRadius),
        new Vector3(0f, 0f, outerRadius)
    };
    
    public static Vector3 GetFirstCorner (HexDirection direction) {
        return corners[(int)direction];
    }

    public static Vector3 GetSecondCorner (HexDirection direction) {
        return corners[(int)direction + 1];
    }
    
    public static Vector3 GetFirstSolidCorner (HexDirection direction) {
        return corners[(int)direction] * solidFactor;
    }

    public static Vector3 GetSecondSolidCorner (HexDirection direction) {
        return corners[(int)direction + 1] * solidFactor;
    }
    
    public static Vector3 GetBridge (HexDirection direction) {
        return (corners[(int)direction] + corners[(int)direction + 1]) *
               blendFactor;
    }
    
    public static HexEdgeType GetEdgeType (int elevation1, int elevation2) {
        if (elevation1 == elevation2) {
            return HexEdgeType.Flat;
        }
        int delta = elevation2 - elevation1;
        if (delta == 1 || delta == -1) {
            return HexEdgeType.Slope;
        }
        return HexEdgeType.Cliff;
    }

    public static Vector4 SampleNoise(Vector3 position)
    {
        return noiseSource.GetPixelBilinear(position.x * noiseScale,
            position.z * noiseScale);
    }
}

public static class HexDirectionExtensions {
    // 返回反方向
    public static HexDirection Opposite (this HexDirection direction) {
        return (int)direction < 3 ? (direction + 3) : (direction - 3);
    }
    // 返回前一个和后一个方向
    public static HexDirection Previous (this HexDirection direction) {
        return direction == HexDirection.NE ? HexDirection.NW : (direction - 1);
    }

    public static HexDirection Next (this HexDirection direction) {
        return direction == HexDirection.NW ? HexDirection.NE : (direction + 1);
    }
}

public enum HexDirection
{
    NE, E, SE, SW, W, NW
}

public enum HexEdgeType
{
    Flat, Slope, Cliff
}