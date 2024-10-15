This project showcases a well-structured API for a Social Network application, built using a clean architecture approach. The **technology stack** includes:
- ASP.NET Core
- Entity Framework
- PostgreSQL
- Fluent Validation
- Dependency Injection (DI)

**Key features** implemented in this project are:
- User Authorization
- Friendships Management
- Posts, Likes, and Comments
- Group Creation and Roles
- Group and User Blacklisting

**Description:**

Users can create personal posts or form groups where posts are shared. 
Each group has its own role-based system, allowing members to have specific permissions within the group. 
Posts can receive likes and comments, and both comments and posts can be edited or deleted. 
Additionally, users can engage in private conversations, where they can send, edit, delete messages, mark them as read, or even delete entire conversations. 
Users have the ability to block one another, while groups can also block specific users.

Additionally, the application distinguishes between user and admin roles, each with their own dedicated API endpoints and Swagger schemas to reflect their distinct functionality.
