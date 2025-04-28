using System.Collections.Generic;
using System.Text;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts
{
    public class InventoryWindowView : MonoBehaviour
    {
        private const float HeaderAnimationStartPosition = 691f;
        private const float HeaderAnimationEndPosition = 387.23f;
        private const float HeaderAnimationDuration = 1f;
        private const float BlockAnimationStartPosition = -1000f;
        private const float BlockAnimationEndPosition = -51f;
        private const float BlockAnimationDuration = 1f;
        
        [SerializeField] private ItemView itemTemplate;
        [SerializeField] private RectTransform block;
        [SerializeField] private RectTransform header;
        [SerializeField] private InventoryItemsConfig inventoryItemsConfig;
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private PooledGridContainer pooledGridContainer;
        [SerializeField] private SelectBlockView selectBlockView;
        [SerializeField] private Button logButton;

        private SimpleEventBus.SimpleEventBus EventBus { get; } = new();
        
        private void Start()
        {
            var slotsData = CreateSlotsData();
            pooledGridContainer.Initialize(slotsData, EventBus);
            selectBlockView.Initialize(EventBus, slotsData, pooledGridContainer.RemoveAt, pooledGridContainer.UpdateInternal);
            logButton.onClick.AddListener(() => LogData(slotsData));
            AnimateOpenWindow();
        }

        private void LogData(List<ItemData> slotsData)
        {
            StringBuilder stringBuilder = new StringBuilder();
            Dictionary<string, int> itemsCountByType = new Dictionary<string, int>();
            foreach (var itemData in slotsData)
            {
                if (!itemsCountByType.TryAdd(itemData.ItemConfig.Name, itemData.CurrentCount))
                {
                    itemsCountByType[itemData.ItemConfig.Name] += itemData.CurrentCount;
                }
            }

            foreach (var item in itemsCountByType)
            {
                stringBuilder.Append($"{item.Key}: {item.Value}\n");
            }
          
            Debug.Log(stringBuilder.ToString());
            AnimationConstSettings.AnimateSelect(logButton.transform);
        }

        private void AnimateOpenWindow()
        {
            var pos = new Vector3(block.anchoredPosition.x, BlockAnimationStartPosition);
            block.anchoredPosition = pos;
            pos.y = BlockAnimationEndPosition;
            block.DOAnchorPos(pos, BlockAnimationDuration).SetEase(Ease.OutBack);
            
            pos = new Vector3(header.anchoredPosition.x, HeaderAnimationStartPosition);
            header.anchoredPosition = pos;
            pos.y = HeaderAnimationEndPosition;
            header.DOAnchorPos(pos, HeaderAnimationDuration).SetEase(Ease.OutBack);
        }

        private List<ItemData> CreateSlotsData()
        {
            List<ItemData> slotConfigs = new List<ItemData>();
            foreach (var config in inventoryItemsConfig.ItemConfigs)
            {
                if (!config.IsStackable)
                {
                    for (var j = 0; j < config.Count; j++)
                    {
                        slotConfigs.Add(new ItemData(config));
                    }
                    continue;
                }
                slotConfigs.Add(new ItemData(config));
            }

            return slotConfigs;
        }
    }
}