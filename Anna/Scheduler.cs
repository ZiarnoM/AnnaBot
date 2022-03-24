using System;
using System.Data;
using System.Threading;

namespace Anna
{
    public class Scheduler
    {
        public static void checkScheduler()
        {
            
            Thread aoe = new Thread(new ThreadStart(() =>
            {
                while (true)
                {
                    Thread.Sleep(3000);
                    
                    DataRowCollection schedulerTasksToRun =  Db.ExecSqlCollection("select SchedulerId from ReportsScheduler where ActiveP = 1 and NextStart < GETDATE()",new object[] {});
                    foreach (DataRow dataRow in schedulerTasksToRun)
                    {
                        Console.WriteLine(dataRow.ItemArray[0]);
                    }
                    
                }

            }));
            
            aoe.Start();
        }
    }
}