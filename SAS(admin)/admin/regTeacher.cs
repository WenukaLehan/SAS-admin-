
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;

namespace SAS_admin_.admin
{
    public partial class regTeacher : Form
    {
      

        
        public regTeacher()
        {
            InitializeComponent();
            ChangeColumnType();
            this.dataGridView1.Rows.Add("1");
            this.dataGridView1.Rows.Add("2");
        }

        private void ChangeColumnType()
        {
            // Remove existing column

            // Add new column with the desired type (e.g., Button)
            DataGridViewButtonColumn newColumn = new DataGridViewButtonColumn();
            newColumn.Name = "Update";
            newColumn.HeaderText = "Update";
            newColumn.Text = "Update";
            newColumn.DefaultCellStyle.BackColor = Color.Blue;
            newColumn.UseColumnTextForButtonValue = true;
            dataGridView1.Columns.Add(newColumn);

            DataGridViewButtonColumn newColumn1 = new DataGridViewButtonColumn();
            newColumn1.Name = "Delete";
            newColumn1.HeaderText = "Delete";
            newColumn1.Text = "Delete";
            newColumn1.DefaultCellStyle.BackColor = Color.Red;
            newColumn1.DefaultCellStyle.SelectionBackColor = Color.Red;
            newColumn1.UseColumnTextForButtonValue = true;
            dataGridView1.Columns.Add(newColumn1);

            
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.Columns[e.ColumnIndex].Name == "Update")
            {
                if (MessageBox.Show("Are you sure you want to update?", "Update", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {

                }
            }

        }
    }
}
