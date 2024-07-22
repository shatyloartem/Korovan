using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;


public class SceneManage : MonoBehaviour
{
    [SerializeField] private PlayerInventory selectedOponentInvwntory;

    public void OnMouseDown()
    {
        DataStorage.Instance.SetOponentInventory(selectedOponentInvwntory);

        SceneManager.LoadScene("BattleScene");
    }
}
