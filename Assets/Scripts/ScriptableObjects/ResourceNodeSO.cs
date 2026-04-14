using UnityEngine;

[CreateAssetMenu(fileName = "new Resource Node SO", menuName = "ScriptableObjects/ResourceNodeSO")]
public class ResourceNodeSO : ScriptableObject
{
    public Transform prefab;
    public string objectName;
    public int maxHealth;
}
