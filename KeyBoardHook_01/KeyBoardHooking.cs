using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KeyBoardHook_01
{
    internal class KeyBoardHooking
    {

        [DllImport("user32.dll")]
        static extern IntPtr SetWindowsHookEx(int idHook, keyboardHookProc callback, IntPtr hInstance, uint threadId);
        [DllImport("user32.dll")]
        static extern bool UnhookWindowsHookEx(IntPtr hInstance);
        [DllImport("user32.dll")]
        static extern int CallNextHookEx(IntPtr idHook, int nCode, int wParam, ref keyboardHookStruct IParam);
        [DllImport("user32.dll")]
        static extern short GetKeyState(int nCode);
        [DllImport("kernel32.dll")]
        static extern IntPtr LoadLibrary(string IpFileName); // 라이브러리 등록

        public delegate int keyboardHookProc(int code, int wParam, ref keyboardHookStruct IParam);     // callback Delegate

        // keyboardHookStruct 구조체 정의
        public struct keyboardHookStruct
        {
            public int vkCode;
            public int scanCode;
            public int flags;
            public int time;
            public int dwExtraInfo;
        }

        // 정의 되어 있는 상수 값
        const int VK_SHIFT = 0x10;
        const int VK_CONTROL = 0x11;
        const int VK_MENU = 0x12;

        const int WH_KEYBOARD_LL = 13;
        const int WM_KEYDOWN = 0x100;
        const int WM_KEYUP = 0x101;
        const int WM_SYSKEYDOWN = 0x104;
        const int WM_SYSKEYUP = 0x105;

        private keyboardHookProc khp;
        IntPtr hhook = IntPtr.Zero; // 포인터나 핸들 초기화 "hhook"핸들을 리턴 하기 위한 변수

        public event KeyEventHandler KeyDown;
        public event KeyEventHandler KeyUp;

        public KeyBoardHooking()
        {
            khp = new keyboardHookProc(hookProc); // 델리게이트 이벤트 등록
            //hook(); // DLL등록
        }

        private int hookProc(int code, int wParam, ref keyboardHookStruct IParam)
        {
            if (code >= 0)
            {
                Keys key = (Keys)IParam.vkCode;
                if ((GetKeyState(VK_CONTROL) & 0x80) != 0)
                    key |= Keys.Control;
                if ((GetKeyState(VK_MENU) & 0x80) != 0)
                    key |= Keys.Alt;
                if ((GetKeyState(VK_SHIFT) & 0x80) != 0)
                    key |= Keys.Shift;

                KeyEventArgs kea = new KeyEventArgs(key);

                if ((wParam == WM_KEYDOWN || wParam == WM_SYSKEYDOWN) && (KeyDown != null))
                {
                    KeyDown(this, kea);
                }
                else if ((wParam == WM_KEYUP || wParam == WM_SYSKEYUP) && (KeyUp != null))
                {
                    KeyUp(this, kea);
                }
                if (kea.Handled) return 1;

            }
            return CallNextHookEx(hhook, code, wParam, ref IParam);
        }

        public void hook()
        {
            IntPtr hInstance = LoadLibrary("user32.dll");
            hhook = SetWindowsHookEx(WH_KEYBOARD_LL, khp, hInstance, 0);
        }

        private void unHook()
        {
            UnhookWindowsHookEx(hhook);
        }
    }
}
