namespace FrontEnd

open WebSharper
open WebSharper.Sitelets

module Auth =

    [<Rpc>]
    let IdentitySignout() =
        let ctx = Web.Remoting.GetContext()
        async {
            do! ctx.UserSession.Logout()
            return ()
        }

    let private IdentitySignin (ctx:Web.Context) (username:string) (isPersistent:bool) =
        async {
            do! ctx.UserSession.LoginUser(username,isPersistent)
            return ()
        }

    [<Rpc>]
    let CheckCredentials (login:string) (password:string) (keepLogged:bool) = 
        // retrieve the context outside the async block
        let ctx = Web.Remoting.GetContext()

        async {
            if login = "admin" && password = "admin" then
                let username = "admin user"
                do! IdentitySignin ctx username keepLogged

                return Result.Ok ""
            else
                return Result.Error [ "Wrong credentials! try 'admin' for both login and password." ]
        }

    let IsLogged (ctx:Context<FrontEnd.Config.Route.EndPoint>) =
        ctx.UserSession.GetLoggedInUser()
