using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(FPSCounter))]
public class FPSDisplay : MonoBehaviour
{
	public Text fpsLabel;

	FPSCounter fpsCounter;

	private void Awake()
	{
		fpsCounter = GetComponent<FPSCounter>();
	}

	private void Update()
	{
		fpsLabel.text = fpsCounter.FPS.ToString();
	}
}