namespace RKYC.UI.Windows.HIMS.Nurse.Common
{
    partial class InputMethodForm
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
            this.gridControl_InputData = new DevExpress.XtraGrid.GridControl();
            this.gridView_inputSettingData = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.labelControl_inputText = new DevExpress.XtraEditors.LabelControl();
            this.textEdit_inputText = new DevExpress.XtraEditors.TextEdit();
            this.comboBoxEdit_InputType = new DevExpress.XtraEditors.ComboBoxEdit();
            this.simpleButton_refresh = new DevExpress.XtraEditors.SimpleButton();
            this.labelControl_hospital = new DevExpress.XtraEditors.LabelControl();
            this.comboBoxEdit_hospital = new DevExpress.XtraEditors.ComboBoxEdit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl_InputData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView_inputSettingData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit_inputText.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.comboBoxEdit_InputType.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.comboBoxEdit_hospital.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // gridControl_InputData
            // 
            this.gridControl_InputData.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(4);
            this.gridControl_InputData.Location = new System.Drawing.Point(4, 124);
            this.gridControl_InputData.MainView = this.gridView_inputSettingData;
            this.gridControl_InputData.Margin = new System.Windows.Forms.Padding(4);
            this.gridControl_InputData.Name = "gridControl_InputData";
            this.gridControl_InputData.Size = new System.Drawing.Size(910, 780);
            this.gridControl_InputData.TabIndex = 0;
            this.gridControl_InputData.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView_inputSettingData});
            // 
            // gridView_inputSettingData
            // 
            this.gridView_inputSettingData.GridControl = this.gridControl_InputData;
            this.gridView_inputSettingData.Name = "gridView_inputSettingData";
            this.gridView_inputSettingData.OptionsBehavior.Editable = false;
            this.gridView_inputSettingData.OptionsView.ShowGroupPanel = false;
            this.gridView_inputSettingData.DoubleClick += new System.EventHandler(this.gridView_inputSettingData_DoubleClick);
            // 
            // labelControl_inputText
            // 
            this.labelControl_inputText.Appearance.Font = new System.Drawing.Font("Tahoma", 12F);
            this.labelControl_inputText.Location = new System.Drawing.Point(318, 46);
            this.labelControl_inputText.Margin = new System.Windows.Forms.Padding(4);
            this.labelControl_inputText.Name = "labelControl_inputText";
            this.labelControl_inputText.Size = new System.Drawing.Size(72, 29);
            this.labelControl_inputText.TabIndex = 1;
            this.labelControl_inputText.Text = "输入码";
            // 
            // textEdit_inputText
            // 
            this.textEdit_inputText.Location = new System.Drawing.Point(406, 42);
            this.textEdit_inputText.Margin = new System.Windows.Forms.Padding(4);
            this.textEdit_inputText.Name = "textEdit_inputText";
            this.textEdit_inputText.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 12F);
            this.textEdit_inputText.Properties.Appearance.Options.UseFont = true;
            this.textEdit_inputText.Size = new System.Drawing.Size(182, 36);
            this.textEdit_inputText.TabIndex = 2;
            this.textEdit_inputText.TextChanged += new System.EventHandler(this.textEdit_inputText_TextChanged);
            // 
            // comboBoxEdit_InputType
            // 
            this.comboBoxEdit_InputType.Location = new System.Drawing.Point(603, 42);
            this.comboBoxEdit_InputType.Margin = new System.Windows.Forms.Padding(4);
            this.comboBoxEdit_InputType.Name = "comboBoxEdit_InputType";
            this.comboBoxEdit_InputType.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 12F);
            this.comboBoxEdit_InputType.Properties.Appearance.Options.UseFont = true;
            this.comboBoxEdit_InputType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.comboBoxEdit_InputType.Properties.Sorted = true;
            this.comboBoxEdit_InputType.Size = new System.Drawing.Size(162, 36);
            this.comboBoxEdit_InputType.TabIndex = 3;
            this.comboBoxEdit_InputType.SelectedIndexChanged += new System.EventHandler(this.comboBoxEdit_InputType_SelectedIndexChanged);
            // 
            // simpleButton_refresh
            // 
            this.simpleButton_refresh.Location = new System.Drawing.Point(789, 45);
            this.simpleButton_refresh.Margin = new System.Windows.Forms.Padding(4);
            this.simpleButton_refresh.Name = "simpleButton_refresh";
            this.simpleButton_refresh.Size = new System.Drawing.Size(112, 34);
            this.simpleButton_refresh.TabIndex = 5;
            this.simpleButton_refresh.Text = "刷新(&R)";
            this.simpleButton_refresh.Click += new System.EventHandler(this.simpleButton_refresh_Click);
            // 
            // labelControl_hospital
            // 
            this.labelControl_hospital.Appearance.Font = new System.Drawing.Font("Tahoma", 12F);
            this.labelControl_hospital.Location = new System.Drawing.Point(40, 46);
            this.labelControl_hospital.Margin = new System.Windows.Forms.Padding(4);
            this.labelControl_hospital.Name = "labelControl_hospital";
            this.labelControl_hospital.Size = new System.Drawing.Size(48, 29);
            this.labelControl_hospital.TabIndex = 6;
            this.labelControl_hospital.Text = "院区";
            // 
            // comboBoxEdit_hospital
            // 
            this.comboBoxEdit_hospital.Location = new System.Drawing.Point(99, 44);
            this.comboBoxEdit_hospital.Margin = new System.Windows.Forms.Padding(4);
            this.comboBoxEdit_hospital.Name = "comboBoxEdit_hospital";
            this.comboBoxEdit_hospital.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 12F);
            this.comboBoxEdit_hospital.Properties.Appearance.Options.UseFont = true;
            this.comboBoxEdit_hospital.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.comboBoxEdit_hospital.Size = new System.Drawing.Size(195, 36);
            this.comboBoxEdit_hospital.TabIndex = 7;
            this.comboBoxEdit_hospital.SelectedIndexChanged += new System.EventHandler(this.comboBoxEdit_hospital_SelectedIndexChanged);
            // 
            // InputMethodForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(920, 922);
            this.Controls.Add(this.comboBoxEdit_hospital);
            this.Controls.Add(this.labelControl_hospital);
            this.Controls.Add(this.simpleButton_refresh);
            this.Controls.Add(this.comboBoxEdit_InputType);
            this.Controls.Add(this.textEdit_inputText);
            this.Controls.Add(this.labelControl_inputText);
            this.Controls.Add(this.gridControl_InputData);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "InputMethodForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "InputMethodForm";
            this.Load += new System.EventHandler(this.InputMethodForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridControl_InputData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView_inputSettingData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit_inputText.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.comboBoxEdit_InputType.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.comboBoxEdit_hospital.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraGrid.GridControl gridControl_InputData;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView_inputSettingData;
        private DevExpress.XtraEditors.LabelControl labelControl_inputText;
        private DevExpress.XtraEditors.TextEdit textEdit_inputText;
        private DevExpress.XtraEditors.ComboBoxEdit comboBoxEdit_InputType;
        private DevExpress.XtraEditors.SimpleButton simpleButton_refresh;
        private DevExpress.XtraEditors.LabelControl labelControl_hospital;
        private DevExpress.XtraEditors.ComboBoxEdit comboBoxEdit_hospital;
    }
}