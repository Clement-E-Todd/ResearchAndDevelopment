using UnityEngine;
using System.Collections;

public class DirectionalColorView : MonoBehaviour
{
	[Range(0.001f, 2.0f)]
	public float viewDepth = 0.5f;

    private ComputeShader computeShader;
    private RenderTexture renderTexture;

    int kernelHandle = -1;

    struct ViewInfo
    {
		public ViewInfo(Vector3 position, Vector3 forward, Vector3 right, float viewDepth, int width, int height)
    	{
			this.position = position;
			this.forward = forward;
			this.right = right;
			
    		this.viewDepth = viewDepth;
    		
    		this.width = width;
    		this.height = height;
    	}
    
		public Vector3 position;
		public Vector3 forward;
		public Vector3 right;
		
        public float viewDepth;
        
        public int width;
        public int height;
        
        public const int sizeInBytes = sizeof(float) * (3 + 3 + 3 + 1) + sizeof(int) * 2;
    }

    void Awake()
    {
        // Load the shader
        computeShader = Resources.Load<ComputeShader>("DirectionalColorShader");

        // Get a handle to the method we'll be calling in the shader
        kernelHandle = computeShader.FindKernel("UpdateView");

        // Create the render texture
        renderTexture = new RenderTexture(32, 18, 16);
        renderTexture.filterMode = FilterMode.Point;
        renderTexture.enableRandomWrite = true;
        renderTexture.Create();

        // For the main method of the shader, set the readable/writable texture named "Result" to our render texture
		computeShader.SetTexture(kernelHandle, "viewTexture", renderTexture);
    }

    void FixedUpdate()
    {
        // Provide new values to the shader
		ViewInfo[] viewInfo = new ViewInfo[1];
		viewInfo[0] = new ViewInfo(transform.position,
								   transform.forward,
								   transform.right,
								   viewDepth,
								   renderTexture.width,
								   renderTexture.height);
		
		ComputeBuffer viewInfoBuffer = new ComputeBuffer(1, ViewInfo.sizeInBytes);
		viewInfoBuffer.SetData(viewInfo);
		computeShader.SetBuffer(kernelHandle, "viewInfoBuffer", viewInfoBuffer);

        // Run the shader to update the render texture
		computeShader.Dispatch(kernelHandle, 16, 9, 1);
        
        // Clean up
        viewInfoBuffer.Release();
    }

    void OnGUI()
    {
        // Draw the render texture to the screen
        if (Event.current.type.Equals(EventType.Repaint))
        {
            Graphics.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), renderTexture);
        }
    }
}
