using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class ButtonSoundScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
   public AudioSource pointerEnterAudio;

   public AudioSource pointerUpAudio;

   public void OnPointerEnter(PointerEventData eventData)
   {
      pointerEnterAudio.Play();
      gameObject.transform.localScale = new Vector2(1.1f, 1.1f);
      gameObject.transform.eulerAngles = new Vector3(0f, 0f, Random.Range(-3.0f, 3.0f));
   }

   public void OnPointerExit(PointerEventData eventData)
   {
      gameObject.transform.localScale = new Vector2(1.0f, 1.0f);
      gameObject.transform.eulerAngles = Vector3.zero;
   }

   public void OnPointerDown(PointerEventData eventData)
   {
      pointerUpAudio.Play();
   }
}