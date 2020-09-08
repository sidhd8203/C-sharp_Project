using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace action_list.Action
{
    class DeleteFolder
    {

        /*  期間が過ぎたフォルダは削除　 */
        public bool Delete_Folder(int del_days)
        {
            //削除する日付設定
            string del_Date = DateTime.Today.AddDays(-del_days).ToString("yyyyMMdd");

            //今日と過ぎているか比較
            if (del_Date != DateTime.Now.ToString("yyyyMMdd"))
            {
                //削除日付フォルダを設定
                string delfolderName = System.Environment.CurrentDirectory + @"\" + DateTime.Today.AddDays(-del_days).ToString("yyyy") + @"\" +
                     DateTime.Today.AddDays(-del_days).ToString("MM");

                //エラー対応
                try
                {
                    DirectoryInfo di = new DirectoryInfo(delfolderName);

                    //フォルダがあるか？
                    if (di.Exists)
                    {
                        //フォルダ内容を配列に格納
                        DirectoryInfo[] dirInfo = di.GetDirectories();

                        //内容の中に最後に書かれたものを削除するための繰り返し文
                        foreach (DirectoryInfo dir in dirInfo)
                        {
                            //フォルダ名が過ぎてるか？確認作業
                            if (del_Date.CompareTo(dir.Name) > 0)
                            {
                                //読み書きの設定があるファイルは状態をノーマルに変更(削除するために)
                                dir.Attributes = FileAttributes.Normal;
                                //削除を行う
                                Console.WriteLine("{0}のフォルダ削除", dir.Name);
                                dir.Delete(true);
                            }
                        }
                    }
                }
                catch (Exception) { }

                //削除が正常に終わるとtrueをリターンする
                return true;
            }

            //削除するものがなければfalseをリターンする
            return false;
        }

    }
}
