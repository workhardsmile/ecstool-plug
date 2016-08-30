using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using System.ComponentModel.Design;
using System.Collections;
using System.IO;
using System.Diagnostics;

namespace PlugsRoot
{
    public interface IApplication : IServiceContainer
    {
        StatusStrip MyStatusStrip { get; }
        DockPanel MyDockPanel { get; }
        ArrayList aReturn { get; }
        string HomePath { get; }
        string CronPath { get; }
        void AddCommand(string cmdStr);
    }
    public interface IPlugin
    {
        void ShowOwn();
        string GetText();
        IApplication Application { get; set; }
    }
    public partial class PlugRoot : DockContent,IPlugin
    {
        public IApplication application = null;
        public PlugRoot()
        {
            InitializeComponent();
        }
        public void ShowOwn()
        {
            this.Show(application.MyDockPanel, DockState.Document);
        }
        public string GetText()
        {
            return this.Text;
        }
        public IApplication Application
        {
            get
            {
                return application;
            }
            set
            {
                application = value;
            }
        }
    }
    [AttributeUsage(AttributeTargets.Class)]
    public class PluginInfoAttribute : System.Attribute
    {
        public PluginInfoAttribute() { }
        public PluginInfoAttribute(string name, string version, string author, string group, bool loadWhenStart)
        {
            this._Name = name;
            this._Version = version;
            this._Author = author;
            this._Group = group;
            this._LoadWhenStart = loadWhenStart;
        }
        public string Name { get { return _Name; } }
        public string Version { get { return _Version; } }
        public string Author { get { return _Author; } }
        public string Group { get { return _Group; } }
        public bool LoadWhenStart { get { return _LoadWhenStart; } }
        ///
        /// 用来存储一些有用的信息
        ///
        public object Tag
        {
            get { return _Tag; }
            set { _Tag = value; }
        }
        ///
        /// 用来存储序号
        ///
        public int Index
        {
            get { return _Index; }
            set { _Index = value; }
        }

