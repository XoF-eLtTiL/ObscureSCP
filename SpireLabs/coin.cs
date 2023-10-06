﻿namespace SpireLabs
{
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Item;
    using Exiled.Events.EventArgs.Player;
    using MEC;
    using PlayerRoles;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using UnityEngine;
    using System.IO;
    using Exiled.CustomItems.API.Features;
    using Exiled.API.Features.Spawn;
    using Exiled.API.Features.Pickups;
    using Exiled.API.Features.Doors;
    using System;
    using PlayerRoles.FirstPersonControl;
    using HarmonyLib;
    using System.Reflection.Emit;
    using NorthwoodLib.Pools;
    using Mirror;
    using Exiled.API.Features.Roles;
    using static UnityEngine.GraphicsBuffer;
    using InventorySystem.Items.ThrowableProjectiles;
    using Exiled.API.Features.Items;
    using Hazards;
    using Exiled.API.Features.Hazards;
    using Utf8Json.Resolvers.Internal;
    using Exiled.API.Features.Toys;
    using InventorySystem.Items.Usables.Scp330;
    
    internal static class coin
    {
        public static string[] good = { "You gained 20HP!", "You gained a 5 second speed boost!", "You found a keycard!", "You are invisible for 5 seconds!", "You are healed!", "GRENADE FOUNTAIN!", "Ammo pile!!", "FREE CANDY!", "You can't die for the next 3s!", "You bring health to those around you!", "Nice hat.." };
        public static string[] bad = { "You now have 50HP!", "You dropped all of your items, How clumsy...", "You have heavy feet for 5 seconds...", "Pocket Sand!", "You got lost and found yourself in a random room!", "You flipped the coin so hard your hands fell off!", "BOOM!", "Sent To Brazil!!!", "You have been placed under the upside down curse, but only others can see it!", "You caused a blackout in your zone!", "Door stuck! DOOR STUCK!", "Your coin melted :(" };

        private static IEnumerator<float> grenadeFountain(Player p)
        {
            int bombs = 0;
            while (bombs != 5)
            {
                p.ThrowGrenade(ProjectileType.FragGrenade, false);
                p.ThrowGrenade(ProjectileType.FragGrenade, false);
                yield return Timing.WaitForSeconds(0.1f);
                bombs++;
            }
        }

        private static IEnumerator<float> ammoFountain(Player p)
        {
            var itemTotal = 0;
            var rnd = new System.Random();
            int num = rnd.Next(0, 100);
            if (num >= 95)
            {
                itemTotal = 80;
            }
            else
            {
                itemTotal = 10;
            }

            Log.Info("Running ammo fountain on " + p);
            int items = 0;
            while (items != itemTotal)
            {
              Pickup.CreateAndSpawn(ItemType.Ammo9x19, p.Position, p.Rotation);
              Pickup.CreateAndSpawn(ItemType.Ammo762x39, p.Position, p.Rotation);
              Pickup.CreateAndSpawn(ItemType.Ammo12gauge, p.Position, p.Rotation);
              Pickup.CreateAndSpawn(ItemType.Ammo556x45, p.Position, p.Rotation);
              Pickup.CreateAndSpawn(ItemType.Ammo44cal, p.Position, p.Rotation);


                yield return Timing.WaitForOneFrame;
                items++;
            }
        }
        private static IEnumerator<float> candyFountain(Player p)
        {
            var rnd = new System.Random();
            Log.Info("Running candy fountain on " + p);
            int items = 0;
            while (items != 10)
            {
                Exiled.API.Features.Pickups.Scp330Pickup Scp330 = (Exiled.API.Features.Pickups.Scp330Pickup)Pickup.Create(ItemType.SCP330);
                int num = rnd.Next(0, 6);
                if (num == 0)
                {
                    Scp330.Candies.Add(CandyKindID.Red);
                }
                else if (num == 1)
                {
                    Scp330.Candies.Add(CandyKindID.Blue);
                }
                else if (num == 2)
                {
                    Scp330.Candies.Add(CandyKindID.Yellow);
                }
                else if (num == 3)
                {
                    Scp330.Candies.Add(CandyKindID.Green);
                }
                else if (num == 4)
                {
                    Scp330.Candies.Add(CandyKindID.Purple);
                }
                else if (num == 5)
                {
                    Scp330.Candies.Add(CandyKindID.Rainbow);
                }
                else if (num == 6)
                {
                    Scp330.Candies.Add(CandyKindID.Pink);
                }
                Pickup.Spawn(Scp330, new Vector3(p.Position.x, p.Position.y, p.Position.z), p.Rotation);

                yield return Timing.WaitForOneFrame;

                items++;
            }
        }

        private static IEnumerator<float> scl(Player p)
        {
            p.Scale = Vector3.one * -1;
            yield return Timing.WaitForSeconds(30);
            p.Scale = Vector3.one;

        }

        private static void pp(Player pl)
        {
            

            Primitive p = Primitive.Create(pl.Transform.position + (pl.Transform.forward*1.25f) +(pl.Transform.up * 0.75f), Vector3.zero, Vector3.one, false);

            p.Color = new Color(0, 255, 0);
            p.Scale = Vector3.one * 0.25f;
            p.MovementSmoothing = 125;
            p.Base.gameObject.tag = "Cube";
            p.Spawn();
            p.Base.gameObject.transform.SetParent(pl.CameraTransform);

        }

        private static IEnumerator<float> raycastHeal(Player p)
        {
            for (int l = 0; l < 10; l++)
            {
                foreach (Player pp in Player.List)
                {
                    if (pp.DisplayNickname == null)
                    { }
                    else
                    {
                        if (pp != p)
                        {
                            RaycastHit hit;
                            Vector3 dir = pp.Transform.position - new Vector3(p.Transform.position.x - 0.1f, p.Transform.position.y, p.Transform.position.z);
                            dir = dir.normalized;
                            Ray r = new Ray(new Vector3(p.Transform.position.x - 0.1f, p.Transform.position.y, p.Transform.position.z), dir);
                            Physics.Raycast(r, out hit, maxDistance: 5);
                            Player ppp;
                            Player.TryGet(hit.collider, out ppp);
                            if (ppp != pp)
                            {
                                dir = pp.Transform.position - new Vector3(p.Transform.position.x + 0.1f, p.Transform.position.y, p.Transform.position.z);
                                r = new Ray(new Vector3(p.Transform.position.x + 0.1f, p.Transform.position.y, p.Transform.position.z), dir);
                                Physics.Raycast(r, out hit, maxDistance: 5);
                                Player.TryGet(hit.collider, out ppp);
                                if (ppp != pp)
                                {
                                    dir = pp.Transform.position - new Vector3(p.Transform.position.x, p.Transform.position.y, p.Transform.position.z + 0.1f);
                                    r = new Ray(new Vector3(p.Transform.position.x, p.Transform.position.y, p.Transform.position.z + 0.1f), dir);
                                    Physics.Raycast(r, out hit, maxDistance: 5);
                                    Player.TryGet(hit.collider, out ppp);
                                    if (ppp != pp)
                                    {
                                        dir = pp.Transform.position - new Vector3(p.Transform.position.x, p.Transform.position.y, p.Transform.position.z - 0.1f);
                                        r = new Ray(new Vector3(p.Transform.position.x, p.Transform.position.y, p.Transform.position.z - 0.1f), dir);
                                        Physics.Raycast(r, out hit, maxDistance: 5);
                                        Player.TryGet(hit.collider, out ppp);
                                        if (ppp != pp)
                                        {
                                            dir = pp.Transform.position - new Vector3(p.Transform.position.x, p.Transform.position.y + 0.1f, p.Transform.position.z);
                                            r = new Ray(new Vector3(p.Transform.position.x, p.Transform.position.y + 0.1f, p.Transform.position.z), dir);
                                            Physics.Raycast(r, out hit, maxDistance: 5);
                                            Player.TryGet(hit.collider, out ppp);
                                        }
                                        else
                                        {
                                            if (ppp.IsHuman)
                                            {
                                                ppp.Heal(7.5f, false);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (ppp.IsHuman)
                                        {
                                            ppp.Heal(7.5f, false);
                                        }
                                    }
                                }
                                else
                                {
                                    if (ppp.IsHuman)
                                    {
                                        ppp.Heal(7.5f, false);
                                    }
                                }
                            }
                            else
                            {
                                if (ppp.IsHuman)
                                {
                                    ppp.Heal(7.5f, false);
                                }
                            }
                        }
                    }

                }
                yield return Timing.WaitForSeconds(0.5f);
            }
            yield return Timing.WaitForSeconds(1f);
        }

        internal static void Player_FlippingCoin(FlippingCoinEventArgs ev)
        {
            //pp(ev.Player);
            var rnd = new System.Random();
            int num = rnd.Next(0, 100);
            int result = 0;
            if (num > 20 && num < 45) result = 1;
            if (num > 45 && num < 100) result = 2;
            if (result == 1)
            {
                Exiled.API.Features.Log.Info($"{ev.Player.Nickname} flipped a coin and got a good result!");
                switch (rnd.Next(0, good.Count()))
                {
                    case 0:
                        //ev.Player.ShowHint(good[0], 3);
                        Timing.RunCoroutine(guiHandler.sendHint(ev.Player, "\n\n\n\n\n\n\n\n\n\n" + good[0], 5));
                        ev.Player.Heal(20, true);
                        if (ev.Player.Role == RoleTypeId.NtfCaptain)
                        {
                            ev.Player.MaxHealth = 150;
                        }
                        else
                        {
                            ev.Player.MaxHealth = 100;
                        }
                        break;
                    case 1:
                        Timing.RunCoroutine(guiHandler.sendHint(ev.Player, "\n\n\n\n\n\n\n\n\n\n" + good[1], 5));
                        ev.Player.EnableEffect(EffectType.MovementBoost, 5);
                        ev.Player.ChangeEffectIntensity(EffectType.MovementBoost, 105, 5);
                        break;
                    case 2:
                        bool todrop = false;
                        Timing.RunCoroutine(guiHandler.sendHint(ev.Player, "\n\n\n\n\n\n\n\n\n\n" +  good[2], 5));
                        if (ev.Player.IsInventoryFull)
                        {
                            todrop = true;

                        }
                        else
                        {
                            todrop = false;
                        }

                        var rnd2 = new System.Random();
                        int card = rnd2.Next(1, 3);
                        if (card == 1)
                        {
                            if (todrop)
                            {
                                Pickup.CreateAndSpawn(ItemType.KeycardZoneManager, ev.Player.Position, ev.Player.Rotation);
                            }
                            else
                            {
                                ev.Player.AddItem(ItemType.KeycardZoneManager);
                            }
                        }
                        else if (card == 2)
                        {
                            if (todrop)
                            {
                                Pickup.CreateAndSpawn(ItemType.KeycardMTFOperative, ev.Player.Position, ev.Player.Rotation);
                            }
                            else
                            {
                                ev.Player.AddItem(ItemType.KeycardMTFOperative);
                            }
                        }
                        else
                        {
                            if (todrop)
                            { 
                                Pickup.CreateAndSpawn(ItemType.KeycardResearchCoordinator, ev.Player.Position, ev.Player.Rotation);
                            }
                            else
                            {
                                ev.Player.AddItem(ItemType.KeycardResearchCoordinator);
                            }
                        }
                        break;
                    case 3:
                        Timing.RunCoroutine(guiHandler.sendHint(ev.Player, "\n\n\n\n\n\n\n\n\n\n" + good[3], 5));
                        ev.Player.EnableEffect(EffectType.Invisible, 5);
                        break;
                    case 4:
                        ev.Player.Heal(150, false);
                        Timing.RunCoroutine(guiHandler.sendHint(ev.Player, "\n\n\n\n\n\n\n\n\n\n" + good[4], 5));
                        break;
                    case 5:
                        Timing.RunCoroutine(guiHandler.sendHint(ev.Player, "\n\n\n\n\n\n\n\n\n\n" + good[5], 5));
                        Timing.RunCoroutine(grenadeFountain(ev.Player));
                        break;
                    case 6:
                        Timing.RunCoroutine(guiHandler.sendHint(ev.Player, "\n\n\n\n\n\n\n\n\n\n" + good[6], 5));
                        Timing.RunCoroutine(ammoFountain(ev.Player));
                        break;
                    case 7:
                        Timing.RunCoroutine(guiHandler.sendHint(ev.Player, "\n\n\n\n\n\n\n\n\n\n" + good[7], 5));
                        Timing.RunCoroutine(candyFountain(ev.Player));
                        break;
                    case 8:
                        Timing.RunCoroutine(guiHandler.sendHint(ev.Player, "\n\n\n\n\n\n\n\n\n\n" + good[8], 5));
                        ev.Player.EnableEffect(EffectType.DamageReduction, 3);
                        ev.Player.ChangeEffectIntensity(EffectType.DamageReduction, 255, 3);
                        break;
                    case 9:
                        Timing.RunCoroutine(guiHandler.sendHint(ev.Player, "\n\n\n\n\n\n\n\n\n\n" + good[9], 5));
                        Timing.RunCoroutine(raycastHeal(ev.Player));
                        break;
                    case 10:
                        Timing.RunCoroutine(guiHandler.sendHint(ev.Player, "\n\n\n\n\n\n\n\n\n\n" + good[10], 5));
                        bool tp = false;
                        if (ev.Player.IsInventoryFull) tp = true;
                        else tp = false;
                        if (tp)
                        {
                            Pickup.CreateAndSpawn(ItemType.SCP268, ev.Player.Position, ev.Player.Rotation);
                        }
                        else
                        {
                            ev.Player.AddItem(ItemType.SCP268);
                        }
                        break;
                }

            }
            else if (result == 2)
            {
                Exiled.API.Features.Log.Info($"{ev.Player.Nickname} flipped a coin and got a bad result!");
                switch (rnd.Next(0, bad.Count()))
                {
                    case 0:
                        Timing.RunCoroutine(guiHandler.sendHint(ev.Player, "\n\n\n\n\n\n\n\n\n\n" + bad[0], 5));
                        ev.Player.Health = 50;
                        break;
                    case 1:
                        Timing.RunCoroutine(guiHandler.sendHint(ev.Player, "\n\n\n\n\n\n\n\n\n\n" + bad[1], 5));
                        ev.Player.DropItems();
                        break;
                    case 2:
                        Timing.RunCoroutine(guiHandler.sendHint(ev.Player, "\n\n\n\n\n\n\n\n\n\n" + bad[2], 5));
                        ev.Player.EnableEffect(EffectType.SinkHole, 5);
                        break;
                    case 3:
                        Timing.RunCoroutine(guiHandler.sendHint(ev.Player, "\n\n\n\n\n\n\n\n\n\n" + bad[3], 5));
                        ev.Player.EnableEffect(EffectType.Flashed, 5);
                        break;
                    case 4:
                        if (Warhead.IsDetonated)
                            break;
                        Timing.RunCoroutine(guiHandler.sendHint(ev.Player, "\n\n\n\n\n\n\n\n\n\n" + bad[4], 5));
                        var r = new System.Random();
                        var n = r.Next(0, 2);
                        bool goodRoom = false;
                        Room room = Room.List.ElementAt(4);
                        Door door = Room.List.ElementAt(4).Doors.FirstOrDefault();
                        while (goodRoom == false)
                        {
                            var roomNd = new System.Random();
                            int roomNum = roomNd.Next(0, Room.List.Count());
                            if (Map.IsLczDecontaminated)
                            {
                                if (Room.List.ElementAt(roomNum).Type != RoomType.HczTesla && Room.List.ElementAt(roomNum).Zone != ZoneType.LightContainment)
                                {
                                    goodRoom = true;
                                    door = Room.List.ElementAt(roomNum).Doors.FirstOrDefault();
                                }
                            }
                            else
                            {
                                if (Room.List.ElementAt(roomNum).Type != RoomType.HczTesla)
                                {
                                    goodRoom = true;
                                    door = Room.List.ElementAt(roomNum).Doors.FirstOrDefault();
                                }
                            }

                        }
                        ev.Player.Teleport(new Vector3(door.Position.x, door.Position.y + 1f, door.Position.z));

                        break;
                    case 5:
                        Timing.RunCoroutine(guiHandler.sendHint(ev.Player, "\n\n\n\n\n\n\n\n\n\n" + bad[5], 5));
                        ev.Player.EnableEffect(EffectType.SeveredHands, 999);
                        ev.Player.EnableEffect(EffectType.CardiacArrest, 60);
                        ev.Player.ChangeEffectIntensity(EffectType.CardiacArrest, 5);
                        break;
                    case 6:
                        ev.Player.Vaporize();
                        break;
                    case 7:
                        Timing.RunCoroutine(guiHandler.sendHint(ev.Player, "\n\n\n\n\n\n\n\n\n\n" + bad[7], 5));
                        ZoneType zt = ev.Player.CurrentRoom.Zone;
                        ev.Player.Teleport(Room.List.FirstOrDefault(x => x.Type == RoomType.Pocket));
                        ev.Player.EnableEffect(EffectType.PocketCorroding, 60);
                        Timing.RunCoroutine(enterPD(ev.Player, zt));
                        break;
                    case 8:
                        Timing.RunCoroutine(guiHandler.sendHint(ev.Player, "\n\n\n\n\n\n\n\n\n\n" + bad[8], 5));

                        ev.Player.Scale = Vector3.one * -1;
                        Timing.RunCoroutine(scl(ev.Player));
                        break;
                    case 9:
                        Timing.RunCoroutine(guiHandler.sendHint(ev.Player, "\n\n\n\n\n\n\n\n\n\n" + bad[9], 5));
                        var zone = ev.Player.CurrentRoom.Zone;
                        Map.TurnOffAllLights(7.5f, zone);
                        break;
                    case 10:
                        Timing.RunCoroutine(guiHandler.sendHint(ev.Player, "\n\n\n\n\n\n\n\n\n\n" + bad[10], 5));
                        Room rm = ev.Player.CurrentRoom;
                        rm.LockDown(10);
                        break;
                    case 11:
                        Timing.RunCoroutine(guiHandler.sendHint(ev.Player, "\n\n\n\n\n\n\n\n\n\n" + bad[11], 5));
                        ev.Player.RemoveHeldItem(true);
                        break;
                }
            }
            else
            {
                Exiled.API.Features.Log.Info($"{ev.Player.Nickname} flipped a coin and got nothing!");
                ev.Player.ShowHint("No consequences, this time...", 3);
                Timing.RunCoroutine(guiHandler.sendHint(ev.Player, "\n\n\n\n\n\n\n\n\n\n" + "No consequences, this time...", 3));
            }
        }

        private static IEnumerator<float> enterPD(Player p, ZoneType zt)
        {
            Door door = Room.List.FirstOrDefault().Doors.FirstOrDefault();
            yield return Timing.WaitForOneFrame;
            Exiled.Events.Handlers.Player.EscapingPocketDimension += ExitVoid;
            Exiled.Events.Handlers.Player.FailingEscapePocketDimension += FixThing;
            void ExitVoid(EscapingPocketDimensionEventArgs ev)
            {
                bool goodRoom = false;
                while (goodRoom == false)
                {
                    var roomNd = new System.Random();
                    int roomNum = roomNd.Next(0, Room.List.Count());
                    if (Room.List.ElementAt(roomNum).Type != RoomType.HczTesla && Room.List.ElementAt(roomNum).Zone == zt)
                    {
                        goodRoom = true;
                        door = Room.List.ElementAt(roomNum).Doors.FirstOrDefault();
                    }
                }
                ev.TeleportPosition = new Vector3(door.Position.x, door.Position.y + 1.5f, door.Position.z);
                p.DisableEffect(EffectType.PocketCorroding);
                Exiled.Events.Handlers.Player.EscapingPocketDimension -= ExitVoid;
                Exiled.Events.Handlers.Player.FailingEscapePocketDimension -= FixThing;
            }
            void FixThing(FailingEscapePocketDimensionEventArgs e)
            {
                Exiled.Events.Handlers.Player.EscapingPocketDimension -= ExitVoid;
                Exiled.Events.Handlers.Player.FailingEscapePocketDimension -= FixThing;
            }
            yield return Timing.WaitForOneFrame;
        }
    }
}