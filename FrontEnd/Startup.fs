namespace FrontEnd

open Owin
open Microsoft.Owin
open Microsoft.Owin.Security.Cookies
open Microsoft.AspNet.Identity

open Helpers
open Config.Route

type Startup() =
  member this.Configuration (app : IAppBuilder) =
    this.ConfigureAuth(app)
  
  member this.ConfigureAuth(app : IAppBuilder) =
    // Enable the application to use a cookie to store information for the signed in user
    
    let cookieAuthOptions = CookieAuthenticationOptions()
    cookieAuthOptions.AuthenticationType <- DefaultAuthenticationTypes.ApplicationCookie
    cookieAuthOptions.LoginPath <- PathString(Link Login)

    app.UseCookieAuthentication(cookieAuthOptions) |> ignore
    ()
