using UnityEngine;
using System.Collections;

public class BoneEditorColorPallet : MonoBehaviour
{
	GameObject swatchPrefab;
	
	public int currentSelectionIndex = 0;
	
	public RectTransform swatchPanel;
	
	void Awake()
	{
		swatchPrefab = Resources.Load<GameObject>("Prefabs/ColorPalletSwatch");
		
		for (int i = 0; i < 16; ++i)
		{
			BoneEditorColorPalletSwatch swatch = Instantiate(swatchPrefab).GetComponent<BoneEditorColorPalletSwatch>();
			swatch.transform.SetParent(swatchPanel);
			swatch.palletIndex = i;
		}
	}
}
