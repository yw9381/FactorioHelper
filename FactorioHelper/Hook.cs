using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using System.Windows.Interop;
using FactorioHelper;

namespace Hook
{
    public class GameWindowCheck
    {
        public int GameWindowHwnd = 0;
        public int InputWindowHwnd = 0;

        [DllImport("User32.dll", EntryPoint = "FindWindow")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", EntryPoint = "GetForegroundWindow")]
        public static extern IntPtr GetForegroundWindow();

    }
    public class KeyBoardHook : GameWindowCheck
    {
        [StructLayout(LayoutKind.Sequential)]
        public class KeyBoardHookStruct
        {
            public int vkCode;
            public int scanCode;
            public int flags;
            public int time;
            public int dwExtraInfo;
        }

        // 装置钩子的函数  
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, int threadId);

        // 卸下钩子的函数  
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern bool UnhookWindowsHookEx(int idHook);

        // 下一个钩挂的函数  
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int CallNextHookEx(int idHook, int nCode, Int32 wParam, IntPtr lParam);

        // 钩子回调函数  
        public delegate int HookProc(int nCode, Int32 wParam, IntPtr lParam);

        // 取得当前线程编号（线程钩子需要用到）
        [DllImport("kernel32.dll")]
        static extern int GetCurrentThreadId();

        //使用WINDOWS API函数代替获取当前实例的函数,防止钩子失效
        [DllImport("kernel32.dll")]
        public static extern IntPtr GetModuleHandle(string name);

        // 全局的键盘事件  
        //public event System.Windows.Input.KeyboardEventHandler OnKeyboardActivity;
        public event KeyEventHandler OnKeyDownEvent;
        //public event KeyEventHandler OnKeyUpEvent;
        //public event KeyPressEventHandler OnKeyPressEvent;

        //获取按键的状态
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int GetKeyboardState(byte[] pbKeyState);

       [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern short GetKeyState(int vKey);

        public const int WH_KEYBOARD_LL = 13;
        public const int WM_KEYDOWN = 0x100;
        public const int WM_KEYUP = 0x101;
        public const int WM_SYSKEYDOWN = 0x104;
        public const int WM_SYSKEYUP = 0x105;

        // 声明键盘钩子事件类型
        private HookProc KeyBoardHookProcedure;
        // 键盘钩子句柄
        private static int kbHook = 0;
        //存放被按下的控制键，用来生成具体的键
        //private List<Keys> preKeysList = new List<Keys>();

        public void HookStart()
        {
            if (kbHook == 0)
            {
                KeyBoardHookProcedure = new HookProc(KeyBoardHookProc);
                //kbHook = SetWindowsHookEx(WH_KEYBOARD_LL, KeyBoardHookProcedure, Marshal.GetHINSTANCE(System.Reflection.Assembly.GetExecutingAssembly().GetModules()[0]), 0);
                kbHook = SetWindowsHookEx(WH_KEYBOARD_LL, KeyBoardHookProcedure, GetModuleHandle(Process.GetCurrentProcess().MainModule.ModuleName), 0);

                //如果设置钩子失败.   
                if (kbHook == 0)
                {
                    HookStop();
                    throw new Exception("SetWindowsHookEx failed.");
                }
            }

        }
        public void HookStop()
        {
            bool retKeyboard = true;
            if (kbHook != 0)
            {
                retKeyboard = UnhookWindowsHookEx(kbHook);
                kbHook = 0;
            }
            // 如果卸下钩子失败  
            if (!retKeyboard)
                throw new Exception("UnhookWindowsHookEx failed.");
        }

        public int KeyBoardHookProc(int nCode, int wParam, IntPtr lParam)
        {
            // 侦听键盘事件
            if (nCode >= 0)
            {
                KeyBoardHookStruct MyKeyBoardHookStruct = (KeyBoardHookStruct)Marshal.PtrToStructure(lParam, typeof(KeyBoardHookStruct));
                if (OnKeyDownEvent != null && (wParam == WM_KEYDOWN || wParam == WM_SYSKEYDOWN))
                {
                    Keys key = (Keys)MyKeyBoardHookStruct.vkCode;
                    KeyEventArgs e = new KeyEventArgs(key);
                    OnKeyDownEvent(this, e);
                }

                // 这里写按下后做什么事  
                if (MyKeyBoardHookStruct.flags == 0)
                {
                    switch (MyKeyBoardHookStruct.vkCode)
                    {
                        case (int)Keys.Oem3:    // ~ 键
                            // 打开输入框
                            //MessageBox.Show("~键");
                            if (GameWindowHwnd == GetForegroundWindow().ToInt32())
                            {

                                //InputWindow.Show();
                            }
                            else
                            {
                                if (InputWindowHwnd == GetForegroundWindow().ToInt32())
                                {
                                }
                            }
                            break;
                        case (int)Keys.Enter:   // 回车键
                            // 如果输入框打开且有字则发送并关闭
                            MessageBox.Show("回车键");
                            return 1;
                        case (int)Keys.Escape:  // Esc键
                            // 如果输入框打开则关闭
                            MessageBox.Show("Esc键");
                            break;
                        default:
                            // MessageBox.Show(MyKeyBoardHookStruct.vkCode.ToString());
                            break;
                    }
                }
                // 这里写弹起后做什么事
                if (MyKeyBoardHookStruct.flags == 128)
                {
                    switch (MyKeyBoardHookStruct.vkCode)
                    {
                        default:
                            break;
                    }

                }
            }
            return CallNextHookEx(kbHook, nCode, wParam, lParam);
        }
    }
}
