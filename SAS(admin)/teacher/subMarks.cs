﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SAS_admin_.teacher
{
    public partial class subMarks : Form
    {
        public subMarks()
        {
            InitializeComponent();
        }

        string teacherName, teacherId;
        public void add(string tName, string tId)
        {
            teacherName = tName;
            teacherId = tId;
            MessageBox.Show("Comming son with next version of software..,", "information", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }
    }
}
