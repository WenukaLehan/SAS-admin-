﻿namespace SAS_admin_.attender
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.comboBoxComPorts = new System.Windows.Forms.ComboBox();
            this.button1 = new System.Windows.Forms.Button();
            this.attendenceGrid = new System.Windows.Forms.DataGridView();
            this.button2 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.attendenceGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // comboBoxComPorts
            // 
            this.comboBoxComPorts.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboBoxComPorts.FormattingEnabled = true;
            this.comboBoxComPorts.ItemHeight = 20;
            this.comboBoxComPorts.Location = new System.Drawing.Point(81, 62);
            this.comboBoxComPorts.Name = "comboBoxComPorts";
            this.comboBoxComPorts.Size = new System.Drawing.Size(290, 28);
            this.comboBoxComPorts.TabIndex = 0;
            this.comboBoxComPorts.Click += new System.EventHandler(this.comboBoxComPorts_Click);
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(471, 54);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(122, 36);
            this.button1.TabIndex = 1;
            this.button1.Text = "Start";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // attendenceGrid
            // 
            this.attendenceGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.attendenceGrid.Location = new System.Drawing.Point(81, 143);
            this.attendenceGrid.Name = "attendenceGrid";
            this.attendenceGrid.RowHeadersWidth = 51;
            this.attendenceGrid.RowTemplate.Height = 24;
            this.attendenceGrid.Size = new System.Drawing.Size(1263, 600);
            this.attendenceGrid.TabIndex = 2;
            // 
            // button2
            // 
            this.button2.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button2.Location = new System.Drawing.Point(631, 54);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(122, 36);
            this.button2.TabIndex = 3;
            this.button2.Text = "Stop";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1376, 792);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.attendenceGrid);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.comboBoxComPorts);
            this.Name = "Form1";
            this.Text = "Attendence Marking";
            ((System.ComponentModel.ISupportInitialize)(this.attendenceGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBoxComPorts;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.DataGridView attendenceGrid;
        private System.Windows.Forms.Button button2;
    }
}