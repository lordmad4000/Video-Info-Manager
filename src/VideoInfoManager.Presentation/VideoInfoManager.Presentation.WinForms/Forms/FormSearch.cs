using System.ComponentModel;
using System.Text;
using System.Data;
using VideoInfoManager.Application.Interfaces;
using VideoInfoManager.Application.DTOs;
using VideoInfoManager.Presentation.WinForms.Helpers;
using VideoInfoManager.Domain.Enums;
using Microsoft.Extensions.Configuration;
using VideoInfoManager.Presentation.WinForms.Models;
using VideoInfoManager.Application.Models;

namespace VideoInfoManager.Presentation.WinForms.Forms;

public partial class FormSearch : Form
{
    private const int MinSearchLength = 3;

    private List<VideoInfoStatus> _videoInfoStatuses = new List<VideoInfoStatus>();
    private string[]? _lastSearch { get; set; } = null;
    private IEnumerable<VideoInfoDTO>? _lastSearchData { get; set; } = null;
    private VideoInfoDTO? _videoInfoSelected { get; set; } = null;

    private readonly IVideoInfoAppService _videoInfoAppService;
    private readonly IVideoInfoManagerAppService _videoInfoManagerAppService;
    private readonly IConfiguration _configuration;
    private readonly VideoInfoRenameConfiguration[]? _videoInfoRenameConfigurations;

    public FormSearch(IVideoInfoAppService videoInfoAppService, IVideoInfoManagerAppService videoInfoManagerAppService, IConfiguration configuration)
    {
        _videoInfoAppService = videoInfoAppService;
        _videoInfoManagerAppService = videoInfoManagerAppService;
        _configuration = configuration;
        _videoInfoRenameConfigurations = _configuration.GetSection("VideoInfoRenameConfiguration").Get<VideoInfoRenameConfiguration[]>();
        Initialize();
    }

    #region PRIVATE EVENTS
    private void FormSearch_ResizeEnd(object sender, EventArgs e) => ResizeControls();
    private void FormSearch_Resize(object sender, EventArgs e) => ResizeControls();
    private void btnClear_Click(object sender, EventArgs e) => rtbVideoInfo.Text = "";
    private void menuItemModify_Click(object sender, EventArgs e) => ShowModify();
    private void btnModifyCancel_Click(object sender, EventArgs e)
    {
        ShowSearch();
        Search(_lastSearch, true);
    }

