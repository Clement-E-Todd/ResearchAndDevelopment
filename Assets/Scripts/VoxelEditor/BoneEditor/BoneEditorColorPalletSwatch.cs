using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class BoneEditorColorPalletSwatch : MonoBehaviour, IPointerClickHandler
{
	public Image backgroundImage;
	public Image colorImage;
	public Image highlightImage;

	public Sprite clearSprite;
	public Sprite lockedSprite;
	public Sprite nullSprite;
	
	public int palletIndex;
	
	void Update()
	{
		if (palletIndex == 0)
		{
			backgroundImage.sprite = lockedSprite;
			colorImage.color = Color.clear;
		}
		else if (palletIndex <= BoneEditor.Instance.CurrentBone.GetColorPalletCount())
		{
			backgroundImage.sprite = clearSprite;
			colorImage.color = BoneEditor.Instance.CurrentBone.GetColorAtPalletIndex(palletIndex).ToSystemColor();
		}
		else
		{
			backgroundImage.sprite = nullSprite;
			colorImage.color = Color.clear;
		}
		
		if (BoneEditor.Instance.palletEditor.currentSelectionIndex == palletIndex)
		{
			float color = 0.5f + Mathf.Sin(Time.time*Mathf.PI*2) / 4;
			highlightImage.color = new Color(color, color, color, 1f);
		}
		else
		{
			highlightImage.color = Color.clear;
		}
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (palletIndex <= BoneEditor.Instance.CurrentBone.GetColorPalletCount())
		{
			BoneEditor.Instance.palletEditor.currentSelectionIndex = palletIndex;
		}
	}
}
