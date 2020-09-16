# Usage
## Lua
### Export with all possible vehicle parameters
```lua
local v = GetEntityCoords(PlayerPedId())
exports.mmNetworking:CreateVehicle(GetHashKey("adder"), v.x, v.y, v.z, 0.0, 
{
  IsNetwork = true,
  NetMissionEntity = true,
  SetVehicleAlarm = true,
  SetVehicleCustomPrimaryColour = { 255,0,0 },
  SetVehicleCustomSecondaryColour = { 255,255,0 },
  SetVehicleNumberPlateText = "test1337",
  SetVehicleDoorsLocked = 1
},
function(vehicle)
-- your code here

end)
```
### Export without any vehicle parameters
```lua
local v = GetEntityCoords(PlayerPedId())
exports.mmNetworking:CreateVehicle(GetHashKey("adder"), v.x, v.y, v.z, 0.0, {}, function(vehicle)
-- your code here

end)
```
