using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace EFDemoApp.Blogs
{
    public class BloggingContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlite("Data Source=blogging.db");
        }
    }

    public class Blog
    {
        public int BlogId { get; set; }
        public string Url { get; set; }

        public List<Post> Posts { get; } = new List<Post>();

        public void AddPost(Post post)
        {
            this.Posts.Add(post);
            post.Blog = this;
            //post.BlogId = this.BlogId;
        }
    }

    public class Post
    {
        public int PostId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        public int BlogId { get; set; }
        public Blog Blog { get; set; }
    }

    public static class BlogDemo
    {
        public static void Run()
        {
            using (var db = new BloggingContext())
            {
                // Create the database if it doesn't exist
                db.Database.EnsureCreated();
                
                // Create
                Console.WriteLine("create a new blog");
                var nowTicks = DateTime.Now.Ticks;
                var newBlog = new Blog { Url = "http://blogs.msdn.com/adonet/" + nowTicks };
                var mockPostCount = DateTime.Now.Second % 5;
                for (int i = 0; i < mockPostCount; i++)
                {
                    newBlog.AddPost(new Post() { Title = "Title" + i, Content = "Content" + i });
                }
                db.Add(newBlog);
                db.Entry(newBlog).State = EntityState.Added;
                db.SaveChanges();

                // Read
                Console.WriteLine("Querying for a blog");
                var blog = db.Blogs
                    .OrderBy(b => b.BlogId)
                    .First();

                // Update
                Console.WriteLine("Updating the blog and adding a post");
                blog.Url = "https://devblogs.microsoft.com/dotnet";
                blog.Posts.Add(
                    new Post
                    {
                        Title = "Hello World",
                        Content = "I wrote an app using EF Core!"
                    });
                db.SaveChanges();

                // Delete
                Console.WriteLine("Delete the blog");
                db.Remove(blog);
                db.SaveChanges();

                // query
                ShowAllBlogs(db);
            }

        }

        //todo repository
        //public static IIncludableQueryable<TEntity, TProperty> Include<TEntity, TProperty>([NotNullAttribute] this IQueryable<TEntity> source, [NotNullAttribute] Expression<Func<TEntity, TProperty>> navigationPropertyPath) where TEntity : class;
        private static void ShowAllBlogs(BloggingContext db)
        {
            Console.WriteLine("Querying for all blogs");
            var theBlogs = db.Blogs.Include(x => x.Posts)
                .OrderBy(b => b.BlogId).ToList();
            foreach (var theBlog in theBlogs)
            {
                Console.WriteLine("{0}, {1}, POST COUNT: {2}", theBlog.BlogId, theBlog.Url, theBlog.Posts.Count);
                foreach (var post in theBlog.Posts)
                {
                    Console.WriteLine("\t{0}, {1}, {2}", post.BlogId, post.Title, post.Content);
                }
            }
            Console.WriteLine("-------------------");
        }
    }
}
