using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerInventory", menuName = "ScriptableObjects/PlayerInventory", order = 2)]
public class PlayerInventory : ScriptableObject
{
    [SerializeField] private List<BattleUnits_Info> Units;

    public List<BattleUnits_Info> Units_;
}
