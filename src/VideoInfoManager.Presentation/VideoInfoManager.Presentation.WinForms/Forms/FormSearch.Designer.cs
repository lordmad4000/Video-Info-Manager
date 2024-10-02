namespace VideoInfoManager.Presentation.WinForms.Forms;

partial class FormSearch
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
        pnSearch = new Panel();
        pnHeader = new Panel();
        cbL = new CheckBox();
        pnShowed = new Panel();
        cbD = new CheckBox();
        cbB = new CheckBox();
        cbS = new CheckBox();
        cbP = new CheckBox();
        tbSearch = new TextBox();
        btnPaste = new Button();
        btnShowHide = new Button();
        label1 = new Label();
        pnModify = new Panel();
        label4 = new Label();
        tbModifyId = new TextBox();
        label3 = new Label();
        label2 = new Label();
        btnModifySave = new Button();
        tbModifyName = new TextBox();
        cbModifyStatus = new ComboBox();
        btnModifyCancel = new Button();
        pnAddData = new Panel();
        btnLowed = new Button();
        btnExport = new Button();
        btnSearchByAuthor = new Button();
        btnClear = new Button();
        btnCutFirst = new Button();
        btnPasteAndRename = new Button();
        rtbVideoInfo = new RichTextBox();
        btnDeleted = new Button();
        btnBackuped = new Button();
        btnSaved = new Button();
        btnPended = new Button();
        dgvVideoInfo = new DataGridView();
        pnSearch.SuspendLayout();
        pnHeader.SuspendLayout();
        pnShowed.SuspendLayout();
        pnModify.SuspendLayout();
        pnAddData.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvVideoInfo).BeginInit();
        SuspendLayout();
        // 
        // pnSearch
        // 
        pnSearch.BorderStyle = BorderStyle.FixedSingle;
        pnSearch.Controls.Add(pnHeader);
        pnSearch.Controls.Add(pnModify);
        pnSearch.Controls.Add(pnAddData);
        pnSearch.Controls.Add(dgvVideoInfo);
        pnSearch.Location = new Point(12, 12);
        pnSearch.Name = "pnSearch";
        pnSearch.Size = new Size(1594, 500);
        pnSearch.TabIndex = 5;
        // 
        // pnHeader
        // 
        pnHeader.Controls.Add(cbL);
        pnHeader.Controls.Add(pnShowed);
        pnHeader.Controls.Add(tbSearch);
        pnHeader.Controls.Add(btnPaste);
        pnHeader.Controls.Add(btnShowHide);
        pnHeader.Controls.Add(label1);
        pnHeader.Location = new Point(14, 3);
        pnHeader.Name = "pnHeader";
        pnHeader.Size = new Size(500, 68);
        pnHeader.TabIndex = 13;
        // 
        // cbL
        // 
        cbL.Checked = true;
        cbL.CheckState = CheckState.Checked;
        cbL.Location = new Point(398, 5);
        cbL.Name = "cbL";
        cbL.Size = new Size(90, 19);
        cbL.TabIndex = 14;
        cbL.Text = "Lowed";
        cbL.UseVisualStyleBackColor = true;
        cbL.CheckStateChanged += cbStatus_Changed;
        // 
        // pnShowed
        // 
        pnShowed.Controls.Add(cbD);
        pnShowed.Controls.Add(cbB);
        pnShowed.Controls.Add(cbS);
        pnShowed.Controls.Add(cbP);
        pnShowed.Location = new Point(7, 0);
        pnShowed.Name = "pnShowed";
        pnShowed.Size = new Size(487, 27);
        pnShowed.TabIndex = 12;
        // 
        // cbD
        // 
        cbD.Checked = true;
        cbD.CheckState = CheckState.Checked;
        cbD.Location = new Point(295, 5);
        cbD.Name = "cbD";
        cbD.Size = new Size(90, 19);
        cbD.TabIndex = 13;
        cbD.Text = "Deleted";
        cbD.UseVisualStyleBackColor = true;
        cbD.CheckStateChanged += cbStatus_Changed;
        // 
        // cbB
        // 
        cbB.Checked = true;
        cbB.CheckState = CheckState.Checked;
        cbB.Location = new Point(198, 5);
        cbB.Name = "cbB";
        cbB.Size = new Size(90, 19);
        cbB.TabIndex = 12;
        cbB.Text = "Backuped";
        cbB.UseVisualStyleBackColor = true;
        cbB.CheckStateChanged += cbStatus_Changed;
        // 
        // cbS
        // 
        cbS.Checked = true;
        cbS.CheckState = CheckState.Checked;
        cbS.Location = new Point(102, 5);
        cbS.Name = "cbS";
        cbS.Size = new Size(90, 19);
        cbS.TabIndex = 11;
        cbS.Text = "Saved";
        cbS.UseVisualStyleBackColor = true;
        cbS.CheckStateChanged += cbStatus_Changed;
        // 
        // cbP
        // 
        cbP.Checked = true;
        cbP.CheckState = CheckState.Checked;
        cbP.Location = new Point(6, 5);
        cbP.Name = "cbP";
        cbP.Size = new Size(90, 19);
        cbP.TabIndex = 10;
        cbP.Text = "Pended";
        cbP.UseVisualStyleBackColor = true;
        cbP.CheckStateChanged += cbStatus_Changed;
        // 
        // tbSearch
        // 
        tbSearch.Location = new Point(48, 40);
        tbSearch.Name = "tbSearch";
        tbSearch.Size = new Size(297, 23);
        tbSearch.TabIndex = 10;
        tbSearch.TextChanged += tbSearch_TextChanged;
        // 
        // btnPaste
        // 
        btnPaste.Location = new Point(351, 31);
        btnPaste.Name = "btnPaste";
        btnPaste.Size = new Size(65, 33);
        btnPaste.TabIndex = 9;
        btnPaste.Text = "Paste";
        btnPaste.UseVisualStyleBackColor = true;
        btnPaste.Click += btnPaste_Click;
        // 
        // btnShowHide
        // 
        btnShowHide.AllowDrop = true;
        btnShowHide.Location = new Point(423, 31);
        btnShowHide.Name = "btnShowHide";
        btnShowHide.Size = new Size(65, 33);
        btnShowHide.TabIndex = 8;
        btnShowHide.Text = "Add Data";
        btnShowHide.UseVisualStyleBackColor = true;
        btnShowHide.Click += btnShowHide_Click;
        btnShowHide.DragEnter += btnShowHide_DragEnter;
        // 
        // label1
        // 
        label1.AutoSize = true;
        label1.Location = new Point(0, 45);
        label1.Name = "label1";
        label1.Size = new Size(42, 15);
        label1.TabIndex = 7;
        label1.Text = "Search";
        // 
        // pnModify
        // 
        pnModify.Controls.Add(label4);
        pnModify.Controls.Add(tbModifyId);
        pnModify.Controls.Add(label3);
        pnModify.Controls.Add(label2);
        pnModify.Controls.Add(btnModifySave);
        pnModify.Controls.Add(tbModifyName);
        pnModify.Controls.Add(cbModifyStatus);
        pnModify.Controls.Add(btnModifyCancel);
        pnModify.Location = new Point(1026, 72);
        pnModify.Name = "pnModify";
        pnModify.Size = new Size(500, 420);
        pnModify.TabIndex = 12;
        pnModify.Visible = false;
        // 
        // label4
        // 
        label4.AutoSize = true;
        label4.Location = new Point(18, 379);
        label4.Name = "label4";
        label4.Size = new Size(17, 15);
        label4.TabIndex = 21;
        label4.Text = "Id";
        label4.Visible = false;
        // 
        // tbModifyId
        // 
        tbModifyId.Location = new Point(63, 371);
        tbModifyId.Name = "tbModifyId";
        tbModifyId.Size = new Size(425, 23);
        tbModifyId.TabIndex = 20;
        tbModifyId.Visible = false;
        // 
        // label3
        // 
        label3.AutoSize = true;
        label3.Location = new Point(18, 76);
        label3.Name = "label3";
        label3.Size = new Size(39, 15);
        label3.TabIndex = 19;
        label3.Text = "Status";
        // 
        // label2
        // 
        label2.AutoSize = true;
        label2.Location = new Point(18, 34);
        label2.Name = "label2";
        label2.Size = new Size(39, 15);
        label2.TabIndex = 18;
        label2.Text = "Name";
        // 
        // btnModifySave
        // 
        btnModifySave.Location = new Point(418, 103);
        btnModifySave.Name = "btnModifySave";
        btnModifySave.Size = new Size(70, 45);
        btnModifySave.TabIndex = 17;
        btnModifySave.Text = "Save";
        btnModifySave.UseVisualStyleBackColor = true;
        btnModifySave.Click += btnModifySave_Click;
        // 
        // tbModifyName
        // 
        tbModifyName.Location = new Point(63, 26);
        tbModifyName.Name = "tbModifyName";
        tbModifyName.Size = new Size(425, 23);
        tbModifyName.TabIndex = 16;
        // 
        // cbModifyStatus
        // 
        cbModifyStatus.DropDownStyle = ComboBoxStyle.DropDownList;
        cbModifyStatus.FormattingEnabled = true;
        cbModifyStatus.Location = new Point(63, 68);
        cbModifyStatus.Name = "cbModifyStatus";
        cbModifyStatus.Size = new Size(238, 23);
        cbModifyStatus.TabIndex = 15;
        // 
        // btnModifyCancel
        // 
        btnModifyCancel.Location = new Point(342, 103);
        btnModifyCancel.Name = "btnModifyCancel";
        btnModifyCancel.Size = new Size(70, 45);
        btnModifyCancel.TabIndex = 10;
        btnModifyCancel.Text = "Cancel";
        btnModifyCancel.UseVisualStyleBackColor = true;
        btnModifyCancel.Click += btnModifyCancel_Click;
        // 
        // pnAddData
        // 
        pnAddData.Controls.Add(btnLowed);
        pnAddData.Controls.Add(btnExport);
        pnAddData.Controls.Add(btnSearchByAuthor);
        pnAddData.Controls.Add(btnClear);
        pnAddData.Controls.Add(btnCutFirst);
        pnAddData.Controls.Add(btnPasteAndRename);
        pnAddData.Controls.Add(rtbVideoInfo);
        pnAddData.Controls.Add(btnDeleted);
        pnAddData.Controls.Add(btnBackuped);
        pnAddData.Controls.Add(btnSaved);
        pnAddData.Controls.Add(btnPended);
        pnAddData.Location = new Point(520, 72);
        pnAddData.Name = "pnAddData";
        pnAddData.Size = new Size(500, 420);
        pnAddData.TabIndex = 8;
        pnAddData.Visible = false;
        // 
        // btnLowed
        // 
        btnLowed.AllowDrop = true;
        btnLowed.Location = new Point(410, 14);
        btnLowed.Name = "btnLowed";
        btnLowed.Size = new Size(80, 80);
        btnLowed.TabIndex = 15;
        btnLowed.Text = "Lowed";
        btnLowed.UseVisualStyleBackColor = true;
        btnLowed.Click += AddData_Click;
        btnLowed.DragDrop += AddData_DragDrop;
        btnLowed.DragEnter += AddData_DragEnter;
        // 
        // btnExport
        // 
        btnExport.Location = new Point(410, 120);
        btnExport.Name = "btnExport";
        btnExport.Size = new Size(80, 45);
        btnExport.TabIndex = 14;
        btnExport.Text = "Export Data";
        btnExport.UseVisualStyleBackColor = true;
        btnExport.Click += btnExport_Click;
        // 
        // btnSearchByAuthor
        // 
        btnSearchByAuthor.Location = new Point(270, 120);
        btnSearchByAuthor.Name = "btnSearchByAuthor";
        btnSearchByAuthor.Size = new Size(110, 45);
        btnSearchByAuthor.TabIndex = 13;
        btnSearchByAuthor.Text = "Search by Author";
        btnSearchByAuthor.UseVisualStyleBackColor = true;
        btnSearchByAuthor.Click += btnSearchByAuthor_Click;
        // 
        // btnClear
        // 
        btnClear.Location = new Point(194, 120);
        btnClear.Name = "btnClear";
        btnClear.Size = new Size(70, 45);
        btnClear.TabIndex = 12;
        btnClear.Text = "Clear";
        btnClear.UseVisualStyleBackColor = true;
        btnClear.Click += btnClear_Click;
        // 
        // btnCutFirst
        // 
        btnCutFirst.Location = new Point(118, 120);
        btnCutFirst.Name = "btnCutFirst";
        btnCutFirst.Size = new Size(70, 45);
        btnCutFirst.TabIndex = 11;
        btnCutFirst.Text = "Cut First";
        btnCutFirst.UseVisualStyleBackColor = true;
        btnCutFirst.Click += btnCutFirst_Click;
        // 
        // btnPasteAndRename
        // 
        btnPasteAndRename.Location = new Point(0, 120);
        btnPasteAndRename.Name = "btnPasteAndRename";
        btnPasteAndRename.Size = new Size(112, 45);
        btnPasteAndRename.TabIndex = 10;
        btnPasteAndRename.Text = "Paste and Rename";
        btnPasteAndRename.UseVisualStyleBackColor = true;
        btnPasteAndRename.Click += btnPasteAndRename_Click;
        // 
        // rtbVideoInfo
        // 
        rtbVideoInfo.Location = new Point(0, 171);
        rtbVideoInfo.Name = "rtbVideoInfo";
        rtbVideoInfo.Size = new Size(490, 170);
        rtbVideoInfo.TabIndex = 4;
        rtbVideoInfo.Text = "";
        // 
        // btnDeleted
        // 
        btnDeleted.AllowDrop = true;
        btnDeleted.Location = new Point(307, 14);
        btnDeleted.Name = "btnDeleted";
        btnDeleted.Size = new Size(80, 80);
        btnDeleted.TabIndex = 3;
        btnDeleted.Text = "Deleted";
        btnDeleted.UseVisualStyleBackColor = true;
        btnDeleted.Click += AddData_Click;
        btnDeleted.DragDrop += AddData_DragDrop;
        btnDeleted.DragEnter += AddData_DragEnter;
        // 
        // btnBackuped
        // 
        btnBackuped.AllowDrop = true;
        btnBackuped.Location = new Point(207, 14);
        btnBackuped.Name = "btnBackuped";
        btnBackuped.Size = new Size(80, 80);
        btnBackuped.TabIndex = 2;
        btnBackuped.Text = "Backuped";
        btnBackuped.UseVisualStyleBackColor = true;
        btnBackuped.Click += AddData_Click;
        btnBackuped.DragDrop += AddData_DragDrop;
        btnBackuped.DragEnter += AddData_DragEnter;
        // 
        // btnSaved
        // 
        btnSaved.AllowDrop = true;
        btnSaved.Location = new Point(104, 14);
        btnSaved.Name = "btnSaved";
        btnSaved.Size = new Size(80, 80);
        btnSaved.TabIndex = 1;
        btnSaved.Text = "Saved";
        btnSaved.UseVisualStyleBackColor = true;
        btnSaved.Click += AddData_Click;
        btnSaved.DragDrop += AddData_DragDrop;
        btnSaved.DragEnter += AddData_DragEnter;
        // 
        // btnPended
        // 
        btnPended.AllowDrop = true;
        btnPended.Location = new Point(0, 14);
        btnPended.Name = "btnPended";
        btnPended.Size = new Size(80, 80);
        btnPended.TabIndex = 0;
        btnPended.Text = "Pended";
        btnPended.UseVisualStyleBackColor = true;
        btnPended.Click += AddData_Click;
        btnPended.DragDrop += AddData_DragDrop;
        btnPended.DragEnter += AddData_DragEnter;
        // 
        // dgvVideoInfo
        // 
        dgvVideoInfo.AllowUserToAddRows = false;
        dgvVideoInfo.AllowUserToDeleteRows = false;
        dgvVideoInfo.AllowUserToOrderColumns = true;
        dgvVideoInfo.AllowUserToResizeRows = false;
        dgvVideoInfo.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
        dgvVideoInfo.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dgvVideoInfo.Location = new Point(14, 72);
        dgvVideoInfo.MultiSelect = false;
        dgvVideoInfo.Name = "dgvVideoInfo";
        dgvVideoInfo.ReadOnly = true;
        dgvVideoInfo.RowHeadersVisible = false;
        dgvVideoInfo.RowHeadersWidth = 62;
        dgvVideoInfo.ShowEditingIcon = false;
        dgvVideoInfo.Size = new Size(500, 420);
        dgvVideoInfo.TabIndex = 3;
        dgvVideoInfo.CellMouseEnter += dgvVideoInfo_CellMouseEnter;
        // 
        // FormSearch
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1630, 596);
        Controls.Add(pnSearch);
        MinimumSize = new Size(560, 500);
        Name = "FormSearch";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Video Info Manager";
        ResizeEnd += FormSearch_ResizeEnd;
        Resize += FormSearch_Resize;
        pnSearch.ResumeLayout(false);
        pnHeader.ResumeLayout(false);
        pnHeader.PerformLayout();
        pnShowed.ResumeLayout(false);
        pnModify.ResumeLayout(false);
        pnModify.PerformLayout();
        pnAddData.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)dgvVideoInfo).EndInit();
        ResumeLayout(false);
    }

    #endregion
    private Panel pnSearch;
    private DataGridView dgvVideoInfo;
    private Panel pnAddData;
    private Button btnDeleted;
    private Button btnBackuped;
    private Button btnSaved;
    private Button btnPended;
    private RichTextBox rtbVideoInfo;
    private Button btnPasteAndRename;
    private Button btnCutFirst;
    private Button btnClear;
    private Button btnSearchByAuthor;
    private Button btnExport;
    private Button btnLowed;
    private Panel pnModify;
    private Button btnModifyCancel;
    private TextBox tbModifyName;
    private ComboBox cbModifyStatus;
    private Button btnModifySave;
    private Panel pnHeader;
    private Panel pnShowed;
    private CheckBox cbL;
    private CheckBox cbD;
    private CheckBox cbB;
    private CheckBox cbS;
    private CheckBox cbP;
    private TextBox tbSearch;
    private Button btnPaste;
    private Button btnShowHide;
    private Label label1;
    private Label label3;
    private Label label2;
    private Label label4;
    private TextBox tbModifyId;
}