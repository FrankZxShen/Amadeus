using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System.Threading;

public class VITSRunner : MonoBehaviour
{
    private Process vitsProcess; // 声明为成员变量，以便在其他方法中引用
    private Thread processThread; // 子线程

    public void ActivateVits(bool isChecked)
    {
        if (isChecked)
        {
            // 指定要执行的命令和参数
            string command = "python";
            string arguments = "app_c.py";

            // 创建一个新的进程来执行命令行程序
            vitsProcess = new Process();
            vitsProcess.StartInfo.FileName = "cmd.exe";
            vitsProcess.StartInfo.WorkingDirectory = @"B:\AI\Amadeus\VITS\vits-uma-genshin-honkai2\vits-uma-genshin-honkai";
            vitsProcess.StartInfo.Arguments = "/k activate vits2 && " + command + " " + arguments;
            vitsProcess.StartInfo.UseShellExecute = false;
            vitsProcess.StartInfo.RedirectStandardOutput = true;
            vitsProcess.StartInfo.CreateNoWindow = true;

            // 创建子线程执行命令行操作
            processThread = new Thread(StartProcess);
            processThread.Start();
        }
        else
        {
            // 取消勾选时关闭进程和子线程
            if (vitsProcess != null && !vitsProcess.HasExited)
            {
                vitsProcess.CloseMainWindow(); // 尝试通过关闭主窗口方式关闭进程
                vitsProcess.Close(); // 关闭进程
            }
            if (processThread != null && processThread.IsAlive)
            {
                processThread.Abort(); // 终止子线程
            }
        }
    }

    private void StartProcess()
    {
        // 开始执行命令行程序
        UnityEngine.Debug.Log("Starting process...");
        vitsProcess.Start();
        UnityEngine.Debug.Log("Process started.");

        // 读取命令输出
        string output = vitsProcess.StandardOutput.ReadToEnd();
        UnityEngine.Debug.Log(output);

        // 等待命令行程序执行完成
        vitsProcess.WaitForExit();
    }
}
