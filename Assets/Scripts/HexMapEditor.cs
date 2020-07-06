using UnityEngine;
using UnityEngine.EventSystems;

public class HexMapEditor : MonoBehaviour {

    public Color[] colors;

    public HexGrid hexGrid;

    private Color activeColor;

    private int activeElevation;

    void Awake () {
        SelectColor(0);
    }

    void Update () {
        if (Input.GetMouseButton(0) &&
            !EventSystem.current.IsPointerOverGameObject()) // 如果把ui移到hex上，可以阻止其改变下面hex的颜色
        {
            HandleInput();
        }
    }

    void HandleInput () {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit)) {
            EditCells(hexGrid.GetCell(hit.point));
        }
    }

    void EditCells(HexCell center)
    {
        int centerX = center.coordinates.X;
        int centerZ = center.coordinates.Z;

        for (int r = 0, z = centerZ - brushSize; z <= centerZ; z++, r++) {
            for (int x = centerX - r; x <= centerX + brushSize; x++) {
                EditCell(hexGrid.GetCell(new HexCoordinates(x,z)));
            }
        }
        for (int r = 0, z = centerZ + brushSize; z > centerZ; z--, r++) {
            for (int x = centerX - brushSize; x <= centerX + r; x++) {
                EditCell(hexGrid.GetCell(new HexCoordinates(x, z)));
            }
        }
    }

    private bool applyColor;
    public void SelectColor (int index)
    {
        applyColor = index >= 0;
        if (applyColor)
            activeColor = colors[index];
    }

    private bool applyElevation = true;
    void EditCell(HexCell cell)
    {
        if (!cell) return;
        if (applyColor)
            cell.Color = activeColor;
        if (applyElevation)
            cell.Elevation = activeElevation;
    }

    public void SetApplyElevation(bool toggle)
    {
        applyElevation = toggle;
    }

    public void SetElevation(float elevation)
    {
        activeElevation = (int) elevation;
    }

    private int brushSize;

    public void SetBrushSize(float size)
    {
        brushSize = (int) size;
    }

    public void ShowUI(bool visible)
    {
        hexGrid.ShowUI(visible);
    }
}