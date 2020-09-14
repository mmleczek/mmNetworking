using CitizenFX.Core;
using System;
using System.Collections.Generic;
using static CitizenFX.Core.Native.API;
using System.Globalization;
using System.Linq;

namespace Client
{
    public class Main : BaseScript
    {
        public Dictionary<int, CallbackDelegate> PendingVehicles = new Dictionary<int, CallbackDelegate>();
        public int CurrentRequestId = 0;

        public Main()
        {
            EventHandlers["mmNetworking:resp"] += new Action<int, int>(async (request, networkid) =>
            {
                try
                {
                    Log.Debug($"got request {request} entity {networkid} {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture)}");
                    if (NetworkDoesNetworkIdExist(networkid))
                    {
                        int entity = NetworkGetEntityFromNetworkId(networkid);
                        NetworkRequestControlOfEntity(entity);
                        while (!NetworkHasControlOfEntity(entity))
                        {
                            await Delay(0);
                        }

                        ClearVehicleOfPeds(entity);
                        PendingVehicles[request].Invoke(entity);
                        PendingVehicles.Remove(request);
                    }
                    else
                    {
                        PendingVehicles[request].Invoke(request);
                        PendingVehicles.Remove(request);
                    }
                }
                catch(Exception ex)
                { Log.Error(ex); }
            });

            EventHandlers["mmNetworking:clearareaofvehicles"] += new Action<float, float, float>((x, y, z) => 
            { 
                try
                {
                    World.GetAllVehicles().ToList().Where(_ => Vector3.DistanceSquared(_.Position,new Vector3(x, y, z)) < 2.0f).ToList().FirstOrDefault()?.Delete();
                }
                catch(Exception ex)
                {
                    Log.Error(ex);
                }
            });

            Exports.Add("CreateVehicle", new Action<int, float, float, float, float, bool, bool, CallbackDelegate, string>((model, x, y, z, h, isNetwork, netMissionEntity, cb, plate) =>
            {
                try
                {
                    int request = GetRequestId();
                    Log.Debug($"send request {request} {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff",CultureInfo.InvariantCulture)}");
                    TriggerServerEvent("mmNetworking:CreateVehicle", request, model, x, y, z, h, isNetwork, netMissionEntity, plate);
                    Log.Debug(cb.GetType().Name);
                    PendingVehicles.Add(request, cb);
                }
                catch (Exception ex)
                { Log.Error(ex); }
            }));
        }

        public void ClearVehicleOfPeds(int veh)
        {
            for (int i = -1; i < GetVehicleMaxNumberOfPassengers(veh); i++)
            {
                int ped = GetPedInVehicleSeat(veh, i);
                if (DoesEntityExist(ped))
                {
                    if (!IsPedAPlayer(ped))
                    {
                        SetEntityAsMissionEntity(ped, true, false);
                        int _ = ped;
                        DeleteEntity(ref _);
                    }
                }
            }
        }
        public int GetRequestId()
        {
            if (CurrentRequestId < 65535)
            {
                CurrentRequestId = CurrentRequestId + 1;
                return CurrentRequestId;
            }
            else
            {
                CurrentRequestId = 0;
                return CurrentRequestId;
            }
        }
    }
}
