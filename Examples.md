# Examples
## ESX - lua
### File: client/functions.lua
```lua
ESX.Game.SpawnVehicle = function(modelName, coords, heading, cb)
	exports.mmNetworking:CreateVehicle(modelName, coords.x, coords.y, coords.z, heading, 
	{
		IsNetwork = true,
		NetMissionEntity = true
	},
	function(vehicle)
		if cb then
			Citizen.CreateThread(function()
				cb(vehicle)
			end)
		end
	end)
end
```
## Export with all possible vehicle parameters
#### Lua
```lua
local coords = GetEntityCoords(PlayerPedId())
exports.mmNetworking:CreateVehicle(GetHashKey("adder"), coords.x, coords.y, coords.z, 0.0, 
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
#### C#
```csharp
Vector3 coords = Game.PlayerPed.Position;
Exports["mmNetworking"].CreateVehicle("adder", coords.X, coords.Y, coords.Z, 0.0f, new
{
  IsNetwork = true,
  NetMissionEntity = true,
  SetVehicleAlarm = true,
  SetVehicleCustomPrimaryColour = new[] { 255, 0, 0 },
  SetVehicleCustomSecondaryColour = new[] { 255, 255, 0 },
  SetVehicleNumberPlateText = "test1337",
  SetVehicleDoorsLocked = 1
}, new Action<int>((vehicle) =>
{
  //your code here
  //Game.PlayerPed.Task.WarpIntoVehicle((Vehicle)Vehicle.FromHandle(vehicle), VehicleSeat.Driver);
}));
```
## Export without any vehicle parameters
#### Lua
```lua
local coords = GetEntityCoords(PlayerPedId())
exports.mmNetworking:CreateVehicle(GetHashKey("adder"), coords.x, coords.y, coords.z, 0.0, {}, function(vehicle)
-- your code here
end)
```
#### C#
```csharp
Vector3 coords = Game.PlayerPed.Position;
Exports["mmNetworking"].CreateVehicle("adder", coords.X, coords.Y, coords.Z, 0.0f, new {}, new Action<int>((vehicle) =>
{
  //your code here
  //Game.PlayerPed.Task.WarpIntoVehicle((Vehicle)Vehicle.FromHandle(vehicle), VehicleSeat.Driver);
}));
```
