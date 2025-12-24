# TaskTracker

## О проекте
TaskTracker - это сервис для управления проектами и задачами, разработанный с использованием **ASP.NET CORE MVC**

## Функционал
- Регистрация и аунтефикация пользователей (JWT)
- Роли **(Администратор, менеджер, сотрудник)**
- Управление проектами
- Создание, просмотр, обновление и удаление задач
- Назначение задач сотрудникам

## Технологии
- C#
- ASP.NET CORE MVC
- Entity Framework Core
- Microsoft SQL Server
- Identity + JWT

## Как запустить проект
```bash
   git clone https://github.com/akanelovw/TaskTracker.git
```
- Настроить подключение к базе данных
- В appsettings.json установить корректную строку подключения к SQL Server.
- Применить миграции
```bash
   dotnet ef database update
```
- Запустить проект
