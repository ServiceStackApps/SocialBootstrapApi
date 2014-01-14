using System;
using System.Collections.Generic;
using ServiceStack;
using ServiceStack.DataAnnotations;
using ServiceStack.OrmLite;

namespace SocialBootstrapApi.ServiceInterface
{
    //Request DTO
    [Route("/authors")]
    public class Author
    {
        [AutoIncrement]
        [Alias("AuthorID")]
        public Int32 Id { get; set; }
        [Index(Unique = true)]
        [StringLength(40)]
        public string Name { get; set; }
        public DateTime Birthday { get; set; }
        public DateTime? LastActivity { get; set; }
        public Decimal? Earnings { get; set; }
        public bool Active { get; set; }
        [Alias("JobCity")]
        public string City { get; set; }
        [Alias("Comment")]
        public string Comments { get; set; }
        public Int16 Rate { get; set; }
    }

    //Response DTO
    public class AuthorResponse : IHasResponseStatus
    {
        public List<Author> Results1 { get; set; }
        public List<Author> Results2 { get; set; }
        public List<Author> Results3 { get; set; }
        public List<Author> Results4 { get; set; }
        public List<Author> Results5 { get; set; }
        public List<Author> Results6 { get; set; }
        public List<Author> Results7 { get; set; }
        public List<Author> Results8 { get; set; }
        public ResponseStatus ResponseStatus { get; set; } //Where Exceptions get auto-serialized
    }

    //Can be called via any endpoint or format (json,xml,html,jsv,csv,soap), see: http://servicestack.net/ServiceStack.Hello/
    public class AuthorService : Service
    {
        //Get's called by all HTTP Verbs (GET,POST,PUT,DELETE,etc) and endpoints JSON,XMl,JSV,etc
        public object Any(Author request)
        {
            Db.DropAndCreateTable<Author>();

            var authors = new List<Author> {
				new Author { Name = "Demis Bellot", Birthday = DateTime.Today.AddYears(-20), Active = true, Earnings = 99.9m, Comments = "CSharp books", Rate = 10, City = "London" },
				new Author { Name = "Angel Colmenares", Birthday = DateTime.Today.AddYears(-25), Active = true, Earnings = 50.0m, Comments = "CSharp books", Rate = 5, City = "Bogota" },
				new Author { Name = "Adam Witco", Birthday = DateTime.Today.AddYears(-20), Active = true, Earnings = 80.0m, Comments = "Math Books", Rate = 9, City = "London" },
				new Author { Name = "Claudia Espinel", Birthday = DateTime.Today.AddYears(-23), Active = true, Earnings = 60.0m, Comments = "Cooking books", Rate = 10, City = "Bogota" },
				new Author { Name = "Libardo Pajaro", Birthday = DateTime.Today.AddYears(-25), Active = true, Earnings = 80.0m, Comments = "CSharp books", Rate = 9, City = "Bogota" },
				new Author { Name = "Jorge Garzon", Birthday = DateTime.Today.AddYears(-28), Active = true, Earnings = 70.0m, Comments = "CSharp books", Rate = 9, City = "Bogota" },
				new Author { Name = "Alejandro Isaza", Birthday = DateTime.Today.AddYears(-20), Active = true, Earnings = 70.0m, Comments = "Java books", Rate = 0, City = "Bogota" },
				new Author { Name = "Wilmer Agamez", Birthday = DateTime.Today.AddYears(-20), Active = true, Earnings = 30.0m, Comments = "Java books", Rate = 0, City = "Cartagena" },
				new Author { Name = "Rodger Contreras", Birthday = DateTime.Today.AddYears(-25), Active = true, Earnings = 90.0m, Comments = "CSharp books", Rate = 8, City = "Cartagena" },
				new Author { Name = "Chuck Benedict", Birthday = DateTime.Today.AddYears(-22), Active = true, Earnings = 85.5m, Comments = "CSharp books", Rate = 8, City = "London" },
				new Author { Name = "James Benedict II", Birthday = DateTime.Today.AddYears(-22), Active = true, Earnings = 85.5m, Comments = "Java books", Rate = 5, City = "Berlin" },
				new Author { Name = "Ethan Brown", Birthday = DateTime.Today.AddYears(-20), Active = true, Earnings = 45.0m, Comments = "CSharp books", Rate = 5, City = "Madrid" },
				new Author { Name = "Xavi Garzon", Birthday = DateTime.Today.AddYears(-22), Active = true, Earnings = 75.0m, Comments = "CSharp books", Rate = 9, City = "Madrid" },
				new Author { Name = "Luis garzon", Birthday = DateTime.Today.AddYears(-22), Active = true, Earnings = 85.0m, Comments = "CSharp books", Rate = 10, City = "Mexico" },
			};
            Db.InsertAll(authors);

            var agesAgo = DateTime.Today.AddYears(-20).Year;

            return new AuthorResponse {
                Results1 = Db.Select<Author>(rn => rn.Birthday >= new DateTime(agesAgo, 1, 1) && rn.Birthday <= new DateTime(agesAgo, 12, 31)),
                Results2 = Db.Select<Author>(rn => Sql.In(rn.City, "London", "Madrid", "Berlin")),
                Results3 = Db.Select<Author>(rn => rn.Name.StartsWith("A")),
                Results4 = Db.Select<Author>(rn => rn.Name.EndsWith("garzon")),
                Results5 = Db.Select<Author>(rn => rn.Name.ToUpper().EndsWith("GARZON")),
                Results6 = Db.Select<Author>(rn => rn.Name.Contains("Benedict")),
                Results7 = Db.Select<Author>(rn => rn.Earnings <= 50),
                Results8 = Db.Select<Author>(rn => rn.Rate == 10 && rn.City == "Mexico"),
            };
        }
    }
}