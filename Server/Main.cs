using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using System;
using System.Collections.Generic;
using System.Dynamic;

namespace Server
{
    public class Main : BaseScript
    {
        public Main()
        {
            EventHandlers["mmNetworking:CreateVehicle"] += new Action<Player, int, int, float, float, float, float, ExpandoObject>(CreateVehicle_);
        }

        public async void CreateVehicle_([FromSource]Player source, int reqId, int model, float x, float y, float z, float h, ExpandoObject p_)
        {
            try
            {
                bool _isnetwork = false;
                bool _netmissionentity = false;

                var p = p_ as IDictionary<string, dynamic>; 

                if (p.ContainsKey("IsNetwork"))
                {
                    _isnetwork = p["IsNetwork"];
                }

                if (p.ContainsKey("NetMissionEntity"))
                {
                    _netmissionentity = p["NetMissionEntity"];
                }

                int e = CreateVehicle((uint)model, x, y, z, h, _isnetwork, _netmissionentity);
                long init = GetGameTimer();

                while (!DoesEntityExist(e))
                {
                    await Delay(100);
                }

                #region Vehicle Properties
                if (p.ContainsKey("SetVehicleAlarm"))
                {
                    SetVehicleAlarm(e, p["SetVehicleAlarm"]);
                }

                if (p.ContainsKey("SetVehicleBodyHealth"))
                {
                    SetVehicleBodyHealth(e, p["SetVehicleBodyHealth"]);
                }

                if (p.ContainsKey("SetVehicleColourCombination"))
                {
                    SetVehicleColourCombination(e, p["SetVehicleColourCombination"]);
                }

                if (p.ContainsKey("SetVehicleColours"))
                {
                    SetVehicleColours(e, p["SetVehicleColours"][0], p["SetVehicleColours"][1]);
                }

                if (p.ContainsKey("SetVehicleCustomPrimaryColour"))
                {
                    SetVehicleCustomPrimaryColour(e, p["SetVehicleCustomPrimaryColour"][0], p["SetVehicleCustomPrimaryColour"][1], p["SetVehicleCustomPrimaryColour"][2]);
                }

                if (p.ContainsKey("SetVehicleCustomSecondaryColour"))
                {
                    SetVehicleCustomSecondaryColour(e, p["SetVehicleCustomSecondaryColour"][0], p["SetVehicleCustomSecondaryColour"][1], p["SetVehicleCustomSecondaryColour"][2]);
                }

                if (p.ContainsKey("SetVehicleDirtLevel"))
                {
                    SetVehicleDirtLevel(e, p["SetVehicleDirtLevel"]);
                }

                if (p.ContainsKey("SetVehicleDoorBroken"))
                {
                    SetVehicleDoorBroken(e, p["SetVehicleDoorBroken"][0], p["SetVehicleDoorBroken"][1]);
                }

                if (p.ContainsKey("SetVehicleDoorsLocked"))
                {
                    SetVehicleDoorsLocked(e, p["SetVehicleDoorsLocked"]);
                }

                if (p.ContainsKey("SetVehicleNumberPlateText"))
                {
                    if (!string.IsNullOrEmpty(p["SetVehicleNumberPlateText"]))
                    {
                        SetVehicleNumberPlateText(e, p["SetVehicleNumberPlateText"]);
                    }
                }
                #endregion

                int net_ = NetworkGetNetworkIdFromEntity(e);
                Players[Convert.ToInt32(source.Handle)]?.TriggerEvent("mmNetworking:resp", reqId, net_);
            }
            catch (Exception ex)
            { Log.Error(ex); }
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