namespace Async_rpc.Versioned_rpc

open Async_rpc
open Core_kernel

module Menu = 

    module Menu_V1 = Bin_prot_generated_types.Lib.Async_rpc_kernel.Src.Versioned_rpc.Menu.V1.T
    let menu_rpc = Rpc.create {| name = "__Versioned_rpc.Menu" ; version = 1 |} Menu_V1.bin_query Menu_V1.bin_response

    type t = (string*int64) list

    (*on connection, returns menu*)
    let with_menu (connect : Connection.t) (callback : t -> unit) : Or_error.t<unit> = 
        (*sends out message to menu rpc*)

        Rpc.dispatch menu_rpc connect () (fun (resp: Result<(string*int64) list,Protocol.Rpc_error.t>) -> 
            (match resp with
            | Ok menu -> 
                // printfn $"Menu: {menu}";
                callback menu
            | Error e -> printfn $"RPC error: {e}")
        )
    
    (*return menu - might not need*) 
    let supported_rpcs (t : t) = t

    (*return set of supported versions for specified rpc name*)
    let supported_versions (t : t) (rpc_name : string) : int64 Set = 
        (*filter out rpcs w a different name*)
        let filtered_list = (List.filter (fun name_version -> 
            let (name,_version) = name_version
            if name = rpc_name then true else false
            ) t)

        (*add versions from the new filtered list to set*)
        let set_of_versions = (List.fold (fun vers_set (name_version : string*int64) -> 
            let (_name,version) = name_version
            (*add version to set*)
            Set.add version vers_set
            ) Set.empty<int64> filtered_list)

        set_of_versions


module Connection_with_menu = 
    type t = {
        connection : Connection.t;
        menu : Menu.t
    }

    let with_ (connection : Connection.t) (callback : t -> unit) : Or_error.t<unit> = 
        Menu.with_menu connection (fun menu -> 
            let t =  {connection=connection ; menu=menu}
            callback t
        )
    
    (*when calling with_ call get_menu or get_connection in callback given conn_w_menu*)
    let get_menu t = t.menu
    let get_connection t = t.connection

    
