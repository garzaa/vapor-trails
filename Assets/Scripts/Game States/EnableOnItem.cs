using UnityEngine;
using UnityEngine.Serialization;

public class EnableOnItem : MonoBehaviour {
    [SerializeField] Item wanted;
    public bool immediate = true;

    public bool setDisabled = false;

    void Start() {
        CheckState();
    }

    public void CheckState() {
        Item i = GlobalController.inventory.items.GetItem(wanted);
        bool hasItem = (i != null && i.count >= wanted.count);
        if (setDisabled) gameObject.SetActive(!hasItem);
        else gameObject.SetActive(hasItem);
    }
}