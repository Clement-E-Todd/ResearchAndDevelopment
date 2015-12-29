using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class FoxyDataIO
{
	static List<char> streamData;

	enum StreamStatus
	{
		Closed,
		Writing,
		Reading
	}

	static StreamStatus streamStatus = StreamStatus.Closed;
	
	public static void StartWriting()
	{
		if (streamStatus == StreamStatus.Closed)
		{
			streamStatus = StreamStatus.Writing;
			
			streamData = new List<char>();
		}
		else
		{
			Debug.LogError("Cannot start writing; IO stream is already open with status of \"" + streamStatus + "\"!");
		}
	}
	
	public static void StartReading(char[] data)
	{
		if (streamStatus == StreamStatus.Closed)
		{
			streamStatus = StreamStatus.Writing;
			
			streamData = new List<char>(data);
		}
		else
		{
			Debug.LogError("Cannot start reading; IO stream is already open with status of \"" + streamStatus + "\"!");
		}
	}
	
	public static void WriteData(object data, int maxBits)
	{
		
	}
}
