namespace Djambi.Model

module Users =

    type User = 
        {
            id : int
            name : string
        }

    type CreateUserRequest =
        {
            name : string
        }