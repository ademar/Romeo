namespace Romeo.Common

module Utils =

    open System
    open System.Text
    open System.Web

    let ByteToString (buff : byte[] ) =
      let sbinary = new StringBuilder()
      Array.iter (fun (b : byte) -> sbinary.Append(b.ToString("x2")) |> ignore) buff
      sbinary.ToString()

    let pack_H (hex : string) =
      let hex = 
       if ((hex.Length % 2) = 1) then hex + "0"
        else hex
      let bytes = Array.zeroCreate (hex.Length / 2)
      for i in 0 .. 2 .. (hex.Length - 1) do
        bytes.[i / 2] <- Convert.ToByte(hex.Substring(i, 2), 16)
      bytes

    let encoding = Encoding.UTF8

    let hmac (key :string) (message : string) = 
      let keyByte = encoding.GetBytes(key)
      use hmacsha512 = new BillieJeansSHA512(keyByte)
      hmacsha512.ComputeHash(encoding.GetBytes(message)) |> ignore
      ByteToString(hmacsha512.Hash).ToLowerInvariant()

    let ToQueryString ( pairs : (string*string) array) =
      Array.map (fun (a : string, b : string) -> String.Format("{0}={1}", HttpUtility.UrlEncode(a), HttpUtility.UrlEncode(b))) pairs
      |> (fun x -> String.Join("&", x) )
      
    /// the number of microseconds since the epoch where epoch is a Unix term for midnight on Jan 1, 1970
    let nonce (date : DateTime) = 
      let epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
      Convert.ToInt64((date - epoch).Ticks/int64(10));

    let get_date _ = DateTime.UtcNow

    let debug flag (message:string) =
      if flag then
    Console.WriteLine message