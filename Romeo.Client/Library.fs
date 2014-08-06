namespace Romeo

module Client =

    open System
    open System.Text
    open HttpClient
    open Utils

    type Config = 
      { server_url : string
      ; api_key    : string
      ; app_secret : string
      ; debug      : bool }
    

    let call (config : Config) end_point data =

      let url = config.server_url + end_point
      let nonce = (nonce (get_date())).ToString()

      debug config.debug (sprintf "url:%s\n" url)
      debug config.debug ( "nonce:" +  nonce)

      let data = ToQueryString ( Array.concat [ data; [| ("nonce", nonce) |] ])

      debug config.debug ("data:" + data)
      

      let str = end_point + Convert.ToChar(0).ToString() + data
      let signed_data = hmac config.app_secret str

      debug config.debug ("signed_data:" + signed_data)

      let bytes64 = encoding.GetBytes signed_data
      let encoded_data = Convert.ToBase64String bytes64

      debug config.debug ("encoded_data:" + encoded_data)

      let request = 
        createRequest Post url 
        |> withHeader (Custom {name="Api-Key"; value=config.api_key })
        |> withHeader (Custom {name="Api-Sign"; value= encoded_data })
        |> withBody data
      
      let r = getResponseBody request

      debug config.debug (sprintf "response: %s" r)

      Encoding.UTF8.GetBytes  r

