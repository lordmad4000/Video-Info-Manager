using System.ComponentModel;
using System.Text;
using VideoInfoManager.Application.DTOs;
using VideoInfoManager.Presentation.WinForms.Helpers;
using VideoInfoManager.Domain.Enums;
using VideoInfoManager.Presentation.CrossCutting.Models;
using VideoInfoManager.Presentation.CrossCutting.Services;
using VideoInfoManager.Presentation.Crosscutting.Extensions;

namespace VideoInfoManager.Presentation.WinForms.Forms;

public partial class FormSearch : Form
{
    private const int MinSearchLength = 3;
    private VideoInfoDTO? _videoInfoSelected { get; set; } = null;

    private readonly IVideoInfoManagerPresentationAppService _videoInfoManagerPresentationAppService;

    public FormSearch(IVideoInfoManagerPresentationAppService videoInfoManagerPresentationAppService)
    {
        _videoInfoManagerPresentationAppService = videoInfoManagerPresentationAppService;
        InitializeComponent();
        InitializeStatusComponents();
        InitializeMultipleSearchBox();
        InitializeContextMenu();
        InitializeWindow();
        ResizeControls();
    }

    #region PRIVATE EVENTS
    private void FormSearch_ResizeEnd(object sender, EventArgs e) => ResizeControls();
    private void FormSearch_Resize(object sender, EventArgs e) => ResizeControls();
    private void btnClear_Click(object sender, EventArgs e) => rtbVideoInfo.Text = "";
    private void menuItemModify_Click(object sender, EventArgs e) => ShowModify();
    private void btnModifyCancel_Click(object sender, EventArgs e)
    {
        ShowSearch();
        ExecuteLastSearch();
    }

    private void btnModifySave_Click(object sender, EventArgs e)
    {
        var videoInfoDTO = new VideoInfoDTO
        {
            Id = new Guid(tbModifyId.Text),
            Name = tbModifyName.Text,
            Status = GetModifySelectedStatus(),
        };

        if (MessageBox.Show($"Update Data to Data Base?", "Update Data", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
        {
            if (_videoInfoManagerPresentationAppService.Update(videoInfoDTO) == true)
            {
                MessageBox.Show($"{videoInfoDTO.Name} Updated.");
                ExecuteLastSearch();
                ShowSearch();
            }
        }
    }

    private void tbSearch_TextChanged(object sender, EventArgs e)
    {
        if (tbSearch.Text.Length > MinSearchLength)
        {
            Search(new string[] { tbSearch.Text });
        }
    }

    private void btnShowHide_Click(object sender, EventArgs e)
    {
        if (pnAddData.Visible == true) ShowSearch();
        else ShowAddData();
    }

    private void cbStatus_Changed(object sender, EventArgs e)
    {
        ExecuteLastSearch();
    }

    private void btnPaste_Click(object sender, EventArgs e)
    {
        if (Clipboard.ContainsText())
        {
            string clipBoardText = Clipboard.GetText();
            tbSearch.Text = clipBoardText;
        }
        if (Clipboard.ContainsFileDropList())
        {
            var fileList = Clipboard.GetFileDropList();
            if (fileList != null && fileList.Count > 0)
            {
                tbSearch.Text = fileList[0];
            }
        }

        ShowSearch();
    }

    private void btnPasteAndRename_Click(object sender, EventArgs e)
    {
        if (Clipboard.ContainsText())
        {
            string clipBoardText = Clipboard.GetText();
            var splitText = clipBoardText.Split("\n", StringSplitOptions.RemoveEmptyEntries);
            if (splitText is not null && splitText.Length > 0)
            {
                var text = new StringBuilder();
                foreach (string item in splitText)
                {
                    string videoName = _videoInfoManagerPresentationAppService.RenameVideoInfoName(item);
                    text.Append($"{videoName}{Environment.NewLine}");
                }
                rtbVideoInfo.Text += text;
            }
        }
        if (Clipboard.ContainsFileDropList())
        {
            var fileList = Clipboard.GetFileDropList();
            if (fileList is not null && fileList.Count > 0)
            {
                var text = new StringBuilder();
                foreach (var file in fileList)
                {
                    if (file is not null)
                    {
                        text.Append($"{_videoInfoManagerPresentationAppService.NormalizeFileName(file)}{Environment.NewLine}");
                    }
                }
                rtbVideoInfo.Text += text;
            }
        }
    }

    private void btnCutFirst_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(rtbVideoInfo.Text))
        {
            return;
        }

        string result = _videoInfoManagerPresentationAppService.RemoveFirstItem(rtbVideoInfo.Text);
        string cutText = _videoInfoManagerPresentationAppService.GetFirstItem(rtbVideoInfo.Text);

        if (string.IsNullOrEmpty(cutText) is false)
        {
            Clipboard.SetText(cutText.RemoveNewLine());
            rtbVideoInfo.Text = result;
        }
    }

