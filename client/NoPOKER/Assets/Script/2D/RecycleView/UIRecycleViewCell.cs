using UnityEngine;

namespace UI
{
    [RequireComponent(typeof(RectTransform))]
    public abstract class UIRecycleViewCell<T> : MonoBehaviour
    {
        public RectTransform CachedRectTransform => GetComponent<RectTransform>();
        public int Index { get; set; }

        public float Height
        {
            get { return CachedRectTransform.sizeDelta.y; }
            set 
            {
                Vector2 _sizeDelta = CachedRectTransform.sizeDelta;
                _sizeDelta.y = value;
                CachedRectTransform.sizeDelta = _sizeDelta;
            }
        }

        public abstract void UpdateContent(T itemData);

        public Vector2 Top
        {
            get
            {
                Vector3[] _corners = new Vector3[4];
                CachedRectTransform.GetLocalCorners(_corners);
                return CachedRectTransform.anchoredPosition + new Vector2(0f, _corners[1].y);
             }
            set
            {
                Vector3[] _corners = new Vector3[4];
                CachedRectTransform.GetLocalCorners(_corners);
                CachedRectTransform.anchoredPosition = value - new Vector2(0f, _corners[1].y);
            }
        }

        public Vector2 Bottom
        {
            get
            {
                Vector3[] _corners = new Vector3[4];
                CachedRectTransform.GetLocalCorners(_corners);
                return CachedRectTransform.anchoredPosition + new Vector2(0f, _corners[3].y);
            }
            set
            {
                Vector3[] _corners = new Vector3[4];
                CachedRectTransform.GetLocalCorners(_corners);
                CachedRectTransform.anchoredPosition = value - new Vector2(0f, _corners[3].y);
            }
        }
    }
}
