using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingGraphicsInstancer : MonoBehaviour
{
    public bool generateBuilding = false;
    [SerializeField] private GameObject buildingLocation;
    [SerializeField] private GameObject buildingPrefab;
    private GameObject buildingInstance;

    // Start is called before the first frame update
    void Start()
    {
        if(generateBuilding)
            GenerateBuilding();
    }

    public void SetBuildingVisibility(bool visibility)
    {
        if(!buildingInstance)
            GenerateBuilding();
        buildingInstance.SetActive(visibility);
    }

    private void GenerateBuilding()
    {
        buildingInstance = Instantiate(buildingPrefab, buildingLocation.transform);
        buildingInstance.transform.Rotate(Vector3.up, Random.Range(-180f, 180f), Space.World);
    }
}
