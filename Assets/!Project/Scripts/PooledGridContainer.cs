using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts
{
    public enum ReorientMethod
    {
        TopToBottom,
        BottomToTop
    }
    
    public class PooledGridContainer : MonoBehaviour
    {
        [SerializeField] private GridLayoutGroup gridLayoutGroup;
        [SerializeField] private SelectableItemView prefab;
        [SerializeField] private ScrollRect scrollRect;
        private int TotalElementsCount => _data.Count;
        
        private int _defaultTopPadding;
        private int _lineElementsCount;
        private Pool<SelectableItemView> _elementsPool;
        private readonly List<SelectableItemView> _activeElements = new();
        private List<ItemData> _data;
        private float _layoutSpacing;
        private RectOffset _padding;
        private float _elementSize;
        private int _lastElementsCulledAbove = -1;
        private int _startPaddingTop;
        private int _startPaddingBottom;
        private int _startPaddingLeft;
        private int _startPaddingRight;


        public void Initialize(List<ItemData> data, SimpleEventBus.SimpleEventBus simpleEventBus)
        {
            _data = data;
            
            SetStartPaddings();
            CalculateElementSize();
            CalculateLineElementsCount();
            CreatePool(simpleEventBus);
            Initialize();
        }

        public void Add(ItemData item)
        {
            _data.Add(item);
            UpdateInternal();
        }

        public void RemoveAt(int index)
        {
            _data.RemoveAt(index);
            UpdateInternal();
        }

        public void UpdateInternal()
        {
            UpdateContent();
            UpdateActiveElements();
        }

        private void Initialize()
        {
            _lastElementsCulledAbove = -1;
            ResetPosition();
            UpdateInternal();
            Subsribe();
        }

        private void Subsribe()
        {
            scrollRect.onValueChanged.AddListener(ScrollMoved);
        }
        
        private void CalculateElementSize()
        {
            _layoutSpacing = scrollRect.vertical ? gridLayoutGroup.spacing.y : gridLayoutGroup.spacing.x;
            _elementSize = scrollRect.vertical ? prefab.RectTransform.rect.height : prefab.RectTransform.rect.width;
            _elementSize += _layoutSpacing;
        }
        
        private void CalculateLineElementsCount()
        {
            _lineElementsCount = Mathf.CeilToInt(((RectTransform)scrollRect.transform).rect.width / (_elementSize+_padding.left+_padding.right));
            var linesCount = Mathf.CeilToInt(TotalElementsCount / (float)_lineElementsCount);
            AdjustContentSize(_elementSize * linesCount);
        }

        private void CreatePool(SimpleEventBus.SimpleEventBus simpleEventBus)
        {
            var scrollAreaSize = GetScrollAreaSize(scrollRect.viewport);
            var elementsVisibleInScrollArea = Mathf.CeilToInt(scrollAreaSize / _elementSize) * _lineElementsCount;
            _elementsPool = new Pool<SelectableItemView>(prefab, transform, elementsVisibleInScrollArea, simpleEventBus);
        }
        
        private void SetStartPaddings()
        {
            _padding = gridLayoutGroup.padding;
            _startPaddingTop = _padding.top;
            _startPaddingBottom = _padding.bottom;
            _startPaddingLeft = _padding.left;
            _startPaddingRight = _padding.right;
            _defaultTopPadding = _padding.top;
        }

        private void UpdateContent()
        {
            if (_lineElementsCount == 0)
            {
                return;
            }
            var linesCount = Mathf.CeilToInt(TotalElementsCount / (float)_lineElementsCount);
            var lostElements = _lineElementsCount - TotalElementsCount % _lineElementsCount;
            AdjustContentSize(_elementSize * linesCount);

            var scrollAreaSize = GetScrollAreaSize(scrollRect.viewport);
            var elementsVisibleInScrollArea = Mathf.CeilToInt(scrollAreaSize / _elementSize) * _lineElementsCount;
            var totalHiddenElements = TotalElementsCount - elementsVisibleInScrollArea;
            if (lostElements != _lineElementsCount)
            {
                totalHiddenElements += lostElements;
            }
            var elementsCulledAbove = Mathf.Clamp(Mathf.FloorToInt(GetScrollRectNormalizedPosition() * totalHiddenElements), 0,
                Mathf.Clamp(TotalElementsCount, 0, int.MaxValue));

            var requiredElementsInList = Mathf.Min(elementsVisibleInScrollArea + _lineElementsCount, TotalElementsCount);
            if (elementsCulledAbove != TotalElementsCount - (elementsVisibleInScrollArea) - _lineElementsCount + lostElements)
            {
                elementsCulledAbove -= elementsCulledAbove % _lineElementsCount;
            }
            else
            {
                requiredElementsInList = Mathf.Min(elementsVisibleInScrollArea + _lineElementsCount-lostElements, TotalElementsCount);
            }

            ChangeTopPadding(elementsCulledAbove);

            if (_activeElements.Count != requiredElementsInList)
            {
                InitializeElements(requiredElementsInList, elementsCulledAbove);
            }
            else if (_lastElementsCulledAbove != elementsCulledAbove)
            {
                ReorientElement(elementsCulledAbove > _lastElementsCulledAbove ? ReorientMethod.TopToBottom : ReorientMethod.BottomToTop, elementsCulledAbove);
            }

            _lastElementsCulledAbove = elementsCulledAbove;
        }

        private void ChangeTopPadding(float size)
        {
            var requiredSpaceElements = (int)(size/_lineElementsCount);
            var rowSize = (int)(gridLayoutGroup.cellSize.y + gridLayoutGroup.spacing.y);
            gridLayoutGroup.padding.top = rowSize * requiredSpaceElements;
        }

        private void ReorientElement(ReorientMethod reorientMethod, int elementsCulledAbove)
        {
            if (_activeElements.Count <= 1)
            {
                return;
            }

            var count = Mathf.Abs(elementsCulledAbove - _lastElementsCulledAbove);

            if (reorientMethod == ReorientMethod.TopToBottom)
            {
                for (var i = 0; i < count; i++)
                {
                    var top = _activeElements[0];
                    var dataIndex = elementsCulledAbove + _lineElementsCount + i;
                
                    _activeElements.RemoveAt(0);
                    if (dataIndex >= _data.Count)
                    {
                        continue;
                    }
                    _activeElements.Add(top);

                    top.transform.SetSiblingIndex(_activeElements[_activeElements.Count - 2].transform.GetSiblingIndex() + 1);
                    top.SetData(_data[dataIndex], dataIndex);
                }
            }
            else
            {
                for (var i = 0; i < count; i++)
                {
                    var bottom = _activeElements[_activeElements.Count - 1];
                    var dataIndex = elementsCulledAbove - (i + 1 - count);
                    _activeElements.RemoveAt(_activeElements.Count - 1);
                    _activeElements.Insert(0, bottom);

                    bottom.transform.SetSiblingIndex(_activeElements[1].transform.GetSiblingIndex());
                    bottom.SetData(_data[dataIndex], dataIndex);
                }
            }
        }

        private void UpdateActiveElements()
        {
            for (var i = 0; i < _activeElements.Count; i++)
            {
                var activeElement = _activeElements[i];
                if (_lastElementsCulledAbove + i >= _data.Count)
                {
                    continue;
                }

                var dataIndex =_lastElementsCulledAbove + i;
                activeElement.SetData(_data[dataIndex], dataIndex);
            }
        }
        
        private void InitializeElements(int requiredElementsInList, int numElementsCulledAbove)
        {
            ClearPool();

            _activeElements.Clear();

            for (var i = 0; i < requiredElementsInList && i + numElementsCulledAbove < TotalElementsCount; i++)
            {
                _activeElements.Add(CreateElement(i + numElementsCulledAbove));
            }
        }

        private void ClearPool()
        {
            foreach (var t in _activeElements)
            {
                _elementsPool.Return(t);
            }
        }

        private SelectableItemView CreateElement(int index)
        {
            var newElement = _elementsPool.GetNext();
            newElement.transform.SetParent(scrollRect.content, false);
            newElement.transform.SetSiblingIndex(index);
            newElement.SetData( _data[index], index);

            return newElement;
        }

        private void AdjustContentSize(float size)
        {
            var currentSize = scrollRect.content.sizeDelta;
            size -= _layoutSpacing;

            if (scrollRect.vertical)
            {
                if (_padding != null)
                {
                    size += _startPaddingTop + _startPaddingBottom;
                }

                currentSize.y = size;
            }
            else
            {
                if (_padding != null)
                {
                   size += _startPaddingLeft + _startPaddingRight;
                }

                currentSize.x = size;
            }

            scrollRect.content.sizeDelta = currentSize;
        }

        private float GetScrollAreaSize(RectTransform viewPort)
        {
            return scrollRect.vertical ? viewPort.rect.height : viewPort.rect.width;
        }

        private void ResetPosition()
        {
            if (scrollRect.vertical)
            {
                scrollRect.verticalNormalizedPosition = 1f;
            }
            else
            {
                scrollRect.horizontalNormalizedPosition = 0f;
            }
        }

        private float GetScrollRectNormalizedPosition()
        {
            return Mathf.Clamp01(scrollRect.vertical ? 1 - scrollRect.verticalNormalizedPosition : scrollRect.horizontalNormalizedPosition);
        }
        
        private void ScrollMoved(Vector2 delta)
        {
            UpdateInternal();
        }
        
        private void OnDestroy()
        {
            scrollRect.onValueChanged.RemoveListener(ScrollMoved);

            ClearPool();

            _elementsPool?.Dispose();
            _activeElements.Clear();
            gridLayoutGroup.padding.top = _defaultTopPadding;
        }
    }
}