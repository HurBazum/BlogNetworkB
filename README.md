# BlogNetworkB
ASP.Net.Core MVC Blog

роли нужно добавить в таблицу вручную, из программы этого сдалать нельзя:
insert into "Roles" (Name) values("User")
insert into "Roles" (Name) values("Moderator")
insert into "Roles" (Name) values("Admin")
/********************************************************************/

если в базе нет авторов - создаётся админ, дальше обычные юзеры, админ может задавать им роли,
через список авторов
также через список он может удалять пользователей, и посещать их страницы, смотреть их статьи, комментарии(это могут все)

модератор может редактировать данные пользователя, через список всех пользователей, редактировать статьи, переходя на страницу чтения статьи

при изменении имейла, предётся перезаходить

пока что только модератор может удалять и редактировать статьи
