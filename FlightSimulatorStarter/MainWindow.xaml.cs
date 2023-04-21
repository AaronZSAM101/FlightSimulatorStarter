using Microsoft.Win32;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;

namespace SimulatorStarterWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<StartItem> StartItems { get; set; } = new(); // 定义了一个列表，名称StartItems

        public MainWindow()
        {
            InitializeComponent();
        }
        // bool First(StartItem t)
        // {
        //    return t.IsSelected; // 返回了一个bool类型值（根据Selected筛选的依据），类似于一个筛选函数。
        // }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in StartItems.Where(t => t.IsSelected).Where(t => !string.IsNullOrWhiteSpace(t.Path)))
            // LINQ (C#特色，类似SQL语句，筛选/选择你想要的东西），foreach（遍历循环）
            // Where后面的这个东西相当于是判断，也就是判断表达式；t代表的是整个序列中的其中一个，不代表任何值（也就是假定这个t为StartItems中的某一个）
            {
                Process.Start(item.Path);  // 执行启动事件
            }
        }

        /// <summary>
        /// 导入配置文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //点下这个按钮
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Filter = "json文件|*.json",
                Title = "选择配置文件",
                Multiselect = false,
            };
            //→ 出现选择配置文件的提示框
            if (openFileDialog.ShowDialog() == true)
            {
                try
                {

                    string Content = File.ReadAllText(openFileDialog.FileName); // 把冷菜放到一个碗里
                    List<StartItem> Configs = JsonSerializer.Deserialize<List<StartItem>>(Content)!; // 把热过的菜丢到另一个碗里
                    StartItems.Clear(); // 清空原本的List
                    foreach (var item in Configs)
                    {
                        StartItems.Add(item); // 把热过的菜端上桌
                    }

                    ConfigNames.Content = $"配置文件名称：{Path.GetFileNameWithoutExtension(openFileDialog.FileName)}";
                }
                catch
                {
                    MessageBox.Show("配置文件读取失败，请检查配置文件是否正确", "", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            //→ 选择文件后，1.右边的Label会显示该配置文件的名称；2.List会被配置文件的内容所替换

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            // 点下按钮后，出现保存文件的提示框
            SaveFileDialog sfd = new()
            {
                AddExtension = true,
                Filter = "json文件|*.json",
                Title = "保存配置文件"
            };
            if (sfd.ShowDialog() == true)
            {
                // 保存文件逻辑：把List的内容转换成json格式，再写入文件
                try
                {
                    string Profile = JsonSerializer.Serialize(StartItems, options: new JsonSerializerOptions()
                    {
                        WriteIndented = true,
                    });
                    File.WriteAllText(sfd.FileName, Profile);
                    MessageBox.Show("配置文件保存成功，以后要多瘾哦！");
                }
                catch
                {
                    MessageBox.Show("配置文件保存失败，请检查参数输入是否正确", "", MessageBoxButton.OK, MessageBoxImage.Warning);
                }


            }

            // 文件保存后，跳提示框：文件保存成功
        }
    }
    public class StartItem : INotifyPropertyChanged // 属性改变时提示：达成一个协议，当属性改变时，我提醒你改变了（相当于触发了PropertyChanged这个事件）
    {
        private bool isSelected = true; // 默认全选
        private string path = string.Empty; // 路径默认为空
        private string name = string.Empty; // 名称默认为空

        public bool IsSelected
        {
            get => isSelected;
            set
            {
                isSelected = value; // 设置值
                PropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(IsSelected))); // 触发事件
            }
        }
        public string Path
        {
            get => path;
            set
            {
                path = value; // 设置值
                PropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(Path))); // 触发事件
            }
        }

        public string Name
        {
            get => name;
            set
            {
                name = value;  // 设置值
                PropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(Name))); // 触发事件
            }
        }
        public event PropertyChangedEventHandler? PropertyChanged;

    }
}
