using System;
using UnityEngine;

public static class GameUtility
{
    private static Vector2 _LastMousePos;
	public static float GetGameTimeDelta()
	{
		return Time.deltaTime;
	}
    public static float GetGameTime()
    {
        return Time.time;
    }
    public static ref Vector2 GetMousePos()
    {
        if (Input.mousePosition.x < 0 || Input.mousePosition.x > Screen.width)
            return ref _LastMousePos;
        if (Input.mousePosition.y < 0 || Input.mousePosition.y > Screen.height)
            return ref _LastMousePos;
        _LastMousePos = Input.mousePosition;
        return ref _LastMousePos;
    }
}

