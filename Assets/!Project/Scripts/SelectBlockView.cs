using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts
{
    public class SelectBlockView : MonoBehaviour
    {
        [SerializeField] private ItemView itemView;
        [SerializeField] private Button buttonPlus;
        [SerializeField] private Button buttonMinus;
        [SerializeField] private TMP_Text selectedNameText;
        private List<ItemData> _slotConfigs;
        private int _selectedIndex = -1;
        private Action<int> _removeAtAction;
        private Action _updateContentAction;

        public void Initialize(SimpleEventBus.SimpleEventBus eventBus, List<ItemData> slotConfigs, Action<int> removeAtAction, Action updateContentAction)
        {
            _slotConfigs = slotConfigs;
            _removeAtAction = removeAtAction;
            _updateContentAction = updateContentAction;
            itemView.SetData(null, 0);
            selectedNameText.text = string.Empty;
            TryEnableButtons();
            
            eventBus.RegisterEvent().Bind<ItemView>(ItemView.SelectEvent, OnItemSelected).Submit();
        
            buttonMinus.onClick.AddListener(RemoveItem);
            buttonPlus.onClick.AddListener(AddItem);
        }

        private void OnItemSelected(ItemView sView)
        {
            _selectedIndex = sView.SlotIndex;
            itemView.SetData(_slotConfigs[_selectedIndex], _selectedIndex);
            selectedNameText.text = _slotConfigs[_selectedIndex].ItemConfig.Name;
            TryEnableButtons();
        }

        private void TryEnableButtons()
        {
            buttonPlus.interactable = _selectedIndex != -1 &&
                                      _slotConfigs[_selectedIndex].ItemConfig.IsStackable &&
                                      _slotConfigs[_selectedIndex].CurrentCount < 999;
            buttonMinus.interactable = _selectedIndex != -1;
        }

        private void RemoveItem()
        {
            AnimationConstSettings.AnimateSelect(buttonMinus.transform);
            if (_slotConfigs[_selectedIndex].CurrentCount == 1)
            {
                _removeAtAction?.Invoke(_selectedIndex);
                Deselect();
                return;
            }
            _slotConfigs[_selectedIndex].CurrentCount--;
            itemView.UpdateItemCountText();
            _updateContentAction?.Invoke();
            TryEnableButtons();
        }
        
        private void AddItem()
        {
            AnimationConstSettings.AnimateSelect(buttonPlus.transform);
            _slotConfigs[_selectedIndex].CurrentCount++;
            itemView.UpdateItemCountText();
            _updateContentAction?.Invoke();
            TryEnableButtons();
        }

        private void Deselect()
        {
            _selectedIndex = -1;
            itemView.SetData(null, 0);
            selectedNameText.text = string.Empty;
            TryEnableButtons();
        }
    }
}