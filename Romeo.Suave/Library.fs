namespace Romeo

module Suave =

  open System
  open Suave.Types
  open Suave.Http
  open Suave.Http.Successful
  open Suave.Http.RequestErrors
  open Suave.Utils
  open Common.Utils

  /// microseconds
  let nonce_resolution : int64 = 10000000L // 10 seconds

  let verify secret success_part : WebPart =
    request (fun req ->
      let server_nonce = nonce (get_date())
      let headers = req.headers
      match headers %% "api-key", headers %% "api-sign" with
      | Some api_key, Some api_sign ->
        let form = form req
        match form ^^ "nonce" with
        | Some client_nonce ->
          if Math.Abs(server_nonce - Convert.ToInt64 client_nonce) > nonce_resolution then
            BAD_REQUEST <| sprintf "Nonce is not within %d microseconds of server time." nonce_resolution
          else
            let api_secret = secret api_key
            let payload = encoding.GetString req.raw_form
            let str = req.url + Convert.ToChar(0).ToString() + payload
            let signed_data = hmac api_secret str
            let bytes64 = encoding.GetBytes signed_data
            let encoded_data = Convert.ToBase64String bytes64

            if encoded_data = api_sign then
              success_part api_key
            else
              BAD_REQUEST "Invalid signature."
        | None ->
          BAD_REQUEST "Missing 'nonce' parameter"
      | _, _ ->
        BAD_REQUEST "Missing authentication headers")

