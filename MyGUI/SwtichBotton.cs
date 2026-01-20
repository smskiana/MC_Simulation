
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MyGUI
{
    public class SwtichBotton : MonoBehaviour, IPointerDownHandler
    {

        public Image Image;
        public Image OnClickImage;
        public Color OnClickColor = Color.gray;
        public bool IsClicked = true;

        public void OnEnable()
        {
            Image = GetComponent<Image>();
            if (IsClicked)
            {
                Enable();
            }
            else
            {
               Disable();
            }

        }      
        public  void OnPointerDown(PointerEventData eventData)
        {
            Image.color = OnClickColor;
            IsClicked = !IsClicked;
            if (IsClicked)
            {
                Enable();
            }
            else
            {
                Disable();
            }
        }
        protected virtual void Enable()
        {
            if (Image != null) Image.color = Color.white;
            if (OnClickImage != null) OnClickImage.gameObject.SetActive(false);
        }
        protected virtual void Disable()
        {
            if (Image != null) Image.color = OnClickColor;
            if (OnClickImage != null) OnClickImage.gameObject.SetActive(true);
        }
    }
}
