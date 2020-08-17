# api.common

Contains utilities that can be used at all layers of the application.

# api.db

Contains modules for communicating with SQL.

- **Model** module contains data types that correspond to result set rows from different SQL queries.
- **Mapping** module contains logic for mapping from `Djambi.Api.Db.Model` namespace and the `Djambi.Api.Model` namespace.
- **Repository** modules contain the logic for executing SQL commands.

# api.host

Application entry point. Most configuration logic.

# api.logic

Core business logic of application.

- **Service** modules contain complicated business logic that may used by 1 or more endpoints. **Services** can use **Repositories** or other **Services**.
- **Manager** modules are the top of the business logic layer. Public methods on a manager should each execute the operation for a specific API endpoint by composing together the functionality of **Services** and **Repositories**, without any HTTP-related details.

# api.model

Business logic data model.

# api.web

Contains logic for working with the HTTP protocol.

- **Controller** modules are the top layer of all endpoints. Public methods on a manager should each gather context from the HTTP request, call business logic in a **Manager** and then create an HTTP response.
