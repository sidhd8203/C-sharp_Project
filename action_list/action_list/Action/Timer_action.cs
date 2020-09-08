using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace action_list.Action
{
    class Timer_action
    {
        public static List<string> pathList = new List<string>();
        public static void save_pathList(string filename)
        {
            if (pathList.Count > 10)
            {
                pathList.RemoveRange(0, 2);
            }
            pathList.Add(filename);
        }

        
        public struct TaskList
        {
            public string pid;
            public string system_name;
            public string memory;

            public TaskList(string pid, string system_name, string memory)
            {
                this.pid = pid;
                this.system_name = system_name;
                this.memory = memory;
            }

            public void Print()
            {
                Console.WriteLine("{0}\t{1}\t{2}", system_name, pid, memory);
            }
        }

        /** イベントハンドらー設定構文　**/
        public void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            //現在実行ファイルにあるバッジ実行
            String startpath = System.Environment.CurrentDirectory + @"\TaskList_BackupTool.bat";
            FileInfo di = new FileInfo(startpath);

            if (!di.Exists)
            {
                using (StreamWriter fs = new StreamWriter(new FileStream(startpath, FileMode.Create, FileAccess.Write)))
                {

                fs.WriteLine(@"@echo off");
                fs.WriteLine(@"set CURPATH=%cd%\%date:~0,4%");
                fs.WriteLine(@"set hour=%time:~0,2%");
                fs.WriteLine(@"if ""%time:~0,1%""=="" "" set hour=0%time:~1,1%");
                fs.WriteLine(@"set folder_save=%CURPATH%\%date:~5,2%\%date:~0,4%%date:~5,2%%date:~8,2%");
                fs.WriteLine(@"set logfolder_save=%CURPATH%\%date:~5,2%\%date:~0,4%%date:~5,2%%date:~8,2%log");
                fs.WriteLine(@"md %folder_save%");
                fs.WriteLine(@"md %logfolder_save%");
                fs.WriteLine(@"set filename=%CURPATH%\%date:~5,2%\%date:~0,4%%date:~5,2%%date:~8,2%\%hour%%time:~3,2%");
                fs.WriteLine(@"tasklist > %filename%.txt");
                Console.WriteLine("bat作成完了");

                fs.Close();

                }       
            }
                        
            
            Process.Start(startpath);

            //ファイル名を表示する文
            String time = DateTime.Now.ToString("yyyyMMdd_HHmm");
            Console.WriteLine("{0}にバックアップを取りました。", time);


            //バックアップのファイルがあるパス
            string filename = System.Environment.CurrentDirectory + @"\" + DateTime.Now.ToString("yyyy") + @"\" + DateTime.Now.ToString("MM") + @"\"
                 + DateTime.Now.ToString("yyyyMMdd") + @"\" + DateTime.Now.ToString("HHmm") + @".txt";

            //ファイルパスを比較するためにセーブ
            save_pathList(filename);

            //バックアップファイルが生成まで時間がかかるため遅延：５秒
            Thread.Sleep(5000);

            //ファイル読み取りのメソッド
            List<TaskList> list = new List<TaskList>();
            list = Read_TaskList(filename);

            if (pathList.Count > 2)
            {
                //一つのファイルをよこす。比較する。
                check_compare(list);
            }
        }

        /**　ファイル読み取りのメソッド　**/
        public List<TaskList> Read_TaskList(string FilePath)
        {
            string[] file_val;
            List<TaskList> list = new List<TaskList>();

            //ファイルclose忘れ防止のため　using使用
            using (StreamReader fs = new StreamReader(new FileStream(FilePath, FileMode.Open, FileAccess.Read)))
            {

                //不必要なものを先に外す  (題名/======←文)
                for (int i = 0; i < 3; i++) { fs.ReadLine(); }
                //ファイルが終わるまで配列に格納
                file_val = fs.ReadToEnd().Split('\n');

                for (int i = 0; i < file_val.Length; i++)
                {
                    if (file_val[i].Length != 0)
                    {
                        TaskList tl = new TaskList();
                        tl.pid = file_val[i].Substring(26, 8);
                        tl.system_name = file_val[i].Substring(0, 25);
                        tl.memory = file_val[i].Substring(64, 10);
                        list.Add(tl);
                    }                    
                }
                return list;
            }



        }

        //1次検証
        public void check_compare(List<TaskList> filelist)
        {                        
                List<TaskList> list1, list2, list3 = new List<TaskList>();

                bool flag = false;

                //新しくできたファイル
                list1 = filelist;

                //既存のファイル
                list2 = Read_TaskList(pathList[pathList.Count-2]);

                //整列
                list1.Sort((x1, x2) => x1.pid.CompareTo(x2.pid));
                list2.Sort((x1, x2) => x1.pid.CompareTo(x2.pid));

                //比較
                if (list1.Count < list2.Count)
                {
                    for (int i = 0; i < list2.Count; i++)
                    {
                        for (int j = 0; j < list1.Count; j++)
                        {
                            if (list1[j].pid == list2[i].pid　)
                            {
                                if (list1[j].memory == list2[i].memory)
                                {
                                    flag = true;
                                    TaskList tl;
                                    tl.pid = list2[j].pid;
                                    tl.system_name = list2[j].system_name;
                                    tl.memory = list2[j].memory;

                                    tl = new TaskList(tl.pid, tl.system_name, tl.memory);

                                    list3.Add(tl);

                                }
                            }
                        }
                            
                    }
                }
                else
                {
                    for (int i = 0; i < list1.Count; i++)
                    {
                        for (int j = 0; j < list2.Count; j++)
                        {
                            if (list1[i].pid == list2[j].pid)
                            {
                                if (list1[i].memory == list2[j].memory)
                                {
                                    flag = true;
                                    TaskList tl;
                                    tl.pid = list2[j].pid;
                                    tl.system_name = list2[j].system_name;
                                    tl.memory = list2[j].memory;

                                    tl = new TaskList(tl.pid, tl.system_name, tl.memory);

                                    list3.Add(tl);
                                }
                            }
                        }

                    }

                }


                if (flag)
                {
                    if (pathList.Count > 3)
                    {
                        List<TaskList> list = new List<TaskList>();
                        next_compare(list3);
                    }
                    else
                    {
                        log_fileWrite(list3);
                    }                  
                    
                }
                


        }

        //2次検証
        public void next_compare(List<TaskList> filelist)
        {
            List<TaskList> list1, list2, list3 = new List<TaskList>();

            list2 = Read_TaskList(pathList[pathList.Count - 3]);

            if (filelist.Count < list2.Count)
            {
                list1 = list2;
                list2 = filelist;
            }
            else
            {
                list1 = filelist;
            }

            list1.Sort((x1, x2) => x1.pid.CompareTo(x2.pid));
            list2.Sort((x1, x2) => x1.pid.CompareTo(x2.pid));

            for (int i = 0; i < list1.Count; i++)
            {
                for (int j = 0; j < list2.Count; j++)
                {
                    if (list1[i].pid == list2[j].pid)
                    {
                        if (list1[i].memory == list2[j].memory)
                        {
                            TaskList tl;
                            tl.pid = list2[j].pid;
                            tl.system_name = list2[j].system_name;
                            tl.memory = list2[j].memory;

                            tl = new TaskList(tl.pid, tl.system_name, tl.memory);

                            list3.Add(tl);
                        }
                    }
                }
            }

            if (list3.Count != 0)
            {
                if (pathList.Count > 4)
                {
                    third_compare(list3);
                }
                else
                {
                    log_fileWrite(list3);
                }
                
            }
            
        }

        //3次検証
        public void third_compare(List<TaskList> filelist)
        {
            List<TaskList> list1, list2, list3 = new List<TaskList>();

            list2 = Read_TaskList(pathList[pathList.Count - 4]);

            if (filelist.Count < list2.Count)
            {
                list1 = list2;
                list2 = filelist;
            }
            else
            {
                list1 = filelist;
            }

            list1.Sort((x1, x2) => x1.pid.CompareTo(x2.pid));
            list2.Sort((x1, x2) => x1.pid.CompareTo(x2.pid));

            for (int i = 0; i < list1.Count; i++)
            {
                for (int j = 0; j < list2.Count; j++)
                {
                    if (list1[i].pid == list2[j].pid)
                    {
                        if (list1[i].memory == list2[j].memory)
                        {
                            TaskList tl;
                            tl.pid = list2[j].pid;
                            tl.system_name = list2[j].system_name;
                            tl.memory = list2[j].memory;

                            tl = new TaskList(tl.pid, tl.system_name, tl.memory);

                            list3.Add(tl);
                        }
                    }
                }
            }

            if (list3.Count != 0)
            {
                log_fileWrite(list3);
            }
        }


        public void log_fileWrite(List<TaskList> list)
        {
            string FilePath = System.Environment.CurrentDirectory + @"\" + DateTime.Now.ToString("yyyy") + @"\" + DateTime.Now.ToString("MM") + @"\"
                 + DateTime.Now.ToString("yyyyMMdd") + @"log\" + DateTime.Now.ToString("yyyyMMddHHmm") + @"log.txt";

            using (StreamWriter fs = new StreamWriter(new FileStream(FilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite)))
            {
                foreach (TaskList line in list)
                {
                    fs.WriteLine("{0}\t{1}\t{2}", line.system_name, line.pid, line.memory);
                }

                Console.WriteLine("logファイル作成完了");

                fs.Close();
            }
        }
    }
}
