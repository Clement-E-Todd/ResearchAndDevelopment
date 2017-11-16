using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;

public class EditorButton : MonoBehaviour, IPointerDownHandler
{
	public UnityEvent onClickStart;
	public UnityEvent onClickHeld;
	public UnityEvent onClickEnd;
	
	bool leftButtonDown;

	public void OnPointerDown(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Left)
		{
			onClickStart.Invoke();
			leftButtonDown = true;
		}
	}
	
	void Update()
	{
		if (leftButtonDown)
		{
			if (Input.GetMouseButton(0))
			{
				onClickHeld.Invoke();
			}
			else
			{
				onClickEnd.Invoke();
				leftButtonDown = false;
			}
		}
	}
}
