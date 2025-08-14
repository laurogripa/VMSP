using UnityEngine;
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
using System;
using System.Runtime.InteropServices;
#endif
using Tobii.Gaming;

public class GazeMouseController : MonoBehaviour
{
	[SerializeField] private bool enableGazeMouse = true;
	[SerializeField] private bool clickOnSpacebar = true;
	[SerializeField] private float smoothing = 0f;
	[SerializeField] private KeyCode toggleGazeKey = KeyCode.E;
	[SerializeField] private bool useViewportMapping = true; // Use normalized coords from Tobii
	[SerializeField] private bool mapUsingWindowClient = true; // Map to Unity window client area for DPI/multi-monitor correctness
	[SerializeField] private bool enforceTargetAspect = true; // Fit a target aspect within the window client (handles letterboxing/pillarboxing)
	[SerializeField] private int targetAspectWidth = 16;
	[SerializeField] private int targetAspectHeight = 9;

#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
	[DllImport("user32.dll")] private static extern bool SetCursorPos(int X, int Y);
	[DllImport("user32.dll")] private static extern IntPtr GetActiveWindow();
	[DllImport("user32.dll")] private static extern IntPtr GetForegroundWindow();
	[DllImport("user32.dll")] private static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);
	[DllImport("user32.dll")] private static extern bool ClientToScreen(IntPtr hWnd, ref POINT lpPoint);
	[DllImport("user32.dll")] private static extern int GetSystemMetrics(int nIndex);

	private const int SM_XVIRTUALSCREEN = 76;
	private const int SM_YVIRTUALSCREEN = 77;
	private const int SM_CXVIRTUALSCREEN = 78;
	private const int SM_CYVIRTUALSCREEN = 79;

	private const int MOUSEEVENTF_LEFTDOWN = 0x0002;
	private const int MOUSEEVENTF_LEFTUP   = 0x0004;
	[DllImport("user32.dll")] private static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, UIntPtr dwExtraInfo);

	[StructLayout(LayoutKind.Sequential)] private struct RECT { public int left, top, right, bottom; }
	[StructLayout(LayoutKind.Sequential)] private struct POINT { public int x, y; }
#endif

	private Vector2 smoothedPos;

	private void Awake()
	{
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
		smoothedPos = new Vector2(0.5f, 0.5f);
	}

	private void Update()
	{
		if (Input.GetKeyDown(toggleGazeKey))
		{
			enableGazeMouse = !enableGazeMouse;
		}

		if (!enableGazeMouse) { return; }

		var gazePoint = TobiiAPI.GetGazePoint();
		if (gazePoint.IsRecent())
		{
			Vector2 targetViewport = useViewportMapping ? gazePoint.Viewport : new Vector2(gazePoint.Screen.x / Screen.width, gazePoint.Screen.y / Screen.height);

			if (smoothing > 0f)
			{
				smoothedPos = Vector2.Lerp(smoothedPos, targetViewport, 1f - Mathf.Exp(-smoothing * Time.deltaTime));
			}
			else
			{
				smoothedPos = targetViewport;
			}

#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
			int finalX;
			int finalY;

			bool mapped = false;
			if (mapUsingWindowClient)
			{
				IntPtr hwnd = GetActiveWindow();
				if (hwnd == IntPtr.Zero) { hwnd = GetForegroundWindow(); }
				if (hwnd != IntPtr.Zero && GetClientRect(hwnd, out RECT r))
				{
					int clientW = Mathf.Max(1, r.right - r.left);
					int clientH = Mathf.Max(1, r.bottom - r.top);
					POINT origin = new POINT { x = 0, y = 0 };
					if (ClientToScreen(hwnd, ref origin))
					{
						float vx = Mathf.Clamp01(smoothedPos.x);
						float vy = Mathf.Clamp01(smoothedPos.y);

						int contentW = clientW;
						int contentH = clientH;
						int offsetX = 0;
						int offsetY = 0;

						if (enforceTargetAspect && targetAspectWidth > 0 && targetAspectHeight > 0)
						{
							float targetAspect = (float)targetAspectWidth / targetAspectHeight;
							float clientAspect = (float)clientW / clientH;
							if (clientAspect > targetAspect)
							{
								contentW = Mathf.RoundToInt(clientH * targetAspect);
								offsetX = (clientW - contentW) / 2;
								contentH = clientH;
								offsetY = 0;
							}
							else if (clientAspect < targetAspect)
							{
								contentW = clientW;
								contentH = Mathf.RoundToInt(clientW / targetAspect);
								offsetX = 0;
								offsetY = (clientH - contentH) / 2;
							}
						}

						int cx = offsetX + Mathf.RoundToInt(vx * (contentW - 1));
						int cy = offsetY + Mathf.RoundToInt((1f - vy) * (contentH - 1));
						finalX = origin.x + cx;
						finalY = origin.y + cy;
						SetCursorPos(finalX, finalY);
						mapped = true;
					}
				}
			}

			if (!mapped)
			{
				int vLeft = GetSystemMetrics(SM_XVIRTUALSCREEN);
				int vTop = GetSystemMetrics(SM_YVIRTUALSCREEN);
				int vWidth = Mathf.Max(1, GetSystemMetrics(SM_CXVIRTUALSCREEN));
				int vHeight = Mathf.Max(1, GetSystemMetrics(SM_CYVIRTUALSCREEN));

				float vx = Mathf.Clamp01(smoothedPos.x);
				float vy = Mathf.Clamp01(smoothedPos.y);
				finalX = vLeft + Mathf.RoundToInt(vx * (vWidth - 1));
				finalY = vTop + Mathf.RoundToInt((1f - vy) * (vHeight - 1));
				SetCursorPos(finalX, finalY);
			}
#endif
		}

		if (clickOnSpacebar && Input.GetKeyDown(KeyCode.Space))
		{
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
			mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, 0, 0, 0, UIntPtr.Zero);
#endif
		}
	}
}
