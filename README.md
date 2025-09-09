# Capstone Task tracking web application

This project is a web application developed for creating tasks, tracking its states and assign them to specific user.
  * The application contain two web components - a web and a web API applications.
  * The web application is a [ASP.NET Core MVC application]
  * The web API application is a [controller-based ASP.NET Core Web API application].
  * The data store is SQL Light relational database.
  * To access application data the application was used [Entity Framework Core].
  * The application is using the [ASP.NET Core Identity API] to manage users, passwords and profile data. Also, it uses JWT token authentification system.


## Backlog

The application functional requirements are described in the [Functional Requirements](functional-requirements.md) document.

The [backlog with the user stories](https://en.wikipedia.org/wiki/Product_backlog) was implemented is given in the table below. The full list of user stories with descriptions is in the [User Stories](user-stories.md) document.

| Epic | User Story | Description                                                                     | Priority | Is completed? |
|------|------------|---------------------------------------------------------------------------------|----------|---------------|
| EP01 | US01       | View the list of my to-do lists.                                                | 1        |       +       |
| EP01 | US02       | Add a new to-do list.                                                           | 1        |       +       |
| EP01 | US03       | Delete a to-do list.                                                            | 1        |       +       |
| EP01 | US04       | Edit a to-do list.                                                              | 1        |       +       |
| EP02 | US05       | View the list of tasks in a to-do list.                                         | 1        |       +       |
| EP02 | US06       | View the task details page.                                                     | 1        |       +       |
| EP02 | US07       | Add a new to-do task.                                                           | 1        |       +       |
| EP02 | US08       | Delete a to-do task.                                                            | 1        |       +       |
| EP02 | US09       | Edit a to-do task.                                                              | 1        |       +       |
| EP02 | US10       | Highlight tasks that are overdue.                                               | 1        |       +       |
| EP03 | US11       | View a list of tasks assigned to me.                                            | 2        |       +       |
| EP03 | US12       | Filter tasks in my assigned task list.                                          | 2        |       +       |
| EP03 | US13       | Sort tasks in my assigned task list.                                            | 2        |       +       |
| EP03 | US14       | Change the status of a task from the list of assigned tasks.                    | 2        |       +       |
| EP04 | US15       | Search for tasks with specified text in the task title.                         | 3        |       +       |
| EP04 | US16       | Highlight tasks that are overdue on the search result page.                     | 3        |       +       |
| EP05 | US17       | View a list of tags on the task details page.                                   | 5        |       +       |
| EP05 | US18       | View a list of all tags.                                                        | 5        |       +       |
| EP05 | US19       | View a list of tasks tagged by a specific tag.                                  | 5        |       +       |
| EP05 | US20       | Add a tag to a task.                                                            | 5        |       +       |
| EP05 | US21       | Remove a tag that is added to a task.                                           | 5        |       +       |
| EP06 | US22       | View the comments on the task details page.                                     | 6        |       +       |
| EP06 | US23       | Add a new comment to the task.                                                  | 6        |       +       |
| EP06 | US24       | Delete a comment that is added to a task.                                       | 6        |       +       |
| EP06 | US25       | Edit a new comment                                                              | 6        |       +       |
| EP07 | US26       | Sign up                                                                         | 7        |       +       |
| EP07 | US27       | Sign in                                                                         | 7        |       +       |
| EP07 | US28       | Sign out                                                                        | 7        |       +       |
| EP07 | US29       | Restore password                                                                | 8        |       -       |
| EP08 | US30       | Application menu                                                                | 4        |       +       |


## Software Architecture

The architecture of the application is described in the [Software Architecture](software-architecture.md) document.


## Solution Requirements

The requirements for the application are described in the [Solution Requirements](solution-requirements.md) document.

## How to run this project on your machine

1. Clone this project to your local machine.
2. Open the solution.
3. Select executable project as TodoListWebApp.WebApi and https option. Press CTRL+F9.
4. Then select executable project as TodoListWebApp.WebApp and http option. Run it.
5. Open WebApp in your browser (by default it is located on http://localhost:5000/TodoLists)

You can specify url and used port in launchSettings.json file.