    private void btnModifySave_Click(object sender, EventArgs e)
    {
        var videoInfoDTO = new VideoInfoDTO
        {
            Id = new Guid(tbModifyId.Text),
            Name = tbModifyName.Text,
            Status = cbModifyStatus.SelectedText,

        };
        if (MessageBox.Show($"Update Data to Data Base?", "Update Data", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
        {
            if (_videoInfoAppService.Update(videoInfoDTO) == true)
            {
                MessageBox.Show($"{videoInfoDTO.Name} Updated.");
            }
        }

        ShowSearch();
        Search(_lastSearch, true);
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
        if (_lastSearchData != null && _lastSearchData.Count() > 0)
        {
            AddDataTodgvVideoInfo(_lastSearchData);
        }
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

    private void btnPastertbVideoInfo_Click(object sender, EventArgs e)
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
                    string videoName = _videoInfoManagerAppService.RenameVideoInfoName(item, _videoInfoRenameConfigurations);
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
                        text.Append($"{_videoInfoAppService.NormalizeFileName(file)}{Environment.NewLine}");
                    }
                }
                rtbVideoInfo.Text += text;
            }
        }
    }

    private void btnCutFirst_Click(object sender, EventArgs e)
    {
        string text = rtbVideoInfo.Text;
        if (String.IsNullOrWhiteSpace(text) is false)
        {
            var splitText = text.Split("\n", StringSplitOptions.RemoveEmptyEntries);
            if (splitText.Length > 0)
            {
                string cutText = splitText[0];
                rtbVideoInfo.Text = "";
                for (int i = 1; i < splitText.Length; i++)
                {
                    rtbVideoInfo.Text += $"{splitText[i]}{Environment.NewLine}";
                }
                Clipboard.SetText(cutText);
            }
        }
    }

    private void btnSearch_Click(object sender, EventArgs e)
    {
        string text = rtbVideoInfo.Text;
        if (String.IsNullOrWhiteSpace(text) is false)
        {
            var splitText = text.Split("\n", StringSplitOptions.RemoveEmptyEntries);
            if (splitText.Length > 0)
            {
                Search(splitText, true);
            }
        }

        ShowSearch();
    }

    private void AddData_Click(object sender, EventArgs e)
    {
        if (Clipboard.ContainsText())
        {
            string clipBoardText = Clipboard.GetText();
            if (string.IsNullOrEmpty(clipBoardText) == false)
            {
                var data = new DataObject();
                data.SetData("Text", clipBoardText);
                var dragEventArgs = new DragEventArgs(data, 0, 0, 0, DragDropEffects.All, DragDropEffects.Copy);
                AddData_DragDrop(sender, dragEventArgs);
            }
        }
        if (Clipboard.ContainsFileDropList())
        {
            var fileDrop = Clipboard.GetData("FileDrop");
            if (fileDrop != null)
            {
                var data = new DataObject();
                data.SetData("FileDrop", fileDrop);
                var dragEventArgs = new DragEventArgs(data, 0, 0, 0, DragDropEffects.All, DragDropEffects.Copy);
                AddData_DragDrop(sender, dragEventArgs);
            }
        }
    }

    private void AddData_DragEnter(object sender, DragEventArgs e)
    {
        if (e != null && e.Data != null)
        {
            if (e.Data.GetDataPresent(DataFormats.Text))
            {
                e.Effect = DragDropEffects.Copy;
            }
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }
    }

    private void AddData_DragDrop(object sender, DragEventArgs e)
    {
        if (e.Data == null)
        {
            return;
        }
        string? textData = null;
        if (e.Data.GetDataPresent(DataFormats.Text))
        {
            var dragDropData = e.Data.GetData(DataFormats.Text);
            if (dragDropData != null)
            {
                textData = dragDropData.ToString();
            }
        }
        else if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            var dragDropData = e.Data.GetData(DataFormats.FileDrop, true);
            if (dragDropData != null)
            {
                var fileList = (string[])dragDropData;
                var text = new StringBuilder();
                foreach (var file in fileList)
                {
                    string normalizedFileName = _videoInfoAppService.NormalizeFileName(file);
                    text.Append($"{normalizedFileName}{Environment.NewLine}");
                }
                textData = text.ToString();
            }
        }
        if (textData != null)
        {
            if (sender.GetType() == typeof(Button))
            {
                var button = sender as Button;
                if (button != null)
                {
                    if (MessageBox.Show($"Add/Update {GetVideoInfoStatusByConfigurationName(button.Text).ConfigurationName} Data to Data Base?", "Add/Update Data", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        var results = _videoInfoManagerAppService.ProcessData(textData, GetVideoInfoStatusByConfigurationName(button.Text).Status);
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

    private void btnExport_Click(object sender, EventArgs e)
    {
        var videoInfoDTOs = _videoInfoAppService.GetManyContains("");

        if (videoInfoDTOs is null || videoInfoDTOs.Count() == 0)
        {
            MessageBox.Show("No data found.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        List<VideoInfoDTO> sortedVideoInfo = videoInfoDTOs.OrderBy(c => c.Name)                                                         
                                                          .ToList();

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

        if (_videoInfoAppService.Remove(_videoInfoSelected.Id) == true)
        {
            MessageBox.Show($"{_videoInfoSelected.Name} Removed.");
            if (_lastSearch?.Count() > 0)
                Search(_lastSearch);
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

    private void Search(string[]? search, bool isVideoName = false)
    {
        if (search is null || search.Count() == 0)
            return;

        _lastSearch = search;
        _lastSearchData = isVideoName == true
                     ? _videoInfoManagerAppService.GetManyVideoInfo(search, _videoInfoRenameConfigurations)
                     : _videoInfoAppService.GetManyContains(search[0]);

        AddDataTodgvVideoInfo(_lastSearchData);
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

    private void AddDataTodgvVideoInfo(IEnumerable<VideoInfoDTO> videoInfoDTOs)
    {
        try
        {
            var data = videoInfoDTOs.Where(c => GetActiveStatus().Contains(c.StatusToVideoInfoStatusEnum())).ToList();
            var multipleAuthors = data.Where(c => _videoInfoManagerAppService.IsMultipleAuthor(c.Name, _videoInfoRenameConfigurations));
            var singleAuthor = data.Where(c => _videoInfoManagerAppService.IsMultipleAuthor(c.Name, _videoInfoRenameConfigurations) is false);
            var videoInforSearchList = new List<VideoInfoDTO>();

            if (singleAuthor.Any())
            {
                videoInforSearchList.AddRange(singleAuthor.OrderBy(c => c.Name)
                                                          .ThenByDescending(c => c.Status));
            }
            if (multipleAuthors.Any())
            {
                videoInforSearchList.AddRange(multipleAuthors.OrderBy(c => c.Name)
                                                             .ThenByDescending(c => c.Status));
            }

            videoInforSearchList = GetVideoInfoWithConfigurationNames(videoInforSearchList).ToList();
            var bindingList = new BindingList<VideoInfoDTO>(videoInforSearchList);
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

    private void Initialize_videoInfoStatuses()
    {
        string[]? configurationStatusNames = _configuration.GetSection("StatusNames").Get<string[]>();
        var statuses = Enum.GetValues(typeof(VideoInfoStatusEnum))
                           .Cast<VideoInfoStatusEnum>();

        foreach (var (status, index) in statuses.Select((status, index) => (status, index)))
        {
            var videoInfoStatus = new VideoInfoStatus
            {
                ConfigurationName = status.ToString(),
                StatusName = status.ToString(),
                Status = status
            };

            if (configurationStatusNames?.Length > index)
            {
                videoInfoStatus.ConfigurationName = configurationStatusNames[index];
            }

            _videoInfoStatuses.Add(videoInfoStatus);
        }
    }

    private string[] GetStatusNames(string[]? configurationStatusNames)
    {
        var statusNames = Enum.GetNames(typeof(VideoInfoStatusEnum)).ToArray();

        if (configurationStatusNames?.Length == statusNames.Length)
        {
            for (int i = 0; i < statusNames.Length; i++)
            {
                statusNames[i] = configurationStatusNames[i];
            }
        }

        return statusNames;
    }

    private string[] GetVideoInfoStatusEnumNames() =>
        Enum.GetNames(typeof(VideoInfoStatusEnum)).ToArray();


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

    private VideoInfoStatus GetVideoInfoStatusByStatus(VideoInfoStatusEnum status)
    {
        var videoinfoStatus = _videoInfoStatuses.FirstOrDefault(c => c.Status.Equals(status));
        if (videoinfoStatus is null)
            return new VideoInfoStatus();

        return videoinfoStatus;

    }

    private VideoInfoStatus GetVideoInfoStatusByConfigurationName(string configurationName)
    {
        VideoInfoStatus? videoinfoStatus = _videoInfoStatuses.FirstOrDefault(c => c.ConfigurationName.Equals(configurationName));
        if (videoinfoStatus is null)
            return new VideoInfoStatus();

        return videoinfoStatus;
    }

    private VideoInfoStatus GetVideoInfoStatusByStatusName(string statusName)
    {
        VideoInfoStatus? videoinfoStatus = _videoInfoStatuses.FirstOrDefault(c => c.StatusName.Equals(statusName));
        if (videoinfoStatus is null)
            return new VideoInfoStatus();

        return videoinfoStatus;
    }

    private int GetStatusIndexByName(string configurationName)
    {
        int index = _videoInfoStatuses.FindIndex(c => c.ConfigurationName.Equals(configurationName));

        if (index == -1)
            return 0;

        return index;
    }

    private void Initialize()
    {
        InitializeComponent();
        Initialize_videoInfoStatuses();
        InitializeStatusComponents();
        InitializeMultipleSearchBox();
        InitializeContextMenu();
        InitializeWindow();
        ResizeControls();
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
        btnPended.Text = _videoInfoStatuses[0].ConfigurationName;
        btnSaved.Text = _videoInfoStatuses[1].ConfigurationName;
        btnBackuped.Text = _videoInfoStatuses[2].ConfigurationName;
        btnDeleted.Text = _videoInfoStatuses[3].ConfigurationName;
        btnLowed.Text = _videoInfoStatuses[4].ConfigurationName;
        cbP.Text = _videoInfoStatuses[0].ConfigurationName;
        cbS.Text = _videoInfoStatuses[1].ConfigurationName;
        cbB.Text = _videoInfoStatuses[2].ConfigurationName;
        cbD.Text = _videoInfoStatuses[3].ConfigurationName;
        cbL.Text = _videoInfoStatuses[4].ConfigurationName;

        foreach (var videoInfoStatus in _videoInfoStatuses)
        {
            cbModifyStatus.Items.Add(videoInfoStatus.ConfigurationName);
        }
    }

    #endregion PRIVATE FUNCIONS
}