        private string _Name = "";
        private string _Version = "";
        private string _Author = "";
        private string _Group = "";
        private object _Tag = null;
        private int _Index = 0;
        // 暂时不会用
        private bool _LoadWhenStart = true;
    }
        public class LogManager
    {
        //<summary>
        //保存日志的文件夹
        //<summary>
        private string logPath = string.Empty;
        public string LogPath
        {
            get
            {
                if (logPath == string.Empty)
                {
                    if (AppDomain.CurrentDomain.BaseDirectory != null)
                    {
                        // winfrom 应用
                        logPath = AppDomain.CurrentDomain.BaseDirectory;
                    }
                    else
                    {
                        // web应用
                        logPath = AppDomain.CurrentDomain.BaseDirectory + @"bin\";
                    }
                }
                return logPath;
            }
            set 
            { 
                logPath = value;
            }
        }
        public LogManager(string logPath)
        {
            this.logPath = logPath;
            //if (!File.Exists(logPath))
            //{
            //    File.CreateText(logPath);
            //} 
        }
        //<summary>
        //写日志
        //<summary>
        public void WriteLog(string msg)
        {
            try
            {                               
                StreamWriter sw = File.AppendText(logPath);
                sw.WriteLine(msg);
                sw.Close();
            }
            catch (Exception)
            {
                throw;
            }
        }
        //<summary>
        //写日志
        //<summary>
        public void WriteLog(string logPath,string msg)
        {
            try
            {
                StreamWriter sw = File.AppendText(logPath);
                sw.WriteLine(msg);
                sw.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
        /// <summary>
        /// 有关对于TreeView节点的填充的相关处理类
        /// </summary>
        public class TreeViewUtils
        {
            #region 有关将整个系统盘符加载到TreeView顶级节点的处理方法
            /// <summary>
            /// 有关将整个系统盘符加载到TreeView顶级节点的处理方法
            /// </summary>
            /// <param name="treeView"></param>
            public static void LoadListListDrivers(TreeView treeView)
            {
                treeView.Nodes.Clear();
                DriveInfo[] drivers = DriveInfo.GetDrives();  //获得整个系统磁盘驱动

                //将盘符名加载到TreeView的顶级节点
                foreach (DriveInfo driver in drivers)
                {
                    TreeNode node = treeView.Nodes.Add(driver.Name, driver.Name, 2);
                }
                //填充TreeView的顶级节点下的子节点
                foreach (TreeNode node in treeView.Nodes)
                {
                    NodeUpdate(node);
                }

            }
            #endregion
            #region 更新TreeView下的节点(列出当前目录下的子目录)
            /// <summary>
            /// 更新TreeView下的节点(列出当前目录下的子目录)
            /// </summary>
            /// <param name="node">上层节点</param>
            public static void NodeUpdate(TreeNode node)
            {
                try
                {
                    node.Nodes.Clear();
                    if (Directory.Exists(node.FullPath))
                    {
                        //获得指定节点目录的目录对象
                        DirectoryInfo dir = new DirectoryInfo(node.FullPath);
                        //遍历该目录下的所有目录
                        foreach (DirectoryInfo d in dir.GetDirectories())
                        {
                            node.Nodes.Add(d.Name, d.Name, 0);    //向指定节点下添加文件目录节点
                        }
                        //遍历该目录下的所有文件
                        foreach (FileInfo f in dir.GetFiles())
                        {
                            node.Nodes.Add(f.Name, f.Name, 1); //向指定节点下添加文件节点
                        }
                    }                    

                    //递归更新所有子结点
                    //foreach (TreeNode no in node.Nodes)
                    //{
                    //    NodeUpdate(no);     //递归调用                    
                    //}
                }
                catch { }
            }
            #endregion

        }
        /// <summary> 
        /// Command 的摘要说明。 
        /// </summary> 
        public class WinCMD
        {
            public string ExePath;
            public string Command;
            public string Argument = "";
            public string Return;
            public int Second;
            /// <summary>
            /// 线程启动软件
            /// </summary>
            public void StartProgram()
            {
                StartExe(ExePath, Argument);
            }
            /// <summary>
            /// 线程执行DOS命令
            /// </summary>
            public void Execute()
            {
                Return = "";
                Return = Execute(Command, Second);
                //RunCmd(Command);
            }
            /// <summary>
            /// dosCommand Dos命令语句
            /// </summary>
            /// <param name="dosCommand"></param>
            /// <returns></returns>   
            public string Execute(string dosCommand)
            {
                return Execute(dosCommand, 300);
            }
            /// <summary>   
            /// 执行DOS命令，返回DOS命令的输出   
            /// </summary>   
            /// <param name="dosCommand">dos命令</param>   
            /// <param name="milliseconds">等待命令执行的时间（单位：秒），   
            /// 如果设定为0，则无限等待</param>   
            /// <returns>返回DOS命令的输出</returns>   
            public string Execute(string command, int seconds)
            {
                string output = "timeout!"; //输出字符串   
                if (command != null && !command.Equals(""))
                {
                    Process process = new Process();//创建进程对象   
                    ProcessStartInfo startInfo = new ProcessStartInfo();
                    startInfo.FileName = "cmd.exe";//设定需要执行的命令   
                    startInfo.Arguments = "/C " + command;//“/C”表示执行完命令后马上退出   
                    //startInfo.Arguments = command;
                    startInfo.UseShellExecute = false;//不使用系统外壳程序启动   
                    startInfo.RedirectStandardInput = false;//不重定向输入   
                    startInfo.RedirectStandardOutput = true; //重定向输出   
                    startInfo.CreateNoWindow = true;//不创建窗口   
                    process.StartInfo = startInfo;
                    try
                    {
                        if (process.Start())//开始进程   
                        {
                            if (seconds == 0)
                            {
                                process.WaitForExit();//这里无限等待进程结束   
                            }
                            else
                            {
                                process.WaitForExit(seconds * 1000); //等待进程结束，等待时间为指定的毫秒   
                            }
                            output = process.StandardOutput.ReadToEnd();//读取进程的输出
                        }
                    }
                    catch
                    {
                    }
                    finally
                    {
                        if (process != null)
                            process.Close();
                    }
                }
                return output;
            }

            public string cmd_str = "";
            public string cmd_outstr = "";
            /// <summary> 
            /// 执行CMD语句 
            /// </summary> 
            /// <param name="cmd">要执行的CMD命令</param> 
            public void RunCmd()
            {
                Process p = new Process();
                p.StartInfo.FileName = "cmd.exe";
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardInput = true;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.CreateNoWindow = true;
                p.Start();
                //if (!cmd_str.Contains("exit"))
                //{
                //    cmd_str = cmd_str + " && exit";
                //}
                p.StandardInput.WriteLine(cmd_str);
                p.StandardInput.WriteLine("exit");
                p.WaitForExit();
                cmd_outstr = p.StandardOutput.ReadToEnd();
                p.Close();
            }
            /// <summary> 
            /// 执行CMD语句 
            /// </summary> 
            /// <param name="cmd">要执行的CMD命令</param> 
            public string RunCmd(string cmd)
            {
                Process p = new Process();
                p.StartInfo.FileName = "cmd.exe";
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardInput = true;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.CreateNoWindow = false;
                p.Start();
                //if (!cmd.Contains("exit"))
                //{
                //    cmd = cmd + " && exit";
                //}
                p.StandardInput.WriteLine(cmd);
                p.StandardInput.WriteLine("exit");
                p.WaitForExit();
                string temp = p.StandardOutput.ReadToEnd();
                p.Close();
                return temp;
            }
            /// <summary> 
            /// 打开软件 
            /// </summary> 
            /// <param name="exePath">软件路径加名称（.exe文件）</param> 
            public void RunProgram(string exePath)
            {
                RunProgram(exePath, "");
            }
            /// <summary> 
            /// 打开软件并执行命令（输入) 
            /// </summary> 
            /// <param name="exePath">软件路径加名称（.exe文件）</param> 
            /// <param name="cmd">要执行的命令</param> 
            public void RunProgram(string exePath, string cmd)
            {
                Process proc = new Process();
                proc.StartInfo.CreateNoWindow = true;
                proc.StartInfo.FileName = exePath;
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.RedirectStandardError = true;
                proc.StartInfo.RedirectStandardInput = true;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.Start();
                if (cmd.Length != 0)
                {
                    proc.StandardInput.WriteLine(cmd);
                }
                proc.Close();
            }
            /// <summary>
            /// 打开软件
            /// </summary>
            /// <param name="exePath"></param>
            /// <returns></returns>
            public string StartExe(string exePath)
            {
                string output = "";
                try
                {
                    Process cmd = new Process();
                    cmd.StartInfo.FileName = exePath;
                    cmd.StartInfo.UseShellExecute = false;
                    cmd.StartInfo.RedirectStandardInput = true;
                    cmd.StartInfo.RedirectStandardOutput = true;
                    cmd.StartInfo.CreateNoWindow = true;
                    cmd.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    cmd.Start();
                    output = cmd.StandardOutput.ReadToEnd();
                    //Console.WriteLine(output);
                    cmd.WaitForExit();
                    cmd.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                return output;
            }
            /// <summary>
            /// 打开软件（内置参数）
            /// </summary>
            /// <param name="exePath"></param>
            /// <param name="argument"></param>
            /// <returns></returns>
            public string StartExe(string exePath, string argument)
            {
                string output = "";
                try
                {
                    Process cmd = new Process();
                    cmd.StartInfo.FileName = exePath;
                    cmd.StartInfo.Arguments = argument;
                    cmd.StartInfo.UseShellExecute = false;
                    cmd.StartInfo.RedirectStandardInput = true;
                    cmd.StartInfo.RedirectStandardOutput = true;
                    cmd.StartInfo.CreateNoWindow = true;
                    cmd.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    cmd.Start();
                    output = cmd.StandardOutput.ReadToEnd();
                    //Console.WriteLine(output);
                    cmd.WaitForExit();
                    cmd.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                return output;
            }
        }
}
