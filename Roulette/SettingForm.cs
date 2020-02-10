﻿using System;
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
            Setting setting = Setting.Load();
            if(setting == null)
            {
                return;
            }
            editBegin.Text = setting.beginDiffer.ToString();
            editEnd.Text = setting.endDiffer.ToString();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Setting setting = new Setting
            {
                beginDiffer = int.Parse(editBegin.Text),
                endDiffer = int.Parse(editEnd.Text)
            };
            setting.Save();
            DialogResult = DialogResult.OK;
            Close();
        }

        public Setting GetSetting()
        {
            return new Setting
            {
                beginDiffer = int.Parse(editBegin.Text),
                endDiffer = int.Parse(editEnd.Text)
            };
        }
    }
}