﻿using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using MEC;
using SpireSCP.GUI.API.Features;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using Interactables.Interobjects.DoorUtils;
using Players = Exiled.Events.Handlers;
using Exiled.API.Features.Items;
using AdminToys;
using Exiled.Events.Handlers;
using Exiled.Events.EventArgs.Scp096;
using Player = Exiled.API.Features.Player;


namespace ObscureLabs.Modules.Gamemode_Handler.Core
{
    internal class MvpSystem : Plugin.Module
    {
        public override string name { get; set; } = "MVPSystem";
        public override bool initOnStart { get; set; } = true;

        public override bool Init()
        {
            try
            {
                Exiled.Events.Handlers.Server.RoundStarted += roundStarted;
                Exiled.Events.Handlers.Player.Escaping += escaping;
                Exiled.Events.Handlers.Player.Died += killingPlayer;
                Exiled.Events.Handlers.Player.UnlockingGenerator += unlockingGenerator;
                Exiled.Events.Handlers.Server.RoundEnded += endingRound;
                Exiled.Events.Handlers.Player.EscapingPocketDimension += escapingPD;
                Exiled.Events.Handlers.Player.ActivatingWarheadPanel += warheadOpen;
                Exiled.Events.Handlers.Scp096.AddingTarget += shyguySeen;
                base.Init();
                return true;
            }
            catch { return false; }
        }

        public override bool Disable()
        {
            try
            {
                _PlayerData.Clear();
                firstPlace = null;
                secondPlace = null;
                thirdPlace = null;
                Exiled.Events.Handlers.Server.RoundStarted -= roundStarted;
                Exiled.Events.Handlers.Player.Escaping -= escaping;
                Exiled.Events.Handlers.Player.Died -= killingPlayer;
                Exiled.Events.Handlers.Player.UnlockingGenerator -= unlockingGenerator;
                Exiled.Events.Handlers.Server.RoundEnded -= endingRound;
                Exiled.Events.Handlers.Player.EscapingPocketDimension -= escapingPD;
                Exiled.Events.Handlers.Player.ActivatingWarheadPanel -= warheadOpen;
                Exiled.Events.Handlers.Scp096.AddingTarget -= shyguySeen;
                base.Disable();
                return true;
            }
            catch { return false; };
        }

        public class PlayerDataInstance
        {
            public Exiled.API.Features.Player player { get; set; }
            public int xp { get; set; }

        }

        static List<PlayerDataInstance> _PlayerData = new List<PlayerDataInstance>();
        static PlayerDataInstance firstPlace;
        static PlayerDataInstance secondPlace;
        static PlayerDataInstance thirdPlace;
        public static void roundStarted()
        {
            foreach (Player p in Plugin.PlayerList)
            {
                _PlayerData.Add(new PlayerDataInstance
                {
                    player = p,
                    xp = 0
                });
            }
        }

        public static void shyguySeen(AddingTargetEventArgs ev)
        {
            if (ev.IsLooking)
            {
                Timing.RunCoroutine(addXPtoPlayer(ev.Player, 1, "Had a player look at your face..."));
            }
        }
        public static void warheadOpen(ActivatingWarheadPanelEventArgs ev)
        {
            Timing.RunCoroutine(addXPtoPlayer(ev.Player, 7, "Opening The Warhead Panel"));
        }


        public static void escapingPD(EscapingPocketDimensionEventArgs ev)
        {
            Timing.RunCoroutine(addXPtoPlayer(ev.Player, 3, "Escaping The Pocket Dimension"));
        }
            
        public static void escaping(EscapingEventArgs ev)
        {
            if (!ev.IsAllowed) { return; }
            if (ev.Player.IsCuffed)
            {
                Timing.RunCoroutine(addXPtoPlayer(ev.Player.Cuffer, 5, "Recruiting Another Player"));
                Timing.RunCoroutine(addXPtoPlayer(ev.Player, 2, "Escaping as a prisoner"));
            }
            else
            {
                Timing.RunCoroutine(addXPtoPlayer(ev.Player, 3, "Escaping Facility"));
            }


        }

        public static void unlockingGenerator(UnlockingGeneratorEventArgs ev)
        {
            
            List<ItemType> keycards = new List<ItemType>
            {
                ItemType.KeycardChaosInsurgency,
            };

            if (ev.Player.Items.Any(item => item is Keycard keycard && keycard.Base.Permissions.HasFlag(KeycardPermissions.ArmoryLevelTwo)))
            {
                Timing.RunCoroutine(addXPtoPlayer(ev.Player, 2, "Unlocking Generator"));
            }
            else { return; }
        }

        public static void killingPlayer(DiedEventArgs ev)
        {
            if (ev.Attacker != null)
            {
                Timing.RunCoroutine(addXPtoPlayer(ev.Attacker, 5, $"Killing Player: {ev.Player.DisplayNickname}"));
            }
        }

        public static void endingRound(RoundEndedEventArgs ev)
        {
            Log.Warn("ROUND ENDED");
            _PlayerData = _PlayerData.OrderByDescending(p => p.xp).ToList();
            //Log.Warn($"{_PlayerData.ToArray()[0].player.DisplayNickname} . {_PlayerData.ToArray()[0].xp}");
            PlayerDataInstance[] PlayerData = _PlayerData.ToArray();
            string h = string.Empty;
            if(PlayerData.Count() == 0)
            {
                h = "There was no MVP this round :(";
            }
            if (PlayerData.Count() == 1)
            {
                if (PlayerData[0].xp > 0)
                {
                    h += $"This rounds MVP was: {PlayerData[0].player.DisplayNickname} with {PlayerData[0].xp} points!";
                }
                else
                {
                    h = "There was no MVP this round :(";
                }
            }
            if (PlayerData.Count() == 2)
            {
                if (PlayerData[0].xp > 0)
                {
                    h += $"This rounds MVP was: {PlayerData[0].player.DisplayNickname} with {PlayerData[0].xp} points!";
                }
                else
                {
                    h = "There was no MVP this round :(";
                }
                if (PlayerData[1].xp > 0)
                {
                    h += $"#2 was: {PlayerData[1].player.DisplayNickname} with {PlayerData[1].xp} points!";
                }
            }
            if (PlayerData.Count() >= 3)
            {
                if (PlayerData[0].xp > 0)
                {
                    h += $"This rounds MVP was: {PlayerData[0].player.DisplayNickname} with {PlayerData[0].xp} points!";
                }
                else
                {
                    h = "There was no MVP this round :(";
                }
                if (PlayerData[1].xp > 0)
                {
                    h += $"#2 was: {PlayerData[1].player.DisplayNickname} with {PlayerData[1].xp} points!";
                }
                if (PlayerData[2].xp > 0)
                {
                    h += $"#3 was: {PlayerData[2].player.DisplayNickname} with {PlayerData[2].xp} points!";
                }
            }

            foreach (Player p in Plugin.PlayerList)
            {
                Manager.SendHint(p, h, 10);
            }


        }

        public static IEnumerator<float> addXPtoPlayer(Player p, int xp, string reason)
        {
            yield return Timing.WaitForOneFrame;
                if (p != null && reason != null)
                _PlayerData.FirstOrDefault(x => x.player.Id == p.Id).xp += xp;
                Manager.SendHint(p, $"You Gained {xp} points for: {reason}", 5);
        }
    }
}


