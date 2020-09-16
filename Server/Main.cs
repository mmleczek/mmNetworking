using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Dynamic;

namespace Server
{
    public class Main : BaseScript
    {
        List<PendingVehicle> _ = new List<PendingVehicle>();

        bool _isworking = false;
        public Main()
        {
            EventHandlers["mmNetworking:CreateVehicle"] += new Action<Player, int, int, float, float, float, float, dynamic>(CreateVehicle_);
        }

        public void CreateVehicle_([FromSource]Player source, int reqId, int model, float x, float y, float z, float h, dynamic param)
        {
            try
            {
                PendingVehicle p = new PendingVehicle()
                {
                    SourceId = Convert.ToInt32(source.Handle),
                    RequestId = reqId,
                    Model = model,
                    X = x,
                    Y = y,
                    Z = z,
                    Heading = h
                };
               
                if(param != null && param is ExpandoObject)
                {
                    p.Param = param;
                }
                else
                {
                    p.Param = new Dictionary<string, dynamic>();
                }

                _.Add(p);

                if (!_isworking)
                {
                    _isworking = true;
                    Tick += MainLoop;
                }
            }
            catch (Exception ex)
            { Log.Error(ex); }
        }
        
        public async Task MainLoop()
        {
            try
            {
                foreach (var p in _.ToList())
                {
                    bool _isnetwork = false;
                    bool _netmissionentity = false;

                    if(p.Param.ContainsKey("IsNetwork"))
                    {
                        _isnetwork = p.Param["IsNetwork"];
                    }

                    if (p.Param.ContainsKey("NetMissionEntity"))
                    {
                        _netmissionentity = p.Param["NetMissionEntity"];
                    }

                    int e = CreateVehicle((uint)p.Model, p.X, p.Y, p.Z, p.Heading, _isnetwork, _netmissionentity);
                    long init = GetGameTimer();
                    while (!DoesEntityExist(e))
                    {
                        if (GetGameTimer() - init > 500)
                        {
                            Players[p.SourceId]?.TriggerEvent("mmNetworking:resp", p.RequestId, 0);
                            _.Remove(p);
                            return;
                        }
                        await Delay(0);
                    }

                    #region Vehicle Properties
                    if (p.Param.ContainsKey("SetVehicleAlarm"))
                    {
                        SetVehicleAlarm(e, p.Param["SetVehicleAlarm"]);
                    }

                    if (p.Param.ContainsKey("SetVehicleBodyHealth"))
                    {
                        SetVehicleBodyHealth(e, p.Param["SetVehicleBodyHealth"]);
                    }

                    if (p.Param.ContainsKey("SetVehicleColourCombination"))
                    {
                        SetVehicleColourCombination(e, p.Param["SetVehicleColourCombination"]);
                    }

                    if (p.Param.ContainsKey("SetVehicleColours"))
                    {
                        SetVehicleColours(e, p.Param["SetVehicleColours"][0], p.Param["SetVehicleColours"][1]);
                    }

                    if (p.Param.ContainsKey("SetVehicleCustomPrimaryColour"))
                    {
                        SetVehicleCustomPrimaryColour(e, p.Param["SetVehicleCustomPrimaryColour"][0], p.Param["SetVehicleCustomPrimaryColour"][1], p.Param["SetVehicleCustomPrimaryColour"][2]);
                    }

                    if (p.Param.ContainsKey("SetVehicleCustomSecondaryColour"))
                    {
                        SetVehicleCustomSecondaryColour(e, p.Param["SetVehicleCustomSecondaryColour"][0], p.Param["SetVehicleCustomSecondaryColour"][1], p.Param["SetVehicleCustomSecondaryColour"][2]);
                    }

                    if (p.Param.ContainsKey("SetVehicleDirtLevel"))
                    {
                        SetVehicleDirtLevel(e, p.Param["SetVehicleDirtLevel"]);
                    }

                    if (p.Param.ContainsKey("SetVehicleDoorBroken"))
                    {
                        SetVehicleDoorBroken(e, p.Param["SetVehicleDoorBroken"][0], p.Param["SetVehicleDoorBroken"][1]);
                    }

                    if (p.Param.ContainsKey("SetVehicleDoorsLocked"))
                    {
                        SetVehicleDoorsLocked(e, p.Param["SetVehicleDoorsLocked"]);
                    }

                    if (p.Param.ContainsKey("SetVehicleNumberPlateText"))
                    {
                        if (!string.IsNullOrEmpty(p.Param["SetVehicleNumberPlateText"]))
                        {
                            SetVehicleNumberPlateText(e, p.Param["SetVehicleNumberPlateText"]);
                        }
                    }
                    #endregion

                    int net_ = NetworkGetNetworkIdFromEntity(e);
                    Players[p.SourceId]?.TriggerEvent("mmNetworking:resp", p.RequestId, net_);
                    _.Remove(p);
                }

                if (_.Count == 0)
                {
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
        public IDictionary<string, dynamic> Param { get; set; }
    }
}