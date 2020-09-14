using CitizenFX.Core;
using System;
using System.Collections.Generic;
using static CitizenFX.Core.Native.API;

namespace Client
{
    public class Main : BaseScript
    {
        public Dictionary<int, CallbackDelegate> PendingVehicles = new Dictionary<int, CallbackDelegate>();
        public int CurrentRequestId = 0;

        public object JsonSerializer { get; private set; }

        public Main()
        {
            EventHandlers["mmNetworking:resp"] += new Action<int, int>(async (request, networkid) =>
            {
                try
                {
                    if (PendingVehicles.ContainsKey(request))
                    {
                        if (NetworkDoesNetworkIdExist(networkid))
                        {
                            int entity = NetworkGetEntityFromNetworkId(networkid);
                            NetworkRequestControlOfEntity(entity);
                            while (!NetworkHasControlOfEntity(entity))
                            {
                                await Delay(0);
                            }

                            ClearVehicleOfPeds(entity);
                            if (PendingVehicles[request] != null)
                            {
                                PendingVehicles[request].Invoke(entity);
                            }
                            PendingVehicles.Remove(request);
                        }
                        else
                        {
                            if (PendingVehicles[request] != null)
                            {
                                PendingVehicles[request].Invoke(0);
                            }
                            PendingVehicles.Remove(request);
                        }
                    }
                }
                catch(Exception ex)
                { Log.Error(ex); }
            });

            Exports.Add("CreateVehicle", new Action<int, float, float, float, float, dynamic, CallbackDelegate>((model, x, y, z, h, o, cb) =>
            {
                try
                {
                    int request = GetRequestId();
                    TriggerServerEvent("mmNetworking:CreateVehicle", request, model, x, y, z, h, o);
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