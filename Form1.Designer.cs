namespace test_task
{
    partial class MainForm
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.dbResultDataGrid = new System.Windows.Forms.DataGridView();
            this.dataSet1 = new System.Data.DataSet();
            this.panel1 = new System.Windows.Forms.Panel();
            this.RUN_BUTTON = new System.Windows.Forms.Button();
            this.PickQueryComboBox = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.dbResultDataGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataSet1)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dbResultDataGrid
            // 
            this.dbResultDataGrid.AllowUserToAddRows = false;
            this.dbResultDataGrid.AllowUserToDeleteRows = false;
            this.dbResultDataGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
            this.dbResultDataGrid.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
            this.dbResultDataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dbResultDataGrid.Location = new System.Drawing.Point(12, 92);
            this.dbResultDataGrid.Name = "dbResultDataGrid";
            this.dbResultDataGrid.ReadOnly = true;
            this.dbResultDataGrid.RowTemplate.ReadOnly = true;
            this.dbResultDataGrid.Size = new System.Drawing.Size(712, 417);
            this.dbResultDataGrid.TabIndex = 0;
            // 
            // dataSet1
            // 
            this.dataSet1.DataSetName = "NewDataSet";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.RUN_BUTTON);
            this.panel1.Controls.Add(this.PickQueryComboBox);
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(712, 74);
            this.panel1.TabIndex = 2;
            // 
            // RUN_BUTTON
            // 
            this.RUN_BUTTON.Enabled = false;
            this.RUN_BUTTON.Location = new System.Drawing.Point(651, 13);
            this.RUN_BUTTON.Name = "RUN_BUTTON";
            this.RUN_BUTTON.Size = new System.Drawing.Size(47, 47);
            this.RUN_BUTTON.TabIndex = 1;
            this.RUN_BUTTON.Text = "RUN";
            this.RUN_BUTTON.UseVisualStyleBackColor = true;
            this.RUN_BUTTON.Click += new System.EventHandler(this.RunButton_OnClick);
            // 
            // PickQueryComboBox
            // 
            this.PickQueryComboBox.FormattingEnabled = true;
            this.PickQueryComboBox.ItemHeight = 13;
            this.PickQueryComboBox.Location = new System.Drawing.Point(3, 27);
            this.PickQueryComboBox.Name = "PickQueryComboBox";
            this.PickQueryComboBox.Size = new System.Drawing.Size(642, 21);
            this.PickQueryComboBox.TabIndex = 0;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(736, 521);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.dbResultDataGrid);
            this.Name = "MainForm";
            this.Text = "Тестовое задание";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_BeforeClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dbResultDataGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataSet1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dbResultDataGrid;
        private System.Data.DataSet dataSet1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button RUN_BUTTON;
        private System.Windows.Forms.ComboBox PickQueryComboBox;
    }
}

