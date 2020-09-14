using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Server
{
    public class Main : BaseScript
    {
        List<PendingVehicle> _ = new List<PendingVehicle>();
        List<PendingVehicle> _2 = new List<PendingVehicle>();

        bool _isworking = false;
        public Main()
        {
            EventHandlers["mmNetworking:CreateVehicle"] += new Action<Player, int, int, float, float, float, float, bool, bool, string>(CreateVehicle_);
        }

        public void CreateVehicle_([FromSource]Player source, int reqId, int model, float x, float y, float z, float h, bool isNetwork, bool netMissionEntity, string plate)
        {
            try
            {
                //Log.Debug($"Got request {source.Name} {reqId} {plate}");
                _.Add(new PendingVehicle()
                {
                    SourceId = Convert.ToInt32(source.Handle),
                    RequestId = reqId,
                    Model = model,
                    X = x,
                    Y = y,
                    Z = z,
                    Heading = h,
                    IsNetwork = isNetwork,
                    NetMissionEntity = netMissionEntity,
                    Plate = plate
                });

                if (!_isworking)
                {
                    //Log.Debug("START");
                    _isworking = true;
                    Tick += MainLoop;
                }
            }
            catch (Exception ex)
            { Log.Error(ex); }
        }
        
        long start = GetGameTimer();
        public async Task MainLoop()
        {
            try
            { 
                foreach (var p in _.ToList())
                {
                    if(GetGameTimer() - start > 1000)
                    {
                        start = GetGameTimer();
                        int e = CreateVehicle((uint)p.Model, p.X, p.Y, p.Z, p.Heading, p.IsNetwork, p.NetMissionEntity);
                        long init = GetGameTimer();
                        while (!DoesEntityExist(e))
                        {
                            if (GetGameTimer() - init > 500)
                            {
                                Players[p.SourceId]?.TriggerEvent("mmNetworking:resp", p.RequestId, 0);
                                Players[p.SourceId]?.TriggerEvent("mmNetworking:clearareaofvehicles");
                               _.Remove(p);
                                return;
                            }
                        }
                        if (!string.IsNullOrEmpty(p.Plate))
                        {
                            SetVehicleNumberPlateText(e, p.Plate);
                        }
                        SetVehicleDoorsLocked(e, 1);

                        int net_ = NetworkGetNetworkIdFromEntity(e);
                        Players[p.SourceId]?.TriggerEvent("mmNetworking:resp", p.RequestId, net_);
                        _.Remove(p);
                    }
                }

                if (_.Count == 0)
                {
                    //Log.Debug("STOP");
                    Tick -= MainLoop;
                    _isworking = false;
                }
            }
            catch (Exception ex)
            { Log.Error(ex); }
            await Task.FromResult(0);
        }

    }

    public class PendingVehicle
    {
        public int SourceId { get; set; }
        public int RequestId { get; set; }
        public int Model { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float Heading { get; set; }
        public bool IsNetwork { get; set; }
        public bool NetMissionEntity { get; set; }
        public string Plate { get; set; }
    }
}
