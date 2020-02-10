using CefSharp;
using CefSharp.WinForms;
using Roulette.Config;
using Roulette.Gamer;
using Roulette.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Roulette
{
    public partial class MainForm : Form
    {
        private ChromiumWebBrowser browser = new ChromiumWebBrowser();
        LogForm logForm = new LogForm();
        GamerBase gamer = null;
        public MainForm()
        {
            InitializeComponent();
            WindowState = FormWindowState.Maximized;
            CefSettings settings = new CefSettings();
            settings.Locale = "zh_CN";
            settings.CefCommandLineArgs.Add("disable-gpu", "1");
            Cef.Initialize(settings);
            browser.Dock = DockStyle.Fill;
            browser.LifeSpanHandler = new OpenPageSelf();
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
            browser.Load(SiteLoader.GetInstance().GetSiteUrl(cbSupplier.Text));
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
            InternalBrowserClickDelegate d = new InternalBrowserClickDelegate(InternalBrowserClick);
            d.BeginInvoke(x, y, InternalBrowserClickCallBack, null);
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
            var host = browser.GetBrowser().GetHost();
            host.SendMouseClickEvent(x, y, MouseButtonType.Left, false, 1, CefEventFlags.None);
            host.SendMouseClickEvent(x, y, MouseButtonType.Left, true, 1, CefEventFlags.None);
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
    }
}
