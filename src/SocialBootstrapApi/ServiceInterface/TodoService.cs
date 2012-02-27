using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using ServiceStack.ServiceInterface;

namespace SocialBootstrapApi.ServiceInterface
{
	//REST Resource DTO
	[DataContract]
	public class Todo
	{
		[DataMember(Order = 1)]
		public long Id { get; set; }
		[DataMember(Order = 2)]
		public string Content { get; set; }
		[DataMember(Order = 3)]
		public int Order { get; set; }
		[DataMember(Order = 4)]
		public bool Done { get; set; }
	}

	//REST Service implementation
	public class TodoService : RestServiceBase<Todo>
	{
		public TodoRepository Repository { get; set; }  //Injected by IOC

		public override object OnGet(Todo request)
		{
			if (request.Id == default(long))
				return Repository.GetAll();

			return Repository.GetById(request.Id);
		}

		//Called for new and update
		public override object OnPost(Todo todo)
		{
			return Repository.Store(todo);
		}

		public override object OnDelete(Todo request)
		{
			Repository.DeleteById(request.Id);
			return null;
		}
	}

	/// <summary>
	/// In-memory repository, so we can run the TODO app without any external dependencies
	/// Registered in Funq as a singleton, auto injected on every request
	/// </summary>
	public class TodoRepository
	{
		private readonly List<Todo> todos = new List<Todo>();

		public List<Todo> GetAll()
		{
			return todos;
		}

		public Todo GetById(long id)
		{
			return todos.FirstOrDefault(x => x.Id == id);
		}

		public Todo Store(Todo todo)
		{
			if (todo.Id == default(long))
			{
				todo.Id = todos.Count == 0 ? 1 : todos.Max(x => x.Id) + 1;
			}
			else
			{
				for (var i = 0; i < todos.Count; i++)
				{
					if (todos[i].Id != todo.Id) continue;

					todos[i] = todo;
					return todo;
				}
			}

			todos.Add(todo);
			return todo;
		}

		public void DeleteById(long id)
		{
			todos.RemoveAll(x => x.Id == id);
		}
	}
}