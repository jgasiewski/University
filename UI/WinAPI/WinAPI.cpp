// WinAPI.cpp : Defines the entry point for the application.
//

#include "stdafx.h"
#include "WinAPI.h"

#include "Ship.hpp"
#include "Ball.hpp"

#include <string>

#define MAX_LOADSTRING 100
#define WINDOW_SIZE 1000
#define SIDE 1
#define MAX_SHOTS 2

Ship ship;
Ball ownShots[MAX_SHOTS];
Ball enemyShots[MAX_SHOTS];
int score = 0;

int msgIDCrossing[2];
int msgIDHit[2];
HWND hWndc;

// Global Variables:
HINSTANCE hInst;                                // current instance
WCHAR szTitle[MAX_LOADSTRING];                  // The title bar text
WCHAR szWindowClass[MAX_LOADSTRING];            // the main window class name

// Forward declarations of functions included in this code module:
ATOM                MyRegisterClass(HINSTANCE hInstance);
BOOL                InitInstance(HINSTANCE, int);
LRESULT CALLBACK    WndProc(HWND, UINT, WPARAM, LPARAM);
INT_PTR CALLBACK    About(HWND, UINT, WPARAM, LPARAM);

int APIENTRY wWinMain(_In_ HINSTANCE hInstance,
                     _In_opt_ HINSTANCE hPrevInstance,
                     _In_ LPWSTR    lpCmdLine,
                     _In_ int       nCmdShow)
{
    UNREFERENCED_PARAMETER(hPrevInstance);
    UNREFERENCED_PARAMETER(lpCmdLine);

	ship.init(SIDE, WINDOW_SIZE);

    // Initialize global strings
    LoadStringW(hInstance, IDS_APP_TITLE, szTitle, MAX_LOADSTRING);
    LoadStringW(hInstance, IDC_WINAPI, szWindowClass, MAX_LOADSTRING);
    MyRegisterClass(hInstance);

    // Perform application initialization:
    if (!InitInstance (hInstance, nCmdShow))
    {
        return FALSE;
    }

    HACCEL hAccelTable = LoadAccelerators(hInstance, MAKEINTRESOURCE(IDC_WINAPI));

    MSG msg;

	msgIDCrossing[0] = RegisterWindowMessage(L"Statki - przejscieL");
	msgIDCrossing[1] = RegisterWindowMessage(L"Statki - przejscieR");
	msgIDHit[0] = RegisterWindowMessage(L"Statki - trafienieL");
	msgIDHit[1] = RegisterWindowMessage(L"Statki - trafienieR");

    // Main message loop:
    while (GetMessage(&msg, nullptr, 0, 0))
    {
        if (!TranslateAccelerator(msg.hwnd, hAccelTable, &msg))
        {
            TranslateMessage(&msg);
            DispatchMessage(&msg);
        }
    }

    return (int) msg.wParam;
}

//
//  FUNCTION: MyRegisterClass()
//
//  PURPOSE: Registers the window class.
//
ATOM MyRegisterClass(HINSTANCE hInstance)
{
    WNDCLASSEXW wcex;

    wcex.cbSize = sizeof(WNDCLASSEX);

    wcex.style          = CS_HREDRAW | CS_VREDRAW;
    wcex.lpfnWndProc    = WndProc;
    wcex.cbClsExtra     = 0;
    wcex.cbWndExtra     = 0;
    wcex.hInstance      = hInstance;
    wcex.hIcon          = LoadIcon(hInstance, MAKEINTRESOURCE(IDI_WINAPI));
    wcex.hCursor        = LoadCursor(nullptr, IDC_ARROW);
    wcex.hbrBackground  = (HBRUSH)(COLOR_WINDOW+1);
    wcex.lpszMenuName   = NULL;
    wcex.lpszClassName  = szWindowClass;
    wcex.hIconSm        = LoadIcon(wcex.hInstance, MAKEINTRESOURCE(IDI_SMALL));

    return RegisterClassExW(&wcex);
}

//
//   FUNCTION: InitInstance(HINSTANCE, int)
//
//   PURPOSE: Saves instance handle and creates main window
//
//   COMMENTS:
//
//        In this function, we save the instance handle in a global variable and
//        create and display the main program window.
//
BOOL InitInstance(HINSTANCE hInstance, int nCmdShow)
{
   hInst = hInstance; // Store instance handle in our global variable

   hWndc = CreateWindow(szWindowClass, 0, WS_BORDER, 100 + 500 * SIDE, 100, 0, 0, NULL, NULL, hInstance, NULL);
   SetWindowLong(hWndc, GWL_STYLE, WS_BORDER);  // With 1 point border
   //SetWindowPos(hWnd, 0, 150, 100, 250, 250, SWP_FRAMECHANGED);

   if (!hWndc)
   {
      return FALSE;
   }

   ShowWindow(hWndc, nCmdShow);
   UpdateWindow(hWndc);

   SetTimer(hWndc, 1, 5, NULL);

   return TRUE;
}

