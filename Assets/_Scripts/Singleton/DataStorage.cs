using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataStorage: MonoBehaviour
{
    [SerializeField] private DataStorage Instance;

    [SerializeField] private PlayerInventory PlayerInventory;
    [SerializeField] private PlayerInventory OponentInventory;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetOponentInventory(PlayerInventory inventory)
    {
        OponentInventory = inventory;
    }
}
