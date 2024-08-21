using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using GTA;
using GTA.Native;
using GTA.Math;

namespace SprintWithControllerA
{
    public class SprintWithControllerA :Script
    {
        public class win32api
        {
            [DllImport("user32.dll")]
            public static extern uint keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);
        }
        public SprintWithControllerA()
        {
            Tick += OnTick;
        }
        private void OnTick(object sender, EventArgs e)
        {
            Ped playerPed = Game.Player.Character;

            float camDist = Vector3.Distance(playerPed.Position, GameplayCamera.Position);

            if (Game.Player.IsAlive && Game.Player.CanControlCharacter && camDist > 1.5f && !playerPed.IsInStealthMode)
            {
                if (playerPed.IsGettingIntoVehicle)
                {
                    Vehicle[] nearV = World.GetNearbyVehicles(playerPed.Position, 1.5f);
                    for (int i1 = 0; i1 < nearV.Length; i1++)
                    {
                        if (nearV[i1].IsBicycle)
                        {
                            Wait(600);
                            if (Function.Call<bool>(Hash.IS_PED_IN_VEHICLE, playerPed, nearV[i1], false))
                            {
                                Function.Call(Hash.SET_CURRENT_PED_WEAPON, playerPed, WeaponHash.Unarmed, true);
                            }

                        }
                    }
                }
                else if (!playerPed.IsStopped && playerPed.IsOnBike)
                {
                    if (Game.IsControlPressed(GTA.Control.Sprint))
                    {

                        for (int i = 0; i < 25; i++)
                        {
                            ReplayStop();
                            win32api.keybd_event(0x42, 0x30, 0, (UIntPtr)0);
                            //win32api.keybd_event(0xA0, 0x2A, 0, (UIntPtr)0); 自転車乗車時にShiftでスプリントを行うようにするとホッピング時に前に傾いてしまう
                            Wait(2);
                            ReplayStop();
                            win32api.keybd_event(0x42, 0x30, 2, (UIntPtr)0);
                            Wait(2);
                            if (!Game.IsControlPressed(GTA.Control.Sprint))
                            {
                                break;
                            }
                        }

                    }

                }
                else if (!playerPed.IsStopped && !playerPed.IsInVehicle())
                {
                    if (playerPed.IsSprinting || playerPed.IsRunning || playerPed.IsWalking
                        || playerPed.IsSwimming || playerPed.IsSwimmingUnderWater)
                    {
                        if (Game.IsControlPressed(GTA.Control.Sprint) && !playerPed.IsAiming && !playerPed.IsShooting && !Game.IsControlPressed(GTA.Control.Reload))
                        {
                            for (int i = 0; i < 50; i++)
                            {
                                ReplayStop();
                                win32api.keybd_event(0xA0, 0x2A, 0, (UIntPtr)0);
                                Wait(2);
                                win32api.keybd_event(0xA0, 0x2A, 2, (UIntPtr)0);
                                if (playerPed.IsAiming || playerPed.IsShooting)
                                {
                                    break;
                                }
                                if (!Game.IsControlPressed(GTA.Control.Sprint))
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
        private void ReplayStop()
        {
            Game.DisableControlThisFrame(GTA.Control.ReplayStartStopRecording);
            Game.DisableControlThisFrame(GTA.Control.ReplayStartStopRecordingSecondary);
            Game.DisableControlThisFrame(GTA.Control.SaveReplayClip);
            Game.DisableControlThisFrame(GTA.Control.ReplayRecord);
        } 
    }
}