//
//  FUNCTION: WndProc(HWND, UINT, WPARAM, LPARAM)
//
//  PURPOSE:  Processes messages for the main window.
//
//  WM_COMMAND  - process the application menu
//  WM_PAINT    - Paint the main window
//  WM_DESTROY  - post a quit message and return
//
//
LRESULT CALLBACK WndProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam)
{
	if (message == msgIDCrossing[SIDE])
	{
		for (int i = 0; i < MAX_SHOTS; ++i)
		{
			bool left = true;
			if (SIDE == 0)left = false;
			if (enemyShots[i].init(left, (int)wParam, (int)lParam))break;
		}
		return 0;
	}
	else if (message == msgIDHit[SIDE])
	{
		bool hit = wParam;
		if (hit) score++;
		ownShots[(int)lParam].active = false;
	}

    switch (message)
    {
	case WM_GETMINMAXINFO:
		{
			MINMAXINFO* mmi = (MINMAXINFO*)lParam;
			HDC hdc = GetDC(hWnd);
			mmi->ptMinTrackSize.x = WINDOW_SIZE * GetDeviceCaps(hdc, HORZRES) / GetDeviceCaps(hdc, HORZSIZE) / 10;
			mmi->ptMinTrackSize.y = WINDOW_SIZE * GetDeviceCaps(hdc, VERTRES) / GetDeviceCaps(hdc, VERTSIZE) / 10;
			mmi->ptMaxTrackSize.x = WINDOW_SIZE * GetDeviceCaps(hdc, HORZRES) / GetDeviceCaps(hdc, HORZSIZE) / 10;
			mmi->ptMaxTrackSize.y = WINDOW_SIZE * GetDeviceCaps(hdc, VERTRES) / GetDeviceCaps(hdc, VERTSIZE) / 10;
		}
		break;
    case WM_PAINT:
        {
            PAINTSTRUCT ps;
            HDC hdc = BeginPaint(hWnd, &ps);

			RECT rect;
			GetClientRect(hWnd, &rect);
			SetMapMode(hdc, MM_TEXT);

			FillRect(hdc, &rect, (HBRUSH)GetStockObject(WHITE_BRUSH));

			SetMapMode(hdc, MM_LOMETRIC);

			std::string text = std::to_string(score);
			TextOutA(hdc, WINDOW_SIZE / 2, 0, text.c_str(), text.length());

			SelectObject(hdc, GetStockObject(BLACK_BRUSH));
			Rectangle(hdc, ship.left(), ship.top(), ship.right(), ship.bottom());

			for (int i = 0; i < MAX_SHOTS; ++i)
			{
				if (ownShots[i].active)
				{
					Ellipse(hdc, ownShots[i].left(), ownShots[i].top(), ownShots[i].right(), ownShots[i].bottom());
				}
				if (enemyShots[i].active)
				{
					if (ship.collide(enemyShots[i].pos[0], enemyShots[i].pos[1]))
					{
						enemyShots[i].active = false;
						PostMessage(HWND_BROADCAST, msgIDHit[1 - SIDE], WPARAM(true), LPARAM(i));
					}
					else if (enemyShots[i].pos[0] < 0 && SIDE == 0)
					{
						enemyShots[i].active = false;
						PostMessage(HWND_BROADCAST, msgIDHit[1 - SIDE], WPARAM(false), LPARAM(i));
					}
					else if (enemyShots[i].pos[0] > WINDOW_SIZE && SIDE == 1)
					{
						enemyShots[i].active = false;
						PostMessage(HWND_BROADCAST, msgIDHit[1 - SIDE], WPARAM(false), LPARAM(i));
					}
					Ellipse(hdc, enemyShots[i].left(), enemyShots[i].top(), enemyShots[i].right(), enemyShots[i].bottom());
				}
			}

            EndPaint(hWnd, &ps);
        }
        break;
	case WM_TIMER:
		{
			static int cd = 0;

			RECT rect;
			GetClientRect(hWnd, &rect);
			int input = 0;
			if (SIDE == 0)
			{
				if (GetAsyncKeyState(VK_UP) < 0) input--;
				if (GetAsyncKeyState(VK_DOWN) < 0) input++;
				if (GetAsyncKeyState(VK_SPACE) < 0 && cd <= 0)
				{
					cd = 20;

					for (int i = 0; i < MAX_SHOTS; ++i)
					{
						if (ownShots[i].init(true, ship.pos[0], ship.pos[1]))
						{
							break;
						}
					}
				}

				ship.move(input);
				--cd;
			}

			for (int i = 0; i < MAX_SHOTS; ++i)
			{
				ownShots[i].update();
				enemyShots[i].update();

				if (!ownShots[i].reported && ownShots[i].active)
				{
					if (SIDE == 0 && ownShots[i].pos[0] >= WINDOW_SIZE)
					{
						ownShots[i].reported = true;
						PostMessage(HWND_BROADCAST, msgIDCrossing[1 - SIDE], WPARAM(WINDOW_SIZE - ownShots[i].pos[0]), LPARAM(ownShots[i].pos[1]));
					}
					else if (SIDE == 1 && ownShots[i].pos[0] <= 0)
					{
						ownShots[i].reported = true;
						PostMessage(HWND_BROADCAST, msgIDCrossing[1 - SIDE], WPARAM(WINDOW_SIZE + ownShots[i].pos[0]), LPARAM(ownShots[i].pos[1]));
					}
				}
			}

			InvalidateRect(hWnd, NULL, FALSE);
		}
		break;
	case WM_MOUSEMOVE:
		{
			if(SIDE == 1)
			{
				RECT rect;
				GetWindowRect(hWnd, &rect);
				LPPOINT mousePos = new POINT();
				GetCursorPos(mousePos);
				float y = (mousePos->y - rect.top) / (float)(rect.bottom - rect.top);
				ship.move(y);
				InvalidateRect(hWnd, NULL, FALSE);
			}
		}
		break;
	case WM_LBUTTONUP:
		{
			if (SIDE == 1)
			{
				for (int i = 0; i < MAX_SHOTS; ++i)
				{
					if (ownShots[i].init(false, ship.pos[0], ship.pos[1]))
					{
						break;
					}
				}
			}
		}
		break;
    case WM_DESTROY:
        PostQuitMessage(0);
        break;
    default:
        return DefWindowProc(hWnd, message, wParam, lParam);
    }
    return 0;
}