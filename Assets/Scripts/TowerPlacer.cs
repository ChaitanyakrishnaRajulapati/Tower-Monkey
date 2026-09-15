using UnityEngine;
using UnityEngine.EventSystems;

public class TowerPlacer : MonoBehaviour
{
    public Camera cam;
    public LayerMask tileMask;

    public TowerSO selectedTower;

    [Header("Tower Buttons (optional)")]
    public TowerSO fastSO;
    public TowerSO longSO;
    public TowerSO screamerSO;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // ✅ prevents placing when clicking UI buttons
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;

            TryPlace();
        }
    }

    void TryPlace()
    {
        if (selectedTower == null) return;
        if (CoinManager.Instance == null) return;

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit, 1000f, tileMask))
            return;

        BuildPoint bp = hit.collider.GetComponent<BuildPoint>();
        if (bp == null || bp.occupied) return;

        // ✅ money check + spend first
        int cost = selectedTower.cost;
        if (!CoinManager.Instance.Spend(cost))
        {
            Debug.Log("Not enough coins! Need " + cost + " Have " + CoinManager.Instance.Coins);
            return;
        }

        // ✅ spawn in center of tile collider
        Vector3 pos = hit.collider.bounds.center;

        Instantiate(selectedTower.prefab, pos, Quaternion.identity);
        bp.occupied = true;
    }

    // Button hooks
    public void SelectFast() => selectedTower = fastSO;
    public void SelectLong() => selectedTower = longSO;
    public void SelectScreamer() => selectedTower = screamerSO;
}


