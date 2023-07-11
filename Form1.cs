﻿using System;
using System.Linq;
using System.Text;
using System.Data;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace MouseClickTool
{/// <summary>
/// 怎么简单怎么来了
/// </summary>
/// 
    public partial class Form1 : Form
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        //新方法：https://stackoverflow.com/questions/5094398/how-to-programmatically-mouse-move-click-right-click-and-keypress-etc-in-winfo
        internal class MouseSimulator
        {
            [DllImport("user32.dll", SetLastError = true)]
            static extern uint SendInput(uint nInputs, ref INPUT pInputs, int cbSize);

            [StructLayout(LayoutKind.Sequential)]
            struct INPUT
            {
                public SendInputEventType type;
                public MouseKeybdhardwareInputUnion mkhi;
            }

            [StructLayout(LayoutKind.Explicit)]
            struct MouseKeybdhardwareInputUnion
            {
                [FieldOffset(0)]
                public MouseInputData mi;
            }

            [Flags]
            enum MouseEventFlags : uint
            {
                MOUSEEVENTF_LEFTDOWN = 0x0002,
                MOUSEEVENTF_LEFTUP = 0x0004,
                MOUSEEVENTF_RIGHTDOWN = 0x0008,
                MOUSEEVENTF_RIGHTUP = 0x0010,
            }

            [StructLayout(LayoutKind.Sequential)]
            struct MouseInputData
            {
                public int dx;
                public int dy;
                public uint mouseData;
                public MouseEventFlags dwFlags;
                public uint time;
                public IntPtr dwExtraInfo;
            }

            enum SendInputEventType : int
            {
                InputMouse
            }

            public static void ClickLeftMouseButton()
            {
                INPUT mouseDownInput = new INPUT();
                mouseDownInput.type = SendInputEventType.InputMouse;
                mouseDownInput.mkhi.mi.dwFlags = MouseEventFlags.MOUSEEVENTF_LEFTDOWN;
                SendInput(1, ref mouseDownInput, Marshal.SizeOf(new INPUT()));

                INPUT mouseUpInput = new INPUT();
                mouseUpInput.type = SendInputEventType.InputMouse;
                mouseUpInput.mkhi.mi.dwFlags = MouseEventFlags.MOUSEEVENTF_LEFTUP;
                SendInput(1, ref mouseUpInput, Marshal.SizeOf(new INPUT()));
            }

            public static void ClickRightMouseButton()
            {
                INPUT mouseDownInput = new INPUT();
                mouseDownInput.type = SendInputEventType.InputMouse;
                mouseDownInput.mkhi.mi.dwFlags = MouseEventFlags.MOUSEEVENTF_RIGHTDOWN;
                SendInput(1, ref mouseDownInput, Marshal.SizeOf(new INPUT()));

                INPUT mouseUpInput = new INPUT();
                mouseUpInput.type = SendInputEventType.InputMouse;
                mouseUpInput.mkhi.mi.dwFlags = MouseEventFlags.MOUSEEVENTF_RIGHTUP;
                SendInput(1, ref mouseUpInput, Marshal.SizeOf(new INPUT()));
            }
        }

        private bool isTaskRunning = false; // Added boolean variable

        public Form1()
        {
            InitializeComponent();
            this.comboBox1.SelectedIndex = 0;
            is_begin.Click += (s, e) =>
            {
                if (!isTaskRunning) // Task is not running
                {
                    if (int.TryParse(is_ms.Text, out int result) && result > 0)
                    {
                        is_ms.ReadOnly = true;
                        isTaskRunning = true; // Start the task
                        is_begin.Text = "Stop";
                        Task.Factory.StartNew(async () =>
                        {
                            await Task.Run(() =>
                            {
                                for (int i = 1; i < 3; i++)
                                {
                                    if (!isTaskRunning) return; // Check if task should be stopped
                                    is_begin.Invoke((MethodInvoker)(() =>
                                    {
                                        is_begin.Text = string.Format("Start({0})", 3 - i);
                                    }));
                                    Thread.Sleep(1000);
                                }
                            });
                            if (isTaskRunning) // Check if task should be stopped
                            {
                                is_begin.Invoke((MethodInvoker)(() =>
                                {
                                    is_begin.Text = "Stop";
                                }));
                                is_ms.Invoke((MethodInvoker)(() =>
                                {
                                    is_ms.ReadOnly = false;
                                }));
                            }

                            if (this.comboBox1.SelectedIndex == 0)
                            {
                                for (; ; )
                                {
                                    if (!isTaskRunning) return; // Check if task should be stopped
                                    await Task.Run(() =>
                                    {
                                        MouseSimulator.ClickLeftMouseButton();
                                        Thread.Sleep(result);
                                    });
                                }
                            }
                            else
                            {
                                for (; ; )
                                {
                                    if (!isTaskRunning) return; // Check if task should be stopped
                                    await Task.Run(() =>
                                    {
                                        MouseSimulator.ClickRightMouseButton();
                                        Thread.Sleep(result);
                                    });
                                }
                            }
                        });
                    }
                    else
                    {
                        MessageBox.Show("Input should be an integer.");
                    }
                }
                else // Task is running, stop the task
                {
                    isTaskRunning = false;
                    is_begin.Text = "Start";
                    is_ms.ReadOnly = false;
                }
            };
        }

        private void is_begin_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}