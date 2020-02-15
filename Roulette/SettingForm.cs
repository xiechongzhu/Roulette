using Roulette.Config;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Roulette
{
    public partial class SettingForm : Form
    {
        public SettingForm()
        {
            InitializeComponent();
            loadSetting();
        }

        private void loadSetting()
        {
            foreach(String key  in Tables.GetInstance().tablePoints.Keys)
            {
                comboBox.Items.Add(key);
            }
            
            Setting setting = Setting.Load();
            if(setting == null)
            {
                return;
            }
            editBegin.Text = setting.diff.ToString();
            editWin.Text = setting.win.ToString();
            comboBox.Text = setting.tableName;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Setting setting = new Setting
            {
                diff = int.Parse(editBegin.Text),
                win = int.Parse(editWin.Text),
                tableName = comboBox.Text
            };
            setting.Save();
            DialogResult = DialogResult.OK;
            Close();
        }

        public Setting GetSetting()
        {
            return new Setting
            {
                diff = int.Parse(editBegin.Text),
                win = int.Parse(editWin.Text)
            };
        }
    }
}
