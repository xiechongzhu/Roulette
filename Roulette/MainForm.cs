using Roulette.Config;
using Roulette.Gamer;
using Roulette.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Roulette
{
    public partial class MainForm : Form
    {
        private DateTime lastReleaseMemoryTime = DateTime.Now;
        private NewWebBrowser browser = new NewWebBrowser();
        LogForm logForm = new LogForm();
        GamerBase gamer = null;
        public MainForm()
        {
            InitializeComponent();
            browser.ScriptErrorsSuppressed = true;
            WindowState = FormWindowState.Maximized;
            browser.Dock = DockStyle.Fill;
            panelWeb.Controls.Add(browser);

            String errMsg = null;
            if(!SiteLoader.GetInstance().LoadConfig(ref errMsg))
            {
                MessageBox.Show(errMsg, "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            foreach(SiteInfo siteInfo in SiteLoader.GetInstance().GetSiteList())
            {
                cbSupplier.Items.Add(siteInfo.Name);
            }
            cbSupplier.SelectedIndex = 0;

            browser.BeforeNewWindow2 += Browser_BeforeNewWindow2;
        }

        private void Browser_BeforeNewWindow2(WebBrowserUrl2 webPra, WebBrowserEvent cancel)
        {
            if(webPra != null)
            {
                browser.Navigate(webPra.Url);
                cancel.cancel = true;
            }
        }

        private void btnCapture_Click(object sender, EventArgs e)
        {
            Image image = BitmapCapture.GetWindowCapture(panelWeb);
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "JPEG图片(*.jpg)|*.jpg";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                image.Save(dialog.FileName);
            }
            image.Dispose();
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            browser.Navigate(SiteLoader.GetInstance().GetSiteUrl(cbSupplier.Text));
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            switch(SiteLoader.GetInstance().GetSupplierType(cbSupplier.Text))
            {
                case SupplierType.SUPPLIER_AG:
                    gamer = new GamerAG(this);
                    break;
                default:
                    break;
            }
            if (gamer != null)
            {
                WindowState = FormWindowState.Normal;
                Size = new Size(1024, 768);
                ImageCapTimer.Start();
                gamer.Start();
                btnStart.Enabled = false;
                btnStop.Enabled = true;
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            ImageCapTimer.Stop();
            gamer.Stop();
            btnStart.Enabled = true;
            btnStop.Enabled = false;
        }

        public void BrowserClick(Int32 x, Int32 y)
        {
            if(InvokeRequired)
            {
                Invoke(new InternalBrowserClickDelegate(InternalBrowserClick), x, y);
            }
            else
            {
                InternalBrowserClick(x, y);
            }
        }

        protected void InternalBrowserClickCallBack(IAsyncResult result)
        {
            AsyncResult _result = (AsyncResult)result;
            InternalBrowserClickDelegate d = (InternalBrowserClickDelegate)_result.AsyncDelegate;
            d.EndInvoke(result);
        }

        delegate void InternalBrowserClickDelegate(Int32 x, Int32 y);
        protected void InternalBrowserClick(Int32 x, Int32 y)
        {
            IntPtr handle = browser.Handle;
            StringBuilder className = new StringBuilder(100);
            while (className.ToString() != "Internet Explorer_Server") // The class control for the browser
            {
                handle = WinApi.GetWindow(handle, 5); // Get a handle to the child window
                WinApi.GetClassName(handle, className, className.Capacity);
            }
            IntPtr lParam = (IntPtr)((y << 16) | x); // The coordinates
            IntPtr wParam = IntPtr.Zero; // Additional parameters for the click (e.g. Ctrl)
            const uint downCode = 0x201; // Left click down code
            const uint upCode = 0x202; // Left click up code
            WinApi.SendMessage(handle, downCode, wParam, lParam); // Mouse button down
            WinApi.SendMessage(handle, upCode, wParam, lParam); // Mouse button up
        }

        delegate void SendLogDelegate(String logString);
        public void addLog(String logString)
        {
            if (InvokeRequired)
            {
                Invoke(new SendLogDelegate(addLog), logString);
            }
            else
            {
                logForm.AddLog(logString);
            }
        }

        private void ImageCapTimer_Tick(object sender, EventArgs e)
        {
            Image image = BitmapCapture.GetWindowCapture(panelWeb);
            gamer.ParseImage(image);
            DateTime now = DateTime.Now;
            if(now > lastReleaseMemoryTime.AddSeconds(10))
            {
                IntPtr pHandle = WinApi.GetCurrentProcess();
                WinApi.SetProcessWorkingSetSize(pHandle, -1, -1);
                lastReleaseMemoryTime = now;
            }
        }

        private void btnLog_Click(object sender, EventArgs e)
        {
            logForm.Visible = true;
            logForm.BringToFront();
        }

        private void btnSetting_Click(object sender, EventArgs e)
        {
            SettingForm settingForm = new SettingForm();
            if(DialogResult.OK == settingForm.ShowDialog())
            {
                gamer?.SetSetting(settingForm.GetSetting());
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(gamer != null && gamer.IsRunning)
            {
                MessageBox.Show("请先停止", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                e.Cancel = true;
            }
        }
    }
}
