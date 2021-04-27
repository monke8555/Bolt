using Npgsql;
using System;
using System.Net;
using HashidsCore.NET;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Bolt.Pages
{
    public class ShortenModel : PageModel
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

        public static async Task<string> Shorten(string URL) {
            if (!URL.StartsWith("http://", StringComparison.OrdinalIgnoreCase) && !URL.StartsWith("https://", StringComparison.OrdinalIgnoreCase)) {
                return "NOTAURL#";
			}
            using (var client = new WebClient()) {
                try {
                    string test = client.DownloadString(URL);
                } catch (Exception e) {
                    if (e.GetType() == typeof(WebException)) {
                        return "404#";
					}
				}
            }

            string Check = await Get(URL);
            if (Check != "") {
                return Check;
			}

            var Hashids = new Hashids("R6V78pmB8y");
            string Short = Hashids.Encode(await Last());
            await using var Connection = new NpgsqlConnection(ConnectionString);
			await Connection.OpenAsync();

			await using (var Query = new NpgsqlCommand($"INSERT INTO main (\"URL\", \"Short\") VALUES ('{System.Web.HttpUtility.UrlEncode(URL.Trim())}', '{Short}')", Connection)) {
				await Query.ExecuteNonQueryAsync();
			}

			return Short;
		}

        private static async Task<int> Last() {
            int Last = 0;

            await using var Connection = new NpgsqlConnection(ConnectionString);
			await Connection.OpenAsync();
            await using (var cmd = new NpgsqlCommand("SELECT \"ID\" FROM main", Connection)) {
                await using (var reader = await cmd.ExecuteReaderAsync()) {
                    while (await reader.ReadAsync()) {
                        Last = reader.GetInt32(0);
                    }
                }
            }

			return Last + 1;
        }

        public static async Task<string> Get(string URL) {
            string Short = "";

            await using var Connection = new NpgsqlConnection(ConnectionString);
            await Connection.OpenAsync();
            await using (var cmd = new NpgsqlCommand($"SELECT \"Short\" FROM main WHERE \"URL\"='{System.Web.HttpUtility.UrlEncode(URL.Trim())}';", Connection)) {
                await using (var reader = await cmd.ExecuteReaderAsync()) {
                    while (await reader.ReadAsync()) {
                        if (reader.HasRows) {
                            Short = reader.GetString(0);
                        } else {
                            return "";
						}
                    }
                }
            }

            return Short;
        }
    }
}