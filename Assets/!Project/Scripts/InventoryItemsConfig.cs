using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "InventoryItems", menuName = "Configs/InventoryItems", order = 1)]
public class InventoryItemsConfig : ScriptableObject
{
    public List<ItemConfig> ItemConfigs;
}

[Serializable]
public class ItemConfig
{
    public Sprite Image;
    public string Name;
    public int Count;
    [FormerlySerializedAs("IsStackble")] public bool IsStackable;
}