using UnityEngine;

public class MissingDataOrigin : MonoBehaviour
{

    public DataModelInfoSO correctModel;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    void OnEnable()
    {
        Debug.Log("Data Origin Scanned");
        UIManager.FlashWarning();
        UIManager.ToggleReleaseModelButton(true);
        GameManager.activeDataOrigin = gameObject;
    }
    void OnDisable()
    {
        Debug.Log("Data Origin NOT Scanned");
        UIManager.StopFlashWarning();
        
        UIManager.ToggleReleaseModelButton(false);
        GameManager.activeDataOrigin = null;
    }
}
