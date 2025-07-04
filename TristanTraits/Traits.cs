﻿using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using Obeliskial_Content;
using UnityEngine;
using static Tristan.CustomFunctions;
using static Tristan.Plugin;
using static Tristan.DescriptionFunctions;
using static Tristan.CharacterFunctions;
using System.Text;
using TMPro;
using Obeliskial_Essentials;
using System.Data.Common;

namespace Tristan
{
    [HarmonyPatch]
    internal class Traits
    {
        // list of your trait IDs

        public static string[] simpleTraitList = ["trait0", "trait1a", "trait1b", "trait2a", "trait2b", "trait3a", "trait3b", "trait4a", "trait4b"];

        public static string[] myTraitList = simpleTraitList.Select(trait => subclassname.ToLower() + trait).ToArray(); // Needs testing

        public static string trait0 = myTraitList[0];
        // static string trait1b = myTraitList[1];
        public static string trait2a = myTraitList[3];
        public static string trait2b = myTraitList[4];
        public static string trait4a = myTraitList[7];
        public static string trait4b = myTraitList[8];

        // public static int infiniteProctection = 0;
        // public static int bleedInfiniteProtection = 0;
        public static bool isDamagePreviewActive = false;

        public static bool isCalculateDamageActive = false;
        public static int infiniteProctection = 0;

        public static string debugBase = "Binbin - Testing " + heroName + " ";


