﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using SocialNetworkApi.Models;
using SocialNetworkApi.Models.NToNs;

namespace SocialNetworkApi.Middlewares
{
	public class DbInitializeMiddleware
	{
		private readonly RequestDelegate _next;
		private ApplicationContext _db;
		private UserManager<User> _userManager;

		public DbInitializeMiddleware(RequestDelegate next)
		{
			this._next = next;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			_db = context.RequestServices.GetService(typeof(ApplicationContext)) as ApplicationContext;
			_userManager = context.RequestServices.GetService(typeof(UserManager<User>)) as UserManager<User>;

			if (_db.Users.Count() != 0)
				return;

			var user1 = new User { UserName = "Roman", NormalizedUserName = "ROMAN" };
			var user2 = new User { UserName = "Admin", NormalizedUserName = "ADMIN" };
			var user3 = new User { UserName = "Support", NormalizedUserName = "SUPPORT" };
			await _userManager.CreateAsync(user1, "123456");
			await _userManager.CreateAsync(user2, "123456");
			await _userManager.CreateAsync(user3, "123456");


			var dialog1 = new Dialog { Title = "Первый диалог", CreatingTime = DateTime.Now, LastMessageTime = DateTime.Now };
			var dialog2 = new Dialog { Title = "Второй диалог", CreatingTime = DateTime.Now, LastMessageTime = DateTime.Now };
			var dialog3 = new Dialog { Title = "Третий диалог", CreatingTime = DateTime.Now, LastMessageTime = DateTime.Now };
			await _db.Dialogs.AddRangeAsync(dialog1, dialog2, dialog3);
			await _db.SaveChangesAsync();


			dialog1.Users = new List<UserToDialog>
			{
				new UserToDialog {Dialog = dialog1, User = user1},
				new UserToDialog {Dialog = dialog1, User = user2},
				new UserToDialog {Dialog = dialog1, User = user3},
			};
			dialog2.Users = new List<UserToDialog>
			{
				new UserToDialog {Dialog = dialog2, User = user1},
				new UserToDialog {Dialog = dialog2, User = user3},
			};
			dialog3.Users = new List<UserToDialog>
			{
				new UserToDialog {Dialog = dialog3, User = user2},
			};
			_db.Dialogs.UpdateRange(dialog1, dialog2, dialog3);
			await _db.SaveChangesAsync();


			var message1 = new Message { Text = "Привет", Author = user1, Dialog = dialog1, SendingTime = DateTime.Now };
			var message2 = new Message { Text = "Это я!", Author = user1, Dialog = dialog2, SendingTime = DateTime.Now };
			var message3 = new Message { Text = "Здравствуй!", Author = user2, Dialog = dialog3, SendingTime = DateTime.Now };
			var message4 = new Message { Text = "Как дела?", Author = user1, Dialog = dialog3, SendingTime = DateTime.Now };
			var message5 = new Message { Text = "Дурка уже в пути", Author = user2, Dialog = dialog2, SendingTime = DateTime.Now };
			await _db.Messages.AddRangeAsync(message1, message2, message3, message4, message5);
			await _db.SaveChangesAsync();


			//user1.Friends = new List<UserToFriend>
			//{
			//	new UserToFriend { User = user1, Friend = user2 },
			//	new UserToFriend { User = user1, Friend = user3 }
			//};
			//user2.Friends = new List<UserToFriend>
			//{
			//	new UserToFriend { User = user2, Friend = user1 },
			//	new UserToFriend { User = user2, Friend = user3 }
			//};
			//user3.Friends = new List<UserToFriend>
			//{
			//	new UserToFriend { User = user3, Friend = user1 },
			//	new UserToFriend { User = user3, Friend = user2 }
			//};
			//_db.Users.UpdateRange(user1, user2, user3);
			//await _db.SaveChangesAsync();


			//user1.Requests.Add(new UserToRequest { RequesterId = user1.Id, ReceiverId = user2.Id });
			//user2.Requests.Add(new UserToRequest { RequesterId = user2.Id, ReceiverId = user3.Id });
			//user3.Requests.Add(new UserToRequest { RequesterId = user3.Id, ReceiverId = user1.Id });
			//_db.Users.UpdateRange(user1, user2, user3);
			//await _db.SaveChangesAsync();




			await _next.Invoke(context);
		}
	}
}