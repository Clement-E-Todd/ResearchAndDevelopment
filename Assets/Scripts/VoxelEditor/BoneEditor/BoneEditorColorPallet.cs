using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BoneEditorColorPallet : MonoBehaviour
{
	GameObject swatchPrefab;
	
	public int currentSelectionIndex = 0;
	
	public RectTransform swatchPanel;
	
	public Text redText;
	public Text greenText;
	public Text blueText;
	public Text alphaText;
	
	VoxelColor ColorFromCurrentSelection
	{
		get
		{
			return BoneEditor.Instance.CurrentBone.GetColorAtPalletIndex(currentSelectionIndex);
		}
		
		set
		{
			BoneEditor.Instance.CurrentBone.SetColorAtPalletIndex(currentSelectionIndex, value);
		}
	}
	
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
	
	void Update()
	{
		if (currentSelectionIndex > 0)
		{
			VoxelColor color = ColorFromCurrentSelection;
			
			redText.text = "R: " + color.r.ToString();
			greenText.text = "G: " + color.g.ToString();
			blueText.text = "B: " + color.b.ToString();
			alphaText.text = "A: " + color.a.ToString();
		}
		else
		{
			redText.text = "R: -";
			greenText.text = "G: -";
			blueText.text = "B: -";
			alphaText.text = "A: -";
		}
	}
	
	public void OnNewPressed()
	{
		BoneEditor.Instance.CurrentBone.CreateNewColorInPallet();
		
		currentSelectionIndex = BoneEditor.Instance.CurrentBone.GetColorPalletCount();
	}
	
	public void OnDeletePressed()
	{
		if (currentSelectionIndex != 0)
		{
			BoneEditor.Instance.CurrentBone.RemoveColorAtPalletIndex(currentSelectionIndex);
			
			currentSelectionIndex = 0;
		}
	}
	
	public void OnIndexUpPressed()
	{
		if (currentSelectionIndex < BoneEditor.Instance.CurrentBone.GetColorPalletCount())
		{
			BoneEditor.Instance.CurrentBone.IncreaseIndexOfColorAtPalletIndex(currentSelectionIndex);
		
			++currentSelectionIndex;
		}
	}
	
	public void OnIndexDownPressed()
	{
		if (currentSelectionIndex > 1)
		{
			BoneEditor.Instance.CurrentBone.DecreaseIndexOfColorAtPalletIndex(currentSelectionIndex);
		
			--currentSelectionIndex;
		}
	}
	
	public void OnRedUpPressed()
	{
		if (currentSelectionIndex != 0)
		{
			VoxelColor color = ColorFromCurrentSelection;
			
			if (color.r < 15)
			{
				++color.r;
				ColorFromCurrentSelection = color;
			}
		}
	}
	
	public void OnRedDownPressed()
	{
		if (currentSelectionIndex != 0)
		{
			VoxelColor color = ColorFromCurrentSelection;
			
			if (color.r > 0)
			{
				--color.r;
				ColorFromCurrentSelection = color;
			}
		}
	}
	
	public void OnGreenUpPressed()
	{
		if (currentSelectionIndex != 0)
		{
			VoxelColor color = ColorFromCurrentSelection;
			
			if (color.g < 15)
			{
				++color.g;
				ColorFromCurrentSelection = color;
			}
		}
	}
	
	public void OnGreenDownPressed()
	{
		if (currentSelectionIndex != 0)
		{
			VoxelColor color = ColorFromCurrentSelection;
			
			if (color.g > 0)
			{
				--color.g;
				ColorFromCurrentSelection = color;
			}
		}
	}
	
	public void OnBlueUpPressed()
	{
		if (currentSelectionIndex != 0)
		{
			VoxelColor color = ColorFromCurrentSelection;
			
			if (color.b < 15)
			{
				++color.b;
				ColorFromCurrentSelection = color;
			}
		}
	}
	
	public void OnBlueDownPressed()
	{
		if (currentSelectionIndex != 0)
		{
			VoxelColor color = ColorFromCurrentSelection;
			
			if (color.b > 0)
			{
				--color.b;
				ColorFromCurrentSelection = color;
			}
		}
	}
	
	public void OnAlphaUpPressed()
	{
		if (currentSelectionIndex != 0)
		{
			VoxelColor color = ColorFromCurrentSelection;
			
			if (color.a < 15)
			{
				++color.a;
				ColorFromCurrentSelection = color;
			}
		}
	}
	
	public void OnAlphaDownPressed()
	{
		if (currentSelectionIndex != 0)
		{
			VoxelColor color = ColorFromCurrentSelection;
			
			if (color.a > 0)
			{
				--color.a;
				ColorFromCurrentSelection = color;
			}
		}
	}
}
