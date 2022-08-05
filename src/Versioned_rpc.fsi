namespace Async_rpc.Versioned_rpc

open Async_rpc
open Core_kernel

module Menu = 
    type t

    val with_menu : Connection.t -> ( t -> unit ) -> unit Or_error.t
    val supported_rpcs : -> t -> t
    val supported_versions : t -> string -> int64 Set

    module For_testing = 
        val create : (string * int64) list -> t

module Connection_with_menu = 
    type t

    val with_ : Connection.t -> ( t -> unit ) -> unit Or_error.t
    val get_menu : t -> Menu.t
    val get_connection : t -> Connection.t