using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using finwise.maui.Models;
using System.Diagnostics;
using finwise.maui.Helpers;

namespace finwise.maui.Handlers
{
    public class DBHandler
    {
        private SQLiteAsyncConnection dbConnection;

        private async Task Init()
        {
            if (dbConnection is not null)
                return;
            try
            {
                dbConnection = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);

                dbConnection.Tracer = new Action<string>(q => Debug.WriteLine(q));
                dbConnection.Trace = true;
                await CreateTables();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

        }

        private async Task CreateTables()
        {
            await dbConnection.CreateTableAsync<Expense>();
            //await dbConnection.CreateTableAsync<Person>();
            //await dbConnection.CreateTableAsync<Tag>();
            //await dbConnection.CreateTableAsync<ExpenseMember>();
            //await dbConnection.CreateTableAsync<ExpenseTag>();
        }

        public async Task<bool> ExecuteQuery(string query)
        {
            await Init();

            var op = await dbConnection.ExecuteAsync(query);
            return op > 0;
        }

        public async Task<IEnumerable<T>> GetItems<T>() where T : BaseTable, new()
        {
            await Init();

            return await dbConnection.Table<T>().ToListAsync();
        }

        public async Task<IEnumerable<T>> GetItemsWithQuery<T>(string query) where T : BaseTable, new()
        {
            await Init();

            return await dbConnection.QueryAsync<T>(query);
        }

        public async Task<int> CreateItem<T>(T item) where T : BaseTable
        {
            try
            {
                await Init();
                return await dbConnection.InsertAsync(item);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating item: {ex.Message}");
                return -1; 
            }
        }

    }
}