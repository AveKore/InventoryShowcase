using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts
{
    public class SelectableItemView : ItemView, IInitializable
    {
        [SerializeField] private Button button;
        private SimpleEventBus.SimpleEventBus _simpleEventBus;
        
        public void Initialize(SimpleEventBus.SimpleEventBus simpleEventBus)
        {
            _simpleEventBus = simpleEventBus;
            _simpleEventBus.RegisterEvent().Bind<ItemView>(SelectEvent, TryDeselectAction).Submit();
            button.onClick.AddListener(SelectAction);
        }
        
        private void TryDeselectAction(ItemView slotIndex)
        {
            if (slotIndex.SlotIndex == SlotIndex)
            {
                Deselect();
            }
        }

        private void SelectAction()
        {
            Select();
            _simpleEventBus.Fire(SelectEvent, this);
        }
        

        private void Deselect()
        {
        }
        
        private void Select()
        {
            AnimationConstSettings.AnimateSelect(gameObject.transform);
        }
    }
}