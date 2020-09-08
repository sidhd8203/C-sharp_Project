using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.IO;
using System.Threading;
using action_list.Action;

namespace action_list
{
    class Program
    {
       

        /***　メイン　***/
        static void Main(string[] args)
        {
            DeleteFolder df = new DeleteFolder();

            //何日前のフォルダを削除するか設定
            int del_days = 1;
            //設定した日付前のフォルダを削除するメソッド
            if (df.Delete_Folder(del_days))
            {
                Console.WriteLine("{0}分以前のフォルダを完全削除しました。", (DateTime.Now + TimeSpan.FromDays(-del_days)).ToString("yyyyMMdd"));
            }

            // 時間の差をもってバッジを実行するようタイマー用意
            System.Timers.Timer timer = new System.Timers.Timer();

            // 時間の差を設定
            timer.Interval = 1 * 60 * 1000; //30分

            Timer_action action = new Timer_action();

            //イベントハンドラー設定
            timer.Elapsed += new ElapsedEventHandler(action.timer_Elapsed);
            timer.Start();

            //終了する時に使う構文
            Console.WriteLine("Press Enter to exit");
            Console.ReadLine();

        }

        

    }
}