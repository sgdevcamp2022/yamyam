using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(ScrollRect))]
    [RequireComponent(typeof(RectTransform))]

    public class UIRecycleViewController<T> : MonoBehaviour
    {
        protected List<T> TableData = new List<T>();
        [SerializeField] protected GameObject CellBase = null;
        [SerializeField] private RectOffset _padding;
        [SerializeField] private float _spacingHeight = 4.0f;
        [SerializeField] private RectOffset _visibleRectPadding = null;

        private LinkedList<UIRecycleViewCell<T>> _cells = new LinkedList<UIRecycleViewCell<T>>();
        private Rect _visibleRect;
        private Vector2 _preScrollPos;

        public RectTransform CachedRectTransform => GetComponent<RectTransform>();
        public ScrollRect CachedScrollRect => GetComponent<ScrollRect>();
        public Vector2 SizeDelta;

        protected virtual void Start()
        {
            CellBase.SetActive(false);
            CachedScrollRect.onValueChanged.AddListener(OnScrollPoschanged);
        }

        protected void InitializeTableView()
        {
            UpdateScrollViewSize();
            UpdateVisibleRect();

            if(_cells.Count <1)
            {
                Vector2 cellTop = new Vector2(0f, -_padding.top);
                for(int i=0;i<TableData.Count; i++)
                {
                    float _cellHeight = GetCellHeightAtIndex(i);
                    Vector2 _cellBottom = cellTop + new Vector2(0f, -_cellHeight);

                    if((cellTop.y <= _visibleRect.y && cellTop.y >=_visibleRect.y - _visibleRect.height)
                        || (_cellBottom.y <= _visibleRect.y && _cellBottom.y >= _visibleRect.y - _visibleRect.height))
                    {
                        UIRecycleViewCell<T> cell = CreateCellForIndex(i);
                        cell.Top = cellTop;
                        break;
                    }
                    cellTop = _cellBottom + new Vector2(0f, _spacingHeight);
                }
                SetFillVisibleRectWithCells();
            }
            else
            {
                LinkedListNode<UIRecycleViewCell<T>> _node = _cells.First;
                UpdateCellForIndex(_node.Value, _node.Value.Index);
                _node = _node.Next;

                while(_node != null)
                {
                    UpdateCellForIndex(_node.Value, _node.Previous.Value.Index + 1);
                    _node.Value.Top = _node.Previous.Value.Bottom + new Vector2(0f, -_spacingHeight);
                    _node = _node.Next;
                }
                SetFillVisibleRectWithCells();
            }
        }

        protected virtual float GetCellHeightAtIndex(int index)
        {
            return CellBase.GetComponent<RectTransform>().sizeDelta.y;
        }
        
        protected void UpdateScrollViewSize()
        {
            float _contentHeight = 0f;
            for(int i=0;i<TableData.Count;i++)
            {
                _contentHeight += GetCellHeightAtIndex(i);
                if (i>0)
                {
                    _contentHeight += _spacingHeight;
                }
            }

            SizeDelta = CachedScrollRect.content.sizeDelta;
            //Vector2 _sizeDelta = new Vector2(0f, 0f);
            SizeDelta.y = _padding.top + _contentHeight + _padding.bottom;
           
            CachedScrollRect.content.sizeDelta = SizeDelta;
        }

        private UIRecycleViewCell<T> CreateCellForIndex(int index)
        {
            GameObject _obj = Instantiate(CellBase) as GameObject;
            _obj.SetActive(true);
            UIRecycleViewCell<T> _cell = _obj.GetComponent<UIRecycleViewCell<T>>();

            Vector3 _scale = _cell.transform.localScale;
            Vector2 _sizeDelta = _cell.CachedRectTransform.sizeDelta;
            Vector2 _offsetMin = _cell.CachedRectTransform.offsetMin;
            Vector2 _offsetMax = _cell.CachedRectTransform.offsetMax;

            _cell.transform.SetParent(CellBase.transform.parent);

            _cell.transform.localScale = _scale;
            _cell.CachedRectTransform.sizeDelta = _sizeDelta;
            _cell.CachedRectTransform.offsetMin = _offsetMin;
            _cell.CachedRectTransform.offsetMax = _offsetMax;

            UpdateCellForIndex(_cell, index);
            _cells.AddLast(_cell);
            return _cell;
        }

        private void UpdateCellForIndex(UIRecycleViewCell<T> cell, int index)
        {
            cell.Index = index;

            if(cell.Index >= 0 && cell.Index <= TableData.Count-1)
            {
                cell.gameObject.SetActive(true);
                cell.UpdateContent(TableData[cell.Index]);
                cell.Height = GetCellHeightAtIndex(cell.Index);
            }
            else
            {
                cell.gameObject.SetActive(false);
            }
        }

        public void UpdateVisibleRect()
        {
            _visibleRect.x = CachedScrollRect.content.anchoredPosition.x + _visibleRectPadding.left;
            _visibleRect.y = -CachedScrollRect.content.anchoredPosition.y + _visibleRectPadding.top;

            _visibleRect.width = CachedRectTransform.rect.width + _visibleRectPadding.left + _visibleRectPadding.right;
            _visibleRect.height = CachedRectTransform.rect.height + _visibleRectPadding.top + _visibleRectPadding.bottom;
        }

        private void SetFillVisibleRectWithCells()
        {
            if(_cells.Count <1)
            {
                return;
            }

            UIRecycleViewCell<T> _lastCell = _cells.Last.Value;
            int _nextCellDataIndex = _lastCell.Index + 1;
            Vector2 _nextCellTop = _lastCell.Bottom + new Vector2(0f, -_spacingHeight);

            while(_nextCellDataIndex < TableData.Count && _nextCellTop.y >= _visibleRect.y - _visibleRect.height)
            {
                UIRecycleViewCell<T> _cell = CreateCellForIndex(_nextCellDataIndex);
                _cell.Top = _nextCellTop;

                _lastCell = _cell;
                _nextCellDataIndex = _lastCell.Index + 1;
                _nextCellTop = _lastCell.Bottom + new Vector2(0f, -_spacingHeight);
            }
        }

        public void OnScrollPoschanged(Vector2 scrollPos)
        {
            UpdateVisibleRect();
            UpdateCells((scrollPos.y < _preScrollPos.y) ? 1 : -1);
            _preScrollPos = scrollPos;
        }

        public void UpdateCells(int scrollDirection)
        {
            if(_cells.Count <1)
            {
                return;
            }
            
            if (scrollDirection > 0)
            {
                UIRecycleViewCell<T> _firstCell = _cells.First.Value;
                while (_firstCell.Bottom.y > _visibleRect.y)
                {
                    UIRecycleViewCell<T> _lastCell = _cells.Last.Value;
                    UpdateCellForIndex(_firstCell, _lastCell.Index + 1);
                    _firstCell.Top = _lastCell.Bottom + new Vector2(0f, -_spacingHeight);

                    _cells.AddLast(_firstCell);
                    _cells.RemoveFirst();
                    _firstCell = _cells.First.Value;
                }
                SetFillVisibleRectWithCells();
            }
            else if (scrollDirection <0)
            {
                UIRecycleViewCell<T> _lastCell = _cells.Last.Value;
                while(_lastCell.Top.y < _visibleRect.y - _visibleRect.height)
                {
                    UIRecycleViewCell<T> firstCell = _cells.First.Value;
                    UpdateCellForIndex(_lastCell, firstCell.Index - 1);
                    _lastCell.Bottom = firstCell.Top + new Vector2(0f, _spacingHeight);

                    _cells.AddFirst(_lastCell);
                    _cells.RemoveLast();
                    _lastCell = _cells.Last.Value;
                }
            }
        }
    }
}



