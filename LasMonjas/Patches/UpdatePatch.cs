using HarmonyLib;
using System;
using System.IO;
using System.Net.Http;
using UnityEngine;
using static LasMonjas.LasMonjas;
using LasMonjas.Objects;
using System.Collections.Generic;
using System.Linq;
using Hazel;
using static LasMonjas.RoleInfo;
using Reactor;
using static LasMonjas.MapOptions;
using LasMonjas.Core;
using static LasMonjas.HudManagerStartPatch;

namespace LasMonjas.Patches {
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    class HudManagerUpdatePatch
    {
        static void resetNameTagsAndColors() {
            Dictionary<byte, PlayerControl> playersById = Helpers.allPlayersById();

            foreach (PlayerControl player in PlayerControl.AllPlayerControls) {
                String playerName = player.Data.PlayerName;
                if (Mimic.transformTimer > 0f && Mimic.mimic == player && Mimic.transformTarget != null) playerName = Mimic.transformTarget.Data.PlayerName;
                if (Puppeteer.morphed && Puppeteer.puppeteer == player && Puppeteer.transformTarget != null) playerName = Puppeteer.transformTarget.Data.PlayerName;

                player.cosmetics.nameText.text = Helpers.hidePlayerName(PlayerControl.LocalPlayer, player) ? "" : playerName;
                if (PlayerControl.LocalPlayer.Data.Role.IsImpostor && player.Data.Role.IsImpostor) {
                    player.cosmetics.nameText.color = Palette.ImpostorRed;
                } else {
                    player.cosmetics.nameText.color = Color.white;
                }
            }
            if (MeetingHud.Instance != null) {
                foreach (PlayerVoteArea player in MeetingHud.Instance.playerStates) {
                    PlayerControl playerControl = playersById.ContainsKey((byte)player.TargetPlayerId) ? playersById[(byte)player.TargetPlayerId] : null;
                    if (playerControl != null) {
                        player.NameText.text = playerControl.Data.PlayerName;
                        if (PlayerControl.LocalPlayer.Data.Role.IsImpostor && playerControl.Data.Role.IsImpostor) {
                            player.NameText.color = Palette.ImpostorRed;
                        } else {
                            player.NameText.color = Color.white;
                        }
                    }
                }
            }
            if (PlayerControl.LocalPlayer.Data.Role.IsImpostor) {
                List<PlayerControl> impostors = PlayerControl.AllPlayerControls.ToArray().ToList();
                impostors.RemoveAll(x => !x.Data.Role.IsImpostor);
                foreach (PlayerControl player in impostors)
                    player.cosmetics.nameText.color = Palette.ImpostorRed;
                if (MeetingHud.Instance != null)
                    foreach (PlayerVoteArea player in MeetingHud.Instance.playerStates) {
                        PlayerControl playerControl = Helpers.playerById((byte)player.TargetPlayerId);
                        if (playerControl != null && playerControl.Data.Role.IsImpostor)
                            player.NameText.color =  Palette.ImpostorRed;
                    }
            }

        }

        static void setPlayerNameColor(PlayerControl p, Color color) {
            p.cosmetics.nameText.color = color;
            if (MeetingHud.Instance != null)
                foreach (PlayerVoteArea player in MeetingHud.Instance.playerStates)
                    if (player.NameText != null && p.PlayerId == player.TargetPlayerId)
                        player.NameText.color = color;
        }

        static void setNameColors() {

            if (howmanygamemodesareon == 1) {
                if (CaptureTheFlag.captureTheFlagMode) {
                    if (CaptureTheFlag.stealerPlayer != null) {
                        setPlayerNameColor(CaptureTheFlag.stealerPlayer, Palette.PlayerColors[15]);
                    }

                    foreach (PlayerControl redplayer in CaptureTheFlag.redteamFlag) {
                        if (redplayer != null) {
                            setPlayerNameColor(redplayer, Palette.PlayerColors[0]);
                        }
                    }
                    foreach (PlayerControl blueplayer in CaptureTheFlag.blueteamFlag) {
                        if (blueplayer != null) {
                            setPlayerNameColor(blueplayer, Palette.PlayerColors[1]);
                        }
                    }
                }
                else if (PoliceAndThief.policeAndThiefMode) {
                    foreach (PlayerControl policeplayer in PoliceAndThief.policeTeam) {
                        if (policeplayer != null) {
                            if (PoliceAndThief.policeplayer02 != null && policeplayer == PoliceAndThief.policeplayer02 || PoliceAndThief.policeplayer04 != null && policeplayer == PoliceAndThief.policeplayer04) {
                                setPlayerNameColor(policeplayer, Palette.PlayerColors[5]);
                            }
                            else {
                                setPlayerNameColor(policeplayer, Palette.PlayerColors[10]);
                            }
                        }
                    }
                    foreach (PlayerControl thiefplayer in PoliceAndThief.thiefTeam) {
                        if (thiefplayer != null) {
                            setPlayerNameColor(thiefplayer, Palette.PlayerColors[16]);
                        }
                    }
                }
                else if (KingOfTheHill.kingOfTheHillMode) {
                    if (KingOfTheHill.usurperPlayer != null) {
                        setPlayerNameColor(KingOfTheHill.usurperPlayer, Palette.PlayerColors[15]);
                    }

                    foreach (PlayerControl greenplayer in KingOfTheHill.greenTeam) {
                        if (greenplayer != null) {
                            setPlayerNameColor(greenplayer, Palette.PlayerColors[2]);
                        }
                    }
                    foreach (PlayerControl yellowplayer in KingOfTheHill.yellowTeam) {
                        if (yellowplayer != null) {
                            setPlayerNameColor(yellowplayer, Palette.PlayerColors[5]);
                        }
                    }
                }
                else if (HotPotato.hotPotatoMode) {

                    foreach (PlayerControl notpotatoplayer in HotPotato.notPotatoTeam) {
                        if (notpotatoplayer != null) {
                            setPlayerNameColor(notpotatoplayer, Palette.PlayerColors[10]);
                        }
                    }

                    foreach (PlayerControl explodedpotatoplayer in HotPotato.explodedPotatoTeam) {
                        if (explodedpotatoplayer != null) {
                            setPlayerNameColor(explodedpotatoplayer, Palette.PlayerColors[9]);
                        }
                    }

                    if (HotPotato.hotPotatoPlayer != null) {
                        setPlayerNameColor(HotPotato.hotPotatoPlayer, Palette.PlayerColors[15]);
                    }
                }
                else if (ZombieLaboratory.zombieLaboratoryMode) {

                    foreach (PlayerControl survivorPlayer in ZombieLaboratory.survivorTeam) {
                        if (survivorPlayer != null) {
                            setPlayerNameColor(survivorPlayer, Palette.PlayerColors[10]);
                        }
                    }

                    foreach (PlayerControl infectedPlayer in ZombieLaboratory.infectedTeam) {
                        if (infectedPlayer != null) {
                            setPlayerNameColor(infectedPlayer, Palette.PlayerColors[5]);
                        }
                    }

                    foreach (PlayerControl zombiePlayer in ZombieLaboratory.zombieTeam) {
                        if (zombiePlayer != null) {
                            setPlayerNameColor(zombiePlayer, Palette.PlayerColors[16]);
                        }
                    }

                    if (ZombieLaboratory.nursePlayer != null) {
                        setPlayerNameColor(ZombieLaboratory.nursePlayer, Palette.PlayerColors[3]);
                    }
                }
                else if (BattleRoyale.battleRoyaleMode) {

                    if (BattleRoyale.matchType == 0) {
                        foreach (PlayerControl soloPlayer in BattleRoyale.soloPlayerTeam) {
                            if (soloPlayer != null) {
                                setPlayerNameColor(soloPlayer, Palette.PlayerColors[2]);
                            }
                        }
                    }
                    else {
                        if (BattleRoyale.serialKiller != null) {
                            setPlayerNameColor(BattleRoyale.serialKiller, Palette.PlayerColors[15]);
                        }

                        foreach (PlayerControl limeplayer in BattleRoyale.limeTeam) {
                            if (limeplayer != null) {
                                setPlayerNameColor(limeplayer, Palette.PlayerColors[11]);
                            }
                        }

                        foreach (PlayerControl pinkplayer in BattleRoyale.pinkTeam) {
                            if (pinkplayer != null) {
                                setPlayerNameColor(pinkplayer, Palette.PlayerColors[13]);
                            }
                        }
                    }
                }
            }
            else {
                // Crewmates name color
                if (Captain.captain != null && Captain.captain == PlayerControl.LocalPlayer)
                    setPlayerNameColor(Captain.captain, Captain.color);
                else if (Mechanic.mechanic != null && Mechanic.mechanic == PlayerControl.LocalPlayer)
                    setPlayerNameColor(Mechanic.mechanic, Mechanic.color);
                else if (Sheriff.sheriff != null && Sheriff.sheriff == PlayerControl.LocalPlayer)
                    setPlayerNameColor(Sheriff.sheriff, Sheriff.color);
                else if (Detective.detective != null && Detective.detective == PlayerControl.LocalPlayer)
                    setPlayerNameColor(Detective.detective, Detective.color);
                else if (Forensic.forensic != null && Forensic.forensic == PlayerControl.LocalPlayer)
                    setPlayerNameColor(Forensic.forensic, Forensic.color);
                else if (TimeTraveler.timeTraveler != null && TimeTraveler.timeTraveler == PlayerControl.LocalPlayer)
                    setPlayerNameColor(TimeTraveler.timeTraveler, TimeTraveler.color);
                else if (Squire.squire != null && Squire.squire == PlayerControl.LocalPlayer)
                    setPlayerNameColor(Squire.squire, Squire.color);
                else if (Cheater.cheater != null && Cheater.cheater == PlayerControl.LocalPlayer)
                    setPlayerNameColor(Cheater.cheater, Cheater.color);
                else if (FortuneTeller.fortuneTeller != null && FortuneTeller.fortuneTeller == PlayerControl.LocalPlayer)
                    setPlayerNameColor(FortuneTeller.fortuneTeller, FortuneTeller.color);
                else if (Hacker.hacker != null && Hacker.hacker == PlayerControl.LocalPlayer)
                    setPlayerNameColor(Hacker.hacker, Hacker.color);
                else if (Sleuth.sleuth != null && Sleuth.sleuth == PlayerControl.LocalPlayer)
                    setPlayerNameColor(Sleuth.sleuth, Sleuth.color);
                else if (Fink.fink != null && Fink.fink == PlayerControl.LocalPlayer)
                    setPlayerNameColor(Fink.fink, Fink.color);
                else if (Kid.kid != null && Kid.kid == PlayerControl.LocalPlayer)
                    setPlayerNameColor(Kid.kid, Kid.color);
                else if (Welder.welder != null && Welder.welder == PlayerControl.LocalPlayer)
                    setPlayerNameColor(Welder.welder, Welder.color);
                else if (Spiritualist.spiritualist != null && Spiritualist.spiritualist == PlayerControl.LocalPlayer)
                    setPlayerNameColor(Spiritualist.spiritualist, Spiritualist.color);
                else if (Coward.coward != null && Coward.coward == PlayerControl.LocalPlayer)
                    setPlayerNameColor(Coward.coward, Coward.color);
                else if (Vigilant.vigilant != null && Vigilant.vigilant == PlayerControl.LocalPlayer)
                    setPlayerNameColor(Vigilant.vigilant, Vigilant.color);
                else if (Vigilant.vigilantMira != null && Vigilant.vigilantMira == PlayerControl.LocalPlayer)
                    setPlayerNameColor(Vigilant.vigilantMira, Vigilant.color);
                else if (Hunter.hunter != null && Hunter.hunter == PlayerControl.LocalPlayer)
                    setPlayerNameColor(Hunter.hunter, Hunter.color);
                else if (Jinx.jinx != null && Jinx.jinx == PlayerControl.LocalPlayer)
                    setPlayerNameColor(Jinx.jinx, Jinx.color);
                else if (Bat.bat != null && Bat.bat == PlayerControl.LocalPlayer)
                    setPlayerNameColor(Bat.bat, Bat.color);
                else if (Necromancer.necromancer != null && Necromancer.necromancer == PlayerControl.LocalPlayer)
                    setPlayerNameColor(Necromancer.necromancer, Necromancer.color);
                else if (Engineer.engineer != null && Engineer.engineer == PlayerControl.LocalPlayer)
                    setPlayerNameColor(Engineer.engineer, Engineer.color);
                else if (Shy.shy != null && Shy.shy == PlayerControl.LocalPlayer)
                    setPlayerNameColor(Shy.shy, Shy.color);

                // Neutrals name color
                else if (Joker.joker != null && Joker.joker == PlayerControl.LocalPlayer)
                    setPlayerNameColor(Joker.joker, Joker.color);
                else if (RoleThief.rolethief != null && RoleThief.rolethief == PlayerControl.LocalPlayer)
                    setPlayerNameColor(RoleThief.rolethief, RoleThief.color);
                else if (Pyromaniac.pyromaniac != null && Pyromaniac.pyromaniac == PlayerControl.LocalPlayer) {
                    setPlayerNameColor(Pyromaniac.pyromaniac, Pyromaniac.color);
                }
                else if (TreasureHunter.treasureHunter != null && TreasureHunter.treasureHunter == PlayerControl.LocalPlayer) {
                    setPlayerNameColor(TreasureHunter.treasureHunter, TreasureHunter.color);
                }
                else if (Devourer.devourer != null && Devourer.devourer == PlayerControl.LocalPlayer) {
                    setPlayerNameColor(Devourer.devourer, Devourer.color);
                }
                else if (Poisoner.poisoner != null && Poisoner.poisoner == PlayerControl.LocalPlayer) {
                    setPlayerNameColor(Poisoner.poisoner, Poisoner.color);
                }
                else if (Puppeteer.puppeteer != null && Puppeteer.puppeteer == PlayerControl.LocalPlayer)
                    setPlayerNameColor(Puppeteer.puppeteer, Puppeteer.color);

                // Rebels name color
                else if (Renegade.renegade != null && Renegade.renegade == PlayerControl.LocalPlayer) {
                    // Renegade can see his minion
                    setPlayerNameColor(Renegade.renegade, Renegade.color);
                    if (Minion.minion != null) {
                        setPlayerNameColor(Minion.minion, Renegade.color);
                    }
                    if (Renegade.fakeMinion != null) {
                        setPlayerNameColor(Renegade.fakeMinion, Renegade.color);
                    }
                }
                else if (BountyHunter.bountyhunter != null && BountyHunter.bountyhunter == PlayerControl.LocalPlayer)
                    setPlayerNameColor(BountyHunter.bountyhunter, BountyHunter.color);
                else if (Trapper.trapper != null && Trapper.trapper == PlayerControl.LocalPlayer)
                    setPlayerNameColor(Trapper.trapper, Trapper.color);
                else if (Yinyanger.yinyanger != null && Yinyanger.yinyanger == PlayerControl.LocalPlayer)
                    setPlayerNameColor(Yinyanger.yinyanger, Yinyanger.color);
                else if (Challenger.challenger != null && Challenger.challenger == PlayerControl.LocalPlayer)
                    setPlayerNameColor(Challenger.challenger, Challenger.color);
                else if (Ninja.ninja != null && Ninja.ninja == PlayerControl.LocalPlayer)
                    setPlayerNameColor(Ninja.ninja, Ninja.color);
                else if (Berserker.berserker != null && Berserker.berserker == PlayerControl.LocalPlayer)
                    setPlayerNameColor(Berserker.berserker, Berserker.color);

                else if (Modifiers.lover1 != null && Modifiers.lover2 != null && (Modifiers.lover1 == PlayerControl.LocalPlayer || Modifiers.lover2 == PlayerControl.LocalPlayer)) {
                    setPlayerNameColor(Modifiers.lover1, Modifiers.loverscolor);
                    setPlayerNameColor(Modifiers.lover2, Modifiers.loverscolor);
                }
                // No else if here, as a Lover of team Renegade needs the colors
                if (Minion.minion != null && Minion.minion == PlayerControl.LocalPlayer) {
                    // Minion can see the renegade
                    setPlayerNameColor(Minion.minion, Minion.color);
                    if (Renegade.renegade != null) {
                        setPlayerNameColor(Renegade.renegade, Renegade.color);
                    }
                }

                // Impostor roles with no color changes: Mimic, Painter, Demon, Janitor, Illusionist, Manipulator, Bomberman, Chameleon, Gambler, Sorcerer, Medusa, Hypnotist, Archer 
            }
        }

        static void setNameTags() {

            // Lovers add a heart to their names
            if (Modifiers.lover1 != null && Modifiers.lover2 != null && (Modifiers.lover1 == PlayerControl.LocalPlayer || Modifiers.lover2 == PlayerControl.LocalPlayer)) {
                string suffix = Helpers.cs(Modifiers.loverscolor, " ♥");
                Modifiers.lover1.cosmetics.nameText.text += suffix;
                Modifiers.lover2.cosmetics.nameText.text += suffix;

                if (MeetingHud.Instance != null)
                    foreach (PlayerVoteArea player in MeetingHud.Instance.playerStates)
                        if (Modifiers.lover1.PlayerId == player.TargetPlayerId || Modifiers.lover2.PlayerId == player.TargetPlayerId)
                            player.NameText.text += suffix;
            }

            // Forensic show color type on meeting
            if (PlayerControl.LocalPlayer == Forensic.forensic) {
                if (MeetingHud.Instance != null) {
                    foreach (PlayerVoteArea player in MeetingHud.Instance.playerStates) {
                        var target = Helpers.playerById(player.TargetPlayerId);
                        if (target != null) player.NameText.text += $" ({(Helpers.isLighterColor(target.Data.DefaultOutfit.ColorId) ? "L" : "D")})";
                    }
                }
            }

            // Battle Royale Lifes
            if (BattleRoyale.battleRoyaleMode && howmanygamemodesareon == 1) {
                if (BattleRoyale.matchType == 0) {
                    if (PlayerControl.LocalPlayer.Data.IsDead) {
                        BattleRoyale.soloPlayer01.cosmetics.nameText.text += Helpers.cs(Sheriff.color, " (" + BattleRoyale.soloPlayer01Lifes + "♥)");
                        BattleRoyale.soloPlayer02.cosmetics.nameText.text += Helpers.cs(Sheriff.color, " (" + BattleRoyale.soloPlayer02Lifes + "♥)");
                        BattleRoyale.soloPlayer03.cosmetics.nameText.text += Helpers.cs(Sheriff.color, " (" + BattleRoyale.soloPlayer03Lifes + "♥)");
                        BattleRoyale.soloPlayer04.cosmetics.nameText.text += Helpers.cs(Sheriff.color, " (" + BattleRoyale.soloPlayer04Lifes + "♥)");
                        BattleRoyale.soloPlayer05.cosmetics.nameText.text += Helpers.cs(Sheriff.color, " (" + BattleRoyale.soloPlayer05Lifes + "♥)");
                        BattleRoyale.soloPlayer06.cosmetics.nameText.text += Helpers.cs(Sheriff.color, " (" + BattleRoyale.soloPlayer06Lifes + "♥)");
                        BattleRoyale.soloPlayer07.cosmetics.nameText.text += Helpers.cs(Sheriff.color, " (" + BattleRoyale.soloPlayer07Lifes + "♥)");
                        BattleRoyale.soloPlayer08.cosmetics.nameText.text += Helpers.cs(Sheriff.color, " (" + BattleRoyale.soloPlayer08Lifes + "♥)");
                        BattleRoyale.soloPlayer09.cosmetics.nameText.text += Helpers.cs(Sheriff.color, " (" + BattleRoyale.soloPlayer09Lifes + "♥)");
                        BattleRoyale.soloPlayer10.cosmetics.nameText.text += Helpers.cs(Sheriff.color, " (" + BattleRoyale.soloPlayer10Lifes + "♥)");
                        BattleRoyale.soloPlayer11.cosmetics.nameText.text += Helpers.cs(Sheriff.color, " (" + BattleRoyale.soloPlayer11Lifes + "♥)");
                        BattleRoyale.soloPlayer12.cosmetics.nameText.text += Helpers.cs(Sheriff.color, " (" + BattleRoyale.soloPlayer12Lifes + "♥)");
                        BattleRoyale.soloPlayer13.cosmetics.nameText.text += Helpers.cs(Sheriff.color, " (" + BattleRoyale.soloPlayer13Lifes + "♥)");
                        BattleRoyale.soloPlayer14.cosmetics.nameText.text += Helpers.cs(Sheriff.color, " (" + BattleRoyale.soloPlayer14Lifes + "♥)");
                        BattleRoyale.soloPlayer15.cosmetics.nameText.text += Helpers.cs(Sheriff.color, " (" + BattleRoyale.soloPlayer15Lifes + "♥)");
                    }
                    else {
                        if (BattleRoyale.soloPlayer01 != null && PlayerControl.LocalPlayer == BattleRoyale.soloPlayer01) {
                            string suffix = Helpers.cs(Sheriff.color, " (" + BattleRoyale.soloPlayer01Lifes + "♥)");
                            BattleRoyale.soloPlayer01.cosmetics.nameText.text += suffix;
                        }
                        if (BattleRoyale.soloPlayer02 != null && PlayerControl.LocalPlayer == BattleRoyale.soloPlayer02) {
                            string suffix = Helpers.cs(Sheriff.color, " (" + BattleRoyale.soloPlayer02Lifes + "♥)");
                            BattleRoyale.soloPlayer02.cosmetics.nameText.text += suffix;
                        }
                        if (BattleRoyale.soloPlayer03 != null && PlayerControl.LocalPlayer == BattleRoyale.soloPlayer03) {
                            string suffix = Helpers.cs(Sheriff.color, " (" + BattleRoyale.soloPlayer03Lifes + "♥)");
                            BattleRoyale.soloPlayer03.cosmetics.nameText.text += suffix;
                        }
                        if (BattleRoyale.soloPlayer04 != null && PlayerControl.LocalPlayer == BattleRoyale.soloPlayer04) {
                            string suffix = Helpers.cs(Sheriff.color, " (" + BattleRoyale.soloPlayer04Lifes + "♥)");
                            BattleRoyale.soloPlayer04.cosmetics.nameText.text += suffix;
                        }
                        if (BattleRoyale.soloPlayer05 != null && PlayerControl.LocalPlayer == BattleRoyale.soloPlayer05) {
                            string suffix = Helpers.cs(Sheriff.color, " (" + BattleRoyale.soloPlayer05Lifes + "♥)");
                            BattleRoyale.soloPlayer05.cosmetics.nameText.text += suffix;
                        }
                        if (BattleRoyale.soloPlayer06 != null && PlayerControl.LocalPlayer == BattleRoyale.soloPlayer06) {
                            string suffix = Helpers.cs(Sheriff.color, " (" + BattleRoyale.soloPlayer06Lifes + "♥)");
                            BattleRoyale.soloPlayer06.cosmetics.nameText.text += suffix;
                        }
                        if (BattleRoyale.soloPlayer07 != null && PlayerControl.LocalPlayer == BattleRoyale.soloPlayer07) {
                            string suffix = Helpers.cs(Sheriff.color, " (" + BattleRoyale.soloPlayer07Lifes + "♥)");
                            BattleRoyale.soloPlayer07.cosmetics.nameText.text += suffix;
                        }
                        if (BattleRoyale.soloPlayer08 != null && PlayerControl.LocalPlayer == BattleRoyale.soloPlayer08) {
                            string suffix = Helpers.cs(Sheriff.color, " (" + BattleRoyale.soloPlayer08Lifes + "♥)");
                            BattleRoyale.soloPlayer08.cosmetics.nameText.text += suffix;
                        }
                        if (BattleRoyale.soloPlayer09 != null && PlayerControl.LocalPlayer == BattleRoyale.soloPlayer09) {
                            string suffix = Helpers.cs(Sheriff.color, " (" + BattleRoyale.soloPlayer09Lifes + "♥)");
                            BattleRoyale.soloPlayer09.cosmetics.nameText.text += suffix;
                        }
                        if (BattleRoyale.soloPlayer10 != null && PlayerControl.LocalPlayer == BattleRoyale.soloPlayer10) {
                            string suffix = Helpers.cs(Sheriff.color, " (" + BattleRoyale.soloPlayer10Lifes + "♥)");
                            BattleRoyale.soloPlayer10.cosmetics.nameText.text += suffix;
                        }
                        if (BattleRoyale.soloPlayer11 != null && PlayerControl.LocalPlayer == BattleRoyale.soloPlayer11) {
                            string suffix = Helpers.cs(Sheriff.color, " (" + BattleRoyale.soloPlayer11Lifes + "♥)");
                            BattleRoyale.soloPlayer11.cosmetics.nameText.text += suffix;
                        }
                        if (BattleRoyale.soloPlayer12 != null && PlayerControl.LocalPlayer == BattleRoyale.soloPlayer12) {
                            string suffix = Helpers.cs(Sheriff.color, " (" + BattleRoyale.soloPlayer12Lifes + "♥)");
                            BattleRoyale.soloPlayer12.cosmetics.nameText.text += suffix;
                        }
                        if (BattleRoyale.soloPlayer13 != null && PlayerControl.LocalPlayer == BattleRoyale.soloPlayer13) {
                            string suffix = Helpers.cs(Sheriff.color, " (" + BattleRoyale.soloPlayer13Lifes + "♥)");
                            BattleRoyale.soloPlayer13.cosmetics.nameText.text += suffix;
                        }
                        if (BattleRoyale.soloPlayer14 != null && PlayerControl.LocalPlayer == BattleRoyale.soloPlayer14) {
                            string suffix = Helpers.cs(Sheriff.color, " (" + BattleRoyale.soloPlayer14Lifes + "♥)");
                            BattleRoyale.soloPlayer14.cosmetics.nameText.text += suffix;
                        }
                        if (BattleRoyale.soloPlayer15 != null && PlayerControl.LocalPlayer == BattleRoyale.soloPlayer15) {
                            string suffix = Helpers.cs(Sheriff.color, " (" + BattleRoyale.soloPlayer15Lifes + "♥)");
                            BattleRoyale.soloPlayer15.cosmetics.nameText.text += suffix;
                        }
                    }
                }
                else {
                    foreach (PlayerControl limePlayer in BattleRoyale.limeTeam) {
                        if (limePlayer == PlayerControl.LocalPlayer) {
                            if (BattleRoyale.limePlayer01 != null) {
                                string suffix = Helpers.cs(FortuneTeller.color, " (" + BattleRoyale.limePlayer01Lifes + "♥)");
                                BattleRoyale.limePlayer01.cosmetics.nameText.text += suffix;
                            }
                            if (BattleRoyale.limePlayer02 != null) {
                                string suffix = Helpers.cs(FortuneTeller.color, " (" + BattleRoyale.limePlayer02Lifes + "♥)");
                                BattleRoyale.limePlayer02.cosmetics.nameText.text += suffix;
                            }
                            if (BattleRoyale.limePlayer03 != null) {
                                string suffix = Helpers.cs(FortuneTeller.color, " (" + BattleRoyale.limePlayer03Lifes + "♥)");
                                BattleRoyale.limePlayer03.cosmetics.nameText.text += suffix;
                            }
                            if (BattleRoyale.limePlayer04 != null) {
                                string suffix = Helpers.cs(FortuneTeller.color, " (" + BattleRoyale.limePlayer04Lifes + "♥)");
                                BattleRoyale.limePlayer04.cosmetics.nameText.text += suffix;
                            }
                            if (BattleRoyale.limePlayer05 != null) {
                                string suffix = Helpers.cs(FortuneTeller.color, " (" + BattleRoyale.limePlayer05Lifes + "♥)");
                                BattleRoyale.limePlayer05.cosmetics.nameText.text += suffix;
                            }
                            if (BattleRoyale.limePlayer06 != null) {
                                string suffix = Helpers.cs(FortuneTeller.color, " (" + BattleRoyale.limePlayer06Lifes + "♥)");
                                BattleRoyale.limePlayer06.cosmetics.nameText.text += suffix;
                            }
                            if (BattleRoyale.limePlayer07 != null) {
                                string suffix = Helpers.cs(FortuneTeller.color, " (" + BattleRoyale.limePlayer07Lifes + "♥)");
                                BattleRoyale.limePlayer07.cosmetics.nameText.text += suffix;
                            }
                        }
                    }
                    foreach (PlayerControl pinkPlayer in BattleRoyale.pinkTeam) {
                        if (pinkPlayer == PlayerControl.LocalPlayer) {
                            if (BattleRoyale.pinkPlayer01 != null) {
                                string suffix = Helpers.cs(Shy.color, " (" + BattleRoyale.pinkPlayer01Lifes + "♥)");
                                BattleRoyale.pinkPlayer01.cosmetics.nameText.text += suffix;
                            }
                            if (BattleRoyale.pinkPlayer02 != null) {
                                string suffix = Helpers.cs(Shy.color, " (" + BattleRoyale.pinkPlayer02Lifes + "♥)");
                                BattleRoyale.pinkPlayer02.cosmetics.nameText.text += suffix;
                            }
                            if (BattleRoyale.pinkPlayer03 != null) {
                                string suffix = Helpers.cs(Shy.color, " (" + BattleRoyale.pinkPlayer03Lifes + "♥)");
                                BattleRoyale.pinkPlayer03.cosmetics.nameText.text += suffix;
                            }
                            if (BattleRoyale.pinkPlayer04 != null) {
                                string suffix = Helpers.cs(Shy.color, " (" + BattleRoyale.pinkPlayer04Lifes + "♥)");
                                BattleRoyale.pinkPlayer04.cosmetics.nameText.text += suffix;
                            }
                            if (BattleRoyale.pinkPlayer05 != null) {
                                string suffix = Helpers.cs(Shy.color, " (" + BattleRoyale.pinkPlayer05Lifes + "♥)");
                                BattleRoyale.pinkPlayer05.cosmetics.nameText.text += suffix;
                            }
                            if (BattleRoyale.pinkPlayer06 != null) {
                                string suffix = Helpers.cs(Shy.color, " (" + BattleRoyale.pinkPlayer06Lifes + "♥)");
                                BattleRoyale.pinkPlayer06.cosmetics.nameText.text += suffix;
                            }
                            if (BattleRoyale.pinkPlayer07 != null) {
                                string suffix = Helpers.cs(Shy.color, " (" + BattleRoyale.pinkPlayer07Lifes + "♥)");
                                BattleRoyale.pinkPlayer07.cosmetics.nameText.text += suffix;
                            }
                        }
                    }
                    if (BattleRoyale.serialKiller != null && PlayerControl.LocalPlayer == BattleRoyale.serialKiller) {
                        string suffix = Helpers.cs(Joker.color, " (" + BattleRoyale.serialKillerLifes + "♥)");
                        BattleRoyale.serialKiller.cosmetics.nameText.text += suffix;
                    }
                }
            }
        }

        static void shakeScreenIfReactorSabotage() {
            if (shakeScreenReactor) {
                foreach (PlayerTask task in PlayerControl.LocalPlayer.myTasks) {
                    if (task.TaskType == TaskTypes.ResetReactor || task.TaskType == TaskTypes.ResetSeismic || task.TaskType == TaskTypes.StopCharles) {
                        HudManager.Instance.PlayerCam.shakeAmount = 0.025f;
                        HudManager.Instance.PlayerCam.shakePeriod = 400;
                    }
                }
            }
        }

        static void anonymousCommsSabotage() {
            if (anonymousComms) {
                foreach (PlayerTask task in PlayerControl.LocalPlayer.myTasks) {
                    if (task.TaskType == TaskTypes.FixComms) {
                        // Set grey painting while comms sabotage
                        foreach (PlayerControl player in PlayerControl.AllPlayerControls) {
                            player.setLook("", 6, "", "", "", "");
                        }
                        isHappeningAnonymousComms = true;
                    }
                }
            }
        }
        static void slowSpeedIfOxigenSabotage() {
            if (slowSpeedOxigen) {
                foreach (PlayerTask task in PlayerControl.LocalPlayer.myTasks) {
                    if (task.TaskType == TaskTypes.RestoreOxy) {
                        // Set slow speed while oxygen sabotage
                        NoOxyTask oxygenTask = UnityEngine.Object.FindObjectOfType<NoOxyTask>();
                        foreach (PlayerControl player in PlayerControl.AllPlayerControls) {
                            player.MyPhysics.Speed = Math.Max(1.5f,Math.Min(2.5f, 2.5f * oxygenTask.reactor.Countdown / oxygenTask.reactor.LifeSuppDuration));
                        }
                    }
                }
            }
        }

        static void updateShielded() {
            if (Squire.shielded == null) return;

            if (Squire.shielded.Data.IsDead || Squire.squire == null || Squire.squire.Data.IsDead) {
                Squire.shielded = null;
            }
        }

        static void fortuneTellerUpdate() {
            if (FortuneTeller.fortuneTeller == null || FortuneTeller.fortuneTeller != PlayerControl.LocalPlayer) return;

            // Update revealed players names if not in the duel
            if (!Challenger.isDueling) {
                foreach (PlayerControl p in FortuneTeller.revealedPlayers) {
                    // Update color and name regarding settings and given info
                    string result = p.Data.PlayerName;
                    RoleFortuneTellerInfo si = RoleFortuneTellerInfo.getFortuneTellerRoleInfoForPlayer(p);
                    if (FortuneTeller.kindOfInfo == 0)
                        si.color = si.isGood ? new Color(141f / 255f, 255f / 255f, 255f / 255f, 1) : new Color(255f / 255f, 0f / 255f, 0f / 255f, 1);
                    else if (FortuneTeller.kindOfInfo == 1) {
                        result = p.Data.PlayerName + " (" + si.name + ")";
                    }

                    // Set color and name
                    p.cosmetics.nameText.color = si.color;
                    p.cosmetics.nameText.text = result;
                    if (MeetingHud.Instance != null) {
                        foreach (PlayerVoteArea player in MeetingHud.Instance.playerStates) {
                            if (p.PlayerId == player.TargetPlayerId) {
                                player.NameText.text = result;
                                player.NameText.color = si.color;
                                break;
                            }
                        }
                    }
                }
            }
        }

        static void timerUpdate() {
            var deltaTime = Time.deltaTime;
            if (Hacker.hacker != null) {
                Hacker.hackerTimer -= deltaTime;
            }
            if (Bomberman.bomberman != null) {
                Bomberman.bombTimer -= deltaTime;
            }
            if (Modifiers.performer != null) {
                Modifiers.performerDuration -= deltaTime;
            }
            if (Illusionist.illusionist != null) {
                Illusionist.lightsOutTimer -= deltaTime;
            }
            if (Sleuth.sleuth != null) {
                Sleuth.corpsesPathfindTimer -= deltaTime;
            }
            if (Detective.detective != null) {
                Detective.detectiveTimer -= deltaTime;
            }
            if (Fink.fink != null) {
                Fink.finkTimer -= deltaTime;
            }
            if (Medusa.medusa != null) {
                Medusa.messageTimer -= deltaTime;
            }
            if (Hypnotist.hypnotist != null) {
                Hypnotist.messageTimer -= deltaTime;
            }
            if (Engineer.engineer != null) {
                Engineer.messageTimer -= deltaTime;
            }
            if (Bat.bat != null) {
                Bat.frequencyTimer -= deltaTime;
            }
            if (Berserker.berserker != null && Berserker.killedFirstTime && MeetingHud.Instance == null && !Berserker.berserker.Data.IsDead) {
                Berserker.timeToKill -= deltaTime;
                Berserker.berserkerCountButtonText.text = $"{Berserker.timeToKill.ToString("F0")}";
                if (Berserker.timeToKill < 0) {
                    Berserker.berserker.MurderPlayer(Berserker.berserker);
                }
            }
            if (Shy.shy != null) {
                Shy.timer -= deltaTime;
            }

            if (howmanygamemodesareon == 1) {
                // Capture the flag timer
                if (CaptureTheFlag.captureTheFlagMode) {
                    CaptureTheFlag.matchDuration -= deltaTime;
                    if (CaptureTheFlag.matchDuration < 0) {
                        // both teams with same points = Draw
                        if (CaptureTheFlag.currentRedTeamPoints == CaptureTheFlag.currentBlueTeamPoints) {
                            CaptureTheFlag.triggerDrawWin = true;
                            ShipStatus.RpcEndGame((GameOverReason)CustomGameOverReason.DrawTeamWin, false);
                        }
                        // Red team more points than blue team = red team win
                        else if (CaptureTheFlag.currentRedTeamPoints > CaptureTheFlag.currentBlueTeamPoints) {
                            CaptureTheFlag.triggerRedTeamWin = true;
                            ShipStatus.RpcEndGame((GameOverReason)CustomGameOverReason.RedTeamFlagWin, false);
                        }
                        // otherwise blue team win
                        else {
                            CaptureTheFlag.triggerBlueTeamWin = true;
                            ShipStatus.RpcEndGame((GameOverReason)CustomGameOverReason.BlueTeamFlagWin, false);
                        }
                    }
                }

                // Police and Thief timer, always police team wins if thiefs ran out of time
                if (PoliceAndThief.policeAndThiefMode) {
                    PoliceAndThief.policeplayer01lightTimer -= deltaTime;
                    PoliceAndThief.policeplayer02lightTimer -= deltaTime;
                    PoliceAndThief.policeplayer03lightTimer -= deltaTime;
                    PoliceAndThief.policeplayer04lightTimer -= deltaTime;
                    PoliceAndThief.policeplayer05lightTimer -= deltaTime;
                    PoliceAndThief.policeplayer06lightTimer -= deltaTime;

                    PoliceAndThief.matchDuration -= deltaTime;
                    if (PoliceAndThief.matchDuration < 0) {
                        PoliceAndThief.triggerPoliceWin = true;
                        ShipStatus.RpcEndGame((GameOverReason)CustomGameOverReason.ThiefModePoliceWin, false);
                    }
                }

                // King of the hill timer
                if (KingOfTheHill.kingOfTheHillMode) {
                    KingOfTheHill.matchDuration -= deltaTime;
                    if (KingOfTheHill.matchDuration < 0) {
                        // both teams with same points = draw
                        if (KingOfTheHill.currentGreenTeamPoints == KingOfTheHill.currentYellowTeamPoints) {
                            KingOfTheHill.triggerDrawWin = true;
                            ShipStatus.RpcEndGame((GameOverReason)CustomGameOverReason.TeamHillDraw, false);
                        }
                        // green team more points than yellow team = green team win
                        else if (KingOfTheHill.currentGreenTeamPoints > KingOfTheHill.currentYellowTeamPoints) {
                            KingOfTheHill.triggerGreenTeamWin = true;
                            ShipStatus.RpcEndGame((GameOverReason)CustomGameOverReason.GreenTeamHillWin, false);
                        }
                        // otherwise yellow team win
                        else {
                            KingOfTheHill.triggerYellowTeamWin = true;
                            ShipStatus.RpcEndGame((GameOverReason)CustomGameOverReason.YellowTeamHillWin, false);
                        }
                    }

                    if (KingOfTheHill.totalGreenKingzonescaptured != 0) {
                        KingOfTheHill.currentGreenTeamPoints += KingOfTheHill.totalGreenKingzonescaptured * deltaTime; ;
                        if (KingOfTheHill.currentGreenTeamPoints >= KingOfTheHill.requiredPoints) {
                            KingOfTheHill.triggerGreenTeamWin = true;
                            ShipStatus.RpcEndGame((GameOverReason)CustomGameOverReason.GreenTeamHillWin, false);
                        }
                    }
                    if (KingOfTheHill.totalYellowKingzonescaptured != 0) {
                        KingOfTheHill.currentYellowTeamPoints += KingOfTheHill.totalYellowKingzonescaptured * deltaTime; ;
                        if (KingOfTheHill.currentYellowTeamPoints >= KingOfTheHill.requiredPoints) {
                            KingOfTheHill.triggerYellowTeamWin = true;
                            ShipStatus.RpcEndGame((GameOverReason)CustomGameOverReason.YellowTeamHillWin, false);
                        }
                    }

                    KingOfTheHill.kingpointCounter = "Score: " + "<color=#00FF00FF>" + KingOfTheHill.currentGreenTeamPoints.ToString("F0") + "</color> - " + "<color=#FFFF00FF>" + KingOfTheHill.currentYellowTeamPoints.ToString("F0") + "</color>";

                }

                // Hot Potato timer
                if (HotPotato.hotPotatoMode) {
                    if (HotPotato.firstPotatoTransfered) {
                        HotPotato.timeforTransfer -= deltaTime;

                        if (HotPotato.timeforTransfer <= 0 && !HotPotato.hotPotatoPlayer.Data.IsDead) {
                            HotPotato.hotPotatoPlayer.MurderPlayer(HotPotato.hotPotatoPlayer);
                        }

                        HotPotato.matchDuration -= deltaTime;

                        if (HotPotato.matchDuration < 0) {
                            HotPotato.triggerHotPotatoEnd = true;
                            ShipStatus.RpcEndGame((GameOverReason)CustomGameOverReason.HotPotatoEnd, false);
                        }
                    }
                }

                // ZombieLaboratory timer
                if (ZombieLaboratory.zombieLaboratoryMode) {
                    ZombieLaboratory.matchDuration -= deltaTime;

                    if (ZombieLaboratory.matchDuration < 0) {
                        ZombieLaboratory.triggerZombieWin = true;
                        ShipStatus.RpcEndGame((GameOverReason)CustomGameOverReason.ZombieWin, false);
                    }
                }
                
                // Battle Royale timer
                if (BattleRoyale.battleRoyaleMode) {
                    BattleRoyale.matchDuration -= deltaTime;
                    if (BattleRoyale.matchDuration < 0) {
                        if (BattleRoyale.matchType == 2) {
                            if (BattleRoyale.serialKiller != null) {
                                // all teams with same points = Draw
                                if (BattleRoyale.limePoints == BattleRoyale.pinkPoints && BattleRoyale.pinkPoints == BattleRoyale.serialKillerPoints) {
                                    BattleRoyale.triggerDrawWin = true;
                                    ShipStatus.RpcEndGame((GameOverReason)CustomGameOverReason.BattleRoyaleDraw, false);
                                }
                                // Lime team more points than pink team and serial killer = lime team win
                                else if (BattleRoyale.limePoints > BattleRoyale.pinkPoints && BattleRoyale.limePoints > BattleRoyale.serialKillerPoints) {
                                    BattleRoyale.triggerLimeTeamWin = true;
                                    ShipStatus.RpcEndGame((GameOverReason)CustomGameOverReason.BattleRoyaleLimeTeamWin, false);
                                }
                                // otherwise pink team win
                                else if (BattleRoyale.pinkPoints > BattleRoyale.limePoints && BattleRoyale.pinkPoints > BattleRoyale.serialKillerPoints) {
                                    BattleRoyale.triggerPinkTeamWin = true;
                                    ShipStatus.RpcEndGame((GameOverReason)CustomGameOverReason.BattleRoyalePinkTeamWin, false);
                                }
                                // otherwise serial killer win
                                else if (BattleRoyale.serialKillerPoints > BattleRoyale.limePoints && BattleRoyale.serialKillerPoints > BattleRoyale.pinkPoints) {
                                    BattleRoyale.triggerSerialKillerWin = true;
                                    ShipStatus.RpcEndGame((GameOverReason)CustomGameOverReason.BattleRoyaleSerialKillerWin, false);
                                } 
                                // draw between some of the teams
                                else {
                                    BattleRoyale.triggerDrawWin = true;
                                    ShipStatus.RpcEndGame((GameOverReason)CustomGameOverReason.BattleRoyaleDraw, false);
                                }
                            }
                            else {
                                // both teams with same points = Draw
                                if (BattleRoyale.limePoints == BattleRoyale.pinkPoints) {
                                    BattleRoyale.triggerDrawWin = true;
                                    ShipStatus.RpcEndGame((GameOverReason)CustomGameOverReason.BattleRoyaleDraw, false);
                                }
                                // Lime team more points than pink team = lime team win
                                else if (BattleRoyale.limePoints > BattleRoyale.pinkPoints) {
                                    BattleRoyale.triggerLimeTeamWin = true;
                                    ShipStatus.RpcEndGame((GameOverReason)CustomGameOverReason.BattleRoyaleLimeTeamWin, false);
                                }
                                // otherwise pink team win
                                else {
                                    BattleRoyale.triggerPinkTeamWin = true;
                                    ShipStatus.RpcEndGame((GameOverReason)CustomGameOverReason.BattleRoyalePinkTeamWin, false);
                                }
                            }
                        }
                        else {
                            BattleRoyale.triggerTimeWin = true;
                            ShipStatus.RpcEndGame((GameOverReason)CustomGameOverReason.BattleRoyaleTimeWin, false);
                        }
                    }
                }
            }
        }

        public static void kidUpdate() {
            foreach (PlayerControl p in PlayerControl.AllPlayerControls) {
                if (p == null) continue;

                if (Kid.kid != null && Kid.kid == p)
                    p.transform.localScale = new Vector3(0.45f, 0.45f, 1f);
                else if (Mimic.mimic != null && Mimic.mimic == p && Mimic.transformTarget != null && Mimic.transformTarget == Kid.kid && Mimic.transformTimer > 0f)
                    p.transform.localScale = new Vector3(0.45f, 0.45f, 1f);
                else if (Puppeteer.puppeteer != null && Puppeteer.puppeteer == p && Puppeteer.transformTarget != null && Puppeteer.transformTarget == Kid.kid && Puppeteer.morphed)
                    p.transform.localScale = new Vector3(0.45f, 0.45f, 1f);
                // big chungus update, restore original scale on duel and painting to be more fair
                else if (Modifiers.bigchungus != null && Modifiers.bigchungus == p && !Challenger.isDueling && Painter.painterTimer <= 0 && !isHappeningAnonymousComms) {
                    if (Mimic.mimic != null && Mimic.transformTimer > 0f && Mimic.mimic.PlayerId == Modifiers.bigchungus.PlayerId) {
                        p.transform.localScale = new Vector3(0.7f, 0.7f, 1f);
                    }
                    else if (Puppeteer.puppeteer != null && Puppeteer.morphed && Puppeteer.puppeteer.PlayerId == Modifiers.bigchungus.PlayerId) {
                        p.transform.localScale = new Vector3(0.7f, 0.7f, 1f);
                    }
                    else {
                        p.transform.localScale = new Vector3(0.9f, 0.9f, 1f);
                    }
                }
                // Mimic and Puppeteer big chungus update
                else if (Mimic.mimic != null && Mimic.mimic == p && Mimic.transformTarget != null && Mimic.transformTarget == Modifiers.bigchungus && Mimic.transformTimer > 0f && !isHappeningAnonymousComms)
                    p.transform.localScale = new Vector3(0.9f, 0.9f, 1f);
                else if (Puppeteer.puppeteer != null && Puppeteer.puppeteer == p && Puppeteer.transformTarget != null && Puppeteer.transformTarget == Modifiers.bigchungus && Puppeteer.morphed && !isHappeningAnonymousComms)
                    p.transform.localScale = new Vector3(0.9f, 0.9f, 1f);
                else
                    p.transform.localScale = new Vector3(0.7f, 0.7f, 1f);
            }
        }

        static void updateImpostorKillButton(HudManager __instance) {
            if (!PlayerControl.LocalPlayer.Data.Role.IsImpostor || MeetingHud.Instance || MapBehaviour.Instance != null && MapBehaviour.Instance.IsOpen) return;
            bool enabled = true;
            if (Demon.demon != null && Demon.demon == PlayerControl.LocalPlayer && !Challenger.isDueling)
                enabled = false;
            else if (Janitor.janitor != null && Janitor.dragginBody && PlayerControl.LocalPlayer == Janitor.janitor)
                enabled = false;
            else if (Archer.archer != null && PlayerControl.LocalPlayer == Archer.archer && !Challenger.isDueling)
                enabled = false;
            else if (Challenger.isDueling)
                enabled = false;
            else if (howmanygamemodesareon == 1)
                enabled = false;
            if (enabled) __instance.KillButton.Show();
            else __instance.KillButton.Hide();
        }

        static void spiritualistUpdate() {

            if (PlayerControl.LocalPlayer == Spiritualist.spiritualist && Spiritualist.spiritualist != null) {
                foreach (var player in PlayerControl.AllPlayerControls) {
                    if (player.Data.IsDead) {
                        player.cosmetics.currentBodySprite.BodySprite.gameObject.SetActive(true);
                        player.cosmetics.currentBodySprite.BodySprite.enabled = true;
                        player.cosmetics.nameText.enabled = true;
                        player.cosmetics.nameText.gameObject.SetActive(true);
                    }
                }
            }

            // Identify Spiritualist by name color if you're dead
            foreach (PlayerControl p in PlayerControl.AllPlayerControls) {
                if (Spiritualist.spiritualist != null && !Spiritualist.spiritualist.Data.IsDead && p == PlayerControl.LocalPlayer && p.Data.IsDead) {
                    Spiritualist.spiritualist.cosmetics.nameText.color = Spiritualist.color;
                }
            }
        }

        static void chameleonUpdate() {

            if (Chameleon.chameleon == null) return;

            Chameleon.chameleonTimer -= Time.deltaTime;

            if (Chameleon.chameleonTimer > 0f) {
                if (Chameleon.chameleon == PlayerControl.LocalPlayer) {
                    Chameleon.chameleon.cosmetics.nameText.color = new Color(Chameleon.chameleon.cosmetics.nameText.color.r, Chameleon.chameleon.cosmetics.nameText.color.g, Chameleon.chameleon.cosmetics.nameText.color.b, 0.5f);
                    Chameleon.chameleon.cosmetics.colorBlindText.color = new Color(Chameleon.chameleon.cosmetics.colorBlindText.color.r, Chameleon.chameleon.cosmetics.colorBlindText.color.g, Chameleon.chameleon.cosmetics.colorBlindText.color.b, 0.5f);
                    if (Chameleon.chameleon.cosmetics.currentPet != null && Chameleon.chameleon.cosmetics.currentPet.rend != null && Chameleon.chameleon.cosmetics.currentPet.shadowRend != null) {
                        Chameleon.chameleon.cosmetics.currentPet.rend.color = new Color(Chameleon.chameleon.cosmetics.currentPet.rend.color.r, Chameleon.chameleon.cosmetics.currentPet.rend.color.g, Chameleon.chameleon.cosmetics.currentPet.rend.color.b, 0.5f);
                        Chameleon.chameleon.cosmetics.currentPet.shadowRend.color = new Color(Chameleon.chameleon.cosmetics.currentPet.shadowRend.color.r, Chameleon.chameleon.cosmetics.currentPet.shadowRend.color.g, Chameleon.chameleon.cosmetics.currentPet.shadowRend.color.b, 0.5f);
                    }
                    if (Chameleon.chameleon.cosmetics.hat != null) {
                        Chameleon.chameleon.cosmetics.hat.Parent.color = new Color(Chameleon.chameleon.cosmetics.hat.Parent.color.r, Chameleon.chameleon.cosmetics.hat.Parent.color.g, Chameleon.chameleon.cosmetics.hat.Parent.color.b, 0.5f);
                        Chameleon.chameleon.cosmetics.hat.BackLayer.color = new Color(Chameleon.chameleon.cosmetics.hat.BackLayer.color.r, Chameleon.chameleon.cosmetics.hat.BackLayer.color.g, Chameleon.chameleon.cosmetics.hat.BackLayer.color.b, 0.5f);
                        Chameleon.chameleon.cosmetics.hat.FrontLayer.color = new Color(Chameleon.chameleon.cosmetics.hat.FrontLayer.color.r, Chameleon.chameleon.cosmetics.hat.FrontLayer.color.g, Chameleon.chameleon.cosmetics.hat.FrontLayer.color.b, 0.5f);
                    }
                    if (Chameleon.chameleon.cosmetics.visor != null) {
                        Chameleon.chameleon.cosmetics.visor.Image.color = new Color(Chameleon.chameleon.cosmetics.visor.Image.color.r, Chameleon.chameleon.cosmetics.visor.Image.color.g, Chameleon.chameleon.cosmetics.visor.Image.color.b, 0.5f);
                    }
                    Chameleon.chameleon.MyPhysics.myPlayer.cosmetics.skin.layer.color = new Color(Chameleon.chameleon.MyPhysics.myPlayer.cosmetics.skin.layer.color.r, Chameleon.chameleon.MyPhysics.myPlayer.cosmetics.skin.layer.color.g, Chameleon.chameleon.MyPhysics.myPlayer.cosmetics.skin.layer.color.b, 0.5f);
                }
                else {
                    Chameleon.chameleon.cosmetics.nameText.color = new Color(Chameleon.chameleon.cosmetics.nameText.color.r, Chameleon.chameleon.cosmetics.nameText.color.g, Chameleon.chameleon.cosmetics.nameText.color.b, 0f);
                    Chameleon.chameleon.cosmetics.colorBlindText.color = new Color(Chameleon.chameleon.cosmetics.colorBlindText.color.r, Chameleon.chameleon.cosmetics.colorBlindText.color.g, Chameleon.chameleon.cosmetics.colorBlindText.color.b, 0f);
                    if (Chameleon.chameleon.cosmetics.currentPet != null && Chameleon.chameleon.cosmetics.currentPet.rend != null && Chameleon.chameleon.cosmetics.currentPet.shadowRend != null) {
                        Chameleon.chameleon.cosmetics.currentPet.rend.color = new Color(Chameleon.chameleon.cosmetics.currentPet.rend.color.r, Chameleon.chameleon.cosmetics.currentPet.rend.color.g, Chameleon.chameleon.cosmetics.currentPet.rend.color.b, 0f);
                        Chameleon.chameleon.cosmetics.currentPet.shadowRend.color = new Color(Chameleon.chameleon.cosmetics.currentPet.shadowRend.color.r, Chameleon.chameleon.cosmetics.currentPet.shadowRend.color.g, Chameleon.chameleon.cosmetics.currentPet.shadowRend.color.b, 0f);
                    }
                    if (Chameleon.chameleon.cosmetics.hat != null) {
                        Chameleon.chameleon.cosmetics.hat.Parent.color = new Color(Chameleon.chameleon.cosmetics.hat.Parent.color.r, Chameleon.chameleon.cosmetics.hat.Parent.color.g, Chameleon.chameleon.cosmetics.hat.Parent.color.b, 0f);
                        Chameleon.chameleon.cosmetics.hat.BackLayer.color = new Color(Chameleon.chameleon.cosmetics.hat.BackLayer.color.r, Chameleon.chameleon.cosmetics.hat.BackLayer.color.g, Chameleon.chameleon.cosmetics.hat.BackLayer.color.b, 0f);
                        Chameleon.chameleon.cosmetics.hat.FrontLayer.color = new Color(Chameleon.chameleon.cosmetics.hat.FrontLayer.color.r, Chameleon.chameleon.cosmetics.hat.FrontLayer.color.g, Chameleon.chameleon.cosmetics.hat.FrontLayer.color.b, 0f);
                    }
                    if (Chameleon.chameleon.cosmetics.visor != null) {
                        Chameleon.chameleon.cosmetics.visor.Image.color = new Color(Chameleon.chameleon.cosmetics.visor.Image.color.r, Chameleon.chameleon.cosmetics.visor.Image.color.g, Chameleon.chameleon.cosmetics.visor.Image.color.b, 0f);
                    }
                    Chameleon.chameleon.MyPhysics.myPlayer.cosmetics.skin.layer.color = new Color(Chameleon.chameleon.MyPhysics.myPlayer.cosmetics.skin.layer.color.r, Chameleon.chameleon.MyPhysics.myPlayer.cosmetics.skin.layer.color.g, Chameleon.chameleon.MyPhysics.myPlayer.cosmetics.skin.layer.color.b, 0f);
                }
            }

            // Chameleon reset
            if (Chameleon.chameleonTimer <= 0f) {
                Chameleon.resetChameleon();
            }
        }
        static void bountyHunterSuicideIfDisconnect() {
            if (BountyHunter.bountyhunter == null) return;

            if (BountyHunter.usedTarget && BountyHunter.hasToKill.Data.Disconnected && BountyHunter.bountyhunter == PlayerControl.LocalPlayer && !BountyHunter.bountyhunter.Data.IsDead) {
                BountyHunter.bountyhunter.MurderPlayer(BountyHunter.bountyhunter);
            }
        }
        static void yinyangerUpdate() {

            if (Yinyanger.yinyanger == null || Yinyanger.yinyanger.Data.IsDead) {
                return;
            }

            if (Yinyanger.yinyedplayer != null && (Yinyanger.yinyedplayer.Data.Disconnected || Yinyanger.yinyedplayer.Data.IsDead)) {
                // If the yined victim is disconnected or dead reset the yined use so a new target can be selected
                Yinyanger.resetYined();
            }
            if (Yinyanger.yangyedplayer != null && (Yinyanger.yangyedplayer.Data.Disconnected || Yinyanger.yangyedplayer.Data.IsDead)) {
                // If the yanged victim is disconnected or dead reset the yanged use so a new target can be selectet
                Yinyanger.resetYanged();
            }

            if (Yinyanger.yinyedplayer != null && Yinyanger.yangyedplayer != null && !Yinyanger.colision) {
                if (Vector2.Distance(Yinyanger.yinyedplayer.transform.position, Yinyanger.yangyedplayer.transform.position) < 0.5f) {
                    yinYang();
                }
            }
        }

        public static void yinYang() {
            new YinYang(1, Yinyanger.yinyedplayer);
            new YinYang(1, Yinyanger.yangyedplayer);
            Yinyanger.colision = true;
            HudManager.Instance.StartCoroutine(Effects.Lerp(1, new Action<float>((p) => {
                if (Yinyanger.yinyanger == PlayerControl.LocalPlayer || Yinyanger.yinyedplayer == PlayerControl.LocalPlayer || Yinyanger.yangyedplayer == PlayerControl.LocalPlayer) {
                    SoundManager.Instance.PlaySound(CustomMain.customAssets.yinyangerYinyangColisionClip, false, 100f);
                }
                Yinyanger.yinyedplayer.moveable = false;
                Yinyanger.yinyedplayer.NetTransform.Halt();
                Yinyanger.yangyedplayer.moveable = false;
                Yinyanger.yangyedplayer.NetTransform.Halt();

                Yinyanger.yinedPlayer = Yinyanger.yinyedplayer;
                Yinyanger.yanedPlayer = Yinyanger.yangyedplayer;
                if (p == 1f) {

                    RPCProcedure.uncheckedMurderPlayer(Yinyanger.yinyanger.PlayerId, Yinyanger.yinedPlayer.PlayerId, 0);

                    RPCProcedure.uncheckedMurderPlayer(Yinyanger.yinyanger.PlayerId, Yinyanger.yanedPlayer.PlayerId, 0);

                    Yinyanger.yinyedplayer.moveable = true;
                    Yinyanger.yangyedplayer.moveable = true;

                    Yinyanger.resetYined();
                    Yinyanger.resetYanged();
                    Yinyanger.colision = false;
                }
            })));
            return;
        }
        
        static void challengerUpdate() {

            if (Challenger.challenger == null || !Challenger.isDueling) {
                return;
            }

            // Set grey painting while dueling
            foreach (PlayerControl player in PlayerControl.AllPlayerControls) {
                player.setLook("", 6, "", "", "", "");
            }

            // 30 sec duel duration
            Challenger.duelDuration -= Time.deltaTime;
            if (Challenger.duelDuration < 0 && Challenger.onlyOneFinishDuel && !Challenger.timeOutDuel) {
                Challenger.onlyOneFinishDuel = false;
                Challenger.timeOutDuel = true;
                challengerFinishDuel(1);
            }

            while ((!Challenger.challengerRock && !Challenger.challengerPaper && !Challenger.challengerScissors) || (!Challenger.rivalRock && !Challenger.rivalPaper && !Challenger.rivalScissors))
                return;

            if (Challenger.onlyOneFinishDuel && !Challenger.timeOutDuel) {
                Challenger.onlyOneFinishDuel = false;
                challengerFinishDuel(0);
            }

        }

        public static void challengerFinishDuel(byte duelflag) {

            if (Challenger.challengerRock) {
                new RockPaperScissors(3, Challenger.challenger, 1);
            }
            else if (Challenger.challengerPaper) {
                new RockPaperScissors(3, Challenger.challenger, 2);
            }
            else if (Challenger.challengerScissors) {
                new RockPaperScissors(3, Challenger.challenger, 3);
            }

            if (Challenger.rivalRock) {
                new RockPaperScissors(3, Challenger.rivalPlayer, 1);
            }
            else if (Challenger.rivalPaper) {
                new RockPaperScissors(3, Challenger.rivalPlayer, 2);
            }
            else if (Challenger.rivalScissors) {
                new RockPaperScissors(3, Challenger.rivalPlayer, 3);
            }

            if (duelflag == 0) {
                HudManager.Instance.StartCoroutine(Effects.Lerp(3, new Action<float>((p) => {

                    if (p == 1f) {

                        bool challengerOrRival = true;

                        if (Challenger.challengerRock && Challenger.rivalPaper) {
                            Challenger.rivalPlayer.MurderPlayer(Challenger.challenger);
                            SoundManager.Instance.PlaySound(CustomMain.customAssets.challengerDuelKillClip, false, 5f);
                        }
                        else if (Challenger.challengerRock && Challenger.rivalScissors) {
                            Challenger.challenger.MurderPlayer(Challenger.rivalPlayer);
                            SoundManager.Instance.PlaySound(CustomMain.customAssets.challengerDuelKillClip, false, 5f);
                            challengerOrRival = false;
                        }
                        else if (Challenger.challengerPaper && Challenger.rivalRock) {
                            Challenger.challenger.MurderPlayer(Challenger.rivalPlayer);
                            SoundManager.Instance.PlaySound(CustomMain.customAssets.challengerDuelKillClip, false, 5f);
                            challengerOrRival = false;
                        }
                        else if (Challenger.challengerPaper && Challenger.rivalScissors) {
                            Challenger.rivalPlayer.MurderPlayer(Challenger.challenger);
                            SoundManager.Instance.PlaySound(CustomMain.customAssets.challengerDuelKillClip, false, 5f);
                        }
                        else if (Challenger.challengerScissors && Challenger.rivalPaper) {
                            Challenger.challenger.MurderPlayer(Challenger.rivalPlayer);
                            SoundManager.Instance.PlaySound(CustomMain.customAssets.challengerDuelKillClip, false, 5f);
                            challengerOrRival = false;
                        }
                        else if (Challenger.challengerScissors && Challenger.rivalRock) {
                            Challenger.rivalPlayer.MurderPlayer(Challenger.challenger);
                            SoundManager.Instance.PlaySound(CustomMain.customAssets.challengerDuelKillClip, false, 5f);
                        }

                        if (challengerOrRival) {
                            var body = UnityEngine.Object.FindObjectsOfType<DeadBody>().FirstOrDefault(b => b.ParentId == Challenger.challenger.PlayerId);
                            body.transform.position = new Vector3(75f, 0f, -5);
                        }
                        else {
                            var body = UnityEngine.Object.FindObjectsOfType<DeadBody>().FirstOrDefault(b => b.ParentId == Challenger.rivalPlayer.PlayerId);
                            body.transform.position = new Vector3(75f, 0f, -5);
                        }
                    }
                })));
            }
            else {
                HudManager.Instance.StartCoroutine(Effects.Lerp(3, new Action<float>((p) => {

                    if (p == 1f) {

                        int whoDied = 0;

                        if ((Challenger.challengerRock || Challenger.challengerPaper || Challenger.challengerScissors) && (!Challenger.rivalRock && !Challenger.rivalPaper && !Challenger.rivalScissors)) {
                            Challenger.challenger.MurderPlayer(Challenger.rivalPlayer);
                            SoundManager.Instance.PlaySound(CustomMain.customAssets.challengerDuelKillClip, false, 5f);
                            whoDied = 2;
                        }
                        else if ((!Challenger.challengerRock && !Challenger.challengerPaper && !Challenger.challengerScissors) && (Challenger.rivalRock || Challenger.rivalPaper || Challenger.rivalScissors)) {
                            Challenger.rivalPlayer.MurderPlayer(Challenger.challenger);
                            SoundManager.Instance.PlaySound(CustomMain.customAssets.challengerDuelKillClip, false, 5f);
                            whoDied = 1;
                        }
                        else if ((!Challenger.challengerRock && !Challenger.challengerPaper && !Challenger.challengerScissors) && (!Challenger.rivalRock || !Challenger.rivalPaper || !Challenger.rivalScissors)) {
                            Challenger.challenger.MurderPlayer(Challenger.rivalPlayer);
                            Challenger.rivalPlayer.MurderPlayer(Challenger.challenger);
                            SoundManager.Instance.PlaySound(CustomMain.customAssets.challengerDuelKillClip, false, 5f);
                            whoDied = 3;
                        }

                        switch (whoDied) {
                            case 1:
                                var body = UnityEngine.Object.FindObjectsOfType<DeadBody>().FirstOrDefault(b => b.ParentId == Challenger.challenger.PlayerId);
                                body.transform.position = new Vector3(75f, 0f, -5);
                                break;
                            case 2:
                                var bodytwo = UnityEngine.Object.FindObjectsOfType<DeadBody>().FirstOrDefault(b => b.ParentId == Challenger.rivalPlayer.PlayerId);
                                bodytwo.transform.position = new Vector3(75f, 0f, -5);
                                break;
                            case 3:
                                var bodythree = UnityEngine.Object.FindObjectsOfType<DeadBody>().FirstOrDefault(b => b.ParentId == Challenger.rivalPlayer.PlayerId);
                                bodythree.transform.position = new Vector3(75f, 0f, -5);
                                var bodyfour = UnityEngine.Object.FindObjectsOfType<DeadBody>().FirstOrDefault(b => b.ParentId == Challenger.challenger.PlayerId);
                                bodyfour.transform.position = new Vector3(75f, 0f, -5); 
                                break;
                        }
                    }
                })));
            }

            HudManager.Instance.StartCoroutine(Effects.Lerp(6, new Action<float>((p) => {

                if (p == 1f) {
                    // Undo the character transform
                    foreach (PlayerControl player in PlayerControl.AllPlayerControls) {
                        if (player == PlayerControl.LocalPlayer) {
                            player.transform.position = positionBeforeDuel;
                        }
                    }
                    RPCProcedure.changeMusic(2);
                    Challenger.timeOutDuel = false;
                }
            })));

            HudManager.Instance.StartCoroutine(Effects.Lerp(7, new Action<float>((p) => {

                if (p == 1f) {

                    GameObject emerButton = null; 

                    switch (PlayerControl.GameOptions.MapId) {
                        case 0:
                            emerButton = GameObject.Find("EmergencyConsole");
                            break;
                        case 1:
                            emerButton = GameObject.Find("EmergencyConsole");
                            break;
                        case 2:
                            emerButton = GameObject.Find("EmergencyButton");
                            break;
                        case 3:
                            emerButton = GameObject.Find("EmergencyConsole");
                            break;
                        case 4:
                            emerButton = GameObject.Find("task_emergency");
                            break;
                        case 5:
                            emerButton = GameObject.Find("console-mr-callmeeting");
                            break;
                    }

                    // If after the duel both are dead, teleport their body to the player location
                    if (Challenger.challenger.Data.IsDead && Challenger.rivalPlayer.Data.IsDead) {
                        var bodyChallenger = UnityEngine.Object.FindObjectsOfType<DeadBody>().FirstOrDefault(b => b.ParentId == Challenger.challenger.PlayerId);
                        if (PlayerControl.GameOptions.MapId == 5) {
                            if (bodyChallenger.transform.position.y > 0) {
                                bodyChallenger.transform.position = new Vector3(5f, 19.5f, -5);
                            }
                            else {
                                bodyChallenger.transform.position = new Vector3(1.35f, -28.25f, -5);
                            }
                        }
                        else {
                            bodyChallenger.transform.position = emerButton.transform.position + new Vector3(0.5f, 0f, -0.5f);
                        }
                        var bodyRival = UnityEngine.Object.FindObjectsOfType<DeadBody>().FirstOrDefault(b => b.ParentId == Challenger.rivalPlayer.PlayerId);
                        if (PlayerControl.GameOptions.MapId == 5) {
                            if (bodyRival.transform.position.y > 0) {
                                bodyRival.transform.position = new Vector3(5f, 19.5f, -5);
                            }
                            else {
                                bodyRival.transform.position = new Vector3(1.35f, -28.25f, -5);
                            }
                        }
                        else {
                            bodyRival.transform.position = emerButton.transform.position + new Vector3(0.5f, 0f, -0.5f);
                        }
                        // If after the duel one of them was a lover, teleport the other lover body too
                        if (Modifiers.lover1 != null && (Challenger.rivalPlayer.PlayerId == Modifiers.lover1.PlayerId || Challenger.challenger.PlayerId == Modifiers.lover1.PlayerId)) {
                            var bodyLover2 = UnityEngine.Object.FindObjectsOfType<DeadBody>().FirstOrDefault(b => b.ParentId == Modifiers.lover2.PlayerId);
                            if (PlayerControl.GameOptions.MapId == 5) {
                                if (bodyLover2.transform.position.y > 0) {
                                    bodyLover2.transform.position = new Vector3(5f, 19.5f, -5);
                                }
                                else {
                                    bodyLover2.transform.position = new Vector3(1.35f, -28.25f, -5);
                                }
                            }
                            else {
                                bodyLover2.transform.position = emerButton.transform.position + new Vector3(0.5f, 0f, -0.5f);
                            }
                        }
                        else if (Modifiers.lover2 != null && (Challenger.rivalPlayer.PlayerId == Modifiers.lover2.PlayerId || Challenger.challenger.PlayerId == Modifiers.lover2.PlayerId)) {
                            var bodyLover1 = UnityEngine.Object.FindObjectsOfType<DeadBody>().FirstOrDefault(b => b.ParentId == Modifiers.lover1.PlayerId);
                            if (PlayerControl.GameOptions.MapId == 5) {
                                if (bodyLover1.transform.position.y > 0) {
                                    bodyLover1.transform.position = new Vector3(5f, 19.5f, -5);
                                }
                                else {
                                    bodyLover1.transform.position = new Vector3(1.35f, -28.25f, -5);
                                }
                            }
                            else {
                                bodyLover1.transform.position = emerButton.transform.position + new Vector3(0.5f, 0f, -0.5f);
                            }
                        }
                    }
                    // If after the duel the challenger is dead, teleport his body to the player location
                    else if (Challenger.challenger.Data.IsDead) {
                        var bodyC = UnityEngine.Object.FindObjectsOfType<DeadBody>().FirstOrDefault(b => b.ParentId == Challenger.challenger.PlayerId);
                        if (PlayerControl.GameOptions.MapId == 5) {
                            if (bodyC.transform.position.y > 0) {
                                bodyC.transform.position = new Vector3(5f, 19.5f, -5);
                            }
                            else {
                                bodyC.transform.position = new Vector3(1.35f, -28.25f, -5);
                            }
                        }
                        else {
                            bodyC.transform.position = emerButton.transform.position + new Vector3(0.5f, 0f, -0.5f);
                        }
                        // If after the duel one of them was a lover, teleport the other lover body too
                        if (Modifiers.lover1 != null && Challenger.challenger.PlayerId == Modifiers.lover1.PlayerId) {
                            var bodyLover2 = UnityEngine.Object.FindObjectsOfType<DeadBody>().FirstOrDefault(b => b.ParentId == Modifiers.lover2.PlayerId);
                            if (PlayerControl.GameOptions.MapId == 5) {
                                if (bodyLover2.transform.position.y > 0) {
                                    bodyLover2.transform.position = new Vector3(5f, 19.5f, -5);
                                }
                                else {
                                    bodyLover2.transform.position = new Vector3(1.35f, -28.25f, -5);
                                }
                            }
                            else {
                                bodyLover2.transform.position = emerButton.transform.position + new Vector3(0.5f, 0f, -0.5f);
                            }
                        }
                        else if (Modifiers.lover2 != null && Challenger.challenger.PlayerId == Modifiers.lover2.PlayerId) {
                            var bodyLover1 = UnityEngine.Object.FindObjectsOfType<DeadBody>().FirstOrDefault(b => b.ParentId == Modifiers.lover1.PlayerId);
                            if (PlayerControl.GameOptions.MapId == 5) {
                                if (bodyLover1.transform.position.y > 0) {
                                    bodyLover1.transform.position = new Vector3(5f, 19.5f, -5);
                                }
                                else {
                                    bodyLover1.transform.position = new Vector3(1.35f, -28.25f, -5);
                                }
                            }
                            else {
                                bodyLover1.transform.position = emerButton.transform.position + new Vector3(0.5f, 0f, -0.5f);
                            }
                        }
                    }
                    // If after the duel the rival is dead, teleport his body to the player location
                    else if (Challenger.rivalPlayer.Data.IsDead) {
                        var bodyR = UnityEngine.Object.FindObjectsOfType<DeadBody>().FirstOrDefault(b => b.ParentId == Challenger.rivalPlayer.PlayerId);
                        if (PlayerControl.GameOptions.MapId == 5) {
                            if (bodyR.transform.position.y > 0) {
                                bodyR.transform.position = new Vector3(5f, 19.5f, -5);
                            }
                            else {
                                bodyR.transform.position = new Vector3(1.35f, -28.25f, -5);
                            }
                        }
                        else {
                            bodyR.transform.position = emerButton.transform.position + new Vector3(0.5f, 0f, -0.5f);
                        }
                        // If after the duel one of them was a lover, teleport the other lover body too
                        if (Modifiers.lover1 != null && Challenger.rivalPlayer.PlayerId == Modifiers.lover1.PlayerId) {
                            var bodyLover2 = UnityEngine.Object.FindObjectsOfType<DeadBody>().FirstOrDefault(b => b.ParentId == Modifiers.lover2.PlayerId);
                            if (PlayerControl.GameOptions.MapId == 5) {
                                if (bodyLover2.transform.position.y > 0) {
                                    bodyLover2.transform.position = new Vector3(5f, 19.5f, -5);
                                }
                                else {
                                    bodyLover2.transform.position = new Vector3(1.35f, -28.25f, -5);
                                }
                            }
                            else {
                                bodyLover2.transform.position = emerButton.transform.position + new Vector3(0.5f, 0f, -0.5f);
                            }
                        }
                        else if (Modifiers.lover2 != null && Challenger.rivalPlayer.PlayerId == Modifiers.lover2.PlayerId) {
                            var bodyLover1 = UnityEngine.Object.FindObjectsOfType<DeadBody>().FirstOrDefault(b => b.ParentId == Modifiers.lover1.PlayerId);
                            if (PlayerControl.GameOptions.MapId == 5) {
                                if (bodyLover1.transform.position.y > 0) {
                                    bodyLover1.transform.position = new Vector3(5f, 19.5f, -5);
                                }
                                else {
                                    bodyLover1.transform.position = new Vector3(1.35f, -28.25f, -5);
                                }
                            }
                            else {
                                bodyLover1.transform.position = emerButton.transform.position + new Vector3(0.5f, 0f, -0.5f);
                            }
                        }
                    }
                }
            })));

            HudManager.Instance.StartCoroutine(Effects.Lerp(8, new Action<float>((p) => {
                if (p == 1f) {
                    // Reset painting after dueling
                    foreach (PlayerControl player in PlayerControl.AllPlayerControls) {
                        if (player == null) continue;
                        player.setDefaultLook();
                    }

                    timeTravelerRewindTimeButton.Timer = timeTravelerRewindTimeButton.MaxTimer;
                    timeTravelerShieldButton.Timer = timeTravelerShieldButton.MaxTimer;
                    // Reset challenger values after dueling
                    Challenger.ResetValues();                    
                }
            })));
        }

        static void vigilantMiraUpdate() {

            if (Vigilant.vigilantMira == null || Vigilant.vigilantMira.Data.IsDead || Vigilant.vigilantMira != PlayerControl.LocalPlayer || PlayerControl.GameOptions.MapId != 1) {
                return;
            }

            // Vigilant activate/deactivate doorlog item with Q
            if (Input.GetKeyDown(KeyCode.Q)) {
                Vigilant.doorLogActivated = !Vigilant.doorLogActivated;
                Vigilant.doorLog.SetActive(Vigilant.doorLogActivated);
            }
        }

        static void janitorUpdate() {

            if (Janitor.janitor == null)
                return;

            if (Janitor.dragginBody) {
                DeadBody[] array = UnityEngine.Object.FindObjectsOfType<DeadBody>();
                for (int i = 0; i < array.Length; i++) {
                    if (GameData.Instance.GetPlayerById(array[i].ParentId).PlayerId == Janitor.bodyId) {                      
                        var currentPosition = Janitor.janitor.GetTruePosition();
                        var velocity = Janitor.janitor.gameObject.GetComponent<Rigidbody2D>().velocity.normalized;
                        var newPos = ((Vector2)Janitor.janitor.GetTruePosition()) - (velocity / 3) + new Vector2(0.15f, 0.25f) + array[i].myCollider.offset;
                        if (!PhysicsHelpers.AnythingBetween(
                            currentPosition,
                            newPos,
                            Constants.ShipAndObjectsMask,
                            false
                        )) {
                            if (PlayerControl.GameOptions.MapId == 5) {
                                array[i].transform.position = newPos;
                                array[i].transform.position += new Vector3(0,0, -0.5f);
                            }
                            else {
                                array[i].transform.position = newPos;
                            }
                        }
                    }
                }
            }
        }

        static void batUpdate() {
            if (Bat.bat == null)
                return;

            if (Bat.frequencyTimer > 0 && Bat.bat != PlayerControl.LocalPlayer) {
                if (!Bat.bat.Data.IsDead && Vector2.Distance(Bat.bat.transform.position, PlayerControl.LocalPlayer.transform.position) < (1f * Bat.frequencyRange)) {
                    
                    PlayerControl.LocalPlayer.killTimer += Time.fixedDeltaTime;

                    foreach (CustomButton button in CustomButton.buttons) {
                        if (button.isEffectActive) continue;

                        if (!PlayerControl.LocalPlayer.Data.Role.IsImpostor) {
                            if (button.Timer > 1f)
                                button.Timer -= Time.fixedDeltaTime * 0.5f;
                        }
                        else {
                            if (button.MaxTimer > 0f)
                                if (button.Timer > 1f)
                                    button.Timer += Time.fixedDeltaTime;
                        }
                    }
                }
            }
        }

        static void necromancerUpdate() {

            if (Necromancer.necromancer == null)
                return;

            if (Necromancer.dragginBody) {
                DeadBody[] array = UnityEngine.Object.FindObjectsOfType<DeadBody>();
                for (int i = 0; i < array.Length; i++) {
                    if (GameData.Instance.GetPlayerById(array[i].ParentId).PlayerId == Necromancer.bodyId) {
                        var currentPosition = Necromancer.necromancer.GetTruePosition();
                        var velocity = Necromancer.necromancer.gameObject.GetComponent<Rigidbody2D>().velocity.normalized;
                        var newPos = ((Vector2)Necromancer.necromancer.GetTruePosition()) - (velocity / 3) + new Vector2(0.15f, 0.25f) + array[i].myCollider.offset;
                        if (!PhysicsHelpers.AnythingBetween(
                            currentPosition,
                            newPos,
                            Constants.ShipAndObjectsMask,
                            false
                        )) {
                            if (PlayerControl.GameOptions.MapId == 5) {
                                array[i].transform.position = newPos;
                                array[i].transform.position += new Vector3(0, 0, -0.5f);
                            }
                            else {
                                array[i].transform.position = newPos;
                            }
                        }
                    }
                }
            }
        }
        
        static void captureTheFlagUpdate() {

            if (!CaptureTheFlag.captureTheFlagMode && howmanygamemodesareon != 1)
                return;

            if (CaptureTheFlag.redPlayerWhoHasBlueFlag != null && CaptureTheFlag.redPlayerWhoHasBlueFlag.Data.Disconnected) {
                CaptureTheFlag.blueflag.transform.parent = CaptureTheFlag.blueflagbase.transform.parent;
                switch (PlayerControl.GameOptions.MapId) {
                    // Skeld
                    case 0:
                        if (activatedSensei) {
                            CaptureTheFlag.blueflag.transform.position = new Vector3(7.7f, -1.15f, 0.5f);
                        }
                        else {
                            CaptureTheFlag.blueflag.transform.position = new Vector3(16.5f, -4.65f, 0.5f);
                        }
                        break;
                    // MiraHQ
                    case 1:
                        CaptureTheFlag.blueflag.transform.position = new Vector3(23.25f, 5.05f, 0.5f);
                        break;
                    // Polus
                    case 2:
                        CaptureTheFlag.blueflag.transform.position = new Vector3(5.4f, -9.65f, 0.5f);
                        break;
                    // Dleks
                    case 3:
                        CaptureTheFlag.blueflag.transform.position = new Vector3(-16.5f, -4.65f, 0.5f);
                        break;
                    // Airship
                    case 4:
                        CaptureTheFlag.blueflag.transform.position = new Vector3(33.6f, 1.25f, 0.5f);
                        break;
                    // Submerged
                    case 5:
                        CaptureTheFlag.blueflag.transform.position = new Vector3(12.5f, -31.45f, -0.011f);
                        break;
                }
                CaptureTheFlag.blueflagtaken = false;
                CaptureTheFlag.redPlayerWhoHasBlueFlag = null;
            }

            if (CaptureTheFlag.bluePlayerWhoHasRedFlag != null && CaptureTheFlag.bluePlayerWhoHasRedFlag.Data.Disconnected) {
                CaptureTheFlag.redflag.transform.parent = CaptureTheFlag.redflagbase.transform.parent;
                switch (PlayerControl.GameOptions.MapId) {
                    // Skeld
                    case 0:
                        if (activatedSensei) {
                            CaptureTheFlag.redflag.transform.position = new Vector3(-17.5f, -1.35f, 0.5f);
                        }
                        else {
                            CaptureTheFlag.redflag.transform.position = new Vector3(-20.5f, -5.35f, 0.5f);
                        }
                        break;
                    // MiraHQ
                    case 1:
                        CaptureTheFlag.redflag.transform.position = new Vector3(2.525f, 10.55f, 0.5f);
                        break;
                    // Polus
                    case 2:
                        CaptureTheFlag.redflag.transform.position = new Vector3(36.4f, -21.7f, 0.5f);
                        break;
                    // Dleks
                    case 3:
                        CaptureTheFlag.redflag.transform.position = new Vector3(20.5f, -5.35f, 0.5f);
                        break;
                    // Airship
                    case 4:
                        CaptureTheFlag.redflag.transform.position = new Vector3(-17.5f, -1.2f, 0.5f);
                        break;
                    // Submerged
                    case 5:
                        CaptureTheFlag.redflag.transform.position = new Vector3(-8.35f, 28.05f, 0.03f);
                        break;
                }
                CaptureTheFlag.redflagtaken = false;
                CaptureTheFlag.bluePlayerWhoHasRedFlag = null;
            }
        }

        static void policeandthiefUpdate() {

            if (!PoliceAndThief.policeAndThiefMode && howmanygamemodesareon != 1)
                return;

            // Check number of thiefs if a thief disconnects
            foreach (PlayerControl thief in PoliceAndThief.thiefTeam) {
                if (thief.Data.Disconnected) {

                    if (PoliceAndThief.thiefplayer01 != null && thief.PlayerId == PoliceAndThief.thiefplayer01.PlayerId && PoliceAndThief.thiefplayer01IsStealing) {
                        PoliceAndThief.thiefTeam.Remove(PoliceAndThief.thiefplayer01);
                        RPCProcedure.policeandThiefRevertedJewelPosition(thief.PlayerId, PoliceAndThief.thiefplayer01JewelId);
                    }
                    else if (PoliceAndThief.thiefplayer02 != null && thief.PlayerId == PoliceAndThief.thiefplayer02.PlayerId && PoliceAndThief.thiefplayer02IsStealing) {
                        PoliceAndThief.thiefTeam.Remove(PoliceAndThief.thiefplayer02);
                        RPCProcedure.policeandThiefRevertedJewelPosition(thief.PlayerId, PoliceAndThief.thiefplayer02JewelId);
                    }
                    else if (PoliceAndThief.thiefplayer03 != null && thief.PlayerId == PoliceAndThief.thiefplayer03.PlayerId && PoliceAndThief.thiefplayer03IsStealing) {
                        PoliceAndThief.thiefTeam.Remove(PoliceAndThief.thiefplayer03);
                        RPCProcedure.policeandThiefRevertedJewelPosition(thief.PlayerId, PoliceAndThief.thiefplayer03JewelId);
                    }
                    else if (PoliceAndThief.thiefplayer04 != null && thief.PlayerId == PoliceAndThief.thiefplayer04.PlayerId && PoliceAndThief.thiefplayer04IsStealing) {
                        PoliceAndThief.thiefTeam.Remove(PoliceAndThief.thiefplayer04);
                        RPCProcedure.policeandThiefRevertedJewelPosition(thief.PlayerId, PoliceAndThief.thiefplayer04JewelId);
                    }
                    else if (PoliceAndThief.thiefplayer05 != null && thief.PlayerId == PoliceAndThief.thiefplayer05.PlayerId && PoliceAndThief.thiefplayer05IsStealing) {
                        PoliceAndThief.thiefTeam.Remove(PoliceAndThief.thiefplayer05);
                        RPCProcedure.policeandThiefRevertedJewelPosition(thief.PlayerId, PoliceAndThief.thiefplayer05JewelId);
                    }
                    else if (PoliceAndThief.thiefplayer06 != null && thief.PlayerId == PoliceAndThief.thiefplayer06.PlayerId && PoliceAndThief.thiefplayer06IsStealing) {
                        PoliceAndThief.thiefTeam.Remove(PoliceAndThief.thiefplayer06);
                        RPCProcedure.policeandThiefRevertedJewelPosition(thief.PlayerId, PoliceAndThief.thiefplayer06JewelId);
                    }
                    else if (PoliceAndThief.thiefplayer07 != null && thief.PlayerId == PoliceAndThief.thiefplayer07.PlayerId && PoliceAndThief.thiefplayer07IsStealing) {
                        PoliceAndThief.thiefTeam.Remove(PoliceAndThief.thiefplayer07);
                        RPCProcedure.policeandThiefRevertedJewelPosition(thief.PlayerId, PoliceAndThief.thiefplayer07JewelId);
                    }
                    else if (PoliceAndThief.thiefplayer08 != null && thief.PlayerId == PoliceAndThief.thiefplayer08.PlayerId && PoliceAndThief.thiefplayer08IsStealing) {
                        PoliceAndThief.thiefTeam.Remove(PoliceAndThief.thiefplayer08);
                        RPCProcedure.policeandThiefRevertedJewelPosition(thief.PlayerId, PoliceAndThief.thiefplayer08JewelId);
                    }
                    else if (PoliceAndThief.thiefplayer09 != null && thief.PlayerId == PoliceAndThief.thiefplayer09.PlayerId && PoliceAndThief.thiefplayer09IsStealing) {
                        PoliceAndThief.thiefTeam.Remove(PoliceAndThief.thiefplayer09);
                        RPCProcedure.policeandThiefRevertedJewelPosition(thief.PlayerId, PoliceAndThief.thiefplayer09JewelId);
                    }

                    PoliceAndThief.thiefpointCounter = "Stealed Jewels: " + "<color=#00F7FFFF>" + PoliceAndThief.currentJewelsStoled + "/" + PoliceAndThief.requiredJewels + "</color> | " + "Thiefs Captured: " + "<color=#928B55FF>" + PoliceAndThief.currentThiefsCaptured + "/" + PoliceAndThief.thiefTeam.Count + "</color>";
                    if (PoliceAndThief.currentThiefsCaptured == PoliceAndThief.thiefTeam.Count) {
                        PoliceAndThief.triggerPoliceWin = true;
                        ShipStatus.RpcEndGame((GameOverReason)CustomGameOverReason.ThiefModePoliceWin, false);
                    }
                    break;
                }
            }

            foreach (PlayerControl police in PoliceAndThief.policeTeam) {
                if (police.Data.Disconnected) {
                    if (PoliceAndThief.policeplayer01 != null && police.PlayerId == PoliceAndThief.policeplayer01.PlayerId) {
                        PoliceAndThief.policeTeam.Remove(PoliceAndThief.policeplayer01);
                    }
                    else if (PoliceAndThief.policeplayer02 != null && police.PlayerId == PoliceAndThief.policeplayer02.PlayerId) {
                        PoliceAndThief.policeTeam.Remove(PoliceAndThief.policeplayer02);
                    }
                    else if (PoliceAndThief.policeplayer03 != null && police.PlayerId == PoliceAndThief.policeplayer03.PlayerId) {
                        PoliceAndThief.policeTeam.Remove(PoliceAndThief.policeplayer03);
                    }
                    else if (PoliceAndThief.policeplayer04 != null && police.PlayerId == PoliceAndThief.policeplayer04.PlayerId) {
                        PoliceAndThief.policeTeam.Remove(PoliceAndThief.policeplayer04);
                    }
                    else if (PoliceAndThief.policeplayer05 != null && police.PlayerId == PoliceAndThief.policeplayer05.PlayerId) {
                        PoliceAndThief.policeTeam.Remove(PoliceAndThief.policeplayer05);
                    }
                    else if (PoliceAndThief.policeplayer06 != null && police.PlayerId == PoliceAndThief.policeplayer06.PlayerId) {
                        PoliceAndThief.policeTeam.Remove(PoliceAndThief.policeplayer06);
                    }

                    if (PoliceAndThief.policeTeam.Count <= 0) {
                        PoliceAndThief.triggerThiefWin = true;
                        ShipStatus.RpcEndGame((GameOverReason)CustomGameOverReason.ThiefModeThiefWin, false);
                    }
                    break;
                }
            }
        }

        static void kingOfTheHillUpdate() {

            if (!KingOfTheHill.kingOfTheHillMode && howmanygamemodesareon != 1)
                return;

            // If king disconnects, assing new king
            if (KingOfTheHill.greenKingplayer != null && KingOfTheHill.greenKingplayer.Data.Disconnected) {
                KingOfTheHill.greenTeam.Remove(KingOfTheHill.greenKingplayer);
                KingOfTheHill.greenKingplayer = null;
                KingOfTheHill.greenKingplayer = KingOfTheHill.greenTeam[0];
                if (PlayerControl.GameOptions.MapId == 5) {
                    KingOfTheHill.greenkingaura.transform.position = new Vector3(KingOfTheHill.greenKingplayer.transform.position.x, KingOfTheHill.greenKingplayer.transform.position.y, -0.5f);
                }
                else {
                    KingOfTheHill.greenkingaura.transform.position = new Vector3(KingOfTheHill.greenKingplayer.transform.position.x, KingOfTheHill.greenKingplayer.transform.position.y, 0.4f);
                }
                KingOfTheHill.greenkingaura.transform.parent = KingOfTheHill.greenKingplayer.transform;
                if (PlayerControl.LocalPlayer == KingOfTheHill.greenKingplayer) {
                    new CustomMessage("You're the new <color=#00FF00FF>Green King</color>!", 5, -1, 1.6f, 11);
                    KingOfTheHill.localArrows[3].arrow.SetActive(false);
                }
                KingOfTheHill.greenKingIsReviving = false;

                // Remove minion player from new king
                if (KingOfTheHill.greenplayer01 != null && KingOfTheHill.greenTeam[0] == KingOfTheHill.greenplayer01) {
                    KingOfTheHill.greenplayer01 = null;
                }
                else if (KingOfTheHill.greenplayer02 != null && KingOfTheHill.greenTeam[0] == KingOfTheHill.greenplayer02) {
                    KingOfTheHill.greenplayer02 = null;
                }
                else if (KingOfTheHill.greenplayer03 != null && KingOfTheHill.greenTeam[0] == KingOfTheHill.greenplayer03) {
                    KingOfTheHill.greenplayer03 = null;
                }
                else if (KingOfTheHill.greenplayer04 != null && KingOfTheHill.greenTeam[0] == KingOfTheHill.greenplayer04) {
                    KingOfTheHill.greenplayer04 = null;
                }
                else if (KingOfTheHill.greenplayer05 != null && KingOfTheHill.greenTeam[0] == KingOfTheHill.greenplayer05) {
                    KingOfTheHill.greenplayer05 = null;
                }
                else if (KingOfTheHill.greenplayer06 != null && KingOfTheHill.greenTeam[0] == KingOfTheHill.greenplayer06) {
                    KingOfTheHill.greenplayer06 = null;
                }

                KingOfTheHill.greenTeam.RemoveAt(0);
                KingOfTheHill.greenTeam.Add(KingOfTheHill.greenKingplayer);
                return;
            }           

            if (KingOfTheHill.yellowKingplayer != null && KingOfTheHill.yellowKingplayer.Data.Disconnected) {
                KingOfTheHill.yellowTeam.Remove(KingOfTheHill.yellowKingplayer);
                KingOfTheHill.yellowKingplayer = null;
                KingOfTheHill.yellowKingplayer = KingOfTheHill.yellowTeam[0];
                if (PlayerControl.GameOptions.MapId == 5) {
                    KingOfTheHill.yellowkingaura.transform.position = new Vector3(KingOfTheHill.yellowKingplayer.transform.position.x, KingOfTheHill.yellowKingplayer.transform.position.y, -0.5f);
                }
                else {
                    KingOfTheHill.yellowkingaura.transform.position = new Vector3(KingOfTheHill.yellowKingplayer.transform.position.x, KingOfTheHill.yellowKingplayer.transform.position.y, 0.4f);
                }
                KingOfTheHill.yellowkingaura.transform.parent = KingOfTheHill.yellowKingplayer.transform;
                if (PlayerControl.LocalPlayer == KingOfTheHill.yellowKingplayer) {
                    new CustomMessage("You're the new <color=#FFFF00FF>Yellow King</color>!", 5, -1, 1.6f, 11);
                    KingOfTheHill.localArrows[3].arrow.SetActive(false);
                }
                KingOfTheHill.yellowKingIsReviving = false;

                // Remove minion player from new king
                if (KingOfTheHill.yellowplayer01 != null && KingOfTheHill.yellowTeam[0] == KingOfTheHill.yellowplayer01) {
                    KingOfTheHill.yellowplayer01 = null;
                }
                else if (KingOfTheHill.yellowplayer02 != null && KingOfTheHill.yellowTeam[0] == KingOfTheHill.yellowplayer02) {
                    KingOfTheHill.yellowplayer02 = null;
                }
                else if (KingOfTheHill.yellowplayer03 != null && KingOfTheHill.yellowTeam[0] == KingOfTheHill.yellowplayer03) {
                    KingOfTheHill.yellowplayer03 = null;
                }
                else if (KingOfTheHill.yellowplayer04 != null && KingOfTheHill.yellowTeam[0] == KingOfTheHill.yellowplayer04) {
                    KingOfTheHill.yellowplayer04 = null;
                }
                else if (KingOfTheHill.yellowplayer05 != null && KingOfTheHill.yellowTeam[0] == KingOfTheHill.yellowplayer05) {
                    KingOfTheHill.yellowplayer05 = null;
                }
                else if (KingOfTheHill.yellowplayer06 != null && KingOfTheHill.yellowTeam[0] == KingOfTheHill.yellowplayer06) {
                    KingOfTheHill.yellowplayer06 = null;
                }

                KingOfTheHill.yellowTeam.RemoveAt(0);
                KingOfTheHill.yellowTeam.Add(KingOfTheHill.yellowKingplayer);
                return;
            }        
        }

        static void hotPotatoUpdate() {

            if (!HotPotato.hotPotatoMode && howmanygamemodesareon != 1)
                return;

            // Hide hot potato sprite if in vent
            if (HotPotato.hotPotatoPlayer != null && HotPotato.hotPotato != null) {
                if (HotPotato.hotPotatoPlayer.inVent) {
                    HotPotato.hotPotato.SetActive(false);
                }
                else {
                    HotPotato.hotPotato.SetActive(true);
                }
            }

            // If hot potato disconnects, assing new potato and reset timer
            if (HotPotato.hotPotatoPlayer != null && HotPotato.hotPotatoPlayer.Data.Disconnected) {

                if (!HotPotato.firstPotatoTransfered) {
                    HotPotato.firstPotatoTransfered = true;
                    new CustomMessage("Hot Potato: ", HotPotato.matchDuration, -1, -1f, 18);
                    new CustomMessage("Time Left: ", HotPotato.matchDuration, -1, -1.3f, 15);
                    HotPotato.hotpotatopointCounter = "Hot Potato: " + "<color=#808080FF>" + HotPotato.hotPotatoPlayer.name + "</color> | " + "Cold Potatoes: " + "<color=#00F7FFFF>" + HotPotato.notPotatoTeam.Count + "</color>";
                    new CustomMessage(HotPotato.hotpotatopointCounter, HotPotato.matchDuration, -1, 1.9f, 17);
                }

                HotPotato.timeforTransfer = HotPotato.savedtimeforTransfer;

                int notPotatosAlives = -1;
                HotPotato.notPotatoTeamAlive.Clear();
                foreach (PlayerControl remainPotato in HotPotato.notPotatoTeam) {
                    if (!remainPotato.Data.IsDead) {
                        notPotatosAlives += 1;
                        HotPotato.notPotatoTeamAlive.Add(remainPotato);
                    }
                }

                if (notPotatosAlives < 1) {
                    HotPotato.triggerHotPotatoEnd = true;
                    ShipStatus.RpcEndGame((GameOverReason)CustomGameOverReason.HotPotatoEnd, false);
                }

                HotPotato.hotPotatoPlayer = HotPotato.notPotatoTeam[0];
                HotPotato.hotPotato.transform.position = HotPotato.hotPotatoPlayer.transform.position + new Vector3(0, 0.5f, -0.25f);
                HotPotato.hotPotato.transform.parent = HotPotato.hotPotatoPlayer.transform;

                // If hot potato timed out, assing new potato
                if (HotPotato.notPotato01 != null && HotPotato.notPotatoTeam[0] == HotPotato.notPotato01) {
                    HotPotato.notPotato01 = null;
                }
                else if (HotPotato.notPotato02 != null && HotPotato.notPotatoTeam[0] == HotPotato.notPotato02) {
                    HotPotato.notPotato02 = null;
                }
                else if (HotPotato.notPotato03 != null && HotPotato.notPotatoTeam[0] == HotPotato.notPotato03) {
                    HotPotato.notPotato03 = null;
                }
                else if (HotPotato.notPotato04 != null && HotPotato.notPotatoTeam[0] == HotPotato.notPotato04) {
                    HotPotato.notPotato04 = null;
                }
                else if (HotPotato.notPotato05 != null && HotPotato.notPotatoTeam[0] == HotPotato.notPotato05) {
                    HotPotato.notPotato05 = null;
                }
                else if (HotPotato.notPotato06 != null && HotPotato.notPotatoTeam[0] == HotPotato.notPotato06) {
                    HotPotato.notPotato06 = null;
                }
                else if (HotPotato.notPotato07 != null && HotPotato.notPotatoTeam[0] == HotPotato.notPotato07) {
                    HotPotato.notPotato07 = null;
                }
                else if (HotPotato.notPotato08 != null && HotPotato.notPotatoTeam[0] == HotPotato.notPotato08) {
                    HotPotato.notPotato08 = null;
                }
                else if (HotPotato.notPotato09 != null && HotPotato.notPotatoTeam[0] == HotPotato.notPotato09) {
                    HotPotato.notPotato09 = null;
                }
                else if (HotPotato.notPotato10 != null && HotPotato.notPotatoTeam[0] == HotPotato.notPotato10) {
                    HotPotato.notPotato10 = null;
                }
                else if (HotPotato.notPotato11 != null && HotPotato.notPotatoTeam[0] == HotPotato.notPotato11) {
                    HotPotato.notPotato11 = null;
                }
                else if (HotPotato.notPotato12 != null && HotPotato.notPotatoTeam[0] == HotPotato.notPotato12) {
                    HotPotato.notPotato12 = null;
                }
                else if (HotPotato.notPotato13 != null && HotPotato.notPotatoTeam[0] == HotPotato.notPotato13) {
                    HotPotato.notPotato13 = null;
                }
                else if (HotPotato.notPotato14 != null && HotPotato.notPotatoTeam[0] == HotPotato.notPotato14) {
                    HotPotato.notPotato14 = null;
                }

                HotPotato.notPotatoTeam.RemoveAt(0);

                hotPotatoButton.Timer = HotPotato.transferCooldown;

                new CustomMessage("<color=#808080FF>" + HotPotato.hotPotatoPlayer.name + "</color> is the new Hot Potato!", 5, -1, 1f, 16);
                HotPotato.hotpotatopointCounter = "Hot Potato: " + "<color=#808080FF>" + HotPotato.hotPotatoPlayer.name + "</color> | " + "Cold Potatoes: " + "<color=#00F7FFFF>" + notPotatosAlives + "</color>";
            }

            // If notpotato disconnects, check number of notpotatos
            foreach (PlayerControl notPotato in HotPotato.notPotatoTeam) {
                if (notPotato.Data.Disconnected) {

                    int notPotatosAlives = -1;
                    HotPotato.notPotatoTeamAlive.Clear();
                    foreach (PlayerControl remainPotato in HotPotato.notPotatoTeam) {
                        if (!remainPotato.Data.IsDead) {
                            notPotatosAlives += 1;
                            HotPotato.notPotatoTeamAlive.Add(remainPotato);
                        }
                    }

                    if (notPotatosAlives < 1) {
                        HotPotato.triggerHotPotatoEnd = true;
                        ShipStatus.RpcEndGame((GameOverReason)CustomGameOverReason.HotPotatoEnd, false);
                    }
                    
                    if (HotPotato.notPotato01 != null && notPotato.PlayerId == HotPotato.notPotato01.PlayerId) {
                        HotPotato.notPotatoTeam.Remove(HotPotato.notPotato01);
                    }
                    else if (HotPotato.notPotato02 != null && notPotato.PlayerId == HotPotato.notPotato02.PlayerId) {
                        HotPotato.notPotatoTeam.Remove(HotPotato.notPotato02);
                    }
                    else if (HotPotato.notPotato03 != null && notPotato.PlayerId == HotPotato.notPotato03.PlayerId) {
                        HotPotato.notPotatoTeam.Remove(HotPotato.notPotato03);
                    }
                    else if (HotPotato.notPotato04 != null && notPotato.PlayerId == HotPotato.notPotato04.PlayerId) {
                        HotPotato.notPotatoTeam.Remove(HotPotato.notPotato04);
                    }
                    else if (HotPotato.notPotato05 != null && notPotato.PlayerId == HotPotato.notPotato05.PlayerId) {
                        HotPotato.notPotatoTeam.Remove(HotPotato.notPotato05);
                    }
                    else if (HotPotato.notPotato06 != null && notPotato.PlayerId == HotPotato.notPotato06.PlayerId) {
                        HotPotato.notPotatoTeam.Remove(HotPotato.notPotato06);
                    }
                    else if (HotPotato.notPotato07 != null && notPotato.PlayerId == HotPotato.notPotato07.PlayerId) {
                        HotPotato.notPotatoTeam.Remove(HotPotato.notPotato07);
                    }
                    else if (HotPotato.notPotato08 != null && notPotato.PlayerId == HotPotato.notPotato08.PlayerId) {
                        HotPotato.notPotatoTeam.Remove(HotPotato.notPotato08);
                    }
                    else if (HotPotato.notPotato09 != null && notPotato.PlayerId == HotPotato.notPotato09.PlayerId) {
                        HotPotato.notPotatoTeam.Remove(HotPotato.notPotato09);
                    }
                    else if (HotPotato.notPotato10 != null && notPotato.PlayerId == HotPotato.notPotato10.PlayerId) {
                        HotPotato.notPotatoTeam.Remove(HotPotato.notPotato10);
                    }
                    else if (HotPotato.notPotato11 != null && notPotato.PlayerId == HotPotato.notPotato11.PlayerId) {
                        HotPotato.notPotatoTeam.Remove(HotPotato.notPotato11);
                    }
                    else if (HotPotato.notPotato12 != null && notPotato.PlayerId == HotPotato.notPotato12.PlayerId) {
                        HotPotato.notPotatoTeam.Remove(HotPotato.notPotato12);
                    }
                    else if (HotPotato.notPotato13 != null && notPotato.PlayerId == HotPotato.notPotato13.PlayerId) {
                        HotPotato.notPotatoTeam.Remove(HotPotato.notPotato13);
                    }
                    else if (HotPotato.notPotato14 != null && notPotato.PlayerId == HotPotato.notPotato14.PlayerId) {
                        HotPotato.notPotatoTeam.Remove(HotPotato.notPotato14);
                    }

                    HotPotato.hotpotatopointCounter = "Hot Potato: " + "<color=#808080FF>" + HotPotato.hotPotatoPlayer.name + "</color> | " + "Cold Potatoes: " + "<color=#00F7FFFF>" + notPotatosAlives + "</color>";
                    break;
                }
            }
        }

        static void zombieLaboratoryUpdate() {

            if (!ZombieLaboratory.zombieLaboratoryMode && howmanygamemodesareon != 1)
                return;

            // Check number of survivors if a survivor disconnects
            foreach (PlayerControl survivor in ZombieLaboratory.survivorTeam) {
                if (survivor.Data.Disconnected) {

                    if (ZombieLaboratory.nursePlayer != null && survivor.PlayerId == ZombieLaboratory.nursePlayer.PlayerId) {
                        ZombieLaboratory.survivorTeam.Remove(ZombieLaboratory.nursePlayer);
                        ZombieLaboratory.zombieLaboratoryCounter = "Key Items: " + "<color=#FF00FFFF>" + ZombieLaboratory.currentKeyItems + " / 6</color> | " + "Survivors: " + "<color=#00CCFFFF>" + ZombieLaboratory.survivorTeam.Count + "</color> " + "| " + "Infected: " + "<color=#FFFF00FF>" + ZombieLaboratory.infectedTeam.Count + "</color> " + "| " + "Zombies: " + "<color=#996633FF>" + ZombieLaboratory.zombieTeam.Count + "</color>";
                        ZombieLaboratory.triggerZombieWin = true;
                        ShipStatus.RpcEndGame((GameOverReason)CustomGameOverReason.ZombieWin, false);
                    }
                    else if (ZombieLaboratory.survivorPlayer01 != null && survivor.PlayerId == ZombieLaboratory.survivorPlayer01.PlayerId) {
                        if (ZombieLaboratory.survivorPlayer01IsInfected) {
                            ZombieLaboratory.survivorPlayer01IsInfected = false;
                            ZombieLaboratory.infectedTeam.Remove(ZombieLaboratory.survivorPlayer01);
                        }
                        ZombieLaboratory.survivorTeam.Remove(ZombieLaboratory.survivorPlayer01);
                        RPCProcedure.zombieLaboratoryRevertedKeyPosition(survivor.PlayerId, ZombieLaboratory.survivorPlayer01FoundBox);
                    }
                    else if (ZombieLaboratory.survivorPlayer02 != null && survivor.PlayerId == ZombieLaboratory.survivorPlayer02.PlayerId) {
                        if (ZombieLaboratory.survivorPlayer02IsInfected) {
                            ZombieLaboratory.survivorPlayer02IsInfected = false;
                            ZombieLaboratory.infectedTeam.Remove(ZombieLaboratory.survivorPlayer02);
                        }
                        ZombieLaboratory.survivorTeam.Remove(ZombieLaboratory.survivorPlayer02);
                        RPCProcedure.zombieLaboratoryRevertedKeyPosition(survivor.PlayerId, ZombieLaboratory.survivorPlayer02FoundBox);
                    }
                    else if (ZombieLaboratory.survivorPlayer03 != null && survivor.PlayerId == ZombieLaboratory.survivorPlayer03.PlayerId) {
                        if (ZombieLaboratory.survivorPlayer03IsInfected) {
                            ZombieLaboratory.survivorPlayer03IsInfected = false;
                            ZombieLaboratory.infectedTeam.Remove(ZombieLaboratory.survivorPlayer03);
                        }
                        ZombieLaboratory.survivorTeam.Remove(ZombieLaboratory.survivorPlayer03);
                        RPCProcedure.zombieLaboratoryRevertedKeyPosition(survivor.PlayerId, ZombieLaboratory.survivorPlayer03FoundBox);
                    }
                    else if (ZombieLaboratory.survivorPlayer04 != null && survivor.PlayerId == ZombieLaboratory.survivorPlayer04.PlayerId) {
                        if (ZombieLaboratory.survivorPlayer04IsInfected) {
                            ZombieLaboratory.survivorPlayer04IsInfected = false;
                            ZombieLaboratory.infectedTeam.Remove(ZombieLaboratory.survivorPlayer04);
                        }
                        ZombieLaboratory.survivorTeam.Remove(ZombieLaboratory.survivorPlayer04);
                        RPCProcedure.zombieLaboratoryRevertedKeyPosition(survivor.PlayerId, ZombieLaboratory.survivorPlayer04FoundBox);
                    }
                    else if (ZombieLaboratory.survivorPlayer05 != null && survivor.PlayerId == ZombieLaboratory.survivorPlayer05.PlayerId) {
                        if (ZombieLaboratory.survivorPlayer05IsInfected) {
                            ZombieLaboratory.survivorPlayer05IsInfected = false;
                            ZombieLaboratory.infectedTeam.Remove(ZombieLaboratory.survivorPlayer05);
                        }
                        ZombieLaboratory.survivorTeam.Remove(ZombieLaboratory.survivorPlayer05);
                        RPCProcedure.zombieLaboratoryRevertedKeyPosition(survivor.PlayerId, ZombieLaboratory.survivorPlayer05FoundBox);
                    }
                    else if (ZombieLaboratory.survivorPlayer06 != null && survivor.PlayerId == ZombieLaboratory.survivorPlayer06.PlayerId) {
                        if (ZombieLaboratory.survivorPlayer06IsInfected) {
                            ZombieLaboratory.survivorPlayer06IsInfected = false;
                            ZombieLaboratory.infectedTeam.Remove(ZombieLaboratory.survivorPlayer06);
                        }
                        ZombieLaboratory.survivorTeam.Remove(ZombieLaboratory.survivorPlayer06);
                        RPCProcedure.zombieLaboratoryRevertedKeyPosition(survivor.PlayerId, ZombieLaboratory.survivorPlayer06FoundBox);
                    }
                    else if (ZombieLaboratory.survivorPlayer07 != null && survivor.PlayerId == ZombieLaboratory.survivorPlayer07.PlayerId) {
                        if (ZombieLaboratory.survivorPlayer07IsInfected) {
                            ZombieLaboratory.survivorPlayer07IsInfected = false;
                            ZombieLaboratory.infectedTeam.Remove(ZombieLaboratory.survivorPlayer07);
                        }
                        ZombieLaboratory.survivorTeam.Remove(ZombieLaboratory.survivorPlayer07);
                        RPCProcedure.zombieLaboratoryRevertedKeyPosition(survivor.PlayerId, ZombieLaboratory.survivorPlayer07FoundBox);
                    }
                    else if (ZombieLaboratory.survivorPlayer08 != null && survivor.PlayerId == ZombieLaboratory.survivorPlayer08.PlayerId) {
                        if (ZombieLaboratory.survivorPlayer08IsInfected) {
                            ZombieLaboratory.survivorPlayer08IsInfected = false;
                            ZombieLaboratory.infectedTeam.Remove(ZombieLaboratory.survivorPlayer08);
                        }
                        ZombieLaboratory.survivorTeam.Remove(ZombieLaboratory.survivorPlayer08);
                        RPCProcedure.zombieLaboratoryRevertedKeyPosition(survivor.PlayerId, ZombieLaboratory.survivorPlayer08FoundBox);
                    }
                    else if (ZombieLaboratory.survivorPlayer09 != null && survivor.PlayerId == ZombieLaboratory.survivorPlayer09.PlayerId) {
                        if (ZombieLaboratory.survivorPlayer09IsInfected) {
                            ZombieLaboratory.survivorPlayer09IsInfected = false;
                            ZombieLaboratory.infectedTeam.Remove(ZombieLaboratory.survivorPlayer09);
                        }
                        ZombieLaboratory.survivorTeam.Remove(ZombieLaboratory.survivorPlayer09);
                        RPCProcedure.zombieLaboratoryRevertedKeyPosition(survivor.PlayerId, ZombieLaboratory.survivorPlayer09FoundBox);
                    }
                    else if (ZombieLaboratory.survivorPlayer10 != null && survivor.PlayerId == ZombieLaboratory.survivorPlayer10.PlayerId) {
                        if (ZombieLaboratory.survivorPlayer10IsInfected) {
                            ZombieLaboratory.survivorPlayer10IsInfected = false;
                            ZombieLaboratory.infectedTeam.Remove(ZombieLaboratory.survivorPlayer10);
                        }
                        ZombieLaboratory.survivorTeam.Remove(ZombieLaboratory.survivorPlayer10);
                        RPCProcedure.zombieLaboratoryRevertedKeyPosition(survivor.PlayerId, ZombieLaboratory.survivorPlayer10FoundBox);
                    }
                    else if (ZombieLaboratory.survivorPlayer11 != null && survivor.PlayerId == ZombieLaboratory.survivorPlayer11.PlayerId) {
                        if (ZombieLaboratory.survivorPlayer11IsInfected) {
                            ZombieLaboratory.survivorPlayer11IsInfected = false;
                            ZombieLaboratory.infectedTeam.Remove(ZombieLaboratory.survivorPlayer11);
                        }
                        ZombieLaboratory.survivorTeam.Remove(ZombieLaboratory.survivorPlayer11);
                        RPCProcedure.zombieLaboratoryRevertedKeyPosition(survivor.PlayerId, ZombieLaboratory.survivorPlayer11FoundBox);
                    }
                    else if (ZombieLaboratory.survivorPlayer12 != null && survivor.PlayerId == ZombieLaboratory.survivorPlayer12.PlayerId) {
                        if (ZombieLaboratory.survivorPlayer12IsInfected) {
                            ZombieLaboratory.survivorPlayer12IsInfected = false;
                            ZombieLaboratory.infectedTeam.Remove(ZombieLaboratory.survivorPlayer12);
                        }
                        ZombieLaboratory.survivorTeam.Remove(ZombieLaboratory.survivorPlayer12);
                        RPCProcedure.zombieLaboratoryRevertedKeyPosition(survivor.PlayerId, ZombieLaboratory.survivorPlayer12FoundBox);
                    }
                    else if (ZombieLaboratory.survivorPlayer13 != null && survivor.PlayerId == ZombieLaboratory.survivorPlayer13.PlayerId) {
                        if (ZombieLaboratory.survivorPlayer13IsInfected) {
                            ZombieLaboratory.survivorPlayer13IsInfected = false;
                            ZombieLaboratory.infectedTeam.Remove(ZombieLaboratory.survivorPlayer13);
                        }
                        ZombieLaboratory.survivorTeam.Remove(ZombieLaboratory.survivorPlayer13);
                        RPCProcedure.zombieLaboratoryRevertedKeyPosition(survivor.PlayerId, ZombieLaboratory.survivorPlayer13FoundBox);
                    }

                    // Check win condition
                    ZombieLaboratory.zombieLaboratoryCounter = "Key Items: " + "<color=#FF00FFFF>" + ZombieLaboratory.currentKeyItems + " / 6</color> | " + "Survivors: " + "<color=#00CCFFFF>" + ZombieLaboratory.survivorTeam.Count + "</color> " + "| " + "Infected: " + "<color=#FFFF00FF>" + ZombieLaboratory.infectedTeam.Count + "</color> " + "| " + "Zombies: " + "<color=#996633FF>" + ZombieLaboratory.zombieTeam.Count + "</color>";
                    if (ZombieLaboratory.survivorTeam.Count == 1) {
                        ZombieLaboratory.triggerZombieWin = true;
                        ShipStatus.RpcEndGame((GameOverReason)CustomGameOverReason.ZombieWin, false);
                    }
                    break;
                }
            }

            foreach (PlayerControl zombie in ZombieLaboratory.zombieTeam) {
                if (zombie.Data.Disconnected) {
                    // Check win condition
                    if (ZombieLaboratory.zombiePlayer01 != null && zombie.PlayerId == ZombieLaboratory.zombiePlayer01.PlayerId) {
                        ZombieLaboratory.zombieTeam.Remove(ZombieLaboratory.zombiePlayer01);
                    }
                    else if (ZombieLaboratory.zombiePlayer02 != null && zombie.PlayerId == ZombieLaboratory.zombiePlayer02.PlayerId) {
                        ZombieLaboratory.zombieTeam.Remove(ZombieLaboratory.zombiePlayer02);
                    }
                    else if (ZombieLaboratory.zombiePlayer03 != null && zombie.PlayerId == ZombieLaboratory.zombiePlayer03.PlayerId) {
                        ZombieLaboratory.zombieTeam.Remove(ZombieLaboratory.zombiePlayer03);
                    }
                    else if (ZombieLaboratory.zombiePlayer04 != null && zombie.PlayerId == ZombieLaboratory.zombiePlayer04.PlayerId) {
                        ZombieLaboratory.zombieTeam.Remove(ZombieLaboratory.zombiePlayer04);
                    }
                    else if (ZombieLaboratory.zombiePlayer05 != null && zombie.PlayerId == ZombieLaboratory.zombiePlayer05.PlayerId) {
                        ZombieLaboratory.zombieTeam.Remove(ZombieLaboratory.zombiePlayer05);
                    }
                    else if (ZombieLaboratory.zombiePlayer06 != null && zombie.PlayerId == ZombieLaboratory.zombiePlayer06.PlayerId) {
                        ZombieLaboratory.zombieTeam.Remove(ZombieLaboratory.zombiePlayer06);
                    }
                    else if (ZombieLaboratory.zombiePlayer07 != null && zombie.PlayerId == ZombieLaboratory.zombiePlayer07.PlayerId) {
                        ZombieLaboratory.zombieTeam.Remove(ZombieLaboratory.zombiePlayer07);
                    }
                    else if (ZombieLaboratory.zombiePlayer08 != null && zombie.PlayerId == ZombieLaboratory.zombiePlayer08.PlayerId) {
                        ZombieLaboratory.zombieTeam.Remove(ZombieLaboratory.zombiePlayer08);
                    }
                    else if (ZombieLaboratory.zombiePlayer09 != null && zombie.PlayerId == ZombieLaboratory.zombiePlayer09.PlayerId) {
                        ZombieLaboratory.zombieTeam.Remove(ZombieLaboratory.zombiePlayer09);
                    }
                    else if (ZombieLaboratory.zombiePlayer10 != null && zombie.PlayerId == ZombieLaboratory.zombiePlayer10.PlayerId) {
                        ZombieLaboratory.zombieTeam.Remove(ZombieLaboratory.zombiePlayer10);
                    }
                    else if (ZombieLaboratory.zombiePlayer11 != null && zombie.PlayerId == ZombieLaboratory.zombiePlayer11.PlayerId) {
                        ZombieLaboratory.zombieTeam.Remove(ZombieLaboratory.zombiePlayer11);
                    }
                    else if (ZombieLaboratory.zombiePlayer12 != null && zombie.PlayerId == ZombieLaboratory.zombiePlayer12.PlayerId) {
                        ZombieLaboratory.zombieTeam.Remove(ZombieLaboratory.zombiePlayer12);
                    }
                    else if (ZombieLaboratory.zombiePlayer13 != null && zombie.PlayerId == ZombieLaboratory.zombiePlayer13.PlayerId) {
                        ZombieLaboratory.zombieTeam.Remove(ZombieLaboratory.zombiePlayer13);
                    }
                    else if (ZombieLaboratory.zombiePlayer14 != null && zombie.PlayerId == ZombieLaboratory.zombiePlayer14.PlayerId) {
                        ZombieLaboratory.zombieTeam.Remove(ZombieLaboratory.zombiePlayer14);
                    }
                    ZombieLaboratory.zombieLaboratoryCounter = "Key Items: " + "<color=#FF00FFFF>" + ZombieLaboratory.currentKeyItems + " / 6</color> | " + "Survivors: " + "<color=#00CCFFFF>" + ZombieLaboratory.survivorTeam.Count + "</color> " + "| " + "Infected: " + "<color=#FFFF00FF>" + ZombieLaboratory.infectedTeam.Count + "</color> " + "| " + "Zombies: " + "<color=#996633FF>" + ZombieLaboratory.zombieTeam.Count + "</color>";
                    if (ZombieLaboratory.zombieTeam.Count <= 0) {
                        ZombieLaboratory.triggerSurvivorWin = true;
                        ShipStatus.RpcEndGame((GameOverReason)CustomGameOverReason.SurvivorWin, false);
                    }
                    break;
                }
            }
        }

        static void battleRoyaleUpdate() {

            if (!BattleRoyale.battleRoyaleMode && howmanygamemodesareon != 1)
                return;
            
            if (BattleRoyale.matchType == 0) {
                // If solo player disconnects, check number of players
                foreach (PlayerControl soloPlayer in BattleRoyale.soloPlayerTeam) {
                    if (soloPlayer.Data.Disconnected) {

                        if (BattleRoyale.soloPlayer01 != null && soloPlayer.PlayerId == BattleRoyale.soloPlayer01.PlayerId) {
                            BattleRoyale.soloPlayerTeam.Remove(BattleRoyale.soloPlayer01);
                        }
                        else if (BattleRoyale.soloPlayer02 != null && soloPlayer.PlayerId == BattleRoyale.soloPlayer02.PlayerId) {
                            BattleRoyale.soloPlayerTeam.Remove(BattleRoyale.soloPlayer02);
                        }
                        else if (BattleRoyale.soloPlayer03 != null && soloPlayer.PlayerId == BattleRoyale.soloPlayer03.PlayerId) {
                            BattleRoyale.soloPlayerTeam.Remove(BattleRoyale.soloPlayer03);
                        }
                        else if (BattleRoyale.soloPlayer04 != null && soloPlayer.PlayerId == BattleRoyale.soloPlayer04.PlayerId) {
                            BattleRoyale.soloPlayerTeam.Remove(BattleRoyale.soloPlayer04);
                        }
                        else if (BattleRoyale.soloPlayer05 != null && soloPlayer.PlayerId == BattleRoyale.soloPlayer05.PlayerId) {
                            BattleRoyale.soloPlayerTeam.Remove(BattleRoyale.soloPlayer05);
                        }
                        else if (BattleRoyale.soloPlayer06 != null && soloPlayer.PlayerId == BattleRoyale.soloPlayer06.PlayerId) {
                            BattleRoyale.soloPlayerTeam.Remove(BattleRoyale.soloPlayer06);
                        }
                        else if (BattleRoyale.soloPlayer07 != null && soloPlayer.PlayerId == BattleRoyale.soloPlayer07.PlayerId) {
                            BattleRoyale.soloPlayerTeam.Remove(BattleRoyale.soloPlayer07);
                        }
                        else if (BattleRoyale.soloPlayer08 != null && soloPlayer.PlayerId == BattleRoyale.soloPlayer08.PlayerId) {
                            BattleRoyale.soloPlayerTeam.Remove(BattleRoyale.soloPlayer08);
                        }
                        else if (BattleRoyale.soloPlayer09 != null && soloPlayer.PlayerId == BattleRoyale.soloPlayer09.PlayerId) {
                            BattleRoyale.soloPlayerTeam.Remove(BattleRoyale.soloPlayer09);
                        }
                        else if (BattleRoyale.soloPlayer10 != null && soloPlayer.PlayerId == BattleRoyale.soloPlayer10.PlayerId) {
                            BattleRoyale.soloPlayerTeam.Remove(BattleRoyale.soloPlayer10);
                        }
                        else if (BattleRoyale.soloPlayer11 != null && soloPlayer.PlayerId == BattleRoyale.soloPlayer11.PlayerId) {
                            BattleRoyale.soloPlayerTeam.Remove(BattleRoyale.soloPlayer11);
                        }
                        else if (BattleRoyale.soloPlayer12 != null && soloPlayer.PlayerId == BattleRoyale.soloPlayer12.PlayerId) {
                            BattleRoyale.soloPlayerTeam.Remove(BattleRoyale.soloPlayer12);
                        }
                        else if (BattleRoyale.soloPlayer13 != null && soloPlayer.PlayerId == BattleRoyale.soloPlayer13.PlayerId) {
                            BattleRoyale.soloPlayerTeam.Remove(BattleRoyale.soloPlayer13);
                        }
                        else if (BattleRoyale.soloPlayer14 != null && soloPlayer.PlayerId == BattleRoyale.soloPlayer14.PlayerId) {
                            BattleRoyale.soloPlayerTeam.Remove(BattleRoyale.soloPlayer14);
                        }
                        else if (BattleRoyale.soloPlayer15 != null && soloPlayer.PlayerId == BattleRoyale.soloPlayer15.PlayerId) {
                            BattleRoyale.soloPlayerTeam.Remove(BattleRoyale.soloPlayer15);
                        }

                        int soloPlayersAlives = 0;

                        foreach (PlayerControl remainPlayer in BattleRoyale.soloPlayerTeam) {

                            if (!remainPlayer.Data.IsDead) {
                                soloPlayersAlives += 1;
                            }

                        }

                        BattleRoyale.battleRoyalepointCounter = "Battle Royale Fighters: " + "<color=#009F57FF>" + soloPlayersAlives + "</color>";

                        if (soloPlayersAlives <= 1) {
                            BattleRoyale.triggerSoloWin = true;
                            ShipStatus.RpcEndGame((GameOverReason)CustomGameOverReason.BattleRoyaleSoloWin, false);
                        }
                        break;
                    }
                }
            }
            else {
                // lime Team disconnects
                foreach (PlayerControl limePlayer in BattleRoyale.limeTeam) {
                    if (limePlayer.Data.Disconnected) {

                        if (BattleRoyale.limePlayer01 != null && limePlayer.PlayerId == BattleRoyale.limePlayer01.PlayerId) {
                            BattleRoyale.limeTeam.Remove(BattleRoyale.limePlayer01);
                        }
                        else if (BattleRoyale.limePlayer02 != null && limePlayer.PlayerId == BattleRoyale.limePlayer02.PlayerId) {
                            BattleRoyale.limeTeam.Remove(BattleRoyale.limePlayer02);
                        }
                        else if (BattleRoyale.limePlayer03 != null && limePlayer.PlayerId == BattleRoyale.limePlayer03.PlayerId) {
                            BattleRoyale.limeTeam.Remove(BattleRoyale.limePlayer03);
                        }
                        else if (BattleRoyale.limePlayer04 != null && limePlayer.PlayerId == BattleRoyale.limePlayer04.PlayerId) {
                            BattleRoyale.limeTeam.Remove(BattleRoyale.limePlayer04);
                        }
                        else if (BattleRoyale.limePlayer05 != null && limePlayer.PlayerId == BattleRoyale.limePlayer05.PlayerId) {
                            BattleRoyale.limeTeam.Remove(BattleRoyale.limePlayer05);
                        }
                        else if (BattleRoyale.limePlayer06 != null && limePlayer.PlayerId == BattleRoyale.limePlayer06.PlayerId) {
                            BattleRoyale.limeTeam.Remove(BattleRoyale.limePlayer06);
                        }
                        else if (BattleRoyale.limePlayer07 != null && limePlayer.PlayerId == BattleRoyale.limePlayer07.PlayerId) {
                            BattleRoyale.limeTeam.Remove(BattleRoyale.limePlayer07);
                        }

                        int limePlayersAlive = 0;

                        foreach (PlayerControl remainingLimePlayer in BattleRoyale.limeTeam) {

                            if (!remainingLimePlayer.Data.IsDead) {
                                limePlayersAlive += 1;
                            }

                        }

                        int pinkPlayersAlive = 0;

                        foreach (PlayerControl remainingPinkPlayer in BattleRoyale.pinkTeam) {

                            if (!remainingPinkPlayer.Data.IsDead) {
                                pinkPlayersAlive += 1;
                            }

                        }

                        if (BattleRoyale.serialKiller != null) {

                            int serialKillerAlive = 0;

                            foreach (PlayerControl serialKiller in BattleRoyale.serialKillerTeam) {

                                if (!serialKiller.Data.IsDead) {
                                    serialKillerAlive += 1;
                                }

                            }

                            if (BattleRoyale.matchType == 1) {
                                BattleRoyale.battleRoyalepointCounter = "Lime Team: " + "<color=#39FF14FF>" + limePlayersAlive + "</color> | " + "Pink Team: " + "<color=#F2BEFFFF>" + pinkPlayersAlive + "</color> | " + "Serial Killer: " + "<color=#808080FF>" + serialKillerAlive + "</color>";
                                if (limePlayersAlive <= 0 && pinkPlayersAlive <= 0 && !BattleRoyale.serialKiller.Data.IsDead) {
                                    BattleRoyale.triggerSerialKillerWin = true;
                                    ShipStatus.RpcEndGame((GameOverReason)CustomGameOverReason.BattleRoyaleSerialKillerWin, false);
                                }
                                else if (pinkPlayersAlive <= 0 && BattleRoyale.serialKiller.Data.IsDead) {
                                    BattleRoyale.triggerLimeTeamWin = true;
                                    ShipStatus.RpcEndGame((GameOverReason)CustomGameOverReason.BattleRoyaleLimeTeamWin, false);
                                }
                                else if (limePlayersAlive <= 0 && BattleRoyale.serialKiller.Data.IsDead) {
                                    BattleRoyale.triggerPinkTeamWin = true;
                                    ShipStatus.RpcEndGame((GameOverReason)CustomGameOverReason.BattleRoyalePinkTeamWin, false);
                                }
                            }                           
                        }
                        else {
                            if (BattleRoyale.matchType == 1) {
                                BattleRoyale.battleRoyalepointCounter = "Lime Team: " + "<color=#39FF14FF>" + limePlayersAlive + "</color> | " + "Pink Team: " + "<color=#F2BEFFFF>" + pinkPlayersAlive + "</color>";
                                if (pinkPlayersAlive <= 0) {
                                    BattleRoyale.triggerLimeTeamWin = true;
                                    ShipStatus.RpcEndGame((GameOverReason)CustomGameOverReason.BattleRoyaleLimeTeamWin, false);
                                }
                                else if (limePlayersAlive <= 0) {
                                    BattleRoyale.triggerPinkTeamWin = true;
                                    ShipStatus.RpcEndGame((GameOverReason)CustomGameOverReason.BattleRoyalePinkTeamWin, false);
                                }
                            }
                        }
                        break;
                    }
                }
                // Pink Team disconnects
                foreach (PlayerControl pinkPlayer in BattleRoyale.pinkTeam) {
                    if (pinkPlayer.Data.Disconnected) {

                        if (BattleRoyale.pinkPlayer01 != null && pinkPlayer.PlayerId == BattleRoyale.pinkPlayer01.PlayerId) {
                            BattleRoyale.pinkTeam.Remove(BattleRoyale.pinkPlayer01);
                        }
                        else if (BattleRoyale.pinkPlayer02 != null && pinkPlayer.PlayerId == BattleRoyale.pinkPlayer02.PlayerId) {
                            BattleRoyale.pinkTeam.Remove(BattleRoyale.pinkPlayer02);
                        }
                        else if (BattleRoyale.pinkPlayer03 != null && pinkPlayer.PlayerId == BattleRoyale.pinkPlayer03.PlayerId) {
                            BattleRoyale.pinkTeam.Remove(BattleRoyale.pinkPlayer03);
                        }
                        else if (BattleRoyale.pinkPlayer04 != null && pinkPlayer.PlayerId == BattleRoyale.pinkPlayer04.PlayerId) {
                            BattleRoyale.pinkTeam.Remove(BattleRoyale.pinkPlayer04);
                        }
                        else if (BattleRoyale.pinkPlayer05 != null && pinkPlayer.PlayerId == BattleRoyale.pinkPlayer05.PlayerId) {
                            BattleRoyale.pinkTeam.Remove(BattleRoyale.pinkPlayer05);
                        }
                        else if (BattleRoyale.pinkPlayer06 != null && pinkPlayer.PlayerId == BattleRoyale.pinkPlayer06.PlayerId) {
                            BattleRoyale.pinkTeam.Remove(BattleRoyale.pinkPlayer06);
                        }
                        else if (BattleRoyale.pinkPlayer07 != null && pinkPlayer.PlayerId == BattleRoyale.pinkPlayer07.PlayerId) {
                            BattleRoyale.pinkTeam.Remove(BattleRoyale.pinkPlayer07);
                        }

                        int limePlayersAlive = 0;

                        foreach (PlayerControl remainingLimePlayer in BattleRoyale.limeTeam) {

                            if (!remainingLimePlayer.Data.IsDead) {
                                limePlayersAlive += 1;
                            }

                        }

                        int pinkPlayersAlive = 0;

                        foreach (PlayerControl remainingPinkPlayer in BattleRoyale.pinkTeam) {

                            if (!remainingPinkPlayer.Data.IsDead) {
                                pinkPlayersAlive += 1;
                            }

                        }

                        if (BattleRoyale.serialKiller != null) {

                            int serialKillerAlive = 0;

                            foreach (PlayerControl serialKiller in BattleRoyale.serialKillerTeam) {

                                if (!serialKiller.Data.IsDead) {
                                    serialKillerAlive += 1;
                                }

                            }

                            if (BattleRoyale.matchType == 1) {
                                BattleRoyale.battleRoyalepointCounter = "Lime Team: " + "<color=#39FF14FF>" + limePlayersAlive + "</color> | " + "Pink Team: " + "<color=#F2BEFFFF>" + pinkPlayersAlive + "</color> | " + "Serial Killer: " + "<color=#808080FF>" + serialKillerAlive + "</color>";
                                if (limePlayersAlive <= 0 && pinkPlayersAlive <= 0 && !BattleRoyale.serialKiller.Data.IsDead) {
                                    BattleRoyale.triggerSerialKillerWin = true;
                                    ShipStatus.RpcEndGame((GameOverReason)CustomGameOverReason.BattleRoyaleSerialKillerWin, false);
                                }
                                else if (pinkPlayersAlive <= 0 && BattleRoyale.serialKiller.Data.IsDead) {
                                    BattleRoyale.triggerLimeTeamWin = true;
                                    ShipStatus.RpcEndGame((GameOverReason)CustomGameOverReason.BattleRoyaleLimeTeamWin, false);
                                }
                                else if (limePlayersAlive <= 0 && BattleRoyale.serialKiller.Data.IsDead) {
                                    BattleRoyale.triggerPinkTeamWin = true;
                                    ShipStatus.RpcEndGame((GameOverReason)CustomGameOverReason.BattleRoyalePinkTeamWin, false);
                                }
                            }
                        }
                        else {
                            if (BattleRoyale.matchType == 1) {
                                BattleRoyale.battleRoyalepointCounter = "Lime Team: " + "<color=#39FF14FF>" + limePlayersAlive + "</color> | " + "Pink Team: " + "<color=#F2BEFFFF>" + pinkPlayersAlive + "</color>";
                                if (pinkPlayersAlive <= 0) {
                                    BattleRoyale.triggerLimeTeamWin = true;
                                    ShipStatus.RpcEndGame((GameOverReason)CustomGameOverReason.BattleRoyaleLimeTeamWin, false);
                                }
                                else if (limePlayersAlive <= 0) {
                                    BattleRoyale.triggerPinkTeamWin = true;
                                    ShipStatus.RpcEndGame((GameOverReason)CustomGameOverReason.BattleRoyalePinkTeamWin, false);
                                }
                            }
                        }
                        break;
                    }
                }
                // Serial Killer disconnects
                if (BattleRoyale.serialKiller != null && BattleRoyale.serialKiller.Data.Disconnected) {

                    BattleRoyale.serialKillerTeam.Remove(BattleRoyale.serialKiller);

                    int limePlayersAlive = 0;

                    foreach (PlayerControl limePlayer in BattleRoyale.limeTeam) {

                        if (!limePlayer.Data.IsDead) {
                            limePlayersAlive += 1;
                        }

                    }

                    int pinkPlayersAlive = 0;

                    foreach (PlayerControl pinkPlayer in BattleRoyale.pinkTeam) {

                        if (!pinkPlayer.Data.IsDead) {
                            pinkPlayersAlive += 1;
                        }

                    }

                    int serialKillerAlive = 0;

                    if (BattleRoyale.matchType == 1) {
                        BattleRoyale.battleRoyalepointCounter = "Lime Team: " + "<color=#39FF14FF>" + limePlayersAlive + "</color> | " + "Pink Team: " + "<color=#F2BEFFFF>" + pinkPlayersAlive + "</color> | " + "Serial Killer: " + "<color=#808080FF>" + serialKillerAlive + "</color>";
                        if (pinkPlayersAlive <= 0) {
                            BattleRoyale.triggerLimeTeamWin = true;
                            ShipStatus.RpcEndGame((GameOverReason)CustomGameOverReason.BattleRoyaleLimeTeamWin, false);
                        }
                        else if (limePlayersAlive <= 0) {
                            BattleRoyale.triggerPinkTeamWin = true;
                            ShipStatus.RpcEndGame((GameOverReason)CustomGameOverReason.BattleRoyalePinkTeamWin, false);
                        }
                    }

                }
            }
        }
        
        
        static void UpdateMiniMap() {

            if (MapBehaviour.Instance != null && MapBehaviour.Instance.IsOpen && howmanygamemodesareon == 1) {
                switch (PlayerControl.GameOptions.MapId) {
                    case 0:
                        GameObject minimapSabotageSkeld = GameObject.Find("Main Camera/Hud/ShipMap(Clone)/InfectedOverlay");
                        minimapSabotageSkeld.SetActive(false);
                        if (activatedSensei && !updatedSenseiMinimap) {
                            GameObject mymap = GameObject.Find("Main Camera/Hud/ShipMap(Clone)/Background");
                            mymap.GetComponent<SpriteRenderer>().sprite = CustomMain.customAssets.customMinimap.GetComponent<SpriteRenderer>().sprite;
                            GameObject hereindicator = GameObject.Find("Main Camera/Hud/ShipMap(Clone)/HereIndicatorParent");
                            hereindicator.transform.position = hereindicator.transform.position + new Vector3(0.23f, -0.8f, 0);

                            // Map room names
                            GameObject minimapNames = GameObject.Find("Main Camera/Hud/ShipMap(Clone)/RoomNames (1)");
                            minimapNames.transform.GetChild(0).transform.position = minimapNames.transform.GetChild(0).transform.position + new Vector3(0f, -0.5f, 0); // Upper engine
                            minimapNames.transform.GetChild(2).transform.position = minimapNames.transform.GetChild(2).transform.position + new Vector3(0.7f, -0.55f, 0); // Reactor
                            minimapNames.transform.GetChild(3).transform.position = minimapNames.transform.GetChild(3).transform.position + new Vector3(1.75f, 2.37f, 0); // security
                            minimapNames.transform.GetChild(4).transform.position = minimapNames.transform.GetChild(4).transform.position + new Vector3(0.89f, -1.18f, 0); // medbey
                            minimapNames.transform.GetChild(5).transform.position = minimapNames.transform.GetChild(5).transform.position + new Vector3(0.52f, -1.32f, 0); // Cafetería
                            minimapNames.transform.GetChild(6).transform.position = minimapNames.transform.GetChild(6).transform.position + new Vector3(1f, -1.59f, 0); // weapons
                            minimapNames.transform.GetChild(7).transform.position = minimapNames.transform.GetChild(7).transform.position + new Vector3(-1.72f, -3.03f, 0); // nav
                            minimapNames.transform.GetChild(8).transform.position = minimapNames.transform.GetChild(8).transform.position + new Vector3(-0.08f, 1.45f, 0); // shields
                            minimapNames.transform.GetChild(9).transform.position = minimapNames.transform.GetChild(9).transform.position + new Vector3(1.1f, 2.88f, 0); // cooms
                            minimapNames.transform.GetChild(10).transform.position = minimapNames.transform.GetChild(10).transform.position + new Vector3(-2.2f, -0.82f, 0); // storage
                            minimapNames.transform.GetChild(11).transform.position = minimapNames.transform.GetChild(11).transform.position + new Vector3(0.32f, -1.02f, 0); // Admin
                            minimapNames.transform.GetChild(12).transform.position = minimapNames.transform.GetChild(12).transform.position + new Vector3(0.53f, -2.1f, 0); // electrical
                            minimapNames.transform.GetChild(13).transform.position = minimapNames.transform.GetChild(13).transform.position + new Vector3(-3.5f, -0.5f, 0); // o2

                            // Map sabotage
                            GameObject minimapSabotage = GameObject.Find("Main Camera/Hud/ShipMap(Clone)/InfectedOverlay");
                            minimapSabotage.transform.GetChild(0).gameObject.SetActive(false); // cafeteria doors
                            minimapSabotage.transform.GetChild(2).gameObject.SetActive(false); // medbey doors
                            minimapSabotage.transform.GetChild(3).transform.GetChild(0).gameObject.SetActive(false); // electrical doors
                            minimapSabotage.transform.GetChild(5).gameObject.SetActive(false); // upper engine doors
                            minimapSabotage.transform.GetChild(6).gameObject.SetActive(false); // lower engine doors
                            minimapSabotage.transform.GetChild(7).gameObject.SetActive(false); // storage doors
                            minimapSabotage.transform.GetChild(9).gameObject.SetActive(false); // security doors

                            minimapSabotage.transform.GetChild(1).transform.position = minimapSabotage.transform.GetChild(1).transform.position + new Vector3(0.95f, 3.3f, 0); // Sabotage cooms
                            minimapSabotage.transform.GetChild(3).transform.GetChild(1).transform.position = minimapSabotage.transform.GetChild(3).transform.GetChild(1).transform.position + new Vector3(0.165f, -1.2f, 0); // Sabotage electrical
                            minimapSabotage.transform.GetChild(4).transform.position = minimapSabotage.transform.GetChild(4).transform.position + new Vector3(-3f, 0.05f, 0); // Sabotage o2
                            minimapSabotage.transform.GetChild(8).transform.position = minimapSabotage.transform.GetChild(8).transform.position + new Vector3(0.6f, 0.1f, 0); // Sabotage reactor


                            updatedSenseiMinimap = true;
                        }
                        break;
                    case 1:
                        GameObject minimapSabotageMira = GameObject.Find("Main Camera/Hud/HqMap(Clone)/InfectedOverlay");
                        minimapSabotageMira.SetActive(false);
                        break;
                    case 2:
                        GameObject minimapSabotagePolus = GameObject.Find("Main Camera/Hud/PbMap(Clone)/InfectedOverlay");
                        minimapSabotagePolus.SetActive(false);
                        break;
                    case 3:
                        GameObject minimapSabotageDleks = GameObject.Find("Main Camera/Hud/ShipMap(Clone)/InfectedOverlay");
                        minimapSabotageDleks.SetActive(false);
                        break;
                    case 4:
                        GameObject minimapSabotageAirship = GameObject.Find("Main Camera/Hud/AirshipMap(Clone)/InfectedOverlay");
                        minimapSabotageAirship.SetActive(false);
                        break;
                    case 5:
                        GameObject minimapSabotageSubmerged = GameObject.Find("Main Camera/Hud/HudMapPrefab(Clone)(Clone)/MapHud/InfectedOverlay");
                        minimapSabotageSubmerged.SetActive(false);
                        break;
                }
            }
            else if (MapBehaviour.Instance != null && MapBehaviour.Instance.IsOpen && PlayerControl.GameOptions.MapId == 0 && activatedSensei && !updatedSenseiMinimap && howmanygamemodesareon != 1) {
                GameObject mymap = GameObject.Find("Main Camera/Hud/ShipMap(Clone)/Background");
                mymap.GetComponent<SpriteRenderer>().sprite = CustomMain.customAssets.customMinimap.GetComponent<SpriteRenderer>().sprite;
                GameObject hereindicator = GameObject.Find("Main Camera/Hud/ShipMap(Clone)/HereIndicatorParent");
                hereindicator.transform.position = hereindicator.transform.position + new Vector3(0.23f, -0.8f, 0);

                // Map room names
                GameObject minimapNames = GameObject.Find("Main Camera/Hud/ShipMap(Clone)/RoomNames (1)");
                minimapNames.transform.GetChild(0).transform.position = minimapNames.transform.GetChild(0).transform.position + new Vector3(0f, -0.5f, 0); // upper engine
                minimapNames.transform.GetChild(2).transform.position = minimapNames.transform.GetChild(2).transform.position + new Vector3(0.7f, -0.55f, 0); // Reactor
                minimapNames.transform.GetChild(3).transform.position = minimapNames.transform.GetChild(3).transform.position + new Vector3(1.75f, 2.37f, 0); // security
                minimapNames.transform.GetChild(4).transform.position = minimapNames.transform.GetChild(4).transform.position + new Vector3(0.89f, -1.18f, 0); // medbey
                minimapNames.transform.GetChild(5).transform.position = minimapNames.transform.GetChild(5).transform.position + new Vector3(0.52f, -1.32f, 0); // Cafetería
                minimapNames.transform.GetChild(6).transform.position = minimapNames.transform.GetChild(6).transform.position + new Vector3(1f, -1.59f, 0); // weapons
                minimapNames.transform.GetChild(7).transform.position = minimapNames.transform.GetChild(7).transform.position + new Vector3(-1.72f, -3.03f, 0); // nav
                minimapNames.transform.GetChild(8).transform.position = minimapNames.transform.GetChild(8).transform.position + new Vector3(-0.08f, 1.45f, 0); // shields
                minimapNames.transform.GetChild(9).transform.position = minimapNames.transform.GetChild(9).transform.position + new Vector3(1.1f, 2.88f, 0); // cooms
                minimapNames.transform.GetChild(10).transform.position = minimapNames.transform.GetChild(10).transform.position + new Vector3(-2.2f, -0.82f, 0); // storage
                minimapNames.transform.GetChild(11).transform.position = minimapNames.transform.GetChild(11).transform.position + new Vector3(0.32f, -1.02f, 0); // Admin
                minimapNames.transform.GetChild(12).transform.position = minimapNames.transform.GetChild(12).transform.position + new Vector3(0.53f, -2.1f, 0); // elec
                minimapNames.transform.GetChild(13).transform.position = minimapNames.transform.GetChild(13).transform.position + new Vector3(-3.5f, -0.5f, 0); // o2

                // Map sabotage
                GameObject minimapSabotage = GameObject.Find("Main Camera/Hud/ShipMap(Clone)/InfectedOverlay");
                minimapSabotage.transform.GetChild(0).gameObject.SetActive(false); // cafeteria doors
                minimapSabotage.transform.GetChild(2).gameObject.SetActive(false); // medbey doors
                minimapSabotage.transform.GetChild(3).transform.GetChild(0).gameObject.SetActive(false); // Puertas electricidad
                minimapSabotage.transform.GetChild(5).gameObject.SetActive(false); // upper engine doors
                minimapSabotage.transform.GetChild(6).gameObject.SetActive(false); // lower engine doors
                minimapSabotage.transform.GetChild(7).gameObject.SetActive(false); // storage doors
                minimapSabotage.transform.GetChild(9).gameObject.SetActive(false); // security doors

                minimapSabotage.transform.GetChild(1).transform.position = minimapSabotage.transform.GetChild(1).transform.position + new Vector3(0.95f, 3.3f, 0); // Sabotage cooms
                minimapSabotage.transform.GetChild(3).transform.GetChild(1).transform.position = minimapSabotage.transform.GetChild(3).transform.GetChild(1).transform.position + new Vector3(0.165f, -1.2f, 0); // Sabotage elec
                minimapSabotage.transform.GetChild(4).transform.position = minimapSabotage.transform.GetChild(4).transform.position + new Vector3(-3f, 0.05f, 0); // Sabotage o2
                minimapSabotage.transform.GetChild(8).transform.position = minimapSabotage.transform.GetChild(8).transform.position + new Vector3(0.6f, 0.1f, 0); // Sabotage reactor


                updatedSenseiMinimap = true;
            }

            // If bomb, lights actives or special 1vs1 condition, prevent sabotage open map
            if (howmanygamemodesareon != 1 && PlayerControl.LocalPlayer.Data.Role.IsImpostor && MapBehaviour.Instance != null && MapBehaviour.Instance.IsOpen && (alivePlayers <= 2 || Bomberman.activeBomb || Challenger.isDueling || Illusionist.lightsOutTimer > 0)) {
                MapBehaviour.Instance.Close();
            }
        }

        static void updateReportButton(HudManager __instance) {
            if (howmanygamemodesareon != 1) {
                if (!activatedReportButtonAfterCustomMode) {
                    __instance.ReportButton.gameObject.SetActive(true);
                    __instance.ReportButton.graphic.enabled = true;
                    __instance.ReportButton.enabled = true;
                    activatedReportButtonAfterCustomMode = true;
                }
                return;
            }

            bool enabled = true;
            if (howmanygamemodesareon == 1)
                enabled = false;
            enabled &= __instance.ReportButton.isActiveAndEnabled;

            __instance.ReportButton.gameObject.SetActive(enabled);
            __instance.ReportButton.graphic.enabled = enabled;
            __instance.ReportButton.enabled = enabled;
        }

        static void Postfix(HudManager __instance) {
            if (AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started) return;

            CustomButton.HudUpdate();
            resetNameTagsAndColors();
            setNameColors();
            setNameTags();
            UpdateMiniMap();

            // Impostors
            updateImpostorKillButton(__instance);

            // Timer updates
            timerUpdate();

            // Custom gamemode report button update
            updateReportButton(__instance);


            // Better Sabotages
            shakeScreenIfReactorSabotage();
            anonymousCommsSabotage();
            slowSpeedIfOxigenSabotage();

            // Squire update
            updateShielded();

            // Kid
            kidUpdate();

            // FortuneTeller update
            fortuneTellerUpdate();

            // Spiritualist update
            spiritualistUpdate();

            // Chameleon update
            chameleonUpdate();

            //BountyHunter update
            bountyHunterSuicideIfDisconnect(); 
            
            // Yinyanger update
            yinyangerUpdate();

            // Challenger update
            challengerUpdate();

            // VigilantMira update
            vigilantMiraUpdate();

            // Bat update
            batUpdate();

            // Janitor corpse moving
            janitorUpdate();

            // Necromancer corpse moving
            necromancerUpdate();


            // Capture the flag flags movement + fix if someone disconnnects
            captureTheFlagUpdate();

            // Police and thief jewel restore values if someone disconnnects
            policeandthiefUpdate();

            // King of the hill point time count
            kingOfTheHillUpdate();

            // Hot Potato disconnect update
            hotPotatoUpdate();

            // ZombieLaboratory disconnect update
            zombieLaboratoryUpdate();

            // Battle Royale disconnect update
            battleRoyaleUpdate();

        }
    }
}