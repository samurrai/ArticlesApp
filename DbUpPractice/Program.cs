using Dapper;
using DbUp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace DbUpPractice
{
    class Program
    {
        private static string _connectionString = ConfigurationManager.ConnectionStrings["appConnection"].ConnectionString;

        static void Main(string[] args)
        {
            CheckMigrations();
            using (var connection = new SqlConnection(_connectionString))
            {
                Console.WriteLine("Что вы хотите сделать?");
                Console.WriteLine("1)Просмотреть новости");
                Console.WriteLine("2)Добавить новость");
                Console.WriteLine("3)Добавить комментарий к новости");
                Console.WriteLine("4)Выйти");
                while (true)
                {
                    if (int.TryParse(Console.ReadLine(), out int choice))
                    {
                        if (choice == 1)
                        {
                            foreach (var article in ShowArticles(connection))
                            {
                                Console.WriteLine(article.Title);
                                Console.WriteLine(article.Content);
                                Console.WriteLine(article.PostingDate);
                            }
                        }
                        else if (choice == 2)
                        {
                            AddArticle(connection);
                        }
                        else if (choice == 3)
                        {
                            AddComment(connection);
                        }
                        else if (choice == 4)
                        {
                            Environment.Exit(0);
                        }
                        else
                        {
                            Console.WriteLine("Некорректный ввод");
                        }
                    }
                }
            }
        }

        private static List<Article> ShowArticles(SqlConnection connection)
        {
            return connection.Query<Article>("select * from articles").ToList();
        }

        private static void AddComment(SqlConnection connection)
        {
            Comment comment = new Comment();
            while(true)
            {
                Console.WriteLine("Введите имя автора новости");
                comment.AuthorName = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(comment.AuthorName))
                {
                    break;
                }
            }
            while (true)
            {
                Console.WriteLine("Введите текст комментария");
                comment.Text = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(comment.Text))
                {
                    break;
                }
            }
            while (true)
            {
                Console.WriteLine("Введите дату комментария в формате дд/мм/гггг");
                string dateString = Console.ReadLine();
                if (dateString[2] == '/' && dateString[5] == '/')
                {
                    string[] date = dateString.Split();
                    if (int.TryParse(date[0], out int day) && int.TryParse(date[0], out int month) && int.TryParse(date[0], out int year))
                    {
                        if ((day > 0 && day < 32) && (month > 0 && month < 13) && (year > 1970 && year < 2300))
                        {
                            comment.CommentDate = new DateTime(year, month, day);
                            break;
                        }
                    }
                }
            }
            while (true)
            {
                Console.WriteLine("Введите название новости");
                string title = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(title))
                {
                    bool isFound = false;
                    foreach (var article in ShowArticles(connection)) {
                        if (article.Title == title)
                        {
                            comment.Article = article;
                            comment.ArticleId = article.Id;
                        }
                    }
                    if (isFound)
                    {
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Новость не найдена");
                    }
                }
            }
            connection.Execute("insert into comments values(@Id, @AuthorName, @Text, @CommentDate, @Articleid)", comment);
        }

        private static void AddArticle(SqlConnection connection)
        {
            Article article = new Article();
            while (true)
            {
                Console.WriteLine("Введите название новости");
                article.Title = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(article.Title))
                {
                    break;
                }
            }
            while (true)
            {
                Console.WriteLine("Введите содержание новости");
                article.Content = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(article.Content))
                {
                    break;
                }
            }
            while (true)
            {
                Console.WriteLine("Введите дату новости в формате дд/мм/гггг");
                string dateString = Console.ReadLine();
                if (dateString[2] == '/' && dateString[5] == '/')
                {
                    string[] date = dateString.Split();
                    if (int.TryParse(date[0], out int day) && int.TryParse(date[0], out int month) && int.TryParse(date[0], out int year))
                    {
                        if ((day > 0 && day < 32) && (month > 0 && month < 13) && (year > 1970 && year < 2300))
                        {
                            article.PostingDate = new DateTime(year, month, day);
                            break;
                        }
                    }
                }
            }
            connection.Execute("insert into articles values(@Id, @Title, @Content, @PostingDate)", article);
        }

        private static void CheckMigrations()
        {
            EnsureDatabase.For.SqlDatabase(_connectionString);

            var upgrader = DeployChanges.To
            .SqlDatabase(_connectionString)
            .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
            .LogToConsole()
            .Build();

            var result = upgrader.PerformUpgrade();

            if (!result.Successful)
            {
                throw new Exception("Ошибка соединения");
            }
        }
    }
}
