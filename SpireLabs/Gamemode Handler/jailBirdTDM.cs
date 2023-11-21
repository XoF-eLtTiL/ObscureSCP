﻿using AudioPlayer.Commands.SubCommands;
using Exiled.Events.EventArgs.Server;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Warhead;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Extensions;
using Exiled.API.Features;
using MEC;
using PlayerRoles;
using MapEditorReborn;
namespace SpireLabs.Gamemode_Handler
{
    internal static class jailBirdTDM
    {
        static bool team = true;
        public static IEnumerator<float> startJbTDM()
        {

            var rnd = new System.Random();
            int num = rnd.Next(1, 100);
            Log.Info($"Minigame RNG was: {num} (should be between 30-40)");
            if (num >= 30 && num <= 40)
            {
                Timing.RunCoroutine(runJbTDM());
                yield return Timing.WaitForOneFrame;
            }
        }



            public static IEnumerator<float> runJbTDM()
            {
            yield return Timing.WaitForOneFrame;

                Log.Warn("Unloading all default maps");
                MapEditorReborn.API.Features.MapUtils.LoadMap("empty");
                Log.Warn("Loading Map: pvpA1_2t");
                MapEditorReborn.API.Features.MapUtils.LoadMap("pvpA1_2t");
                foreach (Player p in Player.List)
                {
                    p.Scale = new UnityEngine.Vector3(1, 1, 1);
                    yield return Timing.WaitForOneFrame;
                    if (team == true)
                    {
                        Log.Warn($"{p.DisplayNickname} is CHAOS INSURGENCY TEAM");
                        team = false;
                        p.Role.Set(RoleTypeId.ChaosConscript);
                        p.ClearInventory(true);
                        p.Teleport(new UnityEngine.Vector3(8.48f, 1106.5f, 35.46f));
                    p.Rotation.SetLookRotation(new UnityEngine.Vector3(0, 180, 0));
                }
                else
                    {
                        Log.Warn($"{p.DisplayNickname} is NTF TEAM");
                        team = true;
                        p.Role.Set(RoleTypeId.NtfSergeant);
                        p.ClearInventory(true);
                        p.Teleport(new UnityEngine.Vector3(-20.8f, 1107.5f, 51.66f));
                    p.Rotation.SetLookRotation(new UnityEngine.Vector3(0, 180, 0));
                }
                yield return Timing.WaitForOneFrame;
            }
            }

        }
    }
