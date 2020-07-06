using System;
using UnityEngine;

public class HexCell : MonoBehaviour {
    public HexCoordinates coordinates;

    [SerializeField] private HexCell[] neighbors = default;
    
    private int elevation = int.MinValue; // 海拔高度

    public HexGridChunk chunk;

    public int Elevation
    {
        get => elevation;
        set
        {
            if (elevation == value) return;
            elevation = value;
            // 修改海拔
            var position = transform.localPosition;
            position.y = value * HexMetrics.elevationStep;
            position.y +=
                (HexMetrics.SampleNoise(position).y * 2f - 1f) *
                HexMetrics.elevationPerturbStrength;
            transform.localPosition = position;
            // 修改label
            var uiPosition = uiRect.localPosition;
            uiPosition.z = -position.y;
            uiRect.localPosition = uiPosition;
            
            Refresh();
        }
    }

    public RectTransform uiRect;

    public Vector3 Position => transform.localPosition;

    public Color Color {
        get => color;
        set {
            if (color == value) {
                return;
            }
            color = value;
            Refresh();
        }
    }

    Color color;
    
    public HexCell GetNeighbor(HexDirection direction)
    {
        return neighbors[(int) direction];
    }
    
    public void SetNeighbor (HexDirection direction, HexCell cell) {
        neighbors[(int)direction] = cell;
        cell.neighbors[(int) direction.Opposite()] = this;
    }
    
    public HexEdgeType GetEdgeType (HexDirection direction) {
        return HexMetrics.GetEdgeType(
            elevation, neighbors[(int)direction].elevation
        );
    }
    
    public HexEdgeType GetEdgeType (HexCell otherCell) {
        return HexMetrics.GetEdgeType(
            elevation, otherCell.elevation
        );
    }

    void Refresh()
    {
        if (chunk)
        {
            chunk.Refresh();
            for (int i = 0; i < neighbors.Length; i++)
            {
                HexCell neighbor = neighbors[i];
                if (neighbor != null && neighbor.chunk != chunk)
                {
                    neighbor.chunk.Refresh();
                }
            }
        }
    }
}

