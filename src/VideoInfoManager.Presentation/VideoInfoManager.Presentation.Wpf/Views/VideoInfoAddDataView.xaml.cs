using System.Windows;
using System.Windows.Controls;
using VideoInfoManager.Presentation.CrossCutting.Extensions;
using VideoInfoManager.Presentation.Wpf.ViewModels;

namespace VideoInfoManager.Presentation.Wpf.Views;

public partial class VideoInfoAddDataView : UserControl
{
    private readonly VideoInfoAddDataViewModel? _videoInfoAddData;
    public VideoInfoAddDataView()
    {
        _videoInfoAddData = DependencyInjectionExtensions.GetService<VideoInfoAddDataViewModel>();
        this.DataContext = _videoInfoAddData;
        InitializeComponent();
    }

    private void MultiSearchTextBox_PreviewDragOver(object sender, DragEventArgs e)
    {
        e.Handled = true;
    }

    private void AddData_DragDrop(object sender, DragEventArgs e)
    {
        _videoInfoAddData?.AddData_DragDrop(sender, e);
    }

}