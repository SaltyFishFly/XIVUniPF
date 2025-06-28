using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace XIVUniPF.Classes
{
    /// <summary>
    /// 为窗口提供模糊特效。
    /// 来自：Walterlv.Themes.FluentDesign / WindowAccentCompositor.cs
    /// </summary>
    public class WindowAccentCompositor
    {
        private readonly Window _window;
        private bool _isEnabled;
        private int _blurColor;

        /// <summary>
        /// 创建 <see cref="WindowAccentCompositor"/> 的一个新实例。
        /// </summary>
        /// <param name="window">要创建模糊特效的窗口实例。</param>
        public WindowAccentCompositor(Window window)
        {
            _window = window ?? throw new ArgumentNullException(nameof(window));
        }

        /// <summary>
        /// 获取或设置此窗口模糊特效是否生效的一个状态。
        /// 默认为 false，即不生效。
        /// </summary>
        [DefaultValue(false)]
        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                _isEnabled = value;
                OnIsEnabledChanged(value);
            }
        }

        /// <summary>
        /// 获取或设置此窗口模糊特效叠加的颜色。
        /// </summary>
        public Color Color
        {
            get => Color.FromArgb(
                // 取出R G B A分量。
                (byte)((_blurColor & 0x000000ff) >> 0),
                (byte)((_blurColor & 0x0000ff00) >> 8),
                (byte)((_blurColor & 0x00ff0000) >> 16),
                (byte)((_blurColor & 0xff000000) >> 24));
            set => _blurColor =
                // 组装R G B A分量。
                value.R << 0 |
                value.G << 8 |
                value.B << 16 |
                value.A << 24;
        }

        private void OnIsEnabledChanged(bool isEnabled)
        {
            Window window = _window;
            var handle = new WindowInteropHelper(window).EnsureHandle();
            Composite(handle, isEnabled);
        }

        private void Composite(IntPtr handle, bool isEnabled)
        {
            // 创建 AccentPolicy 对象。
            var accent = new AccentPolicy();

            // 设置特效。
            if (isEnabled)
            {
                accent.AccentState = AccentState.ACCENT_ENABLE_ACRYLICBLURBEHIND;
                accent.GradientColor = _blurColor;
            }
            else
            {
                accent.AccentState = AccentState.ACCENT_DISABLED;
            }

            // 将托管结构转换为非托管对象。
            var accentPolicySize = Marshal.SizeOf(accent);
            var accentPtr = Marshal.AllocHGlobal(accentPolicySize);
            Marshal.StructureToPtr(accent, accentPtr, false);

            // 设置窗口组合特性。
            try
            {
                // 设置模糊特效。
                var data = new WindowCompositionAttributeData
                {
                    Attribute = WindowCompositionAttribute.WCA_ACCENT_POLICY,
                    SizeOfData = accentPolicySize,
                    Data = accentPtr,
                };
                SetWindowCompositionAttribute(handle, ref data);
            }
            finally
            {
                // 释放非托管对象。
                Marshal.FreeHGlobal(accentPtr);
            }
        }

        [DllImport("user32.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
        private static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttributeData data);

        private enum AccentState
        {
            /// <summary>
            /// 完全禁用 DWM 的叠加特效。
            /// </summary>
            ACCENT_DISABLED = 0,
            ACCENT_ENABLE_GRADIENT = 1,
            ACCENT_ENABLE_TRANSPARENTGRADIENT = 2,
            ACCENT_ENABLE_BLURBEHIND = 3,
            ACCENT_ENABLE_ACRYLICBLURBEHIND = 4,
            ACCENT_INVALID_STATE = 5,
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct AccentPolicy
        {
            public AccentState AccentState;
            public int AccentFlags;
            public int GradientColor;
            public int AnimationId;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct WindowCompositionAttributeData
        {
            public WindowCompositionAttribute Attribute;
            public IntPtr Data;
            public int SizeOfData;
        }

        private enum WindowCompositionAttribute
        {
            // 省略其他未使用的字段
            WCA_ACCENT_POLICY = 19,
            // 省略其他未使用的字段
        }
    }
}
