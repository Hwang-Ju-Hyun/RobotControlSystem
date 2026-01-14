using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ControlServer;
using Core_lib.Core.Domain;
using ControlCenter.Server;
using static Core_lib.Core.Domain.Node;

namespace ControlCenter
{    
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Process> clientProcess;        
        public MainWindow()
        {
            InitializeComponent();
            clientProcess = new List<Process>();
        }
        private void OnNodeClicked(object sender, MouseButtonEventArgs e)
        {
            if (sender is Rectangle rect && rect.DataContext is Node node)
            {
                if (Keyboard.IsKeyDown(Key.LeftShift))
                    node.Type = NodeType.START;
                else if (Keyboard.IsKeyDown(Key.LeftCtrl))
                    node.Type = NodeType.GOAL;
                else
                    node.Type = node.Type == NodeType.OBSTACLE ? NodeType.EMPTY : NodeType.OBSTACLE;
            }
        }
        private void OnStartServer(object sender, RoutedEventArgs e)
        {
            try
            {
                //server = new ControlServer.Server();
                if(DataContext is MainViewModel vm)
                {
                    vm.server.Start();
                }                
                // 실행 확인을 위한 로그 (선택 사항)
                MessageBox.Show("서버가 시작되었습니다.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"서버 실행 실패: {ex.Message}");
            }
        }
        private void OnConnectClient(object sender, RoutedEventArgs e)
        {
            try
            {
                Process p = new Process();
                p.StartInfo.FileName = "RobotClient.exe";

                string path = System.IO.Path.GetFullPath(@"C:\Users\HwangJuhyun\Desktop\HwangJuhyun\MyCoding\RobotControlSimulator\RobotControlSystemSimulator\RobotClient\bin\Debug\net8.0");
                p.StartInfo.WorkingDirectory = path;

                p.StartInfo.UseShellExecute = true; // 콘솔 창을 별도로 띄우려면 true 권장
                p.StartInfo.CreateNoWindow = false;

                p.Start();

                clientProcess.Add(p);

                MessageBox.Show("서버 접속 성공");


            }
            catch (Exception ex)
            {
                MessageBox.Show($"서버 접속 실패: {ex.Message}");
            }
        }
        private void StartPathFind(object sender, RoutedEventArgs e)
        {
            try
            {
                if(this.DataContext is MainViewModel vm)
                {
                    vm.StartPathFinding();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}