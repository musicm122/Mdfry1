namespace Mdfry1.Scripts.Extensions;



/*
 *
 *
 func get_simple_path_global(start_global: Vector2, end_global: Vector2, optimize: bool = true) -> PoolVector2Array:
var path: PoolVector2Array = get_simple_path(to_local(start_global), to_local(end_global), optimize)
for i in range(path.size()):
    path[i] = to_global(path[i])
return path
 */
// public static class Navigation2dExtensions
// {
//     public static Stack<Vector2> GetSimplePathGlobal(this Navigation2D nav, Vector2 start, Vector2 end, bool optimize = false)
//     {
//         var path = nav.GetSimplePath(start.ToLocal(), end.ToLocal(), optimize);
//     }
//
// }