        public static void DoCustomTrait(string _trait, ref Trait __instance)
        {
            // get info you may need
            Enums.EventActivation _theEvent = Traverse.Create(__instance).Field("theEvent").GetValue<Enums.EventActivation>();
            Character _character = Traverse.Create(__instance).Field("character").GetValue<Character>();
            Character _target = Traverse.Create(__instance).Field("target").GetValue<Character>();
            int _auxInt = Traverse.Create(__instance).Field("auxInt").GetValue<int>();
            string _auxString = Traverse.Create(__instance).Field("auxString").GetValue<string>();
            CardData _castedCard = Traverse.Create(__instance).Field("castedCard").GetValue<CardData>();
            Traverse.Create(__instance).Field("character").SetValue(_character);
            Traverse.Create(__instance).Field("target").SetValue(_target);
            Traverse.Create(__instance).Field("theEvent").SetValue(_theEvent);
            Traverse.Create(__instance).Field("auxInt").SetValue(_auxInt);
            Traverse.Create(__instance).Field("auxString").SetValue(_auxString);
            Traverse.Create(__instance).Field("castedCard").SetValue(_castedCard);
            TraitData traitData = Globals.Instance.GetTraitData(_trait);
            List<CardData> cardDataList = [];
            List<string> heroHand = MatchManager.Instance.GetHeroHand(_character.HeroIndex);
            Hero[] teamHero = MatchManager.Instance.GetTeamHero();
            NPC[] teamNpc = MatchManager.Instance.GetTeamNPC();

            if (!IsLivingHero(_character))
            {
                return;
            }

            if (_trait == trait0)
            {
                // Fortify on you increases Mind Damage by 1 per charge and stacks to 50.
                // Done in GACM
            }


            else if (_trait == trait2a)
            {
                // trait2a
                // Mental Fortitude applies to all heroes. 
                // When you apply Insane, apply 2 Block to All Heroes. This Block does not benefit from bonuses
                string traitName = traitData.TraitName;
                string traitId = _trait;

                LogDebug($"Handling Trait {traitId}: {traitName}");
                if (_auxString == "insane")
                {
                    int amount = _character.HaveTrait(trait4a) ? 2 * _auxInt : 2;
                    ApplyAuraCurseToAll("block", amount, AppliesTo.Heroes, _character);
                }



            }



            else if (_trait == trait2b)
            {
                // trait2b:
                // At the start of your turn, reduce the cost of your highest cost Attack by 2. 
                // Repeat for Defense, Mind Spell, and Healing Spell.
                string traitName = traitData.TraitName;
                string traitId = _trait;
                Enums.CardType[] cardTypes = [Enums.CardType.Attack, Enums.CardType.Defense, Enums.CardType.Mind_Spell, Enums.CardType.Healing_Spell];
                foreach (Enums.CardType cardType in cardTypes)
                {
                    CardData highestCostCard = GetRandomHighestCostCard(cardType, heroHand);
                    if (highestCostCard == null)
                    {
                        continue;
                    }
                    // int energy = highestCostCard.EnergyCost - highestCostCard.EnergyReductionPermanent - highestCostCard.EnergyReductionTemporal;
                    // LogDebug($"Highest cost card: {highestCostCard.CardName} with energy {energy}, cost {highestCostCard.EnergyCost}, reduction {highestCostCard.EnergyReductionPermanent}, temporal reduction {highestCostCard.EnergyReductionTemporal}");
                    if (highestCostCard != null && IsLivingHero(_character)) //energy >= 6 && 
                    {
                        int amountToReduce = _character.HaveTrait(trait4a) ? 3 : 2;
                        ReduceCardCost(ref highestCostCard, _character, amountToReduce);
                    }
                }
            }

            else if (_trait == trait4a)
            {
                // trait 4a;
                // Feathered Shell applies 2 Block per Insane change applied. Bird of all Trades reduces cost by 3.
                string traitName = traitData.TraitName;
                string traitId = _trait;

                LogDebug($"Handling Trait {traitId}: {traitName}");
            }

            else if (_trait == trait4b)
            {
                // trait 4b:
                // Once per turn, when you play a Mind Spell, add a randomly upgraded Pandemonium to your hand (Costs 0 and Vanish). 
                string traitName = traitData.TraitName;
                string traitId = _trait;
                if (CanIncrementTraitActivations(traitId) && _castedCard.HasCardType(Enums.CardType.Mind_Spell))
                {
                    LogDebug($"Handling Trait {traitId}: {traitName}");
                    AddCardToHand("pandemonium");
                    IncrementTraitActivations(traitId);
                }
            }

        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Trait), "DoTrait")]
        public static bool DoTrait(Enums.EventActivation _theEvent, string _trait, Character _character, Character _target, int _auxInt, string _auxString, CardData _castedCard, ref Trait __instance)
        {
            if ((UnityEngine.Object)MatchManager.Instance == (UnityEngine.Object)null)
                return false;
            Traverse.Create(__instance).Field("character").SetValue(_character);
            Traverse.Create(__instance).Field("target").SetValue(_target);
            Traverse.Create(__instance).Field("theEvent").SetValue(_theEvent);
            Traverse.Create(__instance).Field("auxInt").SetValue(_auxInt);
            Traverse.Create(__instance).Field("auxString").SetValue(_auxString);
            Traverse.Create(__instance).Field("castedCard").SetValue(_castedCard);
            if (Content.medsCustomTraitsSource.Contains(_trait) && myTraitList.Contains(_trait))
            {
                DoCustomTrait(_trait, ref __instance);
                return false;
            }
            return true;
        }



        [HarmonyPostfix]
        [HarmonyPatch(typeof(AtOManager), "GlobalAuraCurseModificationByTraitsAndItems")]
        // [HarmonyPriority(Priority.Last)]
        public static void GlobalAuraCurseModificationByTraitsAndItemsPostfix(ref AtOManager __instance, ref AuraCurseData __result, string _type, string _acId, Character _characterCaster, Character _characterTarget)
        {
            // LogInfo($"GACM {subclassName}");

            Character characterOfInterest = _type == "set" ? _characterTarget : _characterCaster;
            string traitOfInterest;
            switch (_acId)
            {
                // trait0:
                // Fortify on you increases Mind Damage by 1 per charge and stacks to 50.
                case "fortify":
                    traitOfInterest = trait0;
                    AppliesTo appliesTo = characterOfInterest.HaveTrait(trait2a) ? AppliesTo.Heroes : AppliesTo.ThisHero;
                    if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, appliesTo))
                    {
                        __result.GainCharges = true;
                        __result.MaxCharges = 50;
                        __result.MaxMadnessCharges = 50;
                        __result.AuraDamageType4 = Enums.DamageType.Mind;
                        __result.AuraDamageIncreasedPerStack4 = 1;
                    }
                    break;
                case "crack":
                    traitOfInterest = "owlknightcognitivecalm";
                    // AppliesTo appliesTo = characterOfInterest.HaveTrait(trait2a) ? AppliesTo.Heroes : AppliesTo.ThisHero;
                    if (IfCharacterHas(characterOfInterest, CharacterHas.Enchantment, traitOfInterest, AppliesTo.Monsters))
                    {
                        __result.AuraDamageType = Enums.DamageType.None;
                        __result.AuraDamageIncreasedPerStack = 0;
                        __result.AuraDamageType2 = Enums.DamageType.None;
                        __result.AuraDamageIncreasedPerStack2 = 0;
                        __result.AuraDamageType3 = Enums.DamageType.None;
                        __result.AuraDamageIncreasedPerStack3 = 0;
                        __result.AuraDamageType4 = Enums.DamageType.None;
                        __result.AuraDamageIncreasedPerStack4 = 0;
                        __result.HealAttackerPerStack = 1;
                        __result.HealAttackerConsumeCharges = 1;
                    }
                    break;
                case "insane":
                    traitOfInterest = trait0;
                    if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.Monsters))
                    {
                        __result.ResistModified2 = Enums.DamageType.Blunt;
                        __result.ResistModifiedPercentagePerStack2 = -0.3f;
                    }
                    break;
            }
        }




        [HarmonyPrefix]
        [HarmonyPatch(typeof(CharacterItem), nameof(CharacterItem.CalculateDamagePrePostForThisCharacter))]
        public static void CalculateDamagePrePostForThisCharacterPrefix()
        {
            isDamagePreviewActive = true;
        }
        [HarmonyPostfix]
        [HarmonyPatch(typeof(CharacterItem), nameof(CharacterItem.CalculateDamagePrePostForThisCharacter))]
        public static void CalculateDamagePrePostForThisCharacterPostfix()
        {
            isDamagePreviewActive = false;
        }


        [HarmonyPrefix]
        [HarmonyPatch(typeof(MatchManager), nameof(MatchManager.SetDamagePreview))]
        public static void SetDamagePreviewPrefix()
        {
            isDamagePreviewActive = true;
        }
        [HarmonyPostfix]
        [HarmonyPatch(typeof(MatchManager), nameof(MatchManager.SetDamagePreview))]
        public static void SetDamagePreviewPostfix()
        {
            isDamagePreviewActive = false;
        }





    }
}

