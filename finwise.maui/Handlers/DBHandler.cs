using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using finwise.maui.Models;
using System.Diagnostics;

namespace finwise.maui.Handlers
{
    public  class DBHandler
    {
        SQLiteAsyncConnection dbConnection;

        private async Task Init()
        {
            if (dbConnection is not null)
                return;

            dbConnection = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
            await dbConnection.CreateTableAsync<Expense>();
            await dbConnection.CreateTableAsync<Person>();
            await dbConnection.CreateTableAsync<Tag>();
            await dbConnection.CreateTableAsync<ExpenseMember>();
            await dbConnection.CreateTableAsync<ExpenseTag>();
        }

        public async Task<List<Expense>> ActivityCRUD(string mode, List<Expense> activityList)
        {

            await Init();

            try
            {
                if(activityList != null)
                {
                    switch (mode)
                    {
                        case "insert":
                            await dbConnection.InsertAllAsync(activityList);
                            break;
                        case "update":
                            await dbConnection.UpdateAllAsync(activityList);
                            break;
                        case "deleteOne":
                            await dbConnection.DeleteAsync(activityList[0]);
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    switch (mode)
                    {
                        case "read":
                            await dbConnection.Table<Expense>().ToListAsync();
                            break;
                        default:
                            break;
                    }
                }
                
            }catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return null;
        }
    }
}
