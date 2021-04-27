using Npgsql;
using System;
using HashidsCore.NET;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Bolt.Pages
{
    public class GetURLModel : PageModel
    {
        private static string Host = "127.0.0.1";
        private static string Username = "postgres";
        private static string Database = "public";
        private static string Password = "";
        private static string Port = "5432";
        private static string Ssl = "Require";
        private static string Trust = "true";

        private static string ConnectionString =
            String.Format(
                "Server={0};Username={1};Database={2};Port={3};Password={4};SSL Mode={5};Trust Server Certificate={6}",
                Host,
                Username,
                Database,
                Port,
                Password,
                Ssl,
                Trust);

        public void OnGet()
        {
        }

        public static async Task<string> Get(string Short) {
            string URL = "";

            await using var Connection = new NpgsqlConnection(ConnectionString);
            await Connection.OpenAsync();
            await using (var cmd = new NpgsqlCommand("SELECT \"URL\" FROM main WHERE \"Short\"='"+ Short +"';", Connection)) {
                await using (var reader = await cmd.ExecuteReaderAsync()) {
                    if (reader.HasRows) {
                        while (await reader.ReadAsync()) {
                            URL = reader.GetString(0);
                        }
                    } else {
                        return "INVALID#";
					}
                }
            }

            return URL;
		}
    }
}