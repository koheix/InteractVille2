using UnityEngine;

public class InventoryToggle : MonoBehaviour
{
    public GameObject inventoryUI;
    private bool isOpen = false;

    void Start(){
        inventoryUI.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Escape))
        {
            isOpen = !isOpen;
            inventoryUI.SetActive(isOpen);
        }
    }
}