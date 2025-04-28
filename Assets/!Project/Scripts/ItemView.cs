using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts
{
    public class ItemView : MonoBehaviour
    {
        public const string SelectEvent = "Select";
        
        [SerializeField] private Image itemImage;
        [SerializeField] private TMP_Text countText;
        private ItemData _itemData;
        
        public RectTransform RectTransform => transform as RectTransform;
        public int SlotIndex { get; private set; }
        private bool IsStackable { get; set; }
        
   
        public void SetData(ItemData itemData, int slotIndex)
        {
            itemImage.gameObject.SetActive(itemData != null);
            if (itemData == null)
            {
                countText.gameObject.SetActive(false);
                _itemData = null;
                return;
            }
            SlotIndex = slotIndex;
            itemImage.sprite = itemData.ItemConfig.Image;
            IsStackable = itemData.ItemConfig.IsStackable;
            countText.gameObject.SetActive(IsStackable);
            countText.text = itemData.CurrentCount.ToString();
            _itemData = itemData;
        }

        public void UpdateItemCountText()
        {
            countText.text =  _itemData.CurrentCount.ToString();
        }
    }
}