    private void btnSearchByAuthor_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(rtbVideoInfo.Text))
        {
            return;
        }

        var search = new string[] { rtbVideoInfo.Text };
        string[] lines = rtbVideoInfo.Text.SplitNewLine(StringSplitOptions.None);
        if (lines.Count() > 1)
        {
            search = lines;
        }

        _videoInfoManagerPresentationAppService.Search(search, GetActiveStatus(), true);
        AddDataTodgvVideoInfo(_videoInfoManagerPresentationAppService.GetResults());

        ShowSearch();
    }

    private void AddData_Click(object sender, EventArgs e)
    {
        if (Clipboard.ContainsText())
        {
            string data = Clipboard.GetText();
            if (string.IsNullOrEmpty(data) == false)
            {
                AddData(sender, data);
            }
        }
        if (Clipboard.ContainsFileDropList())
        {
            string[]? data = (string[]?)Clipboard.GetData("FileDrop");
            if (data != null)
            {
                AddData(sender, NormalizeAndConvertToStringWithNewLineSeparators(data));
            }
        }
    }

    private void AddData_DragEnter(object sender, DragEventArgs e)
    {
        e.Effect = DragDropEffects.Copy;
    }

    private void AddData_DragDrop(object sender, DragEventArgs e)
    {
        if (e.Data == null)
        {
            return;
        }
        if (e.Data.GetDataPresent(DataFormats.Text))
        {
            string? data = (string?)e.Data.GetData(DataFormats.Text);
            if (data != null)
            {
                AddData(sender, data.ToString());
            }
        }
        else if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            string[]? data = (string[]?)e.Data.GetData(DataFormats.FileDrop, true);
            if (data != null)
            {
                AddData(sender, NormalizeAndConvertToStringWithNewLineSeparators(data));
            }
        }
    }

    private void btnExport_Click(object sender, EventArgs e)
    {
        List<VideoInfoDTO>? sortedVideoInfo = _videoInfoManagerPresentationAppService.GetAllDataOrderByName();

        if (sortedVideoInfo is null || sortedVideoInfo.Count() == 0)
        {
            MessageBox.Show("No data found.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var saveFileDialog = new SaveFileDialog();
        saveFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
        saveFileDialog.DefaultExt = "txt";
        saveFileDialog.RestoreDirectory = true;

        if (saveFileDialog.ShowDialog() == DialogResult.OK)
        {
            var path = saveFileDialog.FileName;
            using (var fs = new FileStream(path, FileMode.Create))
            {
                using (var sw = new StreamWriter(fs))
                {
                    foreach (var videoInfo in sortedVideoInfo)
                    {
                        string item = $"({videoInfo.Status}) {videoInfo.Name}";
                        sw.WriteLine(item);
                    }
                }
            }
            MessageBox.Show("Data exported.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

    private void btnShowHide_DragEnter(object sender, DragEventArgs e)
    {
        e.Effect = DragDropEffects.None;
        btnShowHide_Click(sender, e);
    }

    private void dgvVideoInfo_MouseClick(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Right)
        {
            if (dgvVideoInfo.Rows.Count <= 0)
                return;

            _videoInfoSelected = GetRowData(dgvVideoInfo.HitTest(e.X, e.Y).RowIndex);
        }

    }

    private void dgvVideoInfo_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
    {
        if (dgvVideoInfo.Rows.Count <= 0 || e.RowIndex < 0)
        {
            _videoInfoSelected = null;
            return;
        }

        _videoInfoSelected = GetRowData(e.RowIndex);
    }

    private void MenuStrip_Opening(object sender, CancelEventArgs e)
    {
        if (_videoInfoSelected is null)
            e.Cancel = true;
    }

    private void menuItemRemove_Click(object sender, EventArgs e)
    {
        if (_videoInfoSelected is null)
            return;

        if (_videoInfoManagerPresentationAppService.Delete(_videoInfoSelected) == true)
        {
            MessageBox.Show($"{_videoInfoSelected.Name} Removed.");
            ExecuteLastSearch();
        }
    }

    #endregion PRIVATE EVENTS

    #region PRIVATE FUNCIONS
    private void ResizeControls()
    {
        pnSearch.Width = this.Width - 40;
        pnSearch.Height = this.Height - 70;
        dgvVideoInfo.Width = pnSearch.Width - 30;
        dgvVideoInfo.Height = pnSearch.Height - 90;
    }

    private void ExecuteLastSearch()
    {
        _videoInfoManagerPresentationAppService.LastSearch(GetActiveStatus(), true);

        List<VideoInfoDTO> lastSearchData = _videoInfoManagerPresentationAppService.GetResults();

        if (lastSearchData != null && lastSearchData.Count() > 0)
        {
            AddDataTodgvVideoInfo(lastSearchData);
        }
    }

    private void Search(string[]? search, bool isVideoName = false)
    {
        if (search is null || search.Count() == 0)
            return;

        _videoInfoManagerPresentationAppService.Search(search, GetActiveStatus());
        AddDataTodgvVideoInfo(_videoInfoManagerPresentationAppService.GetResults());
    }

    private IEnumerable<VideoInfoDTO> GetVideoInfoWithConfigurationNames(IEnumerable<VideoInfoDTO> videoInfoDTOs)
    {
        var videoInfoWithConfigurationNames = new List<VideoInfoDTO>();

        foreach (var videoInfo in videoInfoDTOs)
        {
            var videoInfoWithConfigurationName = new VideoInfoDTO
            {
                Id = videoInfo.Id,
                Name = videoInfo.Name,
                Status = GetVideoInfoStatusByStatusName(videoInfo.Status).ConfigurationName
            };

            videoInfoWithConfigurationNames.Add(videoInfoWithConfigurationName);
        }

        return videoInfoWithConfigurationNames;
    }

    private void AddDataTodgvVideoInfo(List<VideoInfoDTO> videoInfoDTOs)
    {
        try
        {
            var bindingList = new BindingList<VideoInfoDTO>(videoInfoDTOs);
            var source = new BindingSource(bindingList, null);
            dgvVideoInfo.DataSource = source;

            dgvVideoInfo.Columns["Status"].DisplayIndex = 0;
            dgvVideoInfo.Columns["Name"].DisplayIndex = 1;
            dgvVideoInfo.Columns["Id"].DisplayIndex = 2;
            dgvVideoInfo.Columns["Id"].Visible = false;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private void ShowModify()
    {
        if (_videoInfoSelected is null)
            return;

        cbModifyStatus.SelectedIndex = GetStatusIndexByName(_videoInfoSelected.Status);
        tbModifyName.Text = _videoInfoSelected.Name;
        tbModifyId.Text = _videoInfoSelected.Id.ToString();
        pnHeader.Visible = false;
        pnModify.Visible = true;
        dgvVideoInfo.Visible = false;
        pnModify.Left = dgvVideoInfo.Left;
        pnModify.Top = dgvVideoInfo.Top;
    }

    private void ShowAddData()
    {
        btnShowHide.Text = "Search";
        dgvVideoInfo.Visible = false;
        pnModify.Visible = false;
        pnAddData.Visible = true;
        pnAddData.Left = dgvVideoInfo.Left;
        pnAddData.Top = dgvVideoInfo.Top;
    }

    private void ShowSearch()
    {
        btnShowHide.Text = "Add Data";
        pnModify.Visible = false;
        pnHeader.Visible = true;
        pnAddData.Visible = false;
        dgvVideoInfo.Visible = true;
    }

    private List<VideoInfoStatusEnum> GetActiveStatus()
    {
        var activeStatus = new List<VideoInfoStatusEnum>();
        if (cbP.Checked) activeStatus.Add(VideoInfoStatusEnum.Pended);
        if (cbS.Checked) activeStatus.Add(VideoInfoStatusEnum.Saved);
        if (cbB.Checked) activeStatus.Add(VideoInfoStatusEnum.Backuped);
        if (cbD.Checked) activeStatus.Add(VideoInfoStatusEnum.Deleted);
        if (cbL.Checked) activeStatus.Add(VideoInfoStatusEnum.Lowed);

        return activeStatus;
    }

    private VideoInfoDTO? GetRowData(int rowIndex)
    {
        if (rowIndex < 0)
            return null;

        string? id = dgvVideoInfo[0, rowIndex].Value.ToString();
        string? name = dgvVideoInfo[1, rowIndex].Value.ToString();
        string? status = dgvVideoInfo[2, rowIndex].Value.ToString();
        var videoInfoDTO = new VideoInfoDTO
        {
            Id = id is null
                ? Guid.Empty
                : new Guid(id),
            Name = name ?? "",
            Status = status ?? ""
        };

        return videoInfoDTO;
    }

    private string GetModifySelectedStatus()
    {
        var status = cbModifyStatus.GetItemText(cbModifyStatus.SelectedItem);

        if (status is null)
            status = _videoInfoManagerPresentationAppService.GetVideoInfoStatuses()[0].Status.ToString();

        return GetVideoInfoStatusByConfigurationName(status).StatusName;
    }

    private VideoInfoStatus GetVideoInfoStatusByConfigurationName(string configurationName)
    {
        VideoInfoStatus? videoinfoStatus = _videoInfoManagerPresentationAppService.GetVideoInfoStatuses().FirstOrDefault(c => c.ConfigurationName.Equals(configurationName));
        if (videoinfoStatus is null)
            return new VideoInfoStatus();

        return videoinfoStatus;
    }

    private VideoInfoStatus GetVideoInfoStatusByStatusName(string statusName)
    {
        VideoInfoStatus? videoinfoStatus = _videoInfoManagerPresentationAppService.GetVideoInfoStatuses().FirstOrDefault(c => c.StatusName.Equals(statusName));
        if (videoinfoStatus is null)
            return new VideoInfoStatus();

        return videoinfoStatus;
    }

    private int GetStatusIndexByName(string configurationName)
    {
        int index = _videoInfoManagerPresentationAppService.GetVideoInfoStatuses().FindIndex(c => c.ConfigurationName.Equals(configurationName));

        if (index == -1)
            return 0;

        return index;
    }

    private void InitializeWindow()
    {
        this.Size = new System.Drawing.Size(560, 500);
        this.Icon = IconExtractor.Extract("shell32.dll", 218, true);
    }

    private void InitializeMultipleSearchBox()
    {
        rtbVideoInfo.AllowDrop = true;
        rtbVideoInfo.DragEnter += AddData_DragEnter;
        rtbVideoInfo.DragDrop += AddData_DragDrop;
    }

    private void InitializeContextMenu()
    {
        var menuStrip = new ContextMenuStrip();
        menuStrip.Opening += new CancelEventHandler(MenuStrip_Opening);
        var menuItem1 = new ToolStripMenuItem("Remove");
        menuItem1.Name = "Remove";
        menuItem1.Click += new EventHandler(menuItemRemove_Click);
        var menuItem2 = new ToolStripMenuItem("Modify");
        menuItem2.Name = "Modify";
        menuItem2.Click += new EventHandler(menuItemModify_Click);
        menuStrip.Items.Add(menuItem1);
        menuStrip.Items.Add(menuItem2);

        dgvVideoInfo.ContextMenuStrip = menuStrip;
    }

    private void InitializeStatusComponents()
    {
        btnPended.Text = _videoInfoManagerPresentationAppService.GetVideoInfoStatuses()[0].ConfigurationName;
        btnSaved.Text = _videoInfoManagerPresentationAppService.GetVideoInfoStatuses()[1].ConfigurationName;
        btnBackuped.Text = _videoInfoManagerPresentationAppService.GetVideoInfoStatuses()[2].ConfigurationName;
        btnDeleted.Text = _videoInfoManagerPresentationAppService.GetVideoInfoStatuses()[3].ConfigurationName;
        btnLowed.Text = _videoInfoManagerPresentationAppService.GetVideoInfoStatuses()[4].ConfigurationName;
        cbP.Text = _videoInfoManagerPresentationAppService.GetVideoInfoStatuses()[0].ConfigurationName;
        cbS.Text = _videoInfoManagerPresentationAppService.GetVideoInfoStatuses()[1].ConfigurationName;
        cbB.Text = _videoInfoManagerPresentationAppService.GetVideoInfoStatuses()[2].ConfigurationName;
        cbD.Text = _videoInfoManagerPresentationAppService.GetVideoInfoStatuses()[3].ConfigurationName;
        cbL.Text = _videoInfoManagerPresentationAppService.GetVideoInfoStatuses()[4].ConfigurationName;

        foreach (var videoInfoStatus in _videoInfoManagerPresentationAppService.GetVideoInfoStatuses())
        {
            cbModifyStatus.Items.Add(videoInfoStatus.ConfigurationName);
        }
    }

    private void AddData(object sender, string? textData)
    {
        if (textData != null)
        {
            if (sender.GetType() == typeof(Button))
            {
                var button = sender as Button;
                if (button != null)
                {
                    if (MessageBox.Show($"Add/Update {GetVideoInfoStatusByConfigurationName(button.Text).ConfigurationName} Data to Data Base?", "Add/Update Data", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        var results = _videoInfoManagerPresentationAppService.ProcessData(textData, button.Text);
                        if (string.IsNullOrEmpty(results) == false)
                        {
                            string state = results.Split('[', ']')[1];
                            VideoInfoStatus videoInfoStatus = GetVideoInfoStatusByStatusName(state);
                            results = results.Replace(state, videoInfoStatus.ConfigurationName);
                            MessageBox.Show(results);
                        }
                    }
                }
            }
            if (sender.GetType() == typeof(RichTextBox))
            {
                var rtb = sender as RichTextBox;
                if (rtb != null)
                {
                    rtb.Text += textData;
                }
            }
        }
    }

    private string? NormalizeAndConvertToStringWithNewLineSeparators(string[] data)
    {
        if (data is null)
        {
            return null;
        }
        var text = new StringBuilder();
        foreach (var file in data)
        {
            string normalizedFileName = _videoInfoManagerPresentationAppService.NormalizeFileName(file);
            text.Append($"{normalizedFileName}{Environment.NewLine}");
        }

        return text.ToString();
    }


    #endregion PRIVATE FUNCIONS

}
