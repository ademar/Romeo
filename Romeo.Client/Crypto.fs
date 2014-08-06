module Crypto

open System.Security.Cryptography

type BillieJeansSHA512(key : byte[]) as this = 
  inherit HMAC()
  do 
    this.HashName <- "System.Security.Cryptography.SHA512CryptoServiceProvider"
    this.HashSizeValue <- 512
    this.BlockSizeValue <- 128
    this.Key <- key.Clone() :?>  byte[]

