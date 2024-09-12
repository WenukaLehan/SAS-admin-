

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Xml.Linq;

using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using MessageBox = System.Windows.Forms.MessageBox;

namespace SAS_admin_.teacher
{
    public partial class attendence : Form
    {
        public static attendence instance;
        public Label tb;
        

        
        DataTable dt = new DataTable();

        public attendence()
        {
            InitializeComponent();
            
        }

        
        private void iconButton1_Click(object sender, EventArgs e)
        {
           
        }

        

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            
        }


        

    }
}
