using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleInitializer : MonoBehaviour
{
    [SerializeField] private Transform PlayerSpawnPoint;
    [SerializeField] private Transform OponentSpawnPoint;

    private void Start()
    {
        
    }

    private void PlaceUnits(List<BattleUnits_Info> units, Transform spawnPoint)
    {
        foreach (var unitData in units)
        {
            GameObject unit = Instantiate(unitData.Prefab_, spawnPoint.position, Quaternion.identity);
            unit.name = unitData.name;
        }
    }

}


