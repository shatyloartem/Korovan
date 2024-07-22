using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[CreateAssetMenu (fileName = "BattleUnits", menuName = "ScriptableObjects/BattleUnit", order = 1)]
public class BattleUnits_Info : ScriptableObject
{
    [SerializeField] private string UnitClass;
    [SerializeField] private string Name;

    [SerializeField] private int HealthPoint;
    [SerializeField] private int Damage;

    [SerializeField] private GameObject Prefab;
    
    public string UnitName_ => UnitClass;
    public string Name_ => Name;

    public int HealthPoint_ => HealthPoint;
    public int Damage_ => Damage;

    public GameObject Prefab_ => Prefab;
}
